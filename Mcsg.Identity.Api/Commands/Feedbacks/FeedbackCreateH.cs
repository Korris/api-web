using MediatR;
using SixLabors.ImageSharp;

namespace Mcsg.Identity.Api.Commands;

using Common.Core.Constants;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Enums;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Interfaces;
using Requests;
using Validators;
using static Common.SeedWork.Constants.Message;

/// <summary>
/// Handler
/// </summary>
public class FeedbackCreateH : BaseMinioH, IRequestHandler<FeedbackCreateR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="setting">Setting</param>
    /// <param name="sc">Storage client</param>
    public FeedbackCreateH(IMcsgContext context, ISetting setting, IStorageClient sc) : base(context, setting, sc) { }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(FeedbackCreateR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        var vr = new FeedbackCreateV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToValue();
            throw new BadRequestException(M000, t);
        }

        if (request.UserId == null)
        {
            throw new BadRequestException(M109);
        }

        // Create
        var feedbackType = request.Type.ToEnum(FeedbackType.ReportAbuse);
        var ett = Feedback.Create(feedbackType, request.UserId, request.Email!, request.Comment + ""!);
        var feedback = await _context.Feedbacks.AddAsync(ett, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var files = request.Files;
        if (files?.Count > 0)
        {
            foreach (var file in files)
            {
                var hashId = Setting.ResourceConfig.HashLength.GetRandomString();
                var hashFileName = file.GetHashName(hashId);
                var objectName = $"{Setting.MinioFolder.User}/{request.UserFolder}/feedbacks/{hashFileName}";
                var bucketName = _setting.GetMinio(request.MinioInstance).BucketName;

                using (var stream = file.OpenReadStream())
                {
                    stream.Position = 0;
                    using var image = await Image.LoadAsync(stream);
                    stream.Position = 0;

                    await _sc.GetStrategy(MinioInstanceType.Default).PutObject(stream, objectName, bucketName);

                    var resource = new SystemResource
                    {
                        AuthorId = request.UserId,
                        HashId = hashId,
                        Title = Path.GetFileNameWithoutExtension(file.FileName),
                        Name = hashFileName,
                        Url = objectName,
                        BucketName = bucketName,
                        Type = file.IsImageType() ? ResourceType.Image : ResourceType.Video,
                        CreatedBy = request.UserId,
                        Width = image.Width,
                        Height = image.Height,
                        Size = file.Length,
                        CompressedSize = file.Length,
                        MinioInstance = request.MinioInstance,
                        FeedbackId = feedback.Entity.Id,
                    };

                    await _context.SystemResources.AddAsync(resource, cancellationToken);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        res.SetSuccess(ett.ToViewDto());

        return res;
    }

    #endregion

}