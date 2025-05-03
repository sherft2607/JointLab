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
 * Filename    :   MetaXRAcousticMaterial.cs
 * Content     :   Acoustic material class
 ***********************************************************************************/
using Meta.XR.Acoustics;
using System;
using System.Linq;
using UnityEngine;
using Native = MetaXRAcousticNativeInterface;

/// \brief This class is used for to create or edit an Acoustic Material
public sealed class MetaXRAcousticMaterial : MonoBehaviour, Meta.XR.Acoustics.IMaterialDataProvider
{
    //***********************************************************************
    // Public Fields

    /// \brief The properties of this material
    /// \see MetaXRAcousticMaterialProperties
    [SerializeField]
    private MetaXRAcousticMaterialProperties properties;
    internal MetaXRAcousticMaterialProperties Properties
    {
        get => properties;
        set => properties = value;
    }

    /// \brief If true the user has edited the properties beyond the default presets
    [SerializeField]
    private bool hasCustomData = false;

    /// \brief This is a reference to the material properties the user has edited
    [SerializeField]
    internal Meta.XR.Acoustics.MaterialData customData = null;
    /// \brief This holds either a material preset or the user's custom material data
    public Meta.XR.Acoustics.MaterialData Data => hasCustomData ? customData : properties?.Data;

    internal Color Color => Data != null ? Data.color : Color.magenta;
    internal void CopyPresetToCustomData(MetaXRAcousticMaterialProperties.BuiltinPreset preset)
    {
        if (!hasCustomData)
        {
            Debug.LogError("Material doesn't have custom data", gameObject);
            return;
        }
        MetaXRAcousticMaterialProperties.SetPreset(preset, ref customData);
    }

#if UNITY_EDITOR
    internal void AppendHash(ref Hash128 hash)
    {
        if (!hasCustomData && properties == null)
            return;

        foreach (Meta.XR.Acoustics.Spectrum.Point p in Data.absorption.points)
        {
            hash.Append(p.frequency);
            hash.Append(p.data);
        }
        foreach (Meta.XR.Acoustics.Spectrum.Point p in Data.transmission.points)
        {
            hash.Append(p.frequency);
            hash.Append(p.data);
        }
        foreach (Meta.XR.Acoustics.Spectrum.Point p in Data.scattering.points)
        {
            hash.Append(p.frequency);
            hash.Append(p.data);
        }
    }

#endif
    //***********************************************************************
    // Private Fields

    [NonSerialized]
    private IntPtr materialHandle = IntPtr.Zero;

    //***********************************************************************
    // Start / Destroy

    /// Initialize the audio material. This is called after Awake() and before the first Update().
    void Start()
    {
        if (!gameObject.isStatic)
            StartInternal();
    }

    internal bool StartInternal()
    {
        // Ensure that the material is not initialized twice.
        if (materialHandle != IntPtr.Zero)
            return true; // already initialized

        // Create the internal material.
        materialHandle = CreateMaterialNativeHandle(Data);

        return true;
    }

    /// Destroy the audio scene. This is called when the scene is deleted.
    void OnDestroy()
    {
        DestroyInternal();
    }

    internal void DestroyInternal()
    {
        if (materialHandle != IntPtr.Zero)
        {
            DestroyMaterialNativeHandle(materialHandle);
            materialHandle = IntPtr.Zero;
        }
    }

    //***********************************************************************
    // Upload
    internal bool ApplyMaterialProperties()
    {
        return ApplyPropertiesToNative(materialHandle, Data);
    }

    internal static IntPtr CreateMaterialNativeHandle(Meta.XR.Acoustics.MaterialData data = null)
    {
        IntPtr handle = IntPtr.Zero;
        if (Native.Interface.CreateAudioMaterial(out handle) != MetaXRAcousticGeometry.Success)
        {
            Debug.LogError("Unable to create internal audio material");
            return handle;
        }

        if (data != null)
            ApplyPropertiesToNative(handle, data);

        return handle;
    }

    internal static void DestroyMaterialNativeHandle(IntPtr handle)
    {
        // Destroy the material.
        Native.Interface.DestroyAudioMaterial(handle);
    }

    private static bool ApplyPropertiesToNative(IntPtr handle, Meta.XR.Acoustics.MaterialData data)
    {
        return ApplyPropertiesToNative(handle, data, null);
    }

    private static bool ApplyPropertiesToNative(IntPtr handle, Meta.XR.Acoustics.MaterialData data, GameObject gameObject)
    {
        if (handle == IntPtr.Zero || data == null)
        {
            if (gameObject != null)
            {
                string path = ((gameObject.scene != null) ? gameObject.scene.name : "") + ":" + string.Join("/", gameObject.GetComponentsInParent<Transform>().Select(t => t.name).Reverse().ToArray());
                Debug.LogWarning($"Acoustic Material configured with empty properties: {path}", gameObject);
            }
            return false;
        }

        // Absorption
        Native.Interface.AudioMaterialReset(handle, MaterialProperty.ABSORPTION);
        foreach (Meta.XR.Acoustics.Spectrum.Point p in data.absorption.points)
        {
            Native.Interface.AudioMaterialSetFrequency(handle, MaterialProperty.ABSORPTION,
                                                          p.frequency, p.data);
        }

        // Transmission
        Native.Interface.AudioMaterialReset(handle, MaterialProperty.TRANSMISSION);
        foreach (Meta.XR.Acoustics.Spectrum.Point p in data.transmission.points)
        {
            Native.Interface.AudioMaterialSetFrequency(handle, MaterialProperty.TRANSMISSION,
                                                          p.frequency, p.data);
        }

        // Scattering
        Native.Interface.AudioMaterialReset(handle, MaterialProperty.SCATTERING);
        foreach (Meta.XR.Acoustics.Spectrum.Point p in data.scattering.points)
        {
            Native.Interface.AudioMaterialSetFrequency(handle, MaterialProperty.SCATTERING,
                                                          p.frequency, p.data);
        }

        return true;
    }
}
