namespace Mcsg.Api.Areas.Social.Services;

using Common.Domain;
using Common.Domain.Entities;
using Mcsg.Api.Areas.Social.Dtos;
using Mcsg.Api.Areas.Social.Interfaces;

public class MetaDataService : IMetaDataService
{
    public MetaDataService(IMcsgContext context)
    {
        _context = context;
    }

    public async Task<MetaDataDto> AddMetaDataToObject<T>(MetaDataDto request, Guid objId)
    {
        var metaData = new SocialMetaData
        {
            Title = request.Title,
            Description = request.Description,
            Domain = request.Domain,
            Url = request.Url
        };
        switch (typeof(T))
        {
            case
           var cls when cls == typeof(SocialPost):
                {
                    metaData.PostId = objId;
                    break;
                }
            case
            var cls when cls == typeof(SocialSubPost):
                {
                    metaData.SubPostId = objId;
                    break;
                }
            case
            var cls when cls == typeof(SocialPostComment):
                {
                    metaData.PostCommentId = objId;
                    break;
                }
            case
            var cls when cls == typeof(SocialSubPostComment):
                {
                    metaData.SubPostCommentId = objId;
                    break;
                }
        }

        await _context.SocialMetaDatas.AddAsync(metaData);
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
