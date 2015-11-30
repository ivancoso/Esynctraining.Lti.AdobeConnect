namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using Esynctraining.Core.Enums;
    using Esynctraining.FluentValidation;
    using FluentValidation;

    /// <summary>
    /// The user DTO validator.
    /// </summary>
    public sealed class GeoStateDTOValidator : AbstractValidator<GeoStateDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeoStateDTOValidator"/> class.
        /// </summary>
        /// <param name="stateModel">
        /// The state Model.
        /// </param>
        public GeoStateDTOValidator(StateModel stateModel)
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            this.RuleFor(model => model.stateId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "State Id is empty")
                .Must(x => stateModel.GetOneById(x).Value != null).WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "State doesn't exist");
            this.RuleFor(model => model.latitude).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Latitude is empty");
            this.RuleFor(model => model.longitude).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Longitude is empty");
            this.RuleFor(model => model.zoomLevel).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Zoom level is empty");
        }
    }
}