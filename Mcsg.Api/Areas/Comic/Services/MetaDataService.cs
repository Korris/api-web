namespace Mcsg.Api.Areas.Comic.Services;

using Common.Domain;
using Common.Domain.Entities;
using Mcsg.Api.Areas.Comic.Dtos;
using Mcsg.Api.Areas.Comic.Interfaces;
using Mcsg.Api.Interfaces;

public class MetaDataService : IMetaDataService
{
    public MetaDataService(IMcsgContext context)
    {
        _context = context;
    }

    public async Task<MetaDataDto> AddMetaDataToObject<T>(MetaDataDto request, Guid objId)
    {
        var metaData = new ComicMetaData
        {
            Title = request.Title,
            Description = request.Description,
            Domain = request.Domain,
            Url = request.Url
        };
        switch (typeof(T))
        {
            case
           var cls when cls == typeof(ComicPost):
                {
                    metaData.PostId = objId;
                    break;
                }
            case
            var cls when cls == typeof(ComicSubPost):
                {
                    metaData.SubPostId = objId;
                    break;
                }
            case
            var cls when cls == typeof(ComicPostComment):
                {
                    metaData.PostCommentId = objId;
                    break;
                }
            case
            var cls when cls == typeof(ComicSubPostComment):
                {
                    metaData.SubPostCommentId = objId;
                    break;
                }
        }

        await _context.ComicMetaDatas.AddAsync(metaData);
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
