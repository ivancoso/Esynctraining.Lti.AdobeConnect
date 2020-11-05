namespace Esynctraining.AC.Provider.Entities
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// SCO Info.
    /// </summary>
    [XmlRoot("sco")]
    public class ScoInfoByUrl : ScoInfo
    {
        public partial class OwnerPrincipal
        {
            public string PrincipalId { get; set; }

            public string AccountId { get; set; }

            public bool HasChildren { get; set; }

            public bool IsHidden { get; set; }

            public bool IsPrimary { get; set; }

            public string ExtLogin { get; set; }

            public string Login { get; set; }

            public string Name { get; set; }

            public string Email { get; set; }

        }


        public OwnerPrincipal Owner { get; set; }

    }

}
