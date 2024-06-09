using Mcsg.Social.Api.Constants;
using Mcsg.Lib.Common.Exceptions;
using Mcsg.Lib.Common.Interfaces;
using Mcsg.Lib.Data.Domain.Entities;
using Mcsg.Lib.Data.Repositories;

namespace Mcsg.Social.Api.Validators
{
    public class TagFavoriteValidator : IValidator<TagFavorite>
    {
        private readonly IRepository<Tag> _tagRepository;
        public TagFavoriteValidator(IRepository<Tag> tagRepository)
        {
            _tagRepository = tagRepository;
        }
        public async Task OnValidate(TagFavorite data)
        {
            _ = await _tagRepository.GetByIdAsync(data.TagId, "\"Id\"") ?? throw new BadRequestException(ApiErrorCode.TAG_NOT_EXIST, ApiErrorMessage.TAG_NOT_EXIST);
        }
    }
}
