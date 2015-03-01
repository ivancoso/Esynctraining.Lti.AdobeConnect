namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;

    using FluentValidation;

    /// <summary>
    /// The AppletItem DTO validator.
    /// </summary>
    public class AppletItemDTOValidator : AbstractValidator<AppletItemDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppletItemDTOValidator"/> class.
        /// </summary>
        public AppletItemDTOValidator()
        {
            this.RuleFor(model => model.appletName).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Applet name is empty");
        }
    }
}