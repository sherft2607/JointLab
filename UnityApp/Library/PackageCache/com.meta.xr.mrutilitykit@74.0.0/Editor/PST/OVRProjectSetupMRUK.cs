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

using Meta.XR.ImmersiveDebugger;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace Meta.XR.MRUtilityKit.Editor
{
    [InitializeOnLoad]
    internal static class OVRProjectSetupMRUK
    {
        static OVRProjectSetupMRUK()
        {
            // Add components that depend on DepthAPI to PST

            // Scene requirement support
            OVRProjectSetup.AddTask(
                level: OVRProjectSetup.TaskLevel.Required,
                group: OVRProjectSetup.TaskGroup.Features,
                isDone: buildTargetGroup =>
                {
                    var ovrCameraRig = FindComponentInScene<OVRCameraRig>();
                    var sceneSupport = OVRProjectConfig.CachedProjectConfig.sceneSupport;
                    return
                        ovrCameraRig == null ||
                        sceneSupport == OVRProjectConfig.FeatureSupport.Supported ||
                        sceneSupport == OVRProjectConfig.FeatureSupport.Required;
                },
                message: "MR Utility Kit recommends Scene Support to be set to \"Required\"",
                fix: buildTargetGroup =>
                {
                    var projectConfig = OVRProjectConfig.CachedProjectConfig;
                    projectConfig.sceneSupport = OVRProjectConfig.FeatureSupport.Required;
                    projectConfig.anchorSupport = OVRProjectConfig.AnchorSupport.Enabled;
                    OVRProjectConfig.CommitProjectConfig(projectConfig);
                },
                fixMessage: "Set Scene Support to \"Required\" in the project config"
            );

            // Immersive Scene Debugger prefab reference
            OVRProjectSetup.AddTask(
                level: OVRProjectSetup.TaskLevel.Recommended,
                group: OVRProjectSetup.TaskGroup.Features,
                isDone: buildTargetGroup =>
                {
                    var mruk = FindComponentInScene<MRUK>();
                    return !RuntimeSettings.Instance.ImmersiveDebuggerEnabled || mruk == null || mruk._immersiveSceneDebuggerPrefab != null;
                },
                message: "ImmersiveSceneDebugger prefab reference is missing in MRUK component. Scene debug will be unavailable in Immersive Debugger.",
                fix: buildTargetGroup => ReferenceImmersiveSceneDebuggerFromAsset(),
                fixMessage: "Set ImmersiveSceneDebugger prefab reference in MRUK component"
            );
        }

        private static void ReferenceImmersiveSceneDebuggerFromAsset()
        {
            var mruk = FindComponentInScene<MRUK>();
            const string pathToAsset = "Packages/com.meta.xr.mrutilitykit/Core/Tools/ImmersiveSceneDebugger.prefab";
            mruk._immersiveSceneDebuggerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(pathToAsset);
        }

        private static T FindComponentInScene<T>() where T : Component
        {
            var scene = SceneManager.GetActiveScene();
            var rootGameObjects = scene.GetRootGameObjects();
            foreach (var rootGameObject in rootGameObjects)
            {
                if (rootGameObject.TryGetComponent(out T foundComponent))
                {
                    return foundComponent;
                }
            }
            return null;
        }
    }
}
