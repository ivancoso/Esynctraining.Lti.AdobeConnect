namespace Esynctraining.AC.Provider.Entities
{
    using System;
    using System.Xml.Serialization;

    public class MeetingPermissionInfo : PermissionBase
    {
        [XmlAttribute("permission-id")]
        public MeetingPermissionId PermissionId { get; set; }


        public MeetingPermissionInfo() { }

        public MeetingPermissionInfo(PermissionInfo value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            // not great solution (
            PrincipalId = value.PrincipalId;
            HasChildren = value.HasChildren;
            IsPrimary = value.IsPrimary;
            Login = value.Login;
            Name = value.Name;
            Description = value.Description;

            PermissionId = (MeetingPermissionId)Enum.Parse(typeof(MeetingPermissionId), value.PermissionId.ToString());
        }
        
    }

}
