namespace eSyncTraining.Web.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public abstract class ScoDetailModelBase
    {
        public ScoDetailModelBase()
        {
            this.BeginDate = DateTime.Now;
            this.EndDate = DateTime.Now.AddHours(1);
            this.DateCreated = DateTime.Now;
            this.DateModified = DateTime.Now;
        }

        [Display(Name = "Sco Id")]
        public string ScoId { get; set; }

        [Display(Name = "Folder Id")]
        [Required(ErrorMessage = "is required.")]
        public string FolderId { get; set; }

        //[Display(Name = "Template")]
        //public string TemplateId { get; set; }

        [Display(Name = "URL Path")]
        public string UrlPath { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Language")]
        public string Language { get; set; }

        //[Display(Name = "Sco Tag")]
        //public string ScoTag { get; set; }

        [Display(Name = "Begins at")]
        public DateTime BeginDate { get; set; }

        [Display(Name = "Ends at")]
        public DateTime EndDate { get; set; }
        

        [Display(Name = "Created at")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Modified at")]
        public DateTime DateModified { get; set; }
    }
}
