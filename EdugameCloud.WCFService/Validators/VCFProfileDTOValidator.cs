namespace EdugameCloud.WCFService.Validators
{
    using System;
    using System.Xml.Linq;
    using System.Xml.Schema;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.FluentValidation;
    using Esynctraining.Core.Providers;

    using FluentValidation;

    /// <summary>
    /// The VCFProfileDTO validator.
    /// </summary>
    public sealed class VCFProfileDTOValidator : AbstractValidator<VCFProfileDTO>
    {
        private readonly dynamic settings;

        private string validationError;

        /// <summary>
        /// Initializes a new instance of the <see cref="VCFProfileDTOValidator"/> class.
        /// </summary>
        public VCFProfileDTOValidator(ApplicationSettingsProvider settings)
        {
            this.settings = settings;
            this.RuleFor(model => model.xmlProfile)
                .NotEmpty()
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "xml profile is empty")
                .Must(this.ValidateAgainstVCFProfileSchema)
                .WithDynamicError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, () => "xml profile is invalid: " + this.validationError);
        }

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
            try
            {
                var schemas = new XmlSchemaSet();
                schemas.Add(null, this.settings.XSDProfileLocation);
                XDocument doc = XDocument.Parse(xml);
                string msg = string.Empty;
                doc.Validate(
                    schemas,
                    (o, e) =>
                        {
                            msg = e.Message;
                        });
                bool valid = string.IsNullOrWhiteSpace(msg);
                if (!valid)
                {
                    this.validationError = msg;
                }

                return valid;
            }
            catch (Exception ex)
            {
                this.validationError = ex.ToString();
                return false;
            }
        }
    }
}