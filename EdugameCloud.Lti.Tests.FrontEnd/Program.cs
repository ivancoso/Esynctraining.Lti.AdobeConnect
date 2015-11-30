using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace EdugameCloud.Lti.Tests.FrontEnd
{
    class Program
    {
        static void Main(string[] args)
        {
            const string ToolFileName = @"c:\Users\isa\Downloads\curl-7.45.0-win64-mingw\bin\curl.exe";
            const string arguments = @"--config c:\Users\isa\Downloads\curl-7.45.0-win64-mingw\bin\dev.config";
            string output = RunExternalExe(ToolFileName, arguments);


            var url = Regex.Match(output, "<h2>Object moved to <a href=\"(?<url>[\\w\\&;]*)\">here</a>.</h2>");

            //var ee = Regex.Match(output, "Object moved to <a href=\\\"/extjs/entry?primaryColor=9966&(amp);lmsProviderName=[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}&(amp);");
            var parsing = Regex.Match(output, "Object moved to <a href=\\\"/extjs/entry[?]primaryColor=(?<color>[0-9]+)[&][a]mp;lmsProviderName=(?<session>[A-Fa-f0-9]{8}(?:-[A-Fa-f0-9]{4}){3}-[A-Fa-f0-9]{12})[&][a]mp;");
            if (parsing.Success)
            {
                Guid sessionToken;
                if (Guid.TryParse(parsing.Groups["session"].Value, out sessionToken))
                {
                    string arguments2 =
                        string.Format("--config c:\\Users\\isa\\Downloads\\curl-7.45.0-win64-mingw\\bin\\dev-recordings.config --data \"lmsProviderName={0}&meetingId=12697\" ",
                        sessionToken);
                    string output2 = RunExternalExe(ToolFileName, arguments2);

                    // OK Http status + success JSON result
                    bool ok = output2.StartsWith("HTTP/1.1 200 OK") && output2.Contains("\"isSuccess\":true");
                    //--data "lmsProviderName=9e1c80ed-d43a-42bf-8166-a5590053937f&meetingId=12697" 
                }
            }
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
