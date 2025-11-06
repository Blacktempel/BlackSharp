/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using BlackSharp.Core.Logging;

namespace BlackSharp.Core.Tests.Logging
{
    [TestClass]
    public class LoggerTests
    {
        static object _Lock = new object();

        LogLevel _LogLevel = LogLevel.Debug;

        [TestMethod]
        public void Add()
        {
            var message0 = "Add Message 0";
            var message1 = "Add Message 1";
            var message2 = "Add Message 2";

            string str;

            lock (_Lock)
            {
                Logger.Instance.IsEnabled = true;
                Logger.Instance.LogLevel = _LogLevel;

                Assert.AreEqual(_LogLevel, Logger.Instance.LogLevel);

                Logger.Instance.Add(_LogLevel, message0, DateTime.Now);
                Logger.Instance.Add(_LogLevel, message1);
                Logger.Instance.Add(new LogItem(_LogLevel, message2, DateTime.Now));

                str = Logger.Instance.ToString();
            }

            Assert.Contains(message0, str);
            Assert.Contains(message1, str);
            Assert.Contains(message2, str);
        }

        [TestMethod]
        public void Remove()
        {
            var message = "Remove Message";

            string str;

            lock (_Lock)
            {
                Logger.Instance.IsEnabled = true;
                Logger.Instance.LogLevel = _LogLevel;

                Logger.Instance.Add(_LogLevel, message, DateTime.Now);
                Logger.Instance.Remove(_LogLevel);

                str = Logger.Instance.ToString();
            }

            Assert.DoesNotContain(message, str);
        }

        [TestMethod]
        public void GetStringForLogLevel()
        {
            string str;

            lock (_Lock)
            {
                str = Logger.GetStringForLogLevel(_LogLevel);
            }

            Assert.AreEqual("[DEBUG]", str);
        }
    }
}
