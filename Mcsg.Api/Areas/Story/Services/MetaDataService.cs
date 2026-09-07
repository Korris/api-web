namespace Mcsg.Api.Areas.Story.Services;

using Common.Domain;
using Common.Domain.Entities;
using Mcsg.Api.Areas.Story.Dtos;
using Mcsg.Api.Areas.Story.Interfaces;

public class MetaDataService : IMetaDataService
{
    public MetaDataService(IMcsgContext context)
    {
        _context = context;
    }

    public async Task<MetaDataDto> AddMetaDataToObject<T>(MetaDataDto request, Guid objId)
    {
        var metaData = new StoryMetaData
        {
            Title = request.Title,
            Description = request.Description,
            Domain = request.Domain,
            Url = request.Url
        };
        switch (typeof(T))
        {
            case
           var cls when cls == typeof(StoryPost):
                {
                    metaData.PostId = objId;
                    break;
                }
            case
            var cls when cls == typeof(StorySubPost):
                {
                    metaData.SubPostId = objId;
                    break;
                }
            case
            var cls when cls == typeof(StoryPostComment):
                {
                    metaData.PostCommentId = objId;
                    break;
                }
            case
            var cls when cls == typeof(StorySubPostComment):
                {
                    metaData.SubPostCommentId = objId;
                    break;
                }
        }

        await _context.StoryMetaDatas.AddAsync(metaData);
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
