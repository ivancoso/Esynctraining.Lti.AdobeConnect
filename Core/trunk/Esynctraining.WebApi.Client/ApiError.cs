using System.Collections.Generic;
using System.Text;

namespace Esynctraining.WebApi.Client
{
    public class ApiError
    {
        public string message { get; set; }

        public Dictionary<string, string[]> modelState { get; set; }

        public override string ToString()
        {
            var txt = new StringBuilder(message);
            if (modelState != null)
            {
                txt.Append(" Details: ");
                foreach (var msgSet in modelState.Values)
                    foreach (string msg in msgSet)
                    {
                        txt.Append(msg);
                        txt.Append(" ");
                    }
            }

            return txt.ToString();
        }

    }

}
