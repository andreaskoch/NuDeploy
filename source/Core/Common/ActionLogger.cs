using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace NuDeploy.Core.Common
{
    public class ActionLogger : IActionLogger, IDisposable
    {
        public const string LogFilenamePattern = "NuDeploy.{0}.log";

        public const string ValueSeperator = " - ";

        private readonly ApplicationInformation applicationInformation;

        private readonly IFilesystemAccessor filesystemAccessor;

        private readonly StreamWriter logStreamWriter;

        private readonly Regex tooMuchWhiteSpaceRegex = new Regex("\\s{2,}");

        public ActionLogger(ApplicationInformation applicationInformation, IFilesystemAccessor filesystemAccessor)
        {
            this.applicationInformation = applicationInformation;
            this.filesystemAccessor = filesystemAccessor;

            var logfilPath = this.GetLogfilePath();
            this.logStreamWriter = new StreamWriter(logfilPath, true, Encoding.UTF8);
        }

        public void Log(string message, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            string content = this.GetTrimmedMessage(string.Format(message, args));
            if (string.IsNullOrWhiteSpace(content))
            {
                return;
            }

            var timestamp = DateTimeOffset.Now;
            string date = timestamp.ToString("yyyy-MM-dd");
            string time = timestamp.ToString("H:mm:ss");
            string utcOffset = string.Concat("UTC", timestamp.ToString("zzz"));
            string user = string.Format("{0}\\{1}", this.applicationInformation.ExecutingUser.Domain, this.applicationInformation.ExecutingUser.Username);
            string computer = this.applicationInformation.MachineName;

            var entry = new[] { date, time, utcOffset, user, computer, content };

            this.logStreamWriter.WriteLine(string.Join(ValueSeperator, entry));
            this.logStreamWriter.Flush();
        }

        public void Dispose()
        {
            if (this.logStreamWriter != null)
            {
                this.logStreamWriter.Close();
            }
        }

        private string GetLogfilePath()
        {
            if (!this.filesystemAccessor.DirectoryExists(this.applicationInformation.LogFolder))
            {
                Directory.CreateDirectory(this.applicationInformation.LogFolder);
            }

            string logfileName = string.Format(LogFilenamePattern, DateTime.Today.ToString("yyyy-MM-dd"));
            return Path.Combine(this.applicationInformation.LogFolder, logfileName);
        }

        private string GetTrimmedMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return message;
            }

            var trimmed = message.Trim();
            trimmed = trimmed.Replace("\r\n", string.Empty);
            trimmed = this.tooMuchWhiteSpaceRegex.Replace(trimmed, " ");

            return trimmed;
        }
    }
}