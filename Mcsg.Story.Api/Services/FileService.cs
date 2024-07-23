using Microsoft.EntityFrameworkCore;

namespace Mcsg.Story.Api.Services;

using Common.Core.Constants;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Extensions;
using Dtos;
using Interfaces;
using static Common.SeedWork.Constants.Error;
using static Common.SeedWork.Constants.Message;

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
    public FileService(McsgContext context, ISetting setting, IStorageClient sc, IJobService jobService)
    {
        _context = context;
        _setting = setting;
        _sc = sc;
        _jobService = jobService;
    }

    /// <summary>
    /// UploadImage async
    /// </summary>
    /// <param name="file">File</param>
    /// <param name="userId">UserId</param>
    /// <returns>Return the result</returns>
    /// <exception cref="NotFoundException">NotFoundException</exception>
    public async Task<UploadFileDto> UploadImageAsync(IFormFile file, Guid? userId)
    {
        if (file == null || file.Length == 0)
        {
            throw new NotFoundException(E201, M201);
        }
        if (!file.IsImageType())
        {
            throw new NotFoundException(E202, M202);
        }

        return await UploadFileAsync(file, userId);
    }

    /// <summary>
    /// UploadFile async
    /// </summary>
    /// <param name="file">File</param>
    /// <param name="userId">UserId</param>
    /// <returns>Return the result</returns>
    /// <exception cref="NotFoundException">NotFoundException</exception>
    public async Task<UploadFileDto> UploadFileAsync(IFormFile file, Guid? userId)
    {
        if (file == null || file.Length == 0)
        {
            throw new NotFoundException(E201, M201);
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new NotFoundException(E203, M203);
        }

        // Upload to temp folder
        var hashId = Setting.ResourceConfig.HashLength.GetRandomString();
        var hashFileName = file.GetHashName(hashId);
        var tempBlobName = "";
        var objectName = "";
        var fileTitle = file.FileName;
        var imgWidth = 0;
        var imgHeight = 0;

        if (file.IsImage() && !file.IsGifAnimated())
        {
            hashFileName = hashFileName.ToJpg();
            fileTitle = fileTitle.ToJpg();
            tempBlobName = hashFileName.GetTempBlobName(user.UserFolder);

            var compressedImage = file.CompressAndConvertToJpeg(_setting.Minio.ImageDownQuality);
            imgWidth = compressedImage.Width;
            imgHeight = compressedImage.Height;
            using (var stream = compressedImage.Image.OpenReadStream())
            {
                objectName = $"{Setting.MinioFolder.Story}/{tempBlobName}";
                await _sc.Strategy.PutObject(stream, objectName, null);
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

            tempBlobName = hashFileName.GetTempBlobName(user.UserFolder);
            using (var stream = file.OpenReadStream())
            {
                objectName = $"{Setting.MinioFolder.Story}/{tempBlobName}";
                await _sc.Strategy.PutObject(stream, objectName, null);
            }
        }

        // Insert to resource with type is temp
        var resource = new StoryResource
        {
            AuthorId = userId,
            HashId = hashId,
            Title = Path.GetFileNameWithoutExtension(fileTitle),
            Name = hashFileName,
            Url = objectName,
            Type = file.IsImageType() ? ResourceType.Image : ResourceType.Video,
            CreatedBy = userId,
            Width = imgWidth,
            Height = imgHeight,
            Size = file.Length
        };

        await _context.StoryResources.AddAsync(resource);
        await _context.SaveChangesAsync();

        var shareUrl = await _sc.Strategy.PresignedGetObject(resource.Url, _setting.Minio.MaxExpiryInSeconds, null);

        return new UploadFileDto()
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
    /// ProcessFeedFiles async
    /// </summary>
    /// <param name="req">Request</param>
    /// <param name="userId">UserId</param>
    /// <param name="userFolder">User folder</param>
    /// <param name="userAvatar">User avatar</param>
    /// <param name="userName">UserName</param>
    /// <param name="postId">PostId</param>
    /// <param name="postHashId">PostHashId</param>
    /// <returns>Return the result</returns>
    public async Task<List<SubUploadFileDto>> ProcessFeedFilesAsync(List<ResourcePostDto> req, Guid userId, string userFolder, string userAvatar, string userName, Guid postId, string postHashId)
    {
        var subPosts = new List<SubUploadFileDto>();

        // Complete resource files
        var (resources, subPostResponses) = await CompleteFilesAsync(req, userId, userName, userFolder, userAvatar, postId, true);

        // Map to response for feed service
        foreach (var resource in resources)
        {
            var subPostHashId = subPostResponses.FirstOrDefault(p => p.Id == resource.SubPostId);
            var shareUrl = await _sc.Strategy.PresignedGetObject(resource.Url, _setting.Minio.MaxExpiryInSeconds, null);

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
    /// ProcessComicFiles async
    /// </summary>
    /// <param name="req">Request</param>
    /// <param name="userId">UserId</param>
    /// <param name="userFolder">User folder</param>
    /// <param name="userAvatar">User avatar</param>
    /// <param name="userName">UserName</param>
    /// <param name="subPostId">SubPostId</param>
    /// <returns>Return the result</returns>
    public async Task<List<UploadFileDto>> ProcessComicFilesAsync(List<ResourcePostDto> req, Guid userId, string userFolder, string userAvatar, string userName, Guid subPostId)
    {
        var files = new List<UploadFileDto>();

        // Complete resource files
        var (resources, subPostResponses) = await CompleteFilesAsync(req, userId, userName, userFolder, userAvatar, subPostId, false);

        // Map to response for comic service
        foreach (var resource in resources)
        {
            files.Add(new UploadFileDto()
            {
                HashId = resource.HashId,
                Order = resource.Order,
                Url = _setting.Minio.MediaApiUrl.GetMediaPath(resource.Name, resource.Url)
            });
        }

        return files;
    }

    /// <summary>
    /// UpdateFeedFiles async
    /// </summary>
    /// <param name="req">Request</param>
    /// <param name="userId">UserId</param>
    /// <param name="userFolder">User folder</param>
    /// <param name="userAvatar">User avatar</param>
    /// <param name="userName">UserName</param>
    /// <param name="postId">PostId</param>
    /// <param name="postHashId">PostHashId</param>
    /// <returns>Return the result</returns>
    /// <exception cref="NotFoundException">NotFoundException</exception>
    public async Task<List<SubUploadFileDto>> UpdateFeedFilesAsync(List<ResourcePostDto> req, Guid userId, string userFolder, string userAvatar, string userName, Guid postId, string postHashId)
    {
        if (string.IsNullOrWhiteSpace(userFolder))
        {
            throw new NotFoundException(E203, M203);
        }

        var resourcesDb = await QueryResourceByPostId(postId).ToArrayAsync();
        var subPostDB = await _context.StorySubPostAvailable.Where(p => p.PostId == postId).ToListAsync();

        //Update
        var resourceDbHashId = resourcesDb.Select(x => x.HashId).ToList();
        var resourceRequestHashId = req.Select(x => x.HashId).ToList();

        var listResourceNotAdd = req.Where(x => !resourceDbHashId.Contains(x.HashId)).ToList();
        var listResourceAddded = resourcesDb.Where(x => resourceRequestHashId.Contains(x.HashId)).ToList();

        // Complete resource files
        var (listResourcesNew, subPostResponses) = await CompleteFilesAsync(listResourceNotAdd, userId, userName, userFolder, userAvatar, postId, true);

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
            var resourceReq = req.FirstOrDefault(x => x.HashId == resourceAdded.HashId);
            var subPostByResource = subPostDB.FirstOrDefault(p => p.Id == resourceAdded.SubPostId);

            if (resourceReq != null && subPostByResource != null && ((resourceReq.Order != resourceAdded.Order) || subPostByResource.Body != resourceReq.Body))
            {
                resourceAdded.Order = resourceReq.Order;
                subPostByResource.Order = resourceReq.Order;
                subPostByResource.Body = resourceReq.Body;
            }
        }
        await _context.SaveChangesAsync();

        // Map to response for feed service
        var resourcesResult = listResourceAddded.Concat(listResourcesNew);
        resourcesResult = resourcesResult.Where(x => !listRemoveHashId.Contains(x.HashId)).ToList();
        var subPosts = new List<SubUploadFileDto>();

        var subpostAndResourceHashId = await (from a in _context.StorySubPostAvailable
                                              join b in _context.StoryResourceAvailable
                                                on a.Id equals b.SubPostId into g1
                                              from b in g1.DefaultIfEmpty()
                                              join c in _context.Posts
                                                on a.PostId equals c.Id into g2
                                              from c in g2.DefaultIfEmpty()
                                              where c.Id == postId
                                              select new
                                              {
                                                  SubPostHashId = a.HashId,
                                                  SubPostId = a.Id
                                              }).ToListAsync();

        foreach (var resource in resourcesResult.OrderBy(p => p.Order))
        {
            var shareUrl = await _sc.Strategy.PresignedGetObject(resource.Url, _setting.Minio.MaxExpiryInSeconds, null);
            var subPostData = subpostAndResourceHashId.FirstOrDefault(p => p.SubPostId == resource.SubPostId);
            subPosts.Add(new SubUploadFileDto
            {
                Body = req.FirstOrDefault(p => p.Order == resource.Order).Body,
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
            throw new NotFoundException(E203, M203);
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

                tempBlobName = $"{Setting.MinioFolder.Story}/{tempBlobName}";
                var isExistTempFile = await _sc.Strategy.StatObjectAsync(tempBlobName, null);
                if (isExistTempFile != null)
                {
                    await _sc.Strategy.RemoveObject(tempBlobName, null);
                }

                targetBlobName = $"{Setting.MinioFolder.Story}/{targetBlobName}";
                var isExistTargetFile = await _sc.Strategy.StatObjectAsync(targetBlobName, null);
                if (isExistTargetFile != null)
                {
                    await _sc.Strategy.RemoveObject(targetBlobName, null);
                }
            }
        }
    }

    /// <summary>
    /// CompleteFiles async
    /// </summary>
    /// <param name="req"></param>
    /// <param name="userId"></param>
    /// <param name="userName"></param>
    /// <param name="userFolder"></param>
    /// <param name="userAvatar"></param>
    /// <param name="postId"></param>
    /// <param name="addSubPost"></param>
    /// <returns></returns>
    /// <exception cref="NotFoundException"></exception>
    private async Task<Tuple<List<StoryResource>, List<SubUploadFileDto>>> CompleteFilesAsync(List<ResourcePostDto> req, Guid userId, string userName, string userFolder, string userAvatar, Guid postId, bool addSubPost)
    {
        var response = new List<StoryResource>();
        var subPostResponses = new List<SubUploadFileDto>();

        if (string.IsNullOrWhiteSpace(userFolder))
        {
            throw new NotFoundException(E203, M203);
        }

        var hashIds = req.Select(x => x.HashId).ToList();
        if (hashIds == null || hashIds.Count == 0)
        {
            return Tuple.Create(response, subPostResponses);
        }

        var subFolder = addSubPost ? "sub-posts" : "posts";
        subFolder = $"{userFolder}/{subFolder}/{postId}";

        var resourceList = await _context.StoryResourceAvailable.Where(p => hashIds.Contains(p.HashId)).ToListAsync();
        foreach (var resource in resourceList)
        {
            if (resource == null)
            {
                continue;
            }

            var resourceReq = req.FirstOrDefault(x => x.HashId == resource.HashId);

            #region -- Copy file from temp target --
            var tempBlobName = resource.Name.GetTempBlobName(userFolder);
            var targetBlobName = resource.Name.GetMediaBlobName(subFolder);

            var tempObjectName = $"{Setting.MinioFolder.Story}/{tempBlobName}";
            var isExistTempFile = await _sc.Strategy.StatObjectAsync(tempObjectName, null);

            var targetObjectName = $"{Setting.MinioFolder.Story}/{targetBlobName}";
            var isExistTargetFile = await _sc.Strategy.StatObjectAsync(targetObjectName, null);

            if (isExistTempFile != null && isExistTargetFile == null)
            {
                await _sc.Strategy.CopyObject(tempObjectName, targetObjectName, null, null);

                resource.Size = isExistTempFile!.Size;
                await _sc.Strategy.RemoveObject(tempObjectName, null);
            }
            #endregion

            var subPostId = resource.SubPostId ?? postId;
            if (addSubPost)
            {
                var subPost = new StorySubPost
                {
                    Title = resource.Title,
                    PostId = postId,
                    UserId = userId,
                    Body = resourceReq.Body,
                    CreatedBy = resource.CreatedBy,
                    Status = PostStatus.Public,
                    Order = resourceReq.Order,
                    Permission = PostPermission.Public,
                    PublishDate = DateTime.UtcNow,
                    HashId = Setting.PostConfig.SubHashLength.GetRandomString(),
                    IsExclusive = false
                };

                await _context.StorySubPosts.AddAsync(subPost);
                subPostId = subPost.Id;

                subPostResponses.Add(new SubUploadFileDto { HashId = subPost.HashId, Id = subPostId });
            }

            resource.Type = resource.Name.GetResourceType();
            resource.Url = targetObjectName;
            resource.SubPostId = subPostId;
            resource.Order = resourceReq.Order;
            await _context.SaveChangesAsync();

            await _jobService.CreateConvertJob(resource, userName, userAvatar, targetObjectName);
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
    private IQueryable<StoryResource> QueryResourceByPostId(Guid postId)
    {
        return from a in _context.StoryResourceAvailable
               join b in _context.StorySubPosts
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
            var a = await _context.StoryResourceAvailable.Where(p => hashIds.Contains(p.HashId)).ToListAsync();
            a.ForEach(p => p.IsDelete = true);

            willDelete = true;
        }

        if (subPostIds?.Count > 0)
        {
            var b = await _context.StorySubPostAvailable.Where(p => subPostIds.Contains(p.Id)).ToListAsync();
            b.ForEach(p => p.IsDelete = true);

            willDelete = true;
        }

        if (willDelete)
        {
            await _context.SaveChangesAsync();
        }
    }

    #region -- Fields --

    /// <summary>
    /// DB Context
    /// </summary>
    private readonly McsgContext _context;

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
