/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Meta.XR.Simulator.Editor;
using UnityEditor;
using static Meta.XR.Simulator.ProcessPort;

using System.Text.RegularExpressions;
using System.Text;

#if META_XR_SDK_CORE_SUPPORTS_TOOLBAR
using Meta.XR.Editor.PlayCompanion;
using Meta.XR.Editor.StatusMenu;
using Styles = Meta.XR.Editor.PlayCompanion.Styles;
using Meta.XR.Simulator.Editor.SyntheticEnvironments;

#if META_XR_SDK_CORE_74_OR_NEWER
using Meta.XR.Editor.ToolingSupport;
#endif

#endif

namespace Meta.XR.Simulator
{
    [InitializeOnLoad]
    internal static class Utils
    {
        public const string PublicName = "Meta XR Simulator";
        public const string MenuPath = "Meta/" + PublicName;
        public const string PackageName = "com.meta.xr.simulator";
        public const string PackagePath = "Packages/" + PackageName;

        private const string ToolbarItemTooltip =
#if UNITY_2022_2_OR_NEWER
            "Set Play mode to use Meta XR Simulator\n<i>Simulates Meta Quest headset and features on desktop</i>";
#else
            "Set Play mode to use Meta XR Simulator\nSimulates Meta Quest headset and features on desktop";
#endif

#if META_XR_SDK_CORE_SUPPORTS_TOOLBAR
#if META_XR_SDK_CORE_74_OR_NEWER
        internal static readonly ToolDescriptor ToolDescriptor = new()
#else
        internal static readonly Meta.XR.Editor.StatusMenu.Item ToolDescriptor = new()
#endif
        {
            Name = PublicName,
            Color = Meta.XR.Editor.UserInterface.Styles.Colors.Meta,
            Icon = Styles.Contents.MetaXRSimulator,
#if META_XR_SDK_CORE_74_OR_NEWER
            MqdhCategoryId = "857564592791179",
            AddToStatusMenu = true,
            AddToMenu = false,
#endif
#if META_XR_SDK_CORE_68_OR_NEWER
            PillIcon = () =>
                Enabler.Activated
                    ? (Meta.XR.Editor.UserInterface.Styles.Contents.CheckIcon,
                        Meta.XR.Editor.UserInterface.Styles.Colors.Meta,
                        false)
                    : (null, null, false),
#else
                PillIcon = () =>
                    Enabler.Activated
                        ? (Meta.XR.Editor.UserInterface.Styles.Contents.CheckIcon,
                            Meta.XR.Editor.UserInterface.Styles.Colors.Meta)
                        : (null, null),
#endif
            InfoTextDelegate = () => (Enabler.Activated ? "Activated" : "Deactivated", null),
            OnClickDelegate = origin => Enabler.ToggleSimulator(true, origin.ToString().ToSimulatorOrigin()),
            Order = 4,
            CloseOnClick = false
        };
#endif

        static Utils()
        {

#if META_XR_SDK_CORE_SUPPORTS_TOOLBAR
#if !META_XR_SDK_CORE_74_OR_NEWER
            StatusMenu.RegisterItem(ToolDescriptor);
#endif

            void MaybeStopServers()
            {
                if (Settings.AutomaticServers)
                {
                    SyntheticEnvironmentServer.Stop();
                }
            }

            var xrSimulatorItem = new Meta.XR.Editor.PlayCompanion.Item()
            {
                Order = 10,
                Name = PublicName,
                Tooltip = ToolbarItemTooltip,
                Icon = Styles.Contents.MetaXRSimulator,
                Color = Meta.XR.Editor.UserInterface.Styles.Colors.Meta,
                Show = true,
                ShouldBeSelected = () => Enabler.Activated,
                ShouldBeUnselected = () => !Enabler.Activated,
                OnSelect = () => { Enabler.ActivateSimulator(true, Origins.Toolbar); },
                OnUnselect = () =>
                {
                    Enabler.DeactivateSimulator(true, Origins.Toolbar);
                    MaybeStopServers();
                },
                OnEnteringPlayMode = () =>
                {
                    if (Settings.AutomaticServers)
                    {
                        Registry.GetByInternalName(Settings.LastEnvironment)?
                            .Launch(true, Settings.DisplayServers);
                    }
                },
                OnExitingPlayMode = MaybeStopServers,
#if META_XR_SDK_CORE_69_OR_NEWER
                OnEditorQuitting = MaybeStopServers,
#endif
            };

            Manager.RegisterItem(xrSimulatorItem);
#endif
        }

        public enum Origins
        {
            Unknown = -1,
            Settings,
            Menu,
            StatusMenu,
            Console,
            Component,
            Toolbar
        }

        public static Origins ToSimulatorOrigin(this string origin)
        {
            Enum.TryParse(origin, out Origins simulatorOrigin);
            return simulatorOrigin;
        }

        public static void ReportInfo(string title, string body)
        {
            UnityEngine.Debug.Log($"[{title}] {body}");
        }

        public static void ReportWarning(string title, string body)
        {
            UnityEngine.Debug.LogWarning($"[{title}] {body}");
        }

        public static void ReportError(string title, string body)
        {
            UnityEngine.Debug.LogError($"[{title}] {body}");
        }

        public static void DisplayDialogOrError(string title, string body, bool forceHideDialog = false)
        {
            if (!forceHideDialog && !Enabler.UnityRunningInBatchmode)
            {
                EditorUtility.DisplayDialog(title, body, "Ok");
            }

            ReportError(title, body);
        }

        public static void LaunchProcess(string binaryPath, string arguments, string logContext, bool createWindow = true)
        {
            ReportInfo(logContext, "Launching " + binaryPath + ", createWindow=" + createWindow + ", arguments=" + arguments);
            var sesProcess = new Process();
            sesProcess.StartInfo.FileName = binaryPath;
            sesProcess.StartInfo.Arguments = arguments;

            if (!createWindow)
            {
                sesProcess.StartInfo.CreateNoWindow = true;
                sesProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                sesProcess.StartInfo.UseShellExecute = false;
                sesProcess.StartInfo.RedirectStandardOutput = true;
                sesProcess.StartInfo.RedirectStandardError = true;
            }
            if (!sesProcess.Start())
            {
                DisplayDialogOrError(logContext, "failed to launch " + binaryPath);
            }
        }

        public static void StopProcess(string processPort, string logContext)
        {
            var existingProcesses = GetProcessesByPort(processPort);
            foreach (var existingProcess in existingProcesses)
            {
                ReportInfo(logContext, $"Stopping {existingProcess.processName} with PID {existingProcess.processId}");
                var p = Process.GetProcessById(existingProcess.processId);
                try
                {
                    p.Kill();
                    p.WaitForExit();
                }
                catch (InvalidOperationException ex)
                {
                    ReportInfo(logContext, $"Received an InvalidOperationException when killing {existingProcess.processId}, reason={ex.Message},\nstack={ex.StackTrace}");
                }
            }
        }


        // From http://csharptest.net/529/how-to-correctly-escape-command-line-arguments-in-c/index.html
        readonly static Regex invalidChar = new Regex("[\x00\x0a\x0d]");//  these can not be escaped
        readonly static Regex needsQuotes = new Regex(@"\s|""");//          contains whitespace or two quote characters
        readonly static Regex escapeQuote = new Regex(@"(\\*)(""|$)");//    one or more '\' followed with a quote or end of string
        /// <summary>
        /// Quotes all arguments that contain whitespace, or begin with a quote and returns a single
        /// argument string for use with Process.Start().
        /// </summary>
        /// <param name="args">A list of strings for arguments, may not contain null, '\0', '\r', or '\n'</param>
        /// <returns>The combined list of escaped/quoted strings</returns>
        /// <exception cref="System.ArgumentNullException">Raised when one of the arguments is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Raised if an argument contains '\0', '\r', or '\n'</exception>
        public static string EscapeArguments(params string[] args)
        {
            StringBuilder arguments = new StringBuilder();

            for (int carg = 0; args != null && carg < args.Length; carg++)
            {
                if (args[carg] == null) { throw new ArgumentNullException("args[" + carg + "]"); }
                if (invalidChar.IsMatch(args[carg])) { throw new ArgumentOutOfRangeException("args[" + carg + "]"); }
                if (args[carg] == String.Empty) { arguments.Append("\"\""); }
                else if (!needsQuotes.IsMatch(args[carg]))
                {
                    arguments.Append(args[carg]);
                }
                else
                {
                    arguments.Append('"');
                    arguments.Append(escapeQuote.Replace(args[carg], m =>
                    m.Groups[1].Value + m.Groups[1].Value +
                    (m.Groups[2].Value == "\"" ? "\\\"" : "")
                    ));
                    arguments.Append('"');
                }
                if (carg + 1 < args.Length)
                    arguments.Append(' ');
            }
            return arguments.ToString();
        }
    }
}
