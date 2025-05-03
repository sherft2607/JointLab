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
using System.Linq;
using Meta.XR.Util;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace Meta.XR.MRUtilityKit
{
    /// <summary>
    /// Manages dynamic mesh and collider generation based on scene elements.
    /// </summary>
    /// <remarks>
    /// This class is responsible for creating and managing the anchor's rendering and their associated colliders within a scene.
    /// It allows for dynamic updates based on scene changes, such as room and anchor updates, and provides options for
    /// shadow casting, visibility, and material customization.
    /// <see cref="EffectMeshObject "/> is used to represent the generated objects.
    /// </remarks>
    [Feature(Feature.Scene)]
    [HelpURL("https://developers.meta.com/horizon/reference/mruk/latest/class_meta_x_r_m_r_utility_kit_effect_mesh")]
    public class EffectMesh : MonoBehaviour
    {
        /// <summary>
        /// When the scene data is loaded, this controls what room(s) the effect mesh is applied to.
        /// See <see cref="MRUK.RoomFilter"/> for more details.
        /// </summary>
        [Tooltip("When the scene data is loaded, this controls what room(s) the effect mesh is applied to.")]
        public MRUK.RoomFilter SpawnOnStart = MRUK.RoomFilter.CurrentRoomOnly;

        /// <summary>
        /// If enabled, updates on scene elements such as rooms and anchors will be handled by this class
        /// </summary>
        [Tooltip("If enabled, updates on scene elements such as rooms and anchors will be handled by this class")]
        internal bool TrackUpdates = true;

        /// <summary>
        /// The material applied to the generated mesh. If you'd like a multi-material room, you can use another EffectMesh object with
        /// a different Mesh Material.
        /// </summary>
        [Tooltip("The material applied to the generated mesh. If you'd like a multi-material room, you can use another EffectMesh object with a different Mesh Material.")]
        [FormerlySerializedAs("_MeshMaterial")]
        public Material MeshMaterial;

        [Obsolete("BorderSize functionality has been removed.")]
        [NonSerialized]
        [FormerlySerializedAs("_borderSize")]
        public float BorderSize = 0.0f;

        /// <summary>
        /// If enabled, a BoxCollider will be generated for each mesh component or a MeshCollider if the anchor has a triangle mesh.
        /// </summary>
        [Tooltip("Generate a BoxCollider for each mesh component.")]
        [FormerlySerializedAs("addColliders")]
        public bool Colliders = false;

        [Tooltip("Cut holes in the mesh for door frames and/or window frames. NOTE: This does not apply if border size is non-zero.")]
        /// <summary>
        /// Cut holes in the mesh for door frames and/or window frames. NOTE: This does not apply if border size is non-zero
        /// </summary>
        public MRUKAnchor.SceneLabels CutHoles;

        [Tooltip("Whether the effect mesh objects will cast a shadow.")]
        [SerializeField]
        private bool castShadows = true;

        [Tooltip("Hide the effect mesh.")]
        [SerializeField]
        private bool hideMesh = false;


        private MRUK.SceneTrackingSettings SceneTrackingSettings;

        [HideInInspector] public int Layer = 0; // the layer to assign the effect objects to

        /// <summary>
        /// Gets or sets a value indicating whether the effect mesh objects should cast shadows.
        /// </summary>
        /// <value>
        /// <c>true</c> if effect mesh objects should cast shadows; otherwise, <c>false</c>.
        /// </value>
        public bool CastShadow
        {
            get => castShadows;
            set
            {
                ToggleShadowCasting(value);
                castShadows = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the effect mesh objects should be hidden.
        /// </summary>
        /// <value>
        /// <c>true</c> if effect mesh objects should be hidden; otherwise, <c>false</c>.
        /// </value>
        public bool HideMesh
        {
            get => hideMesh;
            set
            {
                ToggleEffectMeshVisibility(!value);
                hideMesh = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether colliders for the effect mesh objects should be active.
        /// </summary>
        /// <value>
        /// <c>true</c> if colliders for effect mesh objects should be active; otherwise, <c>false</c>.
        /// </value>
        [Obsolete("This property is deprecated. Please use '" + nameof(ToggleEffectMeshColliders) + "' instead.")]
        public bool ToggleColliders
        {
            get => Colliders;
            set
            {
                ToggleEffectMeshColliders(!value);
                Colliders = value;
            }
        }

        /// <summary>
        /// Defines the modes for calculating texture coordinates along the U-axis (horizontal) for wall textures.
        /// </summary>
        public enum WallTextureCoordinateModeU
        {
            METRIC, // The texture coordinates start at 0 and increase by 1 unit every meter.
            METRIC_SEAMLESS, // The texture coordinates start at 0 and increase by 1 unit every meter but are adjusted to end on a whole number to avoid seams.
            MAINTAIN_ASPECT_RATIO, // The texture coordinates are adjusted to the V dimensions to ensure the aspect ratio is maintained.
            MAINTAIN_ASPECT_RATIO_SEAMLESS, // The texture coordinates are adjusted to the V dimensions to ensure the aspect ratio is maintained but are adjusted to end on a whole number to avoid seams.
            STRETCH, // The texture coordinates range from 0 to 1.
            STRETCH_SECTION, // The texture coordinates start at 0 and increase to 1 for each individual wall section.
        };

        /// <summary>
        /// Defines the modes for calculating texture coordinates along the V-axis (vertical) for wall textures.
        /// </summary>
        public enum WallTextureCoordinateModeV
        {
            METRIC, // The texture coordinates start at 0 and increase by 1 unit every meter.
            MAINTAIN_ASPECT_RATIO, // The texture coordinates are adjusted to the U dimensions to ensure the aspect ratio is maintained.
            STRETCH, // The texture coordinates range from 0 to 1.
        };

        /// <summary>
        /// Defines the modes for calculating texture coordinates for anchor surfaces
        /// </summary>
        public enum AnchorTextureCoordinateMode
        {
            METRIC, // The texture coordinates start at 0 and increase by 1 unit every meter.
            STRETCH, // The texture coordinates range from 0 to 1 across the anchor surface.
        };

        /// <summary>
        /// Represents the texture coordinate modes for walls and anchors and defines how texture coordinates
        /// are calculated for different surfaces based on specified modes.
        /// See <see cref="WallTextureCoordinateModeU"/>, <see cref="WallTextureCoordinateModeV"/>, and <see cref="AnchorTextureCoordinateMode"/> for more details.
        /// </summary>
        [Serializable]
        public class TextureCoordinateModes
        {
            /// <summary>
            /// Specifies the texture coordinate mode for the U-axis of wall texture.
            /// To achieve seamless textures that highlight each wall the <see cref="WallTextureCoordinateModeU"/> should be set to <see cref="WallTextureCoordinateModeU.STRETCH_SECTION"/>
            /// <list type=" bullet">
            /// <item> <term> METRIC: </term> <description> The texture coordinates start at 0 and increase by
            /// 1 unit every meter. </description> </item>
            /// <item> <term> METRIC_SEAMLESS: </term> <description> The texture coordinates start at 0
            /// and increase by 1 unit every meter but are adjusted to end on a whole number to avoid
            /// seams. </description> </item>
            /// <item> <term> MAINTAIN_ASPECT_RATIO: </term> <description> The texture coordinates are
            /// adjusted to the V dimensions to ensure the aspect ratio is maintained. </description>
            /// </item>
            /// <item> <term> MAINTAIN_ASPECT_RATIO_SEAMLESS: </term> <description> The texture coordinates
            /// are adjusted to the V dimensions to ensure the aspect ratio is maintained but are
            /// adjusted to end on a whole number to avoid seams. </description> </item>
            /// <item> <term> STRETCH: </term> <description> The texture coordinates range from 0 to
            /// 1. </description> </item>
            /// <item> <term> STRETCH_SECTION: </term> <description> The texture coordinates start at 0
            /// and increase to 1 for each individual wall section. </description> </item>
            /// </list>
            /// </summary>
            [FormerlySerializedAs("U")] public WallTextureCoordinateModeU WallU = WallTextureCoordinateModeU.METRIC;

            /// <summary>
            /// Specifies the texture coordinate mode for the V-axis of wall textures.
            /// To achieve seamless textures that highlight each wall the <see cref="WallTextureCoordinateModeV"/> should be set to <see cref="WallTextureCoordinateModeV.STRETCH_SECTION"/>
            /// <list type=" bullet">
            /// <item> <term> METRIC: </term> <description> The texture coordinates start at 0 and increase by
            /// 1 unit every meter. </description> </item>
            /// <item> <term> METRIC_SEAMLESS: </term> <description> The texture coordinates start at 0
            /// and increase by 1 unit every meter but are adjusted to end on a whole number to avoid
            /// seams. </description> </item>
            /// <item> <term> MAINTAIN_ASPECT_RATIO: </term> <description> The texture coordinates are
            /// adjusted to the U dimensions to ensure the aspect ratio is maintained. </description>
            /// </item>
            /// <item> <term> MAINTAIN_ASPECT_RATIO_SEAMLESS: </term> <description> The texture coordinates
            /// are adjusted to the U dimensions to ensure the aspect ratio is maintained but are
            /// adjusted to end on a whole number to avoid seams. </description> </item>
            /// <item> <term> STRETCH: </term> <description> The texture coordinates range from 0 to
            /// 1. </description> </item>
            /// <item> <term> STRETCH_SECTION: </term> <description> The texture coordinates start at 0
            /// and increase to 1 for each individual wall section. </description> </item>
            /// </list>
            /// </summary>
            [FormerlySerializedAs("V")] public WallTextureCoordinateModeV WallV = WallTextureCoordinateModeV.METRIC;

            // Specifies the texture coordinate mode for anchor surfaces.
            /// <summary>
            /// Specifies the texture coordinate mode for anchor surfaces.
            /// <list type=" bullet">
            /// <item> <term> METRIC: </term> <description> The texture coordinates start at 0 and increase by
            /// 1 unit every meter. </description> </item>
            /// <item> <term> STRETCH: </term> <description> The texture coordinates range from 0 to
            /// 1 across the anchor surface. </description> </item>
            /// </list>
            /// </summary>
            public AnchorTextureCoordinateMode AnchorUV = AnchorTextureCoordinateMode.METRIC;
        };

        [Tooltip("Can not exceed 8.")]
        public TextureCoordinateModes[] textureCoordinateModes = new TextureCoordinateModes[1] { new() };

        [Tooltip(
            "Specifies the scene labels that determine which anchors representations are created by the effect mesh.")]
        [FormerlySerializedAs("_include")]
        public MRUKAnchor.SceneLabels Labels;

        private static readonly string Suffix = "_EffectMesh";

        private Dictionary<MRUKAnchor, EffectMeshObject> effectMeshObjects = new();

        /// <summary>
        /// Gets a dictionary that maps MRUKAnchor instances to their corresponding spawned EffectMeshObject.
        /// This should be treated as read-only, do not modify the contents.
        /// </summary>
        public IReadOnlyDictionary<MRUKAnchor, EffectMeshObject> EffectMeshObjects => effectMeshObjects;


        /// <summary>
        /// Represents an object that holds the components necessary for an effect mesh.
        /// Encapsulates the GameObject, Mesh, and Collider components used to create and manage effect meshes.
        /// Is used to store and access the generated <see cref="EffectMesh"/> and collider components.
        /// </summary>
        public class EffectMeshObject
        {
            /// <summary>
            /// The GameObject associated with the effect mesh object. Makes it easier to access the transform and other components.
            /// </summary>
            public GameObject effectMeshGO;

            /// <summary>
            /// The Mesh component associated with the effect mesh. This component holds the mesh data generated by the <see cref="EffectMesh"/>.
            public Mesh mesh;

            /// <summary>
            /// The Collider component associated with the effect mesh object. Can be a BoxCollider or a MeshCollider. Could be null if
            /// <see cref="EffectMesh.Colliders"/> was set to false.
            public Collider collider; // The Collider component associated with the effect mesh
        }

        private void Start()
        {
            OVRTelemetry.Start(TelemetryConstants.MarkerId.LoadEffectMesh).Send();
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
                        CreateMesh(MRUK.Instance.GetCurrentRoom());
                        break;
                    case MRUK.RoomFilter.AllRooms:
                        CreateMesh();
                        break;
                }
            });

            if (!TrackUpdates)
            {
                return;
            }

            MRUK.Instance.RoomCreatedEvent.AddListener(ReceiveCreatedRoom);
            MRUK.Instance.RoomRemovedEvent.AddListener(ReceiveRemovedRoom);
        }

        private void ReceiveRemovedRoom(MRUKRoom room)
        {
            // there is no check on ```TrackUpdates``` when removing a room.
            DestroyMesh(room);
            UnregisterAnchorUpdates(room);
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
            // &
            // only create when the anchor or parent room is tracked
            if (SceneTrackingSettings.UnTrackedRooms.Contains(anchor.Room) ||
                SceneTrackingSettings.UnTrackedAnchors.Contains(anchor) ||
                !TrackUpdates)
            {
                return;
            }

            DestroyMesh(anchor);
            CreateEffectMesh(anchor);
        }

        private void ReceiveAnchorRemovedCallback(MRUKAnchor anchor)
        {
            // there is no check on ```TrackUpdates``` when removing an anchor.
            DestroyMesh(anchor);
        }

        private void ReceiveAnchorCreatedEvent(MRUKAnchor anchor)
        {
            // only create the anchor when we track updates
            // &
            // only create when the parent room is tracked
            if (SceneTrackingSettings.UnTrackedRooms.Contains(anchor.Room) ||
                !TrackUpdates)
            {
                return;
            }

            CreateEffectMesh(anchor);
        }

        private void ReceiveCreatedRoom(MRUKRoom room)
        {
            //only create the room when we track room updates
            if (TrackUpdates &&
                SpawnOnStart == MRUK.RoomFilter.AllRooms)
            {
                CreateMesh(room);
                RegisterAnchorUpdates(room);
            }
        }

        /// <summary>
        ///     Given a counter-clockwise set of points, triangulate the interior
        /// </summary>
        void CreateInteriorPolygon(ref int[] indexArray, ref int indexCounter, int baseCount, List<Vector2> points)
        {
            Triangulator.TriangulatePoints(points, null, out var vertices, out var indices);
            int capTriCount = indices.Length / 3;
            for (int j = 0; j < capTriCount; j++)
            {
                int id0 = indices[j * 3];
                int id1 = indices[j * 3 + 1];
                int id2 = indices[j * 3 + 2];

                indexArray[indexCounter++] = baseCount + id0;
                indexArray[indexCounter++] = baseCount + id1;
                indexArray[indexCounter++] = baseCount + id2;
            }
        }

        /// <summary>
        ///     Create a triangle fan given the number of points to triangulate
        /// </summary>
        void CreateInteriorTriangleFan(ref int[] indexArray, ref int indexCounter, int baseCount, int pointsInLoop)
        {
            int capTriCount = pointsInLoop - 2;
            for (int j = 0; j < capTriCount; j++)
            {
                int id1 = j + 1;
                int id2 = j + 2;
                indexArray[indexCounter++] = baseCount;
                indexArray[indexCounter++] = baseCount + id1;
                indexArray[indexCounter++] = baseCount + id2;
            }
        }

        /// <summary>
        ///     Creates effect mesh for all elements in all rooms
        /// </summary>
        public void CreateMesh()
        {
            foreach (var room in MRUK.Instance.Rooms)
            {
                CreateMesh(room);
                RegisterAnchorUpdates(room);
            }
        }


        /// <summary>
        ///     Destroys mesh the objects instantiated based on the provided label filter.
        /// </summary>
        /// <param name="label">
        ///     The filter for mesh object destruction.
        ///     If a mesh object's anchor labels pass this filter, the mesh object will be destroyed.
        ///     Default value includes all labels.
        /// </param>
        public void DestroyMesh(LabelFilter label = new LabelFilter())
        {
            List<MRUKAnchor> itemsToRemove = new();
            foreach (var kv in effectMeshObjects)
            {
                bool filterByLabel = label.PassesFilter(kv.Key.Label);
                if (kv.Value.effectMeshGO && filterByLabel)
                {
                    DestroyImmediate(kv.Value.effectMeshGO);
                    itemsToRemove.Add(kv.Key);
                }
            }

            foreach (var itemToRemove in itemsToRemove)
            {
                effectMeshObjects.Remove(itemToRemove);
                SceneTrackingSettings.UnTrackedAnchors.Add(itemToRemove);
            }
        }

        /// <summary>
        ///     Destroys all meshs in the given room and mark the room as not tracked anymore by this class
        /// </summary>
        /// <param name="room">MRUK Room</param>
        public void DestroyMesh(MRUKRoom room)
        {
            var anchors = room.Anchors;
            foreach (var anchor in anchors)
            {
                DestroyMesh(anchor);
            }

            SceneTrackingSettings.UnTrackedRooms.Add(room);
        }

        /// <summary>
        ///     Destroys the meshs associated with the provided anchor and mark the anchor as not tracked anymore by this class
        /// </summary>
        /// <param name="anchor">MRUK Anchor</param>
        public void DestroyMesh(MRUKAnchor anchor)
        {
            if (effectMeshObjects.TryGetValue(anchor, out var eMO))
            {
                if (eMO.effectMeshGO)
                {
                    DestroyImmediate(eMO.effectMeshGO);
                    effectMeshObjects.Remove(anchor);
                    SceneTrackingSettings.UnTrackedAnchors.Add(anchor);
                }
            }
        }

        /// <summary>
        ///     Adds colliders to the mesh objects instantiated based on the provided label filter.
        /// </summary>
        /// <param name="label">
        ///     The filter to determine which mesh objects receive a collider.
        ///     If a mesh object's anchor labels pass this filter and the mesh object does not already have a collider, a new collider is added.
        ///     Default value includes all labels.
        /// </param>
        public void AddColliders(LabelFilter label = new LabelFilter())
        {
            foreach (var kv in effectMeshObjects)
            {
                bool filterByLabel = label.PassesFilter(kv.Key.Label);
                if (kv.Key && !kv.Value.collider && filterByLabel)
                {
                    kv.Value.collider = AddCollider(kv.Key, kv.Value);
                }
            }
        }

        /// <summary>
        ///     Destroy the colliders of the instantiated mesh objects based on the provided label filter.
        /// </summary>
        /// <param name="label">
        ///     The filter to determine which mesh objects receive a collider.
        ///     If a mesh object's anchor labels pass this filter and the mesh object does not already have a collider, a new collider is added.
        ///     Default value includes all labels.
        /// </param>
        public void DestroyColliders(LabelFilter label = new LabelFilter())
        {
            foreach (var kv in effectMeshObjects)
            {
                bool filterByLabel = label.PassesFilter(kv.Key.Label);
                if (kv.Value.collider && filterByLabel)
                {
                    DestroyImmediate(kv.Value.collider);
                }
            }
        }

        /// <summary>
        /// Toggles the shadow casting behavior of effect mesh objects based on the specified label filter.
        /// </summary>
        /// <param name="shouldCast">Determines whether shadows should be cast by the effect mesh objects.</param>
        /// <param name="label">A filter that specifies which effect mesh objects should have their shadow casting toggled.
        /// The default is a new instance of LabelFilter, which includes all labels.</param>
        public void ToggleShadowCasting(bool shouldCast, LabelFilter label = new LabelFilter())
        {
            foreach (var kv in effectMeshObjects)
            {
                bool filterByLabel = label.PassesFilter(kv.Key.Label);
                if (kv.Value.effectMeshGO && filterByLabel)
                {
                    ShadowCastingMode castingMode = castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off;
                    kv.Value.effectMeshGO.GetComponent<MeshRenderer>().shadowCastingMode = castingMode;
                }
            }
        }

        /// <summary>
        ///     Toggles the visibility of the effect mesh objects instantiated based on the provided label filter.
        /// </summary>
        /// <param name="shouldShow">Determines whether the effect mesh objects should be visible or not.</param>
        /// <param name="label">
        ///     The filter to determine which effect mesh objects have their visibility toggled.
        ///     If an effect mesh object's anchor labels pass this filter, its visibility is toggled according to the 'shouldShow' parameter.
        ///     Default value includes all labels.
        /// </param>
        /// <param name="materialOverride">
        ///     An optional material to apply to the effect mesh objects when their visibility is toggled.
        ///     If not provided, the material of the mesh objects remains unchanged.
        /// </param>
        public void ToggleEffectMeshVisibility(bool shouldShow, LabelFilter label = new LabelFilter(), Material materialOverride = null)
        {
            foreach (var kv in effectMeshObjects)
            {
                bool filterByLabel = label.PassesFilter(kv.Key.Label);
                if (kv.Value.effectMeshGO && filterByLabel)
                {
                    kv.Value.effectMeshGO.GetComponent<MeshRenderer>().enabled = shouldShow;
                    if (materialOverride)
                    {
                        kv.Value.effectMeshGO.GetComponent<MeshRenderer>().material = materialOverride;
                    }
                }
            }
        }

        /// <summary>
        ///     Toggles the colliders of the effect mesh objects. Creates one if not existing.
        /// </summary>
        /// <param name="doEnable">Determines whether the effect mesh objects should have an active collider or not.</param>
        /// <param name="label">
        ///     The filter to determine which effect mesh objects have their visibility toggled.
        ///     If an effect mesh object's anchor labels pass this filter, its visibility is toggled according to the 'shouldShow' parameter.
        ///     Default value includes all labels.
        /// </param>
        public void ToggleEffectMeshColliders(bool doEnable, LabelFilter label = new LabelFilter())
        {
            foreach (var kv in effectMeshObjects)
            {
                var filterByLabel = label.PassesFilter(kv.Key.Label);
                if (!filterByLabel)
                {
                    continue;
                }

                if (!kv.Value.collider)
                {
                    AddCollider(kv.Key, kv.Value);
                }

                kv.Value.collider.enabled = doEnable;
            }
        }

        /// <summary>
        ///     Overrides the material of the effect mesh objects instantiated based on the provided label filter.
        /// </summary>
        /// <param name="newMaterial">The new material to apply to the effect mesh objects.</param>
        /// <param name="label">
        ///     The filter to determine which effect mesh objects have their material overridden.
        ///     If an effect mesh object's anchor labels pass this filter, its material is changed to the new material.
        ///     Default value is a new instance of LabelFilter.
        /// </param>
        public void OverrideEffectMaterial(Material newMaterial, LabelFilter label = new LabelFilter())
        {
            foreach (var kv in effectMeshObjects)
            {
                bool filterByLabel = label.PassesFilter(kv.Key.Label);
                if (kv.Value.effectMeshGO && filterByLabel)
                {
                    kv.Value.effectMeshGO.GetComponent<MeshRenderer>().material = newMaterial;
                }
            }
        }

        private static (Dictionary<MRUKAnchor.SceneLabels, List<MRUKAnchor>>, float) GenerateData(MRUKRoom room)
        {
            var totalWallLength = 0.0f;

            List<MRUKAnchor> regularAndInvisibleWalls = new();
            const MRUKAnchor.SceneLabels regularOrInvisibleWallLabel = MRUKAnchor.SceneLabels.WALL_FACE | MRUKAnchor.SceneLabels.INVISIBLE_WALL_FACE;
            foreach (var anchorInfo in room.Anchors)
            {
                if (anchorInfo.HasAnyLabel(regularOrInvisibleWallLabel))
                {
                    regularAndInvisibleWalls.Add(anchorInfo);
                }
            }

            OrderWalls(regularAndInvisibleWalls, ref totalWallLength);

            var d = new Dictionary<MRUKAnchor.SceneLabels, List<MRUKAnchor>>
            {
                { regularOrInvisibleWallLabel, regularAndInvisibleWalls },
            };

            return (d, totalWallLength);
        }

        private static void OrderWalls(List<MRUKAnchor> walls, ref float wallLength)
        {
            foreach (var wall in walls)
            {
                wallLength += wall.PlaneRect.Value.size.x;
            }

            int count = walls.Count;
            if (count <= 1)
            {
                return;
            }

            MRUKAssert.AreEqual(count, walls.Distinct().Count(), "count, walls.Distinct().Count()");
            using (new OVRObjectPool.ListScope<MRUKAnchor>(out var result))
            {
                int iniIndex = count - 1;
                MRUKAnchor current = walls[iniIndex];
                result.Add(current);
                walls.RemoveAt(iniIndex);

                while (walls.Count > 0)
                {
                    float minDistance = float.MaxValue;
                    int closestWallIndex = -1;
                    Vector3 rightPos = current.transform.position - current.transform.right * current.PlaneRect.Value.size.x * 0.5f;
                    for (int i = 0; i < walls.Count; i++)
                    {
                        MRUKAnchor wall = walls[i];
                        Vector3 leftPos = wall.transform.position + wall.transform.right * wall.PlaneRect.Value.size.x * 0.5f;
                        float distance = Vector3.Distance(rightPos, leftPos);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closestWallIndex = i;
                        }
                    }
                    Assert.AreNotEqual(-1, closestWallIndex);
                    current = walls[closestWallIndex];
                    result.Add(current);
                    walls.RemoveAt(closestWallIndex);
                }

                MRUKAssert.AreEqual(count, result.Distinct().Count(), "count, result.Distinct().Count()");
                walls.AddRange(result);
            }
        }

        /// <summary>
        ///     Creates effect mesh for all objects in the given room
        /// </summary>
        /// <param name="room">The room to apply to</param>
        public void CreateMesh(MRUKRoom room)
        {
            CreateMesh(room, null);
        }

        /// <summary>
        ///     Creates effect mesh for all objects in the given room
        /// </summary>
        /// <param name="room">The room to apply to</param>
        /// <param name="connectedRooms">An optional list of connected rooms</param>
        private void CreateMesh(MRUKRoom room, List<MRUKRoom> connectedRooms)
        {
            // To get all the anchors in the space:
            var sceneAnchors = room.Anchors;

            var (sortedObjects, totalWallLength) = GenerateData(room);

            MRUKAnchor floor = null;
            MRUKAnchor ceiling = null;

            for (var i = 0; i < sceneAnchors.Count; i++)
            {
                var anchorInfo = sceneAnchors[i];
                if (anchorInfo && anchorInfo.HasAnyLabel(Labels))
                {
                    if (anchorInfo.HasAnyLabel(MRUKAnchor.SceneLabels.WALL_FACE))
                    {
                        continue;
                    }


                    if (anchorInfo.HasAnyLabel(MRUKAnchor.SceneLabels.CEILING))
                    {
                        ceiling = anchorInfo;
                    }
                    else if (anchorInfo.HasAnyLabel(MRUKAnchor.SceneLabels.FLOOR))
                    {
                        floor = anchorInfo;
                    }
                    else if (anchorInfo.HasAnyLabel(MRUKAnchor.SceneLabels.GLOBAL_MESH))
                    {
                        CreateGlobalMeshObject(anchorInfo);
                    }
                    else
                    {
                        CreateEffectMesh(anchorInfo);
                    }
                }
            }

            var uSpacing = 0.0f;
            foreach (var (lbl, items) in sortedObjects)
            {
                if (IncludesLabel(lbl))
                {
                    foreach (var wall in items)
                    {
                        if (IncludesLabel(GetOriginalLabel(wall.Label)))
                        {
                            CreateEffectMeshWall(wall, totalWallLength, ref uSpacing, connectedRooms);
                        }
                    }
                }
            }

            if (ceiling)
            {
                CreateEffectMesh(ceiling);
            }

            if (floor)
            {
                CreateEffectMesh(floor);
            }

            if (!TrackUpdates)
            {
                if (!SceneTrackingSettings.UnTrackedRooms.Contains(room))
                {
                    SceneTrackingSettings.UnTrackedRooms.Add(room);
                }
            }
        }
        private bool IncludesLabel(MRUKAnchor.SceneLabels label)
        {
            return (Labels & label) != 0;
        }

        /// <summary>
        /// The <see cref="OVRSemanticLabels.GetClassifications"/> method upgrades the InvisibleWallFace to also have the WallFace for backward compatibility.<br/>
        /// This method does the opposite: given the upgraded label, it returns the original one.
        /// </summary>
        private static MRUKAnchor.SceneLabels GetOriginalLabel(MRUKAnchor.SceneLabels upgradedLabel)
        {
            if ((upgradedLabel & MRUKAnchor.SceneLabels.INVISIBLE_WALL_FACE) != 0)
            {
                return MRUKAnchor.SceneLabels.INVISIBLE_WALL_FACE;
            }
            if ((upgradedLabel & MRUKAnchor.SceneLabels.WALL_FACE) != 0)
            {
                return MRUKAnchor.SceneLabels.WALL_FACE;
            }
            return upgradedLabel;
        }

        /// <summary>
        /// Creates an effect mesh for a specified anchor using the default border size.
        /// </summary>
        /// <param name="anchorInfo">The anchor information used to create the effect mesh.</param>
        /// <returns>An instance of <see cref="EffectMeshObject"/> representing the created effect mesh
        /// or null if the mesh could not be created.</returns>
        public EffectMeshObject CreateEffectMesh(MRUKAnchor anchorInfo)
        {
            if (effectMeshObjects.ContainsKey(anchorInfo))
            {
                //Anchor already has an EffectMeshComponent
                return null;
            }

            EffectMeshObject effectMeshObject = new EffectMeshObject();
            var newMesh = Utilities.SetupAnchorMeshGeometry(anchorInfo, false, textureCoordinateModes);


            var newGameObject = new GameObject(anchorInfo.name + Suffix);
            newGameObject.transform.SetParent(anchorInfo.transform, false);
            newGameObject.layer = Layer;

            effectMeshObject.effectMeshGO = newGameObject;

            var meshFilter = newGameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = newMesh;

            // only attach MeshRenderer if a material has been assigned
            if (MeshMaterial != null)
            {
                var newRenderer = newGameObject.AddComponent<MeshRenderer>();
                newRenderer.material = MeshMaterial;
                newRenderer.shadowCastingMode = castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off;
                newRenderer.enabled = !hideMesh;
            }

            newMesh.name = anchorInfo.name;

            effectMeshObject.mesh = newMesh;

            if (Colliders)
            {
                effectMeshObject.collider = AddCollider(anchorInfo, effectMeshObject);
            }

            effectMeshObjects.Add(anchorInfo, effectMeshObject);
            return effectMeshObject;
        }

        private Collider AddCollider(MRUKAnchor anchorInfo, EffectMeshObject effectMeshObject)
        {
            if (anchorInfo.VolumeBounds.HasValue)
            {
                var boxCollider = effectMeshObject.effectMeshGO.AddComponent<BoxCollider>();
                boxCollider.size = anchorInfo.VolumeBounds.Value.size;
                boxCollider.center = anchorInfo.VolumeBounds.Value.center;
                return boxCollider;
            }

            var meshCollider = effectMeshObject.effectMeshGO.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = effectMeshObject.mesh;
            meshCollider.convex = false;
            return meshCollider;
        }

        float GetSeamlessFactor(float totalWallLength, float stepSize)
        {
            float roundedTotalWallLength = Mathf.Round(totalWallLength / stepSize);
            roundedTotalWallLength = Mathf.Max(1, roundedTotalWallLength);
            return totalWallLength / roundedTotalWallLength;
        }

        EffectMeshObject CreateEffectMeshWall(MRUKAnchor anchorInfo, float totalWallLength, ref float uSpacing, List<MRUKRoom> connectedRooms)
        {
            if (effectMeshObjects.ContainsKey(anchorInfo))
            {
                //WallAnchor already has an EffectMeshComponent
                return null;
            }

            EffectMeshObject effectMeshObject = new();

            GameObject newGameObject = new GameObject(anchorInfo.name + Suffix);
            newGameObject.layer = Layer;
            newGameObject.transform.SetParent(anchorInfo.transform, false);

            effectMeshObject.effectMeshGO = newGameObject;

            Mesh newMesh = new Mesh();
            var meshFilter = newGameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = newMesh;

            // only attach MeshRenderer if a material has been assigned
            if (MeshMaterial != null)
            {
                MeshRenderer newRenderer = newGameObject.AddComponent<MeshRenderer>();
                newRenderer.material = MeshMaterial;
                newRenderer.shadowCastingMode = castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off;
                newRenderer.enabled = !hideMesh;
            }

            Vector2 wallScale = anchorInfo.PlaneRect.Value.size;
            float ceilingHeight = wallScale.y;

            List<List<Vector2>> holes = null;

            Rect wallRect = anchorInfo.PlaneRect.Value;

            foreach (var child in anchorInfo.ChildAnchors)
            {
                if (child.PlaneRect.HasValue)
                {
                    bool shouldCutHoles = (child.Label & CutHoles) != 0;
                    if (!shouldCutHoles)
                    {
                        continue;
                    }

                    Vector2 relativePos = anchorInfo.transform.InverseTransformPoint(child.transform.position);

                    var childRect = child.PlaneRect.Value;
                    childRect.position += new Vector2(relativePos.x, relativePos.y);
                    List<Vector2> childOutline = new(child.PlaneBoundary2D.Count);
                    // Reverse the order for the holes, this is necessary for the hole cutting algorithm to work
                    // correctly.
                    for (int i = child.PlaneBoundary2D.Count - 1; i >= 0; i--)
                    {
                        childOutline.Add(child.PlaneBoundary2D[i] + relativePos);
                    }

                    holes ??= new List<List<Vector2>>();
                    holes.Add(childOutline);
                }
            }

            Triangulator.TriangulatePoints(anchorInfo.PlaneBoundary2D, holes, out var vertices, out var triangles);

            int totalVertices = vertices.Length;

            int UVChannelCount = Math.Min(8, textureCoordinateModes.Length);

            Vector3[] MeshVertices = new Vector3[totalVertices];

            for (int i = 0; i < vertices.Length; ++i)
            {
                MeshVertices[i] = vertices[i];
            }

            Vector2[][] MeshUVs = new Vector2[UVChannelCount][];
            for (int x = 0; x < UVChannelCount; x++)
            {
                MeshUVs[x] = new Vector2[totalVertices];
            }

            Color32[] MeshColors = new Color32[totalVertices];
            Vector3[] MeshNormals = new Vector3[totalVertices];
            Vector4[] MeshTangents = new Vector4[totalVertices];

            int vertCounter = 0;

            float seamlessScaleFactor = GetSeamlessFactor(totalWallLength, 1);

            // direction to points
            float thisSegmentLength = wallScale.x;

            Vector3 wallNorm = Vector3.forward;
            Vector4 wallTan = new Vector4(1, 0, 0, 1);

            for (int j = 0; j < vertices.Length; j++)
            {
                var vert = MeshVertices[j];

                float u = vert.x - wallRect.xMin;
                float v = vert.y - wallRect.yMin;

                for (int x = 0; x < UVChannelCount; x++)
                {
                    float denominatorX;
                    float denominatorY;
                    float defaultSpacing = uSpacing;
                    // Determine the scaling in the V direction first, if this is set to maintain aspect
                    // ratio we need to come back to it after U scaling has been determined.
                    switch (textureCoordinateModes[x].WallV)
                    {
                        // Default to stretch in case maintain aspect ratio is set for both axes
                        default:
                        case WallTextureCoordinateModeV.STRETCH:
                            denominatorY = ceilingHeight;
                            break;
                        case WallTextureCoordinateModeV.METRIC:
                            denominatorY = 1;
                            break;
                    }

                    switch (textureCoordinateModes[x].WallU)
                    {
                        default:
                        case WallTextureCoordinateModeU.STRETCH:
                            denominatorX = totalWallLength;
                            break;
                        case WallTextureCoordinateModeU.METRIC:
                            denominatorX = 1;
                            break;
                        case WallTextureCoordinateModeU.METRIC_SEAMLESS:
                            denominatorX = seamlessScaleFactor;
                            break;
                        case WallTextureCoordinateModeU.MAINTAIN_ASPECT_RATIO:
                            denominatorX = denominatorY;
                            break;
                        case WallTextureCoordinateModeU.MAINTAIN_ASPECT_RATIO_SEAMLESS:
                            denominatorX = GetSeamlessFactor(totalWallLength, denominatorY);
                            break;
                        case WallTextureCoordinateModeU.STRETCH_SECTION:
                            denominatorX = thisSegmentLength;
                            defaultSpacing = 0;
                            break;
                    }

                    // Do another pass on V in case it has maintain aspect ratio set
                    if (textureCoordinateModes[x].WallV == WallTextureCoordinateModeV.MAINTAIN_ASPECT_RATIO)
                    {
                        denominatorY = denominatorX;
                    }

                    MeshUVs[x][vertCounter] = new Vector2((defaultSpacing + thisSegmentLength - u) / denominatorX, v / denominatorY);
                }

                MeshVertices[vertCounter] = new Vector3(u - thisSegmentLength / 2, v - ceilingHeight / 2, 0);
                MeshColors[vertCounter] = Color.white;
                MeshNormals[vertCounter] = wallNorm;
                MeshTangents[vertCounter] = wallTan;

                vertCounter++;
            }

            uSpacing += thisSegmentLength;

            int[] MeshTriangles = triangles;

            newMesh.Clear();
            newMesh.name = anchorInfo.name;
            newMesh.vertices = MeshVertices;
            for (int x = 0; x < UVChannelCount; x++)
            {
                switch (x)
                {
                    case 0:
                        newMesh.uv = MeshUVs[x];
                        break;
                    case 1:
                        newMesh.uv2 = MeshUVs[x];
                        break;
                    case 2:
                        newMesh.uv3 = MeshUVs[x];
                        break;
                    case 3:
                        newMesh.uv4 = MeshUVs[x];
                        break;
                    case 4:
                        newMesh.uv5 = MeshUVs[x];
                        break;
                    case 5:
                        newMesh.uv6 = MeshUVs[x];
                        break;
                    case 6:
                        newMesh.uv7 = MeshUVs[x];
                        break;
                    case 7:
                        newMesh.uv8 = MeshUVs[x];
                        break;
                }
            }

            newMesh.colors32 = MeshColors;
            newMesh.triangles = MeshTriangles;
            newMesh.normals = MeshNormals;
            newMesh.tangents = MeshTangents;

            effectMeshObject.mesh = newMesh;

            if (Colliders)
            {
                effectMeshObject.collider = AddCollider(anchorInfo, effectMeshObject);
            }

            effectMeshObjects.Add(anchorInfo, effectMeshObject);
            return effectMeshObject;
        }

        void CreateGlobalMeshObject(MRUKAnchor globalMeshAnchor)
        {
            if (!globalMeshAnchor)
            {
                Debug.LogWarning("No global mesh was found in the current room");
                return;
            }

            if (effectMeshObjects.ContainsKey(globalMeshAnchor))
            {
                //Anchor already has an EffectMeshComponent
                return;
            }

            var effectMeshObject = new EffectMeshObject();

            var globalMeshGO = new GameObject(globalMeshAnchor.name + Suffix, typeof(MeshFilter), typeof(MeshRenderer));
            globalMeshGO.layer = Layer;
            globalMeshGO.transform.SetParent(globalMeshAnchor.transform, false);
            effectMeshObject.effectMeshGO = globalMeshGO;

            globalMeshAnchor.Mesh.RecalculateNormals();
            var trimesh = globalMeshAnchor.Mesh;

            globalMeshGO.GetComponent<MeshFilter>().sharedMesh = trimesh;

            if (Colliders)
            {
                var meshCollider = globalMeshGO.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = trimesh;
                effectMeshObject.collider = meshCollider;
            }

            var renderer = globalMeshGO.GetComponent<MeshRenderer>();
            if (MeshMaterial != null)
            {
                renderer.material = MeshMaterial;
            }

            renderer.enabled = !hideMesh;
            renderer.shadowCastingMode = castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off;
            effectMeshObject.mesh = trimesh;
            effectMeshObjects.Add(globalMeshAnchor, effectMeshObject);
        }

        /// <summary>
        /// Utility function that sets the parent transform for all effect mesh objects managed by this class.
        /// </summary>
        /// <param name="newParent">The new parent transform to which all effect mesh objects will be attached.</param>
        public void SetEffectObjectsParent(Transform newParent)
        {
            foreach (var kv in effectMeshObjects)
            {
                kv.Value.effectMeshGO.transform.SetParent(newParent);
            }
        }
    }
}
