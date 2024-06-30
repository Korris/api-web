namespace Mcsg.Social.Api.Services
{
    using Interfaces;
    using Lib.Data.Domain.Entities;
    using Lib.Data.Repositories;
    using Lib.Data.Repositories.Interface;
    using Models;
    using Requests;

    public class MetaDataService : IMetaDataService
    {
        private readonly ILogger<LinkPreviewService> _logger;
        private readonly IRepository<MetaData> _metaDataRepository;
        public MetaDataService(ILogger<LinkPreviewService> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _metaDataRepository = unitOfWork.GetRepository<MetaData>();
        }
        public async Task<MetaDataResponse> AddMetaDataToObject<T>(MetaDataReq request, Guid objId)
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
            await _metaDataRepository.InsertAsync(metaData);
            return new MetaDataResponse
            {
                Description = metaData.Description,
                Domain = metaData.Domain,
                Title = metaData.Title,
                Url = metaData.Url
            };
        }
    }
}
