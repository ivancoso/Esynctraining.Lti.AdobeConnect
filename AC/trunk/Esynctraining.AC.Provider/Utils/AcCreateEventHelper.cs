using System;
using System.Collections.Generic;
using Esynctraining.AC.Provider.DataObjects;

namespace Esynctraining.AC.Provider.Utils
{
    public class AcCreateEventHelper
    {
        public static Dictionary<string, string> GetPostFormFields(SaveEventFields saveEventFields, string owasp)
        {
            var result = new Dictionary<string, string>();

            result.Add("field-id=event-template,value", saveEventFields.EventTemplateId);
            result.Add("name", saveEventFields.Name);
            result.Add("event-type", saveEventFields.EventType);
            result.Add("event-category", saveEventFields.EventCategory);
            result.Add("list-sco-id", saveEventFields.ListScoId);
            result.Add("owner-permission-id", saveEventFields.OwnerPermissionId);
            result.Add("url-path", saveEventFields.UrlPath);
            result.Add("field-id=event-info,value", saveEventFields.EventInfo);
            result.Add("description", saveEventFields.Description);
            result.Add("default:password-bypass", saveEventFields.PasswordByPass.ToString().ToLower());
            result.Add("default:principal-id=logged-in-access,permission-id", saveEventFields.LoggedInAccess);
            result.Add("catalog-view", saveEventFields.CatalogView);
            result.Add("default:catalog-view", saveEventFields.DefaultCatalogView);
            result.Add("show-in-catalog", saveEventFields.ShowInCatalog.ToString().ToLower());
            result.Add("default:registration-type", saveEventFields.DefaultRegistrationType);

            if (saveEventFields.StartDate != DateTime.MinValue && saveEventFields.StartDate != DateTime.MaxValue)
            {
                result.Add("day:date-begin", saveEventFields.StartDate.Day.ToString().ToLower());
                result.Add("month:date-begin", saveEventFields.StartDate.Month.ToString().ToLower());
                result.Add("year:date-begin", saveEventFields.StartDate.Year.ToString().ToLower());
                result.Add("hhmm:date-begin", saveEventFields.StartDate.ToString("HH:mm"));
            }
            if (saveEventFields.EndDate != DateTime.MinValue && saveEventFields.EndDate != DateTime.MaxValue)
            {
                result.Add("day:date-end", saveEventFields.EndDate.Day.ToString().ToLower());
                result.Add("month:date-end", saveEventFields.EndDate.Month.ToString().ToLower());
                result.Add("year:date-end", saveEventFields.EndDate.Year.ToString().ToLower());
                result.Add("hhmm:date-end", saveEventFields.EndDate.ToString("HH:mm"));
            }

            result.Add("time-zone-id", saveEventFields.TimeZoneId.ToString().ToLower());
            result.Add("override:lang", saveEventFields.Lang);
            result.Add("default:is-registration-limit-enabled", saveEventFields.RegistrationLimitEnabled.ToString().ToLower());
            result.Add("default:tag-id=11041,enable-tag", saveEventFields.Tag11041.ToString().ToLower());
            result.Add("default:tag-id=11038,enable-tag", saveEventFields.Tag11038.ToString().ToLower());
            result.Add("default:tag-id=11039,enable-tag", saveEventFields.Tag11039.ToString().ToLower());
            result.Add("default:tag-id=11040,enable-tag", saveEventFields.Tag11040.ToString().ToLower());

            //result.Add("feature=FZEk4ljHCRBv7fhQ9Lmd4Q__", saveEventFields.Feature);
            result.Add("OWASP_CSRFTOKEN", owasp);

            return result;
        }
    }
}