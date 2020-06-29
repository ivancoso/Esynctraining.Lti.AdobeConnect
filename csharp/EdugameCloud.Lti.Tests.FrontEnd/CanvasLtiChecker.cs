using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace EdugameCloud.Lti.Tests.FrontEnd
{
    public sealed class CanvasLtiChecker
    {
        private readonly string _curlExeFullPath;
        private readonly string _configFolder;
        private int _meetingId = -1;


        public CanvasLtiChecker(string curlExeFullPath, string configFolder)
        {
            _curlExeFullPath = curlExeFullPath;
            _configFolder = configFolder;
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
                List<MeetingDTO> meetings = GetEntryPageAndParseMeetings(redirectUrl);
                if ((meetings != null) && meetings.Any(x => x.type == 1))
                    _meetingId = meetings.First(x => x.type == 1).id; //Meeting
            }

            var parsing = Regex.Match(output, "Object moved to <a href=\\\"/extjs/entry[?]primaryColor=(?<color>[A-Z-a-z0-9]+)[&][a]mp;lmsProviderName=(?<session>[A-Fa-f0-9]{8}(?:-[A-Fa-f0-9]{4}){3}-[A-Fa-f0-9]{12})[&][a]mp;");

            if (parsing.Success)
            {
                result.Add("canvas-login: SUCCESS - Returns 302 Http status code to 'extjs/entry' page.");
            }
            else
            {
                result.Add("canvas-login: ERROR.");
            }

            if (parsing.Success)
            {
                Guid sessionToken;
                if (Guid.TryParse(parsing.Groups["session"].Value, out sessionToken))
                {
                    DoCall("Lti/Template/GetAll", "templates.cfg", sessionToken, result);

                    if (_meetingId > 0)
                    {
                        DoCall("Lti/Recording/GetAll", "recordings.cfg", sessionToken, result);
                        DoCall("Lti/Meeting/Attendance", "reports-attendance.cfg", sessionToken, result);
                        DoCall("Lti/Meeting/Sessions", "reports-sessions.cfg", sessionToken, result);
                        string usersOutput = DoCall("Lti/User/GetAll", "users.cfg", sessionToken, result);

                        // TODO: do it with newly created meeting!!
                        // NOTE: set Mike Kollen as teacher - if it is not in list
                        DoCall("Lti/Meeting/SetDefaultACRoles", "users-sync.cfg", sessionToken, result);

                        if (!string.IsNullOrWhiteSpace(usersOutput))
                        {
                            List<LmsUserDTO> users = ParseUsers(usersOutput);
                            LmsUserDTO user = users.FirstOrDefault(x => x.name == "Mike Kollen");
                            if (user != null)
                            {
                                CallChangeRole(sessionToken, user, "Presenter", result);
                                CallChangeRole(sessionToken, user, "Host", result);
                                CallRemoveFromMeeting(sessionToken, user, result);
                            }
                        }
                    }
                    else
                    {
                        result.Add("=No meetings found=");
                    }

                    CreateMeeting(sessionToken, result);

                    List<MeetingDTO> meetings = GetEntryPageAndParseMeetings(redirectUrl);
                    MeetingDTO meetingToDelete = meetings.Single(x => x.name == "AutoCreatedMeeting");

                    DeleteMeeting(sessionToken, meetingToDelete.id, result);
                }
            }

            return result;
        }

        private void CallChangeRole(Guid sessionToken, LmsUserDTO user, string acRoleName, List<string> result)
        {
            try
            {
                string data = string.Format("ac_id={0}&ac_role={1}&lms_role={2}&id=1&name=Mike+Kollen&is_editable=true&email=mike\"%\"40esynctraining.com&guest_id=&lmsProviderName={3}&meetingId={4}",
                    user.ac_id,
                    acRoleName,
                    user.lms_role,
                    sessionToken.ToString().ToLower(), 
                    _meetingId.ToString());
                string arguments = string.Format("--data \"{1}\" --config {0} -s",
                    Path.Combine(_configFolder, "users-update.cfg"),
                    data);
                var output = RunExternalExe(_curlExeFullPath, arguments);

                bool ok = output.StartsWith("HTTP/1.1 200 OK") && output.Contains("\"isSuccess\":true");
                //--data "lmsProviderName=9e1c80ed-d43a-42bf-8166-a5590053937f&meetingId=12697" 
                if (ok)
                {
                    result.Add("Lti/User/Update(to " + acRoleName+ "): SUCCESS: Returns 200 Http status code + 'isSuccess' JSON result");
                }
                else
                {
                    result.Add("Lti/User/Update: ERROR.");
                }
            }
            catch (Exception ex)
            {
                result.Add("Lti/User/Update: ERROR. Monitoring tool issue: " + ex.Message);
            }
        }

        private void CallRemoveFromMeeting(Guid sessionToken, LmsUserDTO user, List<string> result)
        {
            try
            {
                string data = string.Format("ac_id={0}&ac_role={1}&lms_role={2}&id=1&name=Mike+Kollen&is_editable=true&email=mike\"%\"40esynctraining.com&guest_id=&lmsProviderName={3}&meetingId={4}",
                    user.ac_id,
                    "",
                    user.lms_role,
                    sessionToken.ToString().ToLower(),
                    _meetingId.ToString());
                string arguments = string.Format("--data \"{1}\" --config {0} -s",
                    Path.Combine(_configFolder, "users-removefrommeeting.cfg"),
                    data);
                var output = RunExternalExe(_curlExeFullPath, arguments);

                bool ok = output.StartsWith("HTTP/1.1 200 OK") && output.Contains("\"isSuccess\":true");
                if (ok)
                {
                    result.Add("Lti/user/removefrommeeting: SUCCESS: Returns 200 Http status code + 'isSuccess' JSON result");
                }
                else
                {
                    result.Add("Lti/user/removefrommeeting: ERROR.");
                }
            }
            catch (Exception ex)
            {
                result.Add("Lti/user/removefrommeeting: ERROR. Monitoring tool issue: " + ex.Message);
            }
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
            //1448451144
            //nameValueCollection.Add("oauth_timestamp", "1448451144");
            nameValueCollection.Add("oauth_nonce", Guid.NewGuid().ToString()); // "XXw271C51BRTLPNaYyWXh3Y9fWU3OXX63OP1BejmCAkAzA"

            string signature = BltiBuilder.Calculate(nameValueCollection);
            nameValueCollection.Add("oauth_signature", signature);

            var pairs = new List<string>();
            foreach (string key in nameValueCollection)
            {
                string value = nameValueCollection[key];
                pairs.Add(string.Format("{0}={1}", key, Uri.EscapeDataString(value)));
            }

            string data = string.Join("&", pairs).Replace("%", "\"%\"");

            // http://stackoverflow.com/questions/18215389/how-do-i-measure-request-and-response-times-at-once-using-curl
            //            string arguments = string.Format("--config {0}  -w %{{time_connect}}:%{{time_starttransfer}}:%{{time_total}} -s --data \"{1}\"",
            string arguments = string.Format("--data \"{1}\" --config {0} -s",
                Path.Combine(_configFolder, "login.cfg"),
                data);
            return RunExternalExe(_curlExeFullPath, arguments);
        }

        private void CreateMeeting(Guid sessionToken, List<string> result)
        {
            try
            {
                var jsData = Path.Combine(_configFolder, "create-with-users-data.js");

                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(jsData));
                var pairs = new List<string>();
                foreach (var item in values)
                {
                    pairs.Add(string.Format("{0}={1}", item.Key, Uri.EscapeDataString(item.Value)));
                }
                pairs.Add(string.Format("{0}={1}", "lmsProviderName", Uri.EscapeDataString(sessionToken.ToString())));

                string data = string.Join("&", pairs).Replace("%", "\"%\"");

                string arguments = string.Format("--data \"{1}\" --config {0} -s",
                    Path.Combine(_configFolder, "create-with-users.cfg"),
                    data);
                var output = RunExternalExe(_curlExeFullPath, arguments);

                bool ok = output.StartsWith("HTTP/1.1 200 OK") && output.Contains("\"isSuccess\":true");
                //--data "lmsProviderName=9e1c80ed-d43a-42bf-8166-a5590053937f&meetingId=12697" 
                if (ok)
                {
                    result.Add("CreateMeeting: SUCCESS: Returns 200 Http status code + 'isSuccess' JSON result");
                }
                else
                {
                    result.Add("CreateMeeting: ERROR.");
                }
            }
            catch (Exception ex)
            {
                result.Add("CreateMeeting: ERROR. Monitoring tool issue: " + ex.Message);
            }
        }

        private void DeleteMeeting(Guid sessionToken, int meetingId, List<string> result)
        {
            try
            {
                //--data "lmsProviderName=35d1e9bb-290a-492c-ac12-a55b000bc8d1&meetingId=25154" 
                string data = string.Format("lmsProviderName={0}&meetingId={1}", sessionToken.ToString().ToLower(), meetingId.ToString());
                string arguments = string.Format("--data \"{1}\" --config {0} -s",
                    Path.Combine(_configFolder, "meeting-delete.cfg"),
                    data);
                var output = RunExternalExe(_curlExeFullPath, arguments);

                bool ok = output.StartsWith("HTTP/1.1 200 OK") && output.Contains("\"isSuccess\":true");
                //--data "lmsProviderName=9e1c80ed-d43a-42bf-8166-a5590053937f&meetingId=12697" 
                if (ok)
                {
                    result.Add("DeleteMeeting: SUCCESS: Returns 200 Http status code + 'isSuccess' JSON result");
                }
                else
                {
                    result.Add("DeleteMeeting: ERROR.");
                }
            }
            catch (Exception ex)
            {
                result.Add("DeleteMeeting: ERROR. Monitoring tool issue: " + ex.Message);
            }
        }

        private List<MeetingDTO> GetEntryPageAndParseMeetings(string redirectUrl)
        {
            string arguments = string.Format(" https://app.edugamecloud.com{0}", HttpUtility.HtmlDecode(redirectUrl));
            string page = RunExternalExe(_curlExeFullPath, arguments);

            var json = Regex.Match(page, "EsyncLti.meetings =(?<meetingsJson>[^;]+);");
            var jsonContent = json.Groups["meetingsJson"].Value;
            dynamic parsedMeetings = JsonConvert.DeserializeObject(jsonContent);
            return (parsedMeetings.meetings as JArray).ToObject<List<MeetingDTO>>();
        }

        private List<LmsUserDTO> ParseUsers(string output)
        {
            var json = Regex.Match(output, "{(?<usersJson>[^;]+)}$");
            var jsonContent = json.Groups["usersJson"].Value;
            dynamic parsedMeetings = JsonConvert.DeserializeObject("{" + jsonContent + "}");
            return (parsedMeetings.data as JArray).ToObject<List<LmsUserDTO>>();
        }

        private string DoCall(string callName, string config, Guid sessionToken, List<string> result)
        {
            string output = string.Empty;
            try
            {
                string arguments =
                    string.Format("--config {0} --data \"lmsProviderName={1}&meetingId={2}\" ",
                        Path.Combine(_configFolder, config),
                        sessionToken,
                        _meetingId);
                output = RunExternalExe(_curlExeFullPath, arguments);
                
                bool ok = output.StartsWith("HTTP/1.1 200 OK") && output.Contains("\"isSuccess\":true");
                if (ok)
                {
                    result.Add(callName + ": SUCCESS: Returns 200 Http status code + 'isSuccess' JSON result");
                }
                else
                {
                    result.Add(callName + ": ERROR. Output: " + output);
                }
            }
            catch (Exception ex)
            {
                result.Add(callName + ": ERROR. Monitoring tool issue: " + ex.Message);
            }

            return output;
        }

        public static string RunExternalExe(string filename, string arguments = null)
        {
            var process = new Process();

            process.StartInfo.FileName = filename;
            if (!string.IsNullOrEmpty(arguments))
            {
                process.StartInfo.Arguments = arguments;
            }

            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = false;

            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            var stdOutput = new StringBuilder();
            process.OutputDataReceived += (sender, args) => stdOutput.Append(args.Data);

            string stdError = null;
            try
            {
                process.Start();
                process.BeginOutputReadLine();
                stdError = process.StandardError.ReadToEnd();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                throw new Exception("OS error while executing " + Format(filename, arguments) + ": " + e.Message, e);
            }

            if (process.ExitCode == 0)
            {
                return stdOutput.ToString();
            }
            else
            {
                var message = new StringBuilder();

                if (!string.IsNullOrEmpty(stdError))
                {
                    message.AppendLine(stdError);
                }

                if (stdOutput.Length != 0)
                {
                    message.AppendLine("Std output:");
                    message.AppendLine(stdOutput.ToString());
                }

                throw new Exception(Format(filename, arguments) + " finished with exit code = " + process.ExitCode + ": " + message);
            }
        }

        private static string Format(string filename, string arguments)
        {
            return "'" + filename +
                ((string.IsNullOrEmpty(arguments)) ? string.Empty : " " + arguments) +
                "'";
        }


    }

}
