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

namespace Meta.XR.Simulator.Editor.SyntheticEnvironments
{
    /// <summary>
    /// Describes an instance of a Synthetic Environment,
    /// Technically, an argument passed to a Synthetic Environment Server.
    /// </summary>
    /// <remarks>
    /// Call <see cref="Meta.XR.Simulator.Editor.SyntheticEnvironments.Registry.Register"/> to
    /// add a Synthetic Environment to the list of available Synthetic Environment.
    /// </remarks>
    /// <seealso cref="Meta.XR.Simulator.Editor.SyntheticEnvironments.Registry.Register"/>
    /// <seealso cref="Meta.XR.Simulator.Editor.SyntheticEnvironments.Server.Start"/>
    internal class SyntheticEnvironment
    {
        public string Name;
        public string InternalName;
        public string ServerBinaryPath;

        // NOTE: This is a hack to support the deprecated .synthenv path
        // TODO: T203651870 Remove this once we have migrated to the new synth_env_server~ path
        public string ServerBinaryPathWithDot = null;

        /// <summary>
        /// Launch the associated Synthetic Environment Server, passing this Synthetic Environment as argument.
        /// </summary>
        /// <param name="stopExisting">Whether or not a previously existing instance of the Synthetic Environment
        /// <param name="createWindow">Whether or not a window should be created to display the Synthetic Environment.</param>
        /// Server should be stopped first.</param>
        public void Launch(bool stopExisting, bool createWindow)
        {
            SyntheticEnvironmentServer.Start(InternalName, ServerBinaryPath, ServerBinaryPathWithDot, stopExisting, createWindow);
            LocalSharingServer.Start(stopExisting, createWindow);
        }

        internal int Index => Registry.RegisteredEnvironments.IndexOf(this);

        public override string ToString()
        {
            return Name + ": " + ServerBinaryPath + ":" + InternalName;
        }
    }
}
