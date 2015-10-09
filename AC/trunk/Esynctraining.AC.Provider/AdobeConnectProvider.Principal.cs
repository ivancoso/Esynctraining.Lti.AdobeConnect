using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Esynctraining.AC.Provider.Constants;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.EntityParsing;

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
            return this.GetGroupPrincipalUsers(null);
        }

        /// <summary>
        /// Provides a list of users by email
        /// </summary>
        /// <param name="principalId">
        /// The principal Id.
        /// </param>
        /// <returns>
        /// The <see cref="PrincipalCollectionResult"/>.
        /// </returns>
        public PrincipalInfoResult GetOneByPrincipalId(string principalId)
        {
            // act: "principal-info"
            StatusInfo status;

            var principalInfo = this.requestProcessor.Process(Commands.Principal.Info, string.Format(CommandParams.PrincipalId, principalId), out status);

            return ResponseIsOk(principalInfo, status)
                ? new PrincipalInfoResult(status, PrincipalInfoParser.Parse(principalInfo))
                : new PrincipalInfoResult(status);
        }

        /// <summary>
        /// Provides a list of users.
        /// </summary>
        /// <returns>
        /// The <see cref="PrincipalCollectionResult"/>.
        /// </returns>
        public PrincipalCollectionResult GetAllPrincipal()
        {
            // act: "principal-list"
            StatusInfo status;

            var principals = this.requestProcessor.Process(Commands.Principal.List, string.Empty, out status);

            return ResponseIsOk(principals, status)
                ? new PrincipalCollectionResult(status, PrincipalCollectionParser.Parse(principals))
                : new PrincipalCollectionResult(status);
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
            // act: "principal-list"
            StatusInfo status;

            var principals = this.requestProcessor.Process(
                Commands.Principal.List,
                string.Format(CommandParams.PrincipalByEmail, HttpUtility.UrlEncode(email)),
                out status);

            return ResponseIsOk(principals, status)
                ? new PrincipalCollectionResult(status, PrincipalCollectionParser.Parse(principals))
                : new PrincipalCollectionResult(status);
        }

        public PrincipalCollectionResult GetAllByEmail(IEnumerable<string> emails)
        {
            // act: "principal-list"
            StatusInfo status;

            var trios = new List<string>(emails.Count());
            var paramBuilder = new StringBuilder();
            foreach (string email in emails)
            {
                paramBuilder.Length = 0;
                paramBuilder.AppendFormat(CommandParams.PrincipalByEmail, HttpUtility.UrlEncode(email));
                trios.Add(paramBuilder.ToString());
            }

            var principals = this.requestProcessor.Process(Commands.Principal.List, string.Join("&", trios), out status);

            return ResponseIsOk(principals, status)
                ? new PrincipalCollectionResult(status, PrincipalCollectionParser.Parse(principals))
                : new PrincipalCollectionResult(status);
        }

        /// <summary>
        /// Provides a list of users by email
        /// </summary>
        /// <param name="login">
        /// The login.
        /// </param>
        /// <returns>
        /// The <see cref="PrincipalCollectionResult"/>.
        /// </returns>
        public PrincipalCollectionResult GetAllByLogin(string login)
        {
            // act: "principal-list"
            StatusInfo status;

            var principals = this.requestProcessor.Process(
                Commands.Principal.List,
                string.Format(CommandParams.PrincipalByLogin, HttpUtility.UrlEncode(login)),
                out status);

            return ResponseIsOk(principals, status)
                ? new PrincipalCollectionResult(status, PrincipalCollectionParser.Parse(principals))
                : new PrincipalCollectionResult(status);
        }

        public PrincipalCollectionResult GetAllByLogin(IEnumerable<string> logins)
        {
            // act: "principal-list"
            StatusInfo status;

            var trios = new List<string>(logins.Count());
            var paramBuilder = new StringBuilder();
            foreach (string login in logins)
            {
                paramBuilder.Length = 0;
                paramBuilder.AppendFormat(CommandParams.PrincipalByLogin, HttpUtility.UrlEncode(login));
                trios.Add(paramBuilder.ToString());
            }

            var principals = this.requestProcessor.Process(Commands.Principal.List, string.Join("&", trios), out status);

            return ResponseIsOk(principals, status)
                ? new PrincipalCollectionResult(status, PrincipalCollectionParser.Parse(principals))
                : new PrincipalCollectionResult(status);
        }

        public PrincipalCollectionResult GetAllByFieldLike(string fieldName, string searchTerm)
        {
            // act: "principal-list"
            //http://dev.connectextensions.com/api/xml?action=principal-list&filter-like-login=@esynctraining&filter-like-name=sergey
            StatusInfo status;

            var principals = this.requestProcessor.Process(
                Commands.Principal.List,
                string.Format(CommandParams.PrincipalByFieldLike, fieldName, HttpUtility.UrlEncode(searchTerm)),
                out status);

            return ResponseIsOk(principals, status)
                ? new PrincipalCollectionResult(status, PrincipalCollectionParser.Parse(principals))
                : new PrincipalCollectionResult(status);
        }

        public PrincipalCollectionResult GetAllByPrincipalIds(string[] principalIdsToFind)
        {
            if (principalIdsToFind == null)
                throw new ArgumentNullException("principalIdsToFind");

            // act: "principal-list"
            // /api/xml?action=principal-list&filter-principal-id=AAA&filter-principal-id=BBB&filter-principal-id=CCC
            StatusInfo status;

            var parameters = new List<string>(principalIdsToFind.Count());
            var paramBuilder = new StringBuilder();
            foreach (string principalId in principalIdsToFind)
            {
                paramBuilder.Length = 0;
                paramBuilder.AppendFormat(CommandParams.PrincipalByPrincipalId, principalId);
                parameters.Add(paramBuilder.ToString());
            }

            var principals = this.requestProcessor.Process(
                Commands.Principal.List,
                string.Join("&", parameters),
                out status);

            return ResponseIsOk(principals, status)
                ? new PrincipalCollectionResult(status, PrincipalCollectionParser.Parse(principals))
                : new PrincipalCollectionResult(status);
        }

        /// <summary>
        /// Gets all principals if no Group Id specified.
        /// Otherwise gets only users of the specified Group.
        /// </summary>
        /// <param name="groupId">
        /// The group id.
        /// </param>
        /// <returns>
        /// The <see cref="PrincipalCollectionResult"/>.
        /// </returns>
        public PrincipalCollectionResult GetGroupPrincipalUsers(string groupId)
        {
            // act: "principal-list"
            StatusInfo status;

            var principals = this.requestProcessor.Process(Commands.Principal.List, string.IsNullOrWhiteSpace(groupId) ? null : string.Format(CommandParams.PrincipalGroupIdUsersOnly, groupId), out status);

            return ResponseIsOk(principals, status)
                ? new PrincipalCollectionResult(status, PrincipalCollectionParser.Parse(principals))
                : new PrincipalCollectionResult(status);
        }

        /// <summary>
        /// Gets all principals if no Group Id specified.
        /// Otherwise gets only users of the specified Group.
        /// </summary>
        /// <param name="groupId">
        /// The group id.
        /// </param>
        /// <returns>
        /// The <see cref="PrincipalCollectionResult"/>.
        /// </returns>
        public PrincipalCollectionResult GetGroupUsers(string groupId)
        {
            // act: "principal-list"
            StatusInfo status;

            var principals = this.requestProcessor.Process(Commands.Principal.List, string.Format("&group-id={0}&filter-is-member=true", groupId), out status);

            return ResponseIsOk(principals, status)
                ? new PrincipalCollectionResult(status, PrincipalCollectionParser.Parse(principals))
                : new PrincipalCollectionResult(status);
        }

    }

}
