using System;
using System.Runtime.Serialization;
using Esynctraining.AC.Provider.Entities;

namespace EdugameCloud.Lti.Core.Domain.Entities
{
    [DataContract]
    public sealed class AcRole
    {
        public static readonly AcRole None = new AcRole { Id = 0, Name = "None", MeetingPermissionId = MeetingPermissionId.remove };
        public static readonly AcRole Host = new AcRole { Id = 1, Name = "Host", MeetingPermissionId = MeetingPermissionId.host };
        public static readonly AcRole Presenter = new AcRole { Id = 2, Name = "Presenter", MeetingPermissionId = MeetingPermissionId.mini_host };
        public static readonly AcRole Participant = new AcRole { Id = 3, Name = "Participant", MeetingPermissionId = MeetingPermissionId.view };


        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "systemName")]
        public string Name { get; set; }

        [DataMember(Name = "name")]
        public string LocalizableName
        {
            get
            {
                return Resources.AcRole.ResourceManager.GetString(Name);
            }
        }

        public MeetingPermissionId MeetingPermissionId { get; set; }


        public static AcRole GetById(int acRoleId)
        {
            switch (acRoleId)
            {
                case 0: return None;
                case 1: return Host;
                case 2: return Presenter;
                case 3: return Participant;
                default:
                    throw new InvalidOperationException(string.Format("Not supported Role: {0}", acRoleId));
            }
        }

        //public static AcRole GetByName(string acMeetingRoleName)
        //{
        //    if (acMeetingRoleName == None.Name)
        //        return None;

        //    if (acMeetingRoleName == Host.Name)
        //        return Host;

        //    if (acMeetingRoleName == Presenter.Name)
        //        return Presenter;

        //    if (acMeetingRoleName == Participant.Name)
        //        return Participant;

        //    throw new InvalidOperationException(string.Format("Not supported role name: {0}", acMeetingRoleName));
        //}

        public static int? GetRoleId(PermissionId permissionId)
        {
            if (permissionId == PermissionId.host)
                return Host.Id;

            if (permissionId == PermissionId.mini_host)
                return Presenter.Id;

            if (permissionId == PermissionId.view)
                return Participant.Id;

            return null;
        }

    }

}
