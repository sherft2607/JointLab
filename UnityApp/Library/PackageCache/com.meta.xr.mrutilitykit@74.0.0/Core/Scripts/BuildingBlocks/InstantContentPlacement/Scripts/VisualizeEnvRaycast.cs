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
    /// Visualizes the raycast ray and the ray hit point
    /// </summary>
    public class VisualizeEnvRaycast : MonoBehaviour
    {
        [SerializeField, Tooltip("Supply a LineRenderer to visualize the raycast ray")] private LineRenderer _raycastLine;
        [SerializeField, Tooltip("Supply a Transform to see the ray hit point")] private Transform _raycastHitPoint;

        [SerializeField] internal SpaceLocator _spaceLocator;
        private EnvironmentRaycastManager _raycastManager;

        private void Awake() => _raycastManager = FindFirstObjectByType<EnvironmentRaycastManager>();

        private void Update() => VisualizeRay();

        private void VisualizeRay()
        {
            if (_raycastManager == null)
            {
                return;
            };
            var ray = _spaceLocator.GetRaycastRay();
            bool hasHit = _raycastManager.Raycast(ray, out var hit) || hit.status == EnvironmentRaycastHitStatus.HitPointOccluded;
            bool hasNormal = hit.normalConfidence > 0f;

            _raycastLine.enabled = hasHit;
            _raycastHitPoint.gameObject.SetActive(hasHit && hasNormal);

            if (_raycastLine != null)
            {
                _raycastLine.SetPosition(0, ray.origin);
                _raycastLine.SetPosition(1, hit.point);
            }

            if (_raycastHitPoint != null && hasNormal)
            {
                _raycastHitPoint.SetPositionAndRotation(hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
    }
}
