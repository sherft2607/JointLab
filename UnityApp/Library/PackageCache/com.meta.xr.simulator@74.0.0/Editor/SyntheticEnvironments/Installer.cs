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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine.Networking;

namespace Meta.XR.Simulator.Editor.SyntheticEnvironments
{
    internal static class Installer
    {
        private const string Name = "Synthetic Environments";

        const string Version = "v74";

#if UNITY_EDITOR_OSX
        readonly static string AppDataFolderPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Application Support");
#else
        static readonly string AppDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
#endif
        static readonly string XrSimDataFolderPath = Path.Join(Path.Join(AppDataFolderPath, "MetaXR", "MetaXrSimulator"), Version);

        private static bool? _isInstalled;
        public static bool IsInstalled => _isInstalled ??= Directory.Exists(XrSimDataFolderPath);

        public static event Action OnInstalled;

#if UNITY_EDITOR_OSX
        readonly static Regex EnvScriptArgs = new Regex(@"open -n (\S+) --args (\S+)");
#else
        readonly static Regex EnvScriptArgs = new Regex(@"start (\S+) (\S+)");
#endif

        public static List<SyntheticEnvironment> BuildSyntheticEnvironments()
        {
            var synthEnvs = new List<SyntheticEnvironment>();

            // Find all scripts in the package and create a SyntheticEnvironment for each\
            var subDirs = Directory.EnumerateDirectories(XrSimDataFolderPath);
            foreach (var dir in subDirs)
            {
#if UNITY_EDITOR_OSX
                var files = Directory.EnumerateFiles(dir, "*.sh", SearchOption.AllDirectories);
#else
                var files = Directory.EnumerateFiles(dir, "*.bat", SearchOption.AllDirectories);
#endif
                foreach (var file in files)
                {
                    // ReportInfo(Name, "found " + file);
                    // Parse the file
                    var env = ParseSynthEnvScript(file);
                    if (env == null) { continue; }

                    env.ServerBinaryPath = Path.Join(dir, env.ServerBinaryPath);
                    // ReportInfo(Name, "parsed " + env);
                    synthEnvs.Add(env);
                }
            }

            return synthEnvs;
        }

        private static SyntheticEnvironment ParseSynthEnvScript(string file)
        {
            StreamReader reader = File.OpenText(file);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                // ReportInfo(Name, "checking " + line);
                var match = EnvScriptArgs.Match(line);
                if (match.Success)
                {
                    Utils.ReportInfo(Name, "got match " + match);
                    return new SyntheticEnvironment()
                    {
                        Name = Path.GetFileNameWithoutExtension(file),
                        ServerBinaryPath = match.Groups[1].Value,
                        InternalName = match.Groups[2].Value,
                    };
                }
            }
            return null;
        }

        private static UnityWebRequest _webRequest = null;

        public static void InstallSESPackage()
        {
            EditorCoroutineUtility.StartCoroutine(Installer.InstallSesPackage(XrSimDataFolderPath), typeof(Installer));
        }

        private static IEnumerator InstallSesPackage(string installDir)
        {
#if UNITY_EDITOR_OSX
            const string Platform = "MAC";
#else
            const string Platform = "WIN";
#endif

            var DownloadMap = new[] {
                new[]{"v74", "WIN", "8778927728892025"},
                new[]{"v74", "MAC", "28012755145005979"},
            };

            string downloadId = "";
            foreach (var e in DownloadMap)
            {
                if (e[0] == Version && e[1] == Platform)
                {
                    downloadId = e[2];
                    break;
                }
            }

            if (downloadId == "")
            {
                Utils.DisplayDialogOrError(Name, "failed to find downloadId for " + Version + " " + Platform);
                yield break;
            }

            // following https://discussions.unity.com/t/downloadhandlerbuffer-data-gc-allocation-problems/704758/8
            string url = string.Format("https://securecdn.oculus.com/binaries/download/?id={0}", downloadId);

            // TODO: T203804698 save to Downloads folder instead of temp folder...
            string savePath = Path.Join(Path.GetTempPath(), downloadId + ".zip");

            // check if file exists before downloading it again
            if (!File.Exists(savePath))
            {
                int progressId = Progress.Start(Name);
                Progress.ShowDetails(false);
                yield return null;

                _webRequest = UnityWebRequest.Get(url);
                var handler = new DownloadHandlerFile(savePath);
                _webRequest.downloadHandler = handler;
                handler.removeFileOnAbort = true;

                // ReportInfo(Name, "starting to download " + url);
                UnityWebRequestAsyncOperation operation = _webRequest.SendWebRequest();
                operation.completed += operation =>
                {
                    Utils.ReportInfo(Name, "finished downloading " + url);
                };

                while (!_webRequest.downloadHandler.isDone)
                {
                    Progress.Report(progressId, _webRequest.downloadProgress, "Downloading package");
                    yield return null;
                }

                if (_webRequest.result != UnityWebRequest.Result.Success)
                {
                    Utils.DisplayDialogOrError(Name, _webRequest.error);
                    yield break;
                }

                Utils.ReportInfo(Name, "finished saving data to " + savePath);

                _webRequest = null;
                Progress.Remove(progressId);
            }
            else
            {
                Utils.ReportInfo(Name, "found " + savePath + ", skipping download");
            }

            // ensure normalized path
            if (!installDir.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
            {
                installDir += Path.DirectorySeparatorChar;
            }

            if (Directory.Exists(installDir))
            {
                // Ensure directory is deleted before extracting
                Directory.Delete(installDir);
            }

            // Remove quarantine attribute from downloaded file
#if UNITY_EDITOR_OSX
            {
                const string Attribute = "com.apple.provenance";
                var (retCode, contents) = ExecuteProcess("xattr", new string[] { "-d", Attribute, savePath });
                if(retCode != 0)
                {
                    Utils.ReportError(Name, string.Format("failed to remove {0}, retCode={1}, contents={2}", Attribute, retCode, contents));
                }
            }
#endif

            // ReportInfo(Name, "extracting " + savePath + " to " + installDir);
            // unzip
            using (ZipArchive archive = ZipFile.OpenRead(savePath))
            {
                int progressId = Progress.Start(Name);
                Progress.ShowDetails(false);
                yield return null;

                int numEntries = archive.Entries.Count;
                float entryIndex = -1;
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    entryIndex++;
                    if (entry.FullName.EndsWith("/"))
                    {
                        continue;
                    }

                    // Gets the full path to ensure that relative segments are removed.
                    string destinationPath = Path.GetFullPath(Path.Combine(installDir, entry.FullName));

                    // Ordinal match is safest, case-sensitive volumes can be mounted within volumes that
                    // are case-insensitive.
                    if (!destinationPath.StartsWith(installDir, StringComparison.Ordinal))
                    {
                        continue;
                    }
                    // create directory if it doesn't exist
                    var parentDir = Path.GetDirectoryName(destinationPath);
                    if (!Directory.Exists(parentDir))
                    {
                        Directory.CreateDirectory(parentDir);
                    }

                    entry.ExtractToFile(destinationPath, true);

#if UNITY_EDITOR_OSX
                    // Update the file attributes for file
                    string subString = "";
                    int extendedAttributes = entry.ExternalAttributes >> 16;
                    if(extendedAttributes != 0) {
                        string attrString = Convert.ToString(extendedAttributes, 8);
                        subString = attrString.Substring(attrString.Length - 4);
                    } else if(entry.FullName.Contains("SyntheticEnvironmentServer")) {
                        // This is a hack to set the executable bit on the SyntheticEnvironmentServer binary
                        subString = "750";
                    }
                    if(subString.Length > 0) {
                        Utils.ReportInfo(Name, "setting permissions on " + destinationPath + " to " + subString);
                        var (retCode, contents) = ExecuteProcess("chmod", new string[] { subString, destinationPath });
                        if(retCode != 0)
                        {
                            Utils.ReportError(Name, "failed to set permissions on " + destinationPath + ", retCode:" + retCode + ", contents:" + contents);
                        }
                    }
#endif
                    // ReportInfo(Name, "Extracted File:" + entry.FullName + ", ExternalAttributes:0" + Convert.ToString((entry.ExternalAttributes >> 16), 8));
                    Progress.Report(progressId, entryIndex / numEntries, "Extraction progress");
                    yield return null;
                }

                Progress.Remove(progressId);
            }

            Utils.ReportInfo(Name, "finished extracting " + savePath + " to " + installDir);


            _isInstalled = true;
            OnInstalled?.Invoke();
        }

        private static (int retCode, string contents) ExecuteProcess(string path, string[] args)
        {
            using (Process p = new Process())
            {
                var ps = new ProcessStartInfo();

                ps.Arguments = Utils.EscapeArguments(args);
                ps.FileName = path;
                ps.UseShellExecute = false;
                ps.WindowStyle = ProcessWindowStyle.Hidden;
                ps.RedirectStandardInput = true;
                ps.RedirectStandardOutput = true;
                ps.RedirectStandardError = true;

                // ReportInfo(Name, "Executing: " + path + " " + ps.Arguments);

                p.StartInfo = ps;
                p.Start();

                StreamReader stdOutput = p.StandardOutput;
                StreamReader stdError = p.StandardError;

                string content = stdOutput.ReadToEnd() + stdError.ReadToEnd();
                p.WaitForExit();
                int retCode = p.ExitCode;
                return (retCode, content);
            }
        }
    }
}
