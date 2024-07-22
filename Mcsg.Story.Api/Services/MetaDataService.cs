namespace Mcsg.Story.Api.Services;

using Common.Domain;
using Common.Domain.Entities;
using Dtos;
using Interfaces;

public class MetaDataService : IMetaDataService
{
    public MetaDataService(McsgContext context)
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
    private readonly McsgContext _context;

    #endregion
}
