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

using Meta.XR.MRUtilityKit.Extensions;
using Meta.XR.Util;
using UnityEngine;

namespace Meta.XR.MRUtilityKit.SceneDecorator
{
    /// <summary>
    /// A mask that uses a cookie texture to determine the opacity of each pixel.
    /// </summary>
    [Feature(Feature.Scene)]
    public class CookieMask : Mask2D
    {
        /// <summary>
        /// The enumerator to define the samplemode. See each item for the description.
        /// </summary>
        public enum SampleMode
        {
            NEAREST = 0x0, /// <summary>Nearest neighbor</summary>
            NEAREST_REPEAT = 0x1, /// <summary>Nearest neighbor, with repeat</summary>
            NEAREST_REPEAT_MIRROR = 0x2, /// <summary>Nearest neighbor, with repeat and mirror</summary>
            BILINEAR = 0x3, /// <summary>Bilinear</summary>
            BILINEAR_REPEAT = 0x4, /// <summary>Bilinear, with repeat</summary>
            BILINEAR_REPEAT_MIRROR = 0x5 /// <summary>Bilinear, with repeat and mirror</summary>
        }

        /// <summary>
        /// The cookie texture to use for the mask.
        /// </summary>
        [SerializeField]
        public Texture2D cookie;

        /// <summary>
        /// The sample mode to use for the mask.
        /// </summary>
        [SerializeField]
        public SampleMode sampleMode;

        /// <summary>
        /// This method applies the cookie mask to the given candidate.
        /// It first generates an affine transformation based on the provided parameters,
        /// and then applies this transformation to the local position of the candidate.
        /// The resulting position is used to sample the cookie texture using the specified sample mode,
        /// and the resulting value is returned as the mask value for the candidate.
        /// </summary>
        /// <param name="c">The candidate to apply the mask to.</param>
        /// <returns>The mask value for the given candidate.</returns>
        public override float SampleMask(Candidate c)
        {
            var affineTransform = GenerateAffineTransform(offsetX, offsetY, rotation, scaleX, scaleY, shearX, shearY);
            var tuv = Float3X3.Multiply(affineTransform, Vector3Extensions.FromVector2AndZ(c.localPos, 1f));
            tuv /= tuv.z;
            var uv = new Vector2(tuv.x, tuv.y);

            float value;
            switch (sampleMode)
            {
                default:
                case SampleMode.NEAREST:
                    uv *= new Vector2(cookie.width, cookie.height);
                    value = (tuv.x < 0f | tuv.x > 1f | tuv.y < 0f | tuv.y > 1f) ? 0f : cookie.GetPixel((int)uv.x, (int)uv.y).r;
                    break;
                case SampleMode.NEAREST_REPEAT:
                    uv = uv.Frac();
                    uv *= new Vector2(cookie.width, cookie.height);
                    value = cookie.GetPixel((int)uv.x, (int)uv.y).r;
                    break;
                case SampleMode.NEAREST_REPEAT_MIRROR:
                    uv = 2f * (uv - uv.Add(0.5f).Floor()).Abs();
                    uv *= new Vector2(cookie.width, cookie.height);
                    value = cookie.GetPixel((int)uv.x, (int)uv.y).r;
                    break;
                case SampleMode.BILINEAR:
                    value = (uv.x < 0f | uv.x > 1f | uv.y < 0f | uv.y > 1f) ? 0f : cookie.GetPixelBilinear(uv.x, uv.y).r;
                    break;
                case SampleMode.BILINEAR_REPEAT:
                    uv = uv.Frac();
                    value = cookie.GetPixelBilinear(uv.x, uv.y).r;
                    break;
                case SampleMode.BILINEAR_REPEAT_MIRROR:
                    uv = 2f * (uv - uv.Add(0.5f).Floor()).Abs();
                    value = cookie.GetPixelBilinear(uv.x, uv.y).r;
                    break;
            }

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
