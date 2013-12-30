namespace EdugameCloud.MVC.ViewModels
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The company view model.
    /// </summary>
    public class CompanyViewModel : EntityViewModel<Company, int>
    {
        /// <summary>
        /// Gets or sets the company name.
        /// </summary>
        public string CompanyName { get; set; }
    }
}
