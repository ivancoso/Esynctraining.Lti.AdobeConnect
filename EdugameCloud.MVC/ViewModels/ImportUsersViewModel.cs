namespace EdugameCloud.MVC.ViewModels
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.MVC.Attributes;
    using EdugameCloud.MVC.Controllers;

    /// <summary>
    ///     The import users view model.
    /// </summary>
    public class ImportUsersViewModel : BaseViewModel
    {
        #region Constructors and Destructors

        public ImportUsersViewModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportUsersViewModel"/> class.
        /// </summary>
        /// <param name="companies">
        /// The companies.
        /// </param>
        /// <param name="controller">
        /// The controller.
        /// </param>
        public ImportUsersViewModel(IEnumerable<Company> companies, BaseController controller)
            : this(companies, controller, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportUsersViewModel"/> class.
        /// </summary>
        /// <param name="companies">
        /// The companies.
        /// </param>
        /// <param name="controller">
        /// The controller.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        public ImportUsersViewModel(IEnumerable<Company> companies, BaseController controller, int? page)
            : base(controller, page)
        {
            this.SetCompanies(companies);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportUsersViewModel"/> class.
        /// </summary>
        /// <param name="companies">
        /// The companies.
        /// </param>
        public ImportUsersViewModel(IEnumerable<Company> companies)
            : this(companies, null)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the companies.
        /// </summary>
        public IEnumerable<SelectListItem> Companies { get; set; }

        /// <summary>
        /// Gets or sets the company id.
        /// </summary>
        [LocalizedDisplayName("Company", ResourceName = "ImportUsers")]
        public int CompanyId { get; set; }

        /// <summary>
        /// Gets or sets the xml profile.
        /// </summary>
        [LocalizedDisplayName("Users", ResourceName = "ImportUsers")]
        public HttpPostedFileBase ProfilesFile { get; set; }

        /// <summary>
        /// Gets or sets the detailed error.
        /// </summary>
        [LocalizedDisplayName("ImportDetails", ResourceName = "ImportUsers")]
        public string DetailedError { get; set; }

        #endregion

        /// <summary>
        /// The set companies.
        /// </summary>
        /// <param name="companies">
        /// The companies.
        /// </param>
        public void SetCompanies(IEnumerable<Company> companies)
        {
            this.Companies = companies.Select(x => new SelectListItem { Text = x.CompanyName, Value = x.Id.ToString(CultureInfo.InvariantCulture) });
        }
    }
}