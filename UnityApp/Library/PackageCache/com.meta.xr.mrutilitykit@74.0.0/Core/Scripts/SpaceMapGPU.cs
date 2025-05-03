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
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace Meta.XR.MRUtilityKit
{
    /// <summary>
    /// Manages the creation and updating of a <see cref="SpaceMap"/> using GPU resources. This class is designed to handle
    /// dynamic environments, providing real-time updates to the spatial map.
    /// </summary>
    public class SpaceMapGPU : MonoBehaviour
    {
        /// <summary>
        /// Event triggered when the space map is initially created
        /// </summary>
        [field: SerializeField]
        public UnityEvent SpaceMapCreatedEvent
        {
            get;
            private set;
        } = new();

        /// <summary>
        /// Event triggered when the space map is created for a specifc room
        /// </summary>
        public UnityEvent<MRUKRoom> SpaceMapRoomCreatedEvent
        {
            get;
            private set;
        } = new();

        /// <summary>
        /// Event triggered when the space map is updated.
        /// </summary>
        [field: SerializeField]
        public UnityEvent SpaceMapUpdatedEvent
        {
            get;
            private set;
        } = new();

        [Tooltip("When the scene data is loaded, this controls what room(s) the spacemap will run on.")]
        [Header("Scene and Room Settings")]
        public MRUK.RoomFilter CreateOnStart = MRUK.RoomFilter.CurrentRoomOnly;

        [Tooltip("If enabled, updates on scene elements such as rooms and anchors will be handled by this class")]
        internal bool TrackUpdates = true;

        [Space]
        [Header("Textures")]
        [SerializeField]
        [Tooltip("Use this dimension for SpaceMap in X and Y")]
        public int TextureDimension = 512;

        [Tooltip("Colorize the SpaceMap with this Gradient")]
        public Gradient MapGradient = new();

        [Space]
        [Header("SpaceMap Settings")]
        [SerializeField]
        private Material gradientMaterial;

        [SerializeField] private ComputeShader CSSpaceMap;

        [Tooltip("Those Labels will be taken into account when running the SpaceMap")]
        [SerializeField]
        private MRUKAnchor.SceneLabels SceneObjectLabels;

        [Tooltip("Set a color for the inside of an Object")]
        [SerializeField]
        private Color InsideObjectColor;

        [Tooltip("Add this to the border of the capture Camera")]
        [SerializeField]
        private float CameraCaptureBorderBuffer = 0.5f;

        [Space]
        [Header("SpaceMap Debug Settings")]
        [SerializeField]
        [Tooltip("This setting affects your performance. If enabled, the TextureMap will be filled with the SpaceMap")]
        private bool CreateOutputTexture;

        [Tooltip("The Spacemap will be rendered into this Texture.")]
        [SerializeField]
        internal Texture2D OutputTexture;

        [Tooltip("Add here a debug plane")]
        [SerializeField]
        private GameObject DebugPlane;

        [SerializeField] private bool ShowDebugPlane;

        private Color colorFloorWall = Color.red;
        private Color colorSceneObjects = Color.green;
        private Color colorVirtualObjects = Color.blue;

        private Material matFloor;
        private Material matObjects;

        private Camera _captureCamera;
        private readonly float _cameraDistance = 10f;

        private RenderTexture[] _RTextures;

        private const string OculusUnlitShader = "Oculus/Unlit";

        private Texture2D _gradientTexture;

        private int _csSpaceMapKernel;
        private int _csFillSpaceMapKernel;
        private int _csPrepareSpaceMapKernel;

        internal bool Dirty
        {
            get;
            private set;
        } = false;

        private const string SHADER_GLOBAL_SPACEMAPCAMERAMATRIX = "_SpaceMapProjectionViewMatrix";

        [SerializeField]
        private RenderTexture RenderTexture;

        private static readonly int
            WidthID = Shader.PropertyToID("Width"),
            HeightID = Shader.PropertyToID("Height"),
            ColorFloorWallID = Shader.PropertyToID("ColorFloorWall"),
            ColorSceneObjectsID = Shader.PropertyToID("ColorSceneObjects"),
            ColorVirtualObjectsID = Shader.PropertyToID("ColorVirtualObjects"),
            StepID = Shader.PropertyToID("Step"),
            SourceID = Shader.PropertyToID("Source"),
            ResultID = Shader.PropertyToID("Result");

        private CommandBuffer commandBuffer;
        private RenderTexture rtCB;
        private Dictionary<MRUKRoom, RenderTexture> roomTextures;

        /// <summary>
        /// Gets the <see cref="RenderTexture"/> used for the space map for a given room. This property is not available until the space map is created.
        /// </summary>
        /// <param name="room">The <see cref="MRUKRoom"/> for which to get the space map texture.</param>
        /// <returns></returns>
        public RenderTexture GetSpaceMap(MRUKRoom room = null)
        {
            if (room == null)
            {
                //returning the default RenderTexture which can be initialized with AllRooms or CurrentRoom
                return RenderTexture;
            }
            if (!roomTextures.TryGetValue(room, out var rt))
            {
                //returning specific RenderTexture if it got called for a specific room
                Debug.Log($"Rendertexture for room {room} not found, returning default texture. Call StartSpaceMap(room) to create a texture for a specific room.");
                return RenderTexture;
            }
            return rt;
        }


        /// <summary>
        /// Initiates the space mapping process based on the specified room filter. This method sets up the necessary components
        /// and configurations to generate the space map, including updating textures and setting up the capture camera.
        /// </summary>
        /// <param name="roomFilter">The <see cref="MRUK.RoomFilter"/> that determines which rooms are included in the space map,
        /// influencing how the space map is generated.</param>
        /// <returns></returns>
        public async Task StartSpaceMap(MRUK.RoomFilter roomFilter)
        {
            Dirty = true;
            await InitUpdateGradientTexture();
            InitializeCaptureCamera(roomFilter);
            ApplyMaterial();

            UpdateBuffer(roomFilter);

            SpaceMapCreatedEvent.Invoke();
            Dirty = false;
        }

        /// <summary>
        /// Initiates the space mapping process for a specific room.
        /// </summary>
        /// <param name="room">Reference to MRUKRoom</param>
        public async void StartSpaceMap(MRUKRoom room)
        {
            Dirty = true;
            await InitUpdateGradientTexture();
            InitializeCaptureCamera(room);
            ApplyMaterial();

            UpdateBuffer(room);

            SpaceMapRoomCreatedEvent.Invoke(room);
            Dirty = false;
        }

        private void Start()
        {
            _RTextures = new RenderTexture[2];
            roomTextures = new Dictionary<MRUKRoom, RenderTexture>();

            //kernels for compute shader
            _csSpaceMapKernel = CSSpaceMap.FindKernel("SpaceMap");
            _csFillSpaceMapKernel = CSSpaceMap.FindKernel("FillSpaceMap");
            _csPrepareSpaceMapKernel = CSSpaceMap.FindKernel("PrepareSpaceMap");

            matFloor = new Material(Shader.Find(OculusUnlitShader));
            matObjects = new Material(Shader.Find(OculusUnlitShader));
            matFloor.color = colorFloorWall;
            matObjects.color = colorSceneObjects;

            if (MRUK.Instance is null)
            {
                return;
            }

            MRUK.Instance.RegisterSceneLoadedCallback(SceneLoaded);

            if (!TrackUpdates)
            {
                return;
            }

            MRUK.Instance.RoomCreatedEvent.AddListener(ReceiveCreatedRoom);
            MRUK.Instance.RoomRemovedEvent.AddListener(ReceiveRemovedRoom);
            MRUK.Instance.RoomUpdatedEvent.AddListener(ReceiveUpdatedRoom);
        }

        private async void SceneLoaded()
        {
            if (CreateOnStart == MRUK.RoomFilter.None)
            {
                return;
            }

            await StartSpaceMap(CreateOnStart);
        }

        private void InitBuffer()
        {
            commandBuffer = new CommandBuffer()
            {
                name = "SpaceMap"
            };

            rtCB = CreateNewRenderTexture(TextureDimension);
            commandBuffer.SetRenderTarget(rtCB);
        }

        private void UpdateBuffer(MRUKRoom room)
        {
            Prepare();
            if (_RTextures[0] == null)
            {
                return; //initialize phase
            }

            if (_captureCamera == null)
            {
                return;
            }

            if (!_captureCamera.isActiveAndEnabled)
            {
                return;
            }

            commandBuffer.SetViewProjectionMatrices(_captureCamera.worldToCameraMatrix, _captureCamera.projectionMatrix);

            DrawRoomIntoCB(room);
            Graphics.ExecuteCommandBuffer(commandBuffer);

            var rtRoom = CreateNewRenderTexture(RenderTexture.width);
            RunSpaceMap(ref rtRoom);

            if (CreateOutputTexture)
            {
                RenderTexture.active = rtRoom;
                OutputTexture.ReadPixels(new Rect(0, 0, TextureDimension, TextureDimension), 0, 0);
                OutputTexture.Apply();
                RenderTexture.active = null;
            }

            commandBuffer.Clear();
            commandBuffer.Dispose();

            roomTextures[room] = rtRoom;
        }

        private void Prepare()
        {
            InitBuffer();
            InitUpdateRT();
        }

        private void UpdateBuffer(MRUK.RoomFilter roomFilter)
        {
            Prepare();
            if (_RTextures[0] == null)
            {
                return; //initialize phase
            }

            if (_captureCamera == null)
            {
                return;
            }

            if (!_captureCamera.isActiveAndEnabled)
            {
                return;
            }

            commandBuffer.SetViewProjectionMatrices(_captureCamera.worldToCameraMatrix, _captureCamera.projectionMatrix);

            switch (roomFilter)
            {
                case MRUK.RoomFilter.None:
                    break;
                case MRUK.RoomFilter.CurrentRoomOnly:
                    DrawRoomIntoCB(MRUK.Instance.GetCurrentRoom());
                    Graphics.ExecuteCommandBuffer(commandBuffer);
                    break;
                case MRUK.RoomFilter.AllRooms:
                    foreach (var room in MRUK.Instance.Rooms)
                    {
                        DrawRoomIntoCB(room);
                        Graphics.ExecuteCommandBuffer(commandBuffer);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(roomFilter), roomFilter, null);
            }

            RunSpaceMap(ref RenderTexture);

            if (CreateOutputTexture)
            {
                RenderTexture.active = RenderTexture;
                OutputTexture.ReadPixels(new Rect(0, 0, TextureDimension, TextureDimension), 0, 0);
                OutputTexture.Apply();
                RenderTexture.active = null;
            }
            commandBuffer.Clear();
            commandBuffer.Dispose();
        }

        private void DrawRoomIntoCB(MRUKRoom room)
        {
            var rendFloor = room.FloorAnchor.gameObject.GetComponentInChildren<Renderer>();
            commandBuffer.DrawRenderer(rendFloor, matFloor, 0, -1);

            foreach (var anchor in room.Anchors)
            {
                var rend = anchor.gameObject.GetComponentInChildren<Renderer>();
                if (anchor.HasAnyLabel(SceneObjectLabels))
                {
                    commandBuffer.DrawRenderer(rend, matObjects, 0, -1);
                }
            }
        }

        private void RunSpaceMap(ref RenderTexture RT)
        {
            CSSpaceMap.SetInt(WidthID, (int)TextureDimension);
            CSSpaceMap.SetInt(HeightID, (int)TextureDimension);
            CSSpaceMap.SetVector(ColorFloorWallID, colorFloorWall);
            CSSpaceMap.SetVector(ColorSceneObjectsID, colorSceneObjects);
            CSSpaceMap.SetVector(ColorVirtualObjectsID, colorVirtualObjects);

            var threadGroupsX = Mathf.CeilToInt(TextureDimension / 8.0f);
            var threadGroupsY = Mathf.CeilToInt(TextureDimension / 8.0f);

            CSSpaceMap.SetTexture(_csPrepareSpaceMapKernel, SourceID, rtCB);
            CSSpaceMap.SetTexture(_csPrepareSpaceMapKernel, ResultID, _RTextures[0]);
            CSSpaceMap.Dispatch(_csPrepareSpaceMapKernel, threadGroupsX, threadGroupsY, 1);

            var stepAmount = (int)Mathf.Log(TextureDimension, 2);

            int sourceIndex = 0, resultIndex = 0;

            for (var i = 0; i < stepAmount; i++)
            {
                var step = (int)Mathf.Pow(2, stepAmount - i - 1);

                sourceIndex = i % 2;
                resultIndex = (i + 1) % 2;

                CSSpaceMap.SetInt(StepID, step);
                CSSpaceMap.SetTexture(_csSpaceMapKernel, SourceID, _RTextures[sourceIndex]);
                CSSpaceMap.SetTexture(_csSpaceMapKernel, ResultID, _RTextures[resultIndex]);
                CSSpaceMap.Dispatch(_csSpaceMapKernel, threadGroupsX, threadGroupsY, 1);
            }

            //swap indexes to get the correct one for source again
            CSSpaceMap.SetTexture(_csFillSpaceMapKernel, SourceID, _RTextures[resultIndex]);
            CSSpaceMap.SetTexture(_csFillSpaceMapKernel, ResultID, _RTextures[sourceIndex]);
            CSSpaceMap.Dispatch(_csFillSpaceMapKernel, threadGroupsX, threadGroupsY, 1);

            Graphics.Blit(_RTextures[sourceIndex], RT);

            gradientMaterial.SetTexture("_MainTex", _RTextures[sourceIndex]);
            SpaceMapUpdatedEvent.Invoke();
        }

        private void OnDestroy()
        {
            if (MRUK.Instance == null)
            {
                return;
            }

            MRUK.Instance.RoomCreatedEvent.RemoveListener(ReceiveCreatedRoom);
            MRUK.Instance.RoomRemovedEvent.RemoveListener(ReceiveRemovedRoom);
            MRUK.Instance.SceneLoadedEvent.RemoveListener(SceneLoaded);
        }

        private void ReceiveUpdatedRoom(MRUKRoom room)
        {
            if (TrackUpdates)
            {
                RegisterAnchorUpdates(room);
                UpdateBuffer(room);
            }
        }

        private void ReceiveCreatedRoom(MRUKRoom room)
        {
            //only create the effect mesh when we track room updates
            if (TrackUpdates &&
                CreateOnStart == MRUK.RoomFilter.AllRooms)
            {
                RegisterAnchorUpdates(room);
                UpdateBuffer(room);
            }
        }

        private void ReceiveRemovedRoom(MRUKRoom room)
        {
            UnregisterAnchorUpdates(room);
            roomTextures.Remove(room);
        }

        private void UnregisterAnchorUpdates(MRUKRoom room)
        {
            room.AnchorCreatedEvent.RemoveListener(ReceiveAnchorCreatedEvent);
            room.AnchorRemovedEvent.RemoveListener(ReceiveAnchorRemovedCallback);
            room.AnchorUpdatedEvent.RemoveListener(ReceiveAnchorUpdatedCallback);
        }

        private void RegisterAnchorUpdates(MRUKRoom room)
        {
            room.AnchorCreatedEvent.AddListener(ReceiveAnchorCreatedEvent);
            room.AnchorRemovedEvent.AddListener(ReceiveAnchorRemovedCallback);
            room.AnchorUpdatedEvent.AddListener(ReceiveAnchorUpdatedCallback);
        }

        private void ReceiveAnchorUpdatedCallback(MRUKAnchor anchor)
        {
            // only update the anchor when we track updates
            if (!TrackUpdates)
            {
                return;
            }
            UpdateBuffer(anchor.Room);
        }

        private void ReceiveAnchorRemovedCallback(MRUKAnchor anchor)
        {
            // there is no check on ```TrackUpdates``` when removing an anchor.
            UpdateBuffer(anchor.Room);
        }

        private void ReceiveAnchorCreatedEvent(MRUKAnchor anchor)
        {
            // only create the anchor when we track updates
            if (!TrackUpdates)
            {
                return;
            }
            UpdateBuffer(anchor.Room);
        }

        private void Update()
        {
            if (_captureCamera != null)
            {
                Shader.SetGlobalMatrix(SHADER_GLOBAL_SPACEMAPCAMERAMATRIX, _captureCamera.projectionMatrix * _captureCamera.worldToCameraMatrix);
            }

            if (DebugPlane != null && DebugPlane.activeSelf != ShowDebugPlane)
            {
                DebugPlane.SetActive(ShowDebugPlane);
            }
        }

        /// <summary>
        /// Color clamps to edge color if worldPosition is off-grid.
        /// getBilinear blends the color between pixels.
        /// </summary>
        /// <param name="worldPosition">The world position to sample the color from.</param>
        /// <returns>The color at the specified world position. Returns black if the capture camera is not initialized.</returns>
        public Color GetColorAtPosition(Vector3 worldPosition)
        {
            if (_captureCamera == null)
            {
                return Color.black;
            }

            var worldToScreenPoint = _captureCamera.WorldToScreenPoint(worldPosition);

            var xPixel = worldToScreenPoint.x / _captureCamera.pixelWidth;
            var yPixel = worldToScreenPoint.y / _captureCamera.pixelHeight;

            var rawColor = OutputTexture.GetPixelBilinear(xPixel, yPixel);

            var time = 1 - rawColor.r;
            return rawColor.b > 0 ? InsideObjectColor : MapGradient.Evaluate(time is >= 0 and <= 1 ? time : 0);
        }

        private void InitUpdateRT()
        {
            var wh = TextureDimension;

            if (_RTextures[0] == null || _RTextures[0].width != wh || _RTextures[0].height != wh)
            {
                TryReleaseRT(_RTextures[0]);
                TryReleaseRT(_RTextures[1]);
                _RTextures[0] = CreateNewRenderTexture(wh);
                _RTextures[1] = CreateNewRenderTexture(wh);
            }

            RenderTexture.active = _RTextures[0];
            GL.Clear(true, true, Color.clear);
            RenderTexture.active = null;
        }

        private static RenderTexture CreateNewRenderTexture(int wh)
        {
            var rt = new RenderTexture(wh, wh, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear) { enableRandomWrite = true };
            rt.Create();
            RenderTexture.active = rt;
            GL.Clear(true, true, new Color(1, 1, 1, 1));
            RenderTexture.active = null;
            return rt;
        }

        private static void TryReleaseRT(RenderTexture renderTexture)
        {
            if (renderTexture != null)
            {
                renderTexture.Release();
            }
        }

        private void ApplyMaterial()
        {
            gradientMaterial.SetTexture("_GradientTex", _gradientTexture);
            gradientMaterial.SetColor("_InsideColor", InsideObjectColor);
            if (DebugPlane != null)
            {
                DebugPlane.GetComponent<Renderer>().material = gradientMaterial;
            }
        }

        private async Task InitUpdateGradientTexture()
        {
            if (_gradientTexture == null)
            {
                _gradientTexture = new Texture2D(256, 1, TextureFormat.RGBA32, false);
            }

            for (var i = 0; i <= _gradientTexture.width; i++)
            {
                var t = i / (_gradientTexture.width - 1f);
                _gradientTexture.SetPixel(i, 0, MapGradient.Evaluate(t));
            }

            var unityContext = SynchronizationContext.Current;
            await Task.Run(() =>
            {
                unityContext.Post(_ =>
                {
                    _gradientTexture.Apply();
                }, null);
            });
        }

        private void SetupCaptureCamera()
        {
            if (_captureCamera == null)
            {
                _captureCamera = gameObject.AddComponent<Camera>();
            }

            _captureCamera.orthographic = true;
            _captureCamera.stereoTargetEye = StereoTargetEyeMask.None;
            _captureCamera.aspect = 1;
        }

        private void InitializeCaptureCamera(MRUK.RoomFilter roomFilter)
        {
            SetupCaptureCamera();
            var bb = GetBoundingBoxByFilter(roomFilter);
            transform.position = CalculateCameraPosition(bb);
            _captureCamera.orthographicSize = CalculateOrthographicSize(bb);
        }

        private void InitializeCaptureCamera(MRUKRoom room)
        {
            SetupCaptureCamera();
            var bb = GetBoundingBoxByRoom(room);
            transform.position = CalculateCameraPosition(bb);
            _captureCamera.orthographicSize = CalculateOrthographicSize(bb);
        }

        private HashSet<Transform> GetTargets(MRUK.RoomFilter roomFilter)
        {
            HashSet<Transform> targets = new();
            switch (roomFilter)
            {
                case MRUK.RoomFilter.CurrentRoomOnly:
                    targets = GetTargets(MRUK.Instance.GetCurrentRoom());
                    break;
                case MRUK.RoomFilter.AllRooms:
                    foreach (var room in MRUK.Instance.Rooms)
                    {
                        targets.UnionWith(GetTargets(room));
                    }
                    break;
            }

            return targets;
        }

        private HashSet<Transform> GetTargets(MRUKRoom room)
        {
            HashSet<Transform> targets = new();
            foreach (var anchor in room.Anchors)
            {
                if (anchor.HasAnyLabel(SceneObjectLabels))
                {
                    targets.Add(anchor.transform);
                }
            }
            targets.Add(room.FloorAnchor.transform);



            foreach (var roomWallAnchor in room.WallAnchors)
            {
                targets.Add(roomWallAnchor.transform);
            }

            return targets;
        }

        private Rect GetBoundingBoxByRoom(MRUKRoom room)
        {
            var targets = GetTargets(room);
            var (minX, maxX, minZ, maxZ) = CalculateMinMaxXY(targets);
            HandleDebugPlane(minX, maxX, minZ, maxZ);
            return Rect.MinMaxRect(minX - CameraCaptureBorderBuffer, maxZ + CameraCaptureBorderBuffer,
                maxX + CameraCaptureBorderBuffer, minZ - CameraCaptureBorderBuffer);
        }
        private Rect GetBoundingBoxByFilter(MRUK.RoomFilter roomFilter)
        {
            var targets = GetTargets(roomFilter);
            var (minX, maxX, minZ, maxZ) = CalculateMinMaxXY(targets);
            HandleDebugPlane(minX, maxX, minZ, maxZ);
            return Rect.MinMaxRect(minX - CameraCaptureBorderBuffer, maxZ + CameraCaptureBorderBuffer,
                maxX + CameraCaptureBorderBuffer, minZ - CameraCaptureBorderBuffer);
        }

        private void HandleDebugPlane(float minX, float maxX, float minZ, float maxZ)
        {
            if (DebugPlane == null)
            {
                return;
            }

            var sizeX = (maxX - minX + 2 * CameraCaptureBorderBuffer) / 10f;
            var sizeZ = (maxZ - minZ + 2 * CameraCaptureBorderBuffer) / 10f;

            var centerX = (minX + maxX) / 2;
            var centerZ = (minZ + maxZ) / 2;

            if (centerX is float.NaN || centerZ is float.NaN || sizeX is float.NegativeInfinity || sizeZ is float.NegativeInfinity)
            {
                return;
            }

            DebugPlane.transform.localScale = new Vector3(sizeX, 1, sizeZ);
            DebugPlane.transform.position = new Vector3(centerX, DebugPlane.transform.position.y, centerZ);
        }

        private (float, float, float, float) CalculateMinMaxXY(HashSet<Transform> targets)
        {
            var minX = Mathf.Infinity;
            var maxX = Mathf.NegativeInfinity;
            var minZ = Mathf.Infinity;
            var maxZ = Mathf.NegativeInfinity;
            foreach (Transform target in targets)
            {
                Vector3 position = target.position;
                minX = Mathf.Min(minX, position.x);
                maxX = Mathf.Max(maxX, position.x);
                minZ = Mathf.Min(minZ, position.z);
                maxZ = Mathf.Max(maxZ, position.z);
            }

            return (minX, maxX, minZ, maxX);
        }

        private Vector3 CalculateCameraPosition(Rect boundingBox)
        {
            return new Vector3(float.IsNaN(boundingBox.center.x) ? 0 : boundingBox.center.x, float.IsNaN(_cameraDistance) ? 0 : _cameraDistance, float.IsNaN(boundingBox.center.y) ? 0 : boundingBox.center.y);
        }

        private float CalculateOrthographicSize(Rect boundingBox)
        {
            return Mathf.Max(Mathf.Abs(boundingBox.width), Mathf.Abs(boundingBox.height)) / 2;
        }
    }
}
