using Seq.Apps;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System;

namespace Seq.App.JsonFormattedArchive.src
{
    [SeqApp(" Json Formatted Archive", Description = "Writes events to a set of rolling log files for long-term archival storage.")]
    public class JsonFormattedArchiveApp : SeqApp, ISubscribeTo<LogEvent>, IDisposable
    {
        public Logger Logger;

        [SeqAppSetting(
            DisplayName = "Path format",
            HelpText = "The location of the log files. E.g. 'C:\\Logs\\events.log'")]
        public string PathFormat { get; set; } = "events.log";

        [SeqAppSetting(
            DisplayName = "RollingInterval",
            HelpText = @"Infinite: The log file will never roll; no time period information will be appended to the log file name.
                        Year: Roll every year. Filename will have a four-digit year appended in the pattern <code>yyyy</code>.
                        Month: Roll every calendar month. Filename will have <code>yyyyMM</code> appended.
                        Day: Roll every day. Filename will have <code>yyyyMMdd</code> appended.
                        Hour: Roll every hour. Filename will have <code>yyyyMMddHH</code> appended.
                        Minute: Roll every minute. Filename will have <code>yyyyMMddHHmm</code> appended.
                        The default is Day. ",
            IsOptional = true)]
        public RollingInterval RollingInterval { get; set; } = RollingInterval.Day;

        [SeqAppSetting(
            DisplayName = "RetainedFileCountLimit",
            HelpText = "The maximum number of log files that will be retained,    /// including the current log file. For unlimited retention, pass null. The default is 31.",
            IsOptional = true)]
        public int? RetainedFileCountLimit { get; set; } = null;

        protected override void OnAttached()
        {
            Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(
                    new CompactJsonFormatter(),
                    PathFormat,
                    rollingInterval: RollingInterval,
                    retainedFileCountLimit: RetainedFileCountLimit
                )
                .CreateLogger();
        }

        public void On(Event<LogEvent> evt)
        {
            Logger.Write(evt.Data);
        }

        public void Dispose()
        {
            Logger.Dispose();
        }
    }
}
