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

using Meta.XR.Util;
using UnityEngine;

namespace Meta.XR.MRUtilityKit.SceneDecorator
{

    /// <summary>
    /// A mask that samples the distance between an anchor and a component
    /// </summary>
    [Feature(Feature.Scene)]
    public class AnchorComponentDistanceMask : Mask
    {
        /// <summary>
        /// Axis representation
        /// </summary>
        public enum Axis
        {
            X = 0,
            Y = 1,
            Z = 2
        }

        /// <summary>
        /// The axis to sample the distance along
        /// </summary>
        [SerializeField]
        public Axis axis;

        /// <summary>
        /// The distance with the configured axis.
        /// </summary>
        /// <param name="c">The candidate for a location</param>
        /// <returns>Distance</returns>
        public override float SampleMask(Candidate c)
        {
            return c.anchorCompDists[(int)axis];
        }

        /// <summary>
        /// Not used on this mask
        /// </summary>
        /// <param name="c">The candidate</param>
        /// <returns>true</returns>
        public override bool Check(Candidate c)
        {
            return true;
        }
    }
}
