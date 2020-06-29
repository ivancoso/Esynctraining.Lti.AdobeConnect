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

    public class ImportUsersViewModel : BaseViewModel
    {
        #region Constructors and Destructors

        public ImportUsersViewModel()
        {
        }

        public ImportUsersViewModel(IEnumerable<Company> companies, BaseController controller)
            : base(controller)
        {
            SetCompanies(companies);
        }

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
            Companies = companies.Select(x => new SelectListItem { Text = x.CompanyName, Value = x.Id.ToString(CultureInfo.InvariantCulture) });
        }

    }

}