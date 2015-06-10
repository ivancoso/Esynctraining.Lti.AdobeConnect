using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Extensions;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public sealed class AdobeConnectAccountService : IAdobeConnectAccountService
    {
        private readonly ILogger _logger;


        public AdobeConnectAccountService(ILogger logger)
        {
            _logger = logger;
        }


        public ACPasswordPoliciesDTO GetPasswordPolicies(AdobeConnectProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            StatusInfo status;
            UserInfo usr = provider.GetUserInfo(out status);

            if (status.Code != StatusCodes.ok)
            {
                _logger.ErrorFormat("GetPasswordPolicies.GetUserInfo. AC error. Code:{0}.SubCode:{1}.", status.Code, status.SubCode);
                return null;
            }

            if (usr.AccountId.HasValue)
            {
                FieldCollectionResult fields = provider.GetAclFields(usr.AccountId.Value);

                if (fields.Status.Code != StatusCodes.ok)
                {
                    _logger.ErrorFormat("GetPasswordPolicies.GetAclFields. AC error. Code:{0}.SubCode:{1}.", fields.Status.Code, fields.Status.SubCode);
                    return null;
                }

                //bool loginSameAsEmail = fields.Values.First(x => x.FieldId == "login-same-as-email").Value.Equals("YES", StringComparison.OrdinalIgnoreCase);
                bool passwordRequiresDigit = "YES".Equals(GetField(fields, "password-requires-digit"), StringComparison.OrdinalIgnoreCase);
                bool passwordRequiresCapitalLetter = "YES".Equals(GetField(fields, "password-requires-capital-letter"), StringComparison.OrdinalIgnoreCase);
                string passwordRequiresSpecialChars = GetField(fields, "password-requires-special-chars");

                int passwordMinLength = int.Parse(GetField(fields, "password-min-length") ?? "4");
                int passwordMaxLength = int.Parse(GetField(fields, "password-max-length") ?? "32");

                return new ACPasswordPoliciesDTO 
                {
                    passwordRequiresDigit = passwordRequiresDigit,
                    passwordRequiresCapitalLetter = passwordRequiresCapitalLetter,
                    passwordRequiresSpecialChars = passwordRequiresSpecialChars,
                    passwordMinLength = passwordMinLength,
                    passwordMaxLength = passwordMaxLength,
                };
            }

            _logger.Error("GetPasswordPolicies. Account is NULL");
            return null;
        }

        public IEnumerable<PrincipalReportDto> GetMeetingHostReport(AdobeConnectProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            var group = provider.GetGroupsByType("live-admins");
            if (group.Item1.Code != StatusCodes.ok)
                throw new InvalidOperationException("AC.GetGroupsByType error");

            PrincipalCollectionResult usersResult = provider.GetGroupUsers(group.Item2.First().PrincipalId);
            if (usersResult.Status.Code != StatusCodes.ok)
                throw new InvalidOperationException("AC.GetGroupUsers error");

            var result = new List<PrincipalReportDto>();
            foreach (Principal user in usersResult.Values)
            {
                var item = new PrincipalReportDto 
                {
                    Principal = PrincipalDto.Build(user),
                };

                TransactionCollectionResult trxResult = provider.ReportMeetingTransactionsForPrincipal(user.PrincipalId, startIndex: 0, limit: 1);

                if (trxResult.Status.Code != StatusCodes.ok)
                    throw new InvalidOperationException("AC.ReportMeetingTransactionsForPrincipal error");

                TransactionInfo trx = trxResult.Values.FirstOrDefault();

                if (trx != null)
                    item.LastTransaction = TransactionInfoDto.Build(trx);

                result.Add(item);
            }

            return result.AsReadOnly();
        }

        private static string GetField(FieldCollectionResult value, string fieldName)
        {
            Field field = value.Values.FirstOrDefault(x => x.FieldId == fieldName);
            if (field == null)
            {
                return null;
            }

            return field.Value;
        }

    }

}