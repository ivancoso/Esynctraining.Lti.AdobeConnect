namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;

    using FluentValidation;

    /// <summary>
    ///     The company DTO validator.
    /// </summary>
    public class NewsletterSubscriptionDTOValidator : AbstractValidator<NewsletterSubscriptionDTO>
    {
        #region Fields

        /// <summary>
        /// The user model.
        /// </summary>
        private readonly NewsletterSubscriptionModel newsletterSubscriptionModel;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="EmailHistoryDTOValidator" /> class.
        /// </summary>
        public NewsletterSubscriptionDTOValidator(NewsletterSubscriptionModel newsletterSubscriptionModel)
        {
            this.newsletterSubscriptionModel = newsletterSubscriptionModel;
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            this.RuleFor(model => model.email)
                .NotEmpty()
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Email is empty")
                .Must((model, x) => this.ValidateEmailAlreadyExists(model.newsletterSubscriptionId, x))
                .WithError(Errors.CODE_ERRORTYPE_USER_EXISTING, "Email has already been subscribed");
            this.RuleFor(model => model.dateSubscribed)
                .NotEmpty()
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Date subscribed is empty");
            this.RuleFor(model => model.isActive)
                .NotEmpty()
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Is active is empty");
        }

        #endregion

        #region Methods

        /// <summary>
        /// The validate email already exists.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ValidateEmailAlreadyExists(int id, string email)
        {
            NewsletterSubscription newsletterSubscription = this.newsletterSubscriptionModel.GetOneByEmail(email).With(x => x.Value);

            if (newsletterSubscription == null || newsletterSubscription.Id == id)
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}