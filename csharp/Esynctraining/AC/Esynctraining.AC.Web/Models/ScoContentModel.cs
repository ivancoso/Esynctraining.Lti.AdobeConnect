namespace eSyncTraining.Web.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ScoContentModel
    {
        [Display(Name = "Sco Id")]
        public string ScoId { get; set; }

        [Display(Name = "Source Sco Id")]
        public string SourceScoId { get; set; }

        [Display(Name = "Folder Id")]
        public int FolderId { get; set; }

        [Display(Name = "Type")]
        public string Type { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Begins at")]
        public DateTime BeginDate { get; set; }

        [Display(Name = "Ends at")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Path")]
        public string Path { get; set; }

        [Display(Name = "Duration")]
        public int Duration { get; set; }

        [Display(Name = "Created at")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Modified at")]
        public DateTime DateModified { get; set; }

        public bool IsFolder
        {
            get
            {
                return this.Type.Equals("folder", StringComparison.InvariantCultureIgnoreCase);
            }
        }
    }
}
