namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;

    using FluentValidation;

    public class MoodleUserDTOValidator : AbstractValidator<MoodleTokenDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoodleUserDTOValidator"/> class.
        /// </summary>
        public MoodleUserDTOValidator()
        {

        }
    }
}