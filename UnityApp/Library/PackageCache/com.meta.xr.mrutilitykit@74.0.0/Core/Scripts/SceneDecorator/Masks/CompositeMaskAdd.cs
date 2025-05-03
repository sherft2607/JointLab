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
using Meta.XR.Util;
using UnityEngine;

namespace Meta.XR.MRUtilityKit.SceneDecorator
{
    /// <summary>
    /// A mask that combines multiple other masks and adds their values together.
    /// </summary>
    [Feature(Feature.Scene)]
    public class CompositeMaskAdd : Mask2D
    {
        /// <summary>
        /// The settings for the mask layer.
        /// </summary>
        [Serializable]
        public struct MaskLayer
        {
            public Mask mask; /// <summary> The mask to use for this layer. </summary>
            public float outputScale; /// <summary> The scale of the mask output. </summary>
            public float outputLimitMin; /// <summary> The minimum value of the mask output. </summary>
            public float outputLimitMax; /// <summary> The maximum value of the mask output. </summary>
            public float outputOffset; /// <summary> The offset of the mask output. </summary>

            /// <summary>
            /// The constructor for the mask layer.
            /// </summary>
            /// <param name="c">Candidate with the information from the distribution</param>
            /// <returns>The adjusted and clamped value</returns>
            public float SampleMask(Candidate c)
            {
                return mask.SampleMask(c, outputLimitMin, outputLimitMax, outputScale, outputOffset);
            }
        }

        [SerializeField]
        private MaskLayer[] maskLayers;

        /// <summary>
        /// This method applies the mask to the given candidate.
        /// It first generates an affine transformation based on the provided parameters,
        /// and then applies this transformation to the local position of the candidate.
        /// The resulting position is used to sample the mask layers, which are combined
        /// to produce a final value that represents the mask for the candidate.
        /// </summary>
        /// <param name="c">The candidate to apply the mask to.</param>
        /// <returns>The mask value for the given candidate.</returns>
        public override float SampleMask(Candidate c)
        {
            var affineTransform = GenerateAffineTransform(offsetX, offsetY, rotation, scaleX, scaleY, shearX, shearY);
            var tuv = Float3X3.Multiply(affineTransform, new Vector3(c.localPos.x, c.localPos.y, 1f));
            tuv /= tuv.z;
            c.localPos = new Vector2(tuv.x, tuv.y);

            var value = 0f;
            foreach (var layer in maskLayers)
            {
                value += layer.SampleMask(c);
            }

            return value;
        }

        public override bool Check(Candidate c)
        {
            return true;
        }
    }
}
