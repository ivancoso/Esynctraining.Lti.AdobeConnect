using System;
using Esynctraining.AC.Provider.Constants;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.EntityParsing;
using Esynctraining.AC.Provider.Utils;

namespace Esynctraining.AC.Provider
{
    public partial class AdobeConnectProvider
    {
        public SingleObjectResult<CustomField> GetCustomField(string customFieldName)
        {
            if (string.IsNullOrWhiteSpace(customFieldName))
                throw new ArgumentException("Non-empty value expected", nameof(customFieldName));

            // act: "custom-fields"
            return GetResult(Commands.CustomField.CustomFields,
                string.Format(CommandParams.CustomFields.FilterByName, UrlEncode(customFieldName)), 
                "//custom-fields/field",
                ParserSingleton<CustomFieldParser>.Instance);
        }

        public SingleObjectResult<CustomField> CustomFieldUpdate(CustomField value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var commandParams = QueryStringBuilder.EntityToQueryString(value);

            return GetResult(Commands.CustomField.Update,
                commandParams,
                "//field",
                ParserSingleton<CustomFieldParser>.Instance);
        }

        public StatusInfo CustomFieldDelete(string customFieldName)
        {
            if (string.IsNullOrWhiteSpace(customFieldName))
                throw new ArgumentException("Non-empty value expected", nameof(customFieldName));

            SingleObjectResult<CustomField> field = GetCustomField(customFieldName);
            // NOTE: field not found - ok for us. OK?
            if (field.Status.Code == StatusCodes.ok && field.Value == null)
                return field.Status;

            StatusInfo status;
            var commandParams = string.Format(CommandParams.CustomFields.Delete, UrlEncode(field.Value.FieldId), field.Value.ObjectType.ToString().Replace("_", "-"));
            var doc = requestProcessor.Process(Commands.CustomField.Delete, commandParams, out status);
            return status;
        }

    }

}
