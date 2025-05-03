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

using Meta.XR.BuildingBlocks;
using Meta.XR.MultiplayerBlocks.Colocation;
using System;
using UnityEngine;
using Logger = Meta.XR.MultiplayerBlocks.Colocation.Logger;
namespace Meta.XR.MultiplayerBlocks.Shared
{
    /// <summary>
    /// This <see cref="MonoBehaviour"/> is responsible for handling colocation events generated
    /// from <see cref="LocalMatchmaking"/> so we can sequentially complete the colocation process:
    /// As host: upon session created, we create alignment anchor and share with the group
    /// As guest: upon session discovered, we query the shared anchor from group and align camera with it
    /// Both scenarios will report colocation ready to the <see cref="ColocationController.ColocationReadyCallbacks"/>.
    ///
    /// For more information about the group-based Shared Spatial Anchors, checkout the [official documentation](https://developers.meta.com/horizon/documentation/unity/unity-shared-spatial-anchors/#understanding-group-based-vs-user-based-spatial-anchor-sharing-and-loading)
    /// </summary>
    public class ColocationSessionEventHandler : MonoBehaviour
    {
        [SerializeField]
        private GameObject AnchorPrefab;
        private ColocationController _colocationController;
        private SharedAnchorManager _sharedAnchorManager;
        private AlignCameraToAnchor _alignCameraToAnchor;
        private OVRCameraRig _cameraRig;

        private void Awake()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            _colocationController = FindObjectOfType<ColocationController>();
            _cameraRig = FindObjectOfType<OVRCameraRig>();
#pragma warning restore CS0618 // Type or member is obsolete
        }

        private void Start()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var ssaCore = FindObjectOfType<SharedSpatialAnchorCore>();
#pragma warning restore CS0618 // Type or member is obsolete
            if (ssaCore == null)
            {
                throw new InvalidOperationException($"{nameof(SharedSpatialAnchorCore)} component is missing " +
                                                    "from the scene, add this component to allow anchor sharing.");
            }
            _sharedAnchorManager = new SharedAnchorManager(ssaCore);
            if (_colocationController.DebuggingOptions.visualizeAlignmentAnchor)
            {
                _sharedAnchorManager.AnchorPrefab = AnchorPrefab;
            }
            LocalMatchmaking.OnSessionCreateSucceeded.AddListener(OnSessionCreated);
            LocalMatchmaking.OnSessionDiscoverSucceeded.AddListener(OnSessionDiscovered);
        }

        // event handling as host
        private async void OnSessionCreated(Guid groupUuid)
        {
            _ = await _sharedAnchorManager.CreateAlignmentAnchor();
            if (await _sharedAnchorManager.ShareAnchorsWithGroup(groupUuid))
            {
                _colocationController.ColocationReadyCallbacks?.Invoke();
                Logger.Log("Host has created and shared the alignment anchor, " +
                           "and is ready for colocation", LogLevel.Info);
            }
        }

        // event handling as guest
        private async void OnSessionDiscovered(Guid groupUuid)
        {
            var anchors = await _sharedAnchorManager.RetrieveAnchorsFromGroup(groupUuid);
            if (anchors.Count != 0)
            {
                // align camera to anchors
                var alignCamera = _cameraRig.gameObject.AddComponent<AlignCameraToAnchor>();
                alignCamera.CameraAlignmentAnchor = anchors[0];
                alignCamera.RealignToAnchor();
                _colocationController.ColocationReadyCallbacks?.Invoke();
                Logger.Log("Guest has retrieved and aligned with the alignment anchor, " +
                           "and is ready for colocation", LogLevel.Info);
            }
        }

        private void OnDestroy()
        {
            LocalMatchmaking.OnSessionCreateSucceeded.RemoveListener(OnSessionCreated);
            LocalMatchmaking.OnSessionDiscoverSucceeded.RemoveListener(OnSessionDiscovered);
        }
    }
}
