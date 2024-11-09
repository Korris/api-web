namespace Mcsg.Comic.Api.Validators;

using Common.Domain.Entities;
using Common.Interfaces;
using Common.SeedWork.Exceptions;
using static Common.SeedWork.Constants.Error;
using static Common.SeedWork.Constants.Message;

public class PostFavoriteValidator : IValidator<ComicPostFavorite>
{
    private readonly IRepository<ComicPost> _postRepository;
    public PostFavoriteValidator(IRepository<ComicPost> postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task OnValidate(ComicPostFavorite data)
    {
        _ = await _postRepository.GetByIdAsync(data.PostId, "\"Id\"") ?? throw new BadRequestException(E204, M204);
    }
}
