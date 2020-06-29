using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class State
    {
        public State()
        {
            Address = new HashSet<Address>();
            School = new HashSet<School>();
        }

        public int StateId { get; set; }
        public string StateCode { get; set; }
        public string StateName { get; set; }
        public bool IsActive { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int ZoomLevel { get; set; }

        public virtual ICollection<Address> Address { get; set; }
        public virtual ICollection<School> School { get; set; }
    }
}
