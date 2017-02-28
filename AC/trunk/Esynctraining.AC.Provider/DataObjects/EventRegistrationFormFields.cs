using System.Collections.Generic;

namespace Esynctraining.AC.Provider.DataObjects
{
    public class EventRegistrationFormFields
    {
        public string ScoId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string VerifyPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Dictionary<string,string> AdditionalFields { get;set; }
    }
}