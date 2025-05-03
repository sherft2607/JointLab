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
using Meta.XR.Util;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = System.Random;

namespace Meta.XR.MRUtilityKit
{
    /// <summary>
    /// Manages the spawning of prefabs based on anchor data within a scene, providing various customization options
    /// for scaling, alignment, and selection modes. This class extends the <see cref="ICustomAnchorPrefabSpawner"/> interface,
    /// which can be used together with <see cref="AnchorPrefabSpawnerUtilities"/> to provide custom implementations of the prefab spawning logic.
    /// </summary>
    [HelpURL("https://developers.meta.com/horizon/reference/mruk/latest/class_meta_x_r_m_r_utility_kit_anchor_prefab_spawner")]
    [Feature(Feature.Scene)]
    public class AnchorPrefabSpawner : MonoBehaviour, ICustomAnchorPrefabSpawner
    {
        /// <summary>
        /// Defines the scaling modes available for adjusting prefab sizes to fit the anchor's dimensions.
        /// The scaling mode determines how the prefab will be resized to match the anchor's size.
        /// It is defined for each <see cref="AnchorPrefabGroup"/> and can be set to one of the following options:
        /// </summary>
        /// <remarks> When using he <see cref="ScalingMode.Custom"/> the <see cref="CustomPrefabScaling"/> method
        /// must be implemented in a custom class that extends this class or the <see cref="ICustomPrefabScaling"/> interface. </remarks>
        public enum ScalingMode
        {
            /// <summary>
            /// Stretch each axis to exactly match the size of the Plane/Volume.
            /// This mode ensures that the prefab fills the entire anchor area.
            /// </summary>
            Stretch,

            /// <summary>
            /// Scale each axis by the same amount to maintain the correct aspect ratio.
            /// This mode preserves the original aspect ratio of the prefab.
            /// </summary>
            UniformScaling,

            /// <summary>
            /// Scale the X and Z axes uniformly but the Y scale can be different.
            /// This mode allows for flexible scaling while maintaining some aspect ratio consistency.
            /// </summary>
            UniformXZScale,

            /// <summary>
            /// Don't perform any scaling.
            /// This mode leaves the prefab at its original size, without any adjustments.
            /// </summary>
            NoScaling,

            /// <summary>
            /// Custom logic, extend this class and override <see cref="CustomPrefabScaling"/> with your own implementation.
            /// This mode enables developers to create custom scaling logic tailored to their specific needs.
            /// </summary>
            Custom
        }

        /// <summary>
        /// Defines the alignment modes available for positioning prefabs relative to the anchor's dimensions.
        /// The alignment mode determines how the prefab will be positioned within the anchor area.
        /// It is defined for each <see cref="AnchorPrefabGroup"/> and can be set to one of the following options:
        /// </summary>
        /// <remarks> When using he <see cref="AlignMode.Custom"/> the <see cref="CustomPrefabAlignment"/> method
        /// must be implemented in a custom class that extends this class or the <see cref="ICustomPrefabScaling"/> interface. </remarks>
        public enum AlignMode
        {
            /// <summary>
            /// For volumes align to the base, for planes align to the center.
            /// This mode provides a default alignment that works well for most use cases.
            /// </summary>
            Automatic,

            /// <summary>
            /// Align the bottom of the prefab with the bottom of the volume or plane.
            /// This mode is useful when you want the prefab to sit at the base of the anchor.
            /// </summary>
            Bottom,

            /// <summary>
            /// Align the center of the prefab with the center of the volume or plane.
            /// This mode is useful when you want the prefab to be centered within the anchor.
            /// </summary>
            Center,

            /// <summary>
            /// Don't add any local offset to the prefab.
            /// This mode leaves the prefab at its original position, without any adjustments.
            /// </summary>
            NoAlignment,

            /// <summary>
            /// Custom logic, extend this class and override <see cref="CustomPrefabAlignment"/> with your own implementation.
            /// This mode enables developers to create custom alignment logic tailored to their specific needs.
            /// </summary>
            Custom
        }

        /// <summary>
        /// Defines the selection modes available for choosing prefabs from a list.
        /// The selection mode determines which prefab will be spawned when multiple options are available.
        /// It is defined for each <see cref="AnchorPrefabGroup"/> and can be set to one of the following options:
        /// </summary>
        /// <remarks> When using he <see cref="SelectionMode.Custom"/> the <see cref="CustomPrefabSelection"/> method
        /// must be implemented in a custom class that extends this class or the <see cref="ICustomPrefabScaling"/> interface. </remarks>
        public enum SelectionMode
        {
            /// <summary>
            /// Randomly choose a prefab from the list every time.
            /// This mode adds randomness to the spawning process, making it more dynamic.
            /// </summary>
            Random,

            /// <summary>
            /// Choose the prefab that has the smallest difference between its size and the one of the anchor.
            /// This mode selects the prefab that best fits the anchor's size, minimizing gaps or overlaps.
            /// </summary>
            ClosestSize,

            /// <summary>
            /// Custom logic, extend this class and override <see cref="CustomPrefabSelection"/> with your own implementation.
            /// This mode enables developers to create custom selection logic tailored to their specific needs.
            /// </summary>
            Custom
        }

        /// <summary>
        /// Represents a group of prefabs associated with specific scene labels, along with settings for how they should be spawned.
        /// This struct encapsulates the necessary information for spawning prefabs based on anchor data.
        /// </summary>
        /// <remarks> When using <see cref="ScalingMode.Custom"/>, <see cref="AlignMode.Custom"/>, or <see cref="SelectionMode.Custom"/>
        /// the <see cref="CustomPrefabScaling"/>, <see cref="CustomPrefabAlignment"/> or <see cref="CustomPrefabAlignment"/>
        /// methods must be implemented in a custom class that extends either the <see cref="AnchorPrfabSpawner"/>
        /// or the <see cref="ICustomPrefabAlignment"/> interface directly. </remarks>
        [Serializable]
        public struct AnchorPrefabGroup : IEquatable<AnchorPrefabGroup>
        {
            /// <summary>
            /// Anchors to include. Each anchor labeled with one of these labels will have a prefab spawned for it.
            /// </summary>
            [FormerlySerializedAs("_include")]
            [SerializeField, Tooltip("Anchors to include.")]
            public MRUKAnchor.SceneLabels Labels;

            /// <summary>
            /// This list contains the prefabs that can be spawned for the specified labels.
            /// The prefab will be chosen depending on the <see cref="SelectionMode"/> set for this group.
            /// </summary>
            [SerializeField, Tooltip("Prefab(s) to spawn (randomly chosen from list.)")]
            public List<GameObject> Prefabs;

            /// <summary>
            /// The logic that determines what prefab to choose when spawning the relative labels' game objects.
            /// </summary>
            [SerializeField]
            [Tooltip("The logic that determines what prefab to chose when spawning the relative labels' game objects")]
            public SelectionMode PrefabSelection;

            /// <summary>
            /// When enabled, the prefab will be rotated to try and match the aspect ratio of the volume as closely as possible.
            /// This is most useful for long and thin volumes; keep this disabled for objects with an aspect ratio close to 1:1.
            /// Only applies to volumes.
            /// </summary>
            [SerializeField, Tooltip(
                 "When enabled, the prefab will be rotated to try and match the aspect ratio of the volume as closely as possible. This is most useful for long and thin volumes, keep this disabled for objects with an aspect ratio close to 1:1. Only applies to volumes.")]
            public bool MatchAspectRatio;

            /// <summary>
            /// When calculate facing direction is enabled, the prefab will be rotated to face away from the closest wall.
            /// If match aspect ratio is also enabled, that will take precedence and it will be constrained to a choice between 2 directions only.
            /// Only applies to volumes.
            /// </summary>
            [SerializeField, Tooltip(
                 "When calculate facing direction is enabled the prefab will be rotated to face away from the closest wall. If match aspect ratio is also enabled then that will take precedence and it will be constrained to a choice between 2 directions only.Only applies to volumes.")]
            public bool CalculateFacingDirection;

            /// <summary>
            /// Set what scaling mode to apply to the prefab. By default, the prefab will be stretched to fit the size of the plane/volume.
            /// But in some cases, this may not be desirable and can be customized here.
            /// </summary>
            [SerializeField, Tooltip(
                 "Set what scaling mode to apply to the prefab. By default the prefab will be stretched to fit the size of the plane/volume. But in some cases this may not be desirable and can be customized here.")]
            public ScalingMode Scaling;

            /// <summary>
            /// Spawn new object at the center, top, or bottom of the anchor.
            /// This setting determines the vertical alignment of the spawned prefab.
            /// </summary>
            [SerializeField, Tooltip("Spawn new object at the center, top or bottom of the anchor.")]
            public AlignMode Alignment;

            /// <summary>
            /// Don't analyze prefab, just assume a default scale of 1.
            /// This setting simplifies the scaling process by using a default scale.
            /// </summary>
            [SerializeField, Tooltip("Don't analyze prefab, just assume a default scale of 1.")]
            public bool IgnorePrefabSize;

            /// @cond
            public bool Equals(AnchorPrefabGroup other)
            {
                return Labels == other.Labels && Equals(Prefabs, other.Prefabs) &&
                       PrefabSelection == other.PrefabSelection && MatchAspectRatio == other.MatchAspectRatio &&
                       CalculateFacingDirection == other.CalculateFacingDirection && Scaling == other.Scaling &&
                       Alignment == other.Alignment && IgnorePrefabSize == other.IgnorePrefabSize;
            }

            public override bool Equals(object obj)
            {
                return obj is AnchorPrefabGroup other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine((int)Labels, Prefabs, (int)PrefabSelection, MatchAspectRatio,
                    CalculateFacingDirection, (int)Scaling, (int)Alignment, IgnorePrefabSize);
            }

            public static bool operator ==(AnchorPrefabGroup left, AnchorPrefabGroup right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(AnchorPrefabGroup left, AnchorPrefabGroup right)
            {
                return !left.Equals(right);
            }
            /// @endcond
        }

        /// <summary>
        /// "When the scene data is loaded, this controls what room(s) the prefabs will spawn in.
        /// </summary>
        [Tooltip("When the scene data is loaded, this controls what room(s) the prefabs will spawn in.")]
        public MRUK.RoomFilter SpawnOnStart = MRUK.RoomFilter.CurrentRoomOnly;

        /// <summary>
        /// If enabled, updates on scene elements such as rooms and anchors will be handled by this class.
        /// </summary>
        [Tooltip("If enabled, updates on scene elements such as rooms and anchors will be handled by this class")]
        internal bool TrackUpdates = true;

        /// <summary>
        /// Specify a seed value for consistent prefab selection (0 = Random).
        /// </summary>
        [Tooltip("Specify a seed value for consistent prefab selection (0 = Random).")]
        public int SeedValue;

        /// <summary>
        /// Gets a dictionary that maps MRUKAnchor instances to their corresponding spawned GameObjects.
        /// This should be treated as read-only, do not modify the contents.
        /// </summary>
        public Dictionary<MRUKAnchor, GameObject> AnchorPrefabSpawnerObjects { get; } = new();

        /// <summary>
        /// Event triggered when a prefab is spawned. This event will be deprecated in a future version.
        /// </summary>
        [Obsolete("Event onPrefabSpawned will be deprecated in a future version"), NonSerialized]
        public UnityEvent onPrefabSpawned = new();

        /// <summary>
        /// Use AnchorPrefabSpawnerObjects property instead.
        /// </summary>
        [Obsolete(
            "Use AnchorPrefabSpawnerObjects property instead. This property is inefficient because it will generate a new list each time it is accessed")]
        public List<GameObject> SpawnedPrefabs => new(AnchorPrefabSpawnerObjects.Values);

        /// <summary>
        /// The list of <see cref="AnchorPrefabGroup"/> configurations that determine how prefabs are spawned based on anchor data.
        /// </summary>
        public List<AnchorPrefabGroup> PrefabsToSpawn;

        protected Random _random; // An instance of the Random class used to generate random numbers.
        private MRUK.SceneTrackingSettings SceneTrackingSettings;
        private static readonly string Suffix = "(PrefabSpawner Clone)";
        private Func<Vector3, Vector3> _customPrefabScalingVolume;
        private Func<Bounds, Bounds?, (Vector3, Vector3)> _customPrefabAlignmentVolume;
        private Func<Vector2, Vector2> _customPrefabScalingPlaneRect;
        private Func<Rect, Bounds?, (Vector3, Vector2)> _customPrefabAlignmentPlaneRect;
        private Func<MRUKAnchor, List<GameObject>, GameObject> _customPrefabSelection;

        protected virtual void Start()
        {
            OVRTelemetry.Start(TelemetryConstants.MarkerId.LoadAnchorPrefabSpawner).Send();
            if (MRUK.Instance is null)
            {
                return;
            }

            SceneTrackingSettings.UnTrackedRooms = new();
            SceneTrackingSettings.UnTrackedAnchors = new();

            MRUK.Instance.RegisterSceneLoadedCallback(() =>
            {
                if (SpawnOnStart == MRUK.RoomFilter.None)
                {
                    return;
                }

                switch (SpawnOnStart)
                {
                    case MRUK.RoomFilter.CurrentRoomOnly:
                        SpawnPrefabs(MRUK.Instance.GetCurrentRoom());
                        break;
                    case MRUK.RoomFilter.AllRooms:
                        SpawnPrefabs();
                        break;
                    case MRUK.RoomFilter.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });

            if (!TrackUpdates)
            {
                return;
            }
        }


        protected virtual void OnEnable()
        {
            if (MRUK.Instance)
            {
                MRUK.Instance.RoomCreatedEvent.AddListener(ReceiveCreatedRoom);
                MRUK.Instance.RoomRemovedEvent.AddListener(ReceiveRemovedRoom);
            }
        }

        protected virtual void OnDisable()
        {
            if (MRUK.Instance)
            {
                MRUK.Instance.RoomCreatedEvent.RemoveListener(ReceiveCreatedRoom);
                MRUK.Instance.RoomRemovedEvent.RemoveListener(ReceiveRemovedRoom);
            }
        }

        /// <summary>
        /// Handles the event when a room is removed from the scene. This method clears all prefabs associated with the room
        /// and unregisters updates for anchors within the room.
        /// </summary>
        /// <param name="room">The room that has been removed.</param>
        protected virtual void ReceiveRemovedRoom(MRUKRoom room)
        {
            ClearPrefabs(room);
            UnRegisterAnchorUpdates(room);
        }

        /// <summary>
        /// Unregisters the anchor update events for a specific room. This method stops listening for anchor creation, removal,
        /// and update events within the specified room.
        /// </summary>
        /// <param name="room">The room for which to unregister anchor update events.</param>
        protected virtual void UnRegisterAnchorUpdates(MRUKRoom room)
        {
            room.AnchorCreatedEvent.RemoveListener(ReceiveAnchorCreatedEvent);
            room.AnchorRemovedEvent.RemoveListener(ReceiveAnchorRemovedCallback);
            room.AnchorUpdatedEvent.RemoveListener(ReceiveAnchorUpdatedCallback);
        }

        /// <summary>
        /// Registers the anchor update events for a specific room. This method starts listening for anchor creation, removal,
        /// and update events within the specified room.
        /// </summary>
        /// <param name="room">The room for which to register anchor update events.</param>
        protected virtual void RegisterAnchorUpdates(MRUKRoom room)
        {
            room.AnchorCreatedEvent.AddListener(ReceiveAnchorCreatedEvent);
            room.AnchorRemovedEvent.AddListener(ReceiveAnchorRemovedCallback);
            room.AnchorUpdatedEvent.AddListener(ReceiveAnchorUpdatedCallback);
        }

        /// <summary>
        /// Responds to the event of an anchor being updated within the scene. This method clears existing prefabs and triggers
        /// the spawning of new prefabs based on the updated anchor information, provided that updates are being tracked and
        /// the anchor or its parent room is not marked as untracked.
        /// </summary>
        /// <param name="anchorInfo">The anchor that has been updated.</param>
        protected virtual void ReceiveAnchorUpdatedCallback(MRUKAnchor anchorInfo)
        {
            // only update the anchor when we track updates
            // &
            // only create when the anchor or parent room is tracked
            if (SceneTrackingSettings.UnTrackedRooms.Contains(anchorInfo.Room) ||
                SceneTrackingSettings.UnTrackedAnchors.Contains(anchorInfo) ||
                !TrackUpdates)
            {
                return;
            }

            ClearPrefabs();
            SpawnPrefabs(anchorInfo);
        }

        /// <summary>
        /// Responds to the event of an anchor being removed from the scene. This method clears all prefabs spawned by this spawner.
        /// </summary>
        /// <param name="anchorInfo">The anchor that has been removed.</param>
        protected virtual void ReceiveAnchorRemovedCallback(MRUKAnchor anchorInfo)
        {
            ClearPrefabs();
        }

        /// <summary>
        /// Responds to the event of a new anchor being created within the scene. This method triggers the spawning of prefabs
        /// if the anchor's room is being tracked and updates are enabled.
        /// </summary>
        /// <param name="anchorInfo">The anchor that has been created.</param>
        protected virtual void ReceiveAnchorCreatedEvent(MRUKAnchor anchorInfo)
        {
            // only create the anchor when we track updates
            // &
            // only create when the parent room is tracked
            if (SceneTrackingSettings.UnTrackedRooms.Contains(anchorInfo.Room) ||
                !TrackUpdates)
            {
                return;
            }

            SpawnPrefabs();
        }

        /// <summary>
        /// Responds to the event of a new room being created within the scene. This method triggers the spawning of prefabs
        /// and registers for anchor updates within the room, provided that room updates are being tracked and the configuration
        /// is set to handle all rooms.
        /// </summary>
        /// <param name="room">The room that has been created.</param>
        protected virtual void ReceiveCreatedRoom(MRUKRoom room)
        {
            //only create the room when we track room updates
            if (TrackUpdates &&
                SpawnOnStart == MRUK.RoomFilter.AllRooms)
            {
                SpawnPrefabs(room);
                RegisterAnchorUpdates(room);
            }
        }

        /// <summary>
        ///  Clears all the previously spawned gameobjects from this AnchorPrefabSpawner instance in the given room.
        /// </summary>
        /// <param name="room">The room from where to remove all the spawned objects.</param>
        protected virtual void ClearPrefabs(MRUKRoom room)
        {
            List<MRUKAnchor> anchorsToRemove = new();
            foreach (var kv in AnchorPrefabSpawnerObjects)
            {
                if (kv.Key.Room != room)
                {
                    continue;
                }

                ClearPrefab(kv.Value);
                anchorsToRemove.Add(kv.Key);
            }

            foreach (var anchor in anchorsToRemove)
            {
                AnchorPrefabSpawnerObjects.Remove(anchor);
            }

            SceneTrackingSettings.UnTrackedRooms.Add(room);
        }

        /// <summary>
        /// Destroys the specified GameObject, effectively removing the prefab from the scene
        /// and removing it from the <see cref="AnchorPrefabSpawnerObjects"/> dictionary.
        /// </summary>
        /// <param name="go">The GameObject to be destroyed.</param>
        protected virtual void ClearPrefab(GameObject go)
        {
            Destroy(go);
        }

        /// <summary>
        /// Clears the gameobject associated with the anchor. Useful when receiving an event that a
        /// specific anchor has been removed
        /// </summary>
        /// <param name="anchorInfo">The anchor reference</param>
        protected virtual void ClearPrefab(MRUKAnchor anchorInfo)
        {
            if (!AnchorPrefabSpawnerObjects.ContainsKey(anchorInfo))
            {
                return;
            }

            ClearPrefab(AnchorPrefabSpawnerObjects[anchorInfo]);
            AnchorPrefabSpawnerObjects.Remove(anchorInfo);
            SceneTrackingSettings.UnTrackedAnchors.Add(anchorInfo);
        }

        /// <summary>
        /// Clears all the gameobjects created by this AnchorPrefabSpawner instance for all rooms.
        /// </summary>
        protected virtual void ClearPrefabs()
        {
            foreach (var kv in AnchorPrefabSpawnerObjects)
            {
                ClearPrefab(kv.Value);
            }

            AnchorPrefabSpawnerObjects.Clear();
        }


        /// <summary>
        /// Spawns prefabs for all the rooms in the scene.
        /// This method will iterate through all the rooms in the scene and spawn prefabs for each anchor in the room
        /// according to the <see cref="AnchorPrefabGroup"/> configuration of each label.
        /// </summary>
        /// <param name="clearPrefabs">Clear already existing prefabs before.</param>
        protected virtual void SpawnPrefabs(bool clearPrefabs = true)
        {
            // Perform a cleanup if necessary
            if (clearPrefabs)
            {
                ClearPrefabs();
            }

            foreach (var room in MRUK.Instance.Rooms)
            {
                SpawnPrefabsInternal(room);
            }
#pragma warning disable CS0618 // Type or member is obsolete
            onPrefabSpawned?.Invoke();
#pragma warning restore CS0618 // Type or member is obsolete
        }

        /// <summary>
        /// Spawns prefabs for all the rooms in the scene.
        /// This method will spawn prefabs for each anchor in the room according to the <see cref="AnchorPrefabGroup"/> configurations
        /// of each label.
        /// </summary>
        /// <param name="room">The room reference</param>
        /// <param name="clearPrefabs">clear all before adding them again</param>
        protected virtual void SpawnPrefabs(MRUKRoom room, bool clearPrefabs = true)
        {
            // Perform a cleanup if necessary
            if (clearPrefabs)
            {
                ClearPrefabs();
            }

            SpawnPrefabsInternal(room);
#pragma warning disable CS0618 // Type or member is obsolete
            onPrefabSpawned?.Invoke();
#pragma warning restore CS0618 // Type or member is obsolete
        }

        private void SpawnPrefabsInternal(MRUKRoom room)
        {
            InitializeRandom(ref SeedValue);
            foreach (var anchor in room.Anchors)
            {
                SpawnPrefab(anchor);
            }
        }

        /// <summary>
        /// Spawns a prefab based on the provided anchor information. This method determines the appropriate prefab to spawn
        /// based on the anchor's label, checks for existing instances, and configures the spawned prefab's position, scale,
        /// and orientation according to the anchor's properties and predefined settings.
        /// </summary>
        /// <param name="anchorInfo">The anchor based on which the prefab will be spawned.</param>
        protected virtual void SpawnPrefab(MRUKAnchor anchorInfo)
        {
            var prefabToCreate = LabelToPrefab(anchorInfo.Label, anchorInfo, out var prefabGroup);
            if (prefabToCreate == null)
            {
                return;
            }

            if (AnchorPrefabSpawnerObjects.ContainsKey(anchorInfo))
            {
                Debug.LogWarning("Anchor already associated with a gameobject spawned from this AnchorPrefabSpawner");
                return;
            }

            // Create a new instance of the prefab
            // We will translate location and scale differently depending on the label.
            var prefab = Instantiate(prefabToCreate, anchorInfo.transform);
            prefab.name = string.Concat(prefabToCreate.name, Suffix);
            prefab.name = prefabToCreate.name + Suffix;
            prefab.transform.parent = anchorInfo.transform;

            var prefabBounds = prefabGroup.IgnorePrefabSize ? null : Utilities.GetPrefabBounds(prefabToCreate);

            if (!prefabBounds.HasValue)
            {
                prefabBounds = prefab.GetComponentInChildren<GridSliceResizer>(true)?.OriginalMesh.bounds;
            }

            var prefabSize = prefabBounds?.size ?? Vector3.one;

            if (anchorInfo.VolumeBounds.HasValue)
            {
                var cardinalAxisIndex = 0;
                if (prefabGroup.CalculateFacingDirection && !prefabGroup.MatchAspectRatio)
                {
                    anchorInfo.Room.GetDirectionAwayFromClosestWall(anchorInfo, out cardinalAxisIndex);
                }

                var volumeBounds = AnchorPrefabSpawnerUtilities.RotateVolumeBounds(anchorInfo.VolumeBounds.Value,
                    cardinalAxisIndex);

                var volumeSize = volumeBounds.size;
                var scale = new Vector3(volumeSize.x / prefabSize.x, volumeSize.z / prefabSize.y,
                    volumeSize.y / prefabSize.z); // flipped z and y to correct orientation

                if (prefabGroup.MatchAspectRatio)
                {
                    AnchorPrefabSpawnerUtilities.MatchAspectRatio(anchorInfo, prefabGroup.CalculateFacingDirection,
                        prefabSize, volumeSize, ref cardinalAxisIndex, ref volumeBounds, ref scale);
                }

                scale = prefabGroup.Scaling == ScalingMode.Custom
                    ? CustomPrefabScaling(scale)
                    : AnchorPrefabSpawnerUtilities.ScalePrefab(scale, prefabGroup.Scaling);

                var localPosition = prefabGroup.Alignment == AlignMode.Custom
                    ? CustomPrefabAlignment(volumeBounds, prefabBounds)
                    : AnchorPrefabSpawnerUtilities.AlignPrefabPivot(volumeBounds, prefabBounds, scale,
                        prefabGroup.Alignment);

                prefab.transform.localPosition = Quaternion.AngleAxis(cardinalAxisIndex * 90, Vector3.forward) * localPosition;

                // scene geometry is unusual, we need to swap Y/Z for a more standard prefab structure
                prefab.transform.localRotation = Quaternion.Euler((cardinalAxisIndex - 1) * 90, -90, -90);
                prefab.transform.localScale = scale;
            }

            else if (anchorInfo.PlaneRect.HasValue)
            {
                var planeSize = anchorInfo.PlaneRect.Value.size;
                var scale = new Vector2(planeSize.x / prefabSize.x, planeSize.y / prefabSize.y);

                prefab.transform.localScale = prefabGroup.Scaling == ScalingMode.Custom
                    ? CustomPrefabScaling(scale)
                    : AnchorPrefabSpawnerUtilities.ScalePrefab(scale, prefabGroup.Scaling);

                prefab.transform.localPosition = prefabGroup.Alignment == AlignMode.Custom
                    ? CustomPrefabAlignment(anchorInfo.PlaneRect.Value, prefabBounds)
                    : AnchorPrefabSpawnerUtilities.AlignPrefabPivot(anchorInfo.PlaneRect.Value, prefabBounds, scale,
                        prefabGroup.Alignment);
            }

            AnchorPrefabSpawnerObjects.Add(anchorInfo, prefab);
        }

        private GameObject LabelToPrefab(MRUKAnchor.SceneLabels labels, MRUKAnchor anchor,
            out AnchorPrefabGroup prefabGroup)
        {
            foreach (var item in PrefabsToSpawn)
            {
                if ((item.Labels & labels) == 0 || ((item.Prefabs == null ||
                                                     item.Prefabs.Count == 0) &&
                                                    item.PrefabSelection != SelectionMode.Custom))
                {
                    continue;
                }

                GameObject prefabObjectToSpawn = null;
                if (item.PrefabSelection == SelectionMode.Custom)
                {
                    prefabObjectToSpawn = CustomPrefabSelection(anchor, item.Prefabs);
                }
                else
                {
                    prefabObjectToSpawn =
                        AnchorPrefabSpawnerUtilities.SelectPrefab(anchor, item.PrefabSelection, item.Prefabs,
                            _random);
                }

                prefabGroup = item;
                return prefabObjectToSpawn;
            }

            prefabGroup = new();
            return null;
        }

        /// <summary>
        /// Initializes a new instance of the Random class using the specified seed.
        /// either the seed value or the current system tick count is used.
        /// </summary>
        /// <param name="seed">The seed value to initialize the random number generator.
        /// If zero, the seed will be set to the current system tick count.
        /// </param>
        public void InitializeRandom(ref int seed)
        {
            if (seed == 0)
            {
                seed = Environment.TickCount;
            }

            _random = new Random(seed);
        }

        /// <summary>
        /// Custom logic for selecting a prefab. Extend this class and override this method with your custom logic.
        /// All <see cref="AnchorPrefabGroup"/> with <see cref="SelectionMode"/> set to Custom will use this method.
        /// </summary>
        /// <param name="anchor">The anchor information.</param>
        /// <param name="prefabs">The list of prefabs to choose from.</param>
        /// <returns>The selected prefab GameObject.</returns>
        public virtual GameObject CustomPrefabSelection(MRUKAnchor anchor, List<GameObject> prefabs)
        {
            throw new(
                "A custom prefab selection method was selected but no implementation was provided. " +
                "Extend this class and override the `CustomPrefabSelection` method with your custom logic.");
        }

        /// <summary>
        /// Custom logic for scaling a prefab's volume. Extend this class and override this method with your custom logic.
        /// All <see cref="AnchorPrefabGroup"/> with <see cref="ScalingMode"/> set to Custom will use this method.
        /// </summary>
        /// <param name="localScale">The local scale vector.</param>
        /// <returns>The adjusted scale vector.</returns>
        public virtual Vector3 CustomPrefabScaling(Vector3 localScale)
        {
            throw new NotImplementedException(
                "A custom scaling method for an anchor's volume is selected but no implementation " +
                "was provided. Extend this class and override the `CustomPrefabVolumeScaling` method with your custom logic.");
        }

        /// <summary>
        /// Custom logic for scaling a prefab's plane rectangle. Extend this class and override this method with your custom logic.
        /// </summary>
        /// <param name="localScale">The local scale vector.</param>
        /// <returns>The adjusted scale vector.</returns>
        public virtual Vector2 CustomPrefabScaling(Vector2 localScale)
        {
            throw new NotImplementedException(
                "A custom scaling method was selected but no implementation was provided. " +
                "Extend this class and override the `CustomPrefabPlaneRectScaling` method with your custom logic.");
        }

        /// <summary>
        /// Custom logic for aligning a prefab within a volume. Extend this class and override this method with your custom logic.
        /// All <see cref="AnchorPrefabGroup"/> with <see cref="AlignMode"/> set to Custom will use this method.
        /// </summary>
        /// <param name="anchorVolumeBounds">The bounds of the anchor volume.</param>
        /// <param name="prefabBounds">The optional bounds of the prefab.</param>
        /// <returns>The adjusted position vector.</returns>
        public virtual Vector3 CustomPrefabAlignment(Bounds anchorVolumeBounds, Bounds? prefabBounds)
        {
            throw new NotImplementedException(
                "A custom volume alignment method was selected but no implementation was provided." +
                "Extend this class and override the `CustomPrefabAlignment` method with your custom logic.");
        }

        /// <summary>
        /// Custom logic for aligning a prefab within a plane rectangle. Extend this class and override this method with your custom logic.
        /// All <see cref="AnchorPrefabGroup"/> with <see cref="AlignMode"/> set to Custom will use this method.
        /// </summary>
        /// <param name="anchorPlaneRect">The rectangle of the anchor plane.</param>
        /// <param name="prefabBounds">The optional bounds of the prefab.</param>
        /// <returns>The adjusted position vector.</returns>
        public virtual Vector3 CustomPrefabAlignment(Rect anchorPlaneRect, Bounds? prefabBounds)
        {
            throw new NotImplementedException(
                "A custom prefab selection method was selected but no implementation was provided. " +
                "Extend this class and override the `CustomPrefabAlignment` method with your custom logic.");
        }

        private void OnDestroy()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            onPrefabSpawned.RemoveAllListeners();
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
