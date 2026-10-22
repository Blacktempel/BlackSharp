// SPDX-License-Identifier: MPL-2.0

using BlackSharp.Core.Extensions;
using System.Collections;
using System.Globalization;
using System.Text;
using PlatformOperatingSystem = BlackSharp.Core.Platform.OperatingSystem;

namespace BlackSharp.Core.Logging
{
    #region LogLevel Enum

    /// <summary>
    /// A level of logging.
    /// If <see cref="Logger.LogLevel"/> is set, for example,
    /// to <see cref="Error"/> then <see cref="Fatal"/> will be logged also,
    /// but not the way around.
    /// Higher <see cref="LogLevel"/> than <see cref="Logger.LogLevel"/>
    /// will be logged, lower will completely be ignored.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// No logging at all.
        /// </summary>
        Disabled,

        /// <summary>
        /// Trace logging, the lowest level.
        /// </summary>
        Trace,

        /// <summary>
        /// Debug logging, for debug purposes.
        /// </summary>
        Debug,

        /// <summary>
        /// Info logging, to get more information of something.
        /// </summary>
        Info,

        /// <summary>
        /// Warn logging, if something warns before an error occurs.
        /// </summary>
        Warn,

        /// <summary>
        /// Error logging, when something goes wrong.
        /// </summary>
        Error,

        /// <summary>
        /// Fatal logging, when something goes horribly wrong and the application (might) need a shutdown.
        /// </summary>
        Fatal
    }

    #endregion

    #region LogItem

    /// <summary>
    /// An item for <see cref="Logger"/>.
    /// </summary>
    public class LogItem
    {
        #region Constructor
        /// <summary>
        /// Constructs new object.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="message">Message to log.</param>
        /// <param name="time">Time to log.</param>
        public LogItem(LogLevel level, string message, DateTime? time)
        {
            Level = level;
            Time = time;
            Message = message;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Log level of this object.
        /// </summary>
        public LogLevel Level { get; }

        /// <summary>
        /// Time of this item.
        /// </summary>
        public DateTime? Time { get; }

        /// <summary>
        /// Message to log.
        /// </summary>
        public string Message { get; }
        #endregion

        #region ToString
        /// <inheritdoc cref="object.ToString"/>
        public override string ToString()
        {
            var time = Time?.ToString("[dd/MM/yyyy HH:mm:ss.fff] ");

            return $"{time}{Logger.GetStringForLogLevel(Level)} {Message}";
        }
        #endregion
    }

    #endregion

    /// <summary>
    /// Records diagnostics in memory or appends them directly to a bounded file, depending on the constructor.
    /// </summary>
    public class Logger : ILogger, IEnumerable<LogItem>
    {
        #region Constructor

        /// <summary>
        /// Creates an independent in-memory logger.
        /// </summary>
        public Logger()
        {
        }

        /// <summary>
        /// Creates a logger that appends entries directly to a bounded file.
        /// </summary>
        /// <param name="filePath">The destination file path.</param>
        /// <param name="minimumLevel">The lowest severity to record.</param>
        /// <param name="maximumFileSizeBytes">The maximum size before rotation.</param>
        /// <param name="retainedFileCount">The number of previous files to retain.</param>
        public Logger(
            string filePath,
            LogLevel minimumLevel = LogLevel.Info,
            long maximumFileSizeBytes = 5 * 1024 * 1024,
            int retainedFileCount = 5)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("A log file path is required.", nameof(filePath));

            if (maximumFileSizeBytes <= 0)
                throw new ArgumentOutOfRangeException(nameof(maximumFileSizeBytes));

            if (retainedFileCount < 1)
                throw new ArgumentOutOfRangeException(nameof(retainedFileCount));

            FilePath             = Path.GetFullPath(filePath);
            LogLevel             = minimumLevel;
            MaximumFileSizeBytes = maximumFileSizeBytes;
            RetainedFileCount    = retainedFileCount;
            IsEnabled            = true;
        }

        #endregion

        #region Fields

        /// <summary>
        /// Synchronizes access to retained log entries.
        /// </summary>
        private readonly object _syncRoot = new object();

        /// <summary>
        /// Retains entries written to this logger instance.
        /// </summary>
        private readonly List<LogItem> _log = new List<LogItem>();

        /// <summary>
        /// Stores the minimum severity with visibility across logging and configuration threads.
        /// </summary>
        private int _minimumLogLevel = (int)LogLevel.Error;

        /// <summary>
        /// Tracks which in-memory entries were already appended to each destination file.
        /// </summary>
        private readonly Dictionary<string, int> _savedEntryCounts = new Dictionary<string, int>(
            PlatformOperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the destination file, or <see langword="null"/> when entries are retained only in memory.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Gets the maximum size of a file-backed log before rotation.
        /// </summary>
        public long MaximumFileSizeBytes { get; }

        /// <summary>
        /// Gets the number of previous file generations retained during rotation.
        /// </summary>
        public int RetainedFileCount { get; }

        /// <summary>
        /// Gets or sets whether logging is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the minimum severity. The memory-only default is <see cref="LogLevel.Error"/>.
        /// </summary>
        public LogLevel LogLevel
        {
            get => (LogLevel)Volatile.Read(ref _minimumLogLevel);
            set => Volatile.Write(ref _minimumLogLevel, (int)value);
        }

        #endregion

        #region Public

        /// <summary>
        /// Adds an element to the log if specified level is higher
        /// or equal to current <see cref="LogLevel"/> and not
        /// <see cref="LogLevel.Disabled"/>.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="message">Log message.</param>
        /// <param name="time">Time of event.</param>
        public void Add(LogLevel level, string message, DateTime? time)
        {
            TryLogCore(level, null, message, null, time);
        }

        /// <summary>
        /// Adds an element to the log if specified level is higher
        /// or equal to current <see cref="LogLevel"/> and not
        /// <see cref="LogLevel.Disabled"/>.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="message">Log message.</param>
        public void Add(LogLevel level, string message)
        {
            Add(level, message, null);
        }

        /// <summary>
        /// Adds an element to the log if specified level is higher
        /// or equal to current <see cref="LogLevel"/> and not
        /// <see cref="LogLevel.Disabled"/>.
        /// </summary>
        /// <param name="item">Item to add.</param>
        public void Add(LogItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (FilePath == null)
            {
                if (!IsEnabled || item.Level < LogLevel || LogLevel == LogLevel.Disabled)
                {
                    return;
                }

                lock (_syncRoot)
                {
                    _log.Add(item);
                }

                return;
            }

            TryLogCore(item.Level, null, item.Message, null, item.Time);
        }

        /// <inheritdoc/>
        public void Log(LogLevel level, string source, string message, Exception exception = null)
        {
            TryLog(level, source, message, exception);
        }

        /// <summary>
        /// Records an event and reports whether a file-backed write succeeded.
        /// </summary>
        /// <param name="level">The severity of the event.</param>
        /// <param name="source">The component or operation that produced the event.</param>
        /// <param name="message">A description of the event.</param>
        /// <param name="exception">An optional associated exception.</param>
        /// <returns>Whether the event was accepted or written successfully.</returns>
        public bool TryLog(LogLevel level, string source, string message, Exception exception = null)
        {
            return TryLogCore(level, source, message, exception, DateTime.Now);
        }

        /// <summary>
        /// Appends a preformatted entry to a file-backed logger using the same rotation rules.
        /// Writes raw crash and audit entries directly while applying file rotation.
        /// </summary>
        /// <param name="entry">The complete entry text.</param>
        /// <returns>Whether the entry was written successfully; memory-only loggers return <see langword="false"/>.</returns>
        public bool TryWriteRaw(string entry)
        {
            if (FilePath == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(entry))
            {
                return true;
            }

            try
            {
                lock (_syncRoot)
                {
                    var directory = Path.GetDirectoryName(FilePath);

                    if (!string.IsNullOrWhiteSpace(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    RotateIfRequired(Encoding.UTF8.GetByteCount(entry));

                    File.AppendAllText(FilePath, entry, new UTF8Encoding(true));
                }

                return true;
            }
            catch
            {
                // Diagnostics must not interrupt an application when the file is unavailable.
                return false;
            }
        }

        /// <summary>
        /// Removes all elements with specified <see cref="Logging.LogLevel"/>.
        /// </summary>
        /// <param name="level">Level to remove from <see cref="Logger"/>.</param>
        public void Remove(LogLevel level)
        {
            lock (_syncRoot)
            {
                foreach (var path in _savedEntryCounts.Keys.ToArray())
                {
                    var savedCount    = Math.Min(_savedEntryCounts[path], _log.Count);
                    var removedCount = 0;

                    for (var index = 0; index < savedCount; ++index)
                    {
                        if (_log[index].Level == level)
                        {
                            ++removedCount;
                        }
                    }

                    _savedEntryCounts[path] = savedCount - removedCount;
                }

                _log.RemoveAll(item => item.Level == level);
            }
        }

        /// <summary>
        /// Gets a log-string for specified <see cref="Logging.LogLevel"/>.
        /// </summary>
        /// <param name="level">Level to get string for.</param>
        /// <returns>Log-String for specified level.</returns>
        public static string GetStringForLogLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Disabled:
                    return string.Empty;
                case LogLevel.Trace:
                    return "[TRACE]";
                case LogLevel.Debug:
                    return "[DEBUG]";
                case LogLevel.Info:
                    return "[INFO]";
                case LogLevel.Warn:
                    return "[WARN]";
                case LogLevel.Error:
                    return "[ERROR]";
                case LogLevel.Fatal:
                    return "[FATAL]";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Saves retained entries to a file. In append mode, only entries added since the last
        /// successful save to the same path are written. For a file-backed logger, copies its current file.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <param name="append">Whether new content should be appended to an existing file.</param>
        /// <returns>If file could be written.</returns>
        public bool SaveToFile(string path, bool append = true)
        {
            try
            {
                var fullPath = Path.GetFullPath(path);

                var pathComparison = PlatformOperatingSystem.IsWindows()
                    ? StringComparison.OrdinalIgnoreCase
                    : StringComparison.Ordinal;

                if (FilePath != null && string.Equals(fullPath, FilePath, pathComparison))
                {
                    return true;
                }

                if (FilePath != null)
                {
                    var content = File.Exists(FilePath) ? File.ReadAllText(FilePath) : string.Empty;

                    if (append)
                    {
                        File.AppendAllText(fullPath, content, Encoding.UTF8);
                    }
                    else
                    {
                        File.WriteAllText(fullPath, content, Encoding.UTF8);
                    }

                    return true;
                }

                lock (_syncRoot)
                {
                    var startIndex = append && File.Exists(fullPath) && _savedEntryCounts.TryGetValue(fullPath, out var savedCount)
                        ? Math.Min(savedCount, _log.Count)
                        : 0;

                    var builder = new StringBuilder();

                    for (var index = startIndex; index < _log.Count; ++index)
                    {
                        builder.AppendLine(_log[index].ToString());
                    }

                    if (append)
                    {
                        File.AppendAllText(fullPath, builder.ToString(), Encoding.UTF8);
                    }
                    else
                    {
                        File.WriteAllText(fullPath, builder.ToString(), Encoding.UTF8);
                    }

                    _savedEntryCounts[fullPath] = _log.Count;

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Private

        /// <summary>
        /// Stores an event in memory or appends it to the configured file.
        /// </summary>
        /// <param name="level">The severity of the event.</param>
        /// <param name="source">The event source.</param>
        /// <param name="message">The event description.</param>
        /// <param name="exception">An optional associated exception.</param>
        /// <param name="time">An optional original event time.</param>
        /// <returns>Whether the event was accepted or written successfully.</returns>
        private bool TryLogCore(LogLevel level, string source, string message, Exception exception, DateTime? time)
        {
            if (!IsEnabled || LogLevel == LogLevel.Disabled || level == LogLevel.Disabled || level < LogLevel)
            {
                return true;
            }

            if (FilePath == null)
            {
                var description = string.IsNullOrWhiteSpace(source)
                    ? message ?? string.Empty
                    : $"[{source.Trim()}] {message}";

                if (exception != null)
                {
                    description += Environment.NewLine + FormatException(exception);
                }

                lock (_syncRoot)
                {
                    _log.Add(new LogItem(level, description, time));
                }

                return true;
            }

            var timestamp = time.HasValue
                ? new DateTimeOffset(time.Value).ToUniversalTime()
                : DateTimeOffset.UtcNow;

            var builder = new StringBuilder();

            builder.Append(timestamp.ToString("O", CultureInfo.InvariantCulture));
            builder.Append(' ').Append(GetStringForLogLevel(level));
            builder.Append(" [").Append(string.IsNullOrWhiteSpace(source) ? "General" : source.Trim()).Append("] ");
            builder.AppendLine(message ?? string.Empty);

            if (exception != null)
            {
                builder.AppendLine(FormatException(exception));
            }

            return TryWriteRaw(builder.ToString());
        }

        /// <summary>
        /// Formats an exception with its type and the detailed Core exception information.
        /// </summary>
        /// <param name="exception">The exception to format.</param>
        /// <returns>The complete diagnostic text.</returns>
        private static string FormatException(Exception exception)
        {
            return exception.GetType().FullName + Environment.NewLine + exception.FullExceptionString();
        }

        /// <summary>
        /// Rotates older generations before an append would exceed the configured file size.
        /// </summary>
        /// <param name="appendedByteCount">The encoded size of the next entry.</param>
        private void RotateIfRequired(int appendedByteCount)
        {
            var currentLength = File.Exists(FilePath) ? new FileInfo(FilePath).Length : 0;

            if (currentLength + appendedByteCount <= MaximumFileSizeBytes)
                return;

            var oldestPath = FilePath + "." + RetainedFileCount;

            if (File.Exists(oldestPath))
            {
                File.Delete(oldestPath);
            }

            for (var index = RetainedFileCount - 1; index >= 1; --index)
            {
                var sourcePath      = FilePath + "." + index;
                var destinationPath = FilePath + "." + (index + 1);

                if (File.Exists(sourcePath))
                {
                    File.Move(sourcePath, destinationPath);
                }
            }

            if (File.Exists(FilePath))
            {
                File.Move(FilePath, FilePath + ".1");
            }
        }

        #endregion

        #region ToString

        /// <inheritdoc cref="object.ToString"/>
        public override string ToString()
        {
            var builder = new StringBuilder();

            lock (_syncRoot)
            {
                foreach (var item in _log)
                {
                    builder.AppendLine(item.ToString());
                }
            }

            return builder.ToString();
        }

        #endregion

        #region IEnumerable

        /// <summary>
        /// Enumerator implementation.
        /// </summary>
        /// <returns>An enumerator.</returns>
        public IEnumerator<LogItem> GetEnumerator()
        {
            lock (_syncRoot)
            {
                return _log.ToList().GetEnumerator();
            }
        }

        /// <summary>
        /// Enumerator implementation.
        /// </summary>
        /// <returns>An enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
