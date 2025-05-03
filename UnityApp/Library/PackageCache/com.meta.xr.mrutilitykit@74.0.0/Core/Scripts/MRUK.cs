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
using System.Collections.Generic;
using System.Threading.Tasks;
using Meta.XR.Util;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Assertions;
using UnityEngine.Diagnostics;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Meta.XR.ImmersiveDebugger;

namespace Meta.XR.MRUtilityKit
{
    /// <summary>
    ///     This class contains convenience functions that allow you to
    ///     query your scene. It also contains functions to load a scene from prefabs or json to work within the Unity editor.
    /// </summary>
    [Feature(Feature.Scene)]
    public partial class MRUK : MonoBehaviour
    {
        /// <summary>
        /// when interacting specifically with tops of volumes, this can be used to
        /// specify where the return position should be aligned on the surface
        /// e.g. some apps  might want a position right in the center of the table (chess)
        /// for others, the edge may be more important (piano or pong)
        /// </summary>
        public enum PositioningMethod
        {
            DEFAULT,
            CENTER,
            EDGE
        }

        /// <summary>
        ///     Specify the source of the scene data.
        /// </summary>
        public enum SceneDataSource
        {
            /// <summary>
            ///     Load scene data from the device.
            /// </summary>
            Device,

            /// <summary>
            ///     Load scene data from prefabs.
            /// </summary>
            Prefab,

            /// <summary>
            ///     First try to load data from the device and if none can be found
            ///     fall back to loading from a prefab.
            /// </summary>
            DeviceWithPrefabFallback,

            /// <summary>
            ///     Load Scene from a Json file
            /// </summary>
            Json,

            /// <summary>
            ///     First try to load data from the device and if none can be found
            ///     fall back to loading from a Json file
            /// </summary>
            DeviceWithJsonFallback,
        }

        /// <summary>
        ///     Specifies the filtering options for selecting rooms within the scene data.
        /// </summary>
        public enum RoomFilter
        {
            None,
            CurrentRoomOnly,
            AllRooms,
        };

        /// <summary>
        ///     Return value from the call to LoadSceneFromDevice
        /// </summary>
        public enum LoadDeviceResult
        {
            /// <summary>
            ///     Scene data loaded successfully.
            /// </summary>
            Success = OVRAnchor.FetchResult.Success,

            /// <summary>
            ///     User did not grant scene permissions.
            /// </summary>
            NoScenePermission = 1,

            /// <summary>
            ///     No rooms were found (e.g. User did not go through space setup)
            /// </summary>
            NoRoomsFound = 2,

            /// <summary>
            ///     Invalid data.
            /// </summary>
            FailureDataIsInvalid = OVRAnchor.FetchResult.FailureDataIsInvalid,

            /// <summary>
            ///     Resource limitation prevented this operation from executing.
            /// </summary>
            /// <remarks>
            ///     Recommend retrying, perhaps after a short delay and/or reducing memory consumption.
            /// </remarks>
            FailureInsufficientResources = OVRAnchor.FetchResult.FailureInsufficientResources,

            /// <summary>
            ///     Insufficient view.
            /// </summary>
            /// <remarks>
            ///     The user needs to look around the environment more for anchor tracking to function.
            /// </remarks>
            FailureInsufficientView = OVRAnchor.FetchResult.FailureInsufficientView,

            /// <summary>
            ///     Insufficient permission.
            /// </summary>
            /// <remarks>
            ///     Recommend confirming the status of the required permissions needed for using anchor APIs.
            /// </remarks>
            FailurePermissionInsufficient = OVRAnchor.FetchResult.FailurePermissionInsufficient,

            /// <summary>
            ///     Operation canceled due to rate limiting.
            /// </summary>
            /// <remarks>
            ///     Recommend retrying after a short delay.
            /// </remarks>
            FailureRateLimited = OVRAnchor.FetchResult.FailureRateLimited,

            /// <summary>
            ///     Too dark.
            /// </summary>
            /// <remarks>
            ///     The environment is too dark to load the anchor.
            /// </remarks>
            FailureTooDark = OVRAnchor.FetchResult.FailureTooDark,

            /// <summary>
            ///     Too bright.
            /// </summary>
            /// <remarks>
            ///     The environment is too bright to load the anchor.
            /// </remarks>
            FailureTooBright = OVRAnchor.FetchResult.FailureTooBright,
        };

        /// <summary>
        /// This struct is used to manage which rooms and anchors are not being tracked.
        /// </summary>
        internal struct SceneTrackingSettings
        {
            internal HashSet<MRUKRoom> UnTrackedRooms;
            internal HashSet<MRUKAnchor> UnTrackedAnchors;
        }

        /// <summary>
        ///  Defines flags for different types of surfaces that can be identified or used within a scene.
        /// </summary>
        [Flags]
        public enum SurfaceType
        {
            FACING_UP = 1 << 0,
            FACING_DOWN = 1 << 1,
            VERTICAL = 1 << 2,
        };

        [Flags]
        private enum AnchorRepresentation
        {
            PLANE = 1 << 0,
            VOLUME = 1 << 1,
        }

        /// <summary>
        /// Gets a value indicating whether the component has been initialized.
        /// </summary>
        public bool IsInitialized
        {
            get;
            private set;
        } = false;

        /// <summary>
        ///     Event that is triggered when the scene is loaded.
        /// </summary>
        [field: SerializeField, FormerlySerializedAs(nameof(SceneLoadedEvent))]
        public UnityEvent SceneLoadedEvent
        {
            get;
            private set;
        } = new();

        /// <summary>
        ///     Event that is triggered when a room is created.
        /// </summary>
        [field: SerializeField, FormerlySerializedAs(nameof(RoomCreatedEvent))]
        public UnityEvent<MRUKRoom> RoomCreatedEvent
        {
            get;
            private set;
        } = new();

        /// <summary>
        ///     Event that is triggered when a room is updated.
        /// </summary>
        [field: SerializeField, FormerlySerializedAs(nameof(RoomUpdatedEvent))]
        public UnityEvent<MRUKRoom> RoomUpdatedEvent
        {
            get;
            private set;
        } = new();

        /// <summary>
        ///     Event that is triggered when a room is removed.
        /// </summary>
        [field: SerializeField, FormerlySerializedAs(nameof(RoomRemovedEvent))]
        public UnityEvent<MRUKRoom> RoomRemovedEvent
        {
            get;
            private set;
        } = new();

        /// <summary>
        ///     When world locking is enabled the position and rotation of the OVRCameraRig/TrackingSpace transform will be adjusted each frame to ensure
        ///     the room anchors are where they should be relative to the camera position. This is necessary to
        ///     ensure the position of the virtual objects in the world do not get out of sync with the real world.
        /// </summary>
        public bool EnableWorldLock = true;

        /// <summary>
        ///     This property indicates if world lock is currently active. This means that <see cref="EnableWorldLock"/> is enabled and the current
        ///     room has successfully been localized. In some cases such as when the headset goes into standby, the user moves to another room and
        ///     comes back out of standby the headset may fail to localize and this property will be false. The property may become true again once
        ///     the device is able to localise the room again (e.g. if the user walks back into the original room). This property can be used to show
        ///     a UI panel asking the user to return to their room.
        /// </summary>
        public bool IsWorldLockActive => EnableWorldLock && _worldLockActive;

        /// <summary>
        ///     When the <see cref="EnableWorldLock"/> is enabled, MRUK will modify the TrackingSpace transform, overwriting any manual changes.
        ///     Use this field to change the position and rotation of the TrackingSpace transform when the world locking is enabled.
        /// </summary>
        [HideInInspector]
        public Matrix4x4 TrackingSpaceOffset = Matrix4x4.identity;

        internal OVRCameraRig _cameraRig { get; private set; }
        private bool _worldLockActive = false;
        private bool _worldLockWasEnabled = false;
        private bool _loadSceneCalled = false;
        private Pose? _prevTrackingSpacePose = default;
        private readonly List<OVRSemanticLabels.Classification> _classificationsBuffer = new List<OVRSemanticLabels.Classification>(1);

        /// <summary>
        ///     This is the final event that tells developer code that Scene API and MR Utility Kit have been initialized, and that the room can be queried.
        /// </summary>
        void InitializeScene()
        {
            try
            {
                SceneLoadedEvent.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            IsInitialized = true;
        }

        /// <summary>
        ///     Register to receive a callback when the scene is loaded. If the scene is already loaded
        ///     at the time this is called, the callback will be invoked immediatly.
        /// </summary>
        /// <param name="callback"></param>
        public void RegisterSceneLoadedCallback(UnityAction callback)
        {
            SceneLoadedEvent.AddListener(callback);
            if (IsInitialized)
            {
                callback();
            }
        }

        /// <summary>
        ///     Register to receive a callback when a new room has been created from scene capture.
        /// </summary>
        /// <param name="callback">
        ///     - `MRUKRoom` The created room object.
        /// </param>
        [Obsolete("Use UnityEvent RoomCreatedEvent directly instead")]
        public void RegisterRoomCreatedCallback(UnityAction<MRUKRoom> callback)
        {
            RoomCreatedEvent.AddListener(callback);
        }

        /// <summary>
        ///     Register to receive a callback when a room has been updated from scene capture.
        /// </summary>
        /// <param name="callback">
        ///     - `MRUKRoom` The updated room object.
        /// </param>
        [Obsolete("Use UnityEvent RoomUpdatedEvent directly instead")]
        public void RegisterRoomUpdatedCallback(UnityAction<MRUKRoom> callback)
        {
            RoomUpdatedEvent.AddListener(callback);
        }

        /// <summary>
        ///     Registers a callback function to be called before the room is removed.
        /// </summary>
        /// <param name="callback">
        ///     The function to be called when the room is removed. It takes one parameter:
        ///     - `MRUKRoom` The removed room object.
        /// </param>
        [Obsolete("Use UnityEvent RoomRemovedEvent directly instead")]
        public void RegisterRoomRemovedCallback(UnityAction<MRUKRoom> callback)
        {
            RoomRemovedEvent.AddListener(callback);
        }

        /// <summary>
        /// Get a list of all the rooms in the scene.
        /// </summary>
        /// <returns>A list of MRUKRoom objects representing all the rooms in the scene.</returns>
        [Obsolete("Use Rooms property instead")]
        public List<MRUKRoom> GetRooms() => Rooms;

        /// <summary>
        /// Get a flat list of all Anchors in the scene
        /// </summary>
        /// <returns>A list of MRUKAnchor objects representing all the anchors in the current room.</returns>
        [Obsolete("Use GetCurrentRoom().Anchors instead")]
        public List<MRUKAnchor> GetAnchors() => GetCurrentRoom().Anchors;

        /// <summary>
        ///  Returns the current room the headset is in. If the headset is not in any given room
        ///  then it will return the room the headset was last in when this function was called.
        ///  If the headset hasn't been in a valid room yet then return the first room in the list.
        ///  If no rooms have been loaded yet then return null.
        /// </summary>
        /// <returns>The current <see cref="MRUKRoom"/> based on the headset's position, or null if no rooms are available.</returns>
        public MRUKRoom GetCurrentRoom()
        {
            // This is a rather expensive operation, we should only do it at most once per frame.
            if (_cachedCurrentRoomFrame != Time.frameCount)
            {
                if (_cameraRig?.centerEyeAnchor.position is Vector3 eyePos)
                {
                    MRUKRoom currentRoom = null;
                    foreach (var room in Rooms)
                    {
                        if (room.IsPositionInRoom(eyePos, false))
                        {
                            currentRoom = room;
                            // In some cases the user may be in multiple rooms at once. If this happens
                            // then we give precedence to rooms which have been loaded locally
                            if (room.IsLocal)
                            {
                                break;
                            }
                        }
                    }
                    if (currentRoom != null)
                    {
                        _cachedCurrentRoom = currentRoom;
                        _cachedCurrentRoomFrame = Time.frameCount;
                        return currentRoom;
                    }
                }
            }

            if (_cachedCurrentRoom != null)
            {
                return _cachedCurrentRoom;
            }

            if (Rooms.Count > 0)
            {
                return Rooms[0];
            }

            return null;
        }

        /// <summary>
        ///     Checks whether any anchors can be loaded.
        /// </summary>
        /// <returns>
        ///     Returns a task-based bool, which is true if
        ///     there are any scene anchors in the system, and false
        ///     otherwise. If false is returned, then either
        ///     the scene permission needs to be set, or the user
        ///     has to run Scene Capture.
        /// </returns>
        public static async Task<bool> HasSceneModel()
        {
            var rooms = new List<OVRAnchor>();
            var result = await OVRAnchor.FetchAnchorsAsync(rooms, new OVRAnchor.FetchOptions
            {
                SingleComponentType = typeof(OVRRoomLayout)
            });
            return result.Success && rooms.Count > 0;
        }

        /// <summary>
        /// Represents the settings for the MRUK instance,
        /// including data source configurations, startup behaviors, and other scene related settings.
        /// </summary>
        [Serializable]
        public partial class MRUKSettings
        {
            [Header("Data Source settings")]
            [SerializeField, Tooltip("Where to load the data from.")]
            public SceneDataSource DataSource = SceneDataSource.Device;

            [SerializeField, Tooltip("The index (0-based) into the RoomPrefabs or SceneJsons array; -1 is random.")]
            public int RoomIndex = -1;

            [SerializeField, Tooltip("The list of prefab rooms to use.")]
            public GameObject[] RoomPrefabs;

            [SerializeField, Tooltip("The list of JSON text files with scene data to use. Uses RoomIndex")]
            public TextAsset[] SceneJsons;

            [Space]
            [Header("Startup settings")]
            [SerializeField, Tooltip("Trigger a scene load on startup. If set to false, you can call LoadSceneFromDevice(), LoadSceneFromPrefab() or LoadSceneFromJsonString() manually.")]
            public bool LoadSceneOnStartup = true;

            [Space]
            [Header("Other settings")]
            [SerializeField, Tooltip("The width of a seat. Used to calculate seat positions with the COUCH label.")]
            public float SeatWidth = 0.6f;


            // SceneJson has been replaced with `TextAsset[] SceneJsons` defined above
            [SerializeField, HideInInspector, Obsolete]
            internal string SceneJson;
        }

        [Tooltip("Contains all the information regarding data loading.")]
        public MRUKSettings SceneSettings;

        MRUKRoom _cachedCurrentRoom = null;
        int _cachedCurrentRoomFrame = 0;


        /// <summary>
        ///     List of all the rooms in the scene.
        /// </summary>
        public List<MRUKRoom> Rooms
        {
            get;
        } = new();


        /// <summary>
        /// Gets the singleton instance of the MRUK class.
        /// </summary>
        public static MRUK Instance
        {
            get;
            private set;
        }

        [SerializeField] internal GameObject _immersiveSceneDebuggerPrefab;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.Assert(false, "There should be only one instance of MRUK!");
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            MRUKNative.LoadMRUKSharedLibrary();

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += change =>
            {
                if (change == PlayModeStateChange.ExitingPlayMode)
                {
                    // Free the shared library when exiting play mode so that it can be recompiled
                    MRUKNative.FreeMRUKSharedLibrary();
                }
            };
#endif

            if (SceneSettings == null)
            {
                return;
            }

            if (SceneSettings.LoadSceneOnStartup)
            {
                // We can't await for the LoadScene result because Awake is not async, silence the warning
#pragma warning disable CS4014
#if !UNITY_EDITOR && UNITY_ANDROID
                // If we are going to load from device we need to ensure we have permissions first
                if ((SceneSettings.DataSource == SceneDataSource.Device || SceneSettings.DataSource == SceneDataSource.DeviceWithPrefabFallback || SceneSettings.DataSource == SceneDataSource.DeviceWithJsonFallback) &&
                    !Permission.HasUserAuthorizedPermission(OVRPermissionsRequester.ScenePermission))
                {
                    var callbacks = new PermissionCallbacks();
                    callbacks.PermissionDenied += permissionId =>
                    {
                        Debug.LogWarning("User denied permissions to use scene data");
                        // Permissions denied, if data source is using prefab fallback let's load the prefab or Json scene instead
                        if (SceneSettings.DataSource == SceneDataSource.DeviceWithPrefabFallback)
                        {
                            LoadScene(SceneDataSource.Prefab);
                        }
                        else if (SceneSettings.DataSource == SceneDataSource.DeviceWithJsonFallback)
                        {
                            LoadScene(SceneDataSource.Json);
                        }
                    };
                    callbacks.PermissionGranted += permissionId =>
                    {
                        // Permissions are now granted and it is safe to try load the scene now
                        LoadScene(SceneSettings.DataSource);
                    };
                    // Note: If the permission request dialog is already active then this call will silently fail
                    // and we won't receive the callbacks. So as a work-around there is a code in Update() to mitigate
                    // this problem.
                    Permission.RequestUserPermission(OVRPermissionsRequester.ScenePermission, callbacks);
                }
                else
#endif
                {
                    LoadScene(SceneSettings.DataSource);
                }
#pragma warning restore CS4014
            }

            // Activate SceneDebugger when ImmersiveDebugger is enabled.
            if (RuntimeSettings.Instance.ImmersiveDebuggerEnabled && _immersiveSceneDebuggerPrefab != null)
            {
                var sceneDebuggerExist = ImmersiveSceneDebugger.Instance != null;
                if (!sceneDebuggerExist)
                {
                    Instantiate(_immersiveSceneDebuggerPrefab);
                }
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
                RoomCreatedEvent.RemoveAllListeners();
                RoomRemovedEvent.RemoveAllListeners();
                RoomUpdatedEvent.RemoveAllListeners();
                SceneLoadedEvent.RemoveAllListeners();
            }
        }

        private void Start()
        {
            if (!_cameraRig)
            {
                _cameraRig = FindAnyObjectByType<OVRCameraRig>();
            }

        }

        private void Update()
        {
            if (SceneSettings.LoadSceneOnStartup && !_loadSceneCalled)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                // This is to cope with the case where the permissions dialog was already opened before we called
                // Permission.RequestUserPermission in Awake() and we don't get the PermissionGranted callback
                if (Permission.HasUserAuthorizedPermission(OVRPermissionsRequester.ScenePermission))
                {
                    // We can't await for the LoadScene result because Awake is not async, silence the warning
#pragma warning disable CS4014
                    LoadScene(SceneSettings.DataSource);
#pragma warning restore CS4014
                }
#endif
            }

            bool worldLockActive = false;

            if (_cameraRig)
            {
                if (EnableWorldLock)
                {
                    var room = GetCurrentRoom();
                    if (room)
                    {
                        if (room.UpdateWorldLock(out Vector3 position, out Quaternion rotation))
                        {
                            if (_prevTrackingSpacePose is Pose pose && (_cameraRig.trackingSpace.position != pose.position || _cameraRig.trackingSpace.rotation != pose.rotation))
                            {
                                Debug.LogWarning("MRUK EnableWorldLock is enabled and is controlling the tracking space position.\n" +
                                                 $"Tracking position was set to {_cameraRig.trackingSpace.position} and rotation to {_cameraRig.trackingSpace.rotation}, this is being overridden by MRUK.\n" +
                                                 $"Use '{nameof(TrackingSpaceOffset)}' instead to translate or rotate the TrackingSpace.");
                            }

                            position = TrackingSpaceOffset.MultiplyPoint3x4(position);
                            rotation = TrackingSpaceOffset.rotation * rotation;
                            _cameraRig.trackingSpace.SetPositionAndRotation(position, rotation);
                            _prevTrackingSpacePose = new(position, rotation);
                            worldLockActive = true;
                        }
                    }
                }
                else if (_worldLockWasEnabled)
                {
                    // Reset the tracking space when disabling world lock
                    _cameraRig.trackingSpace.localPosition = Vector3.zero;
                    _cameraRig.trackingSpace.localRotation = Quaternion.identity;
                    _prevTrackingSpacePose = null;
                }

                _worldLockWasEnabled = EnableWorldLock;
            }

            _worldLockActive = worldLockActive;

            UpdateTrackables();
        }

        /// <summary>
        ///     Load the scene asynchronously from the specified data source
        /// </summary>
        async Task LoadScene(SceneDataSource dataSource)
        {
            _loadSceneCalled = true;
            try
            {
                if (dataSource == SceneDataSource.Device ||
                    dataSource == SceneDataSource.DeviceWithPrefabFallback ||
                    dataSource == SceneDataSource.DeviceWithJsonFallback)
                {
                    await LoadSceneFromDevice();
                }

                if (dataSource == SceneDataSource.Prefab ||
                    (dataSource == SceneDataSource.DeviceWithPrefabFallback && Rooms.Count == 0))
                {
                    if (SceneSettings.RoomPrefabs.Length == 0)
                    {
                        Debug.LogWarning($"Failed to load room from prefab because prefabs list is empty");
                        return;
                    }

                    // Clone the roomPrefab, but essentially replace all its content
                    // if -1 or out of range, use a random one
                    var roomIndex = GetRoomIndex(true);

                    Debug.Log($"Loading prefab room {roomIndex}");

                    var roomPrefab = SceneSettings.RoomPrefabs[roomIndex];
                    LoadSceneFromPrefab(roomPrefab);
                }

                if (dataSource == SceneDataSource.Json ||
                    (dataSource == SceneDataSource.DeviceWithJsonFallback && Rooms.Count == 0))
                {
                    if (SceneSettings.SceneJsons.Length != 0)
                    {
                        var roomIndex = GetRoomIndex(false);

                        Debug.Log($"Loading SceneJson {roomIndex}");

                        var ta = SceneSettings.SceneJsons[roomIndex];
                        LoadSceneFromJsonString(ta.text);
                    }
#pragma warning disable CS0612 // Type or member is obsolete
                    else if (SceneSettings.SceneJson != "")
#pragma warning restore CS0612 // Type or member is obsolete
                    {
#pragma warning disable CS0612 // Type or member is obsolete
                        LoadSceneFromJsonString(SceneSettings.SceneJson);
#pragma warning restore CS0612 // Type or member is obsolete
                    }
                    else
                    {
                        Debug.LogWarning($"The list of SceneJsons is empty");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                throw;
            }
        }

        private int GetRoomIndex(bool fromPrefabs = true)
        {
            var idx = SceneSettings.RoomIndex;
            if (idx == -1)
            {
                idx = UnityEngine.Random.Range(0, fromPrefabs ? SceneSettings.RoomPrefabs.Length : SceneSettings.SceneJsons.Length);
            }

            return idx;
        }

        /// <summary>
        ///     Called when the room is destroyed
        /// </summary>
        /// <remarks>
        ///     This is used to keep the list of active rooms up to date.
        ///     So there should never be any null entries in the list.
        /// </remarks>
        /// <param name="room"></param>
        internal void OnRoomDestroyed(MRUKRoom room)
        {
            Rooms.Remove(room);
            if (_cachedCurrentRoom == room)
            {
                _cachedCurrentRoom = null;
            }
        }

        /// <summary>
        ///     Destroys the rooms and all children
        /// </summary>
        public void ClearScene()
        {
            foreach (var room in Rooms)
            {
                foreach (Transform child in room.transform)
                {
                    Destroy(child.gameObject);
                }

                Destroy(room.gameObject);
            }

            Rooms.Clear();
            _cachedCurrentRoom = null;
        }

        /// <summary>
        /// Loads the scene based on scene data previously shared with the user via
        /// <see cref="MRUKRoom.ShareRoomAsync"/>.
        /// </summary>
        /// <remarks>
        ///
        /// This function should be used in co-located multi-player experiences by "guest"
        /// clients that require scene data previously shared by the "host".
        ///
        /// </remarks>
        /// <param name="roomUuids">A collection of UUIDs of room anchors for which scene data will be loaded from the given group context.</param>
        /// <param name="groupUuid">UUID of the group from which to load the shared rooms.</param>
        /// <param name="alignmentData">Use this parameter to correctly align local and host coordinates when using co-location.<br/>
        /// alignmentRoomUuid: the UUID of the room used for alignment.<br/>
        /// floorWorldPoseOnHost: world-space pose of the FloorAnchor on the host device.<br/>
        /// Using 'null' will disable the alignment, causing the mismatch between the host and the guest. Do this only if your app has custom coordinate alignment.</param>
        /// <param name="removeMissingRooms">
        ///     When enabled, rooms that are already loaded but are not found in roomUuids will be removed.
        ///     This is to support the case where a user deletes a room from their device and the change needs to be reflected in the app.
        /// </param>
        /// <returns>An enum indicating whether loading was successful or not.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="roomUuids"/> is `null`.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="groupUuid"/> equals `Guid.Empty`.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="alignmentData.alignmentRoomUuid"/> equals `Guid.Empty`.</exception>
        public async Task<LoadDeviceResult> LoadSceneFromSharedRooms(IEnumerable<Guid> roomUuids, Guid groupUuid, (Guid alignmentRoomUuid, Pose floorWorldPoseOnHost)? alignmentData, bool removeMissingRooms = true)
        {
            if (roomUuids == null)
            {
                throw new ArgumentNullException(nameof(roomUuids));
            }

            if (groupUuid == Guid.Empty)
            {
                throw new ArgumentException(nameof(groupUuid));
            }

            if (alignmentData?.alignmentRoomUuid == Guid.Empty)
            {
                throw new ArgumentException(nameof(alignmentData.Value.alignmentRoomUuid));
            }

            return await LoadSceneFromDeviceInternal(requestSceneCaptureIfNoDataFound: false, removeMissingRooms, new SharedRoomsData { roomUuids = roomUuids, groupUuid = groupUuid, alignmentData = alignmentData });
        }

        private struct SharedRoomsData
        {
            internal IEnumerable<Guid> roomUuids;
            internal Guid groupUuid;
            internal (Guid alignmentRoomUuid, Pose floorWorldPoseOnHost)? alignmentData;
        }

        /// <summary>
        /// Shares multiple MRUK rooms with a group
        /// </summary>
        /// <param name="rooms">A collection of rooms to be shared.</param>
        /// <param name="groupUuid">UUID of the group to which the room should be shared.</param>
        /// <returns>A task that tracks the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="rooms"/> is `null`.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="groupUuid"/> equals `Guid.Empty`.</exception>
        public OVRTask<OVRResult<OVRAnchor.ShareResult>> ShareRoomsAsync(IEnumerable<MRUKRoom> rooms,
            Guid groupUuid)
        {

            if (rooms == null)
            {
                throw new ArgumentNullException(nameof(rooms));
            }

            if (groupUuid == Guid.Empty)
            {
                throw new ArgumentException(nameof(groupUuid));
            }

            using (new OVRObjectPool.ListScope<OVRAnchor>(out var roomAnchors))
            {
                foreach (var room in rooms)
                {
                    if (!room.IsLocal)
                    {
                        Debug.LogError($"Sharing JSON or Prefab rooms is not supported. Only rooms loaded from device ({nameof(MRUKRoom)}.{nameof(MRUKRoom.IsLocal)} == true) can be shared.");
                        return OVRTask.FromResult(OVRResult<OVRAnchor.ShareResult>.FromFailure(OVRAnchor.ShareResult.FailureOperationFailed));
                    }
                    roomAnchors.Add(room.Anchor);
                }

                return OVRAnchor.ShareAsync(roomAnchors, groupUuid);
            }
        }

        /// <summary>
        ///     Loads the scene from the data stored on the device.
        /// </summary>
        /// <remarks>
        ///     The user must have granted ScenePermissions or this will fail.
        ///
        ///     In order to check if the user has granted permissions, call
        ///     `Permission.HasUserAuthorizedPermission(OVRPermissionsRequester.ScenePermission)`.
        ///
        ///     In order to request permissions from the user, call
        ///     `Permission.RequestUserPermission(OVRPermissionsRequester.ScenePermission, callbacks)`.
        /// </remarks>
        /// <param name="requestSceneCaptureIfNoDataFound">
        ///     If true and no rooms are found when loading from device,
        ///     the request space setup flow will be started.
        /// </param>
        /// <param name="removeMissingRooms">
        ///     When enabled, rooms that are already loaded but are not found in newSceneData will be removed.
        ///     This is to support the case where a user deletes a room from their device and the change needs to be reflected in the app.
        /// </param>
        /// <returns>An enum indicating whether loading was successful or not.</returns>
        public async Task<LoadDeviceResult> LoadSceneFromDevice(bool requestSceneCaptureIfNoDataFound = true, bool removeMissingRooms = true)
            => await LoadSceneFromDeviceInternal(requestSceneCaptureIfNoDataFound, removeMissingRooms);

        private async Task<LoadDeviceResult> LoadSceneFromDeviceInternal(
            bool requestSceneCaptureIfNoDataFound, bool removeMissingRooms
            , SharedRoomsData? sharedRoomsData = null
        )
        {
            var results = await CreateSceneDataFromDevice(
                sharedRoomsData
            );

            if (results.SceneData.Rooms.Count == 0)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                // If no rooms were loaded it could be due to missing scene permissions, check for this and print a warning
                // if that is the issue.
                if (!Permission.HasUserAuthorizedPermission(OVRPermissionsRequester.ScenePermission))
                {
                    Debug.LogWarning($"MRUK couldn't load any scene data. The app does not have permissions for {OVRPermissionsRequester.ScenePermission}.");
                    return LoadDeviceResult.NoScenePermission;
                }
#elif UNITY_EDITOR
                if (OVRManager.isHmdPresent)
                {
                    // If in the editor and an HMD is present, assume either Link or XR Sim is being used.
                    Debug.LogWarning("MRUK couldn't load any scene data. Scene capture does not work over Link or XR Sim.\n" +
                                     "If using Link please capture a scene with the HMD in standalone mode, then access the scene model over Link.\n" +
                                     "If a scene model has already been captured, make sure the spatial data feature has been enabled in Meta Quest Link " +
                                     "(Settings > Beta > Spatial Data over Meta Quest Link).\n" +
                                     "If using XR Sim, make sure you have a synthetic environment loaded that has scene data associated with it.");
                }
#endif
                // There could be 0 rooms because the user has not setup their space on the device.
                // Trigger the space setup flow and try to load again if that is the case.
                if (requestSceneCaptureIfNoDataFound)
                {
                    if (await OVRScene.RequestSpaceSetup())
                    {
                        // Try again but this time don't request a space setup again if there are no rooms to avoid
                        // the user getting stuck in an infinite loop.
                        return await LoadSceneFromDeviceInternal(false, removeMissingRooms, sharedRoomsData);
                    }
                }

                if (results.FetchResult == OVRAnchor.FetchResult.Success)
                {
                    return LoadDeviceResult.NoRoomsFound;
                }

                return (LoadDeviceResult)results.FetchResult;
            }

            if (!UpdateScene(results.SceneData, removeMissingRooms, sharedRoomsData))
            {
                return LoadDeviceResult.FailureDataIsInvalid;
            }

            InitializeScene();

            return LoadDeviceResult.Success;
        }

        /// <summary>
        /// </summary>
        private struct CreateSceneDataResults
        {
            public Data.SceneData SceneData;
            public OVRAnchor.FetchResult FetchResult;
        }

        /// <summary>
        ///     Attempts to create scene data from the device.
        /// </summary>
        /// <returns>A tuple containing a boolean indicating whether the operation was successful, the created scene data, and a list of OVRAnchors.</returns>
        private async Task<CreateSceneDataResults> CreateSceneDataFromDevice(SharedRoomsData? sharedRoomsData = null)
        {
            var componentType = typeof(OVRRoomLayout);
            var rooms = new List<OVRAnchor>();
            OVRAnchor.FetchResult status;
            if (sharedRoomsData.HasValue)
            {
                status = (await OVRAnchor.FetchSharedAnchorsAsync(sharedRoomsData.Value.groupUuid, sharedRoomsData.Value.roomUuids, rooms)).Status;
                // Remove all anchors that don't have the OVRRoomLayout component because the query for anchors shared by group includes all scene anchors
                rooms.RemoveAll(x => !x.TryGetComponent<OVRRoomLayout>(out _));
            }
            else
            {
                status = (await OVRAnchor.FetchAnchorsAsync(rooms, new OVRAnchor.FetchOptions { SingleComponentType = componentType })).Status;
            }

            var sceneData = new Data.SceneData()
            {
                CoordinateSystem = SerializationHelpers.CoordinateSystem.Unity,
                Rooms = new List<Data.RoomData>()
            };

            OVRTelemetry.Start(TelemetryConstants.MarkerId.LoadSceneFromDevice)
                .AddAnnotation(TelemetryConstants.AnnotationType.NumRooms, rooms.Count.ToString())
                .SetResult(status == OVRAnchor.FetchResult.Success ? OVRPlugin.Qpl.ResultType.Success : OVRPlugin.Qpl.ResultType.Fail)
                .Send();

            foreach (var roomAnchor in rooms)
            {
                Data.RoomData roomData = new Data.RoomData()
                {
                    Anchor = roomAnchor,
                    Anchors = new List<Data.AnchorData>(),
                    RoomLayout = new Data.RoomLayoutData()
                };

                if (componentType == typeof(OVRRoomLayout))
                {
                    bool hasRoomLayout = roomAnchor.TryGetComponent<OVRRoomLayout>(out var layout);
                    Assert.IsTrue(hasRoomLayout, nameof(hasRoomLayout));
                    if (!layout.TryGetRoomLayout(out var ceiling, out var floor, out var walls))
                    {
                        Debug.LogWarning("Failed to get room layout");
                        continue;
                    }
                    roomData.RoomLayout.FloorUuid = floor;
                    roomData.RoomLayout.CeilingUuid = ceiling;
                    roomData.RoomLayout.WallsUuid = new List<Guid>(walls?.Length ?? 0);

                    // Make sure order of the walls is preserved
                    foreach (var wall in walls)
                    {
                        roomData.RoomLayout.WallsUuid.Add(wall);
                    }
                }

                var room = roomAnchor.GetComponent<OVRAnchorContainer>();
                var childAnchors = new List<OVRAnchor>();
                if (sharedRoomsData.HasValue)
                {
                    await OVRAnchor.FetchSharedAnchorsAsync(sharedRoomsData.Value.groupUuid, room.Uuids, childAnchors);
                }
                else
                {
                    await OVRAnchor.FetchAnchorsAsync(childAnchors, new OVRAnchor.FetchOptions
                    {
                        Uuids = room.Uuids
                    });
                }



                if (childAnchors.Count == 0)
                {
                    Debug.LogWarning(
                        $"Room contains {room.Uuids.Length} anchors, but FetchAnchorsAsync couldn't find any of them! Skipping room");
                    continue;
                }

                if (roomAnchor.TryGetComponent<OVRSharable>(out var sharable))
                {
                    await sharable.SetEnabledAsync(true);
                }

                var tasks = new List<OVRTask<bool>>();

                // Enable locatable on all the child anchors, we do this before accessing all the positions
                // so that when we actually go to access them they are already enabled and we can get all
                // the position on the same frame. Or as close as possible to the same frame.

                foreach (var child in childAnchors)
                {
                    if (!child.TryGetComponent<OVRLocatable>(out var locatable))
                    {
                        continue;
                    }

                    tasks.Add(locatable.SetEnabledAsync(true));
                }

                await OVRTask.WhenAll(tasks);

                foreach (var child in childAnchors)
                {
                    MRUKAnchor.SceneLabels semanticClassifications = 0;
                    if (child.TryGetComponent(out OVRSemanticLabels labels) && labels.IsEnabled)
                    {
                        labels.GetClassifications(_classificationsBuffer);
                        foreach (var classification in _classificationsBuffer)
                        {
                            semanticClassifications |= Utilities.ClassificationToSceneLabel(classification);
                        }
                    }

                    var anchorData = new Data.AnchorData
                    {
                        Anchor = child,
                        Labels = semanticClassifications
                    };

                    if (child.TryGetComponent(out OVRBounded2D bounds2) && bounds2.IsEnabled)
                    {
                        anchorData.PlaneBounds = new Data.PlaneBoundsData()
                        {
                            Min = bounds2.BoundingBox.min,
                            Max = bounds2.BoundingBox.max,
                        };

                        if (bounds2.TryGetBoundaryPointsCount(out var counts))
                        {
                            using var boundary = new NativeArray<Vector2>(counts, Allocator.Temp);
                            if (bounds2.TryGetBoundaryPoints(boundary))
                            {
                                anchorData.PlaneBoundary2D = new List<Vector2>();
                                for (int i = 0; i < counts; i++)
                                {
                                    anchorData.PlaneBoundary2D.Add(boundary[i]);
                                }
                            }
                        }
                    }

                    if (child.TryGetComponent(out OVRBounded3D bounds3) && bounds3.IsEnabled)
                    {
                        anchorData.VolumeBounds = new Data.VolumeBoundsData()
                        {
                            Min = bounds3.BoundingBox.min,
                            Max = bounds3.BoundingBox.max,
                        };
                    }

                    anchorData.Transform.Scale = Vector3.one;
                    bool gotLocation = false;
                    if (child.TryGetComponent(out OVRLocatable locatable) && locatable.IsEnabled)
                    {
                        if (locatable.TryGetSceneAnchorPose(out var pose))
                        {
                            var position = pose.ComputeWorldPosition(_cameraRig.trackingSpace);
                            var rotation = pose.ComputeWorldRotation(_cameraRig.trackingSpace);
                            if (position.HasValue && rotation.HasValue)
                            {
                                anchorData.Transform.Translation = position.Value;
                                anchorData.Transform.Rotation = rotation.Value.eulerAngles;
                                gotLocation = true;
                            }
                        }
                    }

                    if (!gotLocation)
                    {
                        Debug.LogWarning($"Failed to get location of anchor with UUID: {anchorData.Anchor.Uuid}");
                    }

                    roomData.Anchors.Add(anchorData);
                }


                sceneData.Rooms.Add(roomData);
            }

            return new CreateSceneDataResults { FetchResult = status, SceneData = sceneData };
        }




        private void FindAllObjects(GameObject roomPrefab, out List<GameObject> walls, out List<GameObject> volumes,
            out List<GameObject> planes, out List<GameObject> ceilings, out List<GameObject> floors, out List<GameObject> others)
        {
            walls = new List<GameObject>();
            volumes = new List<GameObject>();
            planes = new List<GameObject>();
            ceilings = new List<GameObject>();
            floors = new List<GameObject>();
            others = new List<GameObject>();

            FindObjects(MRUKAnchor.SceneLabels.WALL_FACE.ToString(), roomPrefab.transform, ref walls);
            FindObjects(MRUKAnchor.SceneLabels.INVISIBLE_WALL_FACE.ToString(), roomPrefab.transform, ref walls);
            FindObjects(MRUKAnchor.SceneLabels.OTHER.ToString(), roomPrefab.transform, ref volumes);
            FindObjects(MRUKAnchor.SceneLabels.TABLE.ToString(), roomPrefab.transform, ref volumes);
            FindObjects(MRUKAnchor.SceneLabels.COUCH.ToString(), roomPrefab.transform, ref volumes);
            FindObjects(MRUKAnchor.SceneLabels.WINDOW_FRAME.ToString(), roomPrefab.transform, ref planes);
            FindObjects(MRUKAnchor.SceneLabels.DOOR_FRAME.ToString(), roomPrefab.transform, ref planes);
            FindObjects(MRUKAnchor.SceneLabels.WALL_ART.ToString(), roomPrefab.transform, ref planes);
            FindObjects(MRUKAnchor.SceneLabels.PLANT.ToString(), roomPrefab.transform, ref volumes);
            FindObjects(MRUKAnchor.SceneLabels.SCREEN.ToString(), roomPrefab.transform, ref volumes);
            FindObjects(MRUKAnchor.SceneLabels.BED.ToString(), roomPrefab.transform, ref volumes);
            FindObjects(MRUKAnchor.SceneLabels.LAMP.ToString(), roomPrefab.transform, ref volumes);
            FindObjects(MRUKAnchor.SceneLabels.STORAGE.ToString(), roomPrefab.transform, ref volumes);
        }

        /// <summary>
        /// Simulates the creation of a scene in the Editor, using transforms and names from our prefab rooms.
        /// </summary>
        /// <param name="scenePrefab">The prefab GameObject representing the scene or a collection of rooms.</param>
        /// <param name="clearSceneFirst">If true, clears the current scene before loading the new one.</param>
        public void LoadSceneFromPrefab(GameObject scenePrefab, bool clearSceneFirst = true)
        {
            OVRTelemetry.Start(TelemetryConstants.MarkerId.LoadSceneFromPrefab)
                .AddAnnotation(TelemetryConstants.AnnotationType.SceneName, scenePrefab.name)
                .Send();

            if (clearSceneFirst)
            {
                ClearScene();
            }

            // first, examine prefab to determine if it's a single room or collection of rooms
            // if the hierarchy is more than two levels deep, consider it a home
            if (scenePrefab.transform.childCount > 0 && scenePrefab.transform.GetChild(0).childCount > 0)
            {
                foreach (Transform room in scenePrefab.transform)
                {
                    LoadRoomFromPrefab(room.gameObject);
                }
            }
            else
            {
                LoadRoomFromPrefab(scenePrefab);
            }

            InitializeScene();
        }

        private void LoadRoomFromPrefab(GameObject roomPrefab)
        {
            FindAllObjects(roomPrefab, out var walls, out var volumes, out var planes, out var ceilings, out var floors, out var others);

            var sceneRoom = new GameObject(roomPrefab.name);
            var roomInfo = sceneRoom.AddComponent<MRUKRoom>();
            roomInfo.Anchor = new OVRAnchor(0, Guid.NewGuid());

            // walls ordered sequentially, CW when viewed top-down
            var orderedWalls = new List<MRUKAnchor>();

            var unorderedWalls = new List<MRUKAnchor>();
            var floorCorners = new List<Vector3>();

            var wallHeight = 0.0f;

            for (var i = 0; i < walls.Count; i++)
            {
                if (i == 0)
                {
                    wallHeight = walls[i].transform.localScale.y;
                }

                var objData = CreateAnchorFromRoomObject(walls[i].transform, walls[i].transform.localScale, AnchorRepresentation.PLANE);
                objData.Room = roomInfo;

                // if this is an INVISIBLE_WALL_FACE, it also needs the WALL_FACE label
                if (walls[i].name.Equals(MRUKAnchor.SceneLabels.INVISIBLE_WALL_FACE.ToString()))
                {
                    objData.Label |= MRUKAnchor.SceneLabels.WALL_FACE;
                }

                objData.transform.parent = sceneRoom.transform;
                objData.transform.Rotate(0, 180, 0);

                unorderedWalls.Add(objData);
                roomInfo.Anchors.Add(objData);
            }

            // There may be imprecision between the prefab walls (misaligned edges)
            // so, we shift them so the edges perfectly match up:
            // bottom left corner of wall is fixed, right corner matches left corner of wall to the right
            var seedId = 0;
            for (var i = 0; i < unorderedWalls.Count; i++)
            {
                var wall = GetAdjacentWall(ref seedId, unorderedWalls);
                orderedWalls.Add(wall);

                var wallRect = wall.PlaneRect.Value;
                var leftCorner = wall.transform.TransformPoint(new Vector3(wallRect.max.x, wallRect.min.y, 0.0f));
                floorCorners.Add(leftCorner);
            }

            for (var i = 0; i < orderedWalls.Count; i++)
            {
                var planeRect = orderedWalls[i].PlaneRect.Value;
                var corner1 = floorCorners[i];
                var nextID = (i == orderedWalls.Count - 1) ? 0 : i + 1;
                var corner2 = floorCorners[nextID];

                var wallRight = (corner1 - corner2);
                wallRight.y = 0.0f;
                var wallWidth = wallRight.magnitude;
                wallRight /= wallWidth;
                var wallUp = Vector3.up;
                var wallFwd = Vector3.Cross(wallRight, wallUp);
                var newPosition = (corner1 + corner2) * 0.5f + Vector3.up * (planeRect.height * 0.5f);
                var newRotation = Quaternion.LookRotation(wallFwd, wallUp);
                var newRect = new Rect(-0.5f * wallWidth, planeRect.y, wallWidth, planeRect.height);

                orderedWalls[i].transform.position = newPosition;
                orderedWalls[i].transform.rotation = newRotation;
                orderedWalls[i].PlaneRect = newRect;
                orderedWalls[i].PlaneBoundary2D = new List<Vector2>
                {
                    new(newRect.xMin, newRect.yMin),
                    new(newRect.xMax, newRect.yMin),
                    new(newRect.xMax, newRect.yMax),
                    new(newRect.xMin, newRect.yMax),
                };

                roomInfo.WallAnchors.Add(orderedWalls[i]);
            }

            for (var i = 0; i < volumes.Count; i++)
            {
                var cubeScale = new Vector3(volumes[i].transform.localScale.x, volumes[i].transform.localScale.z, volumes[i].transform.localScale.y);
                var representation = AnchorRepresentation.VOLUME;
                // Table and couch are special. They also have a plane attached to them.
                if (volumes[i].transform.name == MRUKAnchor.SceneLabels.TABLE.ToString() ||
                    volumes[i].transform.name == MRUKAnchor.SceneLabels.COUCH.ToString())
                {
                    representation |= AnchorRepresentation.PLANE;
                }

                var objData = CreateAnchorFromRoomObject(volumes[i].transform, cubeScale, representation);
                objData.transform.parent = sceneRoom.transform;
                objData.Room = roomInfo;

                // in the prefab rooms, the cubes are more Unity-like and default: Y is up, pivot is centered
                // this needs to be converted to Scene format, in which the pivot is on top of the cube and Z is up
                // for couches we want the functional surface to be in the center
                if (!objData.Label.HasFlag(MRUKAnchor.SceneLabels.COUCH))
                {
                    objData.transform.position += cubeScale.z * 0.5f * Vector3.up;
                }

                objData.transform.Rotate(new Vector3(-90, 0, 0), Space.Self);
                roomInfo.Anchors.Add(objData);
            }

            for (var i = 0; i < planes.Count; i++)
            {
                var objData = CreateAnchorFromRoomObject(planes[i].transform, planes[i].transform.localScale, AnchorRepresentation.PLANE);
                objData.transform.parent = sceneRoom.transform;
                objData.Room = roomInfo;

                // Unity quads have a surface normal facing the opposite direction
                // Rather than have "backwards" walls in the room prefab, we just rotate them here
                objData.transform.Rotate(0, 180, 0);
                roomInfo.Anchors.Add(objData);
            }

            for (var i = 0; i < others.Count; i++)
            {
                var objData = CreateAnchorFromRoomObject(others[i].transform, others[i].transform.localScale, AnchorRepresentation.PLANE);
                objData.transform.parent = sceneRoom.transform;
                objData.Room = roomInfo;

                roomInfo.Anchors.Add(objData);
            }

            var (roomCenter, longestWall, floorScale) = CalculateFloorCeilingData(orderedWalls, floorCorners, wallHeight);

            if (floors.Count > 0)
            {
                foreach (var floor in floors)
                {
                    var objData = CreateAnchorFromRoomObject(floor.transform, floor.transform.localScale, AnchorRepresentation.PLANE);
                    objData.transform.parent = sceneRoom.transform;
                    objData.Room = roomInfo;

                    roomInfo.FloorAnchor = objData;
                    roomInfo.Anchors.Add(objData);
                }
            }
            else
            {
                var objData = CreateFloorCeiling(true, roomCenter, wallHeight, longestWall, floorScale, sceneRoom, roomInfo, floorCorners);
                roomInfo.FloorAnchor = objData;
                roomInfo.Anchors.Add(objData);
            }

            if (ceilings.Count > 0)
            {
                foreach (var ceiling in ceilings)
                {
                    var objData = CreateAnchorFromRoomObject(ceiling.transform, ceiling.transform.localScale, AnchorRepresentation.PLANE);
                    objData.transform.parent = sceneRoom.transform;
                    objData.Room = roomInfo;

                    roomInfo.CeilingAnchor = objData;
                    roomInfo.Anchors.Add(objData);
                }
            }
            else
            {
                var objData = CreateFloorCeiling(false, roomCenter, wallHeight, longestWall, floorScale, sceneRoom, roomInfo, floorCorners);
                roomInfo.CeilingAnchor = objData;
                roomInfo.Anchors.Add(objData);
            }

            // after everything, we need to let the room computation run
            roomInfo.ComputeRoomInfo();
            Rooms.Add(roomInfo);
        }

        private (Vector3, MRUKAnchor, Vector3) CalculateFloorCeilingData(List<MRUKAnchor> orderedWalls, List<Vector3> floorCorners, float wallHeight)
        {
            // mimic OVRSceneManager: floor/ceiling anchor aligns with the longest wall, scaled to room size
            MRUKAnchor longestWall = null;
            var longestWidth = 0.0f;
            foreach (var wall in orderedWalls)
            {
                var wallWidth = wall.PlaneRect.Value.size.x;
                if (wallWidth > longestWidth)
                {
                    longestWidth = wallWidth;
                    longestWall = wall;
                }
            }

            // calculate the room bounds, relative to the longest wall
            var zMin = 0.0f;
            var zMax = 0.0f;
            var xMin = 0.0f;
            var xMax = 0.0f;
            for (var i = 0; i < floorCorners.Count; i++)
            {
                var localPos = longestWall.transform.InverseTransformPoint(floorCorners[i]);

                zMin = i == 0 ? localPos.z : Mathf.Min(zMin, localPos.z);
                zMax = i == 0 ? localPos.z : Mathf.Max(zMax, localPos.z);
                xMin = i == 0 ? localPos.x : Mathf.Min(xMin, localPos.x);
                xMax = i == 0 ? localPos.x : Mathf.Max(xMax, localPos.x);
            }

            var localRoomCenter = new Vector3((xMin + xMax) * 0.5f, 0, (zMin + zMax) * 0.5f);
            var roomCenter = longestWall.transform.TransformPoint(localRoomCenter);
            roomCenter -= Vector3.up * wallHeight * 0.5f;
            var floorScale = new Vector3(zMax - zMin, xMax - xMin, 1);
            return (roomCenter, longestWall, floorScale);
        }

        private MRUKAnchor CreateFloorCeiling(bool isFloor, Vector3 roomCenter, float wallHeight, MRUKAnchor longestWall, Vector3 floorScale, GameObject parent, MRUKRoom roomInfo, List<Vector3> floorCorners)
        {
            var anchorName = (isFloor ? "FLOOR" : "CEILING");

            var position = roomCenter + Vector3.up * wallHeight * (isFloor ? 0 : 1);
            float anchorFlip = isFloor ? 1 : -1;
            var rotation = Quaternion.LookRotation(longestWall.transform.up * anchorFlip, longestWall.transform.right);
            var objData = CreateAnchor(anchorName, position, rotation, floorScale, AnchorRepresentation.PLANE);
            objData.transform.parent = parent.transform;
            objData.Room = roomInfo;

            objData.PlaneBoundary2D = new(floorCorners.Count);
            foreach (var corner in floorCorners)
            {
                var localCorner = objData.transform.InverseTransformPoint(corner);
                objData.PlaneBoundary2D.Add(new Vector2(localCorner.x, localCorner.y));
            }

            if (!isFloor)
            {
                objData.PlaneBoundary2D.Reverse();
            }

            return objData;
        }

        /// <summary>
        ///     Serializes the scene data into a JSON string. The scene data includes rooms, anchors, and their associated properties.
        ///     The method allows for the specification of the coordinate system (Unity or Unreal) and whether to include the global mesh data.
        /// </summary>
        /// <param name="coordinateSystem">The coordinate system to use for the serialization (Unity or Unreal).</param>
        /// <param name="includeGlobalMesh">A boolean indicating whether to include the global mesh data in the serialization. Default is true.</param>
        /// <param name="rooms">A list of rooms to serialize, if this is null then all rooms will be serialized.</param>
        /// <returns>A JSON string representing the serialized scene data.</returns>
        public string SaveSceneToJsonString(SerializationHelpers.CoordinateSystem coordinateSystem = SerializationHelpers.CoordinateSystem.Unity, bool includeGlobalMesh = true,
            List<MRUKRoom> rooms = null)
        {
            return SerializationHelpers.Serialize(coordinateSystem, includeGlobalMesh, rooms);
        }


        /// <summary>
        ///     Loads the scene from a JSON string representing the scene data.
        /// </summary>
        /// <param name="jsonString">The JSON string containing the serialized scene data.</param>
        /// <param name="removeMissingRooms">When enabled, rooms that are already loaded but are not found in JSON the string will be removed.</param>
        public void LoadSceneFromJsonString(string jsonString, bool removeMissingRooms = true)
        {
            var newSceneData = SerializationHelpers.Deserialize(jsonString);

            UpdateScene(newSceneData, removeMissingRooms);

            OVRTelemetry.Start(TelemetryConstants.MarkerId.LoadSceneFromJson)
                .AddAnnotation(TelemetryConstants.AnnotationType.NumRooms, Rooms.Count.ToString())
                .Send();

            InitializeScene();
        }

        private MRUKAnchor CreateAnchorFromRoomObject(Transform refObject, Vector3 objScale, AnchorRepresentation representation)
        {
            return CreateAnchor(refObject.name, refObject.position, refObject.rotation, objScale, representation);
        }

        /// <summary>
        ///     Creates an anchor with the specified properties.
        /// </summary>
        /// <param name="name">The name of the anchor.</param>
        /// <param name="position">The position of the anchor.</param>
        /// <param name="rotation">The rotation of the anchor.</param>
        /// <param name="objScale">The scale of the anchor.</param>
        /// <param name="representation">The representation of the anchor (plane or volume).</param>
        /// <returns>The created anchor.</returns>
        private MRUKAnchor CreateAnchor(string name, Vector3 position, Quaternion rotation, Vector3 objScale, AnchorRepresentation representation)
        {
            var realAnchor = new GameObject(name);
            realAnchor.transform.position = position;
            realAnchor.transform.rotation = rotation;
            MRUKAnchor objData = realAnchor.AddComponent<MRUKAnchor>();
            objData.Label = Utilities.StringLabelToEnum(realAnchor.name);
            if ((representation & AnchorRepresentation.PLANE) != 0)
            {
                var size2d = new Vector2(objScale.x, objScale.y);
                var rect = new Rect(-0.5f * size2d, size2d);

                objData.PlaneRect = rect;
                objData.PlaneBoundary2D = new List<Vector2>
                {
                    new Vector2(rect.xMin, rect.yMin),
                    new Vector2(rect.xMax, rect.yMin),
                    new Vector2(rect.xMax, rect.yMax),
                    new Vector2(rect.xMin, rect.yMax),
                };
            }

            if ((representation & AnchorRepresentation.VOLUME) != 0)
            {
                Vector3 offsetCenter = new Vector3(0, 0, -objScale.z * 0.5f);
                // for couches we want the functional surface to be in the center
                if (objData.Label.HasFlag(MRUKAnchor.SceneLabels.COUCH))
                    offsetCenter = Vector3.zero;
                objData.VolumeBounds = new Bounds(offsetCenter, objScale);
            }

            objData.Anchor = new OVRAnchor(0, Guid.NewGuid());
            return objData;
        }

        void FindObjects(string objName, Transform rootTransform, ref List<GameObject> objList)
        {
            if (rootTransform.name.Equals(objName))
            {
                objList.Add(rootTransform.gameObject);
            }

            foreach (Transform child in rootTransform)
            {
                FindObjects(objName, child, ref objList);
            }
        }

        MRUKAnchor GetAdjacentWall(ref int thisID, List<MRUKAnchor> randomWalls)
        {
            Vector2 thisWallScale = randomWalls[thisID].PlaneRect.Value.size;

            Vector3 halfScale = thisWallScale * 0.5f;
            Vector3 bottomRight = randomWalls[thisID].transform.position - randomWalls[thisID].transform.up * halfScale.y - randomWalls[thisID].transform.right * halfScale.x;
            float closestCornerDistance = Mathf.Infinity;
            // When searching for a matching corner, the correct one should match positions. If they don't, assume there's a crack in the room.
            // This should be an impossible scenario and likely means broken data from Room Setup.
            int rightWallID = 0;
            for (int i = 0; i < randomWalls.Count; i++)
            {
                // compare to bottom left point of other walls
                if (i != thisID)
                {
                    Vector2 testWallHalfScale = randomWalls[i].PlaneRect.Value.size * 0.5f;
                    Vector3 bottomLeft = randomWalls[i].transform.position - randomWalls[i].transform.up * testWallHalfScale.y + randomWalls[i].transform.right * testWallHalfScale.x;
                    float thisCornerDistance = Vector3.Distance(bottomLeft, bottomRight);
                    if (thisCornerDistance < closestCornerDistance)
                    {
                        closestCornerDistance = thisCornerDistance;
                        rightWallID = i;
                    }
                }
            }

            thisID = rightWallID;
            return randomWalls[thisID];
        }

        /// <summary>
        ///     Manages the scene by creating, updating, or deleting rooms and anchors based on new scene data.
        /// </summary>
        /// <param name="newSceneData"> The new scene data.</param>
        /// <param name="removeMissingRooms"> When enabled, rooms that are already loaded but are not found in newSceneData will be removed.
        /// This is to support the case where a user deletes a room from their device and the change needs to be reflected in the app.
        /// </param>
        /// <param name="sharedRoomsData"> Used to align the local room with the host's room. </param>
        /// <returns>A list of managed MRUKRoom objects.</returns>
        private bool UpdateScene(Data.SceneData newSceneData, bool removeMissingRooms, SharedRoomsData? sharedRoomsData = null)
        {
            var anchorOffset = Matrix4x4.identity;
            if (sharedRoomsData.HasValue && sharedRoomsData.Value.alignmentData.HasValue)
            {
                // Find the local room with the uuid of the host's room used for alignment.
                // And re-align all rooms so that the local floor matches the global coordinate of the host floor.
                Data.RoomData? foundRoom = null;
                var alignmentRoomUuid = sharedRoomsData.Value.alignmentData.Value.alignmentRoomUuid;
                foreach (var room in newSceneData.Rooms)
                {
                    if (room.Anchor.Uuid == alignmentRoomUuid)
                    {
                        foundRoom = room;
                        break;
                    }
                }
                if (!foundRoom.HasValue)
                {
                    Debug.LogError($"Room with uuid {alignmentRoomUuid} not found.");
                    return false;
                }
                var floorAnchorIndex = foundRoom.Value.Anchors.FindIndex(x => (x.Labels & MRUKAnchor.SceneLabels.FLOOR) != 0);
                if (floorAnchorIndex == -1)
                {
                    Debug.LogError($"Room with uuid {alignmentRoomUuid} doesn't have the {nameof(MRUKRoom.FloorAnchor)}.");
                    return false;
                }
                var floorAnchor = foundRoom.Value.Anchors[floorAnchorIndex];
                var floorWorldPoseOnHost = sharedRoomsData.Value.alignmentData.Value.floorWorldPoseOnHost;
                var rotDiff = floorWorldPoseOnHost.rotation * Quaternion.Inverse(Quaternion.Euler(floorAnchor.Transform.Rotation));
                var posDiff = floorWorldPoseOnHost.position - rotDiff * floorAnchor.Transform.Translation;
                anchorOffset = Matrix4x4.TRS(posDiff, rotDiff, Vector3.one);

                if (!EnableWorldLock)
                {
                    // If the World Lock is disabled, apply the offset to the tracking space.
                    // Otherwise, camera will not be in sync with the scene anchors.
                    var trackingSpace = _cameraRig.trackingSpace;
                    trackingSpace.position = anchorOffset.MultiplyPoint3x4(trackingSpace.position);
                    trackingSpace.rotation = anchorOffset.rotation * trackingSpace.rotation;
                    Debug.LogWarning(nameof(EnableWorldLock) + " is disabled. MRUK applied the colocation alignment to the " + nameof(OVRCameraRig.trackingSpace) + ". Please enable " + nameof(EnableWorldLock) + " setting to disable this warning.");
                }
            }

            List<Data.RoomData> newRoomsToCreate = new();

            //the existing rooms will get removed from this list and updated separately
            newRoomsToCreate.AddRange(newSceneData.Rooms);

            List<MRUKRoom> roomsToRemove = new();
            List<MRUKRoom> roomsToNotifyUpdated = new();
            List<MRUKRoom> roomsToNotifyRemoved = new();
            List<MRUKRoom> roomsToNotifyCreated = new();


            //check old rooms to see if a new received room match and then perform update on room,
            //update,delete or create on anchors.
            foreach (var oldRoom in Rooms)
            {
                bool foundRoom = false;
                foreach (var newRoomData in newSceneData.Rooms)
                {
                    if (oldRoom.IsIdenticalRoom(newRoomData))
                    {
                        foundRoom = true;
                    }
                    else if (oldRoom.IsSameRoom(newRoomData))
                    {
                        foundRoom = true;

                        // Found the same room but we need to update the anchors within it
                        oldRoom.Anchor = newRoomData.Anchor;
                        oldRoom.UpdateRoomLabel(newRoomData);

                        List<Tuple<MRUKAnchor, Data.AnchorData>> anchorsToUpdate = new();
                        List<Data.AnchorData> newAnchorsToCreate = new();
                        List<MRUKAnchor> anchorsToRemove = new();

                        // Find anchors to create and update
                        foreach (var anchorData in newRoomData.Anchors)
                        {
                            bool foundMatch = false;
                            foreach (var oldAnchor in oldRoom.Anchors)
                            {
                                if (oldAnchor.Equals(anchorData))
                                {
                                    foundMatch = true;
                                    break;
                                }

                                if (oldAnchor.Anchor == anchorData.Anchor)
                                {
                                    anchorsToUpdate.Add(
                                        new Tuple<MRUKAnchor, Data.AnchorData>(oldAnchor, anchorData));

                                    foundMatch = true;
                                    break;
                                }
                            }

                            if (!foundMatch)
                            {
                                newAnchorsToCreate.Add(anchorData);
                            }
                        }

                        // Find anchors to remove
                        foreach (var oldAnchor in oldRoom.Anchors)
                        {
                            bool foundAnchor = false;
                            foreach (var anchorData in newRoomData.Anchors)
                            {
                                if (oldAnchor.Anchor == anchorData.Anchor)
                                {
                                    foundAnchor = true;
                                    break;
                                }
                            }

                            if (!foundAnchor)
                            {
                                anchorsToRemove.Add(oldAnchor);
                            }
                        }

                        foreach (var anchor in newAnchorsToCreate)
                        {
                            oldRoom.CreateAnchor(anchor, anchorOffset);
                        }

                        foreach (var anchor in anchorsToRemove)
                        {
                            oldRoom.AnchorRemovedEvent?.Invoke(anchor);
                            oldRoom.RemoveAndDestroyAnchor(anchor);
                        }

                        foreach (var anchorToUpdate in anchorsToUpdate)
                        {
                            var oldAnchor = anchorToUpdate.Item1;
                            var newAnchorData = anchorToUpdate.Item2;
                            oldAnchor.UpdateAnchor(newAnchorData);
                            oldRoom.AnchorUpdatedEvent?.Invoke(oldAnchor);
                        }

                        oldRoom.UpdateRoom(newRoomData);
                        oldRoom.ComputeRoomInfo();
                        roomsToNotifyUpdated.Add(oldRoom);
                    }

                    if (foundRoom)
                    {
                        for (int i = 0; i < newRoomsToCreate.Count; ++i)
                        {
                            if (newRoomsToCreate[i].Anchor == newRoomData.Anchor)
                            {
                                newRoomsToCreate.RemoveAt(i);
                                break;
                            }
                        }

                        break;
                    }
                }

                if (!foundRoom && removeMissingRooms)
                {
                    roomsToRemove.Add(oldRoom);
                }
            }

            foreach (var oldRoom in roomsToRemove)
            {
                for (int j = oldRoom.Anchors.Count - 1; j >= 0; j--)
                {
                    var anchor = oldRoom.Anchors[j];
                    oldRoom.Anchors.Remove(anchor);
                    oldRoom.AnchorRemovedEvent?.Invoke(anchor);
                    Utilities.DestroyGameObjectAndChildren(anchor.gameObject);
                }

                Rooms.Remove(oldRoom);
                roomsToNotifyRemoved.Add(oldRoom);

                Utilities.DestroyGameObjectAndChildren(oldRoom.gameObject);
            }

            foreach (var newRoomData in newRoomsToCreate)
            {
                //create room and throw events for room and anchors
                var room = CreateRoom(newRoomData, anchorOffset);
                room.ComputeRoomInfo();
                roomsToNotifyCreated.Add(room);
                Rooms.Add(room);
            }

            //finally - send messages
            foreach (var room in roomsToNotifyUpdated)
            {
                RoomUpdatedEvent?.Invoke(room);
            }

            foreach (var room in roomsToNotifyRemoved)
            {
                RoomRemovedEvent?.Invoke(room);
            }

            foreach (var room in roomsToNotifyCreated)
            {
                RoomCreatedEvent?.Invoke(room);
            }
            return true;
        }

        private static MRUKRoom CreateRoom(Data.RoomData roomData, Matrix4x4 anchorOffset)
        {
            GameObject sceneRoom = new GameObject();
            MRUKRoom roomInfo = sceneRoom.AddComponent<MRUKRoom>();

            roomInfo.Anchor = roomData.Anchor;
            foreach (Data.AnchorData anchorData in roomData.Anchors)
            {
                roomInfo.CreateAnchor(anchorData, anchorOffset);
            }

            roomInfo.UpdateRoomLabel(roomData);
            roomInfo.UpdateRoom(roomData);

            return roomInfo;
        }

    }
}
