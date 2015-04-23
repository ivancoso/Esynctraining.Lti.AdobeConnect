using System;

namespace Esynctraining.AC.Provider.Entities
{
    public sealed class PermissionUpdateTrio
    {
        public string ScoId { get; set; }

        public string PrincipalId { get; set; }

        public MeetingPermissionId PermissionId { get; set; }


        internal PermissionId Permission 
        {
            get
            {
                switch (PermissionId)
                {
                    case MeetingPermissionId.host:
                        return Entities.PermissionId.host;
                    case MeetingPermissionId.mini_host:
                        return Entities.PermissionId.mini_host;
                    case MeetingPermissionId.view:
                        return Entities.PermissionId.view;
                    case MeetingPermissionId.remove:
                        return Entities.PermissionId.remove;
                    case MeetingPermissionId.denied:
                        return Entities.PermissionId.denied;
                    case MeetingPermissionId.manage:
                        return Entities.PermissionId.manage;
                    case MeetingPermissionId.publish:
                        return Entities.PermissionId.publish;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

    }

}
