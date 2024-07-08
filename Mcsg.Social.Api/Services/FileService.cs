using Dapper;

namespace Mcsg.Social.Api.Services;

using Common.Core.Enums;
using Common.Core.Interfaces;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Extensions;
using Constants;
using Interfaces;
using Lib.Common.Constants;
using Lib.Common.Extensions;
using Lib.Common.Helpers;
using Lib.Common.Web.Security;
using Lib.Data;
using Lib.Data.Domain.Entities;
using Lib.Data.Enums;
using Lib.Data.Repositories;
using Lib.Data.Repositories.Interface;
using Models;
using Requests;

public partial class FileService : IFileService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IJobService _jobService;
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Resource> _resourceRepository;
    private readonly IRepository<SubPost> _subPostRepository;
    private readonly IConfiguration _configuration;

    public FileService(ICurrentUserService currentUserService
        , IUnitOfWork unitOfWork
        , IConfiguration configuration
        , IJobService jobService
        , McsgDbContext context
        , ISetting setting
        , IStorageClient sc
        )
    {
        _configuration = configuration;
        _currentUserService = currentUserService;
        _userRepository = unitOfWork.GetRepository<User>();
        _resourceRepository = unitOfWork.GetRepository<Resource>();
        _subPostRepository = unitOfWork.GetRepository<SubPost>();
        _jobService = jobService;
        _context = context;
        _setting = setting;
        _sc = sc;
    }

    public async Task<UploadFileResponse> UploadImageAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new NotFoundException(ApiErrorCode.NotFileUpload, ApiErrorMessage.NotFileUpload);
        }
        if (!file.IsImageType())
        {
            throw new NotFoundException(ApiErrorCode.OnlyImageFile, ApiErrorMessage.OnlyImageFile);
        }

        return await UploadFileAsync(file);
    }
    public async Task<UploadFileResponse> UploadFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new NotFoundException(ApiErrorCode.NotFileUpload, ApiErrorMessage.NotFileUpload);
        }

        var currentUser = await _currentUserService.GetCurrentUserAsync();
        var user = await _context.Users.FindAsync(currentUser.UserId);
        if (user == null)
        {
            throw new NotFoundException(ErrorCodes.NotExistedUser, ErrorMessage.AccountNotExist);
        }

        // Upload to temp folder
        string hashId = ResourcesDefinition.HashLength.GetRandomString();
        string hashFileName = file.GetHashName(hashId);
        string tempBlobName = "";
        string fileTitle = file.FileName;
        int imgWidth = 0, imgHeight = 0;
        var objectName = "";

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
                objectName = $"{BlobStorageDefinition.MediaContainer}/{tempBlobName}";
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
                objectName = $"{BlobStorageDefinition.MediaContainer}/{tempBlobName}";
                await _sc.Strategy.PutObject(stream, objectName, null);
            }
        }

        // Insert to resource with type is temp
        var resource = new Resource()
        {
            AuthorId = currentUser.UserId,
            HashId = hashId,
            Title = Path.GetFileNameWithoutExtension(fileTitle),
            Name = hashFileName,
            Url = tempBlobName.CreateMediaUrl(_setting.Minio.MediaEncryptKey),
            ShareUrl = objectName,
            Type = file.IsImageType() ? ResourceType.IMAGE : ResourceType.VIDEO,
            CreatedBy = currentUser.UserId,
            Width = imgWidth,
            Height = imgHeight,
            Size = file.Length
        };

        await _resourceRepository.InsertAsync(resource);

        var shareUrl = await _sc.Strategy.PresignedGetObject(resource.ShareUrl, _setting.Minio.MaxExpiryInSeconds, null);

        return new UploadFileResponse()
        {
            HashId = hashId,
            Url = shareUrl,
            Width = imgWidth,
            Height = imgHeight,
            Type = resource.Type,
            Size = resource.Size
        };
    }

    public async Task<List<SubPostResponse>> ProcessFeedFilesAsync(List<ResourcePostReq> resourceRequest, Guid userId, string userFolder, string userAvatar, Guid postId, string postHashId)
    {
        var subPosts = new List<SubPostResponse>();

        // Complete resource files
        var (resources, subPostResponses) = await CompleteFilesAsyncAndSubPost(resourceRequest, userId, userFolder, userAvatar, postId, true);

        // Map to response for feed service
        foreach (var resource in resources)
        {
            var subPostHashId = subPostResponses.FirstOrDefault(p => p.Id == resource.SubPostId);
            var shareUrl = await _sc.Strategy.PresignedGetObject(resource.ShareUrl, _setting.Minio.MaxExpiryInSeconds, null);

            subPosts.Add(new SubPostResponse
            {
                Status = PostStatus.Public,
                Files = new List<UploadFileResponse> { new UploadFileResponse()
                                        {
                                            SubPostHashId = subPostHashId?.HashId,
                                            HashId = resource?.HashId,
                                            Url = shareUrl,
                                            ShareUrl = resource.ShareUrl,
                                            Height = resource.Height,
                                            Width = resource.Width,
                                            Order = resource.Order,
                                            Type = resource.Type,
                                            Size = resource.Size
                                        }},
                Title = resource.Title,
                //Name = resource.Name,
                Permission = PostPermission.PUBLIC,
                PublishDate = DateTime.UtcNow
            });
        }
        return subPosts;
    }

    public async Task<List<UploadFileResponse>> ProcessComicFilesAsync(List<ResourcePostReq> resourceRequest, Guid userId, string userFolder, string userAvatar, Guid subPostId)
    {
        var files = new List<UploadFileResponse>();

        // Complete resource files
        var resources = await CompleteFilesAsync(resourceRequest, userId, userFolder, userAvatar, subPostId, false);

        // Map to response for comic service
        foreach (var resource in resources)
        {
            files.Add(new UploadFileResponse()
            {
                HashId = resource.HashId,
                Order = resource.Order,
                Url = _setting.Minio.MediaApiUrl.GetMediaPath(resource.Name, resource.Url)
            });
        }

        return files;
    }

    public async Task<List<UploadFileResponse>> ProcessComicFilesUpdateAsync(List<ResourcePostReq> resourceRequest, Guid userId, string userFolder, string userAvatar, Guid subPostId)
    {
        var files = new List<UploadFileResponse>();

        // Complete resource files
        var resources = await CompleteFilesAsync(resourceRequest, userId, userFolder, userAvatar, subPostId, false);

        // Map to response for comic service
        foreach (var resource in resources)
        {
            files.Add(new UploadFileResponse()
            {
                HashId = resource.HashId,
                Order = resource.Order,
                Url = _setting.Minio.MediaApiUrl.GetMediaPath(resource.Name, resource.Url)
            });
        }

        return files;
    }

    private async Task<Tuple<List<Resource>, List<SubPostResponse>>> CompleteFilesAsyncAndSubPost(List<ResourcePostReq> resourceRequest, Guid userId, string userFolder, string userAvatar, Guid postId, bool addSubPost)
    {
        var response = new List<Resource>();
        var subPostResponses = new List<SubPostResponse>();
        if (string.IsNullOrWhiteSpace(userFolder))
        {
            throw new NotFoundException(ErrorCodes.NotExistedUser, ErrorMessage.AccountNotExist);
        }
        var hashIds = resourceRequest.Select(x => x.HashId).ToList();
        if (hashIds != null && hashIds.Any())
        {
            var resourceList = await _resourceRepository.Connection.QueryAsync<Resource>(GetListResourceQuery, new { HashIds = hashIds });

            foreach (var resource in resourceList)
            {
                if (resource == null)
                {
                    continue;
                }

                var resourceReq = resourceRequest.FirstOrDefault(x => x.HashId == resource.HashId);

                #region -- Copy file from temp target --
                string tempBlobName = resource.Name.GetTempBlobName(userFolder);
                string targetBlobName = resource.Name.GetMediaBlobName(userFolder);

                var tempObjectName = $"{BlobStorageDefinition.MediaContainer}/{tempBlobName}";
                var isExistTempFile = await _sc.Strategy.StatObjectAsync(tempObjectName, null);

                var targetObjectName = $"{BlobStorageDefinition.MediaContainer}/{targetBlobName}";
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
                    var subPost = new SubPost()
                    {
                        Title = resource.Title,
                        PostId = postId,
                        UserId = userId,
                        Body = resourceReq.Body,
                        CreatedBy = resource.CreatedBy,
                        Status = PostStatus.Public,
                        Order = resourceReq.Order,
                        Permission = PostPermission.PUBLIC,
                        PublishDate = DateTime.UtcNow,
                        HashId = SystemConfig.SubPostHashLength.GetRandomString(),
                        IsExclusive = false
                    };
                    subPostId = await _subPostRepository.InsertEntityAsync(subPost);
                    subPostResponses.Add(new SubPostResponse { HashId = subPost.HashId, Id = subPostId });
                }

                resource.Type = resource.Name.GetResourceType();
                resource.Url = targetBlobName.CreateMediaUrl(_setting.Minio.MediaEncryptKey);
                resource.ShareUrl = targetObjectName;
                resource.SubPostId = subPostId;
                resource.Order = resourceReq.Order;

                await _jobService.CreateConvertJob(resource, userFolder, userAvatar, targetBlobName);
                await _resourceRepository.UpdateAsync(resource);
                response.Add(resource);
            }
            response = response.OrderBy(x => x.Order).ToList();
        }
        return Tuple.Create(response, subPostResponses);
    }

    private async Task<IEnumerable<Resource>> CompleteFilesAsyncNew(List<ResourcePostReq> resourceRequest, List<Resource> resourceAdded, Guid userId, string userName, string userFolder, string userAvatar, Guid postId, bool addSubPost)
    {
        var response = new List<Resource>();
        if (string.IsNullOrWhiteSpace(userFolder))
        {
            throw new NotFoundException(ErrorCodes.NotExistedUser, ErrorMessage.AccountNotExist);
        }
        var hashIds = resourceRequest.Select(x => x.HashId).ToList();
        if (hashIds != null && hashIds.Any())
        {
            var resourceList = await _resourceRepository.Connection.QueryAsync<Resource>(GetListResourceQuery, new { HashIds = hashIds });

            foreach (var resource in resourceList)
            {
                if (resource == null)
                {
                    continue;
                }

                var resourceReq = resourceRequest.FirstOrDefault(x => x.HashId == resource.HashId);

                #region -- Copy file from temp target --
                string tempBlobName = resource.Name.GetTempBlobName(userFolder);
                string targetBlobName = resource.Name.GetMediaBlobName(userFolder);

                var tempObjectName = $"{BlobStorageDefinition.MediaContainer}/{tempBlobName}";
                var isExistTempFile = await _sc.Strategy.StatObjectAsync(tempObjectName, null);

                var targetObjectName = $"{BlobStorageDefinition.MediaContainer}/{targetBlobName}";
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
                    var subPost = new SubPost()
                    {
                        Title = resource.Title,
                        PostId = postId,
                        UserId = userId,
                        Body = resourceReq.Body,
                        CreatedBy = resource.CreatedBy,
                        Status = PostStatus.Public,
                        Order = resourceReq.Order,
                        Permission = PostPermission.PUBLIC,
                        PublishDate = DateTime.UtcNow,
                        HashId = SystemConfig.SubPostHashLength.GetRandomString(),
                        IsExclusive = false
                    };

                    subPostId = await _subPostRepository.InsertEntityAsync(subPost);
                }

                resource.Type = resource.Name.GetResourceType();
                resource.Url = targetBlobName.CreateMediaUrl(_setting.Minio.MediaEncryptKey);
                resource.ShareUrl = targetObjectName;
                resource.SubPostId = subPostId;
                resource.Order = resourceReq.Order;

                await _jobService.CreateConvertJob(resource, userName, userAvatar, targetBlobName);
                await _resourceRepository.UpdateAsync(resource);

                response.Add(resource);
            }
            response = response.OrderBy(x => x.Order).ToList();
        }
        return response;
    }

    private async Task<IEnumerable<Resource>> CompleteFilesAsync(List<ResourcePostReq> resourceRequest, Guid userId, string userFolder, string userAvatar, Guid postId, bool addSubPost)
    {
        var response = new List<Resource>();
        if (string.IsNullOrWhiteSpace(userFolder))
        {
            throw new NotFoundException(ErrorCodes.NotExistedUser, ErrorMessage.AccountNotExist);
        }
        var hashIds = resourceRequest.Select(x => x.HashId).ToList();
        if (hashIds != null && hashIds.Any())
        {
            var resourceList = await _resourceRepository.Connection.QueryAsync<Resource>(GetListResourceQuery, new { HashIds = hashIds });

            foreach (var resource in resourceList)
            {
                if (resource == null)
                {
                    continue;
                }

                var resourceReq = resourceRequest.FirstOrDefault(x => x.HashId == resource.HashId);

                #region -- Copy file from temp target --
                string tempBlobName = resource.Name.GetTempBlobName(userFolder);
                string targetBlobName = resource.Name.GetMediaBlobName(userFolder);

                var tempObjectName = $"{BlobStorageDefinition.MediaContainer}/{tempBlobName}";
                var isExistTempFile = await _sc.Strategy.StatObjectAsync(tempObjectName, null);

                var targetObjectName = $"{BlobStorageDefinition.MediaContainer}/{targetBlobName}";
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
                    var subPost = new SubPost()
                    {
                        Title = resource.Title,
                        PostId = postId,
                        UserId = userId,
                        Body = resourceReq.Body,
                        CreatedBy = resource.CreatedBy,
                        Status = PostStatus.Public,
                        Order = resourceReq.Order,
                        Permission = PostPermission.PUBLIC,
                        PublishDate = DateTime.UtcNow,
                        HashId = SystemConfig.SubPostHashLength.GetRandomString(),
                        IsExclusive = false
                    };

                    subPostId = await _subPostRepository.InsertEntityAsync(subPost);
                }

                resource.Type = resource.Name.GetResourceType();
                resource.Url = targetBlobName.CreateMediaUrl(_setting.Minio.MediaEncryptKey);
                resource.ShareUrl = targetObjectName;
                resource.SubPostId = subPostId;
                resource.Order = resourceReq.Order;

                await _jobService.CreateConvertJob(resource, userFolder, userAvatar, targetBlobName);
                await _resourceRepository.UpdateAsync(resource);

                response.Add(resource);
            }
            response = response.OrderBy(x => x.Order).ToList();
        }
        return response;
    }

    public async Task RemoveFileAsync(Guid postId, string userFolder)
    {
        if (string.IsNullOrWhiteSpace(userFolder))
        {
            throw new NotFoundException(ErrorCodes.NotExistedUser, ErrorMessage.AccountNotExist);
        }

        var resourcesDb = await _resourceRepository.Connection.QueryAsync<Resource>(GetResourcesByPostIdQuery, new { PostId = postId });
        if (resourcesDb.Any())
        {
            await _resourceRepository
                            .Connection.ExecuteAsync(RemoveFilesOfPostQuery, new
                            {
                                HashIds = resourcesDb.Select(p => p.HashId).ToList(),
                                SubPostIds = resourcesDb.Select(x => x.SubPostId).ToList()
                            });
            foreach (var resource in resourcesDb)
            {
                string tempBlobName = resource.Name.GetTempBlobName(userFolder);
                string targetBlobName = resource.Name.GetMediaBlobName(userFolder);

                tempBlobName = $"{BlobStorageDefinition.MediaContainer}/{tempBlobName}";
                var isExistTempFile = await _sc.Strategy.StatObjectAsync(tempBlobName, null);
                if (isExistTempFile != null)
                {
                    await _sc.Strategy.RemoveObject(tempBlobName, null);
                }

                targetBlobName = $"{BlobStorageDefinition.MediaContainer}/{targetBlobName}";
                var isExistTargetFile = await _sc.Strategy.StatObjectAsync(targetBlobName, null);
                if (isExistTargetFile != null)
                {
                    await _sc.Strategy.RemoveObject(targetBlobName, null);
                }
            }
        }
    }

    public async Task<List<SubPostResponse>> UpdateFeedFilesAsync(List<ResourcePostReq> resourceRequest, Guid userId, string userName, string userFolder, string userAvatar, Guid postId, string postHashId)
    {
        if (string.IsNullOrWhiteSpace(userFolder))
        {
            throw new NotFoundException(ErrorCodes.NotExistedUser, ErrorMessage.AccountNotExist);
        }
        /// resource current in post
        var resourcesDb = await _resourceRepository.Connection.QueryAsync<Resource>(GetResourcesByPostIdQuery, new { PostId = postId });
        var subPostDB = await _subPostRepository.Connection.QueryAsync<SubPost>(GetSubPostByPostIdQuery, new { PostId = postId });
        //Update
        var resourceDbHashId = resourcesDb.Select(x => x.HashId).ToList();
        var resourceRequestHashId = resourceRequest.Select(x => x.HashId).ToList();

        var listResourceNotAdd = resourceRequest.Where(x => !resourceDbHashId.Contains(x.HashId)).ToList();
        var listResourceAddded = resourcesDb.Where(x => resourceRequestHashId.Contains(x.HashId)).ToList();

        // Complete resource files
        var listResourcesNew = await CompleteFilesAsyncNew(listResourceNotAdd, listResourceAddded, userId, userName, userFolder, userAvatar, postId, true);

        //Remove
        var listRemove = resourcesDb.Where(x => !resourceRequestHashId.Contains(x.HashId)).ToList();
        var listRemoveHashId = listRemove.Select(x => x.HashId).ToList();

        if (listRemove.Any())
        {
            await _resourceRepository
                        .Connection.ExecuteAsync(RemoveFilesOfPostQuery, new
                        {
                            HashIds = listRemoveHashId,
                            SubPostIds = listRemove.Select(x => x.SubPostId).ToList()
                        });
        }

        // Check change order
        foreach (var resourceAdded in listResourceAddded)
        {
            var resourceReq = resourceRequest.FirstOrDefault(x => x.HashId == resourceAdded.HashId);
            var subPostByResource = subPostDB.FirstOrDefault(p => p.Id == resourceAdded.SubPostId);

            if (resourceReq != null && subPostByResource != null && ((resourceReq.Order != resourceAdded.Order) || subPostByResource.Body != resourceReq.Body))
            {
                resourceAdded.Order = resourceReq.Order;
                await _resourceRepository.UpdateAsync(resourceAdded);
                subPostByResource.Order = resourceReq.Order;
                subPostByResource.Body = resourceReq.Body;
                await _subPostRepository.UpdateAsync(subPostByResource);
            }
        }

        // Map to response for feed service
        var resourcesResult = listResourceAddded.Concat(listResourcesNew);
        resourcesResult = resourcesResult.Where(x => !listRemoveHashId.Contains(x.HashId)).ToList();
        var subPosts = new List<SubPostResponse>();
        var subpostAndResourceHashId = await _resourceRepository.Connection.QueryAsync<SubPostIds>($@"SELECT  sp.""HashId"" as SubPostHashId, sp.""Id"" as SubPostId from ""SubPosts"" sp
                                                                                                         LEFT JOIN ""Resources"" r on sp.""Id"" = r.""SubPostId""
                                                                                                         LEFT JOIN ""Posts"" p  on sp.""PostId""= p.""Id""
                                                                                                         WHERE p.""Id""=@PostId
                                                                                                         AND r.""IsDelete"" = false
                                                                                                         AND sp.""IsDelete"" = false", new { PostId = postId });
        foreach (var resource in resourcesResult.OrderBy(p => p.Order))
        {
            var shareUrl = await _sc.Strategy.PresignedGetObject(resource.ShareUrl, _setting.Minio.MaxExpiryInSeconds, null);
            var subPostData = subpostAndResourceHashId.FirstOrDefault(p => p.SubPostId == resource.SubPostId);
            subPosts.Add(new SubPostResponse
            {
                Body = resourceRequest.FirstOrDefault(p => p.Order == resource.Order).Body,
                HashId = subPostData?.SubPostHashId ?? "",
                Status = PostStatus.Public,
                Files = new List<UploadFileResponse> { new UploadFileResponse()
                                        {
                                            SubPostHashId = subPostData?.SubPostHashId ?? "",
                                            HashId = resource.HashId ,
                                            Url = shareUrl,
                                            ShareUrl = resource.ShareUrl,
                                            Height = resource.Height,
                                            Width = resource.Width,
                                            Order = resource.Order,
                                            Type = resource.Type,
                                            Size = resource.Size
                                        }},
                Title = resource.Title,
                Permission = PostPermission.PUBLIC,
                PublishDate = DateTime.UtcNow
            });
        }

        return subPosts;
    }

    #region -- Fields --

    /// <summary>
    /// DB Context
    /// </summary>
    private readonly McsgDbContext _context;

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    /// <summary>
    /// Storage client
    /// </summary>
    private readonly IStorageClient _sc;

    #endregion
}
