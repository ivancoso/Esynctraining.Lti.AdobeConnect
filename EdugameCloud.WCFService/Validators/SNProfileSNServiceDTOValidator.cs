namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;

    using FluentValidation;

    /// <summary>
    /// The SNProfileSNServiceDTO validator.
    /// </summary>
    public class SNProfileSNServiceDTOValidator : AbstractValidator<SNProfileSNServiceDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SNProfileSNServiceDTOValidator"/> class.
        /// </summary>
        public SNProfileSNServiceDTOValidator()
        {
            this.RuleFor(model => model.snServiceId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Service id is empty");
        }
    }
}