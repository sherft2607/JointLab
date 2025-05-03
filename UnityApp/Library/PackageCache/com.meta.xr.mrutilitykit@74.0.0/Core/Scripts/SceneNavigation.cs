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
using UnityEngine;
using UnityEngine.Events;
using Unity.AI.Navigation;
using UnityEngine.AI;
using System.Collections.Generic;
using Meta.XR.Util;
using UnityEngine.Serialization;

namespace Meta.XR.MRUtilityKit
{
    /// <summary>
    /// Manages the creation and updating of a navigation mesh (NavMesh) for scene navigation.
    /// This class handles dynamic NavMesh generation based on scene data, including rooms and anchors, and responds to changes in the scene.
    /// </summary>
    [Feature(Feature.Scene)]
    public class SceneNavigation : MonoBehaviour
    {
        [Tooltip("When the scene data is loaded, this controls what room(s) will be used when baking the NavMesh.")]
        public MRUK.RoomFilter BuildOnSceneLoaded = MRUK.RoomFilter.CurrentRoomOnly;

        [Tooltip("If enabled, updates on scene elements such as rooms and anchors will be handled by this class")]
        internal bool TrackUpdates = true;

        [Tooltip("Used for specifying the type of geometry to collect when building a NavMesh")]
        public NavMeshCollectGeometry CollectGeometry = NavMeshCollectGeometry.PhysicsColliders;

        [Tooltip("Used for specifying the type of objects to include when building a NavMesh")]
        public CollectObjects CollectObjects = CollectObjects.Children;

        [Tooltip("The minimum distance to the walls where the navigation mesh can exist.")]
        public float AgentRadius = 0.2f;

        [Tooltip("How much vertical clearance space must exist.")]
        public float AgentHeight = 0.5f;

        [Tooltip("The height of discontinuities in the level the agent can climb over (i.e. steps and stairs).")]
        public float AgentClimb = 0.04f;

        [Tooltip("Maximum slope the agent can walk up.")]
        public float AgentMaxSlope = 5.5f;

        [Tooltip("The agents that will be assigned to the NavMesh generated with the scene data.")]
        public List<NavMeshAgent> Agents;

        [FormerlySerializedAs("SceneObjectsToInclude")]
        [Tooltip("The scene objects that will contribute to the creation of the NavMesh.")]
        public MRUKAnchor.SceneLabels NavigableSurfaces;

        [Tooltip("The scene objects that will carve a hole in the NavMesh.")]
        public MRUKAnchor.SceneLabels SceneObstacles;

        [Tooltip("A bitmask representing the layers to consider when selecting what that will be used for baking.")]
        public LayerMask Layers;

        [Tooltip("The agent's used that is going to be used to build the NavMesh")]
        public int AgentIndex;

        [Tooltip(
            "Determines whether scene data should be used for NavMesh generation.")]
        public bool UseSceneData = true;

        [Tooltip(
            "Determines whether a custom NavMeshAgent configuration should be used. If true, a new agent will be created when building this the NavMesh.")]
        public bool CustomAgent = true;

        [Tooltip(
            "Allows overriding the default voxel size used in NavMesh generation. Enable this to specify a custom voxel size.")]
        public bool OverrideVoxelSize;

        [Tooltip("The NavMesh voxel size in world length units. Should be 4-6 voxels per character diameter.")]
        public float VoxelSize;

        [Tooltip(
            "Allows overriding the default tile size used in NavMesh generation. Enable this to specify a custom tile size.")]
        public bool OverrideTileSize;

        [Tooltip(
            "Specifies the tile size for the NavMesh if OverrideTileSize is enabled. Represents the width and height of the square tiles in world units.")]
        public int TileSize = 256;
#if UNITY_2022_3_OR_NEWER
[Tooltip("Enables the generation of off-mesh links in the NavMesh, allowing agents to navigate between disconnected mesh regions, such as jumping or climbing.")]
        public bool GenerateLinks;
#endif
        private EffectMesh _effectMesh;
        private readonly List<NavMeshBuildSource> _sources = new();
        private readonly List<Mesh> _connectionMeshes = new();

        /// <summary>
        /// Event triggered when the navigation mesh has been initialized.
        /// </summary>
        [field: SerializeField]
        [field: Space(10)]
        public UnityEvent OnNavMeshInitialized
        {
            get;
            private set;
        } = new();

        /// <summary>
        /// Gets a dictionary mapping <see cref="MRUKAnchor"/> objects to their corresponding GameObjects that represent obstacles in the environment.
        /// </summary>
        public Dictionary<MRUKAnchor, GameObject> Obstacles
        {
            get;
            private set;
        } = new();


        /// <summary>
        /// Gets a dictionary mapping <see cref="MRUKAnchor"/> objects to their corresponding GameObjects that represent surfaces in the environment.
        /// </summary>
        [Obsolete(
            "Navigable surfaces are now handled as NavMeshBuildSource hence this container is not going to be populated." +
            "Access the anchors used as navigable surfaces directly.")]
        public Dictionary<MRUKAnchor, GameObject> Surfaces { get; private set; } = new();

        private const float _minimumNavMeshSurfaceArea = 0;
        private NavMeshSurface _navMeshSurface;
        private const string _obstaclePrefix = "_obstacles";
        private Transform _obstaclesRoot;
        private Transform _surfacesRoot;

        private Transform ObstacleRoot
        {
            get
            {
                if (_obstaclesRoot == null)
                {
                    _obstaclesRoot = new GameObject(_obstaclePrefix).transform;
                }

                return _obstaclesRoot;
            }
        }

        private MRUKAnchor.SceneLabels _cachedNavigableSceneLabels;

        private void Awake()
        {
            _navMeshSurface = gameObject.GetComponent<NavMeshSurface>();
            _cachedNavigableSceneLabels = NavigableSurfaces;
        }

        private void Start()
        {
            OVRTelemetry.Start(TelemetryConstants.MarkerId.LoadSceneNavigation).Send();
            if (MRUK.Instance is null)
            {
                return;
            }

            MRUK.Instance.RegisterSceneLoadedCallback(OnSceneLoadedEvent);

            if (!TrackUpdates)
            {
                return;
            }

            MRUK.Instance.RoomCreatedEvent.AddListener(ReceiveCreatedRoom);
            MRUK.Instance.RoomRemovedEvent.AddListener(ReceiveRemovedRoom);
            MRUK.Instance.RoomUpdatedEvent.AddListener(ReceiveUpdatedRoom);
        }

        private void OnSceneLoadedEvent()
        {

            switch (BuildOnSceneLoaded)
            {
                case MRUK.RoomFilter.CurrentRoomOnly:
                    BuildSceneNavMeshForRoom(MRUK.Instance.GetCurrentRoom());
                    break;
                case MRUK.RoomFilter.AllRooms:
                    BuildSceneNavMesh();
                    break;
                case MRUK.RoomFilter.None:
                default:
                    break;
            }
        }

        private void ReceiveCreatedRoom(MRUKRoom room)
        {
            if (!TrackUpdates)
            {
                return;
            }

            BuildSceneNavMeshForRoom(room);
        }

        private void ReceiveUpdatedRoom(MRUKRoom room)
        {
            if (!TrackUpdates)
            {
                return;
            }

            RemoveNavMeshData();
            BuildSceneNavMeshForRoom(room);
        }

        private void ReceiveRemovedRoom(MRUKRoom room)
        {
            if (!TrackUpdates)
            {
                return;
            }

            RemoveNavMeshData();
        }

        /// <summary>
        ///     Toggles the use of global mesh for navigation.
        /// </summary>
        /// <param name="useGlobalMesh">Whether to use the global mesh to build the NavMesh</param>
        /// <param name="agentTypeID">
        ///     The agent type ID to use for creating the scene nav mesh,
        ///     if not specified, a new agent will be created.
        /// </param>
        public void ToggleGlobalMeshNavigation(bool useGlobalMesh, int agentTypeID = -1)
        {
            if (useGlobalMesh && MRUK.Instance.GetCurrentRoom().GlobalMeshAnchor == null)
            {
                Debug.LogWarning("[MRUK] No Global Mesh anchor was found in the scene.");
                return;
            }

            if (useGlobalMesh)
            {
                _cachedNavigableSceneLabels = NavigableSurfaces;
                NavigableSurfaces = MRUKAnchor.SceneLabels.GLOBAL_MESH;
            }
            else
            {
                NavigableSurfaces = _cachedNavigableSceneLabels;
            }

            BuildSceneNavMesh();
        }

        /// <summary>
        ///     Creates a navigation mesh for the entire scene.
        /// </summary>
        public void BuildSceneNavMesh()
        {
            // Use all the rooms
            BuildSceneNavMeshForRoom();
        }

        /// <summary>
        ///     Creates a navigation mesh for the scene.
        /// </summary>
        /// <param name="room">
        ///     Optional parameter for the MRUKRoom to create the NavMesh for.
        ///      If not provided, obstacles will be created for all rooms.
        /// </param>
        /// <remarks>
        ///     This method creates a navigation mesh by collecting geometry from the scene,
        ///     building the navigation mesh data, and adding it to the NavMesh.
        ///     Currently, Unity does not allow the creation of custom NavMeshAgents at runtime.
        ///     It also assigns the created navigation mesh to all NavMeshAgents in the scene.
        /// </remarks>
        public void BuildSceneNavMeshForRoom(MRUKRoom room = null)
        {
            if (!MRUK.Instance)
            {
                throw new NullReferenceException("MRUK instance is not initialized.");
            }
            var rooms = room != null ? new List<MRUKRoom> { room } : MRUK.Instance.Rooms;
            if (rooms.Count == 0)
            {
                throw new InvalidOperationException("No rooms available for NavMesh building.");
            }

            CreateNavMeshSurface(); // in case no NavMeshSurface component was found, create a new one
            RemoveNavMeshData(); // clean up previous data
            var navMeshBounds =
                ResizeNavMeshFromRoomBounds(ref _navMeshSurface, rooms); // resize the nav mesh to fit the room
            _sources.Clear();
            var navMeshBuildSettings = !CustomAgent
                ? NavMesh.GetSettingsByIndex(AgentIndex)
                : CreateNavMeshBuildSettings(AgentRadius, AgentHeight, AgentMaxSlope, AgentClimb);

            _navMeshSurface.agentTypeID = navMeshBuildSettings.agentTypeID;

            ValidateBuildSettings(navMeshBuildSettings, navMeshBounds);

            if (UseSceneData)
            {
                CreateObstacles(rooms);
                CollectSceneSources(rooms, _sources);
            }
            else
            {
#if UNITY_2022_3_OR_NEWER
                NavMeshBuilder.CollectSources(navMeshBounds, _navMeshSurface.layerMask,
                    _navMeshSurface.useGeometry, 0, GenerateLinks, new List<NavMeshBuildMarkup>(), false, _sources);
#else
                NavMeshBuilder.CollectSources(navMeshBounds, _navMeshSurface.layerMask,
                    _navMeshSurface.useGeometry, 0, new List<NavMeshBuildMarkup>(), _sources);
#endif
            }


            var data = NavMeshBuilder.BuildNavMeshData(navMeshBuildSettings, _sources, navMeshBounds,
                Vector3.zero, Quaternion.identity);
            _navMeshSurface.navMeshData = data;
            _navMeshSurface.AddData();
            InitializeNavMesh(navMeshBuildSettings.agentTypeID);
        }

        /// <summary>
        /// Collects navigation mesh sources from a list of rooms and their anchors, and optionally connects rooms in the navigation mesh.
        /// </summary>
        /// <param name="rooms">List of rooms to collect navigation mesh sources from.</param>
        /// <param name="sources">Collection to which the navigation mesh sources are added.</param>
        private void CollectSceneSources(List<MRUKRoom> rooms, ICollection<NavMeshBuildSource> sources)
        {
            var src = new NavMeshBuildSource();
            foreach (var room in rooms)
            {
                foreach (var anchor in room.Anchors)
                {
                    if (!anchor || !anchor.HasAnyLabel(NavigableSurfaces))
                    {
                        continue;
                    }

                    src.transform = anchor.transform.localToWorldMatrix;
                    src.sourceObject = Utilities.SetupAnchorMeshGeometry(anchor, true);
                    src.shape = NavMeshBuildSourceShape.Mesh;
                    sources.Add(src);
                }
            }

        }

        /// <summary>
        ///     Creates a new NavMeshBuildSettings object with the specified agent properties.
        /// </summary>
        /// <param name="agentRadius">The minimum distance to the walls where the navigation mesh can exist.</param>
        /// <param name="agentHeight">How much vertical clearance space must exist.</param>
        /// <param name="agentMaxSlope">Maximum slope the agent can walk up.</param>
        /// <param name="agentClimb">The height of discontinuities in the level the agent can climb over (i.e. steps and stairs).</param>
        /// <returns>A new NavMeshBuildSettings object with the specified agent properties.</returns>
        public NavMeshBuildSettings CreateNavMeshBuildSettings(float agentRadius, float agentHeight,
            float agentMaxSlope, float agentClimb)
        {
            var settings = NavMesh.CreateSettings();
            settings.agentRadius = agentRadius;
            settings.agentHeight = agentHeight;
            settings.agentSlope = agentMaxSlope;
            settings.agentClimb = agentClimb;
            settings.overrideVoxelSize = OverrideVoxelSize;
            if (OverrideVoxelSize)
            {
                settings.voxelSize = VoxelSize;
            }

            settings.overrideTileSize = OverrideTileSize;
            if (OverrideTileSize)
            {
                settings.tileSize = TileSize;
            }
            return settings;
        }

        /// <summary>
        ///     Creates a NavMeshSurface component and sets its properties.
        /// </summary>
        public void CreateNavMeshSurface()
        {
            _navMeshSurface = GetComponent<NavMeshSurface>();
            if (!_navMeshSurface)
            {
                _navMeshSurface = gameObject.AddComponent<NavMeshSurface>();
            }

            _navMeshSurface.minRegionArea = 0.01f;
            _navMeshSurface.voxelSize = VoxelSize;

            if (!UseSceneData)
            {
                _navMeshSurface.collectObjects = CollectObjects;
                _navMeshSurface.useGeometry = CollectGeometry;
                _navMeshSurface.hideFlags = HideFlags.None;
            }
            else
            {
                _navMeshSurface.collectObjects = CollectObjects.Children;
                _navMeshSurface.useGeometry = NavMeshCollectGeometry.RenderMeshes;
                // The NavMeshSurface component attached to the GO can be edited, but its properties will not affect the
                // generated NavMesh. Marking it as non-editable, use the configuration options in the SceneNavigation
                // component to modify tweak the NavMesh build options.
                _navMeshSurface.hideFlags = HideFlags.NotEditable;
            }
            _navMeshSurface.layerMask = Layers;
        }

        /// <summary>
        ///     Removes the NavMeshData from the NavMeshSurface component.
        /// </summary>
        public void RemoveNavMeshData()
        {
            if (!_navMeshSurface)
            {
                return;
            }

            _navMeshSurface.navMeshData = null;
            _navMeshSurface.RemoveData();

            if (Obstacles != null)
            {
                ClearObstacles();
            }
        }

        /// <summary>
        ///     Resizes the NavMeshSurface to fit the room bounds.
        /// </summary>
        /// <param name="surface">The NavMeshSurface to resize.</param>
        /// <param name="room">The room bounds to use. Default is the current room.</param>
        /// <returns>The bounds of the resized NavMeshSurface.</returns>
        public Bounds ResizeNavMeshFromRoomBounds(ref NavMeshSurface surface, MRUKRoom room = null)
        {
            var rooms = room != null
                ? new List<MRUKRoom>() { room }
                : new List<MRUKRoom>() { MRUK.Instance.GetCurrentRoom() };
            return ResizeNavMeshFromRoomBounds(ref surface, rooms);
        }

        /// <summary>
        ///     Resizes the NavMeshSurface to fit the room bounds.
        /// </summary>
        /// <param name="surface">The NavMeshSurface to resize.</param>
        /// <param name="rooms">The rooms bounds to use.</param>
        /// <returns>The bounds of the resized NavMeshSurface.</returns>
        public Bounds ResizeNavMeshFromRoomBounds(ref NavMeshSurface surface, List<MRUKRoom> rooms)
        {
            if (rooms.Count == 0)
            {
                throw new InvalidOperationException("No rooms available to resize the NavMeshSurface.");
            }

            // Initialize bounds with the first room's bounds
            var combinedBounds = rooms[0].GetRoomBounds();
            // Expand the combined bounds to include all other rooms
            for (var i = 1; i < rooms.Count; i++)
            {
                combinedBounds.Encapsulate(rooms[i].GetRoomBounds());
            }

            // Set the NavMeshSurface to the combined bounds
            var combinedCenter = new Vector3(combinedBounds.center.x, combinedBounds.center.y, combinedBounds.center.z);
            surface.center = combinedCenter;
            var combinedSize =
                new Vector3(combinedBounds.size.x, combinedBounds.size.y,
                    combinedBounds.size.z);
            surface.size = combinedSize * 1.1f;
            // Return the adjusted bounds
            return new Bounds(surface.center, surface.size);
        }

        /// <summary>
        ///     Initializes the navigation mesh with the given agent type ID.
        /// </summary>
        /// <param name="agentTypeID">The agent type ID to initialize the navigation mesh with.</param>
        private void InitializeNavMesh(int agentTypeID)
        {
            if (_navMeshSurface.navMeshData.sourceBounds.extents.x *
                _navMeshSurface.navMeshData.sourceBounds.extents.z >
                _minimumNavMeshSurfaceArea)
            {
                if (Agents != null)
                {
                    foreach (var agent in Agents)
                    {
                        agent.agentTypeID = agentTypeID;
                    }
                }

                OnNavMeshInitialized?.Invoke();
            }
            else
            {
                Debug.LogWarning("Failed to generate a nav mesh, this may be because the room is too small" +
                                 " or the AgentType settings are to strict");
            }
        }

        /// <summary>
        ///     Creates obstacles for the given MRUKRoom or all rooms if no room is specified.
        /// </summary>
        /// <param name="room">
        ///     Optional parameter for the MRUKRoom to create the obstacles for.
        ///     If not provided, obstacles will be created for all rooms.
        /// </param>
        public void CreateObstacles(MRUKRoom room = null)
        {
            var rooms = room == null
                ? new List<MRUKRoom>() { room }
                : new List<MRUKRoom>() { MRUK.Instance.GetCurrentRoom() };
            CreateObstacles(rooms);
        }

        /// <summary>
        ///     Creates obstacles for the given MRUKRoom or all rooms if no room is specified.
        /// </summary>
        /// <param name="rooms"> The rooms for which obstacles will be created.</param>
        public void CreateObstacles(List<MRUKRoom> rooms)
        {
            ObstacleRoot.transform.SetParent(transform);
            foreach (var _room in rooms)
            {
                var sceneAnchors = _room.Anchors;
                foreach (var anchor in sceneAnchors)
                {
                    CreateObstacle(anchor);
                }
            }
        }

        /// <summary>
        ///     Creates a NavMeshObstacle for the given MRUKAnchor.
        /// </summary>
        /// <param name="anchor">The MRUKAnchor to create the obstacle for.</param>
        /// <param name="shouldCarve">Optional parameter that determines whether the obstacle should carve the NavMesh. Default is true.</param>
        /// <param name="carveOnlyStationary">
        ///     Optional parameter that determines whether the obstacle should only carve
        ///     the NavMesh when stationary. Default is true.
        /// </param>
        /// <param name="carvingTimeToStationary">
        ///     Optional parameter that sets the time in seconds an obstacle must be
        ///     stationary before it starts carving the NavMesh. Default is 0.2f.
        /// </param>
        /// <param name="carvingMoveThreshold">
        ///     Optional parameter that sets the minimum world space distance the
        ///     obstacle must move before it is considered moving. Default is 0.2f.
        /// </param>
        public void CreateObstacle(MRUKAnchor anchor, bool shouldCarve = true, bool carveOnlyStationary = false,
            float carvingTimeToStationary = 0.2f, float carvingMoveThreshold = 0.2f)
        {
            Vector3 obstacleSize, obstacleCenter;
            if (!anchor || !anchor.HasAnyLabel(SceneObstacles))
            {
                return;
            }

            if (Obstacles.ContainsKey(anchor))
            {
                Debug.LogWarning("Anchor already associated with an obstacle from this SceneNavigation");
                return;
            }

            if (anchor.VolumeBounds != null)
            {
                obstacleSize = anchor.VolumeBounds.Value.size;
                obstacleCenter = anchor.VolumeBounds.Value.center;
            }
            else if (anchor.PlaneRect != null)
            {
                obstacleSize = anchor.PlaneRect.Value.size;
                obstacleCenter = anchor.PlaneRect.Value.center;
            }
            else
            {
                // the anchor cannot be an obstacle
                return;
            }

            InstantiateObstacle(anchor, shouldCarve, carveOnlyStationary, carvingTimeToStationary,
                carvingMoveThreshold,
                obstacleSize, obstacleCenter);
        }

        private void InstantiateObstacle(MRUKAnchor anchor, bool shouldCarve, bool carveOnlyStationary,
            float carvingTimeToStationary, float carvingMoveThreshold, Vector3 obstacleSize, Vector3 obstacleCenter)
        {
            var obstacleGO = new GameObject($"{_obstaclePrefix}_{anchor.name}");
            obstacleGO.transform.SetParent(_obstaclesRoot.transform);
            var obstacle = obstacleGO.AddComponent<NavMeshObstacle>();
            obstacle.carving = shouldCarve;
            obstacle.carveOnlyStationary = carveOnlyStationary;
            obstacle.carvingTimeToStationary = carvingTimeToStationary;
            obstacle.carvingMoveThreshold = carvingMoveThreshold;
            obstacle.shape = NavMeshObstacleShape.Box;
            obstacle.transform.position = anchor.transform.position;
            obstacle.transform.rotation = anchor.transform.rotation;
            obstacle.size = obstacleSize;
            obstacle.center = obstacleCenter;
            Obstacles.Add(anchor, obstacleGO);
        }

        /// <summary>
        ///     This method creates the bottom part of the tunnel that connects two rooms together.
        ///     See <see cref="EffectMesh.CreateRoomConnections"/> for the full implementation.
        ///     </summary>
        ///     <param name="connections">The connections between the rooms.</param>
        ///     <returns>A list containing all the meshes representing the bridges between rooms.</returns>
        private List<Mesh> CreateRoomBridges(List<(MRUKAnchor, MRUKAnchor)> connections)
        {
            _connectionMeshes.Clear();
            var vertices = new Vector3[4];
            var triangles = new int[] { 0, 2, 1, 1, 2, 3 }; // triangulation of the bottom part is fixed
            for (var i = 0; i < connections.Count; i++)
            {
                var anchor1 = connections[i].Item1;
                var anchor2 = connections[i].Item2;
                if (!anchor1.PlaneRect.HasValue || !anchor2.PlaneRect.HasValue)
                {
                    continue;
                }

                var anchor1Points = anchor1.PlaneBoundary2D;
                var anchor2Boundary = anchor2.PlaneBoundary2D;

                var newMesh = new Mesh();
                // Define the bottom part of the tunnel
                vertices[0] = anchor1.transform.TransformPoint(new Vector3(anchor1Points[0].x, anchor1Points[0].y, 0));
                vertices[1] =
                    anchor2.transform.TransformPoint(new Vector3(anchor2Boundary[1].x, anchor2Boundary[1].y, 0));
                vertices[2] = anchor1.transform.TransformPoint(new Vector3(anchor1Points[1].x, anchor1Points[1].y, 0));
                vertices[3] =
                    anchor2.transform.TransformPoint(new Vector3(anchor2Boundary[0].x, anchor2Boundary[0].y, 0));
                newMesh.vertices = vertices;
                newMesh.triangles = triangles;
                newMesh.RecalculateNormals();

                _connectionMeshes.Add(newMesh);
            }

            return _connectionMeshes;
        }

        /// </summary>
        /// <param name="room">
        ///     Optional parameter for the MRUKRoom to create the navigable surfaces for.
        ///     If not provided, navigable surfaces will be created for all rooms.
        /// </param>
        /// <remarks>
        ///     Creating surfaces will not automatically build a new NavMesh. When changing surfaces at run time,
        ///     always use <see cref="BuildSceneNavMesh" /> method
        /// </remarks>
        [Obsolete(
            "Navigable surfaces are now handled as NavMeshBuildSource, and are automatically created when building" +
            "the NavMesh using the scene data. Use EffectMesh to spawn colliders in the place of anchors.", true)]
        public void CreateNavigableSurfaces(MRUKRoom room = null)
        {
            var rooms = new List<MRUKRoom>();
            if (room)
            {
                rooms.Add(room);
            }
            else
            {
                rooms = MRUK.Instance.Rooms;
            }

            if (_surfacesRoot == null)
            {
                _surfacesRoot = new GameObject("_surface").transform;
            }

            _surfacesRoot.transform.SetParent(transform);
            foreach (var _room in rooms)
            {
                var sceneAnchors = _room.Anchors;
                foreach (var anchor in sceneAnchors)
                {
                    CreateNavigableSurface(anchor);
                }
            }

            _navMeshSurface.collectObjects = CollectObjects.Children;
        }

        /// <summary>
        ///     Creates a navigable surface for the given MRUKAnchor.
        /// </summary>
        /// <param name="anchor">The MRUKAnchor to create the navigable surface for.</param>
        private void CreateNavigableSurface(MRUKAnchor anchor)
        {
            Vector3 surfaceSize, surfaceCenter;
            if (!anchor || !anchor.HasAnyLabel(NavigableSurfaces))
            {
                return;
            }

            var surfaceGO = new GameObject($"{"_surface"}_{anchor.name}");
            surfaceGO.transform.SetParent(_surfacesRoot.transform);
            surfaceGO.gameObject.layer = GetFirstLayerFromLayerMask(Layers);
            if (!anchor || !anchor.HasAnyLabel(NavigableSurfaces))
            {
                return;
            }
#pragma warning disable CS0618 // Type or member is obsolete
            if (Surfaces.ContainsKey(anchor))
            {
                Debug.LogWarning("Anchor already associated with an obstacle from this SceneNavigation");
                return;
            }
#pragma warning restore CS0618

            if (anchor.VolumeBounds != null)
            {
                surfaceSize = anchor.VolumeBounds.Value.size;
                surfaceCenter = anchor.VolumeBounds.Value.center;
            }
            else if (anchor.PlaneRect != null)
            {
                surfaceSize = anchor.PlaneRect.Value.size;
                surfaceCenter = anchor.PlaneRect.Value.center;
            }
            else
            {
                // global mesh
                var meshCollider = surfaceGO.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = anchor.Mesh;
                meshCollider.transform.position = anchor.transform.position;
                meshCollider.transform.rotation = anchor.transform.rotation;
#pragma warning disable CS0618 // Type or member is obsolete
                Surfaces.Add(anchor, surfaceGO);
#pragma warning restore CS0618
                return;
            }

            var surfaceCollider = surfaceGO.AddComponent<BoxCollider>();
            surfaceCollider.transform.position = anchor.transform.position;
            surfaceCollider.transform.rotation = anchor.transform.rotation;
            surfaceCollider.size = surfaceSize;
            surfaceCollider.center = surfaceCenter;
#pragma warning disable CS0618 // Type or member is obsolete
            Surfaces.Add(anchor, surfaceGO);
#pragma warning restore CS0618
        }

        /// <summary>
        ///     Clears all obstacles from the Obstacles dictionary associated with the given MRUKRoom.
        ///     If no room is specified, all obstacles are cleared.
        /// </summary>
        /// <param name="room">
        ///     Optional parameter for the MRUKRoom to clear the obstacles for.
        ///     If not provided, all obstacles will be cleared.
        /// </param>
        public void ClearObstacles(MRUKRoom room = null)
        {
            List<MRUKAnchor> obstaclesToRemove = new();
            foreach (var kv in Obstacles)
            {
                if (room != null && kv.Key.Room != room)
                {
                    continue;
                }

                DestroyImmediate(kv.Value);
                obstaclesToRemove.Add(kv.Key);
            }

            foreach (var anchor in obstaclesToRemove)
            {
                Obstacles.Remove(anchor);
            }
        }

        /// <summary>
        ///     Clears the obstacle associated with the given MRUKAnchor from the Obstacles dictionary.
        /// </summary>
        /// <param name="anchor">The MRUKAnchor whose associated obstacle should be cleared.</param>
        public void ClearObstacle(MRUKAnchor anchor)
        {
            if (!Obstacles.TryGetValue(anchor, out var obstacle))
            {
                return;
            }

            DestroyImmediate(obstacle);
            Obstacles.Remove(anchor);
        }

#pragma warning disable CS0618 // Type or member is obsolete
        /// <summary>
        ///     Clears all surfaces from the Obstacles dictionary associated with the given MRUKRoom.
        ///     If no room is specified, all obstacles are cleared.
        /// </summary>
        /// <param name="room">
        ///     Optional parameter for the MRUKRoom to clear the surfaces for.
        ///     If not provided, all obstacles will be cleared.
        /// </param>
        /// <remarks>
        ///     Removing surfaces will not automatically build a new NavMesh. When changing surfaces at run time,
        ///     always use <see cref="BuildSceneNavMesh" /> method
        /// </remarks>
        private void ClearSurfaces(MRUKRoom room = null)
        {
            List<MRUKAnchor> surfacesToRemove = new();
            foreach (var kv in Surfaces)
            {
                if (room != null && kv.Key.Room != room)
                {
                    continue;
                }

                DestroyImmediate(kv.Value);
                surfacesToRemove.Add(kv.Key);
            }

            foreach (var anchor in surfacesToRemove)
            {
                Surfaces.Remove(anchor);
            }
        }

        /// <summary>
        ///     Clears the surface associated with the given MRUKAnchor from the Obstacles dictionary.
        /// </summary>
        /// <param name="anchor">The MRUKAnchor whose associated obstacle should be cleared.</param>
        [Obsolete(
            "Navigable surfaces are now handled as NavMeshBuildSource hence their destruction is handled internally.")]
        public void ClearSurface(MRUKAnchor anchor)
        {
            if (!Surfaces.ContainsKey(anchor))
            {
                return;
            }

            DestroyImmediate(Surfaces[anchor]);
            Surfaces.Remove(anchor);
        }
#pragma warning restore CS0618

        /// <summary>
        ///     Gets the first layer included in the given LayerMask.
        /// </summary>
        /// <param name="layerMask">The LayerMask to get the first layer from.</param>
        /// <returns>Returns the first layer included in the LayerMask.</returns>
        public static int GetFirstLayerFromLayerMask(LayerMask layerMask)
        {
            var layer = 0;
            for (var i = 0; i < 32; i++) // max number of layers in Unity
            {
                if (((1 << i) & layerMask) == 0)
                {
                    continue;
                }

                layer = i;
                break;
            }

            return layer;
        }

        /// <summary>
        ///     Validates the provided NavMeshBuildSettings against the provided NavMeshBounds.
        /// </summary>
        /// <param name="navMeshBuildSettings">The NavMeshBuildSettings to validate.</param>
        /// <param name="navMeshBounds">The Bounds to validate the NavMeshBuildSettings against.</param>
        /// <returns>Returns true if the NavMeshBuildSettings are valid, false otherwise.</returns>
        public static bool ValidateBuildSettings(NavMeshBuildSettings navMeshBuildSettings, Bounds navMeshBounds)
        {
            var report = navMeshBuildSettings.ValidationReport(navMeshBounds);
            if (report.Length <= 0)
            {
                return true;
            }

            var warning = "Some NavMeshBuildSettings constraints were violated:\n";
            foreach (var violation in report)
            {
                warning += "- " + violation + "\n";
            }

            Debug.LogWarning(warning);
            return false;
        }


        private void OnDestroy()
        {
            OnNavMeshInitialized.RemoveAllListeners();
            if (!MRUK.Instance)
            {
                return;
            }

            MRUK.Instance.RoomCreatedEvent.RemoveListener(ReceiveCreatedRoom);
            MRUK.Instance.RoomRemovedEvent.RemoveListener(ReceiveRemovedRoom);
            MRUK.Instance.RoomUpdatedEvent.RemoveListener(ReceiveUpdatedRoom);
            MRUK.Instance.SceneLoadedEvent.RemoveListener(OnSceneLoadedEvent);
        }
    }
}
