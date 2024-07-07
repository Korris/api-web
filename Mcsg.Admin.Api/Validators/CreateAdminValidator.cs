using Microsoft.AspNetCore.Identity;

namespace Mcsg.Admin.Api.Validators
{
    using Common.SeedWork.Exceptions;
    using Constants;
    using Lib.Common.Constants;
    using Lib.Common.Interfaces;
    using Lib.Common.Mail;
    using Lib.Data.Domain.Entities;
    using Requests;

    public class CreateAdminValidator : IValidator<CreateAdminReq>
    {
        private readonly UserManager<User> _userManager;
        public CreateAdminValidator(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task OnValidate(CreateAdminReq req)
        {
            if (!EmailHelper.IsValid(req.Email))
                throw new BadRequestException(ErrorCodes.InvalidEmail, ErrorMessage.InvalidEmail);

            var isEmailExisted = _userManager.Users.Where(x => x.NormalizedEmail == req.Email.ToUpper())
                .Select(x => x.Id).Any();

            if (isEmailExisted)
            {
                throw new BadRequestException(ErrorCodes.DuplicateEmail, ErrorMessage.DuplicateEmail);
            }

            List<string> passwordErrors = new();

            var validators = _userManager.PasswordValidators;

            foreach (var validator in validators)
            {
                var result = await validator.ValidateAsync(_userManager, null, req.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        passwordErrors.Add(error.Description);
                    }
                }
            }

            if (passwordErrors.Any())
            {
                throw new BadRequestException(ErrorCodes.PasswordInCorrect, string.Join(";", passwordErrors));
            }

            if (string.IsNullOrWhiteSpace(req.FirstName))
                throw new BadRequestException(AdminErrorCodes.FIRSTNAME_NOT_EMPTY, AdminErrorMessages.FIRSTNAME_NOT_EMPTY);

            if (string.IsNullOrWhiteSpace(req.LastName))
                throw new BadRequestException(AdminErrorCodes.LASTNAME_NOT_EMPTY, AdminErrorMessages.LASTNAME_NOT_EMPTY);

            await Task.CompletedTask;
        }
    }
}
