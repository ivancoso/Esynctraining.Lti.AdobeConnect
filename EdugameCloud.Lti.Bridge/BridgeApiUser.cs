using System.Collections.Generic;

namespace EdugameCloud.Lti.Bridge
{
    public class BridgeApiUser
    {
        public string Id { get; set; }
        public string Uid { get; set; }
        public string First_name { get; set; }
        public string Last_name { get; set; }
        public string Full_name { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public string Name { get; set; }
    }
}