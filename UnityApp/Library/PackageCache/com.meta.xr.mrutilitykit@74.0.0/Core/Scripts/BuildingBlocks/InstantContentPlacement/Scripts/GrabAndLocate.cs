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

#if META_INTERACTION_SDK_DEFINED
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
#endif // META_INTERACTION_SDK_DEFINED

using UnityEngine;

namespace Meta.XR.MRUtilityKit.BuildingBlocks
{
    /// <summary>
    /// Grab and place the Target prefab in physical environment
    /// </summary>
    public class GrabAndLocate : SpaceLocator
    {
#if META_INTERACTION_SDK_DEFINED
        private HandGrabInteractable _handGrabInteractable;
        private GrabInteractable _grabInteractable;
        private PlaceWithAnchor _placeWithAnchor;
        private OVRCameraRig _cameraRig;
        private bool _requestMove;

        protected override Transform RaycastOrigin => transform;
        protected override float MaxRaycastDistance => 3f;

        public void Awake()
        {
            _handGrabInteractable = GetComponentInChildren<HandGrabInteractable>();
            _grabInteractable = GetComponentInChildren<GrabInteractable>();
            _placeWithAnchor = GetComponent<PlaceWithAnchor>();
            _cameraRig = FindFirstObjectByType<OVRCameraRig>();
        }

        private void OnEnable()
        {
            _handGrabInteractable.WhenStateChanged += OnInteractableStateChanged;
            _grabInteractable.WhenStateChanged += OnInteractableStateChanged;
        }

        private void OnDisable()
        {
            _handGrabInteractable.WhenStateChanged -= OnInteractableStateChanged;
            _grabInteractable.WhenStateChanged -= OnInteractableStateChanged;
        }

        private void OnInteractableStateChanged(InteractableStateChangeArgs stateChange)
        {
            if (stateChange.PreviousState == InteractableState.Select)
            {
                TryLocateSpace(out _);
            }
        }
#endif // META_INTERACTION_SDK_DEFINED

        protected internal override Ray GetRaycastRay()
        {
            // It works better if we keep the ray cast origin little bit further from the surface
            var origin = transform.position + transform.up * 0.5f;
            var direction = -transform.up;
            return new Ray(origin, direction);
        }
    }
}
