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

using UnityEngine;

namespace Meta.XR.MRUtilityKit.BuildingBlocks
{
    /// <summary>
    /// Point a position in physical environment and place the Target prefab
    /// </summary>
    public class PointAndLocate : SpaceLocator
    {
        [Tooltip("Assign a Transform to use that as raycast origin")]
        [SerializeField] internal Transform _raycastOrigin;

        /// <summary>
        /// Transform to use that as raycast origin
        /// </summary>
        protected override Transform RaycastOrigin => _raycastOrigin;

        /// <summary>
        /// Cast a ray from <see cref="RaycastOrigin"/> and place the <see cref="Target"/> object.
        /// </summary>
        public void Locate() => TryLocateSpace(out _);

        protected internal override Ray GetRaycastRay() => new(RaycastOrigin.position, RaycastOrigin.forward);
    }
}
