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

using Meta.XR.Acoustics;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Meta.XR.Acoustics
{
    /***********************************************************************************/
    // ENUMS and STRUCTS
    /***********************************************************************************/
    /// \brief Enumeration of features that can be enabled/disabled.
    ///
    /// \see MetaXRAcousticNativeInterface#INativeInterface#SetAcousticModel
    public enum AcousticModel : int
    {
        Automatic = -1, ///< Automatically select highest quality (if geometry is set the propagation system will be active, otherwise if the callback is set dynamic room modeling is enabled, otherwise fallback to the static shoe box)
        None = 0, ///< Disable all acoustics features
        ShoeboxRoom = 1, ///< Room defined by RoomAcousticProperties (MetaXRRoomAcoustic components)
        AcousticRayTracing = 3, ///< Geometry, Material based propagation system (MetaXRAcoustic components)
    }

    [Flags]
    /// \brief Enumeration of features that can be enabled/disabled.
    ///
    /// \see MetaXRAudioNativeInterface#NativeInterface#SetEnabled
    public enum EnableFlagInternal : uint
    {
        NONE = 0, ///< Disable all features
        SIMPLE_ROOM_MODELING = 2, ///< Enable/disable simple room modeling globally. Default: disabled
        LATE_REVERBERATION = 3, ///< Late reverbervation, requires simple room modeling enabled. Default: disabled
        RANDOMIZE_REVERB = 4, ///< Randomize reverbs to diminish artifacts. Default: enabled
        PERFORMANCE_COUNTERS = 5, ///< Enable profiling. Default: disabled
        DIFFRACTION = 6, ///< Enable Diffraction. Default: disabled
    }

    /// \brief The type of mesh face that is used to define geometry.
    ///
    /// For all face types, the vertices should be provided such that they are in counter-clockwise
    /// order when the face is viewed from the front. The vertex order is used to determine the
    /// surface normal orientation.
    public enum FaceType : uint
    {
        TRIANGLES = 0, ///< The mesh faces are defined by Triangles
        QUADS ///< The mesh faces are defined by Quads
    }

    /// \brief The properties for audio materials. All properties are frequency dependent.
    public enum MaterialProperty : uint
    {
        ABSORPTION = 0, ///< The fraction of sound arriving at a surface that is absorbed by the material. This value is in the range 0 to 1, where 0 indicates a perfectly reflective material, and 1 indicates a perfectly absorptive material. Absorption is inversely related to the reverberation time, and has the strongest impact on the acoustics of an environment. The default absorption is 0.1.
        TRANSMISSION, ///< The fraction of sound arriving at a surface that is transmitted through the material. This value is in the range 0 to 1, where 0 indicates a material that is acoustically opaque, and 1 indicates a material that is acoustically transparent. To preserve energy in the simulation, the following condition must hold: (1 - absorption + transmission) <= 1 If this condition is not met, the transmission and absorption coefficients will be modified to enforce energy conservation. The default transmission is 0.
        SCATTERING ///< The fraction of sound arriving at a surface that is scattered. This property in the range 0 to 1 controls how diffuse the reflections are from a surface, where 0 indicates a perfectly specular reflection and 1 indicates a perfectly diffuse reflection. The default scattering is 0.5.
    }

    /// \brief A struct that defines a grouping of mesh faces and the material that should be applied to the faces.
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MeshGroup
    {
        public UIntPtr indexOffset; ///< The offset in the index buffer of the first index in the group.
        public UIntPtr faceCount; ///< The number of faces that this group uses from the index buffer. The number of bytes read from the index buffer for the group is determined by the formula: (faceCount)*(verticesPerFace)*(bytesPerIndex)
        [MarshalAs(UnmanagedType.U4)]
        public FaceType faceType; ///< The type of face that the group uses. This determines how many indices are needed to define a face.
        public IntPtr material; ///< A handle to the material that should be assigned to the group. If equal to 0/NULL/nullptr, a default material is used instead.
    }

    [Flags]
    /// \brief Flags describing the status an Acoustic Map.
    public enum AcousticMapStatus : uint
    {
        EMPTY = 0, ///< A status flag indicating the Map has no data (i.e. was just created).
        MAPPED = (1 << 0), ///< A status flag indicating the Map has been spatially mapped (but Map not necessarily computed).
        READY = (1 << 1) | MAPPED ///< A status flag indicating the Map is ready for use in a runtime simulation (i.e. has been mapped and computed).
    }

    [Flags]
    /// \brief The boolean flags for an Acoustic Map.
    public enum AcousticMapFlags : uint
    {
        NONE = 0, ///< The flag value when no flags are set.
        STATIC_ONLY = (1 << 0), ///< If set, only objects that have this flag set will be considered for the computation.
        NO_FLOATING = (1 << 1), ///< If set, the Map will not create points that are floating far above the floor (as determined by gravity vector).
        MAP_ONLY = (1 << 2), ///< If set, no data will be calculated. The scene will be mapped but not simulated.
        DIFFRACTION = (1 << 3) ///< If set, the Map will be preprocessed to support runtime diffraction.
    }

    [Flags]
    /// \brief The boolean flags for an audio object.
    public enum ObjectFlags : uint
    {
        EMPTY = 0, ///< If set, the object is not used within the simulation and will be ignored.
        ENABLED = (1 << 0), ///< If set, the object is used within the simulation and impacts the computed acoustics.
        STATIC = (1 << 1) ///< If set, the object is assumed to never move or change geometry. The context may use this flag as a hint to optimize the simulation.
    }

    [Flags]
    /// \brief Flags to describe how to render a mesh
    public enum MeshFlags : uint
    {
        NONE = 0, ///< This flag indicates none of the other mesh flags are enabled
        ENABLE_SIMPLIFICATION = (1 << 0), ///< Turning on Mesh Simplification will reduce the resource usage of geometry propagation at the cost of audio quality.
        ENABLE_DIFFRACTION = (1 << 1), ///< If diffraction is enabled, the geometry will support real-time diffraction. Note, if this is disabled and you try to turn on real time diffraction, you won't hear a difference as the geometry did not render to support geometry.
    }

    [StructLayout(LayoutKind.Sequential)]
    /// \brief A struct that describes how a mesh should be simplified.
    public struct MeshSimplification
    {
        public UIntPtr thisSize; /// The size of this structure. You must set this equal to sizeof(ovrAudioMeshSimplification) This will ensure version compatibility
        [MarshalAs(UnmanagedType.U4)]
        public MeshFlags flags; /// The mesh flags that describe how the mesh should be rendered
        public float unitScale; /// The local unit scale factor for the mesh (the factor that converts from mesh-local coordinates to coordinates in meters). The other length quantities in this struct are converted to mesh-local coordinates using this value.
        public float maxError; /// The maximum allowed error due to simplification, expressed as a distance in meters.
        public float minDiffractionEdgeAngle; /// The minimum angle (degrees) that there must be between two adjacent face normals for their edge to be marked as diffracting.
        public float minDiffractionEdgeLength; /// The minimum length in meters that an edge should have for it to be marked as diffracting.
        public float flagLength; /// The maximum distance in meters that a diffraction flag extends out from the edge.
        public UIntPtr threadCount; /// The number of threads that should be used for processing the mesh. A value of 0 means to use the same number of threads as CPUs. A value of 1 indicates that all work is done on the calling thread (no threads created). This number will be clamped to no more than the number of CPUs.
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]

    /// \brief A function pointer that provides information about the current progress on a long-running operation.
    ///
    /// This function will be called periodically by long-running operations to report progress to the GUI.
    /// \param[out] userData A pointer to any data the user would like to be accessible during the callback
    /// \param[out] progress The progress value is in the range [0,1] and indicates the approximate fraction of the operation that has been performed so far, and could be used to display a progress bar.
    /// \param[out] description The string is a NULL-terminated ASCII description of the current task.
    /// \return indicates whether or not to continue processing (non-zero -> continue, zero -> cancel).
    public delegate bool ProgressCallback(IntPtr userData, string description, float progress);

    [StructLayout(LayoutKind.Sequential)]
    /// \brief A struct containing callback functions used when computing an Acoustic Map.
    public struct SceneIRCallbacks
    {
        public IntPtr userData; /// A pointer to arbitrary user data that is passed into the callback functions.

        [MarshalAs(UnmanagedType.FunctionPtr)]
        public ProgressCallback progress; /// A pointer to function that reports the computation progress to a GUI.
    }

    [StructLayout(LayoutKind.Sequential)]
    /// \brief A struct that describes how an Acoustic Map is computed.
    public struct MapParameters
    {
        public UIntPtr thisSize; /// A value that should be set to the size in bytes of the ovrAudioSceneIRParameters structure. This is used for backward/forward compatibility. = sizeof(ovrAudioSceneIRParameters)
        public SceneIRCallbacks callbacks; /// A struct containing callback functions that receive notifications during computation.
        public UIntPtr threadCount; /// The number of threads that should be used for doing the computation. A value of 0 means to use the same number of threads as CPUs. A value of 1 indicates that all work is done on the calling thread (no threads created). This number will be clamped to no more than the number of CPUs.
        public UIntPtr reflectionCount; /// The number of early reflections that should be stored in the scene IR. Increasing this value increases the size of the IR data, as well as the quality.

        [MarshalAs(UnmanagedType.U4)]
        public AcousticMapFlags flags; /// Flags that describe how an Acoustic Map should be computed.

        public float minResolution; /// The minimum point placement resolution, expressed in meters. This determines the smallest spacing of points in the scene, as well as the smallest space that will be considered as part of the precomputation. This should be a little bit smaller than the smallest possible space that the listener can traverse. Note that decreasing this value doesn't necessarily result in more densely-sampled points - the number of points is more clostly related to the number of distinct acoustic spaces in the scene.
        public float maxResolution; /// The maximum point placement resolution, expressed in meters. This determines the largest spacing of points in the scene. Decreasing this value improves the quality for large open scenes, but also increases the precomputation time and storage required.

        public float headHeight; /// The typical height in meters of the listener's head above floor surfaces. This is used to determine where to vertically place probe points in the scene. The quality will be best for sources and listeners that are around this height.
        public float maxHeight; /// The maximum height in meters of a probe point above floor surfaces. Increase this value to include more points floating in the air.

        public float gravityVectorX; /// The X value in a 3D unit vector indicating the downward direction in the scene. This is used to cull probe points that are irrelevant because they are too high above the floor surface. In a Y-up world, this should be {0,-1,0}, while Z-up would be {0,0,-1}.
        public float gravityVectorY; /// The Y value in a 3D unit vector indicating the downward direction in the scene. This is used to cull probe points that are irrelevant because they are too high above the floor surface. In a Y-up world, this should be {0,-1,0}, while Z-up would be {0,0,-1}.
        public float gravityVectorZ; /// The Z value in a 3D unit vector indicating the downward direction in the scene. This is used to cull probe points that are irrelevant because they are too high above the floor surface. In a Y-up world, this should be {0,-1,0}, while Z-up would be {0,0,-1}.
    }

    /// \brief The properties of an audio control Zone.
    public enum ControlZoneProperty : uint
    {
        RT60 = 0, ///< A frequency-dependent property describing the reverberation time adjustment in the Zone. The value represents a signed additive adjustment to the simulated reverb time (units: seconds).
        REVERB_LEVEL ///< A frequency-dependent property describing the reverberation level adjustment in the Zone. The value represents a signed additive adjustment to the simulated reverb level (units: decibels).
    }
}

/// \brief Parent class that functions as a scope for all the code that wraps binary interfaces for the Unity, Wwise and FMOD plug-ins.
public class MetaXRAcousticNativeInterface
{
    static INativeInterface CachedInterface;

    /// \brief The current interface being used by the system.
    ///
    /// The first access of this field will trigger a scan for any Meta XR Audio plugin binaries, which are searched for in the following order (taking the first one found and halting any further search):
    /// 1. Meta XR Audio Plugin for Wwise
    /// 2. Meta XR Audio Plugin for FMOD
    /// 3. Meta XR Audio Plugin for Unity
    ///
    /// If none are found, Unity is assumed, but attempting to call any of the functions of the returned `NativeInterface` class result in undefined behavior.
    ///
    /// \see NativeInterface
    /// \see UnityNativeInterface
    /// \see WwisePluginInterface
    /// \see FMODPluginInterface
    public static INativeInterface Interface { get { if (CachedInterface == null) CachedInterface = FindInterface(); return CachedInterface; } }

    static INativeInterface FindInterface()
    {
        const int MINIMUM_SDK_VERSION = 92;
        try
        {
            IntPtr temp = WwisePluginInterface.getOrCreateGlobalOvrAudioContext();
            WwisePluginInterface.ovrAudio_GetVersion(out int major, out int minor, out int patch);
            if (minor < MINIMUM_SDK_VERSION)
            {
                Debug.LogError("Incompatible SDK version, update your MetaXRAudioWwise plugin");
                return new DummyInterface();
            }
            Debug.Log("Meta XR Audio Native Interface initialized with Wwise plugin");
            return new WwisePluginInterface();
        }
        catch (System.DllNotFoundException)
        {
            // this is fine
        }

        try
        {
            FMODPluginInterface.ovrAudio_GetPluginContext(out IntPtr temp);
            FMODPluginInterface.ovrAudio_GetVersion(out int major, out int minor, out int patch);
            if (minor < MINIMUM_SDK_VERSION)
            {
                Debug.LogError("Incompatible SDK version, update your MetaXRAudioFMOD plugin");
                return new DummyInterface();
            }
            Debug.Log("Meta XR Audio Native Interface initialized with FMOD plugin");
            return new FMODPluginInterface();
        }
        catch (System.DllNotFoundException)
        {
            // this is fine
        }

        try
        {
            UnityNativeInterface.ovrAudio_GetPluginContext(out IntPtr temp);
            UnityNativeInterface.ovrAudio_GetVersion(out int major, out int minor, out int patch);
            if (minor < MINIMUM_SDK_VERSION)
            {
                Debug.LogError("Incompatible SDK version, update your MetaXRAudioFMOD plugin");
                return new DummyInterface();
            }
            Debug.Log("Meta XR Audio Native Interface initialized with Unity plugin");
            return new UnityNativeInterface();
        }
        catch
        {
            Debug.LogError("Unable to located MetaXRAudio plugin for MetaXRAcoustics!\n" +
                "If you're using Unity audio make sure you have imported the MetaXRAudioUnity package\n" +
                "If you're using Wwise or FMOD make sure you have their Unity integration in your project and the MetaXRAudioWwise or MetaXRAudioFMOD plugins in correct location in the Assets folder");
        }

        return new DummyInterface();
    }

    /// \brief Enumeration of all the data types that could be used in the binary's interface.
    public enum ovrAudioScalarType : uint
    {
        Int8, ///< 8-bit signed integer [-128, 127]
        UInt8, ///< 8-bit unsigned integer [0, 255]
        Int16, ///< 16-bit signed integer [-32768, 32767]
        UInt16, ///< 16-bit unsigned integer [0, 65535]
        Int32, ///< 32-bit signed integer [-2147483648, 21474836]
        UInt32, ///< 32-bit unsigned integer [0, 4294967295]
        Int64, ///< 64-bit signed integer [-9223372036854775808, 9223372036854775807]
        UInt64, ///< 64-bit unsigned integer [0, 184467440737095516]
        Float16, ///< 16-bit floating point number with 1 bit sign, 10 bits exponent, and 15 bits mantissa
        Float32, ///< 32-bit floating point number with 1 bit sign, 23 bits exponent, and 23 bits mantissa
        Float64 ///< 64-bit floating point number with 1 bit sign, 52 bits exponent, and 52 bits mantissa
    }

    /// \brief Abstract parent class for all classes that wrap a binary's interface.
    /// \see UnityNativeInterface
    /// \see WwisePluginInterface
    /// \see FMODPluginInterface
    public interface INativeInterface
    {
        /***********************************************************************************/
        // Settings API
        /// \brief Explicitly set the reflection model
        ///
        /// \param Model[in] The reflection model to use (default is Automatic)
        ///
        /// \see AcousticModel
        int SetAcousticModel(AcousticModel model);

        /// \brief Reset the reverb and cut off reverb tails. It might be useful to call this
        /// function while changing scenes in a game, to prevent reverb from the previous scene
        /// from becoming audible when entering the new scene.
        ///
        /// \param feature specific property to query
        /// \param enabled bool specifying if the feature should be enabled
        /// \return Returns an ovrResult indicating success or failure
        int ResetReverb();

        /// \brief Turn on and off specific features of the library
        ///
        /// \param feature specific property to query
        /// \param enabled bool specifying if the feature should be enabled
        /// \return Returns an ovrResult indicating success or failure
        int SetEnabled(int feature, bool enabled);
        int SetEnabled(EnableFlagInternal feature, bool enabled);

        /***********************************************************************************/
        // Geometry API
        /// \brief Create a geometry object
        ///
        /// \param[out] geometry A pointer to the created geometry object
        /// \return Returns an ovrResult indicating success or failure
        int CreateAudioGeometry(out IntPtr geometry);

        /// \brief Destroy a geometry object
        ///
        /// \param[in] geometry A pointer to the geometry object that should be destroyed
        /// \return Returns an ovrResult indicating success or failure
        int DestroyAudioGeometry(IntPtr geometry);

        /// \brief Set the properties of a geometry object
        ///
        /// \param[in] geometry A pointer to the geometry object
        /// \param[in] flag The property about the geometry to be set
        /// \param[in] enabled bool specifying if the feature should be enabled
        /// \return Returns an ovrResult indicating success or failure
        int AudioGeometrySetObjectFlag(IntPtr geometry, ObjectFlags flag, bool enabled);

        /// \brief Bake mesh data into a geometry object
        ///
        /// \param[in] geometry A pointer to the geometry object where the mesh data is baked
        /// \param[in] vertices An array of vertices that describe the mesh
        /// \param[in] vertexCount The number of vertices in the vertices array
        /// \param[in] indices An array of integers that describe the mesh
        /// \param[in] indexCount The number of integers in the indices array
        /// \param[in] groups An array of mesh group data
        /// \param[in] groupCount The number of mesh groups in the groups array
        /// \return Returns an ovrResult indicating success or failure
        int AudioGeometryUploadMeshArrays(IntPtr geometry,
                                                        float[] vertices, int vertexCount,
                                                        int[] indices, int indexCount,
                                                        MeshGroup[] groups, int groupCount);

        /// \brief Bake mesh data into a geometry object with simplification flags
        ///
        /// \param[in] geometry A pointer to the geometry object where the mesh data is baked
        /// \param[in] vertices An array of vertices that describe the mesh
        /// \param[in] vertexCount The number of vertices in the vertices array
        /// \param[in] indices An array of integers that describe the mesh
        /// \param[in] indexCount The number of integers in the indices array
        /// \param[in] groups An array of mesh group data
        /// \param[in] groupCount The number of mesh groups in the groups array
        /// \param[in] simplification A struct containing flags for mesh simplification
        /// \return Returns an ovrResult indicating success or failure
        int AudioGeometryUploadSimplifiedMeshArrays(IntPtr geometry,
                                                        float[] vertices, int vertexCount,
                                                        int[] indices, int indexCount,
                                                        MeshGroup[] groups, int groupCount,
                                                        ref MeshSimplification simplification);

        /// \brief Set the transform of a geometry object
        ///
        /// \param[in] geometry A pointer to the geometry object
        /// \param[in] matrix A 4x4 matrix that contains the transform to apply
        /// \return Returns an ovrResult indicating success or failure
        int AudioGeometrySetTransform(IntPtr geometry, in Matrix4x4 matrix);

        /// \brief Get the transform of a geometry object
        ///
        /// \param[in] geometry A pointer to the geometry object
        /// \param[out] matrix A 4x4 matrix that contains the transform currently applied to the geometry
        /// \return Returns an ovrResult indicating success or failure
        int AudioGeometryGetTransform(IntPtr geometry, out float[] matrix4x4);

        /// \brief Write a geometry object into a .xrageo file
        ///
        /// \param[in] geometry A pointer to the geometry object to be written
        /// \param[in] filePath A fully qualified path to which the output file should be written
        /// \return Returns an ovrResult indicating success or failure
        int AudioGeometryWriteMeshFile(IntPtr geometry, string filePath);

        /// \brief Read in baked geometry from a .xrageo file into a geometry object
        ///
        /// \param[in] geometry A pointer to the geometry object where the file data will be read
        /// \param[in] filePath A fully qualified path to the .xrageo file that should be read
        /// \return Returns an ovrResult indicating success or failure
        int AudioGeometryReadMeshFile(IntPtr geometry, string filePath);

        /// \brief Read in geometry data as raw data input stored in an array. This allows alternate file reading methods
        ///
        /// \param[in] geometry A pointer to the geometry object where the file data will be read
        /// \param[in] data A pointer to the raw data that should be read
        /// \param[in] dataLength The number of bytes in the raw data
        /// \return Returns an ovrResult indicating success or failure
        int AudioGeometryReadMeshMemory(IntPtr geometry, IntPtr data, UInt64 dataLength);

        /// \brief Save the mesh data of a geometry object into an .obj file
        ///
        /// \param[in] geometry A pointer to the geometry object where the mesh data is baked
        /// \param[in] filePath A fully qualified path to which the output file should be written
        /// \return Returns an ovrResult indicating success or failure
        int AudioGeometryWriteMeshFileObj(IntPtr geometry, string filePath);

        /// \brief Get the mesh information that is currently saved to a geometry object
        ///
        /// \param[in] geometry A pointer to the geometry object where the mesh data is baked
        /// \param[in] vertices An array of vertices that describe the mesh
        /// \param[in] indices An array of integers that describe the mesh
        /// \param[in] materialIndices An array of integers that describe the materials
        /// \return Returns an ovrResult indicating success or failure
        int AudioGeometryGetSimplifiedMesh(IntPtr geometry, out float[] vertices, out uint[] indices, out uint[] materialIndices);

        /***********************************************************************************/
        // Material API
        /// \brief Get the value of a material property at the given frequency
        ///
        /// \param[in] material A pointer to the material object
        /// \param[in] property The material property to get
        /// \param[in] frequency The frequency to get the value at
        /// \param[out] value The returned material property at the given frequency
        /// \return Returns an ovrResult indicating success or failure
        int AudioMaterialGetFrequency(IntPtr material, MaterialProperty property, float frequency, out float value);

        /// \brief Create a new material object
        ///
        /// \param[out] material A pointer to the newly created material object
        /// \return Returns an ovrResult indicating success or failure
        int CreateAudioMaterial(out IntPtr material);

        /// \brief Destroy a material object
        ///
        /// \param[in] material A pointer to the material object to be destroyed
        /// \return Returns an ovrResult indicating success or failure
        int DestroyAudioMaterial(IntPtr material);

        /// \brief Set the value of a material property at the given frequency
        ///
        /// \param[in] material A pointer to the material object
        /// \param[in] property The material property to set
        /// \param[in] frequency The frequency to set the value at
        /// \param[in] value The desired value of the material property at the given frequency
        /// \return Returns an ovrResult indicating success or failure
        int AudioMaterialSetFrequency(IntPtr material, MaterialProperty property, float frequency, float value);

        /// \brief Remove all the set values for a material property
        ///
        /// \param[in] material A pointer to the material object
        /// \param[in] property The material property to get
        /// \return Returns an ovrResult indicating success or failure
        int AudioMaterialReset(IntPtr material, MaterialProperty property);
        /***********************************************************************************/
        // Acoustic Map API
        /// \brief Create a new Acoustic Map object
        ///
        /// \param[out] sceneIR A pointer to the newly created Acoustic Map object
        /// \return Returns an ovrResult indicating success or failure
        int CreateAudioSceneIR(out IntPtr sceneIR);

        /// \brief Destroy an Acoustic Map object
        ///
        /// \param[in] sceneIR A pointer to the Acoustic Map object to be destroyed
        /// \return Returns an ovrResult indicating success or failure
        int DestroyAudioSceneIR(IntPtr sceneIR);

        /// \brief Set an Acoustic Map object to be enabled or disabled
        ///
        /// \param[in] sceneIR A pointer to the Acoustic Map object
        /// \param[in] enabled bool specifying if the Acoustic Map should be enabled or disabled
        /// \return Returns an ovrResult indicating success or failure
        int AudioSceneIRSetEnabled(IntPtr sceneIR, bool enabled);

        /// \brief Get whether an Acoustic Map object is enabled or disabled
        ///
        /// \param[in] sceneIR A pointer to the Acoustic Map object
        /// \param[out] enabled bool specifying if the Acoustic Map is be enabled or disabled
        /// \return Returns an ovrResult indicating success or failure
        int AudioSceneIRGetEnabled(IntPtr sceneIR, out bool enabled);

        /// \brief Get the current computation status of an Acoustic Map object
        ///
        /// \param[in] sceneIR A pointer to the Acoustic Map object
        /// \param[out] status The current computation status of the Acoustic Map object
        /// \return Returns an ovrResult indicating success or failure
        ///
        /// \see AcousticMapStatus
        int AudioSceneIRGetStatus(IntPtr sceneIR, out AcousticMapStatus status);

        /// \brief Generate the default values for the parameters of an Acoustic Map object
        ///
        /// \param[out] parameters The initialized parameters with default values
        /// \return Returns an ovrResult indicating success or failure
        ///
        /// \see MapParameters
        int InitializeAudioSceneIRParameters(out MapParameters parameters);

        /// \brief Bake an Acoustic Map given particular parameters. It will use automatically generated points
        ///
        /// \param[in] sceneIR A pointer to the Acoustic Map object
        /// \param[out] parameters The parameters to be used for the baking
        /// \return Returns an ovrResult indicating success or failure
        ///
        /// \see MapParameters
        int AudioSceneIRCompute(IntPtr sceneIR, ref MapParameters parameters);

        /// \brief Bake an Acoustic Map given particular parameters. It will use custom points
        ///
        /// \param[in] sceneIR A pointer to the Acoustic Map object
        /// \param[in] points An array of custom points to be used for the baking
        /// \param[in] pointCount The number of points in the points array
        /// \param[out] parameters The parameters to be used for the baking
        /// \return Returns an ovrResult indicating success or failure
        ///
        /// \see MapParameters
        int AudioSceneIRComputeCustomPoints(IntPtr sceneIR,
            float[] points, UIntPtr pointCount, ref MapParameters parameters);

        /// \brief Get the number of points used in the Acoustic Map
        ///
        /// \param[in] sceneIR A pointer to the Acoustic Map object
        /// \param[out] pointCount The number of points in the Acoustic Map
        /// \return Returns an ovrResult indicating success or failure
        int AudioSceneIRGetPointCount(IntPtr sceneIR, out UIntPtr pointCount);

        /// \brief Get the exact points used in the Acoustic Map
        ///
        /// \param[in] sceneIR A pointer to the Acoustic Map object
        /// \param[out] points An array of the points used in the Acoustic Map
        /// \param[in] maxPointCount The maximum number of points that can be stored in the points array
        /// \return Returns an ovrResult indicating success or failure
        int AudioSceneIRGetPoints(IntPtr sceneIR, float[] points, UIntPtr maxPointCount);

        /// \brief Set the transform of an Acoustic Map object
        ///
        /// \param[in] sceneIR A pointer to the Acoustic Map object
        /// \param[in] matrix The transform to apply represented in a 4x4 matrix
        /// \return Returns an ovrResult indicating success or failure
        int AudioSceneIRSetTransform(IntPtr sceneIR, in Matrix4x4 matrix);

        /// \brief Get the transform currently applied to an Acoustic Map object
        ///
        /// \param[in] sceneIR A pointer to the Acoustic Map object
        /// \param[out] matrix4x4 The transform currently applied to the Acoustic Map represented in a 4x4 matrix
        /// \return Returns an ovrResult indicating success or failure
        int AudioSceneIRGetTransform(IntPtr sceneIR, out float[] matrix4x4);

        /// \brief Write an Acoustic Map into a .xramap file
        ///
        /// \param[in] sceneIR A pointer to the Acoustic Map object
        /// \param[in] filePath A fully qualified path to which the output file should be written
        /// \return Returns an ovrResult indicating success or failure
        int AudioSceneIRWriteFile(IntPtr sceneIR, string filePath);

        /// \brief Read a .xramap file into an Acoustic Map object
        ///
        /// \param[in] sceneIR A pointer to the Acoustic Map object
        /// \param[in] filePath A fully qualified path which the .xramap file that should be read from
        /// \return Returns an ovrResult indicating success or failure
        int AudioSceneIRReadFile(IntPtr sceneIR, string filePath);

        /// \brief Read a raw data array into an Acoustic Map object. Allows for alternate file reading methods
        ///
        /// \param[in] sceneIR A pointer to the Acoustic Map object
        /// \param[in] data A pointer to the raw data that should be read
        /// \param[in] dataLength The number of bytes in the raw data
        /// \return Returns an ovrResult indicating success or failure
        int AudioSceneIRReadMemory(IntPtr sceneIR, IntPtr data, UInt64 dataLength);

        /***********************************************************************************/
        // Control Zone API
        /// \brief Create a new Control Zone object
        ///
        /// \param[out] control A pointer to the Control Zone object
        /// \return Returns an ovrResult indicating success or failure
        int CreateControlZone(out IntPtr control);

        /// \brief Destroy a new Control Zone object
        ///
        /// \param[in] control A pointer to the Control Zone object
        /// \return Returns an ovrResult indicating success or failure
        int DestroyControlZone(IntPtr control);

        /// \brief Enable or Disable Control Zone object
        ///
        /// \param[in] control A pointer to the Control Zone object
        /// \param[in] enabled bool specifying if the Control Zone should be enabled or disabled
        /// \return Returns an ovrResult indicating success or failure
        int ControlZoneSetEnabled(IntPtr control, bool enabled);

        /// \brief Get whether a Control Zone object is enabled or disabled
        ///
        /// \param[in] control A pointer to the Control Zone object
        /// \param[out] enabled bool specifying if the Control Zone is enabled or disabled
        /// \return Returns an ovrResult indicating success or failure
        int ControlZoneGetEnabled(IntPtr control, out bool enabled);

        /// \brief Set the transform of a Control Zone object
        ///
        /// \param[in] control A pointer to the Control Zone object
        /// \param[in] matrix The transform to apply represented in a 4x4 matrix
        /// \return Returns an ovrResult indicating success or failure
        int ControlZoneSetTransform(IntPtr control, in Matrix4x4 matrix);

        /// \brief Get the transform currently applied to a Control Zone object
        ///
        /// \param[in] control A pointer to the Control Zone object
        /// \param[out] matrix4x4 The transform currently applied to the Control Zone represented in a 4x4 matrix
        /// \return Returns an ovrResult indicating success or failure
        int ControlZoneGetTransform(IntPtr control, out float[] matrix4x4);

        /// \brief Set the boundaries of a Control Zone object.
        ///
        /// \param[in] control A pointer to the Control Zone object
        /// \param[in] sizeX The width of the boundary
        /// \param[in] sizeY The height of the boundary
        /// \param[in] sizeZ The depth of the boundary
        /// \return Returns an ovrResult indicating success or failure
        int ControlZoneSetBox(IntPtr control, float sizeX, float sizeY, float sizeZ);

        /// \brief Get the boundaries of a Control Zone object.
        ///
        /// \param[in] control A pointer to the Control Zone object
        /// \param[out] sizeX The size of the boundary along the x axis
        /// \param[out] sizeY The size of the boundary along the y axis
        /// \param[out] sizeZ The size of the boundary along the z axis
        /// \return Returns an ovrResult indicating success or failure
        int ControlZoneGetBox(IntPtr control, out float sizeX, out float sizeY, out float sizeZ);

        /// \brief Set the distance to fade outward from the boundaries of a Control Zone object.
        ///
        /// \param[in] control A pointer to the Control Zone object
        /// \param[in] fadeX The amount of fade in the x direction
        /// \param[in] fadeY The amount of fade in the y direction
        /// \param[in] fadeZ The amount of fade in the z direction
        /// \return Returns an ovrResult indicating success or failure
        int ControlZoneSetFadeDistance(IntPtr control, float fadeX, float fadeY, float fadeZ);

        /// \brief Get the distance to fade outward from the boundaries of a Control Zone object.
        ///
        /// \param[in] control A pointer to the Control Zone object
        /// \param[out] fadeX The amount of fade in the x direction
        /// \param[out] fadeY The amount of fade in the y direction
        /// \param[out] fadeZ The amount of fade in the z direction
        /// \return Returns an ovrResult indicating success or failure
        int ControlZoneGetFadeDistance(IntPtr control, out float fadeX, out float fadeY, out float fadeZ);

        /// \brief Set a property of the Control Zone object for a particular frequency
        ///
        /// \param[in] control A pointer to the Control Zone object
        /// \param[in] property The property of the Control Zone to set
        /// \param[in] frequency The frequency to change the property at in Hz
        /// \param[in] value The new value of the property at frequency
        /// \return Returns an ovrResult indicating success or failure
        int ControlZoneSetFrequency(IntPtr control, ControlZoneProperty property, float frequency, float value);

        /// \brief Remove all values set for a particular property of a Control Zone object
        ///
        /// \param[in] control A pointer to the Control Zone object
        /// \param[in] property The property of the Control Zone to set
        int ControlZoneReset(IntPtr control, ControlZoneProperty property);
    }

    /***********************************************************************************/
    // UNITY NATIVE
    /***********************************************************************************/
    public class UnityNativeInterface : INativeInterface
    {
        /// \brief Name of the binary this interface wraps.
        ///
        /// This value can be used in  `[DllImport(binaryName)]` decorators and tells Unity what the binary name is for the Unity plug-in.
        public const string binaryName = "MetaXRAudioUnity";

        /***********************************************************************************/
        // Context API: Required to create internal context if it does not exist yet
        IntPtr context_ = IntPtr.Zero;
        IntPtr context
        {
            get
            {
                if (context_ == IntPtr.Zero)
                {
                    ovrAudio_GetPluginContext(out context_);
                    ovrAudio_GetVersion(out int major, out version, out int patch);
                }
                return context_;
            }
        }
        int version = 0;


        /// \brief Get the handle to the current context, creating one if necessary.
        ///
        /// Note that Unity's editor, player, and standalone builds will have different contexts.
        ///
        /// \param[out] context The returned handle to the context.
        /// \return Returns an ovrResult indicating success or failure.
        [DllImport(binaryName)]
        public static extern int ovrAudio_GetPluginContext(out IntPtr context);

        [DllImport(binaryName)]
        public static extern IntPtr ovrAudio_GetVersion(out int Major, out int Minor, out int Patch);

        /***********************************************************************************/
        // Settings API
        [DllImport(binaryName)]
        private static extern int ovrAudio_SetAcousticModel(IntPtr context, AcousticModel quality);
        public int SetAcousticModel(AcousticModel model)
        {
            return ovrAudio_SetAcousticModel(context, model);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_ResetSharedReverb(IntPtr context);
        public int ResetReverb()
        {
            return ovrAudio_ResetSharedReverb(context);
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_Enable(IntPtr context, int what, int enable);
        public int SetEnabled(int feature, bool enabled)
        {
            return ovrAudio_Enable(context, feature, enabled ? 1 : 0);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_Enable(IntPtr context, EnableFlagInternal what, int enable);
        public int SetEnabled(EnableFlagInternal feature, bool enabled)
        {
            return ovrAudio_Enable(context, feature, enabled ? 1 : 0);
        }

        /***********************************************************************************/
        // Geometry API
        [DllImport(binaryName)]
        private static extern int ovrAudio_CreateAudioGeometry(IntPtr context, out IntPtr geometry);
        public int CreateAudioGeometry(out IntPtr geometry)
        {
            return ovrAudio_CreateAudioGeometry(context, out geometry);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_DestroyAudioGeometry(IntPtr geometry);
        public int DestroyAudioGeometry(IntPtr geometry)
        {
            return ovrAudio_DestroyAudioGeometry(geometry);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioGeometrySetObjectFlag(IntPtr geometry, ObjectFlags flag, int enabled);
        public int AudioGeometrySetObjectFlag(IntPtr geometry, ObjectFlags flag, bool enabled)
        {
            if (version < 94)
                return -1;

            return ovrAudio_AudioGeometrySetObjectFlag(geometry, flag, enabled ? 1 : 0);
        }


        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioGeometryUploadMeshArrays(IntPtr geometry,
                                                                        float[] vertices, UIntPtr verticesBytesOffset, UIntPtr vertexCount, UIntPtr vertexStride, ovrAudioScalarType vertexType,
                                                                        int[] indices, UIntPtr indicesByteOffset, UIntPtr indexCount, ovrAudioScalarType indexType,
                                                                        MeshGroup[] groups, UIntPtr groupCount);

        public int AudioGeometryUploadMeshArrays(IntPtr geometry,
                                                        float[] vertices, int vertexCount,
                                                        int[] indices, int indexCount,
                                                        MeshGroup[] groups, int groupCount)
        {
            return ovrAudio_AudioGeometryUploadMeshArrays(geometry,
                vertices, UIntPtr.Zero, (UIntPtr)vertexCount, UIntPtr.Zero, ovrAudioScalarType.Float32,
                indices, UIntPtr.Zero, (UIntPtr)indexCount, ovrAudioScalarType.UInt32,
                groups, (UIntPtr)groupCount);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioGeometryUploadSimplifiedMeshArrays(IntPtr geometry,
                                                                        float[] vertices, UIntPtr verticesBytesOffset, UIntPtr vertexCount, UIntPtr vertexStride, ovrAudioScalarType vertexType,
                                                                        int[] indices, UIntPtr indicesByteOffset, UIntPtr indexCount, ovrAudioScalarType indexType,
                                                                        MeshGroup[] groups, UIntPtr groupCount,
                                                                        ref MeshSimplification simplification);

        public int AudioGeometryUploadSimplifiedMeshArrays(IntPtr geometry,
                                                        float[] vertices, int vertexCount,
                                                        int[] indices, int indexCount,
                                                        MeshGroup[] groups, int groupCount,
                                                        ref MeshSimplification simplification)
        {
            return ovrAudio_AudioGeometryUploadSimplifiedMeshArrays(geometry,
                vertices, UIntPtr.Zero, (UIntPtr)vertexCount, UIntPtr.Zero, ovrAudioScalarType.Float32,
                indices, UIntPtr.Zero, (UIntPtr)indexCount, ovrAudioScalarType.UInt32,
                groups, (UIntPtr)groupCount, ref simplification);
        }

        [DllImport(binaryName)]
        private static extern unsafe int ovrAudio_AudioGeometrySetTransform(IntPtr geometry, float* matrix4x4);
        public int AudioGeometrySetTransform(IntPtr geometry, in Matrix4x4 matrix)
        {
            unsafe
            {
                float* nativeMatrixCopy = stackalloc float[16];

                // Note: flip Z to convert from left-handed (+Z forward) to right-handed (+Z backward)
                nativeMatrixCopy[0] = matrix.m00;
                nativeMatrixCopy[1] = matrix.m10;
                nativeMatrixCopy[2] = -matrix.m20;
                nativeMatrixCopy[3] = matrix.m30;
                nativeMatrixCopy[4] = matrix.m01;
                nativeMatrixCopy[5] = matrix.m11;
                nativeMatrixCopy[6] = -matrix.m21;
                nativeMatrixCopy[7] = matrix.m31;
                nativeMatrixCopy[8] = matrix.m02;
                nativeMatrixCopy[9] = matrix.m12;
                nativeMatrixCopy[10] = -matrix.m22;
                nativeMatrixCopy[11] = matrix.m32;
                nativeMatrixCopy[12] = matrix.m03;
                nativeMatrixCopy[13] = matrix.m13;
                nativeMatrixCopy[14] = -matrix.m23;
                nativeMatrixCopy[15] = matrix.m33;

                return ovrAudio_AudioGeometrySetTransform(geometry, nativeMatrixCopy);
            }
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioGeometryGetTransform(IntPtr geometry, out float[] matrix4x4);
        public int AudioGeometryGetTransform(IntPtr geometry, out float[] matrix4x4)
        {
            return ovrAudio_AudioGeometryGetTransform(geometry, out matrix4x4);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioGeometryWriteMeshFile(IntPtr geometry, string filePath);
        public int AudioGeometryWriteMeshFile(IntPtr geometry, string filePath)
        {
            return ovrAudio_AudioGeometryWriteMeshFile(geometry, filePath);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioGeometryReadMeshFile(IntPtr geometry, string filePath);
        public int AudioGeometryReadMeshFile(IntPtr geometry, string filePath)
        {
            return ovrAudio_AudioGeometryReadMeshFile(geometry, filePath);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioGeometryReadMeshMemory(IntPtr geometry, IntPtr data, UInt64 dataLength);
        public int AudioGeometryReadMeshMemory(IntPtr geometry, IntPtr data, UInt64 dataLength)
        {
            return ovrAudio_AudioGeometryReadMeshMemory(geometry, data, dataLength);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioGeometryWriteMeshFileObj(IntPtr geometry, string filePath);
        public int AudioGeometryWriteMeshFileObj(IntPtr geometry, string filePath)
        {
            return ovrAudio_AudioGeometryWriteMeshFileObj(geometry, filePath);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioGeometryGetSimplifiedMeshWithMaterials(IntPtr geometry, IntPtr unused1, out uint numVertices, IntPtr unused2, IntPtr unused3, out uint numTriangles);

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioGeometryGetSimplifiedMeshWithMaterials(IntPtr geometry, float[] vertices, ref uint numVertices, uint[] indices, uint[] materialIndices, ref uint numTriangles);

        public int AudioGeometryGetSimplifiedMesh(IntPtr geometry, out float[] vertices, out uint[] indices, out uint[] materialIndices)
        {
            int result = ovrAudio_AudioGeometryGetSimplifiedMeshWithMaterials(geometry, IntPtr.Zero, out uint numVertices, IntPtr.Zero, IntPtr.Zero, out uint numTriangles);
            if (result != 0)
            {
                Debug.LogError("unexpected error getting simplified mesh array sizes");
                vertices = null;
                indices = null;
                materialIndices = null;
                return result;
            }

            vertices = new float[numVertices * 3];
            indices = new uint[numTriangles * 3];
            materialIndices = new uint[numTriangles];
            return ovrAudio_AudioGeometryGetSimplifiedMeshWithMaterials(geometry, vertices, ref numVertices, indices, materialIndices, ref numTriangles);
        }

        /***********************************************************************************/
        // Material API
        [DllImport(binaryName)]
        private static extern int ovrAudio_CreateAudioMaterial(IntPtr context, out IntPtr material);
        public int CreateAudioMaterial(out IntPtr material)
        {
            return ovrAudio_CreateAudioMaterial(context, out material);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_DestroyAudioMaterial(IntPtr material);
        public int DestroyAudioMaterial(IntPtr material)
        {
            return ovrAudio_DestroyAudioMaterial(material);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioMaterialSetFrequency(IntPtr material, MaterialProperty property, float frequency, float value);
        public int AudioMaterialSetFrequency(IntPtr material, MaterialProperty property, float frequency, float value)
        {
            return ovrAudio_AudioMaterialSetFrequency(material, property, frequency, value);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioMaterialGetFrequency(IntPtr material, MaterialProperty property, float frequency, out float value);
        public int AudioMaterialGetFrequency(IntPtr material, MaterialProperty property, float frequency, out float value)
        {
            return ovrAudio_AudioMaterialGetFrequency(material, property, frequency, out value);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioMaterialReset(IntPtr material, MaterialProperty property);
        public int AudioMaterialReset(IntPtr material, MaterialProperty property)
        {
            return ovrAudio_AudioMaterialReset(material, property);
        }
        /***********************************************************************************/
        // Acoustic Map API
        [DllImport(binaryName)]
        private static extern int ovrAudio_CreateAudioSceneIR(IntPtr context, out IntPtr sceneIR);
        public int CreateAudioSceneIR(out IntPtr sceneIR)
        {
            return ovrAudio_CreateAudioSceneIR(context, out sceneIR);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_DestroyAudioSceneIR(IntPtr sceneIR);
        public int DestroyAudioSceneIR(IntPtr sceneIR)
        {
            return ovrAudio_DestroyAudioSceneIR(sceneIR);
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioSceneIRSetEnabled(IntPtr sceneIR, int enabled);
        public int AudioSceneIRSetEnabled(IntPtr sceneIR, bool enabled)
        {
            return ovrAudio_AudioSceneIRSetEnabled(sceneIR, enabled ? 1 : 0);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioSceneIRGetEnabled(IntPtr sceneIR, out int enabled);
        public int AudioSceneIRGetEnabled(IntPtr sceneIR, out bool enabled)
        {
            int iEnabled;
            int res = ovrAudio_AudioSceneIRGetEnabled(sceneIR, out iEnabled);
            enabled = iEnabled != 0;
            return res;
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioSceneIRGetStatus(IntPtr sceneIR, out AcousticMapStatus status);
        public int AudioSceneIRGetStatus(IntPtr sceneIR, out AcousticMapStatus status)
        {
            return ovrAudio_AudioSceneIRGetStatus(sceneIR, out status);
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_InitializeAudioSceneIRParameters(out MapParameters parameters);
        public int InitializeAudioSceneIRParameters(out MapParameters parameters)
        {
            return ovrAudio_InitializeAudioSceneIRParameters(out parameters);
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioSceneIRCompute(IntPtr sceneIR, ref MapParameters parameters);
        public int AudioSceneIRCompute(IntPtr sceneIR, ref MapParameters parameters)
        {
            return ovrAudio_AudioSceneIRCompute(sceneIR, ref parameters);
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioSceneIRComputeCustomPoints(IntPtr sceneIR,
            float[] points, UIntPtr pointCount, ref MapParameters parameters);
        public int AudioSceneIRComputeCustomPoints(IntPtr sceneIR,
            float[] points, UIntPtr pointCount, ref MapParameters parameters)
        {
            return ovrAudio_AudioSceneIRComputeCustomPoints(sceneIR, points, pointCount, ref parameters);
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioSceneIRGetPointCount(IntPtr sceneIR, out UIntPtr pointCount);
        public int AudioSceneIRGetPointCount(IntPtr sceneIR, out UIntPtr pointCount)
        {
            return ovrAudio_AudioSceneIRGetPointCount(sceneIR, out pointCount);
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioSceneIRGetPoints(IntPtr sceneIR, float[] points, UIntPtr maxPointCount);
        public int AudioSceneIRGetPoints(IntPtr sceneIR, float[] points, UIntPtr maxPointCount)
        {
            return ovrAudio_AudioSceneIRGetPoints(sceneIR, points, maxPointCount);
        }

        [DllImport(binaryName)]
        private static extern unsafe int ovrAudio_AudioSceneIRSetTransform(IntPtr sceneIR, float* matrix4x4);
        public int AudioSceneIRSetTransform(IntPtr sceneIR, in Matrix4x4 matrix)
        {
            unsafe
            {
                float* nativeMatrixCopy = stackalloc float[16];

                // Note: flip Z to convert from left-handed (+Z forward) to right-handed (+Z backward)
                nativeMatrixCopy[0] = matrix.m00;
                nativeMatrixCopy[1] = matrix.m10;
                nativeMatrixCopy[2] = -matrix.m20;
                nativeMatrixCopy[3] = matrix.m30;
                nativeMatrixCopy[4] = matrix.m01;
                nativeMatrixCopy[5] = matrix.m11;
                nativeMatrixCopy[6] = -matrix.m21;
                nativeMatrixCopy[7] = matrix.m31;
                nativeMatrixCopy[8] = matrix.m02;
                nativeMatrixCopy[9] = matrix.m12;
                nativeMatrixCopy[10] = -matrix.m22;
                nativeMatrixCopy[11] = matrix.m32;
                nativeMatrixCopy[12] = matrix.m03;
                nativeMatrixCopy[13] = matrix.m13;
                nativeMatrixCopy[14] = -matrix.m23;
                nativeMatrixCopy[15] = matrix.m33;

                return ovrAudio_AudioSceneIRSetTransform(sceneIR, nativeMatrixCopy);
            }
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioSceneIRGetTransform(IntPtr sceneIR, out float[] matrix4x4);
        public int AudioSceneIRGetTransform(IntPtr sceneIR, out float[] matrix4x4)
        {
            return ovrAudio_AudioSceneIRGetTransform(sceneIR, out matrix4x4);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioSceneIRWriteFile(IntPtr sceneIR, string filePath);
        public int AudioSceneIRWriteFile(IntPtr sceneIR, string filePath)
        {
            return ovrAudio_AudioSceneIRWriteFile(sceneIR, filePath);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioSceneIRReadFile(IntPtr sceneIR, string filePath);
        public int AudioSceneIRReadFile(IntPtr sceneIR, string filePath)
        {
            return ovrAudio_AudioSceneIRReadFile(sceneIR, filePath);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioSceneIRReadMemory(IntPtr sceneIR, IntPtr data, UInt64 dataLength);
        public int AudioSceneIRReadMemory(IntPtr sceneIR, IntPtr data, UInt64 dataLength)
        {
            return ovrAudio_AudioSceneIRReadMemory(sceneIR, data, dataLength);
        }

        /***********************************************************************************/
        // Control Zone API
        [DllImport(binaryName)]
        private static extern int ovrAudio_CreateControlZone(IntPtr context, out IntPtr control);
        [DllImport(binaryName)]
        private static extern int ovrAudio_CreateControlVolume(IntPtr context, out IntPtr control);
        public int CreateControlZone(out IntPtr control)
        {
            try
            {
                return ovrAudio_CreateControlZone(context, out control);
            }
            catch
            {
                // Hack for v60 compatibility
                return ovrAudio_CreateControlVolume(context, out control);
            }
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_DestroyControlZone(IntPtr control);
        [DllImport(binaryName)]
        private static extern int ovrAudio_DestroyControlVolume(IntPtr control);
        public int DestroyControlZone(IntPtr control)
        {
            try
            {
                return ovrAudio_DestroyControlZone(control);
            }
            catch
            {
                // Hack for v60 compatibility
                return ovrAudio_DestroyControlVolume(control);
            }
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlZoneSetEnabled(IntPtr control, int enabled);
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlVolumeSetEnabled(IntPtr control, int enabled);
        public int ControlZoneSetEnabled(IntPtr control, bool enabled)
        {
            try
            {
                return ovrAudio_ControlZoneSetEnabled(control, enabled ? 1 : 0);
            }
            catch
            {
                // Hack for v60 compatibility
                return ovrAudio_ControlVolumeSetEnabled(control, enabled ? 1 : 0);
            }
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlZoneGetEnabled(IntPtr control, out int enabled);
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlVolumeGetEnabled(IntPtr control, out int enabled);
        public int ControlZoneGetEnabled(IntPtr control, out bool enabled)
        {
            int enabledInt = 0;
            int result;

            try
            {
                result = ovrAudio_ControlZoneGetEnabled(control, out enabledInt);
            }
            catch
            {
                // Hack for v60 compatibility
                result = ovrAudio_ControlVolumeGetEnabled(control, out enabledInt);
            }
            enabled = enabledInt != 0;
            return result;
        }

        [DllImport(binaryName)]
        private static extern unsafe int ovrAudio_ControlZoneSetTransform(IntPtr control, float* matrix4x4);
        [DllImport(binaryName)]
        private static extern unsafe int ovrAudio_ControlVolumeSetTransform(IntPtr control, float* matrix4x4);
        public int ControlZoneSetTransform(IntPtr control, in Matrix4x4 matrix)
        {
            unsafe
            {
                float* nativeMatrixCopy = stackalloc float[16];

                // Note: flip Z to convert from left-handed (+Z forward) to right-handed (+Z backward)
                nativeMatrixCopy[0] = matrix.m00;
                nativeMatrixCopy[1] = matrix.m10;
                nativeMatrixCopy[2] = -matrix.m20;
                nativeMatrixCopy[3] = matrix.m30;
                nativeMatrixCopy[4] = matrix.m01;
                nativeMatrixCopy[5] = matrix.m11;
                nativeMatrixCopy[6] = -matrix.m21;
                nativeMatrixCopy[7] = matrix.m31;
                nativeMatrixCopy[8] = matrix.m02;
                nativeMatrixCopy[9] = matrix.m12;
                nativeMatrixCopy[10] = -matrix.m22;
                nativeMatrixCopy[11] = matrix.m32;
                nativeMatrixCopy[12] = matrix.m03;
                nativeMatrixCopy[13] = matrix.m13;
                nativeMatrixCopy[14] = -matrix.m23;
                nativeMatrixCopy[15] = matrix.m33;
                try
                {
                    return ovrAudio_ControlZoneSetTransform(control, nativeMatrixCopy);
                }
                catch
                {
                    // Hack for v60 compatibility
                    return ovrAudio_ControlVolumeSetTransform(control, nativeMatrixCopy);
                }
            }
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlZoneGetTransform(IntPtr control, out float[] matrix4x4);
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlVolumeGetTransform(IntPtr control, out float[] matrix4x4);
        public int ControlZoneGetTransform(IntPtr control, out float[] matrix4x4)
        {
            try
            {
                return ovrAudio_ControlZoneGetTransform(control, out matrix4x4);
            }
            catch
            {
                // Hack for v60 compatibility
                return ovrAudio_ControlVolumeGetTransform(control, out matrix4x4);
            }
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlZoneSetBox(IntPtr control, float sizeX, float sizeY, float sizeZ);
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlVolumeSetBox(IntPtr control, float sizeX, float sizeY, float sizeZ);
        public int ControlZoneSetBox(IntPtr control, float sizeX, float sizeY, float sizeZ)
        {
            try
            {
                return ovrAudio_ControlZoneSetBox(control, sizeX, sizeY, sizeZ);
            }
            catch
            {
                // Hack for v60 compatibility
                return ovrAudio_ControlVolumeSetBox(control, sizeX, sizeY, sizeZ);
            }
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlZoneGetBox(IntPtr control, out float sizeX, out float sizeY, out float sizeZ);
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlVolumeGetBox(IntPtr control, out float sizeX, out float sizeY, out float sizeZ);
        public int ControlZoneGetBox(IntPtr control, out float sizeX, out float sizeY, out float sizeZ)
        {
            try
            {
                return ovrAudio_ControlZoneGetBox(control, out sizeX, out sizeY, out sizeZ);
            }
            catch
            {
                // Hack for v60 compatibility
                return ovrAudio_ControlVolumeGetBox(control, out sizeX, out sizeY, out sizeZ);
            }
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlZoneSetFadeDistance(IntPtr control, float fadeX, float fadeY, float fadeZ);
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlVolumeSetFadeDistance(IntPtr control, float fadeX, float fadeY, float fadeZ);
        public int ControlZoneSetFadeDistance(IntPtr control, float fadeX, float fadeY, float fadeZ)
        {
            try
            {
                return ovrAudio_ControlZoneSetFadeDistance(control, fadeX, fadeY, fadeZ);
            }
            catch
            {
                // Hack for v60 compatibility
                return ovrAudio_ControlVolumeSetFadeDistance(control, fadeX, fadeY, fadeZ);
            }
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlZoneGetFadeDistance(IntPtr control, out float fadeX, out float fadeY, out float fadeZ);
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlVolumeGetFadeDistance(IntPtr control, out float fadeX, out float fadeY, out float fadeZ);
        public int ControlZoneGetFadeDistance(IntPtr control, out float fadeX, out float fadeY, out float fadeZ)
        {
            try
            {
                return ovrAudio_ControlZoneGetFadeDistance(control, out fadeX, out fadeY, out fadeZ);
            }
            catch
            {
                // Hack for v60 compatibility
                return ovrAudio_ControlVolumeGetFadeDistance(control, out fadeX, out fadeY, out fadeZ);
            }
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlZoneSetFrequency(IntPtr control, ControlZoneProperty property, float frequency, float value);
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlVolumeSetFrequency(IntPtr control, ControlZoneProperty property, float frequency, float value);
        public int ControlZoneSetFrequency(IntPtr control, ControlZoneProperty property, float frequency, float value)
        {
            try
            {
                return ovrAudio_ControlZoneSetFrequency(control, property, frequency, value);
            }
            catch
            {
                // Hack for v60 compatibility
                return ovrAudio_ControlVolumeSetFrequency(control, property, frequency, value);
            }
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlZoneReset(IntPtr control, ControlZoneProperty property);
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlVolumeReset(IntPtr control, ControlZoneProperty property);
        public int ControlZoneReset(IntPtr control, ControlZoneProperty property)
        {
            try
            {
                return ovrAudio_ControlZoneReset(control, property);
            }
            catch
            {
                // Hack for v60 compatibility
                return ovrAudio_ControlVolumeReset(control, property);
            }
        }
    }

    /***********************************************************************************/
    // WWISE
    /***********************************************************************************/
    public class WwisePluginInterface : INativeInterface
    {
        /// \brief Name of the binary this interface wraps.
        ///
        /// This value can be used in  `[DllImport(binaryName)]` decorators and tells Unity what the binary name is for the Wwise plug-in.
        public const string binaryName = "MetaXRAudioWwise";
        /***********************************************************************************/
        // Context API: Required to create internal context if it does not exist yet
        IntPtr context_ = IntPtr.Zero;
        int version;

        IntPtr context
        {
            get
            {
                if (context_ == IntPtr.Zero)
                {
                    context_ = getOrCreateGlobalOvrAudioContext();
                    ovrAudio_GetVersion(out int major, out version, out int patch);
                }
                return context_;
            }
        }

        /// \brief Get the handle to the current context, creating one if necessary.
        ///
        /// Note that Unity's editor, player, and standalone builds will have different contexts.
        ///
        /// \return The returned handle to the context.
        [DllImport(binaryName)]
        public static extern IntPtr getOrCreateGlobalOvrAudioContext();

        [DllImport(binaryName)]
        public static extern IntPtr ovrAudio_GetVersion(out int Major, out int Minor, out int Patch);

        /***********************************************************************************/
        // Settings API
        [DllImport(binaryName)]
        private static extern int ovrAudio_SetAcousticModel(IntPtr context, AcousticModel quality);
        public int SetAcousticModel(AcousticModel model)
        {
            return ovrAudio_SetAcousticModel(context, model);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_ResetSharedReverb(IntPtr context);
        public int ResetReverb()
        {
            return ovrAudio_ResetSharedReverb(context);
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_Enable(IntPtr context, int what, int enable);
        public int SetEnabled(int feature, bool enabled)
        {
            return ovrAudio_Enable(context, feature, enabled ? 1 : 0);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_Enable(IntPtr context, EnableFlagInternal what, int enable);
        public int SetEnabled(EnableFlagInternal feature, bool enabled)
        {
            return ovrAudio_Enable(context, feature, enabled ? 1 : 0);
        }

        /***********************************************************************************/
        // Geometry API
        [DllImport(binaryName)]
        private static extern int ovrAudio_CreateAudioGeometry(IntPtr context, out IntPtr geometry);
        public int CreateAudioGeometry(out IntPtr geometry)
        {
            return ovrAudio_CreateAudioGeometry(context, out geometry);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_DestroyAudioGeometry(IntPtr geometry);
        public int DestroyAudioGeometry(IntPtr geometry)
        {
            return ovrAudio_DestroyAudioGeometry(geometry);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioGeometrySetObjectFlag(IntPtr geometry, ObjectFlags flag, int enabled);
        public int AudioGeometrySetObjectFlag(IntPtr geometry, ObjectFlags flag, bool enabled)
        {
            if (version < 94)
                return -1;

            return ovrAudio_AudioGeometrySetObjectFlag(geometry, flag, enabled ? 1 : 0);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioGeometryUploadMeshArrays(IntPtr geometry,
                                                                        float[] vertices, UIntPtr verticesBytesOffset, UIntPtr vertexCount, UIntPtr vertexStride, ovrAudioScalarType vertexType,
                                                                        int[] indices, UIntPtr indicesByteOffset, UIntPtr indexCount, ovrAudioScalarType indexType,
                                                                        MeshGroup[] groups, UIntPtr groupCount);

        public int AudioGeometryUploadMeshArrays(IntPtr geometry,
                                                        float[] vertices, int vertexCount,
                                                        int[] indices, int indexCount,
                                                        MeshGroup[] groups, int groupCount)
        {
            return ovrAudio_AudioGeometryUploadMeshArrays(geometry,
                vertices, UIntPtr.Zero, (UIntPtr)vertexCount, UIntPtr.Zero, ovrAudioScalarType.Float32,
                indices, UIntPtr.Zero, (UIntPtr)indexCount, ovrAudioScalarType.UInt32,
                groups, (UIntPtr)groupCount);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioGeometryUploadSimplifiedMeshArrays(IntPtr geometry,
                                                                        float[] vertices, UIntPtr verticesBytesOffset, UIntPtr vertexCount, UIntPtr vertexStride, ovrAudioScalarType vertexType,
                                                                        int[] indices, UIntPtr indicesByteOffset, UIntPtr indexCount, ovrAudioScalarType indexType,
                                                                        MeshGroup[] groups, UIntPtr groupCount,
                                                                        ref MeshSimplification simplification);

        public int AudioGeometryUploadSimplifiedMeshArrays(IntPtr geometry,
                                                        float[] vertices, int vertexCount,
                                                        int[] indices, int indexCount,
                                                        MeshGroup[] groups, int groupCount,
                                                        ref MeshSimplification simplification)
        {
            return ovrAudio_AudioGeometryUploadSimplifiedMeshArrays(geometry,
                vertices, UIntPtr.Zero, (UIntPtr)vertexCount, UIntPtr.Zero, ovrAudioScalarType.Float32,
                indices, UIntPtr.Zero, (UIntPtr)indexCount, ovrAudioScalarType.UInt32,
                groups, (UIntPtr)groupCount, ref simplification);
        }

        [DllImport(binaryName)]
        private static extern unsafe int ovrAudio_AudioGeometrySetTransform(IntPtr geometry, float* matrix4x4);
        public int AudioGeometrySetTransform(IntPtr geometry, in Matrix4x4 matrix)
        {
            unsafe
            {
                float* nativeMatrixCopy = stackalloc float[16];

                // Note: flip Z to convert from left-handed (+Z forward) to right-handed (+Z backward)
                nativeMatrixCopy[0] = matrix.m00;
                nativeMatrixCopy[1] = matrix.m10;
                nativeMatrixCopy[2] = -matrix.m20;
                nativeMatrixCopy[3] = matrix.m30;
                nativeMatrixCopy[4] = matrix.m01;
                nativeMatrixCopy[5] = matrix.m11;
                nativeMatrixCopy[6] = -matrix.m21;
                nativeMatrixCopy[7] = matrix.m31;
                nativeMatrixCopy[8] = matrix.m02;
                nativeMatrixCopy[9] = matrix.m12;
                nativeMatrixCopy[10] = -matrix.m22;
                nativeMatrixCopy[11] = matrix.m32;
                nativeMatrixCopy[12] = matrix.m03;
                nativeMatrixCopy[13] = matrix.m13;
                nativeMatrixCopy[14] = -matrix.m23;
                nativeMatrixCopy[15] = matrix.m33;

                return ovrAudio_AudioGeometrySetTransform(geometry, nativeMatrixCopy);
            }
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioGeometryGetTransform(IntPtr geometry, out float[] matrix4x4);
        public int AudioGeometryGetTransform(IntPtr geometry, out float[] matrix4x4)
        {
            return ovrAudio_AudioGeometryGetTransform(geometry, out matrix4x4);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioGeometryWriteMeshFile(IntPtr geometry, string filePath);
        public int AudioGeometryWriteMeshFile(IntPtr geometry, string filePath)
        {
            return ovrAudio_AudioGeometryWriteMeshFile(geometry, filePath);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioGeometryReadMeshFile(IntPtr geometry, string filePath);
        public int AudioGeometryReadMeshFile(IntPtr geometry, string filePath)
        {
            return ovrAudio_AudioGeometryReadMeshFile(geometry, filePath);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioGeometryReadMeshMemory(IntPtr geometry, IntPtr data, UInt64 dataLength);
        public int AudioGeometryReadMeshMemory(IntPtr geometry, IntPtr data, UInt64 dataLength)
        {
            return ovrAudio_AudioGeometryReadMeshMemory(geometry, data, dataLength);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioGeometryWriteMeshFileObj(IntPtr geometry, string filePath);
        public int AudioGeometryWriteMeshFileObj(IntPtr geometry, string filePath)
        {
            return ovrAudio_AudioGeometryWriteMeshFileObj(geometry, filePath);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioGeometryGetSimplifiedMeshWithMaterials(IntPtr geometry, IntPtr unused1, out uint numVertices, IntPtr unused2, IntPtr unused3, out uint numTriangles);

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioGeometryGetSimplifiedMeshWithMaterials(IntPtr geometry, float[] vertices, ref uint numVertices, uint[] indices, uint[] materialIndices, ref uint numTriangles);

        public int AudioGeometryGetSimplifiedMesh(IntPtr geometry, out float[] vertices, out uint[] indices, out uint[] materialIndices)
        {
            int result = ovrAudio_AudioGeometryGetSimplifiedMeshWithMaterials(geometry, IntPtr.Zero, out uint numVertices, IntPtr.Zero, IntPtr.Zero, out uint numTriangles);
            if (result != 0)
            {
                Debug.LogError("unexpected error getting simplified mesh array sizes");
                vertices = null;
                indices = null;
                materialIndices = null;
                return result;
            }

            vertices = new float[numVertices * 3];
            indices = new uint[numTriangles * 3];
            materialIndices = new uint[numTriangles];
            return ovrAudio_AudioGeometryGetSimplifiedMeshWithMaterials(geometry, vertices, ref numVertices, indices, materialIndices, ref numTriangles);
        }

        /***********************************************************************************/
        // Material API
        [DllImport(binaryName)]
        private static extern int ovrAudio_CreateAudioMaterial(IntPtr context, out IntPtr material);
        public int CreateAudioMaterial(out IntPtr material)
        {
            return ovrAudio_CreateAudioMaterial(context, out material);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_DestroyAudioMaterial(IntPtr material);
        public int DestroyAudioMaterial(IntPtr material)
        {
            return ovrAudio_DestroyAudioMaterial(material);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioMaterialSetFrequency(IntPtr material, MaterialProperty property, float frequency, float value);
        public int AudioMaterialSetFrequency(IntPtr material, MaterialProperty property, float frequency, float value)
        {
            return ovrAudio_AudioMaterialSetFrequency(material, property, frequency, value);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioMaterialGetFrequency(IntPtr material, MaterialProperty property, float frequency, out float value);
        public int AudioMaterialGetFrequency(IntPtr material, MaterialProperty property, float frequency, out float value)
        {
            return ovrAudio_AudioMaterialGetFrequency(material, property, frequency, out value);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioMaterialReset(IntPtr material, MaterialProperty property);
        public int AudioMaterialReset(IntPtr material, MaterialProperty property)
        {
            return ovrAudio_AudioMaterialReset(material, property);
        }

        /***********************************************************************************/
        // Acoustic Map API
        [DllImport(binaryName)]
        private static extern int ovrAudio_CreateAudioSceneIR(IntPtr context, out IntPtr sceneIR);
        public int CreateAudioSceneIR(out IntPtr sceneIR)
        {
            return ovrAudio_CreateAudioSceneIR(context, out sceneIR);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_DestroyAudioSceneIR(IntPtr sceneIR);
        public int DestroyAudioSceneIR(IntPtr sceneIR)
        {
            return ovrAudio_DestroyAudioSceneIR(sceneIR);
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioSceneIRSetEnabled(IntPtr sceneIR, int enabled);
        public int AudioSceneIRSetEnabled(IntPtr sceneIR, bool enabled)
        {
            return ovrAudio_AudioSceneIRSetEnabled(sceneIR, enabled ? 1 : 0);
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioSceneIRGetEnabled(IntPtr sceneIR, out int enabled);
        public int AudioSceneIRGetEnabled(IntPtr sceneIR, out bool enabled)
        {
            int iEnabled;
            int res = ovrAudio_AudioSceneIRGetEnabled(sceneIR, out iEnabled);
            enabled = iEnabled != 0;
            return res;
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioSceneIRGetStatus(IntPtr sceneIR, out AcousticMapStatus status);
        public int AudioSceneIRGetStatus(IntPtr sceneIR, out AcousticMapStatus status)
        {
            return ovrAudio_AudioSceneIRGetStatus(sceneIR, out status);
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_InitializeAudioSceneIRParameters(out MapParameters parameters);
        public int InitializeAudioSceneIRParameters(out MapParameters parameters)
        {
            return ovrAudio_InitializeAudioSceneIRParameters(out parameters);
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioSceneIRCompute(IntPtr sceneIR, ref MapParameters parameters);
        public int AudioSceneIRCompute(IntPtr sceneIR, ref MapParameters parameters)
        {
            return ovrAudio_AudioSceneIRCompute(sceneIR, ref parameters);
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioSceneIRComputeCustomPoints(IntPtr sceneIR,
            float[] points, UIntPtr pointCount, ref MapParameters parameters);
        public int AudioSceneIRComputeCustomPoints(IntPtr sceneIR,
            float[] points, UIntPtr pointCount, ref MapParameters parameters)
        {
            return ovrAudio_AudioSceneIRComputeCustomPoints(sceneIR, points, pointCount, ref parameters);
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioSceneIRGetPointCount(IntPtr sceneIR, out UIntPtr pointCount);
        public int AudioSceneIRGetPointCount(IntPtr sceneIR, out UIntPtr pointCount)
        {
            return ovrAudio_AudioSceneIRGetPointCount(sceneIR, out pointCount);
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioSceneIRGetPoints(IntPtr sceneIR, float[] points, UIntPtr maxPointCount);
        public int AudioSceneIRGetPoints(IntPtr sceneIR, float[] points, UIntPtr maxPointCount)
        {
            return ovrAudio_AudioSceneIRGetPoints(sceneIR, points, maxPointCount);
        }

        [DllImport(binaryName)]
        private static extern unsafe int ovrAudio_AudioSceneIRSetTransform(IntPtr sceneIR, float* matrix4x4);
        public int AudioSceneIRSetTransform(IntPtr sceneIR, in Matrix4x4 matrix)
        {
            unsafe
            {
                float* nativeMatrixCopy = stackalloc float[16];

                // Note: flip Z to convert from left-handed (+Z forward) to right-handed (+Z backward)
                nativeMatrixCopy[0] = matrix.m00;
                nativeMatrixCopy[1] = matrix.m10;
                nativeMatrixCopy[2] = -matrix.m20;
                nativeMatrixCopy[3] = matrix.m30;
                nativeMatrixCopy[4] = matrix.m01;
                nativeMatrixCopy[5] = matrix.m11;
                nativeMatrixCopy[6] = -matrix.m21;
                nativeMatrixCopy[7] = matrix.m31;
                nativeMatrixCopy[8] = matrix.m02;
                nativeMatrixCopy[9] = matrix.m12;
                nativeMatrixCopy[10] = -matrix.m22;
                nativeMatrixCopy[11] = matrix.m32;
                nativeMatrixCopy[12] = matrix.m03;
                nativeMatrixCopy[13] = matrix.m13;
                nativeMatrixCopy[14] = -matrix.m23;
                nativeMatrixCopy[15] = matrix.m33;

                return ovrAudio_AudioSceneIRSetTransform(sceneIR, nativeMatrixCopy);
            }
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioSceneIRGetTransform(IntPtr sceneIR, out float[] matrix4x4);
        public int AudioSceneIRGetTransform(IntPtr sceneIR, out float[] matrix4x4)
        {
            return ovrAudio_AudioSceneIRGetTransform(sceneIR, out matrix4x4);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioSceneIRWriteFile(IntPtr sceneIR, string filePath);
        public int AudioSceneIRWriteFile(IntPtr sceneIR, string filePath)
        {
            return ovrAudio_AudioSceneIRWriteFile(sceneIR, filePath);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioSceneIRReadFile(IntPtr sceneIR, string filePath);
        public int AudioSceneIRReadFile(IntPtr sceneIR, string filePath)
        {
            return ovrAudio_AudioSceneIRReadFile(sceneIR, filePath);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioSceneIRReadMemory(IntPtr sceneIR, IntPtr data, UInt64 dataLength);
        public int AudioSceneIRReadMemory(IntPtr sceneIR, IntPtr data, UInt64 dataLength)
        {
            return ovrAudio_AudioSceneIRReadMemory(sceneIR, data, dataLength);
        }

        /***********************************************************************************/
        // Control Zone API
        [DllImport(binaryName)]
        private static extern int ovrAudio_CreateControlZone(IntPtr context, out IntPtr control);
        [DllImport(binaryName)]
        private static extern int ovrAudio_CreateControlVolume(IntPtr context, out IntPtr control);
        public int CreateControlZone(out IntPtr control)
        {
            try
            {
                return ovrAudio_CreateControlZone(context, out control);
            }
            catch
            {
                // Hack for v60 compatibility
                return ovrAudio_CreateControlVolume(context, out control);
            }
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_DestroyControlZone(IntPtr control);
        [DllImport(binaryName)]
        private static extern int ovrAudio_DestroyControlVolume(IntPtr control);
        public int DestroyControlZone(IntPtr control)
        {
            try
            {
                return ovrAudio_DestroyControlZone(control);
            }
            catch
            {
                // Hack for v60 compatibility
                return ovrAudio_DestroyControlVolume(control);
            }
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlZoneSetEnabled(IntPtr control, int enabled);
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlVolumeSetEnabled(IntPtr control, int enabled);
        public int ControlZoneSetEnabled(IntPtr control, bool enabled)
        {
            try
            {
                return ovrAudio_ControlZoneSetEnabled(control, enabled ? 1 : 0);
            }
            catch
            {
                // Hack for v60 compatibility
                return ovrAudio_ControlVolumeSetEnabled(control, enabled ? 1 : 0);
            }
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlZoneGetEnabled(IntPtr control, out int enabled);
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlVolumeGetEnabled(IntPtr control, out int enabled);
        public int ControlZoneGetEnabled(IntPtr control, out bool enabled)
        {
            int enabledInt = 0;
            int result;

            try
            {
                result = ovrAudio_ControlZoneGetEnabled(control, out enabledInt);
            }
            catch
            {
                // Hack for v60 compatibility
                result = ovrAudio_ControlVolumeGetEnabled(control, out enabledInt);
            }
            enabled = enabledInt != 0;
            return result;
        }
        [DllImport(binaryName)]
        private static extern unsafe int ovrAudio_ControlZoneSetTransform(IntPtr control, float* matrix4x4);
        [DllImport(binaryName)]
        private static extern unsafe int ovrAudio_ControlVolumeSetTransform(IntPtr control, float* matrix4x4);
        public int ControlZoneSetTransform(IntPtr control, in Matrix4x4 matrix)
        {
            unsafe
            {
                float* nativeMatrixCopy = stackalloc float[16];

                // Note: flip Z to convert from left-handed (+Z forward) to right-handed (+Z backward)
                nativeMatrixCopy[0] = matrix.m00;
                nativeMatrixCopy[1] = matrix.m10;
                nativeMatrixCopy[2] = -matrix.m20;
                nativeMatrixCopy[3] = matrix.m30;
                nativeMatrixCopy[4] = matrix.m01;
                nativeMatrixCopy[5] = matrix.m11;
                nativeMatrixCopy[6] = -matrix.m21;
                nativeMatrixCopy[7] = matrix.m31;
                nativeMatrixCopy[8] = matrix.m02;
                nativeMatrixCopy[9] = matrix.m12;
                nativeMatrixCopy[10] = -matrix.m22;
                nativeMatrixCopy[11] = matrix.m32;
                nativeMatrixCopy[12] = matrix.m03;
                nativeMatrixCopy[13] = matrix.m13;
                nativeMatrixCopy[14] = -matrix.m23;
                nativeMatrixCopy[15] = matrix.m33;
                try
                {
                    return ovrAudio_ControlZoneSetTransform(control, nativeMatrixCopy);
                }
                catch
                {
                    // Hack for v60 compatibility
                    return ovrAudio_ControlVolumeSetTransform(control, nativeMatrixCopy);
                }
            }
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlZoneGetTransform(IntPtr control, out float[] matrix4x4);
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlVolumeGetTransform(IntPtr control, out float[] matrix4x4);
        public int ControlZoneGetTransform(IntPtr control, out float[] matrix4x4)
        {
            try
            {
                return ovrAudio_ControlZoneGetTransform(control, out matrix4x4);
            }
            catch
            {
                // Hack for v60 compatibility
                return ovrAudio_ControlVolumeGetTransform(control, out matrix4x4);
            }
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlZoneSetBox(IntPtr control, float sizeX, float sizeY, float sizeZ);
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlVolumeSetBox(IntPtr control, float sizeX, float sizeY, float sizeZ);
        public int ControlZoneSetBox(IntPtr control, float sizeX, float sizeY, float sizeZ)
        {
            try
            {
                return ovrAudio_ControlZoneSetBox(control, sizeX, sizeY, sizeZ);
            }
            catch
            {
                // Hack for v60 compatibility
                return ovrAudio_ControlVolumeSetBox(control, sizeX, sizeY, sizeZ);
            }
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlZoneGetBox(IntPtr control, out float sizeX, out float sizeY, out float sizeZ);
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlVolumeGetBox(IntPtr control, out float sizeX, out float sizeY, out float sizeZ);
        public int ControlZoneGetBox(IntPtr control, out float sizeX, out float sizeY, out float sizeZ)
        {
            try
            {
                return ovrAudio_ControlZoneGetBox(control, out sizeX, out sizeY, out sizeZ);
            }
            catch
            {
                // Hack for v60 compatibility
                return ovrAudio_ControlVolumeGetBox(control, out sizeX, out sizeY, out sizeZ);
            }
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlZoneSetFadeDistance(IntPtr control, float fadeX, float fadeY, float fadeZ);
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlVolumeSetFadeDistance(IntPtr control, float fadeX, float fadeY, float fadeZ);
        public int ControlZoneSetFadeDistance(IntPtr control, float fadeX, float fadeY, float fadeZ)
        {
            try
            {
                return ovrAudio_ControlZoneSetFadeDistance(control, fadeX, fadeY, fadeZ);
            }
            catch
            {
                // Hack for v60 compatibility
                return ovrAudio_ControlVolumeSetFadeDistance(control, fadeX, fadeY, fadeZ);
            }
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlZoneGetFadeDistance(IntPtr control, out float fadeX, out float fadeY, out float fadeZ);
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlVolumeGetFadeDistance(IntPtr control, out float fadeX, out float fadeY, out float fadeZ);
        public int ControlZoneGetFadeDistance(IntPtr control, out float fadeX, out float fadeY, out float fadeZ)
        {
            try
            {
                return ovrAudio_ControlZoneGetFadeDistance(control, out fadeX, out fadeY, out fadeZ);
            }
            catch
            {
                // Hack for v60 compatibility
                return ovrAudio_ControlVolumeGetFadeDistance(control, out fadeX, out fadeY, out fadeZ);
            }
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlZoneSetFrequency(IntPtr control, ControlZoneProperty property, float frequency, float value);
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlVolumeSetFrequency(IntPtr control, ControlZoneProperty property, float frequency, float value);
        public int ControlZoneSetFrequency(IntPtr control, ControlZoneProperty property, float frequency, float value)
        {
            try
            {
                return ovrAudio_ControlZoneSetFrequency(control, property, frequency, value);
            }
            catch
            {
                // Hack for v60 compatibility
                return ovrAudio_ControlVolumeSetFrequency(control, property, frequency, value);
            }
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlZoneReset(IntPtr control, ControlZoneProperty property);
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlVolumeReset(IntPtr control, ControlZoneProperty property);
        public int ControlZoneReset(IntPtr control, ControlZoneProperty property)
        {
            try
            {
                return ovrAudio_ControlZoneReset(control, property);
            }
            catch
            {
                // Hack for v60 compatibility
                return ovrAudio_ControlVolumeReset(control, property);
            }
        }
    }

    /***********************************************************************************/
    // FMOD
    /***********************************************************************************/
    public class FMODPluginInterface : INativeInterface
    {
        /// \brief Name of the binary this interface wraps.
        ///
        /// This value can be used in  `[DllImport(binaryName)]` decorators and tells Unity what the binary name is for the FMOD plug-in.
        public const string binaryName = "MetaXRAudioFMOD";

        /***********************************************************************************/
        // Context API: Required to create internal context if it does not exist yet
        IntPtr context_ = IntPtr.Zero;
        int version;
        IntPtr context
        {
            get
            {
                if (context_ == IntPtr.Zero)
                {
                    ovrAudio_GetPluginContext(out context_);
                    ovrAudio_GetVersion(out int major, out version, out int patch);
                }
                return context_;
            }
        }

        /// \brief Get the handle to the current context, creating one if necessary.
        ///
        /// Note that Unity's editor, player, and standalone builds will have different contexts.
        ///
        /// \param[out] context The returned handle to the context.
        /// \return Returns an ovrResult indicating success or failure.
        [DllImport(binaryName)]
        public static extern int ovrAudio_GetPluginContext(out IntPtr context);

        [DllImport(binaryName)]
        public static extern IntPtr ovrAudio_GetVersion(out int Major, out int Minor, out int Patch);

        /***********************************************************************************/
        // Settings API
        [DllImport(binaryName)]
        private static extern int ovrAudio_SetAcousticModel(IntPtr context, AcousticModel quality);
        public int SetAcousticModel(AcousticModel model)
        {
            return ovrAudio_SetAcousticModel(context, model);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_ResetSharedReverb(IntPtr context);
        public int ResetReverb()
        {
            return ovrAudio_ResetSharedReverb(context);
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_Enable(IntPtr context, int what, int enable);
        public int SetEnabled(int feature, bool enabled)
        {
            return ovrAudio_Enable(context, feature, enabled ? 1 : 0);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_Enable(IntPtr context, EnableFlagInternal what, int enable);
        public int SetEnabled(EnableFlagInternal feature, bool enabled)
        {
            return ovrAudio_Enable(context, feature, enabled ? 1 : 0);
        }

        /***********************************************************************************/
        // Geometry API
        [DllImport(binaryName)]
        private static extern int ovrAudio_CreateAudioGeometry(IntPtr context, out IntPtr geometry);
        public int CreateAudioGeometry(out IntPtr geometry)
        {
            return ovrAudio_CreateAudioGeometry(context, out geometry);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_DestroyAudioGeometry(IntPtr geometry);
        public int DestroyAudioGeometry(IntPtr geometry)
        {
            return ovrAudio_DestroyAudioGeometry(geometry);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioGeometrySetObjectFlag(IntPtr geometry, ObjectFlags flag, int enabled);
        public int AudioGeometrySetObjectFlag(IntPtr geometry, ObjectFlags flag, bool enabled)
        {
            if (version < 94)
                return -1;

            return ovrAudio_AudioGeometrySetObjectFlag(geometry, flag, enabled ? 1 : 0);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioGeometryUploadMeshArrays(IntPtr geometry,
                                                                        float[] vertices, UIntPtr verticesBytesOffset, UIntPtr vertexCount, UIntPtr vertexStride, ovrAudioScalarType vertexType,
                                                                        int[] indices, UIntPtr indicesByteOffset, UIntPtr indexCount, ovrAudioScalarType indexType,
                                                                        MeshGroup[] groups, UIntPtr groupCount);

        public int AudioGeometryUploadMeshArrays(IntPtr geometry,
                                                        float[] vertices, int vertexCount,
                                                        int[] indices, int indexCount,
                                                        MeshGroup[] groups, int groupCount)
        {
            return ovrAudio_AudioGeometryUploadMeshArrays(geometry,
                vertices, UIntPtr.Zero, (UIntPtr)vertexCount, UIntPtr.Zero, ovrAudioScalarType.Float32,
                indices, UIntPtr.Zero, (UIntPtr)indexCount, ovrAudioScalarType.UInt32,
                groups, (UIntPtr)groupCount);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioGeometryUploadSimplifiedMeshArrays(IntPtr geometry,
                                                                        float[] vertices, UIntPtr verticesBytesOffset, UIntPtr vertexCount, UIntPtr vertexStride, ovrAudioScalarType vertexType,
                                                                        int[] indices, UIntPtr indicesByteOffset, UIntPtr indexCount, ovrAudioScalarType indexType,
                                                                        MeshGroup[] groups, UIntPtr groupCount,
                                                                        ref MeshSimplification simplification);

        public int AudioGeometryUploadSimplifiedMeshArrays(IntPtr geometry,
                                                        float[] vertices, int vertexCount,
                                                        int[] indices, int indexCount,
                                                        MeshGroup[] groups, int groupCount,
                                                        ref MeshSimplification simplification)
        {
            return ovrAudio_AudioGeometryUploadSimplifiedMeshArrays(geometry,
                vertices, UIntPtr.Zero, (UIntPtr)vertexCount, UIntPtr.Zero, ovrAudioScalarType.Float32,
                indices, UIntPtr.Zero, (UIntPtr)indexCount, ovrAudioScalarType.UInt32,
                groups, (UIntPtr)groupCount, ref simplification);
        }

        [DllImport(binaryName)]
        private static extern unsafe int ovrAudio_AudioGeometrySetTransform(IntPtr geometry, float* matrix4x4);
        public int AudioGeometrySetTransform(IntPtr geometry, in Matrix4x4 matrix)
        {
            unsafe
            {
                float* nativeMatrixCopy = stackalloc float[16];

                // Note: flip Z to convert from left-handed (+Z forward) to right-handed (+Z backward)
                nativeMatrixCopy[0] = matrix.m00;
                nativeMatrixCopy[1] = matrix.m10;
                nativeMatrixCopy[2] = -matrix.m20;
                nativeMatrixCopy[3] = matrix.m30;
                nativeMatrixCopy[4] = matrix.m01;
                nativeMatrixCopy[5] = matrix.m11;
                nativeMatrixCopy[6] = -matrix.m21;
                nativeMatrixCopy[7] = matrix.m31;
                nativeMatrixCopy[8] = matrix.m02;
                nativeMatrixCopy[9] = matrix.m12;
                nativeMatrixCopy[10] = -matrix.m22;
                nativeMatrixCopy[11] = matrix.m32;
                nativeMatrixCopy[12] = matrix.m03;
                nativeMatrixCopy[13] = matrix.m13;
                nativeMatrixCopy[14] = -matrix.m23;
                nativeMatrixCopy[15] = matrix.m33;

                return ovrAudio_AudioGeometrySetTransform(geometry, nativeMatrixCopy);
            }
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioGeometryGetTransform(IntPtr geometry, out float[] matrix4x4);
        public int AudioGeometryGetTransform(IntPtr geometry, out float[] matrix4x4)
        {
            return ovrAudio_AudioGeometryGetTransform(geometry, out matrix4x4);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioGeometryWriteMeshFile(IntPtr geometry, string filePath);
        public int AudioGeometryWriteMeshFile(IntPtr geometry, string filePath)
        {
            return ovrAudio_AudioGeometryWriteMeshFile(geometry, filePath);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioGeometryReadMeshFile(IntPtr geometry, string filePath);
        public int AudioGeometryReadMeshFile(IntPtr geometry, string filePath)
        {
            return ovrAudio_AudioGeometryReadMeshFile(geometry, filePath);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioGeometryReadMeshMemory(IntPtr geometry, IntPtr data, UInt64 dataLength);
        public int AudioGeometryReadMeshMemory(IntPtr geometry, IntPtr data, UInt64 dataLength)
        {
            return ovrAudio_AudioGeometryReadMeshMemory(geometry, data, dataLength);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioGeometryWriteMeshFileObj(IntPtr geometry, string filePath);
        public int AudioGeometryWriteMeshFileObj(IntPtr geometry, string filePath)
        {
            return ovrAudio_AudioGeometryWriteMeshFileObj(geometry, filePath);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioGeometryGetSimplifiedMeshWithMaterials(IntPtr geometry, IntPtr unused1, out uint numVertices, IntPtr unused2, IntPtr unused3, out uint numTriangles);

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioGeometryGetSimplifiedMeshWithMaterials(IntPtr geometry, float[] vertices, ref uint numVertices, uint[] indices, uint[] materialIndices, ref uint numTriangles);

        public int AudioGeometryGetSimplifiedMesh(IntPtr geometry, out float[] vertices, out uint[] indices, out uint[] materialIndices)
        {
            int result = ovrAudio_AudioGeometryGetSimplifiedMeshWithMaterials(geometry, IntPtr.Zero, out uint numVertices, IntPtr.Zero, IntPtr.Zero, out uint numTriangles);
            if (result != 0)
            {
                Debug.LogError("unexpected error getting simplified mesh array sizes");
                vertices = null;
                indices = null;
                materialIndices = null;
                return result;
            }

            vertices = new float[numVertices * 3];
            indices = new uint[numTriangles * 3];
            materialIndices = new uint[numTriangles];
            return ovrAudio_AudioGeometryGetSimplifiedMeshWithMaterials(geometry, vertices, ref numVertices, indices, materialIndices, ref numTriangles);
        }

        /***********************************************************************************/
        // Material API
        [DllImport(binaryName)]
        private static extern int ovrAudio_CreateAudioMaterial(IntPtr context, out IntPtr material);
        public int CreateAudioMaterial(out IntPtr material)
        {
            return ovrAudio_CreateAudioMaterial(context, out material);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_DestroyAudioMaterial(IntPtr material);
        public int DestroyAudioMaterial(IntPtr material)
        {
            return ovrAudio_DestroyAudioMaterial(material);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioMaterialSetFrequency(IntPtr material, MaterialProperty property, float frequency, float value);
        public int AudioMaterialSetFrequency(IntPtr material, MaterialProperty property, float frequency, float value)
        {
            return ovrAudio_AudioMaterialSetFrequency(material, property, frequency, value);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioMaterialGetFrequency(IntPtr material, MaterialProperty property, float frequency, out float value);
        public int AudioMaterialGetFrequency(IntPtr material, MaterialProperty property, float frequency, out float value)
        {
            return ovrAudio_AudioMaterialGetFrequency(material, property, frequency, out value);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioMaterialReset(IntPtr material, MaterialProperty property);
        public int AudioMaterialReset(IntPtr material, MaterialProperty property)
        {
            return ovrAudio_AudioMaterialReset(material, property);
        }
        /***********************************************************************************/
        // Acoustic Map API
        [DllImport(binaryName)]
        private static extern int ovrAudio_CreateAudioSceneIR(IntPtr context, out IntPtr sceneIR);
        public int CreateAudioSceneIR(out IntPtr sceneIR)
        {
            return ovrAudio_CreateAudioSceneIR(context, out sceneIR);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_DestroyAudioSceneIR(IntPtr sceneIR);
        public int DestroyAudioSceneIR(IntPtr sceneIR)
        {
            return ovrAudio_DestroyAudioSceneIR(sceneIR);
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioSceneIRSetEnabled(IntPtr sceneIR, int enabled);
        public int AudioSceneIRSetEnabled(IntPtr sceneIR, bool enabled)
        {
            return ovrAudio_AudioSceneIRSetEnabled(sceneIR, enabled ? 1 : 0);
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioSceneIRGetEnabled(IntPtr sceneIR, out int enabled);
        public int AudioSceneIRGetEnabled(IntPtr sceneIR, out bool enabled)
        {
            int iEnabled;
            int res = ovrAudio_AudioSceneIRGetEnabled(sceneIR, out iEnabled);
            enabled = iEnabled != 0;
            return res;
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioSceneIRGetStatus(IntPtr sceneIR, out AcousticMapStatus status);
        public int AudioSceneIRGetStatus(IntPtr sceneIR, out AcousticMapStatus status)
        {
            return ovrAudio_AudioSceneIRGetStatus(sceneIR, out status);
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_InitializeAudioSceneIRParameters(out MapParameters parameters);
        public int InitializeAudioSceneIRParameters(out MapParameters parameters)
        {
            return ovrAudio_InitializeAudioSceneIRParameters(out parameters);
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioSceneIRCompute(IntPtr sceneIR, ref MapParameters parameters);
        public int AudioSceneIRCompute(IntPtr sceneIR, ref MapParameters parameters)
        {
            return ovrAudio_AudioSceneIRCompute(sceneIR, ref parameters);
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioSceneIRComputeCustomPoints(IntPtr sceneIR,
            float[] points, UIntPtr pointCount, ref MapParameters parameters);
        public int AudioSceneIRComputeCustomPoints(IntPtr sceneIR,
            float[] points, UIntPtr pointCount, ref MapParameters parameters)
        {
            return ovrAudio_AudioSceneIRComputeCustomPoints(sceneIR, points, pointCount, ref parameters);
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioSceneIRGetPointCount(IntPtr sceneIR, out UIntPtr pointCount);
        public int AudioSceneIRGetPointCount(IntPtr sceneIR, out UIntPtr pointCount)
        {
            return ovrAudio_AudioSceneIRGetPointCount(sceneIR, out pointCount);
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioSceneIRGetPoints(IntPtr sceneIR, float[] points, UIntPtr maxPointCount);
        public int AudioSceneIRGetPoints(IntPtr sceneIR, float[] points, UIntPtr maxPointCount)
        {
            return ovrAudio_AudioSceneIRGetPoints(sceneIR, points, maxPointCount);
        }

        [DllImport(binaryName)]
        private static extern unsafe int ovrAudio_AudioSceneIRSetTransform(IntPtr sceneIR, float* matrix4x4);
        public int AudioSceneIRSetTransform(IntPtr sceneIR, in Matrix4x4 matrix)
        {
            unsafe
            {
                float* nativeMatrixCopy = stackalloc float[16];

                // Note: flip Z to convert from left-handed (+Z forward) to right-handed (+Z backward)
                nativeMatrixCopy[0] = matrix.m00;
                nativeMatrixCopy[1] = matrix.m10;
                nativeMatrixCopy[2] = -matrix.m20;
                nativeMatrixCopy[3] = matrix.m30;
                nativeMatrixCopy[4] = matrix.m01;
                nativeMatrixCopy[5] = matrix.m11;
                nativeMatrixCopy[6] = -matrix.m21;
                nativeMatrixCopy[7] = matrix.m31;
                nativeMatrixCopy[8] = matrix.m02;
                nativeMatrixCopy[9] = matrix.m12;
                nativeMatrixCopy[10] = -matrix.m22;
                nativeMatrixCopy[11] = matrix.m32;
                nativeMatrixCopy[12] = matrix.m03;
                nativeMatrixCopy[13] = matrix.m13;
                nativeMatrixCopy[14] = -matrix.m23;
                nativeMatrixCopy[15] = matrix.m33;

                return ovrAudio_AudioSceneIRSetTransform(sceneIR, nativeMatrixCopy);
            }
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioSceneIRGetTransform(IntPtr sceneIR, out float[] matrix4x4);
        public int AudioSceneIRGetTransform(IntPtr sceneIR, out float[] matrix4x4)
        {
            return ovrAudio_AudioSceneIRGetTransform(sceneIR, out matrix4x4);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioSceneIRWriteFile(IntPtr sceneIR, string filePath);
        public int AudioSceneIRWriteFile(IntPtr sceneIR, string filePath)
        {
            return ovrAudio_AudioSceneIRWriteFile(sceneIR, filePath);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioSceneIRReadFile(IntPtr sceneIR, string filePath);
        public int AudioSceneIRReadFile(IntPtr sceneIR, string filePath)
        {
            return ovrAudio_AudioSceneIRReadFile(sceneIR, filePath);
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_AudioSceneIRReadMemory(IntPtr sceneIR, IntPtr data, UInt64 dataLength);
        public int AudioSceneIRReadMemory(IntPtr sceneIR, IntPtr data, UInt64 dataLength)
        {
            return ovrAudio_AudioSceneIRReadMemory(sceneIR, data, dataLength);
        }

        /***********************************************************************************/
        // Control Zone API
        [DllImport(binaryName)]
        private static extern int ovrAudio_CreateControlZone(IntPtr context, out IntPtr control);
        [DllImport(binaryName)]
        private static extern int ovrAudio_CreateControlVolume(IntPtr context, out IntPtr control);
        public int CreateControlZone(out IntPtr control)
        {
            try
            {
                return ovrAudio_CreateControlZone(context, out control);
            }
            catch
            {
                // Hack for v60 compatibility
                return ovrAudio_CreateControlVolume(context, out control);
            }
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_DestroyControlZone(IntPtr control);
        [DllImport(binaryName)]
        private static extern int ovrAudio_DestroyControlVolume(IntPtr control);
        public int DestroyControlZone(IntPtr control)
        {
            try
            {
                return ovrAudio_DestroyControlZone(control);
            }
            catch
            {
                // Hack for v60 compatibility
                return ovrAudio_DestroyControlVolume(control);
            }
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlZoneSetEnabled(IntPtr control, int enabled);
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlVolumeSetEnabled(IntPtr control, int enabled);
        public int ControlZoneSetEnabled(IntPtr control, bool enabled)
        {
            try
            {
                return ovrAudio_ControlZoneSetEnabled(control, enabled ? 1 : 0);
            }
            catch
            {
                // Hack for v60 compatibility
                return ovrAudio_ControlVolumeSetEnabled(control, enabled ? 1 : 0);
            }
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlZoneGetEnabled(IntPtr control, out int enabled);
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlVolumeGetEnabled(IntPtr control, out int enabled);
        public int ControlZoneGetEnabled(IntPtr control, out bool enabled)
        {
            int enabledInt = 0;
            int result;

            try
            {
                result = ovrAudio_ControlZoneGetEnabled(control, out enabledInt);
            }
            catch
            {
                // Hack for v60 compatibility
                result = ovrAudio_ControlVolumeGetEnabled(control, out enabledInt);
            }
            enabled = enabledInt != 0;
            return result;
        }

        [DllImport(binaryName)]
        private static extern unsafe int ovrAudio_ControlZoneSetTransform(IntPtr control, float* matrix4x4);
        [DllImport(binaryName)]
        private static extern unsafe int ovrAudio_ControlVolumeSetTransform(IntPtr control, float* matrix4x4);
        public int ControlZoneSetTransform(IntPtr control, in Matrix4x4 matrix)
        {
            unsafe
            {
                float* nativeMatrixCopy = stackalloc float[16];

                // Note: flip Z to convert from left-handed (+Z forward) to right-handed (+Z backward)
                nativeMatrixCopy[0] = matrix.m00;
                nativeMatrixCopy[1] = matrix.m10;
                nativeMatrixCopy[2] = -matrix.m20;
                nativeMatrixCopy[3] = matrix.m30;
                nativeMatrixCopy[4] = matrix.m01;
                nativeMatrixCopy[5] = matrix.m11;
                nativeMatrixCopy[6] = -matrix.m21;
                nativeMatrixCopy[7] = matrix.m31;
                nativeMatrixCopy[8] = matrix.m02;
                nativeMatrixCopy[9] = matrix.m12;
                nativeMatrixCopy[10] = -matrix.m22;
                nativeMatrixCopy[11] = matrix.m32;
                nativeMatrixCopy[12] = matrix.m03;
                nativeMatrixCopy[13] = matrix.m13;
                nativeMatrixCopy[14] = -matrix.m23;
                nativeMatrixCopy[15] = matrix.m33;
                try
                {
                    return ovrAudio_ControlZoneSetTransform(control, nativeMatrixCopy);
                }
                catch
                {
                    // Hack for v60 compatibility
                    return ovrAudio_ControlVolumeSetTransform(control, nativeMatrixCopy);
                }
            }
        }

        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlZoneGetTransform(IntPtr control, out float[] matrix4x4);
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlVolumeGetTransform(IntPtr control, out float[] matrix4x4);
        public int ControlZoneGetTransform(IntPtr control, out float[] matrix4x4)
        {
            try
            {
                return ovrAudio_ControlZoneGetTransform(control, out matrix4x4);
            }
            catch
            {
                // Hack for v60 compatibility
                return ovrAudio_ControlVolumeGetTransform(control, out matrix4x4);
            }
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlZoneSetBox(IntPtr control, float sizeX, float sizeY, float sizeZ);
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlVolumeSetBox(IntPtr control, float sizeX, float sizeY, float sizeZ);
        public int ControlZoneSetBox(IntPtr control, float sizeX, float sizeY, float sizeZ)
        {
            try
            {
                return ovrAudio_ControlZoneSetBox(control, sizeX, sizeY, sizeZ);
            }
            catch
            {
                // Hack for v60 compatibility
                return ovrAudio_ControlVolumeSetBox(control, sizeX, sizeY, sizeZ);
            }
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlZoneGetBox(IntPtr control, out float sizeX, out float sizeY, out float sizeZ);
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlVolumeGetBox(IntPtr control, out float sizeX, out float sizeY, out float sizeZ);
        public int ControlZoneGetBox(IntPtr control, out float sizeX, out float sizeY, out float sizeZ)
        {
            try
            {
                return ovrAudio_ControlZoneGetBox(control, out sizeX, out sizeY, out sizeZ);
            }
            catch
            {
                // Hack for v60 compatibility
                return ovrAudio_ControlVolumeGetBox(control, out sizeX, out sizeY, out sizeZ);
            }
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlZoneSetFadeDistance(IntPtr control, float fadeX, float fadeY, float fadeZ);
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlVolumeSetFadeDistance(IntPtr control, float fadeX, float fadeY, float fadeZ);
        public int ControlZoneSetFadeDistance(IntPtr control, float fadeX, float fadeY, float fadeZ)
        {
            try
            {
                return ovrAudio_ControlZoneSetFadeDistance(control, fadeX, fadeY, fadeZ);
            }
            catch
            {
                // Hack for v60 compatibility
                return ovrAudio_ControlVolumeSetFadeDistance(control, fadeX, fadeY, fadeZ);
            }
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlZoneGetFadeDistance(IntPtr control, out float fadeX, out float fadeY, out float fadeZ);
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlVolumeGetFadeDistance(IntPtr control, out float fadeX, out float fadeY, out float fadeZ);
        public int ControlZoneGetFadeDistance(IntPtr control, out float fadeX, out float fadeY, out float fadeZ)
        {
            try
            {
                return ovrAudio_ControlZoneGetFadeDistance(control, out fadeX, out fadeY, out fadeZ);
            }
            catch
            {
                // Hack for v60 compatibility
                return ovrAudio_ControlVolumeGetFadeDistance(control, out fadeX, out fadeY, out fadeZ);
            }
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlZoneSetFrequency(IntPtr control, ControlZoneProperty property, float frequency, float value);
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlVolumeSetFrequency(IntPtr control, ControlZoneProperty property, float frequency, float value);
        public int ControlZoneSetFrequency(IntPtr control, ControlZoneProperty property, float frequency, float value)
        {
            try
            {
                return ovrAudio_ControlZoneSetFrequency(control, property, frequency, value);
            }
            catch
            {
                // Hack for v60 compatibility
                return ovrAudio_ControlVolumeSetFrequency(control, property, frequency, value);
            }
        }
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlZoneReset(IntPtr control, ControlZoneProperty property);
        [DllImport(binaryName)]
        private static extern int ovrAudio_ControlVolumeReset(IntPtr control, ControlZoneProperty property);
        public int ControlZoneReset(IntPtr control, ControlZoneProperty property)
        {
            try
            {
                return ovrAudio_ControlZoneReset(control, property);
            }
            catch
            {
                // Hack for v60 compatibility
                return ovrAudio_ControlVolumeReset(control, property);
            }
        }
    }
    public class DummyInterface : INativeInterface
    {
        /***********************************************************************************/
        // Settings API
        public int SetAcousticModel(AcousticModel model) => -1;
        public int ResetReverb() => -1;
        public int SetEnabled(int feature, bool enabled) => -1;
        public int SetEnabled(EnableFlagInternal feature, bool enabled) => -1;

        /***********************************************************************************/
        // Geometry API
        public int CreateAudioGeometry(out IntPtr geometry) { geometry = IntPtr.Zero; return -1; }
        public int DestroyAudioGeometry(IntPtr geometry) => -1;
        public int AudioGeometrySetObjectFlag(IntPtr geometry, ObjectFlags flag, bool enabled) => -1;
        public int AudioGeometryUploadMeshArrays(IntPtr geometry,
                                                        float[] vertices, int vertexCount,
                                                        int[] indices, int indexCount,
                                                        MeshGroup[] groups, int groupCount) => -1;
        public int AudioGeometryUploadSimplifiedMeshArrays(IntPtr geometry,
                                                        float[] vertices, int vertexCount,
                                                        int[] indices, int indexCount,
                                                        MeshGroup[] groups, int groupCount,
                                                        ref MeshSimplification simplification) => -1;
        public int AudioGeometrySetTransform(IntPtr geometry, in Matrix4x4 matrix) => -1;
        public int AudioGeometryGetTransform(IntPtr geometry, out float[] matrix4x4) { matrix4x4 = null; return -1; }
        public int AudioGeometryWriteMeshFile(IntPtr geometry, string filePath) => -1;
        public int AudioGeometryReadMeshFile(IntPtr geometry, string filePath) => -1;
        public int AudioGeometryReadMeshMemory(IntPtr geometry, IntPtr data, UInt64 dataLength) => -1;
        public int AudioGeometryWriteMeshFileObj(IntPtr geometry, string filePath) => -1;

        public int AudioGeometryGetSimplifiedMesh(IntPtr geometry, out float[] vertices, out uint[] indices, out uint[] materialIndices) { vertices = null; indices = null; materialIndices = null; return -1; }

        /***********************************************************************************/
        // Material API
        public int AudioMaterialGetFrequency(IntPtr material, MaterialProperty property, float frequency, out float value) { value = 0.0f; return -1; }
        public int CreateAudioMaterial(out IntPtr material) { material = IntPtr.Zero; return -1; }
        public int DestroyAudioMaterial(IntPtr material) => -1;
        public int AudioMaterialSetFrequency(IntPtr material, MaterialProperty property, float frequency, float value) => -1;
        public int AudioMaterialReset(IntPtr material, MaterialProperty property) => -1;
        /***********************************************************************************/
        // Acoustic Map API
        public int CreateAudioSceneIR(out IntPtr sceneIR) { sceneIR = IntPtr.Zero; return -1; }
        public int DestroyAudioSceneIR(IntPtr sceneIR) => -1;
        public int AudioSceneIRSetEnabled(IntPtr sceneIR, bool enabled) => -1;
        public int AudioSceneIRGetEnabled(IntPtr sceneIR, out bool enabled) { enabled = false; return -1; }
        public int AudioSceneIRGetStatus(IntPtr sceneIR, out AcousticMapStatus status) { status = AcousticMapStatus.EMPTY; return -1; }
        public int InitializeAudioSceneIRParameters(out MapParameters parameters) { parameters = new MapParameters(); return -1; }
        public int AudioSceneIRCompute(IntPtr sceneIR, ref MapParameters parameters) => -1;
        public int AudioSceneIRComputeCustomPoints(IntPtr sceneIR,
            float[] points, UIntPtr pointCount, ref MapParameters parameters) => -1;
        public int AudioSceneIRGetPointCount(IntPtr sceneIR, out UIntPtr pointCount) { pointCount = UIntPtr.Zero; return -1; }
        public int AudioSceneIRGetPoints(IntPtr sceneIR, float[] points, UIntPtr maxPointCount) => -1;
        public int AudioSceneIRSetTransform(IntPtr sceneIR, in Matrix4x4 matrix) => -1;
        public int AudioSceneIRGetTransform(IntPtr sceneIR, out float[] matrix4x4) { matrix4x4 = new float[16]; return -1; }
        public int AudioSceneIRWriteFile(IntPtr sceneIR, string filePath) => -1;
        public int AudioSceneIRReadFile(IntPtr sceneIR, string filePath) => -1;
        public int AudioSceneIRReadMemory(IntPtr sceneIR, IntPtr data, UInt64 dataLength) => -1;

        /***********************************************************************************/
        // Control Zone API
        public int CreateControlZone(out IntPtr control) { control = IntPtr.Zero; return -1; }
        public int DestroyControlZone(IntPtr control) => -1;
        public int ControlZoneSetEnabled(IntPtr control, bool enabled) => -1;
        public int ControlZoneGetEnabled(IntPtr control, out bool enabled) { enabled = false; return -1; }
        public int ControlZoneSetTransform(IntPtr control, in Matrix4x4 matrix) => -1;
        public int ControlZoneGetTransform(IntPtr control, out float[] matrix4x4) { matrix4x4 = new float[16]; return -1; }
        public int ControlZoneSetBox(IntPtr control, float sizeX, float sizeY, float sizeZ) => -1;
        public int ControlZoneGetBox(IntPtr control, out float sizeX, out float sizeY, out float sizeZ) { sizeX = 0.0f; sizeY = 0.0f; sizeZ = 0.0f; return -1; }
        public int ControlZoneSetFadeDistance(IntPtr control, float fadeX, float fadeY, float fadeZ) => -1;
        public int ControlZoneGetFadeDistance(IntPtr control, out float fadeX, out float fadeY, out float fadeZ) { fadeX = 0.0f; fadeY = 0.0f; fadeZ = 0.0f; return -1; }
        public int ControlZoneSetFrequency(IntPtr control, ControlZoneProperty property, float frequency, float value) => -1;
        public int ControlZoneReset(IntPtr control, ControlZoneProperty property) => -1;
    }
}
