namespace Mcsg.Api.Areas.Document.Validators;

using Common.Domain.Entities;
using Common.Interfaces;
using Common.SeedWork.Exceptions;
using static Common.SeedWork.Constants.Error;

public class PostFavoriteValidator : IValidator<DocumentPostFavorite>
{
    private readonly IRepository<DocumentPost> _postRepository;
    public PostFavoriteValidator(IRepository<DocumentPost> postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task OnValidate(DocumentPostFavorite data)
    {
        _ = await _postRepository.GetByIdAsync(data.PostId, "\"Id\"") ?? throw new BadRequestException(nameof(E204), E204);
    }
}
