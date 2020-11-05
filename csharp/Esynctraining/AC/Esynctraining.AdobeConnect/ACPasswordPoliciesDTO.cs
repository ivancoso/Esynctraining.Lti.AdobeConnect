using System.Runtime.Serialization;

namespace Esynctraining.AdobeConnect
{
    [DataContract]
    public class ACPasswordPoliciesDTO
    {
        [DataMember(Name = "passwordRequiresDigit")]
        public bool PasswordRequiresDigit { get; set; }

        [DataMember(Name = "passwordRequiresCapitalLetter")]
        public bool PasswordRequiresCapitalLetter { get; set; }

        [DataMember(Name = "passwordRequiresSpecialChars")]
        public string PasswordRequiresSpecialChars { get; set; }

        [DataMember(Name = "passwordMinLength")]
        public int PasswordMinLength { get; set; }

        [DataMember(Name = "passwordMaxLength")]
        public int PasswordMaxLength { get; set; }

        [DataMember(Name = "loginSameAsEmail")]
        public bool LoginSameAsEmail { get; set; }

    }

}
