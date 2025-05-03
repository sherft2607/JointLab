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
using System.IO;
using Meta.XR.Util;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

namespace Meta.XR.MRUtilityKit
{
    // @cond
    /// <summary>
    /// Provides debugging tools for visualizing and interacting with the scene data.
    /// </summary>
    /// <remarks>
    /// The private methods and properties of this class are accessed through reflection by the Immersive Debugger.
    /// </remarks>
    [Feature(Feature.Scene)]
    public class ImmersiveSceneDebugger : MonoBehaviour
    {
        [Tooltip("Visualize anchors")] public bool ShowDebugAnchors;

        [SerializeField] private Material visualHelperMaterial;
        [SerializeField] private Shader _debugShader;

        private readonly int _srcBlend = Shader.PropertyToID("_SrcBlend");
        private readonly int _dstBlend = Shader.PropertyToID("_DstBlend");
        private readonly int _zWrite = Shader.PropertyToID("_ZWrite");
        private readonly int _cull = Shader.PropertyToID("_Cull");
        private readonly int _color = Shader.PropertyToID("_Color");
        private readonly List<GameObject> _debugAnchors = new();
        private GameObject _globalMeshGO;
        private OVRCameraRig _cameraRig;
        private MRUKRoom _currentRoom;

        private bool _roomHasChanged
        {
            get
            {
                if (_currentRoom == MRUK.Instance.GetCurrentRoom())
                {
                    return false;
                }

                _currentRoom = MRUK.Instance.GetCurrentRoom();
                _globalMeshAnchor = _currentRoom.GlobalMeshAnchor;
                return true;
            }
        }

        // For visual debugging of the room
        private GameObject _debugCube;
        private GameObject _debugSphere;
        private GameObject _debugNormal;
        private GameObject _navMeshViz;
        private GameObject _debugAnchor;
        private bool _previousShowDebugAnchors;
        private Mesh _debugCheckerMesh;
        private MRUKAnchor _previousShownDebugAnchor;
        private MRUKAnchor _globalMeshAnchor;

        private NavMeshTriangulation _navMeshTriangulation;

        private SpaceMapGPU _spaceMapGPU;
        private MeshCollider _globalMeshCollider;
        private Material _navMeshMaterial;
        private string _debugMessage = "";
        private string _currentDebugMessage = "";
        private string _sceneDetails = "";

        private Material _debugMaterial;
        private Material _checkerMeshMaterial;


        private DebugAction _isPositionInRoom;
        private DebugAction _showDebugAnchorsDebugAction;
        private DebugAction _raycastDebugger;
        private MRUK.PositioningMethod _positioningMethod = MRUK.PositioningMethod.CENTER;
        private DebugAction _getBestPoseFromRaycastDebugger;
        private DebugAction _getKeyWallDebugger;
        private MRUKAnchor.SceneLabels _largestSurfaceFilter = MRUKAnchor.SceneLabels.TABLE;
        private DebugAction _getLargestSurfaceDebugger;
        private DebugAction _getClosestSeatPoseDebugger;
        private DebugAction _getClosestSurfacePositionDebugger;
        private bool exportGlobalMeshJSON = true;

        private DebugAction? _currentDebugAction;

        private bool _shouldDisplayGlobalMesh;

        private bool ShouldDisplayGlobalMesh
        {
            get => _shouldDisplayGlobalMesh;
            set
            {
                _shouldDisplayGlobalMesh = value;
                DisplayGlobalMesh(value);
            }
        }

        private bool _shouldToggleGlobalMeshCollision;

        private bool ShouldToggleGlobalMeshCollision
        {
            get => _shouldToggleGlobalMeshCollision;
            set
            {
                _shouldToggleGlobalMeshCollision = value;
                ToggleGlobalMeshCollisions(value);
            }
        }

        private bool _shouldDisplayNavMesh;

        private bool ShouldDisplayNavMesh
        {
            get => _shouldDisplayNavMesh;
            set
            {
                _shouldDisplayNavMesh = value;
                DisplayNavMesh(value);
            }
        }

        /// <summary>
        /// Gets the singleton instance of the class.
        /// </summary>
        internal static ImmersiveSceneDebugger Instance { get; private set; }

        private readonly struct DebugAction
        {
            private readonly Action _setup;
            private readonly Action _cleanup;
            private readonly Action _execute;

            public DebugAction(Action setup, Action execute, Action cleanup)
            {
                _setup = setup;
                _execute = execute;
                _cleanup = cleanup;
            }

            public void Setup()
            {
                _setup?.Invoke();
            }

            public void Cleanup()
            {
                _cleanup?.Invoke();
            }

            public void Execute()
            {
                _execute?.Invoke();
            }
            public bool Equals(DebugAction other)
            {
                return _setup == other._setup &&
                       _execute == other._execute &&
                       _cleanup == other._cleanup;
            }
            public override bool Equals(object obj)
            {
                return obj is DebugAction other && Equals(other);
            }
            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 23 + (_setup?.GetHashCode() ?? 0);
                    hash = hash * 23 + (_execute?.GetHashCode() ?? 0);
                    hash = hash * 23 + (_cleanup?.GetHashCode() ?? 0);
                    return hash;
                }
            }
            public static bool operator ==(DebugAction left, DebugAction right)
            {
                return left.Equals(right);
            }
            public static bool operator !=(DebugAction left, DebugAction right)
            {
                return !left.Equals(right);
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.Assert(false, $"There should be only one instance of {nameof(ImmersiveSceneDebugger)}!");
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            _cameraRig = FindAnyObjectByType<OVRCameraRig>();
            _isPositionInRoom = IsPositionInRoomDebugger();
            _showDebugAnchorsDebugAction = ShowDebugAnchorsDebugger();
            _raycastDebugger = RayCastDebugger();
            _getBestPoseFromRaycastDebugger = GetBestPoseFromRaycastDebugger();
            _getKeyWallDebugger = GetKeyWallDebugger();
            _getLargestSurfaceDebugger = GetLargestSurfaceDebugger();
            _getClosestSeatPoseDebugger = GetClosestSeatPoseDebugger();
            _getClosestSurfacePositionDebugger = GetClosestSurfacePositionDebugger();
        }

        private void Start()
        {
            MRUK.Instance?.RegisterSceneLoadedCallback(OnSceneLoaded);
            OVRTelemetry.Start(TelemetryConstants.MarkerId.LoadSceneDebugger).Send();
            _currentRoom = MRUK.Instance?.GetCurrentRoom();
            _sceneDetails = ShowRoomDetails();
            _debugMaterial = new Material(_debugShader)
            {
                color = Color.green
            };
            _navMeshMaterial = new Material(_debugShader)
            {
                color = Color.cyan
            };
            SetupCheckerMeshMaterial(_debugShader);
            CreateDebugPrimitives();
        }


        private void Update()
        {
            _currentDebugAction?.Execute();
            if (!_currentDebugMessage.Equals(_debugMessage))
            {
                _currentDebugMessage = _debugMessage;
                Debug.Log(_currentDebugMessage);
            }

            // Toggle the anchors debug visuals
            if (ShowDebugAnchors != _previousShowDebugAnchors)
            {
                if (ShowDebugAnchors)
                {
                    foreach (var room in MRUK.Instance.Rooms)
                    {
                        foreach (var anchor in room.Anchors)
                        {
                            var anchorVisual = GenerateDebugAnchor(anchor);
                            _debugAnchors.Add(anchorVisual);
                        }
                    }
                }
                else
                {
                    foreach (var anchorVisual in _debugAnchors)
                    {
                        Destroy(anchorVisual.gameObject);
                    }
                }

                _previousShowDebugAnchors = ShowDebugAnchors;
            }
        }

        private void OnDisable()
        {
            _currentDebugAction = null;
        }

        public void OnDestroy()
        {
            MRUK.Instance?.SceneLoadedEvent.RemoveListener(OnSceneLoaded);
        }

        private void OnSceneLoaded()
        {
            CreateDebugPrimitives();
            if (MRUK.Instance && MRUK.Instance.GetCurrentRoom() && !_globalMeshAnchor)
            {
                _globalMeshAnchor = MRUK.Instance.GetCurrentRoom().GlobalMeshAnchor;
            }
        }

        private void IsPositionInRoom()
        {
            SetDebugAction(_isPositionInRoom);
        }

        private void DisplayDebugAnchors()
        {
            SetDebugAction(_showDebugAnchorsDebugAction);
        }

        private void Raycast()
        {
            SetDebugAction(_raycastDebugger);
        }

        private void GetBestPoseFromRayCast()
        {
            SetDebugAction(_getBestPoseFromRaycastDebugger);
        }

        private void GetKeyWall()
        {
            SetDebugAction(_getKeyWallDebugger);
        }

        private void GetLargestSurface()
        {
            SetDebugAction(_getLargestSurfaceDebugger);
        }

        private void GetClosestSeatPose()
        {
            SetDebugAction(_getClosestSeatPoseDebugger);
        }

        private void GetClosestSurfacePosition()
        {
            SetDebugAction(_getClosestSurfacePositionDebugger);
        }

        private void SetDebugAction(DebugAction newDebugAction)
        {
            // Clean up the current debug action
            _currentDebugAction?.Cleanup();
            if (_currentDebugAction == newDebugAction)
            {
                _currentDebugAction = null;
                return;
            }
            // Set the new debug action
            _currentDebugAction = newDebugAction;
            // Setup and execute the new debug action immediately
            _currentDebugAction?.Setup();
        }

        private Ray GetControllerRay()
        {
            Vector3 rayOrigin;
            Vector3 rayDirection;
            if (OVRInput.activeControllerType == OVRInput.Controller.Touch
                || OVRInput.activeControllerType == OVRInput.Controller.RTouch)
            {
                rayOrigin = _cameraRig.rightHandOnControllerAnchor.position;
                rayDirection = _cameraRig.rightHandOnControllerAnchor.forward;
            }
            else if (OVRInput.activeControllerType == OVRInput.Controller.LTouch)
            {
                rayOrigin = _cameraRig.leftHandOnControllerAnchor.position;
                rayDirection = _cameraRig.leftHandOnControllerAnchor.forward;
            }
            else // hands
            {
                var rightHand = _cameraRig.rightHandAnchor.GetComponentInChildren<OVRHand>();
                // can be null if running in Editor with Meta Linq app and the headset is put off
                if (rightHand != null)
                {
                    rayOrigin = rightHand.PointerPose.position;
                    rayDirection = rightHand.PointerPose.forward;
                }
                else
                {
                    rayOrigin = _cameraRig.centerEyeAnchor.position;
                    rayDirection = _cameraRig.centerEyeAnchor.forward;
                }
            }

            return new Ray(rayOrigin, rayDirection);
        }

        /// <summary>
        /// Highlights the room's key wall, defined as the longest wall in the room which has no other room points behind it
        /// </summary>
        private DebugAction GetKeyWallDebugger()
        {
            return new DebugAction(
                () =>
                {
                    _debugCube.SetActive(true);
                    var wallScale = Vector2.zero;
                    var keyWall = MRUK.Instance?.GetCurrentRoom()?.GetKeyWall(out wallScale);
                    if (keyWall != null && _debugCube != null)
                    {
                        _debugCube.transform.localScale = new Vector3(wallScale.x, wallScale.y, 0.05f);
                        _debugCube.transform.position = keyWall.transform.position;
                        _debugCube.transform.rotation = keyWall.transform.rotation;
                    }

                    _debugMessage = $"[{nameof(GetKeyWallDebugger)}] Size: {wallScale}";
                },
                () => { },
                () => { _debugCube.SetActive(false); }
            );
        }

        /// <summary>
        ///  Highlights the anchor with the largest available surface area.
        /// </summary>
        private DebugAction GetLargestSurfaceDebugger()
        {
            return new DebugAction(
                () => { _debugCube.SetActive(true); },
                () =>
                {
                    var largestSurface = MRUK.Instance?.GetCurrentRoom()?.FindLargestSurface(_largestSurfaceFilter);
                    if (largestSurface != null)
                    {
                        if (_debugCube != null)
                        {
                            var anchorSize = largestSurface.PlaneRect.HasValue ? new Vector3(largestSurface.PlaneRect.Value.width,
                                largestSurface.PlaneRect.Value.height, 0.01f) : largestSurface.VolumeBounds.Value.size;
                            _debugCube.transform.localScale = anchorSize + new Vector3(0.01f, 0.01f, 0.01f);// avoid z-fighting
                            _debugCube.transform.position = largestSurface.PlaneRect.HasValue ?
                                largestSurface.transform.position : largestSurface.transform.TransformPoint(largestSurface.VolumeBounds.Value.center);
                            _debugCube.transform.rotation = largestSurface.transform.rotation;

                        }

                        _debugMessage =
                            $"[{nameof(GetLargestSurface)}] Anchor: {largestSurface.name} Type: {largestSurface.Label}";
                    }
                    else
                    {
                        _debugMessage =
                            $"[{nameof(GetLargestSurface)}] Cannot get surface area for this label in this scene.";
                        _debugCube.SetActive(false);
                    }
                },
                () => { _debugCube.SetActive(false); }
            );
        }

        /// <summary>
        /// Highlights the best-suggested seat, for something like remote caller placement.
        /// </summary>
        private DebugAction GetClosestSeatPoseDebugger()
        {
            return new DebugAction(
                () =>
                {
                    MRUKAnchor seat = null;
                    var seatPose = new Pose();
                    var ray = GetControllerRay();
                    MRUK.Instance?.GetCurrentRoom()?.TryGetClosestSeatPose(ray, out seatPose, out seat);
                    if (seat)
                    {
                        _debugCube.SetActive(true);
                        if (_debugCube != null)
                        {
                            _debugCube.transform.localRotation = seat.transform.localRotation;
                            _debugCube.transform.position = seatPose.position;
                            _debugCube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        }
                        _debugMessage = $"[{nameof(GetClosestSeatPoseDebugger)}] Seat: {seat.name} Position: {seatPose.position}" +
                                      $"Distance: {Vector3.Distance(seatPose.position, ray.origin).ToString("0.##")}";
                    }
                    else
                    {
                        _debugMessage =
                            $"[{nameof(GetClosestSeatPoseDebugger)}]  No seat found in the scene.";
                    }
                },
                () => { },
                () => { _debugCube.SetActive(false); }
            );
        }

        /// <summary>
        /// Highlights the closest position on a SceneAPI surface.
        /// </summary>
        private DebugAction GetClosestSurfacePositionDebugger()
        {
            return new DebugAction(
                () => { _debugNormal.SetActive(true); },
                () =>
                {
                    var origin = GetControllerRay().origin;
                    var surfacePosition = Vector3.zero;
                    var normal = Vector3.up;
                    MRUKAnchor closestAnchor = null;
                    MRUK.Instance?.GetCurrentRoom()
                        ?.TryGetClosestSurfacePosition(origin, out surfacePosition, out closestAnchor, out normal);
                    ShowHitNormal(surfacePosition, normal);

                    if (closestAnchor != null)
                    {
                        _debugMessage =
                        string.Format("[{0}] Anchor: {1} Surface Position: {2} Distance: {3}",
                            nameof(GetClosestSurfacePosition),
                            closestAnchor.name,
                            surfacePosition,
                            Vector3.Distance(origin, surfacePosition).ToString("0.##")
                        );
                    }
                },
                () => { _debugNormal.SetActive(false); }
            );
        }

        /// <summary>
        /// Highlights the best suggested transform to place a widget on a surface.
        /// </summary>
        private DebugAction GetBestPoseFromRaycastDebugger()
        {
            return new DebugAction(
                () => { _debugCube.SetActive(true); },
                () =>
                {
                    var ray = GetControllerRay();
                    MRUKAnchor sceneAnchor = null;

                    var bestPose = MRUK.Instance?.GetCurrentRoom()?.GetBestPoseFromRaycast(ray, Mathf.Infinity,
                        new LabelFilter(), out sceneAnchor, _positioningMethod);
                    if (bestPose.HasValue && sceneAnchor && _debugCube)
                    {
                        _debugCube.transform.position = bestPose.Value.position;
                        _debugCube.transform.rotation = bestPose.Value.rotation;
                        _debugCube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        _debugMessage =
                            string.Format("[{0}] Anchor: {1} Pose Position: {2} Pose Rotation: {3}",
                                nameof(GetBestPoseFromRayCast),
                                sceneAnchor.name,
                                bestPose.Value.position,
                                bestPose.Value.rotation
                            );
                    }
                },
                () => { _debugCube.SetActive(false); }
            );
        }

        /// <summary>
        /// Casts a ray cast forward from the right controller position and draws the normal of the first Scene API object hit.
        /// </summary>
        private DebugAction RayCastDebugger()
        {
            return new DebugAction(
                () => { _debugNormal.SetActive(true); },
                () =>
                {
                    var ray = GetControllerRay();
                    var hit = new RaycastHit();
                    MRUKAnchor anchorHit = null;
                    MRUK.Instance?.GetCurrentRoom()?.Raycast(ray, Mathf.Infinity, out hit, out anchorHit);
                    ShowHitNormal(hit.point, hit.normal);
                    if (anchorHit != null)
                    {
                        _debugMessage =
                            string.Format("[{0}] Anchor: {1} Hit point: {2} Hit normal: {3}",
                                nameof(Raycast),
                                anchorHit.name,
                                hit.point,
                                hit.normal
                            );
                    }
                },
                () => { _debugNormal.SetActive(false); }
            );
        }

        /// <summary>
        /// Moves the debug sphere to the controller position and colors it in green if its position is in the room,
        /// red otherwise.
        /// </summary>
        private DebugAction IsPositionInRoomDebugger()
        {
            return new DebugAction(
                null,
                () =>
                {
                    var ray = GetControllerRay();
                    if (_debugSphere != null)
                    {
                        _debugSphere.SetActive(true);
                        var isInRoom = MRUK.Instance?.GetCurrentRoom()
                            ?.IsPositionInRoom(_debugSphere.transform.position);
                        _debugSphere.transform.position = ray.GetPoint(0.2f); // add some offset
                        _debugSphere.GetComponent<Renderer>().material.color =
                            isInRoom.HasValue && isInRoom.Value ? Color.green : Color.red;
                        _debugMessage =
                            $"[{nameof(IsPositionInRoom)}] Position: {_debugSphere.transform.position} " +
                            $"Is inside the Room: {isInRoom}";
                    }
                },
                () => { _debugSphere.SetActive(false); }
            );
        }

        /// <summary>
        /// Shows the debug anchor visualization mode for the anchor being pointed at.
        /// </summary>
        private DebugAction ShowDebugAnchorsDebugger()
        {
            return new DebugAction(
                null,
                () =>
                {
                    var ray = GetControllerRay();
                    var hit = new RaycastHit();
                    MRUKAnchor anchorHit = null;
                    MRUK.Instance?.GetCurrentRoom()?.Raycast(ray, Mathf.Infinity, out hit, out anchorHit);
                    if (_previousShownDebugAnchor != anchorHit && anchorHit != null)
                    {
                        Destroy(_debugAnchor);
                        _debugAnchor = GenerateDebugAnchor(anchorHit);
                        _previousShownDebugAnchor = anchorHit;
                    }

                    ShowHitNormal(hit.point, hit.normal);
                    _debugMessage =
                        $"[{nameof(ShowDebugAnchorsDebugger)}] Hit point: {hit.point} Hit normal: {hit.normal}";
                },
                () =>
                {
                    Destroy(_debugAnchor);
                    _debugAnchor = null;

                    if (_debugNormal != null)
                    {
                        _debugNormal.SetActive(false);
                    }
                }
            );
        }

        /// <summary>
        /// Displays the global mesh anchor if one is found in the scene.
        /// </summary>
        /// <param name="isOn">If set to true, the global mesh will be displayed.</param>
        public void DisplayGlobalMesh(bool isOn)
        {
            if (!_globalMeshAnchor)
            {
                Debug.Log($"[{nameof(DisplayGlobalMesh)}] No global mesh anchor found in the scene.");
                return;
            }

            if (isOn)
            {
                if (_roomHasChanged || !_globalMeshGO)
                {
                    if (_globalMeshGO)
                    {
                        DestroyImmediate(_globalMeshGO);
                    }

                    InstantiateGlobalMesh((globalMeshSegmentGO, mesh) =>
                    {
                        var meshRenderer = globalMeshSegmentGO.AddComponent<MeshRenderer>();
                        meshRenderer.material = visualHelperMaterial;
                    });
                }
                else
                {
                    _globalMeshGO.GetComponent<MeshRenderer>().enabled = true;
                }
            }
            else
            {
                if (_globalMeshGO)
                {
                    _globalMeshGO.GetComponent<MeshRenderer>().enabled = false;
                }
            }
        }

        /// <summary>
        /// Toggles the global mesh anchor's collision.
        /// </summary>
        /// <param name="isOn">If set to true, collisions for the global mesh anchor will be enabled </param>
        public void ToggleGlobalMeshCollisions(bool isOn)
        {
            if (!_globalMeshAnchor)
            {
                Debug.Log($"[{nameof(ToggleGlobalMeshCollisions)}] No global mesh anchor found in the scene.");
                return;
            }

            if (isOn)
            {
                if (_roomHasChanged || !_globalMeshCollider)
                {
                    if (_globalMeshCollider)
                    {
                        DestroyImmediate(_globalMeshCollider);
                    }

                    var globalMeshColliderGO =
                        new GameObject($"_globalMeshCollider");
                    globalMeshColliderGO.transform.SetParent(_globalMeshAnchor.transform, false);
                    _globalMeshCollider = globalMeshColliderGO.AddComponent<MeshCollider>();
                    _globalMeshCollider.sharedMesh = _globalMeshAnchor.GlobalMesh;
                }
                else
                {
                    _globalMeshCollider.enabled = true;
                }
            }
            else
            {
                if (_globalMeshCollider)
                {
                    _globalMeshCollider.enabled = false;
                }
            }
        }

        private void InstantiateGlobalMesh(Action<GameObject, Mesh> onMeshSegmentInstantiated)
        {
            var processedMesh = Utilities.AddBarycentricCoordinatesToMesh(_globalMeshAnchor.Mesh);
            _globalMeshGO = new GameObject($"_globalMeshViz");
            _globalMeshGO.transform.SetParent(MRUK.Instance.GetCurrentRoom().GlobalMeshAnchor.transform,
                false);
            var meshFilter = _globalMeshGO.AddComponent<MeshFilter>();
            meshFilter.mesh = processedMesh;
            onMeshSegmentInstantiated?.Invoke(_globalMeshGO, processedMesh);
        }

        /// <summary>
        /// Exports the current scene data to a JSON file if the specified condition is met.
        /// </summary>
        public void ExportJSON()
        {
            var scene = SerializationHelpers.Serialize(SerializationHelpers.CoordinateSystem.Unity,
                exportGlobalMeshJSON
            );
            var filename = $"MRUK_Export_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.json";
            var path = Path.Combine(Application.persistentDataPath, filename);
            File.WriteAllText(path, scene);
        }

        /// <summary>
        /// Displays the nav mesh, if present.
        /// </summary>
        /// <param name="isOn">If set to true, the navigation mesh will be displayed, if present</param>
        public void DisplayNavMesh(bool isOn)
        {
            if (isOn)
            {
                var triangulation = NavMesh.CalculateTriangulation();
                if (triangulation.areas.Length == 0 && _navMeshTriangulation.Equals(triangulation))
                {
                    return;
                }

                MeshRenderer navMeshRenderer;
                MeshFilter navMeshFilter;
                if (!_navMeshViz)
                {
                    _navMeshViz = new GameObject("_navMeshViz");
                    navMeshRenderer = _navMeshViz.AddComponent<MeshRenderer>();
                    navMeshFilter = _navMeshViz.AddComponent<MeshFilter>();
                }
                else
                {
                    navMeshRenderer = _navMeshViz.GetComponent<MeshRenderer>();
                    navMeshFilter = _navMeshViz.GetComponent<MeshFilter>();
                    DestroyImmediate(navMeshFilter.mesh);
                    navMeshFilter.mesh = null;
                }

                var navMesh = new Mesh()
                {
                    indexFormat = triangulation.indices.Length > ushort.MaxValue
                        ? IndexFormat.UInt32
                        : IndexFormat.UInt16
                };

                navMesh.SetVertices(triangulation.vertices);
                navMesh.SetIndices(triangulation.indices, MeshTopology.Triangles, 0);
                navMeshRenderer.material = _navMeshMaterial;
                navMeshFilter.mesh = navMesh;
                _navMeshTriangulation = triangulation;
            }
            else
            {
                DestroyImmediate(_navMeshViz);
            }
        }

        private string ShowRoomDetails()
        {
            var currentRoomName = "N/A";
            var numAnchors = 0;
            var numRooms = 0;
            if (MRUK.Instance)
            {
                numRooms = MRUK.Instance.Rooms != null ? MRUK.Instance.Rooms.Count : 0;
                if (MRUK.Instance.GetCurrentRoom())
                {
                    currentRoomName = MRUK.Instance.GetCurrentRoom() != null
                        ? MRUK.Instance.GetCurrentRoom().name
                        : "N/A";
                    numAnchors = MRUK.Instance.GetCurrentRoom().Anchors != null
                        ? MRUK.Instance.GetCurrentRoom().Anchors.Count
                        : 0;
                }
            }

            return
                $"Room Details: Number of rooms: {numRooms}; Current room: {currentRoomName}; Number room anchors:{numAnchors}";
        }


        /// <summary>
        ///     Creates an object to help visually debugging a specific anchor.
        /// </summary>
        private GameObject GenerateDebugAnchor(MRUKAnchor anchor)
        {
            Vector3 anchorScale;
            var anchorPosition = anchor.transform.position;
            var anchorRotation = anchor.transform.rotation;
            if (anchor.VolumeBounds.HasValue)
            {
                // Handle volume anchors
                CreateDebugPrefabSource(anchor);
                var volumeBounds = anchor.VolumeBounds.Value;
                anchorScale = volumeBounds.size;
                anchorPosition += anchorRotation * volumeBounds.center;
            }
            else
            {
                // Handle plane anchors
                CreateDebugPrefabSource(anchor);
                anchorScale = Vector3.zero;
                if (anchor.PlaneRect != null)
                {
                    var quadSize = anchor.PlaneRect.Value.size;
                    anchorScale = new Vector3(quadSize.x, quadSize.y, 1.0f);
                }
            }

            _debugAnchor.transform.position = anchorPosition;
            _debugAnchor.transform.rotation = anchorRotation;
            ScaleChildren(_debugAnchor.transform, anchorScale);
            _debugAnchor.transform.parent = null;
            _debugAnchor.SetActive(true);
            return _debugAnchor;
        }

        private void ScaleChildren(Transform parent, Vector3 localScale)
        {
            foreach (Transform child in parent)
            {
                child.localScale = localScale;
            }
        }

        /// <summary>
        ///     By creating our reference PLANE and VOLUME prefabs in code, we can avoid linking them via Inspector.
        /// </summary>
        private void CreateDebugPrefabSource(MRUKAnchor anchor)
        {
            var isPlane = !anchor.VolumeBounds.HasValue;
            var prefabName = isPlane ? "PlanePrefab" : "VolumePrefab";
            _debugAnchor = new GameObject(prefabName);
            var meshParent = new GameObject("MeshParent");
            meshParent.transform.SetParent(_debugAnchor.transform);
            meshParent.SetActive(false);
            var prefabPivot = new GameObject("Pivot");
            prefabPivot.transform.SetParent(_debugAnchor.transform);
            if (anchor.VolumeBounds.HasValue)
            {
                CreateGridPattern(prefabPivot.transform, new Vector3(0, 0, 0.5f), Quaternion.identity);
                CreateGridPattern(prefabPivot.transform, new Vector3(0, 0, -0.5f), Quaternion.Euler(180, 0, 0));
                CreateGridPattern(prefabPivot.transform, new Vector3(0, 0.5f, 0), Quaternion.Euler(-90, 0, 0));
                CreateGridPattern(prefabPivot.transform, new Vector3(0, -0.5f, 0), Quaternion.Euler(90, 0, 0));
                CreateGridPattern(prefabPivot.transform, new Vector3(-0.5f, 0, 0), Quaternion.Euler(0, -90, 90));
                CreateGridPattern(prefabPivot.transform, new Vector3(0.5f, 0, 0), Quaternion.Euler(0, 90, 90));
            }
            else
            {
                CreateGridPattern(prefabPivot.transform, Vector3.zero, Quaternion.identity);
            }
        }

        // The grid pattern on each anchor is actually a mesh, to avoid a texture
        private void CreateGridPattern(Transform parentTransform, Vector3 localOffset, Quaternion localRotation)
        {
            var newGameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
            newGameObject.name = "Checker";
            newGameObject.transform.SetParent(parentTransform, false);
            newGameObject.transform.localPosition = localOffset;
            newGameObject.transform.localRotation = localRotation;
            newGameObject.transform.localScale = Vector3.one;
            DestroyImmediate(newGameObject.GetComponent<Collider>());
            const float NORMAL_OFFSET = 0.001f;
            if (_debugCheckerMesh == null)
            {
                _debugCheckerMesh = new Mesh();
                const int gridWidth = 10;
                var cellWidth = 1.0f / gridWidth;
                var xPos = -0.5f;
                var yPos = -0.5f;
                var totalTiles = gridWidth * gridWidth / 2;
                var totalVertices = totalTiles * 4;
                var totalIndices = totalTiles * 6;
                var MeshVertices = new Vector3[totalVertices];
                var MeshUVs = new Vector2[totalVertices];
                var MeshColors = new Color32[totalVertices];
                var MeshNormals = new Vector3[totalVertices];
                var MeshTangents = new Vector4[totalVertices];
                var MeshTriangles = new int[totalIndices];
                var vertCounter = 0;
                var indexCounter = 0;
                var quadCounter = 0;
                for (var x = 0; x < gridWidth; x++)
                {
                    var createQuad = x % 2 == 0;
                    for (var y = 0; y < gridWidth; y++)
                    {
                        if (createQuad)
                        {
                            for (var V = 0; V < 4; V++)
                            {
                                var localVertPos = new Vector3(xPos + x * cellWidth, yPos + y * cellWidth,
                                    NORMAL_OFFSET);
                                switch (V)
                                {
                                    case 1:
                                        localVertPos += new Vector3(0, cellWidth, 0);
                                        break;
                                    case 2:
                                        localVertPos += new Vector3(cellWidth, cellWidth, 0);
                                        break;
                                    case 3:
                                        localVertPos += new Vector3(cellWidth, 0, 0);
                                        break;
                                }

                                MeshVertices[vertCounter] = localVertPos;
                                MeshUVs[vertCounter] = Vector2.zero;
                                MeshColors[vertCounter] = Color.black;
                                MeshNormals[vertCounter] = Vector3.forward;
                                MeshTangents[vertCounter] = Vector3.right;
                                vertCounter++;
                            }

                            var baseCount = quadCounter * 4;
                            MeshTriangles[indexCounter++] = baseCount;
                            MeshTriangles[indexCounter++] = baseCount + 2;
                            MeshTriangles[indexCounter++] = baseCount + 1;
                            MeshTriangles[indexCounter++] = baseCount;
                            MeshTriangles[indexCounter++] = baseCount + 3;
                            MeshTriangles[indexCounter++] = baseCount + 2;
                            quadCounter++;
                        }

                        createQuad = !createQuad;
                    }
                }

                _debugCheckerMesh.Clear();
                _debugCheckerMesh.name = "CheckerMesh";
                _debugCheckerMesh.vertices = MeshVertices;
                _debugCheckerMesh.uv = MeshUVs;
                _debugCheckerMesh.colors32 = MeshColors;
                _debugCheckerMesh.triangles = MeshTriangles;
                _debugCheckerMesh.normals = MeshNormals;
                _debugCheckerMesh.tangents = MeshTangents;
                _debugCheckerMesh.RecalculateNormals();
                _debugCheckerMesh.RecalculateTangents();
            }

            newGameObject.GetComponent<MeshFilter>().mesh = _debugCheckerMesh;
            newGameObject.GetComponent<MeshRenderer>().material = _checkerMeshMaterial;
        }

        private void SetupCheckerMeshMaterial(Shader debugShader)
        {
            _checkerMeshMaterial = new Material(debugShader);
            _checkerMeshMaterial.SetOverrideTag("RenderType", "Transparent");
            _checkerMeshMaterial.SetInt(_srcBlend, (int)BlendMode.SrcAlpha);
            _checkerMeshMaterial.SetInt(_dstBlend, (int)BlendMode.One);
            _checkerMeshMaterial.SetInt(_zWrite, 0);
            _checkerMeshMaterial.SetInt(_cull, (int)CullMode.Back);
            _checkerMeshMaterial.DisableKeyword("_ALPHATEST_ON");
            _checkerMeshMaterial.EnableKeyword("_ALPHABLEND_ON");
            _checkerMeshMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            _checkerMeshMaterial.renderQueue = (int)RenderQueue.Transparent;
        }

        /// <summary>
        ///     Creates the debug primitives for visual debugging purposes and to avoid inspector linking.
        /// </summary>
        private void CreateDebugPrimitives()
        {
            _debugCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _debugCube.name = "SceneDebugger_Cube";
            _debugCube.GetComponent<Renderer>().material = _debugMaterial;
            _debugCube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            _debugCube.GetComponent<Collider>().enabled = false;
            _debugCube.SetActive(false);

            _debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _debugSphere.name = "SceneDebugger_Sphere";
            _debugSphere.GetComponent<Renderer>().material = _debugMaterial;
            _debugSphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            _debugSphere.GetComponent<Collider>().enabled = false;
            _debugSphere.SetActive(false);

            _debugNormal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            _debugNormal.name = "SceneDebugger_Normal";
            _debugNormal.GetComponent<Renderer>().material = _debugMaterial;
            _debugNormal.transform.localScale = new Vector3(0.02f, 0.1f, 0.02f);
            _debugNormal.GetComponent<Collider>().enabled = false;
            _debugNormal.SetActive(false);
        }

        /// <summary>
        ///     Convenience method to show the normal of a hit collision.
        /// </summary>
        /// <param name="position">Position of the location to render</param>
        /// <param name="normal">Normal of the hit</param>
        private void ShowHitNormal(Vector3 position, Vector3 normal)
        {
            if (_debugNormal == null)
                return;
            if (position != Vector3.zero && normal != Vector3.zero)
            {
                _debugNormal.SetActive(true);
                _debugNormal.transform.rotation = Quaternion.FromToRotation(-Vector3.up, normal);
                _debugNormal.transform.position =
                    position + -_debugNormal.transform.up * _debugNormal.transform.localScale.y;
            }
            else
            {
                _debugNormal.SetActive(false);
            }
        }
    }
    // @endcond
}
