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
    /// A mask that uses cellular noise to generate a 2D grid of points.
    /// </summary>
    [Feature(Feature.Scene)]
    public class CellularNoiseMask : Mask2D
    {
        /// <summary>
        /// This method generates a sample mask for the given candidate.
        /// It applies an affine transformation to the local position of the candidate,
        /// and then uses the Worley noise function to generate a value between 0 and 1.
        /// The resulting value is used as the sample mask for the candidate.
        /// </summary>
        /// <param name="c">The candidate for which to generate the sample mask.</param>
        /// <returns>The sample mask value for the given candidate.</returns>
        public override float SampleMask(Candidate c)
        {
            var affineTransform = GenerateAffineTransform(offsetX, offsetY, rotation, scaleX, scaleY, shearX, shearY);
            var tuv = Float3X3.Multiply(affineTransform, new Vector3(c.localPos.x, c.localPos.y, 1f));
            tuv /= tuv.z;

            float value = Mathf.Abs(WorleyNoise.cellular(new Vector2(tuv.x, tuv.z)).x);

            return value;
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
