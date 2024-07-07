namespace Mcsg.Social.Api.Validators
{
    using Common.SeedWork.Exceptions;
    using Constants;
    using Lib.Common.Interfaces;
    using Lib.Data.Domain.Entities;
    using Lib.Data.Repositories;

    public class PostFavoriteValidator : IValidator<PostFavorite>
    {
        private readonly IRepository<Post> _postRepository;
        public PostFavoriteValidator(IRepository<Post> postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task OnValidate(PostFavorite data)
        {
            _ = await _postRepository.GetByIdAsync(data.PostId, "\"Id\"") ?? throw new BadRequestException(ApiErrorCode.POST_NOT_EXIST, ApiErrorMessage.POST_NOT_EXIST);
        }
    }
}
