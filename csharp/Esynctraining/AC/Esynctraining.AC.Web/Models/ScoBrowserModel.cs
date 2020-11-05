namespace eSyncTraining.Web.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;

    public class ScoBrowserModel
    {
        [Display(Name = "Values")]
        public IEnumerable<ScoContentModel> Values { get; set; }

        [Display(Name = "Sco Type")]
        public string ScoType { get; set; }

        [Display(Name = "Folder Id")]
        public string FolderId { get; set; }

        public string CreateNewLabel
        {
            get
            {
                switch (this.ScoType)
                {
                    case "events":
                        return "Shared Event";
                    case "user-events":
                        return "User Event";
                    case "meetings":
                        return "Shared Meeting";
                    case "my-meetings":
                        return "My Meeting";
                    case "user-meetings":
                        return "User Meeting";
                    default:
                        return "Undefined Item";
                }
            }
        }
    }
}
