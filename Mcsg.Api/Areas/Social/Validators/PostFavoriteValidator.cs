namespace Mcsg.Api.Areas.Social.Validators;

using Common.Domain.Entities;
using Common.Interfaces;
using Common.SeedWork.Exceptions;
using static Common.SeedWork.Constants.Error;

public class PostFavoriteValidator : IValidator<SocialPostFavorite>
{
    private readonly IRepository<SocialPost> _postRepository;
    public PostFavoriteValidator(IRepository<SocialPost> postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task OnValidate(SocialPostFavorite data)
    {
        _ = await _postRepository.GetByIdAsync(data.PostId, "\"Id\"") ?? throw new BadRequestException(nameof(E204), E204);
    }
}
