/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using BlackSharp.Core.Extensions;
using System.Collections;
using System.Text;

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
        public LogItem(LogLevel level, string message, DateTime? time)
        {
            Level = level;
            Time = time;
            Message = message;
        }
        #endregion

        #region Properties
        public LogLevel Level { get; }
        public DateTime? Time { get; }
        public string Message { get; }
        #endregion

        #region ToString
        public override string ToString()
        {
            var time = Time?.ToString("[dd/MM/yyyy HH:mm:ss.fff] ");
            return $"{time}{Logger.GetStringForLogLevel(Level)} {Message}";
        }
        #endregion
    }
    #endregion

    /// <summary>
    /// A class to enable logging for different configurations.
    /// </summary>
    public class Logger : IEnumerable<LogItem>
    {
        #region Constructor
        /// <summary>
        /// Do not allow creating instance of class (only singleton).
        /// </summary>
        Logger()
        {
        }
        #endregion

        #region Fields
        List<LogItem> _Log = new List<LogItem>();
        #endregion

        #region Properties
        static Logger _Instance;
        public static Logger Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new Logger();
                return _Instance;
            }
        }

        /// <summary>
        /// Specifies if logging is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Describes the current <see cref="Logging.LogLevel"/>. Default is <see cref="Logging.LogLevel.Error"/>.
        /// </summary>
        public LogLevel LogLevel { get; set; } = LogLevel.Error;
        #endregion

        #region Public API

        /// <summary>
        /// Adds an element to the log if specified level is higher
        /// or equal to current <see cref="LogLevel"/> and not
        /// <see cref="Logging.LogLevel.Disabled"/>.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="message">Log message.</param>
        /// <param name="time">Time of event.</param>
        public void Add(LogLevel level, string message, DateTime? time)
        {
            if (!IsEnabled || level < LogLevel || LogLevel == LogLevel.Disabled)
                return;
            _Log.Add(new LogItem(level, message, time));
        }

        /// <summary>
        /// Adds an element to the log if specified level is higher
        /// or equal to current <see cref="LogLevel"/> and not
        /// <see cref="Logging.LogLevel.Disabled"/>.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="message">Log message.</param>
        public void Add(LogLevel level, string message)
        {
            if (!IsEnabled || level < LogLevel || LogLevel == LogLevel.Disabled)
                return;
            _Log.Add(new LogItem(level, message, null));
        }

        /// <summary>
        /// Adds an element to the log if specified level is higher
        /// or equal to current <see cref="LogLevel"/> and not
        /// <see cref="Logging.LogLevel.Disabled"/>.
        /// </summary>
        /// <param name="item">Item to add.</param>
        public void Add(LogItem item)
        {
            if (!IsEnabled || item.Level < LogLevel || LogLevel == LogLevel.Disabled)
                return;
            _Log.Add(item);
        }

        /// <summary>
        /// Removes all elements with specified <see cref="Logging.LogLevel"/>.
        /// </summary>
        /// <param name="level">Level to remove from <see cref="Logger"/>.</param>
        public void Remove(LogLevel level)
        {
            _Log.RemoveIf(x => x.Level == level);
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
        /// Saves content of current <see cref="Logger"/> to a file.<para/>
        /// Note: To save current content to a string, use <see cref="ToString"/>.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <param name="append">Specifies if content should be
        /// appended to the file if it already exists.</param>
        /// <returns>If file could be written.</returns>
        public bool SaveToFile(string path, bool append = true)
        {
            try
            {
                if (append)
                    File.AppendAllText(path, ToString(), Encoding.UTF8);
                else
                    File.WriteAllText(path, ToString(), Encoding.UTF8);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region ToString
        public override string ToString()
        {
            //if (Debugger.IsAttached)
            //    return $"Logger contains {_Log.Count} items.";

            StringBuilder sb = new StringBuilder();
            foreach (var item in this)
            {
                sb.AppendLine(item.ToString());
            }

            return sb.ToString();
        }
        #endregion

        #region IEnumerable
        /// <summary>
        /// Enumerator implementation.
        /// </summary>
        /// <returns>An enumerator.</returns>
        public IEnumerator<LogItem> GetEnumerator()
        {
            return _Log.GetEnumerator();
        }

        /// <summary>
        /// Enumerator implementation.
        /// </summary>
        /// <returns>An enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Log.GetEnumerator();
        }
        #endregion
    }
}
