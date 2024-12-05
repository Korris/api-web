using Microsoft.EntityFrameworkCore;

namespace Mcsg.Document.Api.Services;

using Common.Core.Constants;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Domain;
using Common.Domain.Dtos;
using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Extensions;
using Dtos;
using Interfaces;
using Requests;
using static Common.Core.Constants.Setting;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// File service
/// </summary>
public class FileService : IFileService
{
    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="setting">Setting</param>
    /// <param name="sc">Storage client</param>
    /// <param name="jobService">Job service</param>
    public FileService(IMcsgContext context, ISetting setting, IStorageClient sc, IJobService jobService)
    {
        _context = context;
        _setting = setting;
        _sc = sc;
        _jobService = jobService;
    }

    /// <summary>
    /// UploadImage async
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Return the result</returns>
    /// <exception cref="NotFoundException">NotFoundException</exception>
    public async Task<UploadFileDto> UploadImageAsync(FileCreateR request)
    {
        if (request.File == null || request.File.Length == 0)
        {
            throw new NotFoundException(nameof(E201), E201);
        }
        if (!request.File.IsImageType())
        {
            throw new NotFoundException(nameof(E202), E202);
        }

        return await UploadFileAsync(request);
    }

    /// <summary>
    /// UploadFile async
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Return the result</returns>
    /// <exception cref="NotFoundException">NotFoundException</exception>
    public async Task<UploadFileDto> UploadFileAsync(FileCreateR request)
    {
        var file = request.File;
        if (file == null || file.Length == 0)
        {
            throw new NotFoundException(nameof(E201), E201);
        }

        var user = await _context.UserAvailable.FirstOrDefaultAsync(p => p.Id == request.UserId);
        if (user == null)
        {
            throw new NotFoundException(nameof(E303), E303);
        }

        // Upload to temp folder
        var hashId = Setting.ResourceConfig.HashLength.GetRandomString();
        var hashFileName = file.GetHashName(hashId);
        var fileTitle = file.FileName;
        var imgWidth = 0;
        var imgHeight = 0;
        var minioInstance = request.MinioInstance;
        var bucketName = _setting.GetMinio(minioInstance).BucketName;
        var objectName = "";
        var objectNameOriginal = "";
        var compressedSize = file.Length;

        var type = string.IsNullOrWhiteSpace(request.Type) ? "" : $"/{request.Type}".ToPlural();
        if (request.IsPublic == true)
        {
            bucketName = _sc.GetStrategy(minioInstance).BucketNamePublic;
            objectName = $"{Setting.MinioFolder.Document}/{user.UserFolder}{type}/{hashFileName}";

            if (request.Type == "Thumb")
            {
                objectNameOriginal = objectName.AppendNameSuffix();
            }
        }
        else
        {
            var tempBlobName = hashFileName.GetTempBlobName(user.UserFolder);
            objectName = $"{Setting.MinioFolder.Document}/{tempBlobName}";
        }

        if (file.IsImage() && !file.IsGifAnimated())
        {
            if (!string.IsNullOrWhiteSpace(objectNameOriginal))
            {
                // Compress and save thumbnail
                var compressedThumb = file.CompressAndConvertToJpeg(288, 432, 100);
                if (compressedThumb != null)
                {
                    using (var thumbStream = compressedThumb.Image.OpenReadStream())
                    {
                        await _sc.GetStrategy(minioInstance).PutObject(thumbStream, objectName, bucketName);
                    }
                }
            }
            else
            {
                objectNameOriginal = objectName;
            }

            var compressedImage = file.CompressAndConvertToJpeg(_setting.Minio.ImageDownQuality);
            imgWidth = compressedImage.Width;
            imgHeight = compressedImage.Height;

            using (var stream = compressedImage.Image.OpenReadStream())
            {
                compressedSize = stream.Length;
                await _sc.GetStrategy(minioInstance).PutObject(stream, objectNameOriginal, bucketName);
            }
        }
        else
        {
            // Get ratio when it's gif file.
            if (file.IsGif())
            {
                var ratio = file.GetRatio();
                imgHeight = ratio.Height;
                imgWidth = ratio.Width;
            }

            if (!file.IsDocument())
            {
                throw new BadRequestException(nameof(E210), E210);
            }

            using (var stream = file.OpenReadStream())
            {
                await _sc.GetStrategy(minioInstance).PutObject(stream, objectName, bucketName);
            }
        }

        // Insert to resource with type is temp
        var resource = new DocumentResource
        {
            AuthorId = request.UserId,
            HashId = hashId,
            Title = Path.GetFileNameWithoutExtension(fileTitle),
            Name = hashFileName,
            Url = objectName,
            BucketName = bucketName,
            Type = file.IsImageType() ? ResourceType.Image : ResourceType.Document,
            CreatedBy = request.UserId,
            Width = imgWidth,
            Height = imgHeight,
            Size = file.Length,
            CompressedSize = compressedSize,
            MinioInstance = minioInstance
        };

        await _context.DocumentResources.AddAsync(resource);
        await _context.SaveChangesAsync(default);

        var shareUrl = "";
        if (request.IsPublic == true)
        {
            shareUrl = _setting.GetMinio(minioInstance).GetPublicUrl(resource.BucketName, request.Type == PostResourceType.Thumb ? objectNameOriginal : objectName);
        }
        else
        {
            shareUrl = await _sc.GetPublicUrl(resource.Url, resource.BucketName, minioInstance);
        }

        return new UploadFileDto
        {
            HashId = hashId,
            Url = shareUrl,
            Width = imgWidth,
            Height = imgHeight,
            Type = resource.Type,
            Size = resource.Size
        };
    }

    /// <summary>
    /// ProcessFiles async
    /// </summary>
    /// <param name="dto">UploadResourceDto</param>
    /// <returns>Return the result</returns>
    public async Task<List<SubUploadFileDto>> ProcessFilesAsync(UploadResourceDto dto)
    {
        var subPosts = new List<SubUploadFileDto>();

        // Complete resource files
        var (resources, subPostResponses) = await CompleteFilesAsync(dto, true);

        foreach (var resource in resources)
        {
            var subPostHashId = subPostResponses.FirstOrDefault(p => p.Id == resource.SubPostId);
            var shareUrl = await _sc.GetPublicUrl(resource.Url, resource.BucketName, resource.MinioInstance);

            subPosts.Add(new SubUploadFileDto
            {
                Status = PostStatus.Public,
                Files = [
                    new UploadFileDto
                    {
                        SubPostHashId = subPostHashId?.HashId,
                        HashId = resource?.HashId,
                        Url = shareUrl,
                        Height = resource.Height,
                        Width = resource.Width,
                        Order = resource.Order,
                        Type = resource.Type,
                        Size = resource.Size
                    }],
                Title = resource.Title,
                Permission = PostPermission.Public,
                PublishDate = DateTime.UtcNow
            });
        }

        return subPosts;
    }

    /// <summary>
    /// ProcessDocumentFiles async
    /// </summary>
    /// <param name="dto">UploadResourceDto</param>
    /// <returns>Return the result</returns>
    public async Task<List<UploadFileDto>> ProcessDocumentFilesAsync(UploadResourceDto dto)
    {
        var files = new List<UploadFileDto>();

        // Complete resource files
        var (resources, subPostResponses) = await CompleteFilesAsync(dto, false);

        foreach (var resource in resources)
        {
            files.Add(new UploadFileDto()
            {
                HashId = resource.HashId,
                Order = resource.Order,
                Url = await _sc.GetCdnUrlAsync(resource.Url, resource.BucketName, resource.MinioInstance, resource.Type)
            });
        }

        return files;
    }

    /// <summary>
    /// UpdateFiles async
    /// </summary>
    /// <param name="dto">UploadResourceDto</param>
    /// <returns>Return the result</returns>
    /// <exception cref="NotFoundException">NotFoundException</exception>
    public async Task<List<SubUploadFileDto>> UpdateFilesAsync(UploadResourceDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.UserFolder))
        {
            throw new NotFoundException(nameof(E303), E303);
        }

        var resourcesDb = await QueryResourceByPostId(dto.PostId).ToArrayAsync();
        var subPostDB = await _context.DocumentSubPostAvailable.Where(p => p.PostId == dto.PostId).ToListAsync();

        //Update
        var resourceDbHashId = resourcesDb.Select(x => x.HashId).ToList();
        var resourceRequestHashId = dto.ResourcePosts.Select(x => x.HashId).ToList();

        var urDto = dto.Clone();
        urDto.ResourcePosts = dto.ResourcePosts.Where(x => !resourceDbHashId.Contains(x.HashId)).ToList();
        var listResourceAddded = resourcesDb.Where(x => resourceRequestHashId.Contains(x.HashId)).ToList();

        // Complete resource files
        var (listResourcesNew, subPostResponses) = await CompleteFilesAsync(urDto, true);

        //Remove
        var listRemove = resourcesDb.Where(x => !resourceRequestHashId.Contains(x.HashId)).ToList();
        var listRemoveHashId = listRemove.Select(x => x.HashId).ToList();

        if (listRemove.Any())
        {
            var subPostIds = listRemove.Select(x => x.SubPostId).ToList();
            await RemoveResource(listRemoveHashId, subPostIds);
        }

        // Check change order
        foreach (var resourceAdded in listResourceAddded)
        {
            var resourceReq = dto.ResourcePosts.FirstOrDefault(x => x.HashId == resourceAdded.HashId);
            var subPostByResource = subPostDB.FirstOrDefault(p => p.Id == resourceAdded.SubPostId);

            if (resourceReq != null && subPostByResource != null && ((resourceReq.Order != resourceAdded.Order) || subPostByResource.Body != resourceReq.Body))
            {
                resourceAdded.Order = resourceReq.Order;
                subPostByResource.Order = resourceReq.Order;
                subPostByResource.Body = resourceReq.Body;
            }
        }
        await _context.SaveChangesAsync(default);

        var resourcesResult = listResourceAddded.Concat(listResourcesNew);
        resourcesResult = resourcesResult.Where(x => !listRemoveHashId.Contains(x.HashId)).ToList();
        var subPosts = new List<SubUploadFileDto>();

        var subpostAndResourceHashId = await (from a in _context.DocumentSubPostAvailable
                                              join b in _context.DocumentResourceAvailable
                                                on a.Id equals b.SubPostId into g1
                                              from b in g1.DefaultIfEmpty()
                                              join c in _context.DocumentPosts
                                                on a.PostId equals c.Id into g2
                                              from c in g2.DefaultIfEmpty()
                                              where c.Id == dto.PostId
                                              select new
                                              {
                                                  SubPostHashId = a.HashId,
                                                  SubPostId = a.Id
                                              }).ToListAsync();

        foreach (var resource in resourcesResult.OrderBy(p => p.Order))
        {
            var shareUrl = await _sc.GetPublicUrl(resource.Url, resource.BucketName, resource.MinioInstance);
            var subPostData = subpostAndResourceHashId.FirstOrDefault(p => p.SubPostId == resource.SubPostId);
            subPosts.Add(new SubUploadFileDto
            {
                Body = dto.ResourcePosts.FirstOrDefault(p => p.Order == resource.Order).Body,
                HashId = subPostData?.SubPostHashId ?? "",
                Status = PostStatus.Public,
                Files = [
                    new UploadFileDto
                    {
                        SubPostHashId = subPostData?.SubPostHashId ?? "",
                        HashId = resource.HashId ,
                        Url = shareUrl,
                        Height = resource.Height,
                        Width = resource.Width,
                        Order = resource.Order,
                        Type = resource.Type,
                        Size = resource.Size
                    }],
                Title = resource.Title,
                Permission = PostPermission.Public,
                PublishDate = DateTime.UtcNow
            });
        }

        return subPosts;
    }

    /// <summary>
    /// RemoveFile async
    /// </summary>
    /// <param name="postId">PostId</param>
    /// <param name="userFolder">User folder</param>
    /// <returns>Return the result</returns>
    /// <exception cref="NotFoundException">NotFoundException</exception>
    public async Task RemoveFileAsync(Guid postId, string userFolder)
    {
        if (string.IsNullOrWhiteSpace(userFolder))
        {
            throw new NotFoundException(nameof(E303), E303);
        }

        var resourcesDb = await QueryResourceByPostId(postId).ToArrayAsync();
        if (resourcesDb.Any())
        {
            var hashIds = resourcesDb.Select(p => p.HashId).ToList();
            var subPostIds = resourcesDb.Select(p => p.SubPostId).ToList();
            await RemoveResource(hashIds, subPostIds);

            foreach (var resource in resourcesDb)
            {
                var tempBlobName = resource.Name.GetTempBlobName(userFolder);
                var targetBlobName = resource.Name.GetMediaBlobName(userFolder);

                tempBlobName = $"{Setting.MinioFolder.Document}/{tempBlobName}";
                var isExistTempFile = await _sc.GetStrategy(resource.MinioInstance).StatObject(tempBlobName, null);
                if (isExistTempFile != null)
                {
                    await _sc.GetStrategy(resource.MinioInstance).RemoveObject(tempBlobName, null);
                }

                targetBlobName = $"{Setting.MinioFolder.Document}/{targetBlobName}";
                var isExistTargetFile = await _sc.GetStrategy(resource.MinioInstance).StatObject(targetBlobName, null);
                if (isExistTargetFile != null)
                {
                    await _sc.GetStrategy(resource.MinioInstance).RemoveObject(targetBlobName, null);
                }
            }
        }
    }

    /// <summary>
    /// CompleteFiles async
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="addSubPost"></param>
    /// <returns></returns>
    /// <exception cref="NotFoundException"></exception>
    private async Task<Tuple<List<DocumentResource>, List<SubUploadFileDto>>> CompleteFilesAsync(UploadResourceDto dto, bool addSubPost)
    {
        var response = new List<DocumentResource>();
        var subPostResponses = new List<SubUploadFileDto>();

        if (string.IsNullOrWhiteSpace(dto.UserFolder))
        {
            throw new NotFoundException(nameof(E303), E303);
        }

        var hashIds = dto.ResourcePosts.Select(x => x.HashId).ToList();
        if (hashIds == null || hashIds.Count == 0)
        {
            return Tuple.Create(response, subPostResponses);
        }
        var resourceDb = await _context.DocumentResourceAvailable.Where(p => p.SubPostId == dto.SubPostId).ToListAsync();
        var resourceHashIdRemove = resourceDb.Where(p => !hashIds.Contains(p.HashId)).Select(p => p.HashId).ToList();
        var resourceList = await _context.DocumentResourceAvailable.Where(p => hashIds.Contains(p.HashId)).ToListAsync();
        if (resourceHashIdRemove.Count > 0)
        {
            await RemoveResource(resourceHashIdRemove, new List<Guid?>());
        }
        foreach (var resource in resourceList)
        {
            if (resource == null)
            {
                continue;
            }

            var resourceReq = dto.ResourcePosts.FirstOrDefault(x => x.HashId == resource.HashId);

            #region -- Copy file from temp target --
            var tempBlobName = resource.Name.GetTempBlobName(dto.UserFolder);
            var targetBlobName = resource.Name.GetMediaBlobName(dto.SubFolder);

            var tempObjectName = $"{Setting.MinioFolder.Document}/{tempBlobName}";
            var isExistTempFile = await _sc.GetStrategy(resource.MinioInstance).StatObject(tempObjectName, null);

            var targetObjectName = $"{Setting.MinioFolder.Document}/{targetBlobName}";
            var isExistTargetFile = await _sc.GetStrategy(resource.MinioInstance).StatObject(targetObjectName, null);

            if (isExistTempFile != null && isExistTargetFile == null)
            {
                await _sc.GetStrategy(resource.MinioInstance).CopyObject(tempObjectName, targetObjectName, null, null);

                resource.Size = isExistTempFile!.Size;
                await _sc.GetStrategy(resource.MinioInstance).RemoveObject(tempObjectName, null);
            }
            #endregion

            if (addSubPost)
            {
                var subPost = new DocumentSubPost
                {
                    Title = resource.Title,
                    PostId = dto.PostId,
                    UserId = dto.UserId,
                    Body = resourceReq.Body,
                    CreatedBy = resource.CreatedBy,
                    Status = PostStatus.Public,
                    Order = resourceReq.Order,
                    Permission = PostPermission.Public,
                    PublishDate = DateTime.UtcNow,
                    HashId = Setting.PostConfig.SubHashLength.GetRandomString(),
                    IsExclusive = false
                };

                await _context.DocumentSubPosts.AddAsync(subPost);

                resource.SubPost = subPost;

                subPostResponses.Add(new SubUploadFileDto { HashId = subPost.HashId, Id = subPost.Id });
            }
            else if (dto.SubPostId != null)
            {
                resource.SubPostId = dto.SubPostId;
            }

            resource.Type = resource.Name.GetResourceType();
            resource.Url = targetObjectName;
            resource.Order = resourceReq.Order;
            await _context.SaveChangesAsync(default);

            await _jobService.CreateConvertJob(resource, dto.UserName, dto.UserAvatar, targetObjectName);
            response.Add(resource);
        }

        response = response.OrderBy(x => x.Order).ToList();

        return Tuple.Create(response, subPostResponses);
    }

    /// <summary>
    /// Resource current in post
    /// </summary>
    /// <param name="postId">PostId</param>
    /// <returns>Return a query</returns>
    private IQueryable<DocumentResource> QueryResourceByPostId(Guid postId)
    {
        return from a in _context.DocumentResourceAvailable
               join b in _context.DocumentSubPosts
                  on a.SubPostId equals b.Id
               where a.Type != ResourceType.Temp && b.PostId == postId
               select a;
    }

    /// <summary>
    /// Remove resource
    /// </summary>
    /// <param name="hashIds"></param>
    /// <param name="subPostIds"></param>
    /// <returns>Return the result</returns>
    private async Task RemoveResource(List<string?> hashIds, List<Guid?>? subPostIds)
    {
        var willDelete = false;

        if (hashIds?.Count > 0)
        {
            var a = await _context.DocumentResourceAvailable.Where(p => hashIds.Contains(p.HashId)).ToListAsync();
            a.ForEach(p => p.IsDelete = true);

            willDelete = true;
        }

        if (subPostIds?.Count > 0)
        {
            var b = await _context.DocumentSubPostAvailable.Where(p => subPostIds.Contains(p.Id)).ToListAsync();
            b.ForEach(p => p.IsDelete = true);

            willDelete = true;
        }

        if (willDelete)
        {
            await _context.SaveChangesAsync(default);
        }
    }

    #region -- Fields --

    /// <summary>
    /// DB Context
    /// </summary>
    private readonly IMcsgContext _context;

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    /// <summary>
    /// Storage client
    /// </summary>
    private readonly IStorageClient _sc;

    /// <summary>
    /// Job service
    /// </summary>
    private readonly IJobService _jobService;

    #endregion
}
