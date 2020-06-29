namespace EdugameCloud.Canvas.ViewModels
{
    using System.Collections.Generic;

    using Esynctraining.AC.Provider.Entities;

    using BaseController = EdugameCloud.Canvas.Controllers.BaseController;

    /// <summary>
    /// The login view model.
    /// </summary>
    public class HomeViewModel : BaseViewModel
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeViewModel"/> class.
        /// </summary>
        /// <param name="baseController">
        /// The base controller.
        /// </param>
        public HomeViewModel(BaseController baseController)
            : base(baseController)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeViewModel"/> class.
        /// </summary>
        public HomeViewModel()
        {
        }

        #endregion

        #region Public Properties

        public int custom_canvas_course_id { get; set; }

        public string custom_ac_folder_sco_id { get; set; }

        public string custom_canvas_api_domain { get; set; }

        public string context_id { get; set; }

        public string context_label { get; set; }

        public string context_title { get; set; }
        
        public string lis_person_name_given { get; set; }

        public string lis_person_name_family { get; set; }

        public string lis_person_name_full { get; set; }

        public string launch_presentation_return_url { get; set; }
        
        public string lis_person_contact_email_primary { get; set; }

        public string roles { get; set; }

        #endregion
    }
}
