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
    /// Place and anchor a GameObject to the world, so it doesn't drift away over time.
    /// </summary>
    /// <remarks>
    /// This component is designed to work with <see cref="SpaceLocator"/> components, but it could be used
    /// without <see cref="SpaceLocator"/> components.
    /// </remarks>
    public class PlaceWithAnchor : MonoBehaviour
    {
        [Tooltip("Target transform to place")]
        public Transform Target;

        private Transform _spatialAnchorTransform;
        private OVRSpatialAnchor _spatialAnchor;
        private bool _requestMove;
        private Pose _surfacePose;

        private void Awake()
        {
            _spatialAnchorTransform = new GameObject($"[{gameObject.name}] Spatial Anchor").transform;
            Target ??= transform;
        }

        /// <summary>
        /// Request to move and place object in next Update loop.
        /// </summary>
        /// <param name="pose">Target pose for the transform.</param>
        public void RequestMove(Pose pose)
        {
            _requestMove = true;
            _surfacePose = pose;
        }

        private void Update()
        {
            if (_requestMove && _surfacePose != default)
            {
                SetTargetWithAnchor(_surfacePose);
                _requestMove = false;
            }
        }

        /// <summary>
        /// Listener for <see cref="SpaceLocator.OnSpaceLocateCompleted"/> event.
        /// </summary>
        /// <param name="surfacePose">Pose of the located space</param>
        /// <param name="success">Result of space locate</param>
        public void OnLocateSpace(Pose surfacePose, bool success)
        {
            if (!success)
            {
                Debug.Log($"[{nameof(PlaceWithAnchor)}] Failed to locate space.");
                return;
            }

            RequestMove(surfacePose);
        }

        private void SetTargetWithAnchor(Pose pose)
        {
            EraseAnchor();
            Target.SetParent(null);
            Target.SetPositionAndRotation(pose.position, pose.rotation);
            SetAnchor();
        }

        private void EraseAnchor()
        {
            if (_spatialAnchorTransform.TryGetComponent(out _spatialAnchor))
            {
                _spatialAnchor.EraseAnchorAsync();
                DestroyImmediate(_spatialAnchor);
            }
        }

        private void SetAnchor()
        {
            _spatialAnchorTransform.transform.SetPositionAndRotation(transform.position, transform.rotation);
            _spatialAnchor = _spatialAnchorTransform.gameObject.AddComponent<OVRSpatialAnchor>();
            Target.SetParent(_spatialAnchorTransform.transform);
        }
    }
}
