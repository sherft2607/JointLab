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

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Meta.XR.MRUtilityKit.SceneDecorator
{
    /// <summary>
    /// A mask that can be used to filter out objects from the scene.
    /// </summary>
    [Feature(Feature.Scene)]
    public abstract class Mask : ScriptableObject
    {
        /// <summary>
        /// Executes the mask on a candidate and returns the value.
        /// </summary>
        /// <param name="candidate">Candidate with the information from the distribution</param>
        /// <returns>The float depending on the mask</returns>
        public abstract float SampleMask(Candidate candidate);

        /// <summary>
        /// Checks if the candidate is valid for the given mask
        /// </summary>
        /// <param name="c">Candidate with the information from the distribution</param>
        /// <returns>true if valid</returns>
        public abstract bool Check(Candidate c);

        /// <summary>
        /// Executes the mask on a candidate with scale and offset and returns the value.
        /// </summary>
        /// <param name="candidate">Candidate with the information from the distribution</param>
        /// <param name="scale">The scale to apply before the mask is applied. This can be used for example if you want to have a range of values that are all multiplied by 10. </param>
        /// <param name="offset">The offset to apply before the mask is applied.</param>
        /// <returns>A float depending on the mask </returns>
        public float SampleMask(Candidate candidate, float scale, float offset = 0f)
        {
            return scale * SampleMask(candidate) + offset;
        }

        /// <summary>
        /// A mask that can be used to filter out objects from the scene.
        /// </summary>
        /// <param name="candidate">Contains the decoration prefab and the values from the distribution algorithm</param>
        /// <param name="limitMin">Minimum to fullfill from the mask</param>
        /// <param name="limitMax">Maximum to fullfill from the mask</param>
        /// <param name="scale">Scale the value from the mask</param>
        /// <param name="offset">Add offset from the mask</param>
        /// <returns></returns>
        public float SampleMask(Candidate candidate, float limitMin, float limitMax, float scale = 1f, float offset = 0f)
        {
            var value = scale * SampleMask(candidate) + offset;
            return (limitMin > limitMax)
                ? Mathf.Clamp(limitMin - value, limitMax, limitMin)
                : Mathf.Clamp(value, limitMin, limitMax);
        }

#if UNITY_EDITOR
        [ContextMenu("Delete")]
        private void Delete()
        {
            Undo.DestroyObjectImmediate(this);
            AssetDatabase.SaveAssets();
        }
#endif
    }
}
