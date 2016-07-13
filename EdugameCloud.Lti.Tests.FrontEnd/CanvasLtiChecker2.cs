using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json;

namespace EdugameCloud.Lti.Tests.FrontEnd
{
    public sealed class CanvasLtiChecker2
    {
        private readonly string _configFolder;
        private readonly string _loginUrl;
        private readonly string _sharedSecret;
        private int _meetingId = -1;


        public CanvasLtiChecker2(string configFolder, string loginUrl, string sharedSecret)
        {
            _configFolder = configFolder;
            _loginUrl = loginUrl;
            _sharedSecret = sharedSecret;
        }


        public IEnumerable<string> DoCheckRequests()
        {
            var result = new List<string>();
            string output = DoLogin(result);

            string redirectUrl = null;
            var url = Regex.Match(output, "Object moved to <a href=\\\"(?<redirectUrl>[^\"]+)\\\"");
            if (url.Success)
            {
                redirectUrl = url.Groups["redirectUrl"].Value;
            }

            var parsing = Regex.Match(output, "Object moved to <a href=\\\"/lti/extjs-entry[?]primaryColor=(?<color>[A-Z-a-z0-9]+)[&][a]mp;lmsProviderName=(?<session>[A-Fa-f0-9]{8}(?:-[A-Fa-f0-9]{4}){3}-[A-Fa-f0-9]{12})[&][a]mp;");

            if (parsing.Success)
            {
                result.Add("canvas-login: SUCCESS-SUCCESS-SUCCESS."); // -Returns 302 Http status code to 'extjs/entry' page
            }
            else
            {
                result.Add("canvas-login: ERROR." + ((HttpContext.Current.IsDebuggingEnabled) ? " Output = " + output : ""));
            }
            
            return result;
        }

        private string DoLogin(List<string> result)
        {
            var jsData = Path.Combine(_configFolder, "login-data.js");

            Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(jsData));
            var nameValueCollection = new NameValueCollection();

            foreach (var kvp in values)
            {
                string value = null;
                if (kvp.Value != null)
                    value = kvp.Value;

                nameValueCollection.Add(kvp.Key, value);
            }
            
            double secondsSince1970 = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            nameValueCollection.Add("oauth_timestamp", secondsSince1970.ToString());
            nameValueCollection.Add("oauth_nonce", Guid.NewGuid().ToString()); // "XXw271C51BRTLPNaYyWXh3Y9fWU3OXX63OP1BejmCAkAzA"

            string signature = BltiBuilder.Calculate(nameValueCollection, host: new Uri(_loginUrl).Host, sharedSecret: _sharedSecret);
            nameValueCollection.Add("oauth_signature", signature);

            var pairs = new List<string>();
            foreach (string key in nameValueCollection)
            {
                string value = nameValueCollection[key];
                pairs.Add(string.Format("{0}={1}", key, Uri.EscapeDataString(value)));
            }

            var data = Encoding.ASCII.GetBytes(string.Join("&", pairs));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_loginUrl);
            request.AllowAutoRedirect = false;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";


            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                return new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
        }
        
        //private static string Format(string filename, string arguments)
        //{
        //    return "'" + filename +
        //        ((string.IsNullOrEmpty(arguments)) ? string.Empty : " " + arguments) +
        //        "'";
        //}


    }

}
