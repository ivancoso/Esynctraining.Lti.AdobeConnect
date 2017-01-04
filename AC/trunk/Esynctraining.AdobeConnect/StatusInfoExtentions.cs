using System;
using System.Text;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AdobeConnect
{
    public static class StatusInfoExtentions
    {
        public static string GetErrorInfo(this StatusInfo value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (value.Code == StatusCodes.ok)
                throw new ArgumentException("Error status expected", nameof(value));

            var msg = new StringBuilder(80);
            msg.AppendFormat("Status.Code: {0}. Status.SubCode: {1}. ", value.Code, value.SubCode);

            if (value.UnderlyingExceptionInfo != null)
            {
                msg.AppendFormat("Status.UnderlyingExceptionInfo: {0}.", value.UnderlyingExceptionInfo.Message);
            }

            if (!string.IsNullOrEmpty(value.InvalidField))
            {
                msg.AppendFormat("Invalid Field: {0}.", value.InvalidField);
            }

            return msg.ToString();
        }

    }

}
