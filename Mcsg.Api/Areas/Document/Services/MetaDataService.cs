namespace Mcsg.Api.Areas.Document.Services;

using Common.Domain;
using Common.Domain.Entities;
using Mcsg.Api.Areas.Document.Dtos;
using Mcsg.Api.Areas.Document.Interfaces;

public class MetaDataService : IMetaDataService
{
    public MetaDataService(IMcsgContext context)
    {
        _context = context;
    }

    public async Task<MetaDataDto> AddMetaDataToObject<T>(MetaDataDto request, Guid objId)
    {
        var metaData = new DocumentMetaData
        {
            Title = request.Title,
            Description = request.Description,
            Domain = request.Domain,
            Url = request.Url
        };
        switch (typeof(T))
        {
            case
           var cls when cls == typeof(DocumentPost):
                {
                    metaData.PostId = objId;
                    break;
                }
            case
            var cls when cls == typeof(DocumentSubPost):
                {
                    metaData.SubPostId = objId;
                    break;
                }
            case
            var cls when cls == typeof(DocumentPostComment):
                {
                    metaData.PostCommentId = objId;
                    break;
                }
            case
            var cls when cls == typeof(DocumentSubPostComment):
                {
                    metaData.SubPostCommentId = objId;
                    break;
                }
        }

        await _context.DocumentMetaDatas.AddAsync(metaData);
        await _context.SaveChangesAsync(default);

        return new MetaDataDto
        {
            Description = metaData.Description,
            Domain = metaData.Domain,
            Title = metaData.Title,
            Url = metaData.Url
        };
    }

    #region -- Fields --

    /// <summary>
    /// DB context
    /// </summary>
    private readonly IMcsgContext _context;

    #endregion
}
