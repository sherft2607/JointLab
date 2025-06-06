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
using Meta.XR.Util;
using UnityEngine;

namespace Meta.XR.MRUtilityKit.SceneDecorator
{
    /// <summary>
    /// A mask that uses the SpaceMapGPU to determine if a point is in or out of an object.
    /// </summary>
    [Feature(Feature.Scene)]
    public class SpaceMapGPUMask : Mask
    {
        private SpaceMapGPU spaceMap;
        public override float SampleMask(Candidate candidate)
        {
            if (spaceMap == null)
            {
                spaceMap = FindAnyObjectByType<SpaceMapGPU>();
                if (spaceMap == null)
                {
                    Debug.LogWarning($"SpaceMapGPU cannot be found, does it exist in the Scene?");
                    return 0.0f;
                }
            }

            var value = spaceMap.GetColorAtPosition(candidate.hit.point);
            return value.r;
        }

        public override bool Check(Candidate c)
        {
            return true;
        }
    }
}
