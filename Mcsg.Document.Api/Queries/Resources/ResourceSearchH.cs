using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Document.Api.Queries;

using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Dtos;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Dtos;
using Interfaces;
using Requests;
using Validators;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Handler
/// </summary>
public class ResourceSearchH : BaseMinioH, IRequestHandler<ResourceSearchR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    public ResourceSearchH(IMcsgContext context, ISetting setting, IStorageClient sc) : base(context, setting, sc) { }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(ResourceSearchR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        var vr = new ResourceSearchV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToDic();
            return res.SetError(nameof(E000), E000, t);
        }

        if (request.UserId == null)
        {
            return res.SetError(nameof(E109), E109);
        }

        #region -- Validate on server --
        var ettPost = await _context.Available<DocumentPost>().FirstOrDefaultAsync(p => p.HashId == request.PostHashId, cancellationToken);
        if (ettPost == null)
        {
            var t = new List<DicDto> { new() { Key = nameof(request.PostHashId).ToCamelCase(), Value = request.PostHashId + "" } };
            return res.SetError(nameof(E002), E002, t);
        }
        #endregion

        var q = from sp in _context.Available<DocumentSubPost>()
                join r in _context.Available<DocumentResource>()
                on sp.Id equals r.SubPostId
                where sp.PostId == ettPost.Id && sp.IsAllowDownload
                orderby sp.Order
                select new
                {
                    r.Title,
                    r.Url,
                    sp.Order,
                    r.BucketName,
                    r.MinioInstance
                };

        var items = await q.ToListAsync(cancellationToken);

        var data = new List<ResourceViewDto>();
        foreach (var i in items)
        {
            var dto = new ResourceViewDto
            {
                Title = i.Title,
                Url = await _sc.GetPublicUrl(i.Url, i.BucketName, i.MinioInstance),
                Order = i.Order,
            };

            data.Add(dto);
        }

        return res.SetSuccess(data);
    }

    #endregion
}
