namespace EdugameCloud.Lti.OAuth.Desire2Learn
{
    // http://docs.valence.desire2learn.com/res/enroll.html#Enrollment.ClasslistUser
    public class ClasslistUser
    {
        public string Identifier { get; set; }
        public string ProfileIdentifier { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string OrgDefinedId { get; set; }
        public string Email { get; set; }

        public string FirstName { get; set; }  // Added with LE v1.7 API

        public string LastName { get; set; }  // Added with LE v1.7 API


        public string GetValidFullName()
        {
            if (!string.IsNullOrWhiteSpace(FirstName) && !string.IsNullOrWhiteSpace(LastName))
                return $"{FirstName} {LastName}";

            // NOTE we expect "FirstName LastName" formar
            // But customer has "LastName, FirstName"
            int index = DisplayName.IndexOf(", ");
            if (index >= 0)
            {
                string lastName = DisplayName.Substring(0, index);
                string firstName = DisplayName.Substring(index + 2);
                return $"{firstName} {lastName}";
            }

            return DisplayName;
        }

    }

}