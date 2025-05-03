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
using System.IO;
using System.Text.RegularExpressions;

namespace Meta.XR.Simulator
{
    internal class ProcessPort
    {
        public override string ToString()
        {
            return $"{this.processName}({this.processId}) ({this.protocol} port {this.portNumber})";
        }

        public string processName { get; set; }
        public int processId { get; set; }
        public string portNumber { get; set; }
        public string protocol { get; set; }

        private static string LookupProcess(int pid)
        {
            string procName;
            try
            {
                procName = Process.GetProcessById(pid).ProcessName;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError(ex);
                procName = "-";
            }

            return procName;
        }

        public static List<ProcessPort> GetProcessesByPort(string targetPort)
        {
            var ports = new List<ProcessPort>();

            using (Process p = new Process())
            {
                var ps = new ProcessStartInfo();
#if UNITY_EDITOR_OSX
                    // on macos, use `lsof -b -n -P -iTCP:33792 -sTCP:LISTEN` instead of netstat
                    ps.Arguments = $"-t -n -P -iTCP:{targetPort} -sTCP:LISTEN";
                    ps.FileName = "lsof";
#elif UNITY_EDITOR_WIN
                    ps.Arguments = "-a -n -o";
                    ps.FileName = "netstat.exe";
#else
                return ports;
#endif
                ps.UseShellExecute = false;
                ps.WindowStyle = ProcessWindowStyle.Hidden;
                ps.RedirectStandardInput = true;
                ps.RedirectStandardOutput = true;
                ps.RedirectStandardError = true;

                p.StartInfo = ps;
                p.Start();

                StreamReader stdOutput = p.StandardOutput;
                StreamReader stdError = p.StandardError;

                string content = stdOutput.ReadToEnd() + stdError.ReadToEnd();
                p.WaitForExit();
                int exitStatus = p.ExitCode;
#if UNITY_EDITOR_OSX
                if(exitStatus == 1){
                    // According lsof man page, lsof will return 1 if there is no processes on specified port.
                    return ports;
                }
#endif
                if (exitStatus != 0)
                {
                    UnityEngine.Debug.LogError($"{ps.FileName} {ps.Arguments} call failed, exitCode={exitStatus}, content={content}");
                    return ports;
                }
#if UNITY_EDITOR_WIN
                Regex lineRE = new Regex("\r\n");
#else
                Regex lineRE = new Regex("\n");
#endif
                string[] rows = lineRE.Split(content);
                foreach (string row in rows)
                {
                    int processId = 0;
                    string protocol = "", portNumber = "";
#if UNITY_EDITOR_OSX
                        if(String.IsNullOrEmpty(row))
                        {
                            continue;
                        }
                        processId = Convert.ToInt32(row);
                        protocol = "TCPv6";
                        portNumber = targetPort;
#elif UNITY_EDITOR_WIN

                        Regex tokenRE = new Regex("\\s+");
                        Regex localAddressRE = new Regex(@"\[(.*?)\]");
                        string[] tokens = tokenRE.Split(row);
                        if (tokens.Length > 4 && (tokens[1].Equals("UDP") || tokens[1].Equals("TCP")))
                        {
                            string localAddress = localAddressRE.Replace(tokens[2], "1.1.1.1");
                            portNumber = localAddress.Split(':')[1];
                            if (targetPort != portNumber)
                            {
                                continue;
                            }

                            processId = 0;
                            try
                            {
                                processId = tokens[1].Equals("UDP")
                                    ? Convert.ToInt32(tokens[4])
                                    : Convert.ToInt32(tokens[5]);

                                protocol = localAddress.Contains("1.1.1.1")
                                    ? String.Format("{0}v6", tokens[1])
                                    : String.Format("{0}v4", tokens[1]);
                            }
                            catch (Exception ex)
                            {
                                UnityEngine.Debug.LogError(tokens[1] + " " + tokens[4] + " " + tokens[5]);
                                throw ex;
                            }
                        }
#endif
                    if (processId != 0)
                    {
                        string processName = LookupProcess(processId);
                        UnityEngine.Debug.Log($"Found {protocol} port {portNumber} used by {processName}({processId})");
                        ports.Add(new ProcessPort
                        {
                            protocol = protocol,
                            portNumber = portNumber,
                            processName = processName,
                            processId = processId
                        });
                    }
                }
            }

            return ports;
        }
    }
}
