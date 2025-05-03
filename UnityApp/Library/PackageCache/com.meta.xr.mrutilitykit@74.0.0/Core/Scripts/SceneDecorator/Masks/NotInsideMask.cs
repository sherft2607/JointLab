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
    /// A mask that returns true if the object is not inside a sceneelement with the specified label
    /// </summary>
    [Feature(Feature.Scene)]
    public class NotInsideMask : Mask
    {
        /// <summary>
        /// Define which Labels should be checked for this mask.
        /// </summary>
        [SerializeField] public MRUKAnchor.SceneLabels Labels;

        /// <summary>
        /// This is not used in this mask
        /// </summary>
        /// <param name="c">Candidate with the information from the distribution</param>
        /// <returns>Not used in this mask, always 0</returns>
        public override float SampleMask(Candidate c)
        {
            return 0;
        }

        /// <summary>
        /// Checks if the hit point of the candidate is inside an MRUKAnchor
        /// </summary>
        /// <param name="c">Candidate with the information from the distribution</param>
        /// <returns>The adjusted and clamped value</returns>
        public override bool Check(Candidate c)
        {
            var bounds = Utilities.GetPrefabBounds(c.decorationPrefab);
            foreach (var room in MRUK.Instance.Rooms)
            {
                var isInVolume = room.IsPositionInSceneVolume(c.hit.point, out var anchor, true, bounds.Value.extents.x);
                if (anchor != null)
                {
                    if (anchor.HasAnyLabel(Labels))
                    {
                        return !isInVolume;
                    }
                }
            }

            return true;
        }
    }
}
