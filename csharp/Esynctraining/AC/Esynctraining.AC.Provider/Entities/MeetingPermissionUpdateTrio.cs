﻿using System;

namespace Esynctraining.AC.Provider.Entities
{
    public sealed class MeetingPermissionUpdateTrio : IPermissionUpdateTrio
    {
        public string ScoId { get; set; }

        public string PrincipalId { get; set; }

        public MeetingPermissionId PermissionId { get; set; }


        public PermissionId Permission
        {
            get
            {
                switch (PermissionId)
                {
                    case MeetingPermissionId.none:
                        return Entities.PermissionId.none;
                    case MeetingPermissionId.host:
                        return Entities.PermissionId.host;
                    case MeetingPermissionId.mini_host:
                        return Entities.PermissionId.mini_host;
                    case MeetingPermissionId.view:
                        return Entities.PermissionId.view;
                    case MeetingPermissionId.remove:
                        return Entities.PermissionId.remove;
                    //case MeetingPermissionId.denied:
                    //    return Entities.PermissionId.denied;
                    //case MeetingPermissionId.manage:
                    //    return Entities.PermissionId.manage;
                    //case MeetingPermissionId.publish:
                    //    return Entities.PermissionId.publish;
                    default:
                        throw new NotImplementedException($"PermissionId '{PermissionId}' is not supported value.");
                }
            }
        }

    }

}
