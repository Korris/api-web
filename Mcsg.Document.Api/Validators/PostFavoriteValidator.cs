namespace Mcsg.Document.Api.Validators;

using Common.Domain.Entities;
using Common.Interfaces;
using Common.SeedWork.Exceptions;
using static Common.SeedWork.Constants.Error;
using static Common.SeedWork.Constants.Message;

public class PostFavoriteValidator : IValidator<DocumentPostFavorite>
{
    private readonly IRepository<DocumentPost> _postRepository;
    public PostFavoriteValidator(IRepository<DocumentPost> postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task OnValidate(DocumentPostFavorite data)
    {
        _ = await _postRepository.GetByIdAsync(data.PostId, "\"Id\"") ?? throw new BadRequestException(E204, M204);
    }
}
