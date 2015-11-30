namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.FluentValidation;
    using FluentValidation;

    /// <summary>
    /// The AppletResult DTO validator.
    /// </summary>
    public sealed class AppletResultDTOValidator : AbstractValidator<AppletResultDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppletResultDTOValidator"/> class.
        /// </summary>
        /// <param name="appletResultModel">
        /// The applet Result Model.
        /// </param>
        public AppletResultDTOValidator(AppletResultModel appletResultModel)
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            this.RuleFor(model => model.appletItemId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Applet item id is empty");
            this.RuleFor(model => model.acSessionId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "ACSession id is empty");
            this.RuleFor(model => model.participantName).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Participant name is empty");
            this.RuleFor(model => model.email)
                .Must((model, x) => (!model.isArchive && string.IsNullOrWhiteSpace(x)) || (model.isArchive && !string.IsNullOrWhiteSpace(x)))
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Email should be provided only in isArchive mode")
                .Must((model, x) => (!model.isArchive && string.IsNullOrWhiteSpace(x)) || appletResultModel.GetOneByACSessionIdAndEmail(model.acSessionId, x).Value == null)
                .WithError(Errors.CODE_ERRORTYPE_INVALID_SESSION, "You have already filled in this crossword");
        }

    }
}