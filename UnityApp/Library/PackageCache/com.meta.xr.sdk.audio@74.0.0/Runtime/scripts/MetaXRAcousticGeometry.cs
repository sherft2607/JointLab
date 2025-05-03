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

/************************************************************************************
 * Filename    :   MetaXRAcousticGeometry.cs
 * Content     :   Geometry Functions
                Attach to a game object with meshes and material scripts to create geometry
                NOTE: ensure that Oculus Spatialization is enabled for AudioSource components
 ***********************************************************************************/

#define INCLUDE_TERRAIN_TREES

using Meta.XR.Acoustics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Serialization;
using Native = MetaXRAcousticNativeInterface;

/// \brief Class that allows mesh analysis to precompute an Acoustic Geometry
/// \see MetaXRAcousticNativeInterface
internal class MetaXRAcousticGeometry : MonoBehaviour
{
    internal static bool AUTO_VALIDATE = true;
    internal const string FILE_EXTENSION = "xrageo";
    internal static int EnabledGeometryCount = 0;
    internal static event Action OnAnyGeometryEnabled = () => { };
    //-------
    // PUBLIC

    [SerializeField]
    [FormerlySerializedAs("relativeFilePath_")]
    private string relativeFilePath = "";
#if UNITY_EDITOR
    internal string RelativeFilePath => string.IsNullOrEmpty(relativeFilePath) ? GenerateSuggestedPath() : relativeFilePath;
#else
    internal string RelativeFilePath => relativeFilePath;
#endif
    /// \brief Absolute path to the serialized mesh file that holds the preprocessed mesh geometry.
    /// This path should be absolute and somewhere inside Application.dataPath directory.
    internal string AbsoluteFilePath
    {
        get => Path.GetFullPath(Path.Combine(Application.dataPath, RelativeFilePath));
        set
        {
            string sanitizedPath = value.Replace('\\', '/');
            // Make the path relative to the Assets directory.
            if (sanitizedPath.StartsWith(Application.dataPath))
                relativeFilePath = sanitizedPath.Substring(Application.dataPath.Length + 1);
            else
                Debug.LogError($"invalid path {value}, outside application path {Application.dataPath}", gameObject);
        }
    }

    /// \brief If this is set, then the serialized acoustic geometry file (.xrageo) is used as the mesh data source.
    [SerializeField]
    internal bool FileEnabled = true;

    /// \brief  This button chooses whether or not child meshes of the GameObject where the geometry script is attached are included in the acoustic geometry.
    /// This option can be used to automatically combine all meshes within an object hierarchy into a single optimized acoustic geometry.
    /// This will be faster for ray tracing and produce better quality diffraction than many smaller meshes. This is typically used for the static meshes in a scene.
    [SerializeField]
    internal bool IncludeChildMeshes = true;

    /// The flags for how the mesh is computed.
    [SerializeField]
    internal MeshFlags Flags = MeshFlags.ENABLE_SIMPLIFICATION;

    internal bool EnableSimplification
    {
        get => (Flags & MeshFlags.ENABLE_SIMPLIFICATION) != 0;
        set
        {
            if (value)
                Flags |= MeshFlags.ENABLE_SIMPLIFICATION;
            else
                Flags &= ~MeshFlags.ENABLE_SIMPLIFICATION;
        }
    }

    /// \brief A button that chooses whether or not diffraction information should be computed for a mesh. This enables sound to propagate around that mesh.
    /// If disabled, no diffraction will occur around the mesh and the direct sound will be completely occluded when the source is not visible.
    /// It may be useful to only enable diffraction on large meshes (e.g. scene environment geometry), but to disable it on small props and clutter objects.
    /// This can improve the performance by reducing the total number of edges in the scene.
    internal bool EnableDiffraction
    {
        get => (Flags & MeshFlags.ENABLE_DIFFRACTION) != 0;
        set
        {
            if (value)
                Flags |= MeshFlags.ENABLE_DIFFRACTION;
            else
                Flags &= ~MeshFlags.ENABLE_DIFFRACTION;
        }
    }

    /// \brief The maximum allowed error for the automatic acoustic geometry simplification.
    /// This control specifies an error threshold in meters (regardless of which units the game engine uses).
    /// A relatively large error threshold can be used to reduce the geometry complexity (memory size and runtime ray tracing cost).
    /// The default error threshold is 0.1, i.e. 10 cm. The threshold may be increased further (up to around 0.5 meters) without any problems in most cases.
    [SerializeField]
    private float maxSimplifyError = 0.1f;
    internal float MaxSimplifyError
    {
        get => maxSimplifyError;
        set
        {
#if UNITY_EDITOR
            if (value < 0)
                throw new ArgumentOutOfRangeException("Maximum simplification error must be >= 0.");
#endif
            maxSimplifyError = Math.Max(value, 0.0f);
        }
    }

    /// \brief The minimum angle (degrees) that there must be between two adjacent face normals for their edge to be marked as diffracting.
    [SerializeField]
    private float minDiffractionEdgeAngle = 1.0f;
    internal float MinDiffractionEdgeAngle
    {
        get => minDiffractionEdgeAngle;
        set
        {
#if UNITY_EDITOR
            if (value < 0)
                throw new ArgumentOutOfRangeException("Minimum diffraction edge angle must be >= 0.");
            if (value > 180.0)
                throw new ArgumentOutOfRangeException("Minimum diffraction edge angle must be <= 180.");
#endif
            minDiffractionEdgeAngle = Math.Clamp(value, 0.0f, 180.0f);
        }
    }

    /// \brief The minimum length in meters that an edge should have for it to be marked as diffracting.
    [SerializeField]
    private float minDiffractionEdgeLength = 0.01f;
    internal float MinDiffractionEdgeLength
    {
        get => minDiffractionEdgeLength;
        set
        {
#if UNITY_EDITOR
            if (value < 0)
                throw new ArgumentOutOfRangeException("Minimum diffraction edge length must be >= 0.");
#endif
            minDiffractionEdgeLength = Math.Max(value, 0.0f);
        }
    }

    /// \brief The maximum distance in meters that a diffraction flag extends out from the edge.
    [SerializeField]
    private float flagLength = 1.0f;
    internal float FlagLength
    {
        get => flagLength;
        set
        {
#if UNITY_EDITOR
            if (value < 0)
                throw new ArgumentOutOfRangeException("flag length must be greater than 0.");
#endif
            flagLength = value;
        }
    }

    /// \brief The Level of Detail to use for acoustics, the higher the LOD the less polygons. Typically the highest LOD will be sufficient and the most efficient.
    [SerializeField]
    private int lodSelection = 0;
    internal int LodSelection
    {
        get => lodSelection;
        set => lodSelection = value;
    }

    /// \brief If enabled the acoustic geometry will be computed using the physics Mesh Colliders. If enabled, Meta XR Acoustic Material scripts will be ignored.
    /// The mapping between Mesh Colliders and Meta XR Acoustic Material Properties can be configured in **Project Settings > Meta XR Acoustics**
    [SerializeField]
    private bool useColliders = false;
    internal bool UseColliders
    {
        get => useColliders;
        set => useColliders = value;
    }

    /// \brief If enabled, the overrideExcludeTags will be used to exclude objects from the baked Acoustic Geometry instead of the project setting Exclude Tags
    [SerializeField]
    private bool overrideExcludeTagsEnabled = false;
    internal bool OverrideExcludeTagsEnabled { get => overrideExcludeTagsEnabled; set => overrideExcludeTagsEnabled = value; }

    /// \brief The list of tags to be used for excluding objects from the baked Acoustic Geometry instead of the project setting Exclude Tags.
    [SerializeField]
    private string[] overrideExcludeTags = null;
    internal string[] OverrideExcludeTags { get => overrideExcludeTags; set => overrideExcludeTags = value; }

    internal string[] ExcludeTags => OverrideExcludeTagsEnabled ? OverrideExcludeTags : MetaXRAcousticSettings.Instance.ExcludeTags;


    //-------
    // PRIVATE
    [NonSerialized]
    internal IntPtr geometryHandle = IntPtr.Zero;

    [NonSerialized]
    private bool isLoaded = false;
    internal bool IsLoaded => isLoaded;

    [NonSerialized]
    private int vertexCount = -1;
    internal int VertexCount => vertexCount;

    // Disable unused variable warning that occurs in builds.
    // This preserves serialization layout, while using UNITY_EDITOR
    // to remove the variable would break the serialization layout.
#pragma warning disable 0169
    [SerializeField]
    private Color[] materialColors;
    /// \brief A hash code of the game objects contributing to the current serialized acoustic geometry, for detecting changes.
    [SerializeField]
    private Hash128 HierarchyHash;
#pragma warning restore 0169
#if UNITY_EDITOR
    [NonSerialized]
    private IMaterialDataProvider[] gizmoMaterialMapping;
    [NonSerialized]
    private int[] gizmoVertexMaterialIndices;

    [NonSerialized]
    private uint[] materialIndices;
#endif

    //-------
    // PUBLIC STATIC
    internal const int Success = 0;


    /// <summary>
    /// If script is attached to a gameobject, it will try to create geometry
    /// </summary>
    void Awake()
    {
        StartInternal();
    }

    internal bool StartInternal()
    {
        if (!CreatePropagationGeometry())
            return false;

        // Make sure the geometry has current transform matrix.
        ApplyTransform();

        return true;
    }

    /// <summary>
    /// Call this function to create geometry handle
    /// </summary>
    internal bool CreatePropagationGeometry()
    {
        if (geometryHandle != IntPtr.Zero)
        {
            Debug.LogWarning("Tried to initialize geometry twice, destroying stale copy", gameObject);
            DestroyPropagationGeometry();
        }

        if (geometryHandle != IntPtr.Zero)
        {
            Debug.LogError("Unable to clean up stale geometry", gameObject);
            return false;
        }

        if (Native.Interface.CreateAudioGeometry(out geometryHandle) != Success)
        {
            Debug.LogError("Unable to create geometry handle", gameObject);
            return false;
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (!GatherGeometryEditor(geometryHandle, gameObject, gameObject.transform.worldToLocalMatrix))
                return false;
        }
        else
# endif
        if (FileEnabled)
        {
            if (string.IsNullOrEmpty(relativeFilePath))
            {
                Debug.LogError("No file set, make sure to Bake Mesh to File", gameObject);
                return false;
            }
            else
            {
                if (!ReadFile())
                    return false;
            }
        }
        else
        {
            if (gameObject.isStatic)
            {
                Debug.LogError("Static geometry requires \"File Enabled\"", gameObject);
                return false;
            }
            else
            {
                if (!GatherGeometryRuntime())
                    return false;
            }
        }

        return true;
    }

    void IncrementEnabledGeometryCount()
    {
        ++EnabledGeometryCount;
        if (EnabledGeometryCount == 1)
            OnAnyGeometryEnabled();
    }

    void DecrementEnabledGeometryCount() => --EnabledGeometryCount;

    /// Called when enabled.
    void OnEnable()
    {
        if (geometryHandle == IntPtr.Zero || (!isLoaded && FileEnabled))
            return;

        Debug.Log($"Enabling Geometry: {relativeFilePath}", gameObject);

        Native.Interface.AudioGeometrySetObjectFlag(geometryHandle, ObjectFlags.ENABLED, true);
        ApplyTransform();
        Native.Interface.AudioGeometrySetObjectFlag(geometryHandle, ObjectFlags.STATIC, gameObject.isStatic);

        IncrementEnabledGeometryCount();
    }

    /// Called when disabled.
    void OnDisable()
    {
        if (geometryHandle == IntPtr.Zero)
            return;

        Debug.Log($"Disabling Geometry: {relativeFilePath}", gameObject);

        Native.Interface.AudioGeometrySetObjectFlag(geometryHandle, ObjectFlags.ENABLED, false);
        ApplyTransform();
        Native.Interface.AudioGeometrySetObjectFlag(geometryHandle, ObjectFlags.STATIC, gameObject.isStatic);

        DecrementEnabledGeometryCount();
    }


    /// Update the world transform.
    /// Do this in LateUpdate() instead of Update() because it works better with animations.
    private void LateUpdate()
    {
        if (geometryHandle == IntPtr.Zero)
            return;

        if (transform.hasChanged)
        {
            ApplyTransform();

            // Reset dirty bit.
            transform.hasChanged = false;
        }
    }

    private void ApplyTransform()
    {
        if (geometryHandle == IntPtr.Zero)
            return;

        Native.Interface.AudioGeometrySetTransform(geometryHandle, transform.localToWorldMatrix);
    }

    /// <summary>
    /// Call when destroyed
    /// </summary>
    private void OnDestroy()
    {
        DestroyInternal();
    }

    internal bool DestroyInternal()
    {
        if (!DestroyPropagationGeometry())
            return false;

        return true;
    }

    private bool DestroyPropagationGeometry()
    {
        lock (this)
        {
            if (geometryHandle != IntPtr.Zero && Native.Interface.DestroyAudioGeometry(geometryHandle) != Success)
            {
                Debug.LogError("Unable to destroy geometry", gameObject);
                return false;
            }

            geometryHandle = IntPtr.Zero;

            return true;
        }
    }

    //
    // FUNCTIONS FOR UPLOADING MESHES VIA GAME OBJECT
    //

    static int terrainDecimation = 4;

    private struct MeshMaterial
    {
        internal Mesh mesh;
        internal Transform meshTransform;
        internal IMaterialDataProvider[] meshMaterials;
    }

    private struct TerrainMaterial
    {
        internal Terrain terrain;
        internal IMaterialDataProvider[] terrainMaterials;
        internal Mesh[] treePrototypeMeshes;
    }

    internal interface ITransformVisitor
    {
        System.Object visit(Transform transform, System.Object userData);
    }

    interface IGatherer : ITransformVisitor
    {
        List<MeshMaterial> Meshes { get; }
        List<TerrainMaterial> Terrains { get; }
    }

    class MeshGatherer : IGatherer
    {
        internal MeshGatherer(bool ignoreStatic)
        {
            this.ignoreStatic = ignoreStatic;
        }

        public System.Object visit(Transform transform, System.Object parentData)
        {
            var currentMaterials = parentData as IMaterialDataProvider[];
            MeshFilter[] meshes = transform.GetComponents<MeshFilter>();
            Terrain[] terrains = transform.GetComponents<Terrain>();
            var activeMaterials = transform.GetComponents<MetaXRAcousticMaterial>().Where(x => x.enabled);
            IMaterialDataProvider[] materials = Array.ConvertAll(activeMaterials.ToArray(), x => x);

            // Initialize the current material array to a new array if there are any new materials.
            if (materials != null && materials.Length > 0)
            {
                // Determine the length of the material array.
                int maxLength = materials.Length;
                if (currentMaterials != null && currentMaterials.Length > maxLength)
                    maxLength = currentMaterials.Length;

                Meta.XR.Acoustics.IMaterialDataProvider[] newMaterials = new Meta.XR.Acoustics.IMaterialDataProvider[maxLength];

                // Copy the previous materials into the new array.
                if (currentMaterials != null)
                {
                    for (int i = materials.Length; i < maxLength; i++)
                        newMaterials[i] = currentMaterials[i];
                }
                currentMaterials = newMaterials;

                // Copy the current materials.
                for (int i = 0; i < materials.Length; i++)
                    currentMaterials[i] = materials[i];
            }

            // Gather the meshes.
            foreach (MeshFilter meshFilter in meshes)
            {
                Mesh sharedMesh = meshFilter.sharedMesh;
                if (sharedMesh == null)
                    continue;

                if (ignoreStatic && (!sharedMesh.isReadable || transform.gameObject.isStatic))
                {
                    Debug.LogError($"Mesh: {meshFilter.gameObject.name} not readable. Use \"File Enabled\" for static geometry", transform);
                    ++ignoredMeshCount;
                    continue;
                }
                this.meshes.Add(new MeshMaterial() { mesh = sharedMesh, meshTransform = transform, meshMaterials = currentMaterials });
            }

            // Gather the terrains.
            foreach (Terrain terrain in terrains)
                this.terrains.Add(new TerrainMaterial() { terrain = terrain, terrainMaterials = currentMaterials });

            return currentMaterials;
        }

        private List<MeshMaterial> meshes = new List<MeshMaterial>();
        public List<MeshMaterial> Meshes { get => meshes; }
        private List<TerrainMaterial> terrains = new List<TerrainMaterial>();
        public List<TerrainMaterial> Terrains { get => terrains; }
        internal int ignoredMeshCount = 0;
        internal bool ignoreStatic;
    }

    class ColliderGatherer : IGatherer
    {
        public System.Object visit(Transform transform, System.Object parentData)
        {
            var activeMaterials = transform.GetComponents<MetaXRAcousticMaterial>().Where(x => x.enabled);
            IMaterialDataProvider[] materials = Array.ConvertAll(activeMaterials.ToArray(), x => x);

            MeshCollider[] colliders = transform.GetComponents<MeshCollider>();
            foreach (MeshCollider mc in colliders)
            {
                if (mc.sharedMesh == null)
                    continue;

                if (materials.Length == 0)
                {
                    // No MaterialComponents found, see if there is a mapping for PhysicMaterials
                    MetaXRAcousticMaterialProperties mat = MetaXRAcousticMaterialMapping.Instance.findAcousticMaterial(mc.sharedMaterial);
                    if (mat != null)
                    {
                        materials = new IMaterialDataProvider[] { mat };
#if META_XR_ACOUSTIC_INFO
                        Debug.Log($"Found PhysicMaterial {mc.sharedMaterial?.name} => {mat.name}");
#endif
                    }
                }

                meshes.Add(new MeshMaterial() { mesh = mc.sharedMesh, meshTransform = transform, meshMaterials = materials });
            }

            BoxCollider[] boxColliders = transform.GetComponents<BoxCollider>();
            foreach (BoxCollider bc in boxColliders)
            {
                Mesh box = new Mesh();
                Vector3[] verts = new Vector3[8];
                verts[0] = bc.center + Vector3.Scale(bc.size * 0.5f, new Vector3(1f, 1f, 1f));
                verts[1] = bc.center + Vector3.Scale(bc.size * 0.5f, new Vector3(1f, 1f, -1f));
                verts[2] = bc.center + Vector3.Scale(bc.size * 0.5f, new Vector3(1f, -1f, 1f));
                verts[3] = bc.center + Vector3.Scale(bc.size * 0.5f, new Vector3(1f, -1f, -1f));
                verts[4] = bc.center + Vector3.Scale(bc.size * 0.5f, new Vector3(-1f, 1f, 1f));
                verts[5] = bc.center + Vector3.Scale(bc.size * 0.5f, new Vector3(-1f, 1f, -1f));
                verts[6] = bc.center + Vector3.Scale(bc.size * 0.5f, new Vector3(-1f, -1f, 1f));
                verts[7] = bc.center + Vector3.Scale(bc.size * 0.5f, new Vector3(-1f, -1f, -1f));

                int[] indices = new int[24]{
                    1,0,2,3, // left
                    0,4,6,2, // front
                    4,5,7,6, // right
                    5,1,3,7, // back
                    1,5,4,0, // top
                    2,6,7,3  // bottom
                };

                box.vertices = verts;
                box.SetIndices(indices, MeshTopology.Quads, 0);

                if (materials.Length == 0)
                {
                    // No MaterialComponents found, see if there is a mapping for PhysicMaterials
                    MetaXRAcousticMaterialProperties mat = MetaXRAcousticMaterialMapping.Instance.findAcousticMaterial(bc.sharedMaterial);
                    if (mat != null)
                    {
                        materials = new IMaterialDataProvider[] { mat };
#if META_XR_ACOUSTIC_INFO
                        Debug.Log($"Found PhysicMaterial {bc.sharedMaterial?.name} => {mat.name}");
#endif
                    }
                }

                meshes.Add(new MeshMaterial() { mesh = box, meshTransform = transform, meshMaterials = materials });
            }

            return null;
        }

        private List<MeshMaterial> meshes = new List<MeshMaterial>();
        public List<MeshMaterial> Meshes { get => meshes; }
        private List<TerrainMaterial> terrains = new List<TerrainMaterial>();
        public List<TerrainMaterial> Terrains { get => terrains; }
    }

    private static void traverseMeshHierarchy(GameObject obj, bool includeChildren, string[] excludeTags, bool parentWasExcluded, int lodSelection, ITransformVisitor visitor, System.Object parentData = null)
    {
        if (!obj.activeInHierarchy)
        {
#if META_XR_ACOUSTIC_INFO
            Debug.Log($"Skipping inactive Object {obj.name}", obj);
#endif
            return;
        }

        // Check for LOD. If present, use only the highest LOD and don't recurse to children.
        // Without this, we can accidentally get all LODs merged together.
        LODGroup lodGroup = obj.GetComponent(typeof(LODGroup)) as LODGroup;
        if (lodGroup != null)
        {
#if META_XR_ACOUSTIC_INFO
            Debug.Log($"LOD Group detected during acoustic geometry traversal");
#endif
            LOD[] lods = lodGroup.GetLODs();
            if (lods.Length > 0)
            {
                bool isLodTrivial = (lods.Length == 1 && lods[0].renderers.Length == 1);
                if (isLodTrivial)
                {
                    obj = lods[0].renderers[0].gameObject;
                    includeChildren = false;
                }
                else
                {
                    // Get renderers for user selected LOD. Note they can select any value, so clamp to the known range
                    int lodGroupToUse = Mathf.Clamp(lodSelection, 0, lods.Length - 1);
                    Renderer[] lodRenderers = lods[lodGroupToUse].renderers;

#if META_XR_ACOUSTIC_INFO
                    Debug.Log($"Using LOD Group {lodGroupToUse} for acoustic geometry which has {lodRenderers.Length} renderers", obj);
#endif

                    // Get and add the game object for every renderer at this LOD level
                    // Some meshes split their mesh into multiple cells, which is why we can't just use a single renderer
                    for (int i = 0; i < lodRenderers.Length; i++)
                    {
                        includeChildren = false;
                        if (lodRenderers[i] != null)
                        {
                            if (lodRenderers[i].gameObject == obj)
                                continue; // avoid infinite recursion

                            traverseMeshHierarchy(lodRenderers[i].gameObject, includeChildren, excludeTags, parentWasExcluded, lodSelection, visitor, parentData);
                        }
                    }
                }
            }
        }

        bool shouldVisit = true;
        if (excludeTags.Contains(obj.tag) || parentWasExcluded)
        {
            MetaXRAcousticMaterial mat = obj.GetComponent<MetaXRAcousticMaterial>();
            if (mat == null || !mat.enabled)
            {
#if META_XR_ACOUSTIC_INFO
                Debug.Log($"Skipping Object {obj.name} based on exclude tag: {obj.tag}", obj);
#endif
                shouldVisit = false;
            }
            else
            {
#if META_XR_ACOUSTIC_INFO
                Debug.Log($"Override exclude tag {obj.tag} due to presence of acoustic material in child {mat.gameObject.name}", obj);
#endif
                shouldVisit = true;
            }
        }

        if (shouldVisit)
            parentData = visitor.visit(obj.transform, parentData);

        // Traverse the child transforms.
        if (includeChildren)
        {
            foreach (Transform child in obj.transform)
            {
                if (child.GetComponent<MetaXRAcousticGeometry>() == null)
                    traverseMeshHierarchy(child.gameObject, includeChildren, excludeTags, !shouldVisit, lodSelection, visitor, parentData);
#if META_XR_ACOUSTIC_INFO
                else
                    Debug.Log($"Skipping child: {child.name}, it has it's own {nameof(MetaXRAcousticGeometry)} component");
#endif
            }
        }
    }

#if UNITY_EDITOR
    private bool GatherGeometryEditor(IntPtr geometryHandle, GameObject meshObject, Matrix4x4 worldToLocal)
    {
        return GatherGeometryInternal(geometryHandle, meshObject, worldToLocal, false, out int unused);
    }
#endif

    private bool GatherGeometryInternal(IntPtr geometryHandle, GameObject meshObject, Matrix4x4 worldToLocal, bool ignoreStatic, out int ignoredMeshCount)
    {
        ignoredMeshCount = 0;

        // Get the child mesh objects.
        IGatherer gatherer;
        if (useColliders)
            gatherer = new ColliderGatherer();
        else
            gatherer = new MeshGatherer(ignoreStatic);

        traverseMeshHierarchy(meshObject, IncludeChildMeshes, ExcludeTags, false, lodSelection, gatherer);

        //***********************************************************************
        // Count the number of vertices and indices.

        int totalVertexCount = 0;
        uint totalIndexCount = 0;
        int totalFaceCount = 0;
        int totalMaterialCount = 0;

        foreach (MeshMaterial m in gatherer.Meshes)
            updateCountsForMesh(ref totalVertexCount, ref totalIndexCount, ref totalFaceCount, ref totalMaterialCount, m.mesh);

        IMaterialDataProvider[] treeMaterials = new IMaterialDataProvider[1];

        for (int i = 0; i < gatherer.Terrains.Count; ++i)
        {
            TerrainMaterial t = gatherer.Terrains[i];
            TerrainData terrain = t.terrain.terrainData;

#if UNITY_2019_3_OR_NEWER
            int w = terrain.heightmapResolution;
            int h = terrain.heightmapResolution;
#else
            int w = terrain.heightmapWidth;
            int h = terrain.heightmapHeight;
#endif
            int wRes = (w - 1) / terrainDecimation + 1;
            int hRes = (h - 1) / terrainDecimation + 1;
            int vertexCount = wRes * hRes;
            int indexCount = (wRes - 1) * (hRes - 1) * 6;

            totalMaterialCount++;
            totalVertexCount += vertexCount;
            totalIndexCount += (uint)indexCount;
            totalFaceCount += indexCount / 3;

#if INCLUDE_TERRAIN_TREES
            TreePrototype[] treePrototypes = terrain.treePrototypes;

            if (treePrototypes.Length != 0)
            {
                if (treeMaterials[0] == null)
                {
                    // Use last material attached to terrain for foliage
                    treeMaterials[0] = t.terrainMaterials.Last();
                }

                t.treePrototypeMeshes = new Mesh[treePrototypes.Length];

                // Assume the sharedMesh with the lowest vertex is the lowest LOD
                for (int j = 0; j < treePrototypes.Length; ++j)
                {
                    GameObject prefab = treePrototypes[j].prefab;
                    MeshFilter[] meshFilters = prefab.GetComponentsInChildren<MeshFilter>();
                    int minVertexCount = int.MaxValue;
                    int index = -1;
                    for (int k = 0; k < meshFilters.Length; ++k)
                    {
                        int count = meshFilters[k].sharedMesh.vertexCount;
                        if (count < minVertexCount)
                        {
                            minVertexCount = count;
                            index = k;
                        }
                    }

                    t.treePrototypeMeshes[j] = meshFilters[index].sharedMesh;
                }

                TreeInstance[] trees = terrain.treeInstances;
                foreach (TreeInstance tree in trees)
                {
                    updateCountsForMesh(ref totalVertexCount, ref totalIndexCount, ref totalFaceCount,
                        ref totalMaterialCount, t.treePrototypeMeshes[tree.prototypeIndex]);
                }

                gatherer.Terrains[i] = t;
            }
#endif
        }

        //***********************************************************************
        // Copy the mesh data.

        List<Vector3> tempVertices = new List<Vector3>();
        List<int> tempIndices = new List<int>();

        MeshGroup[] groups = new MeshGroup[totalMaterialCount];
        float[] vertices = new float[totalVertexCount * 3];
        int[] indices = new int[totalIndexCount];

        int vertexOffset = 0;
        int indexOffset = 0;
        int groupOffset = 0;

        foreach (MeshMaterial m in gatherer.Meshes)
        {
            // Compute the combined transform to go from mesh-local to geometry-local space.
            Matrix4x4 matrix = worldToLocal * m.meshTransform.localToWorldMatrix;

            if (!uploadMeshFilter(tempVertices, tempIndices, groups, vertices, indices, ref vertexOffset, ref indexOffset, ref groupOffset, m.mesh, m.meshMaterials, matrix))
                return false;
        }

        foreach (TerrainMaterial t in gatherer.Terrains)
        {
            TerrainData terrain = t.terrain.terrainData;

            // Compute the combined transform to go from mesh-local to geometry-local space.
            Matrix4x4 matrix = worldToLocal * t.terrain.gameObject.transform.localToWorldMatrix;

#if UNITY_2019_3_OR_NEWER
            int w = terrain.heightmapResolution;
            int h = terrain.heightmapResolution;
#else
            int w = terrain.heightmapWidth;
            int h = terrain.heightmapHeight;
#endif
            float[,] tData = terrain.GetHeights(0, 0, w, h);

            Vector3 meshScale = terrain.size;
            meshScale = new Vector3(meshScale.x / (w - 1) * terrainDecimation, meshScale.y, meshScale.z / (h - 1) * terrainDecimation);
            int wRes = (w - 1) / terrainDecimation + 1;
            int hRes = (h - 1) / terrainDecimation + 1;
            int vertexCount = wRes * hRes;
            int triangleCount = (wRes - 1) * (hRes - 1) * 2;

            // Initialize the group.
            groups[groupOffset].faceType = FaceType.TRIANGLES;
            groups[groupOffset].faceCount = (UIntPtr)triangleCount;
            groups[groupOffset].indexOffset = (UIntPtr)indexOffset;

            if (t.terrainMaterials != null && 0 < t.terrainMaterials.Length)
                groups[groupOffset].material = MetaXRAcousticMaterial.CreateMaterialNativeHandle(t.terrainMaterials[0].Data);
            else
                groups[groupOffset].material = IntPtr.Zero;

            // Build vertices and UVs
            for (int y = 0; y < hRes; y++)
            {
                for (int x = 0; x < wRes; x++)
                {
                    int offset = (vertexOffset + y * wRes + x) * 3;
                    Vector3 v = matrix.MultiplyPoint3x4(Vector3.Scale(meshScale, new Vector3(y, tData[x * terrainDecimation, y * terrainDecimation], x)));
                    vertices[offset + 0] = v.x;
                    vertices[offset + 1] = v.y;
                    vertices[offset + 2] = v.z;
                }
            }

            // Build triangle indices: 3 indices into vertex array for each triangle
            for (int y = 0; y < hRes - 1; y++)
            {
                for (int x = 0; x < wRes - 1; x++)
                {
                    // For each grid cell output two triangles
                    indices[indexOffset + 0] = (vertexOffset + (y * wRes) + x);
                    indices[indexOffset + 1] = (vertexOffset + ((y + 1) * wRes) + x);
                    indices[indexOffset + 2] = (vertexOffset + (y * wRes) + x + 1);

                    indices[indexOffset + 3] = (vertexOffset + ((y + 1) * wRes) + x);
                    indices[indexOffset + 4] = (vertexOffset + ((y + 1) * wRes) + x + 1);
                    indices[indexOffset + 5] = (vertexOffset + (y * wRes) + x + 1);
                    indexOffset += 6;
                }
            }

            vertexOffset += vertexCount;
            groupOffset++;

#if INCLUDE_TERRAIN_TREES
            TreeInstance[] trees = terrain.treeInstances;
            foreach (TreeInstance tree in trees)
            {
                Vector3 pos = Vector3.Scale(tree.position, terrain.size);
                Matrix4x4 treeLocalToWorldMatrix = t.terrain.gameObject.transform.localToWorldMatrix;
                treeLocalToWorldMatrix.SetColumn(3, treeLocalToWorldMatrix.GetColumn(3) + new Vector4(pos.x, pos.y, pos.z, 0.0f));
                // TODO: tree rotation
                Matrix4x4 treeMatrix = worldToLocal * treeLocalToWorldMatrix;
                if (!uploadMeshFilter(tempVertices, tempIndices, groups, vertices, indices, ref vertexOffset, ref indexOffset, ref groupOffset, t.treePrototypeMeshes[tree.prototypeIndex], treeMaterials, treeMatrix))
                    return false;
            }
#endif
        }

        if (totalVertexCount == 0)
        {
            string path = ((gameObject.scene != null) ? gameObject.scene.name : "") + ":" + string.Join("/", gameObject.GetComponentsInParent<Transform>().Select(t => t.name).Reverse().ToArray());
            Debug.LogError($"Geometry unable to upload mesh, vertex count is zero {path}", gameObject);
            return false;
        }

        Debug.Log($"Uploading mesh {name} with {totalVertexCount} vertices");

        // Gather the mesh simplification parameters to pass to the upload
        MeshSimplification simplification = new MeshSimplification();
        simplification.thisSize = (UIntPtr)Marshal.SizeOf(typeof(MeshSimplification));
        simplification.flags = Flags;
        simplification.unitScale = 1; // Unity always uses 1 unit equals 1 meter
        simplification.maxError = MaxSimplifyError;
        simplification.minDiffractionEdgeAngle = MinDiffractionEdgeAngle;
        simplification.minDiffractionEdgeLength = MinDiffractionEdgeLength;
        simplification.flagLength = FlagLength;
#if UNITY_EDITOR
        simplification.threadCount = (UIntPtr)0; // Use as many threads as CPUs
#else
        simplification.threadCount = (UIntPtr)1; // Don't create any threads if not in editor.
#endif

        // Upload mesh data
        int result = Native.Interface.AudioGeometryUploadSimplifiedMeshArrays(geometryHandle,
                                                       vertices, totalVertexCount,
                                                       indices, indices.Length,
                                                       groups, groups.Length,
                                                       ref simplification);

        Native.Interface.AudioGeometrySetObjectFlag(geometryHandle, ObjectFlags.ENABLED, isActiveAndEnabled);
        Native.Interface.AudioGeometrySetObjectFlag(geometryHandle, ObjectFlags.STATIC, gameObject.isStatic);

        // Clean up native handles
        foreach (var group in groups)
        {
            if (group.material != null)
                MetaXRAcousticMaterial.DestroyMaterialNativeHandle(group.material);
        }

        if (result == Success)
        {
            // TODO: add an empty mat for
            var materials = new List<IMaterialDataProvider>();
            foreach (MeshMaterial m in gatherer.Meshes)
            {
                int added = 0;
                int subMeshCount = m.mesh.subMeshCount;
                int numRealMaterials = m.meshMaterials == null ? 0 : m.meshMaterials.Length;
                if (numRealMaterials != 0)
                {
                    int amountToAdd = Mathf.Min(numRealMaterials, subMeshCount);
                    for (added = 0; added < amountToAdd; ++added)
                        materials.Add(m.meshMaterials[added]);

                    // splat the last material on remaining submeshes
                    for (added = amountToAdd; added < subMeshCount; ++added)
                        materials.Add(m.meshMaterials[numRealMaterials - 1]);
                }
                else
                {
                    for (int i = 0; i < subMeshCount; ++i)
                        materials.Add(null); // default material
                }
            }

            foreach (TerrainMaterial t in gatherer.Terrains)
            {
                if (t.terrainMaterials != null && t.terrainMaterials.Length != 0)
                    materials.AddRange(t.terrainMaterials);
            }

#if UNITY_EDITOR
            gizmoMaterialMapping = materials.ToArray();
            UpdateGizmoMesh(geometryHandle);
            HierarchyHash = ComputeHash();
#endif

            return true;
        }
        else
        {
            return false;
        }
    }

    private static bool uploadMeshFilter(List<Vector3> tempVertices, List<int> tempIndices, MeshGroup[] groups, float[] vertices, int[] indices,
        ref int vertexOffset, ref int indexOffset, ref int groupOffset, Mesh mesh, IMaterialDataProvider[] materials, Matrix4x4 matrix)
    {
        // Get the mesh vertices.
        tempVertices.Clear();
        mesh.GetVertices(tempVertices);

        // Copy the Vector3 vertices into a packed array of floats for the API.
        int meshVertexCount = tempVertices.Count;
        for (int i = 0; i < meshVertexCount; i++)
        {
            // Transform into the parent space.
            Vector3 v = matrix.MultiplyPoint3x4(tempVertices[i]);
            int offset = (vertexOffset + i) * 3;
            vertices[offset + 0] = v.x;
            vertices[offset + 1] = v.y;
            vertices[offset + 2] = v.z;
        }

        // Copy the data for each submesh.
        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            MeshTopology topology = mesh.GetTopology(i);

            if (topology == MeshTopology.Triangles || topology == MeshTopology.Quads)
            {
                // Get the submesh indices.
                tempIndices.Clear();
                mesh.GetIndices(tempIndices, i);
                int subMeshIndexCount = tempIndices.Count;

                // Copy and adjust the indices.
                for (int j = 0; j < subMeshIndexCount; j++)
                    indices[indexOffset + j] = tempIndices[j] + vertexOffset;

                // Initialize the group.
                if (topology == MeshTopology.Triangles)
                {
                    groups[groupOffset + i].faceType = FaceType.TRIANGLES;
                    groups[groupOffset + i].faceCount = (UIntPtr)(subMeshIndexCount / 3);
                }
                else if (topology == MeshTopology.Quads)
                {
                    groups[groupOffset + i].faceType = FaceType.QUADS;
                    groups[groupOffset + i].faceCount = (UIntPtr)(subMeshIndexCount / 4);
                }

                groups[groupOffset + i].indexOffset = (UIntPtr)indexOffset;

                if (materials != null && materials.Length != 0)
                {
                    int matIndex = i;
                    if (matIndex >= materials.Length)
                        matIndex = materials.Length - 1;

                    groups[groupOffset + i].material = MetaXRAcousticMaterial.CreateMaterialNativeHandle(materials[matIndex].Data);
                }
                else
                {
                    groups[groupOffset + i].material = IntPtr.Zero;
                }

                indexOffset += subMeshIndexCount;
            }
        }

        vertexOffset += meshVertexCount;
        groupOffset += mesh.subMeshCount;

        return true;
    }

    private static void updateCountsForMesh(ref int totalVertexCount, ref uint totalIndexCount, ref int totalFaceCount, ref int totalMaterialCount, Mesh mesh)
    {
        totalMaterialCount += mesh.subMeshCount;
        totalVertexCount += mesh.vertexCount;

        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            MeshTopology topology = mesh.GetTopology(i);
            if (topology == MeshTopology.Triangles || topology == MeshTopology.Quads)
            {
                uint meshIndexCount = mesh.GetIndexCount(i);
                totalIndexCount += meshIndexCount;

                if (topology == MeshTopology.Triangles)
                    totalFaceCount += (int)meshIndexCount / 3;
                else if (topology == MeshTopology.Quads)
                    totalFaceCount += (int)meshIndexCount / 4;
            }
        }
    }

    internal bool GatherGeometryRuntime()
    {
        Debug.Log("Gathering geometry");

        if (!GatherGeometryInternal(geometryHandle, gameObject, gameObject.transform.worldToLocalMatrix, ignoreStatic: Application.isPlaying, out int ignoredMeshCount))
            return false;

        if (ignoredMeshCount != 0)
        {
            Debug.LogWarning(
                $"Failed to upload meshes, {ignoredMeshCount} static meshes ignored. Turn on \"File Enabled\" to process static meshes offline", gameObject);
        }

        return true;
    }

#if UNITY_EDITOR
    internal void FixPathCaseMismatch()
    {
        string caseSensitivePath = MetaXRAudioUtils.GetCaseSensitivePathForFile(AbsoluteFilePath);
        if (AbsoluteFilePath != caseSensitivePath)
        {
            int trim = Application.dataPath.Length + 1;
            Debug.LogWarning($"File path case mismatch detected!\n old: {AbsoluteFilePath}\n new: {caseSensitivePath}");
            AbsoluteFilePath = caseSensitivePath.Replace('\\', '/');
        }
    }

    void OnValidate()
    {
        // GameObject is a non-instanced prefab, skip
        if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this))
            return;

        if (vertexCount == -1 && !UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode && !UnityEditor.BuildPipeline.isBuildingPlayer)
        {
            vertexCount = 0; // set numVertices so we don't spin here
            if (AUTO_VALIDATE)
            {
                StartInternal();
                DestroyInternal();
            }
        }

        FixPathCaseMismatch();
    }

    [NonSerialized]
    Mesh gizmoMesh;
    internal Mesh GizmoMesh => gizmoMesh; // exposed for tests

    internal Color DEFAULT_MATERIAL_COLOR1 => DEFAULT_MATERIAL_COLOR;

    private void UpdateGizmoMesh() => UpdateGizmoMesh(geometryHandle);

    readonly Color DEFAULT_MATERIAL_COLOR = Color.yellow;
    private void UpdateGizmoMeshColors()
    {
        if (gizmoMaterialMapping != null)
            materialColors = Array.ConvertAll(gizmoMaterialMapping.ToArray(), x => (x?.Data != null) ? x.Data.color : DEFAULT_MATERIAL_COLOR);

        Color[] vertexColors = new Color[gizmoVertexMaterialIndices.Length];
        for (int i = 0; i < gizmoVertexMaterialIndices.Length; i++)
        {
            if (gizmoVertexMaterialIndices[i] >= materialColors.Length || i >= vertexColors.Length || i >= gizmoVertexMaterialIndices.Length)
            {
                Debug.LogError($"out of bounds: i={i}/{vertexColors.Length} ({gizmoVertexMaterialIndices.Length}) - [{i}]={gizmoVertexMaterialIndices[i]}");
                return;
            }
            vertexColors[i] = materialColors[gizmoVertexMaterialIndices[i]];
        }
        gizmoMesh.colors = vertexColors;
    }

    private void UpdateGizmoMesh(IntPtr handle)
    {
        gizmoMesh = null;
        if (handle == IntPtr.Zero)
        {
            Debug.LogError("Unable to update Gizmo: Geometry not loaded", gameObject);
            return;
        }

        if (Native.Interface.AudioGeometryGetSimplifiedMesh(handle, out float[] vertices, out uint[] indices, out materialIndices) != 0)
            return;

        vertexCount = vertices.Length / 3;

        if (vertexCount != 0)
        {
            gizmoMesh = new Mesh();
            UnityEngine.Rendering.VertexAttributeDescriptor[] attributes = new UnityEngine.Rendering.VertexAttributeDescriptor[1] { new UnityEngine.Rendering.VertexAttributeDescriptor(UnityEngine.Rendering.VertexAttribute.Position, UnityEngine.Rendering.VertexAttributeFormat.Float32, 3) };
            int vertexCount = vertices.Length / 3;
            gizmoMesh.SetVertexBufferParams(vertexCount, attributes);
            gizmoMesh.SetVertexBufferData(vertices, 0, 0, vertices.Length);
            gizmoMesh.SetIndexBufferParams(indices.Length, UnityEngine.Rendering.IndexFormat.UInt32);
            gizmoMesh.SetIndexBufferData(indices, 0, 0, indices.Length, UnityEngine.Rendering.MeshUpdateFlags.Default);
            UnityEngine.Rendering.SubMeshDescriptor desc = new UnityEngine.Rendering.SubMeshDescriptor();
            desc.indexCount = indices.Length;
            desc.topology = MeshTopology.Triangles;
            desc.vertexCount = vertexCount;

            int triangleCount = indices.Length / 3;
            gizmoVertexMaterialIndices = new int[vertexCount];
            for (int i = 0; i < triangleCount; i++)
            {
                gizmoVertexMaterialIndices[indices[(i * 3) + 0]] = (int)materialIndices[i];
                gizmoVertexMaterialIndices[indices[(i * 3) + 1]] = (int)materialIndices[i];
                gizmoVertexMaterialIndices[indices[(i * 3) + 2]] = (int)materialIndices[i];
            }

            UpdateGizmoMeshColors();

            gizmoMesh.SetSubMesh(0, desc);
            gizmoMesh.RecalculateNormals();

            Debug.Log($"Simplified mesh with {vertexCount} vertices", gameObject);
        }
    }

    /// Draw the editor debug view of the control.
    [UnityEditor.DrawGizmo(UnityEditor.GizmoType.NotInSelectionHierarchy | UnityEditor.GizmoType.Pickable | UnityEditor.GizmoType.Selected)]
    void OnDrawGizmos() => DrawDebug(false);

    /// Draw the editor debug view of the control when selected
    [UnityEditor.DrawGizmo(UnityEditor.GizmoType.NotInSelectionHierarchy | UnityEditor.GizmoType.Pickable | UnityEditor.GizmoType.Selected)]
    void OnDrawGizmosSelected() => DrawDebug(true);

    [UnityEditor.DrawGizmo(UnityEditor.GizmoType.NotInSelectionHierarchy | UnityEditor.GizmoType.Pickable | UnityEditor.GizmoType.Selected)]
    private void DrawDebug(bool selected)
    {
        if (gizmoMesh != null)
        {
            UpdateGizmoMeshColors();

            var color = new Color(1.0f, 1.0f, 1.0f, selected ? 0.5f : 0.1f);

            // Taken from: https://docs.unity3d.com/ScriptReference/GL.html
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            var mat = new Material(shader);
            mat.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            mat.SetInt("_ZWrite", 0);

            // X-Ray vision
            //mat.SetInt("_ZTest", 0);

            // Avoid Z-fighting
            mat.SetFloat("_ZBias", -1);

            mat.SetColor("_Color", color);
            mat.SetPass(0);

            if (Event.current.type != EventType.Repaint)
            {
                // skip drawing gizmo on mouse events to avoid messing up selection
                return;
            }

            GL.wireframe = true;
            Graphics.DrawMeshNow(gizmoMesh, transform.localToWorldMatrix);
            GL.wireframe = false;

            color.a *= 0.5f;
            mat.color = color;
            mat.SetPass(0);
            Graphics.DrawMeshNow(gizmoMesh, transform.localToWorldMatrix);
        }
    }

    class AgeChecker : ITransformVisitor
    {
        internal AgeChecker(DateTime timeStamp, bool useColliders)
        {
            this.timeStamp = timeStamp;
            this.useColliders = useColliders;
        }

        public System.Object visit(Transform transform, System.Object parentData)
        {
            var activeMaterials = transform.GetComponents<MetaXRAcousticMaterial>().Where(x => x.enabled);
            IMaterialDataProvider[] materials = Array.ConvertAll(activeMaterials.ToArray(), x => x);

            if ( useColliders )
            {
                MeshCollider[] meshColliders = transform.GetComponents<MeshCollider>();
                BoxCollider[] boxColliders = transform.GetComponents<BoxCollider>();

                foreach (MeshCollider mc in meshColliders)
                {
                    if( CheckMeshTime( mc.sharedMesh, transform.gameObject ) )
                    {
                        return null;
                    }
                }

                // Include transform in hash if it has any relevant components.
                if ( (meshColliders != null && meshColliders.Length > 0) || (boxColliders != null && boxColliders.Length > 0) ||
                    (materials != null && materials.Length > 0) )
                {
                    AppendTransformHash( ref hash, transform );
                }
            }
            else
            {
                MeshFilter[] meshes = transform.GetComponents<MeshFilter>();
                Terrain[] terrains = transform.GetComponents<Terrain>();

                foreach (MeshFilter meshFilter in meshes)
                {
                    if( CheckMeshTime( meshFilter.sharedMesh, transform.gameObject ) )
                    {
                        return null;
                    }
                }

                // Include transform in hash if it has any relevant components.
                if ( (meshes != null && meshes.Length > 0) || (terrains != null && terrains.Length > 0) ||
                    (materials != null && materials.Length > 0) )
                {
                    AppendTransformHash( ref hash, transform );
                }
            }
            return null;
        }

        private bool CheckMeshTime( Mesh mesh, GameObject obj )
        {
            if ( mesh != null )
            {
                string meshPath = UnityEditor.AssetDatabase.GetAssetPath(mesh);
                DateTime lastModified = System.IO.File.Exists(meshPath) ? System.IO.File.GetLastWriteTime(meshPath) : DateTime.MinValue;
#if META_XR_ACOUSTIC_INFO
                Debug.Log($"modified {lastModified}, last build {timeStamp}");
#endif
                if (lastModified > timeStamp)
                {
                    Debug.LogWarning($"Newer mesh file {meshPath}", obj);
                    isOlder = true; // mesh file changed
                    return true;
                }
            }
            return false;
        }

        readonly DateTime timeStamp;
        internal bool isOlder = false;
        readonly bool useColliders;
        internal Hash128 hash = new Hash128();
    }

    internal bool IsOlder(DateTime timeStamp)
    {
        AgeChecker ageChecker = new AgeChecker(timeStamp, useColliders);
        traverseMeshHierarchy(gameObject, IncludeChildMeshes, ExcludeTags, false, lodSelection, ageChecker);
        return ageChecker.isOlder || ageChecker.hash != HierarchyHash;
    }

    private static void AppendTransformHash( ref Hash128 hash, Transform transform )
    {
        hash.Append(transform.position.x);
        hash.Append(transform.position.y);
        hash.Append(transform.position.z);
        hash.Append(transform.rotation.w);
        hash.Append(transform.rotation.x);
        hash.Append(transform.rotation.y);
        hash.Append(transform.rotation.z);
        hash.Append(transform.localScale.x);
        hash.Append(transform.localScale.y);
        hash.Append(transform.localScale.z);
    }

    class HashAppender : ITransformVisitor
    {
        internal HashAppender(Hash128 hash, bool useColliders)
        {
            this._hash = hash;
            this.useColliders = useColliders;
        }

        public System.Object visit(Transform transform, System.Object parentData)
        {
            var activeMaterials = transform.GetComponents<MetaXRAcousticMaterial>().Where(x => x.enabled);
            IMaterialDataProvider[] materials = Array.ConvertAll(activeMaterials.ToArray(), x => x);
            if ( useColliders )
            {
                MeshCollider[] meshColliders = transform.GetComponents<MeshCollider>();
                BoxCollider[] boxColliders = transform.GetComponents<BoxCollider>();
                // Include transform in hash if it has any relevant components.
                if ( (meshColliders != null && meshColliders.Length > 0) || (boxColliders != null && boxColliders.Length > 0) ||
                    (materials != null && materials.Length > 0) )
                {
                    AppendTransformHash( ref _hash, transform );
                }
            }
            else
            {
                MeshFilter[] meshes = transform.GetComponents<MeshFilter>();
                Terrain[] terrains = transform.GetComponents<Terrain>();
                // Include transform in hash if it has any relevant components.
                if ( (meshes != null && meshes.Length > 0) || (terrains != null && terrains.Length > 0) ||
                    (materials != null && materials.Length > 0) )
                {
                    AppendTransformHash( ref _hash, transform );
                }
            }
            return null;
        }

        private Hash128 _hash;
        internal Hash128 Hash { get => _hash; }
        readonly bool useColliders;
    }

    internal void AppendHash(ref Hash128 hash)
    {
        HashAppender hashAppender = new HashAppender(hash,useColliders);
        traverseMeshHierarchy(gameObject, IncludeChildMeshes, ExcludeTags, false, lodSelection, hashAppender);
        hash = hashAppender.Hash;
    }

    internal Hash128 ComputeHash()
    {
        Hash128 hash = new Hash128();
        AppendHash( ref hash );
        return hash;
    }

    private static string GenerateFileName(Transform current)
    {
        if (current.parent == null)
            return current.gameObject.scene.name + "/" + current.name;

        return GenerateFileName(current.parent) + "-" + current.name;
    }

    string GenerateSuggestedPath()
    {
        string basePath = $"{MetaXRAcousticSettings.AcousticFileRootDir}/{GenerateFileName(transform)}";
        string modifier = "";
        int counter = 0;

        string suggestion = "";

        // avoid name collisions
        do
        {
            suggestion = $"{basePath}{modifier}.{FILE_EXTENSION}";
            modifier = "-" + counter;
            ++counter;

            // sanity check to prevent hang
            if (counter > 10000) {
                Debug.LogError("Unable to find suitable file name", gameObject);
                return "";
            }

        } while (System.IO.File.Exists(suggestion));

        return suggestion;
    }


    //***********************************************************************
    // WriteFile - Write the serialized mesh file.
    internal bool WriteFile()
    {
        // Create a temporary geometry.
        if (Native.Interface.CreateAudioGeometry(out IntPtr tempGeometryHandle) != Success)
        {
            Debug.LogError("Failed to create temp geometry handle", gameObject);
            return false;
        }

        // Upload the mesh geometry.
        if (!GatherGeometryEditor(tempGeometryHandle, gameObject, gameObject.transform.worldToLocalMatrix))
        {
            if (Native.Interface.DestroyAudioGeometry(tempGeometryHandle) != Success)
                Debug.LogError("Failed to destroy temp geometry handle", gameObject);

            return false;
        }


        if (!WriteFileInternal(tempGeometryHandle))
        {
            if (Native.Interface.DestroyAudioGeometry(tempGeometryHandle) != Success)
                Debug.LogError("Failed to destroy temp geometry handle", gameObject);

            return false;
        }

        // Destroy the geometry.
        if (Native.Interface.DestroyAudioGeometry(tempGeometryHandle) != Success)
        {
            Debug.LogError("Failed to destroy temp geometry handle", gameObject);
            return false;
        }

        return true;
    }

    internal bool WriteFileInternal(IntPtr handle)
    {
        if (string.IsNullOrEmpty(relativeFilePath))
        {
            if (string.IsNullOrEmpty(gameObject.scene.name))
            {
                Debug.LogError("Cannot autogenerate name scene hasn't been saved", gameObject);
                return false;
            }
            relativeFilePath = GenerateSuggestedPath();
            Debug.Log($"No file path specified, autogenerated: {relativeFilePath}", gameObject);
        }

        MetaXRAudioUtils.CreateDirectoryForFilePath(AbsoluteFilePath);

        bool shouldAdd = !File.Exists(AbsoluteFilePath);
        if (!shouldAdd && UnityEditor.VersionControl.Provider.isActive)
        {
            var checkout = UnityEditor.VersionControl.Provider.Checkout(AbsoluteFilePath, UnityEditor.VersionControl.CheckoutMode.Asset);
            checkout.Wait();
            Debug.Log($"Checkout {RelativeFilePath}: success = {checkout.success}");
        }

        // Write the mesh to a file.
        Debug.Log($"Writing mesh geometry: {AbsoluteFilePath}", gameObject);
        if (Native.Interface.AudioGeometryWriteMeshFile(handle, AbsoluteFilePath) != Success)
        {
            Debug.LogError($"Error writing mesh file {AbsoluteFilePath}", gameObject);
            return false;
        }

        if (shouldAdd && UnityEditor.VersionControl.Provider.isActive)
        {
            var checkout = UnityEditor.VersionControl.Provider.Checkout(AbsoluteFilePath, UnityEditor.VersionControl.CheckoutMode.Asset);
            checkout.Wait();
            Debug.Log($"Add {RelativeFilePath}: success = {checkout.success}");
        }

        UpdateGizmoMesh(handle);

        if (!FileEnabled)
        {
            Debug.LogWarning("File Successfully written but File Enabled is off, turn it on to use file");
        }

        return true;
    }
#endif

    //***********************************************************************
    // ReadFile - Read the serialized mesh file.
    internal bool ReadFile()
    {
        if (string.IsNullOrEmpty(AbsoluteFilePath))
        {
            Debug.LogError("Invalid mesh file path", gameObject);
            return false;
        }

        int index = AbsoluteFilePath.IndexOf("StreamingAssets");
        if (Application.isPlaying && index > 0)
        {
            string subPath = AbsoluteFilePath.Substring(index + 16);
            StartCoroutine(LoadGeometryAsync(subPath));
        }
        else
        {
            if (Native.Interface.AudioGeometryReadMeshFile(geometryHandle, AbsoluteFilePath) != Success)
            {
                Debug.LogError($"Error reading mesh file {AbsoluteFilePath}", gameObject);
                return false;
            }

            Native.Interface.AudioGeometrySetObjectFlag(geometryHandle, ObjectFlags.ENABLED, isActiveAndEnabled);
            ApplyTransform();
            Native.Interface.AudioGeometrySetObjectFlag(geometryHandle, ObjectFlags.STATIC, gameObject.isStatic);

#if UNITY_EDITOR
            UpdateGizmoMesh();
#endif
        }

        return true;
    }

    private IEnumerator LoadGeometryAsync(string relativePath)
    {
        string path = Application.streamingAssetsPath + "/" + relativePath;
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        path = "file://" + path;
#endif
        Debug.Log($"Loading Geometry {name} from StreamingAssets {path}", gameObject);

        float startTime = Time.realtimeSinceStartup;

        Profiler.BeginSample("MetaXRAcousticGeometry web request get");
        var unityWebRequest = UnityEngine.Networking.UnityWebRequest.Get(path);
        Profiler.EndSample();

        yield return unityWebRequest.SendWebRequest();
        if (!string.IsNullOrEmpty(unityWebRequest.error))
        {
            Debug.LogError($"web request: done={unityWebRequest.isDone}: {unityWebRequest.error}", gameObject);
        }

        float readTime = Time.realtimeSinceStartup;
        float readDuration = readTime - startTime;
        Debug.Log($"Geometry {name}, read time = {readDuration}", gameObject);

        LoadGeometryFromMemory(unityWebRequest.downloadHandler.nativeData);
    }

    async void LoadGeometryFromMemory(Unity.Collections.NativeArray<byte>.ReadOnly data)
    {
        if (data.Length == 0)
            return;

        float startTime = Time.realtimeSinceStartup;

        int result = -1;
        await Task.Run(() =>
        {
            unsafe
            {
                IntPtr ptr = (IntPtr)Unity.Collections.LowLevel.Unsafe.NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(data);
                lock (this)
                {
                    if (geometryHandle != IntPtr.Zero)
                    {
                        result = Native.Interface.AudioGeometryReadMeshMemory(geometryHandle, ptr, (UInt64)data.Length);
                        GC.KeepAlive(data);
                    }
                }
            }
        });

        if (result == Success)
        {
            float loadDuration = Time.realtimeSinceStartup - startTime;
            Debug.Log($"Sucessfully loaded Geometry {name}, load time = {loadDuration}", gameObject);

            Native.Interface.AudioGeometrySetObjectFlag(geometryHandle, ObjectFlags.ENABLED, isActiveAndEnabled);
            ApplyTransform();
            Native.Interface.AudioGeometrySetObjectFlag(geometryHandle, ObjectFlags.STATIC, gameObject.isStatic);

#if UNITY_EDITOR
            UpdateGizmoMesh();
#endif

            isLoaded = true;

            if (isActiveAndEnabled)
            {
                IncrementEnabledGeometryCount();
            }
        }
        else
        {
            Debug.Log($"Unable to read the geometry {name}", gameObject);
        }
    }

#if UNITY_EDITOR
    internal bool WriteToObj()
    {
        // Create a temporary geometry.
        if (Native.Interface.CreateAudioGeometry(out IntPtr tempGeometryHandle) != Success)
        {
            Debug.LogError("Failed to create temp geometry handle", gameObject);
            return false;
        }

        // Upload the mesh geometry.
        if (!GatherGeometryEditor(tempGeometryHandle, gameObject, gameObject.transform.worldToLocalMatrix))
        {
            return false;
        }

        // Write the mesh to a .obj file.
        if (Native.Interface.AudioGeometryWriteMeshFileObj(tempGeometryHandle, AbsoluteFilePath + ".obj") != Success)
        {
            Debug.LogError($"Error writing .obj file {AbsoluteFilePath}.obj", gameObject);
            return false;
        }

        // Destroy the geometry.
        if (Native.Interface.DestroyAudioGeometry(tempGeometryHandle) != Success)
        {
            Debug.LogError("Failed to destroy temp geometry handle", gameObject);
            return false;
        }

        return true;
    }
#endif
}
