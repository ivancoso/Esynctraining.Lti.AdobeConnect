namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Lti.DTO;

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