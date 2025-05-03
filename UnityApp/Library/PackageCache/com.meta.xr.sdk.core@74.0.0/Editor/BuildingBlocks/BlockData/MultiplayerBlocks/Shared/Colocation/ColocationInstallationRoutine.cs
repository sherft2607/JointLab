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

using Meta.XR.BuildingBlocks.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Meta.XR.MultiplayerBlocks.Shared.Editor
{
    public class ColocationInstallationRoutine : NetworkInstallationRoutine
    {
        [SerializeField]
        [Variant(Behavior = VariantAttribute.VariantBehavior.Definition,
            Description = "[Recommended] Use Bluetooth & WiFi based ColocationSession API (with Local Matchmaking block) " +
                          "to colocate players, then use networking framework for gameplay network sync.",
            Default = true)]
        private bool useColocationSession = true; // if this is chosen, don't need to show matchmaking options
        protected override bool CanInstallMatchmaking() => !useColocationSession;
        protected override bool RequireNetworkImplementationChoice() => !useColocationSession;
        internal override IEnumerable<BlockData> ComputeOptionalDependencies()
        {
            return useColocationSession ?
                new[] { Utils.GetBlockData(BlockDataIds.LocalMatchmaking) } :
                new[] { Utils.GetBlockData(BlockDataIds.PlatformInit) }.Concat(base.ComputeOptionalDependencies());
        }
    }
}
