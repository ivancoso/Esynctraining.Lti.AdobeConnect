using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Esynctraining.AC.Provider.Constants;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.EntityParsing;
using Esynctraining.AC.Provider.Extensions;
using Esynctraining.AC.Provider.Utils;

namespace Esynctraining.AC.Provider
{
    public partial class AdobeConnectProvider
    {
        /// <summary>
        /// Provides a complete list of users and groups, including primary groups.
        /// </summary>
        /// <returns>
        /// The <see cref="PrincipalCollectionResult"/>.
        /// </returns>
        public PrincipalCollectionResult GetAllPrincipals()
        {
            return CallPrincipalList(string.Empty);
        }

        public PrincipalInfoResult GetOneByPrincipalId(string principalId)
        {
            if (string.IsNullOrWhiteSpace(principalId))
                throw new ArgumentException("Non-empty value expected", nameof(principalId));

            // act: "principal-info"
            StatusInfo status;

            var principalInfo = this.requestProcessor.Process(Commands.Principal.Info, string.Format(CommandParams.PrincipalId, principalId), out status);

            return ResponseIsOk(principalInfo, status)
                ? new PrincipalInfoResult(status, PrincipalInfoParser.Parse(principalInfo))
                : new PrincipalInfoResult(status);
        }
        
        /// <summary>
        /// Provides a list of users by email
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="PrincipalCollectionResult"/>.
        /// </returns>
        public PrincipalCollectionResult GetAllByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Non-empty value expected", nameof(email));

            var filter = string.Format(CommandParams.PrincipalByEmail, UrlEncode(email));
            return CallPrincipalList(filter);
        }

        public PrincipalCollectionResult GetAllByEmail(IEnumerable<string> emails)
        {
            if (emails == null)
                throw new ArgumentNullException(nameof(emails));
            if (!emails.Any())
                throw new ArgumentException("Emails list should have values", nameof(emails));
            if (emails.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("All emails should be non empty", nameof(emails));

            var trios = new List<string>(emails.Count());
            var paramBuilder = new StringBuilder();
            foreach (string email in emails)
            {
                paramBuilder.Length = 0;
                paramBuilder.AppendFormat(CommandParams.PrincipalByEmail, UrlEncode(email));
                trios.Add(paramBuilder.ToString());
            }

            var filter = string.Join("&", trios);
            return CallPrincipalList(filter);
        }

        /// <summary>
        /// Provides a list of users by login.
        /// </summary>
        /// <param name="login">
        /// The login.
        /// </param>
        /// <returns>
        /// The <see cref="PrincipalCollectionResult"/>.
        /// </returns>
        public PrincipalCollectionResult GetAllByLogin(string login)
        {
            var filter = string.Format(CommandParams.PrincipalByLogin, UrlEncode(login));

            return CallPrincipalList(filter);
        }

        public PrincipalCollectionResult GetAllByLogin(IEnumerable<string> logins)
        {
            if (logins == null)
                throw new ArgumentNullException(nameof(logins));
            if (!logins.Any())
                throw new ArgumentException("Logins list should have values", nameof(logins));
            if (logins.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("All logins should be non empty", nameof(logins));

            var trios = new List<string>(logins.Count());
            var paramBuilder = new StringBuilder();
            foreach (string login in logins)
            {
                paramBuilder.Length = 0;
                paramBuilder.AppendFormat(CommandParams.PrincipalByLogin, UrlEncode(login));
                trios.Add(paramBuilder.ToString());
            }

            var filter = string.Join("&", trios);
            return CallPrincipalList(filter);
        }

        public PrincipalCollectionResult GetAllByFieldLike(string fieldName, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                throw new ArgumentException("Non-empty value expected", nameof(fieldName));

            //http://dev.connectextensions.com/api/xml?action=principal-list&filter-like-login=@esynctraining&filter-like-name=sergey

            var filter = string.Format(CommandParams.PrincipalByFieldLike, fieldName, UrlEncode(searchTerm));
            return CallPrincipalList(filter);
        }

        public PrincipalCollectionResult GetAllByPrincipalIds(string[] principalIdsToFind)
        {
            if (principalIdsToFind == null)
                throw new ArgumentNullException(nameof(principalIdsToFind));
            if (!principalIdsToFind.Any())
                throw new ArgumentException("Principal Id list should have values", nameof(principalIdsToFind));
            if (principalIdsToFind.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("All principalIds should be non empty", nameof(principalIdsToFind));

            // act: "principal-list"
            // /api/xml?action=principal-list&filter-principal-id=AAA&filter-principal-id=BBB&filter-principal-id=CCC

            var parameters = new List<string>(principalIdsToFind.Count());
            var paramBuilder = new StringBuilder();
            foreach (string principalId in principalIdsToFind)
            {
                paramBuilder.Length = 0;
                paramBuilder.AppendFormat(CommandParams.PrincipalByPrincipalId, principalId);
                parameters.Add(paramBuilder.ToString());
            }

            var filter = string.Join("&", parameters);
            return CallPrincipalList(filter);
        }

        /// <summary>
        /// Gets all group's *user* principals. 
        /// Doesn't return groups!
        /// </summary>
        /// <param name="groupId">
        /// The group id.
        /// </param>
        /// <returns>
        /// The <see cref="PrincipalCollectionResult"/>.
        /// </returns>
        public PrincipalCollectionResult GetGroupPrincipalUsers(string groupId)
        {
            if (string.IsNullOrWhiteSpace(groupId))
                throw new ArgumentException("Non-empty value expected", nameof(groupId));

            string filter = string.Format(CommandParams.PrincipalGroupIdUsersOnly, groupId);
            return CallPrincipalList(filter);
        }

        //todo:rename or create separate method for filter-type=userS
        /// <summary>
        /// Gets principal with passed principal Id when this principal is member of the specified Group.
        /// </summary>
        /// <param name="groupId">
        /// The group id.
        /// </param>
        /// <param name="principalId">
        /// The principal id.
        /// </param>
        /// <returns>
        /// The <see cref="PrincipalCollectionResult"/>.
        /// </returns>
        public PrincipalCollectionResult GetGroupPrincipalUsers(string groupId, string principalId)
        {
            if (string.IsNullOrWhiteSpace(groupId))
                throw new ArgumentException("Group ID can't be empty", nameof(groupId));
            if (string.IsNullOrWhiteSpace(principalId))
                throw new ArgumentException("Principal ID can't be empty", nameof(principalId));

            string filter = string.Format(CommandParams.PrincipalGroupIdPrincipalId, groupId, principalId);
            return CallPrincipalList(filter);
        }

        /// <summary>
        /// Gets all group principals. Both users and groups.
        /// </summary>
        /// <param name="groupId">
        /// The group id.
        /// </param>
        /// <returns>
        /// The <see cref="PrincipalCollectionResult"/>.
        /// </returns>
        public PrincipalCollectionResult GetGroupUsers(string groupId)
        {
            string filter = $"&group-id={groupId}&filter-is-member=true";
            return CallPrincipalList(filter);
        }

        /// <summary>
        /// Creates or updates a user or group. The user or group (that is, the principal) is created or
        /// updated in the same account as the user making the call.
        /// </summary>
        /// <param name="principalSetup">The principal setup.</param>
        /// <returns>Status Info.</returns>
        public PrincipalResult PrincipalUpdate(PrincipalSetup principalSetup, bool isUpdateOperation = false)
        {
            // action=principal-update
            var commandParams = QueryStringBuilder.EntityToQueryString(principalSetup, isUpdateOperation);

            StatusInfo status;
            var doc = requestProcessor.Process(Commands.Principal.Update, commandParams, out status);

            return new PrincipalResult(status, PrincipalParser.Parse(doc));
        }

        /// <summary>
        /// Creates or updates a user or group. The user or group (that is, the principal) is created or
        /// updated in the same account as the user making the call.
        /// </summary>
        /// <param name="principalId">
        /// The principal Id.
        /// </param>
        /// <param name="newPassword">
        /// The new Password.
        /// </param>
        /// <returns>
        /// Status Info.
        /// </returns>
        public GenericResult PrincipalUpdatePassword(string principalId, string newPassword)
        {
            StatusInfo status;
            var parameters = string.Format(
                CommandParams.PrincipalUpdatePassword,
                principalId,
                UrlEncode(newPassword),
                UrlEncode(newPassword));
            requestProcessor.Process(Commands.Principal.UpdatePassword, parameters, out status);

            return new GenericResult(status);
        }

        public GenericResult PrincipalUpdateType(string principalId, PrincipalType type)
        {
            StatusInfo status;
            var parameters = string.Format(
                CommandParams.PrincipalUpdateType,
                principalId,
                type.ToXmlString());
            requestProcessor.Process(Commands.Principal.UpdateType, parameters, out status);
            /*
<?xml version="1.0" encoding="ISO-8859-1"?>
<results>
    <status code="ok"/>
    <update-principal-type type="user" principal-id="158343788"/>
</results>
             */
            return new GenericResult(status);
        }

        /// <summary>
        /// Creates or updates a user or group. The user or group (that is, the principal) is created or
        /// updated in the same account as the user making the call.
        /// </summary>
        /// <param name="principalDelete">The principal setup.</param>
        /// <returns>Status Info.</returns>
        public PrincipalResult PrincipalDelete(PrincipalDelete principalDelete)
        {
            // action=principals-delete
            var commandParams = QueryStringBuilder.EntityToQueryString(principalDelete);

            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.Principal.Delete, commandParams, out status);

            return new PrincipalResult(status, PrincipalParser.Parse(doc));
        }

        public PrincipalCollectionResult GetAllByEmailAndType(string email, PrincipalType principalType)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Non-empty value expected", nameof(email));

            var filter = string.Format(CommandParams.PrincipalByEmailAndType, UrlEncode(email), principalType.ToXmlString());

            return CallPrincipalList(filter);
        }

        private PrincipalCollectionResult CallPrincipalList(string filter)
        {
            return DoCallPrincipalList(filter, 0, Int32.MaxValue);
        }

        private PrincipalCollectionResult DoCallPrincipalList(string filter, int startIndex, int limit)
        {
            // act: "principal-list"
            StatusInfo status;

            var principals = this.requestProcessor.Process(Commands.Principal.List, filter.AppendPagingIfNeeded(startIndex, limit), out status);

            IEnumerable<Principal> data = PrincipalCollectionParser.Parse(principals);
            bool okResponse = ResponseIsOk(principals, status);

            if (!okResponse)
            {
                if (status.Code == StatusCodes.operation_size_error)
                {
                    int? actualAcLimit = status.TryGetSubCodeAsInt32();
                    if (actualAcLimit.HasValue)
                    {
                        return DoCallPrincipalList(filter + "&sort-principal-id=asc", startIndex, actualAcLimit.Value);
                    }
                }
                return new PrincipalCollectionResult(status);
            }

            if (data.Count() < limit)
                return new PrincipalCollectionResult(status, data);

            PrincipalCollectionResult nextPage = DoCallPrincipalList(filter, startIndex + limit, limit);
            if (!nextPage.Success)
                return nextPage;

            return new PrincipalCollectionResult(status, data.Concat(nextPage.Values));
        }
        
    }

}
