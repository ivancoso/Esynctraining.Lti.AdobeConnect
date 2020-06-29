namespace EdugameCloud.MVC.Validators
{
    using System.Web;
    using EdugameCloud.MVC.Links;
    using EdugameCloud.MVC.ViewModels;
    using Esynctraining.Core.Utils;

    using FluentValidation;

    /// <summary>
    ///     The VCF view model validator.
    /// </summary>
    public class VCFViewModelValidator : AbstractValidator<VCFViewModel>
    {
        #region Fields

        /// <summary>
        /// The validation error.
        /// </summary>
        private string validationError;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="VCFViewModelValidator" /> class.
        /// </summary>
        public VCFViewModelValidator()
        {
            this.RuleFor(x => x.XmlProfile)
                .NotEmpty()
                .WithMessage("xml profile is empty")
                .Must(this.ValidateAgainstVCFProfileSchema)
                .WithMessage("xml profile is invalid: " + this.validationError);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The validate against schema.
        /// </summary>
        /// <param name="xml">
        /// The xml.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool ValidateAgainstVCFProfileSchema(string xml)
        {
            var xsdFileName = HttpContext.Current.Server.MapPath(Content.xsd.vcfProfile_xsd);
            return XsdValidator.ValidateXmlAgainsXsd(xml, xsdFileName, out this.validationError);
        }

        #endregion
    }
}