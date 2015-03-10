namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;

    using FluentValidation;

    /// <summary>
    ///     The company DTO validator.
    /// </summary>
    public class EmailHistoryDTOValidator : AbstractValidator<EmailHistoryDTO>
    {
        #region Fields

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="EmailHistoryDTOValidator" /> class.
        /// </summary>
        public EmailHistoryDTOValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            this.RuleFor(model => model.sentFrom)
                .NotEmpty()
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Sent from is empty");
            this.RuleFor(model => model.sentTo)
                .NotEmpty()
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Sent to is empty");
            this.RuleFor(model => model.date)
                .NotEmpty()
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Date created is empty");
        }

        #endregion

        #region Methods

        #endregion
    }
}