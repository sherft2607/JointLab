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
using UnityEngine.Serialization;

namespace Meta.XR.MRUtilityKit
{
    /// <summary>
    /// Allows for fast generation of valid (inside the room, outside furniture bounds) random positions for content spawning.
    /// Optional method to pin directly to surfaces.
    /// This class leverages the <see cref="MRUKRoom.GenerateRandomPositionInRoom"/> and <see cref="MRUKRoom.GenerateRandomPositionOnSurface"/> methods
    /// to provide a simple interface for spawning content in the room.
    /// </summary>
    [Feature(Feature.Scene)]
    public class FindSpawnPositions : MonoBehaviour
    {
        /// <summary>
        /// When the scene data is loaded, this controls what room(s) the prefabs will spawn in.
        /// </summary>
        [Tooltip("When the scene data is loaded, this controls what room(s) the prefabs will spawn in.")]
        public MRUK.RoomFilter SpawnOnStart = MRUK.RoomFilter.CurrentRoomOnly;

        /// <summary>
        /// Prefab to be placed into the scene, or object in the scene to be moved around.
        /// </summary>
        [SerializeField, Tooltip("Prefab to be placed into the scene, or object in the scene to be moved around.")]
        public GameObject SpawnObject;

        /// <summary>
        /// Number of SpawnObject(s) to place into the scene per room, only applies to Prefabs.
        /// </summary>
        [SerializeField, Tooltip("Number of SpawnObject(s) to place into the scene per room, only applies to Prefabs.")]
        public int SpawnAmount = 8;

        /// <summary>
        /// Maximum number of times to attempt spawning/moving an object before giving up.
        /// </summary>
        [SerializeField, Tooltip("Maximum number of times to attempt spawning/moving an object before giving up.")]
        public int MaxIterations = 1000;

        /// <summary>
        /// Defines possible locations where objects can be spawned.
        /// </summary>
        public enum SpawnLocation
        {
            Floating, // Spawn somewhere floating in the free space within the room
            AnySurface, // Spawn on any surface (i.e. a combination of all 3 options below)
            VerticalSurfaces, // Spawn only on vertical surfaces such as walls, windows, wall art, doors, etc...
            OnTopOfSurfaces, // Spawn on surfaces facing upwards such as ground, top of tables, beds, couches, etc...
            HangingDown // Spawn on surfaces facing downwards such as the ceiling
        }

        /// <summary>
        /// Attach content to scene surfaces.
        /// </summary>
        [FormerlySerializedAs("selectedSnapOption")]
        [SerializeField, Tooltip("Attach content to scene surfaces.")]
        public SpawnLocation SpawnLocations = SpawnLocation.Floating;

        /// <summary>
        /// When using surface spawning, use this to filter which anchor labels should be included. Eg, spawn only on TABLE or OTHER.
        /// </summary>
        [SerializeField, Tooltip("When using surface spawning, use this to filter which anchor labels should be included. Eg, spawn only on TABLE or OTHER.")]
        public MRUKAnchor.SceneLabels Labels = ~(MRUKAnchor.SceneLabels)0;

        /// <summary>
        /// If enabled then the spawn position will be checked to make sure there is no overlap with physics colliders including themselves.
        /// </summary>
        [SerializeField, Tooltip("If enabled then the spawn position will be checked to make sure there is no overlap with physics colliders including themselves.")]
        public bool CheckOverlaps = true;

        /// <summary>
        /// Required free space for the object (Set negative to auto-detect using GetPrefabBounds)
        /// default to auto-detect. This value represents the extents of the bounding box
        /// </summary>
        [SerializeField, Tooltip("Required free space for the object (Set negative to auto-detect using GetPrefabBounds)")]
        public float OverrideBounds = -1;

        /// <summary>
        /// Set the layer(s) for the physics bounding box checks, collisions will be avoided with these layers.
        /// </summary>
        [FormerlySerializedAs("layerMask")]
        [SerializeField, Tooltip("Set the layer(s) for the physics bounding box checks, collisions will be avoided with these layers.")]
        public LayerMask LayerMask = -1;

        /// <summary>
        /// The clearance distance required in front of the surface in order for it to be considered a valid spawn position
        /// </summary>
        [SerializeField, Tooltip("The clearance distance required in front of the surface in order for it to be considered a valid spawn position")]
        public float SurfaceClearanceDistance = 0.1f;

        private void Start()
        {
            OVRTelemetry.Start(TelemetryConstants.MarkerId.LoadFindSpawnPositions).Send();
            if (MRUK.Instance && SpawnOnStart != MRUK.RoomFilter.None)
            {
                MRUK.Instance.RegisterSceneLoadedCallback(() =>
                {
                    switch (SpawnOnStart)
                    {
                        case MRUK.RoomFilter.AllRooms:
                            StartSpawn();
                            break;
                        case MRUK.RoomFilter.CurrentRoomOnly:
                            StartSpawn(MRUK.Instance.GetCurrentRoom());
                            break;
                    }
                });
            }
        }

        /// <summary>
        /// Starts the spawning process for all rooms.
        /// </summary>
        public void StartSpawn()
        {
            foreach (var room in MRUK.Instance.Rooms)
            {
                StartSpawn(room);
            }
        }

        /// <summary>
        /// Starts the spawning process for a specific room. A maximum of <see cref="MaxIterations"/> attempts will be made to find a valid spawn position.
        /// <see cref="MRUKRoom.GenerateRandomPositionInRoom"/> and <see cref="MRUKRoom.GenerateRandomPositionOnSurface"/> are used to generate the positions.
        /// </summary>
        /// <param name="room">The room to spawn objects in.</param>
        public void StartSpawn(MRUKRoom room)
        {
            var prefabBounds = Utilities.GetPrefabBounds(SpawnObject);
            float minRadius = 0.0f;
            const float clearanceDistance = 0.01f;
            float baseOffset = -prefabBounds?.min.y ?? 0.0f;
            float centerOffset = prefabBounds?.center.y ?? 0.0f;
            Bounds adjustedBounds = new();

            if (prefabBounds.HasValue)
            {
                minRadius = Mathf.Min(-prefabBounds.Value.min.x, -prefabBounds.Value.min.z, prefabBounds.Value.max.x, prefabBounds.Value.max.z);
                if (minRadius < 0f)
                {
                    minRadius = 0f;
                }

                var min = prefabBounds.Value.min;
                var max = prefabBounds.Value.max;
                min.y += clearanceDistance;
                if (max.y < min.y)
                {
                    max.y = min.y;
                }

                adjustedBounds.SetMinMax(min, max);
                if (OverrideBounds > 0)
                {
                    Vector3 center = new Vector3(0f, clearanceDistance, 0f);
                    Vector3 size = new Vector3(OverrideBounds * 2f, clearanceDistance * 2f, OverrideBounds * 2f); // OverrideBounds represents the extents, not the size
                    adjustedBounds = new Bounds(center, size);
                }
            }

            for (int i = 0; i < SpawnAmount; ++i)
            {
                bool foundValidSpawnPosition = false;
                for (int j = 0; j < MaxIterations; ++j)
                {
                    Vector3 spawnPosition = Vector3.zero;
                    Vector3 spawnNormal = Vector3.zero;
                    if (SpawnLocations == SpawnLocation.Floating)
                    {
                        var randomPos = room.GenerateRandomPositionInRoom(minRadius, true);
                        if (!randomPos.HasValue)
                        {
                            break;
                        }

                        spawnPosition = randomPos.Value;
                    }
                    else
                    {
                        MRUK.SurfaceType surfaceType = 0;
                        switch (SpawnLocations)
                        {
                            case SpawnLocation.AnySurface:
                                surfaceType |= MRUK.SurfaceType.FACING_UP;
                                surfaceType |= MRUK.SurfaceType.VERTICAL;
                                surfaceType |= MRUK.SurfaceType.FACING_DOWN;
                                break;
                            case SpawnLocation.VerticalSurfaces:
                                surfaceType |= MRUK.SurfaceType.VERTICAL;
                                break;
                            case SpawnLocation.OnTopOfSurfaces:
                                surfaceType |= MRUK.SurfaceType.FACING_UP;
                                break;
                            case SpawnLocation.HangingDown:
                                surfaceType |= MRUK.SurfaceType.FACING_DOWN;
                                break;
                        }

                        if (room.GenerateRandomPositionOnSurface(surfaceType, minRadius, new LabelFilter(Labels), out var pos, out var normal))
                        {
                            spawnPosition = pos + normal * baseOffset;
                            spawnNormal = normal;
                            var center = spawnPosition + normal * centerOffset;
                            // In some cases, surfaces may protrude through walls and end up outside the room
                            // check to make sure the center of the prefab will spawn inside the room
                            if (!room.IsPositionInRoom(center))
                            {
                                continue;
                            }

                            // Ensure the center of the prefab will not spawn inside a scene volume
                            if (room.IsPositionInSceneVolume(center))
                            {
                                continue;
                            }

                            // Also make sure there is nothing close to the surface that would obstruct it
                            if (room.Raycast(new Ray(pos, normal), SurfaceClearanceDistance, out _))
                            {
                                continue;
                            }
                        }
                    }

                    Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, spawnNormal);
                    if (CheckOverlaps && prefabBounds.HasValue)
                    {
                        if (Physics.CheckBox(spawnPosition + spawnRotation * adjustedBounds.center, adjustedBounds.extents, spawnRotation, LayerMask, QueryTriggerInteraction.Ignore))
                        {
                            continue;
                        }
                    }

                    foundValidSpawnPosition = true;

                    if (SpawnObject.gameObject.scene.path == null)
                    {
                        Instantiate(SpawnObject, spawnPosition, spawnRotation, transform);
                    }
                    else
                    {
                        SpawnObject.transform.position = spawnPosition;
                        SpawnObject.transform.rotation = spawnRotation;
                        return; // ignore SpawnAmount once we have a successful move of existing object in the scene
                    }

                    break;
                }

                if (!foundValidSpawnPosition)
                {
                    Debug.LogWarning($"Failed to find valid spawn position after {MaxIterations} iterations. Only spawned {i} prefabs instead of {SpawnAmount}.");
                    break;
                }
            }
        }
    }
}
