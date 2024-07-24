namespace Mcsg.Social.Api.Validators;

using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
using Lib.Common.Interfaces;
using Lib.Data.Repositories;
using static Common.SeedWork.Constants.Error;
using static Common.SeedWork.Constants.Message;

public class PostFavoriteValidator : IValidator<SocialPostFavorite>
{
    private readonly IRepository<SocialPost> _postRepository;
    public PostFavoriteValidator(IRepository<SocialPost> postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task OnValidate(SocialPostFavorite data)
    {
        _ = await _postRepository.GetByIdAsync(data.PostId, "\"Id\"") ?? throw new BadRequestException(E204, M204);
    }
}
