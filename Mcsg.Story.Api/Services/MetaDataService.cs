namespace Mcsg.Story.Api.Services;

using Dtos;
using Interfaces;
using Lib.Data;
using Lib.Data.Domain.Entities;

public class MetaDataService : IMetaDataService
{
    public MetaDataService(McsgDbContext context)
    {
        _context = context;
    }

    public async Task<MetaDataDto> AddMetaDataToObject<T>(MetaDataDto request, Guid objId)
    {
        var metaData = new MetaData
        {
            Title = request.Title,
            Description = request.Description,
            Domain = request.Domain,
            Url = request.Url
        };
        switch (typeof(T))
        {
            case
           var cls when cls == typeof(Post):
                {
                    metaData.PostId = objId;
                    break;
                }
            case
            var cls when cls == typeof(SubPost):
                {
                    metaData.SubPostId = objId;
                    break;
                }
            case
            var cls when cls == typeof(PostComment):
                {
                    metaData.PostCommentId = objId;
                    break;
                }
            case
            var cls when cls == typeof(SubPostComment):
                {
                    metaData.SubPostCommentId = objId;
                    break;
                }
        }

        await _context.MetaDatas.AddAsync(metaData);
        await _context.SaveChangesAsync();

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
    private readonly McsgDbContext _context;

    #endregion
}
