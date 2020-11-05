using System.Collections.Generic;

namespace Esynctraining.Zoom.ApiWrapper.Model
{
    public class NewGroupMembers
    {
        public List<NewGroupMember> Members { get; set; }
    }

    public class NewGroupMember
    {
        public string Id { get; set; }
        public string Email { get; set; }
    }
}
