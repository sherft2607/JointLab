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
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Meta.XR.MRUtilityKit.BuildingBlocks
{
    /// <summary>
    /// Base class to locate space in physical environment for Instant Content Placement Building Block.
    /// </summary>
    /// <remarks>See <see cref="EnvironmentRaycastManager"/> more information for ray-casting on physical environment.</remarks>
    public abstract class SpaceLocator : MonoBehaviour
    {
        public SurfaceOrientation PreferredSurfaceOrientation = SurfaceOrientation.Vertical | SurfaceOrientation.HorizontalFaceUp | SurfaceOrientation.HorizontalFaceDown;

        [Tooltip("Use CustomSize instead of local scale of Target")]
        [SerializeField] public bool UseCustomSize;

        [Tooltip("Size the of the space to locate")]
        [SerializeField] public Vector3 CustomSize = Vector3.one * 0.25f;

        [Space]
        [Tooltip("This event will trigger when a suitable space is located within user's physical environment")]
        [Space][SerializeField] private UnityEvent<Pose, bool> _onSpaceLocateCompleted = new();

        /// <summary>
        /// Origin of raycast ray
        /// </summary>
        protected virtual Transform RaycastOrigin { get; set; }

        /// <summary>
        /// Maximum length of raycast ray
        /// </summary>
        protected virtual float MaxRaycastDistance { get; set; } = 100f;

        /// <summary>
        /// This event will trigger when a suitable space is located within user's physical environment.
        /// </summary>
        public UnityEvent<Pose, bool> OnSpaceLocateCompleted
        {
            get => _onSpaceLocateCompleted;
            set => _onSpaceLocateCompleted = value;
        }

        /// <summary>
        /// Resulted raycast hit information
        /// </summary>
        public EnvironmentRaycastHit RaycastHitResult => _raycastHit;

        private EnvironmentRaycastManager _raycastManager;
        private EnvironmentRaycastHit _raycastHit;

        private const float VerticalSurfaceAngleThreshold = 0.3f;
        private const float HorizontalSurfaceAngleThreshold = 0.7f;
        private const float NormalConfidenceThreshold = 0.4f;
        private Vector3 _sizeToLocate;

        private void Start() => _raycastManager = FindFirstObjectByType<EnvironmentRaycastManager>();

        protected internal abstract Ray GetRaycastRay();

        /// <summary>
        /// Locate a space in physical environment
        /// </summary>
        /// <param name="surfacePose">Located surface pose data.</param>
        /// <returns>True if a space is located successfully, otherwise, false.</returns>
        protected virtual bool TryLocateSpace(out Pose surfacePose)
        {
            surfacePose = default;
            var ray = GetRaycastRay();
            var hasHit = _raycastManager.Raycast(ray, out var hit, MaxRaycastDistance);
            hasHit &= hit.normalConfidence > NormalConfidenceThreshold;
            if (!hasHit)
            {
                OnSpaceLocateCompleted?.Invoke(default, false);
                return false;
            }

            if (PreferredSurfaceOrientation != SurfaceOrientation.Any)
            {
                var currentSurfaceType = GetSurfaceOrientation(hit.normal);
                if ((currentSurfaceType & PreferredSurfaceOrientation) == 0)
                {
                    OnSpaceLocateCompleted?.Invoke(default, false);
                    return false;
                }
            }

            var success = TryCalculateSurfacePose(hit, ray, out surfacePose);
            OnSpaceLocateCompleted?.Invoke(surfacePose, success);
            return success;
        }

        private bool TryCalculateSurfacePose(EnvironmentRaycastHit hit, Ray ray, out Pose surfacePose)
        {
            surfacePose = default;
            var upwards = CalculateUpwardFromPlacementSide(hit, transform, ray);

            var bounds = Utilities.GetPrefabBounds(transform.gameObject);
            var estimatedSizeToLocate = bounds?.size ?? Vector3.one * EnvironmentDepthManagerRaycastExtensions.MinXYSize;
            var targetSize = UseCustomSize ? CustomSize : estimatedSizeToLocate;

            if (!_raycastManager.PlaceBox(ray, targetSize, upwards, out _raycastHit))
            {
                return false;
            }

            Vector3 forward = Vector3.Cross(_raycastHit.normal, Vector3.Cross(transform.forward, _raycastHit.normal).normalized);
            var rotation = Quaternion.LookRotation(forward, _raycastHit.normal);

            surfacePose = new Pose(_raycastHit.point, rotation);
            return true;
        }

        private Vector3 CalculateUpwardFromPlacementSide(EnvironmentRaycastHit hit, Transform rayOrigin, Ray ray)
        {
            return IsVertical(hit.normal) ? Vector3.up : Vector3.ProjectOnPlane(rayOrigin.up, Vector3.Cross(ray.direction, Vector3.up));
        }

        /// <summary>
        /// Defines different types of surfaces within a user environment
        /// </summary>
        [Flags]
        public enum SurfaceOrientation
        {
            None = 0,
            Any = 1,
            Vertical = 2,
            HorizontalFaceUp = 4,
            HorizontalFaceDown = 8
        }

        private static bool IsVertical(Vector3 normal) => Mathf.Abs(Vector3.Dot(normal, Vector3.up)) < VerticalSurfaceAngleThreshold;
        private static bool IsHorizontalDown(Vector3 normal) => Vector3.Dot(normal, Vector3.down) > HorizontalSurfaceAngleThreshold;
        private static bool IsHorizontalUp(Vector3 normal) => Vector3.Dot(normal, Vector3.up) > HorizontalSurfaceAngleThreshold;

        private static SurfaceOrientation GetSurfaceOrientation(Vector3 normal)
        {
            var currentSurfaceType = (SurfaceOrientation)0;
            if (IsHorizontalDown(normal)) currentSurfaceType = SurfaceOrientation.HorizontalFaceDown;
            else if (IsHorizontalUp(normal)) currentSurfaceType = SurfaceOrientation.HorizontalFaceUp;
            else if (IsVertical(normal)) currentSurfaceType = SurfaceOrientation.Vertical;
            return currentSurfaceType;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SpaceLocator), true)]
    internal class SpaceLocatorEditor : Editor {
        public override void OnInspectorGUI()
        {
            var targetObject = target as SpaceLocator;
            if(targetObject != null && targetObject.UseCustomSize)
            {
                DrawDefaultInspector();
            }
            else
            {
                DrawPropertiesExcluding(serializedObject, nameof(SpaceLocator.CustomSize));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif // UNITY_EDITOR
}
