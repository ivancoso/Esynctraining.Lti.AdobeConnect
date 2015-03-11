namespace EdugameCloud.Lti.OAuth.Desire2Learn
{
    public class UserData
    {
        public int OrgId { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string ExternalEmail { get; set; }
        public string OrgDefinedId { get; set; }
        public string UniqueIdentifier { get; set; }
        public UserActivationData Activation { get; set; }
    }
}