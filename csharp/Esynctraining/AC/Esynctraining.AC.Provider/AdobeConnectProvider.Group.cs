﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Esynctraining.AC.Provider.Constants;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AC.Provider
{
    public partial class AdobeConnectProvider
    {
        /// <summary>
        /// NOTE: Searches custom groups only (with type = 'group')
        /// </summary>
        public PrincipalResult GetGroupByName(string groupName)
        {
            var groupsResult = GetGroupsByType(PrincipalType.group);
            var group = groupsResult.Values?.FirstOrDefault(g => g.Name.Equals(groupName, StringComparison.InvariantCultureIgnoreCase));
            if (null != group)
            {
                return new PrincipalResult(groupsResult.Status, group);
            }

            return new PrincipalResult(groupsResult.Status);
        }

        public PrincipalCollectionResult GetGroupsByType(PrincipalType type)
        {
            var filter = "&filter-type=" + type.ToString().Replace("_", "-");
            return CallPrincipalList(filter);
        }

        public PrincipalCollectionResult GetPrimaryGroupsByType(PrincipalType type)
        {
            var filter = "&filter-is-primary=true&filter-type=" + type.ToString().Replace("_", "-");
            return CallPrincipalList(filter);
        }


        private StatusInfo AddToGroup(string principalId, string groupId)
        {
            if (string.IsNullOrWhiteSpace(principalId))
                throw new ArgumentException("Non-empty value expected", nameof(principalId));
            if (string.IsNullOrWhiteSpace(groupId))
                throw new ArgumentException("Non-empty value expected", nameof(groupId));

            StatusInfo status;
            requestProcessor.Process(Commands.Principal.GroupMembershipUpdate, string.Format(CommandParams.GroupMembership, groupId, principalId, "true"), out status);
            return status;
        }

        public StatusInfo AddToGroup(IEnumerable<string> principalIds, string groupId)
        {
            if (principalIds == null)
                throw new ArgumentNullException(nameof(principalIds));
            if (principalIds.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("All Principal Ids should be non empty", nameof(principalIds));
            if (string.IsNullOrWhiteSpace(groupId))
                throw new ArgumentException("Non-empty value expected", nameof(groupId));

            StatusInfo status;

            var trios = new List<string>(principalIds.Count());
            var paramBuilder = new StringBuilder();
            foreach (string principalId in principalIds)
            {
                paramBuilder.Length = 0;
                paramBuilder.AppendFormat(CommandParams.GroupMembership, groupId, principalId, "true");
                trios.Add(paramBuilder.ToString());
            }

            this.requestProcessor.Process(Commands.Principal.GroupMembershipUpdate, string.Join("&", trios), out status);
            return status;
        }

        public StatusInfo AddToGroupByType(string principalId, PrincipalType type)
        {
            if (string.IsNullOrWhiteSpace(principalId))
                throw new ArgumentException("Non-empty value expected", nameof(principalId));

            var groups = GetGroupsByType(type);
            var group = groups.Values?.FirstOrDefault();
            return group != null ? AddToGroup(principalId, group.PrincipalId) : groups.Status;
        }

        public StatusInfo AddToGroupByType(IEnumerable<string> principalIds, PrincipalType type)
        {
            if (principalIds == null)
                throw new ArgumentNullException(nameof(principalIds));
            if (principalIds.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("All Principal Ids should be non empty", nameof(principalIds));

            var groups = GetGroupsByType(type);
            var group = groups.Values?.FirstOrDefault();
            return group != null ? AddToGroup(principalIds, group.PrincipalId) : groups.Status;
        }


        public StatusInfo RemoveFromGroup(string principalId, string groupId)
        {
            if (string.IsNullOrWhiteSpace(principalId))
                throw new ArgumentException("Non-empty value expected", nameof(principalId));
            if (string.IsNullOrWhiteSpace(groupId))
                throw new ArgumentException("Non-empty value expected", nameof(groupId));

            StatusInfo status;
            requestProcessor.Process(Commands.Principal.GroupMembershipUpdate, string.Format(CommandParams.GroupMembership, groupId, principalId, "false"), out status);
            return status;
        }

        public bool RemoveFromGroupByType(string principalId, PrincipalType type)
        {
            if (string.IsNullOrWhiteSpace(principalId))
                throw new ArgumentException("Non-empty value expected", nameof(principalId));

            var group = GetGroupsByType(type).Values?.FirstOrDefault();
            if (group != null)
            {
                return ResponseIsOk(RemoveFromGroup(principalId, group.PrincipalId));
            }

            return false;
        }

    }

}
