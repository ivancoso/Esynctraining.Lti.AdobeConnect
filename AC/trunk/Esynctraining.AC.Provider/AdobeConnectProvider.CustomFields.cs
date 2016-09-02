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
            StatusInfo status;

            var doc = this.requestProcessor.Process(Commands.CustomField.CustomFields, 
                string.Format(CommandParams.CustomFields.FilterByName, UrlEncode(customFieldName)),
                out status);

            return GetResult(doc, status, "//custom-fields/field", ParserSingleton<CustomFieldParser>.Instance);
        }

        public SingleObjectResult<CustomField> CustomFieldUpdate(CustomField value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            // action=principal-update
            var commandParams = QueryStringBuilder.EntityToQueryString(value);

            StatusInfo status;
            var doc = requestProcessor.Process(Commands.CustomField.Update, commandParams, out status);

            return GetResult(doc, status, "//field", ParserSingleton<CustomFieldParser>.Instance);
        }


    }

}
