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

/************************************************************************************
 * Filename    :   MetaXRAcousticBuildPreprocessor.cs
 * Content     :   Removes MetaXRAudioUnity binary from builds when using FMOD/Wwise
 ***********************************************************************************/

using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

static class MetaXRAcousticBuildProcessor
{
    class Preprocessor : IPreprocessBuildWithReport
    {
        readonly string[] runtimePluginNames =
        {
            "libMetaXRAudioUnity.so",
            "MetaXRAudioUnity.dll",
        };

        public int callbackOrder => 0;

        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
        {
            SetRuntimePluginCopyDelegate();
        }

        void SetRuntimePluginCopyDelegate()
        {
            var allPlugins = PluginImporter.GetAllImporters();
            foreach (var plugin in allPlugins)
            {
                if (plugin.isNativePlugin)
                {
                    foreach (var pluginName in runtimePluginNames)
                    {
                        if (plugin.assetPath.Contains(pluginName))
                        {
                            plugin.SetIncludeInBuildDelegate(ShouldIncludeRuntimePluginsInBuild);
                            break;
                        }
                    }
                }
            }
        }

        static bool ShouldIncludeRuntimePluginsInBuild(string path)
        {
            if (path.Contains("MetaXRAudioUnity"))
            {
                const string AudioManagerAssetPath = "ProjectSettings/AudioManager.asset";
                SerializedObject audioManager = new SerializedObject(UnityEditor.AssetDatabase.LoadAllAssetsAtPath(AudioManagerAssetPath)[0]);
                SerializedProperty spatializerPluginProperty = audioManager.FindProperty("m_DisableAudio");
                bool disableUnityAudio = spatializerPluginProperty.boolValue;
                if (disableUnityAudio)
                {
                    Debug.Log("Detected Unity Audio is disabled, excluding MetaXRAudioUnity plugin from build");
                }
                return !disableUnityAudio;
            }

            return true;
        }
    }
}
