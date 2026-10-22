// SPDX-License-Identifier: MPL-2.0

using BlackSharp.Platform.Startup.Models;
using BlackSharp.Platform.Startup.Utilities;
using Microsoft.Win32.TaskScheduler;
using System.Diagnostics;
using System.Security.Principal;
using PlatformOperatingSystem = BlackSharp.Core.Platform.OperatingSystem;

#pragma warning disable CA1416 // Callers select this service only on Windows.

namespace BlackSharp.Platform.Startup.Platform.Windows
{
    /// <summary>
    /// Registers boot and login startup commands with Windows Task Scheduler.
    /// </summary>
    public static class WindowsTaskStartupRegistration
    {
        #region Public

        /// <summary>
        /// Creates or removes a task that runs under SYSTEM when Windows boots.
        /// </summary>
        /// <param name="taskName">The registered task name.</param>
        /// <param name="description">The task description.</param>
        /// <param name="command">The command to run when enabling the task.</param>
        /// <param name="enabled">Whether the task should be registered.</param>
        public static void SetBootTaskEnabled(string taskName, string description, StartupCommand command, bool enabled)
        {
            if (!PlatformOperatingSystem.IsWindows())
            {
                throw new PlatformNotSupportedException("Task Scheduler startup registration requires Windows.");
            }

            using var taskService = new TaskService();

            if (!enabled)
            {
                taskService.RootFolder.DeleteTask(taskName, exceptionOnNotExists: false);
                return;
            }

            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            using var definition = taskService.NewTask();
            definition.RegistrationInfo.Description = description;

            definition.Principal.Id        = "System";
            definition.Principal.UserId    = "SYSTEM";
            definition.Principal.LogonType = TaskLogonType.ServiceAccount;
            definition.Principal.RunLevel  = TaskRunLevel.Highest;

            definition.Triggers.Add(new BootTrigger { Enabled = true });

            var settings = definition.Settings;
            settings.MultipleInstances          = TaskInstancesPolicy.IgnoreNew;
            settings.DisallowStartIfOnBatteries = false;
            settings.StopIfGoingOnBatteries     = false;
            settings.AllowHardTerminate         = true;
            settings.StartWhenAvailable         = true;
            settings.RunOnlyIfNetworkAvailable  = false;
            settings.IdleSettings.StopOnIdleEnd = false;
            settings.IdleSettings.RestartOnIdle = false;
            settings.AllowDemandStart           = true;
            settings.Enabled                    = true;
            settings.Hidden                     = false;
            settings.RunOnlyIfIdle              = false;
            settings.WakeToRun                  = false;
            settings.ExecutionTimeLimit         = TimeSpan.Zero;
            settings.Priority                   = ProcessPriorityClass.BelowNormal;
            settings.RestartInterval            = TimeSpan.FromMinutes(1);
            settings.RestartCount               = 3;

            AddAction(definition, command);

            using var registeredTask = taskService.RootFolder.RegisterTaskDefinition(
                taskName, definition, TaskCreation.CreateOrUpdate, "SYSTEM", null, TaskLogonType.ServiceAccount);
        }

        /// <summary>
        /// Creates or removes a non-elevated login task for one Windows user.
        /// </summary>
        /// <param name="taskName">The registered task name.</param>
        /// <param name="description">The task description.</param>
        /// <param name="command">The command to run when enabling the task.</param>
        /// <param name="enabled">Whether the task should be registered.</param>
        public static void SetCurrentUserLoginTaskEnabled(string taskName, string description, StartupCommand command, bool enabled)
        {
            if (!PlatformOperatingSystem.IsWindows())
            {
                throw new PlatformNotSupportedException("Task Scheduler startup registration requires Windows.");
            }

            using var identity = WindowsIdentity.GetCurrent();

            var userSID = identity.User?.Value
                ?? throw new InvalidOperationException("The current Windows user has no security identifier.");

            using var taskService = new TaskService();

            if (!enabled)
            {
                taskService.RootFolder.DeleteTask(taskName, exceptionOnNotExists: false);
                return;
            }

            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            using var definition = taskService.NewTask();
            definition.RegistrationInfo.Description = description;
            definition.Principal.UserId    = userSID;
            definition.Principal.LogonType = TaskLogonType.InteractiveToken;
            definition.Principal.RunLevel  = TaskRunLevel.LUA;

            definition.Triggers.Add(new LogonTrigger { UserId = userSID, Enabled = true });

            definition.Settings.MultipleInstances          = TaskInstancesPolicy.IgnoreNew;
            definition.Settings.DisallowStartIfOnBatteries = false;
            definition.Settings.StopIfGoingOnBatteries     = false;
            definition.Settings.StartWhenAvailable         = true;
            definition.Settings.ExecutionTimeLimit         = TimeSpan.Zero;

            AddAction(definition, command);

            using var registeredTask = taskService.RootFolder.RegisterTaskDefinition(
                taskName, definition, TaskCreation.CreateOrUpdate, userSID, null, TaskLogonType.InteractiveToken);
        }

        /// <summary>
        /// Creates or removes a non-elevated login task for an authenticated user group.
        /// </summary>
        /// <param name="taskName">The registered task name.</param>
        /// <param name="description">The task description.</param>
        /// <param name="command">The command to run when enabling the task.</param>
        /// <param name="groupSID">The Windows group security identifier.</param>
        /// <param name="enabled">Whether the task should be registered.</param>
        public static void SetGroupLoginTaskEnabled(string taskName, string description, StartupCommand command, string groupSID, bool enabled)
        {
            if (!PlatformOperatingSystem.IsWindows())
            {
                throw new PlatformNotSupportedException("Task Scheduler startup registration requires Windows.");
            }

            using var taskService = new TaskService();

            if (!enabled)
            {
                taskService.RootFolder.DeleteTask(taskName, exceptionOnNotExists: false);
                return;
            }

            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            using var definition = taskService.NewTask();
            definition.RegistrationInfo.Description = description;
            definition.Principal.GroupId   = groupSID;
            definition.Principal.LogonType = TaskLogonType.Group;
            definition.Principal.RunLevel  = TaskRunLevel.LUA;

            definition.Triggers.Add(new LogonTrigger { Enabled = true });

            definition.Settings.MultipleInstances          = TaskInstancesPolicy.Parallel;
            definition.Settings.DisallowStartIfOnBatteries = false;
            definition.Settings.StopIfGoingOnBatteries     = false;
            definition.Settings.AllowDemandStart           = true;
            definition.Settings.ExecutionTimeLimit         = TimeSpan.Zero;

            AddAction(definition, command);

            using var registeredTask = taskService.RootFolder.RegisterTaskDefinition(
                taskName, definition, TaskCreation.CreateOrUpdate, groupSID, null, TaskLogonType.Group);
        }

        #endregion

        #region Private

        /// <summary>
        /// Appends a quoted executable action to a task definition.
        /// </summary>
        /// <param name="definition">The task being registered.</param>
        /// <param name="command">The executable and its arguments.</param>
        private static void AddAction(TaskDefinition definition, StartupCommand command)
        {
            definition.Actions.Add(new ExecAction(
                command.ExecutablePath,
                string.Join(" ", command.Arguments.Select(StartupArgumentQuoting.QuoteWindowsArgument)),
                command.WorkingDirectory));
        }

        #endregion
    }
}

#pragma warning restore CA1416
