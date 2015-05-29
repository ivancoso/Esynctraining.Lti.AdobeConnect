using Esynctraining.AC.Provider.Entities;

namespace EdugameCloud.Lti.Core.Domain.Entities
{
    public sealed class AcRole
    {
        public static readonly AcRole Host = new AcRole { Id = 1, Name = "Host", MeetingPermissionId = MeetingPermissionId.host };
        public static readonly AcRole Presenter = new AcRole { Id = 2, Name = "Presenter", MeetingPermissionId = MeetingPermissionId.mini_host };
        public static readonly AcRole Participant = new AcRole { Id = 3, Name = "Participant", MeetingPermissionId = MeetingPermissionId.view };


        public int Id { get; set; }

        public string Name { get; set; }

        public MeetingPermissionId MeetingPermissionId { get; set; }


        public static AcRole GetById(int acRoleId)
        {
            switch (acRoleId)
            {
                case 1: return Host;
                case 2: return Presenter;
                case 3: return Participant;
                default: return null;
            }
        }

        public static AcRole GetRoleName(PermissionId permissionId)
        {
            return permissionId == PermissionId.host
                ? Host.Name
                : (permissionId == PermissionId.mini_host
                ? Presenter.Name
                : Participant.Name);
        }

    }

}
