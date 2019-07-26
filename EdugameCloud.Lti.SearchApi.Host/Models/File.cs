using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class File
    {
        public File()
        {
            AcuserMode = new HashSet<AcuserMode>();
            BuildVersion = new HashSet<BuildVersion>();
            CompanyTheme = new HashSet<CompanyTheme>();
            DistractorImage = new HashSet<Distractor>();
            DistractorLeftImage = new HashSet<Distractor>();
            DistractorRightImage = new HashSet<Distractor>();
            Question = new HashSet<Question>();
            SubModuleItemTheme = new HashSet<SubModuleItemTheme>();
        }

        public Guid FileId { get; set; }
        public string FileName { get; set; }
        public string Path { get; set; }
        public int? Height { get; set; }
        public int? Width { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public bool? IsActive { get; set; }
        public int? Status { get; set; }
        public int? X { get; set; }
        public int? Y { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual ICollection<AcuserMode> AcuserMode { get; set; }
        public virtual ICollection<BuildVersion> BuildVersion { get; set; }
        public virtual ICollection<CompanyTheme> CompanyTheme { get; set; }
        public virtual ICollection<Distractor> DistractorImage { get; set; }
        public virtual ICollection<Distractor> DistractorLeftImage { get; set; }
        public virtual ICollection<Distractor> DistractorRightImage { get; set; }
        public virtual ICollection<Question> Question { get; set; }
        public virtual ICollection<SubModuleItemTheme> SubModuleItemTheme { get; set; }
    }
}
