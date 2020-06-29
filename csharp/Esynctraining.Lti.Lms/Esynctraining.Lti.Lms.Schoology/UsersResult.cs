using System.Collections.Generic;

namespace Esynctraining.Lti.Lms.Schoology
{
    internal class UsersResult
    {
        public List<User> user { get; set; }
        public string total { get; set; }
        public Links links { get; set; }
    }
}
