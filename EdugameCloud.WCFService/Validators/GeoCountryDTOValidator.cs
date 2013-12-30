namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using FluentValidation;

    /// <summary>
    /// The user DTO validator.
    /// </summary>
    public class GeoCountryDTOValidator : AbstractValidator<GeoCountryDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeoCountryDTOValidator"/> class.
        /// </summary>
        /// <param name="countryModel">
        /// The country Model.
        /// </param>
        public GeoCountryDTOValidator(CountryModel countryModel)
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            this.RuleFor(model => model.countryId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Country Id is empty")
                .Must(x => countryModel.GetOneById(x).Value != null).WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Country doesn't exist");
            this.RuleFor(model => model.latitude).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Latitude is empty");
            this.RuleFor(model => model.longitude).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Longitude is empty");
            this.RuleFor(model => model.zoomLevel).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Zoom level is empty");
        }
    }
}