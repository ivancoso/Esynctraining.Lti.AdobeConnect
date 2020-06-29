using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class SubModuleItemTheme
    {
        public int SubModuleItemThemeId { get; set; }
        public string BgColor { get; set; }
        public Guid? BgImageId { get; set; }
        public string TitleColor { get; set; }
        public string QuestionColor { get; set; }
        public string InstructionColor { get; set; }
        public string CorrectColor { get; set; }
        public string IncorrectColor { get; set; }
        public string SelectionColor { get; set; }
        public string HintColor { get; set; }
        public int SubModuleItemId { get; set; }

        public virtual File BgImage { get; set; }
        public virtual SubModuleItem SubModuleItem { get; set; }
    }
}
