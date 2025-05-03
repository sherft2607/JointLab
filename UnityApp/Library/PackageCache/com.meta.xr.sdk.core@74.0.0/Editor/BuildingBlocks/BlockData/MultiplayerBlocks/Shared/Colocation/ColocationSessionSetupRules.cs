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

using UnityEditor;
namespace Meta.XR.BuildingBlocks.Editor
{
    /// <summary>
    /// Setup rule for Colocation Session. It depends on the
    /// <see cref="OVRColocationSession"/> API that leverages the OpenXR
    /// extension for Colocation Session, hence a feature support is needed to use it.
    /// However, as the API is mostly static, we cannot guarantee generic usage
    /// of the API with this rule. So only check for Local Matchmaking Building Blocks usage.
    ///
    /// For more information about the <see cref="OVRColocationSession"/> API, checkout the [official documentation](https://developers.meta.com/horizon/documentation/unity/unity-colocation-discovery)
    /// </summary>
    [InitializeOnLoad]
    public class ColocationSessionSetupRules
    {
        static ColocationSessionSetupRules()
        {
            OVRProjectSetup.AddTask(
                level: OVRProjectSetup.TaskLevel.Required,
                group: OVRProjectSetup.TaskGroup.Features,
                isDone: _ => !LocalMatchmakingBlockExists ||
                             OVRProjectConfig.CachedProjectConfig.colocationSessionSupport != OVRProjectConfig.FeatureSupport.None,
                message: "When using Local Matchmaking in your project it's required to enable the Colocation Session capability in the project config",
                fix: _ =>
                {
                    var projectConfig = OVRProjectConfig.CachedProjectConfig;
                    projectConfig.colocationSessionSupport = OVRProjectConfig.FeatureSupport.Supported;
                    OVRProjectConfig.CommitProjectConfig(projectConfig);
                },
                fixMessage: "Enable Colocation Session support in the project config"
            );
        }

        private static bool LocalMatchmakingBlockExists => Utils.GetBlock(BlockDataIds.LocalMatchmaking) != null;
    }
}
