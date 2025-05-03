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
 * Filename    :   MetaXRAcousticControlZone.cs
 * Content     :   Class for modifying acoustic properties within a 3D zone.
 ***********************************************************************************/

using Meta.XR.Acoustics;
using System;
using System.Collections.Generic;
using UnityEngine;
using Native = MetaXRAcousticNativeInterface;
using Point = Meta.XR.Acoustics.Spectrum.Point;
using Spectrum = Meta.XR.Acoustics.Spectrum;

/// \brief Class that provides control over the Control Zone behavior
///
/// The Control Zone is a 3D box that can be used to modify the reverb properties of points inside it. Use the transform to adjust the size and position of the Control Zone.
///
/// \see MetaXRAcousticNativeInterface
internal sealed class MetaXRAcousticControlZone : MonoBehaviour
{
    //***********************************************************************
    // Public Fields
    [Serializable]
    internal class State
    {
        [SerializeField]
        internal Color color = Color.blue;
        [SerializeField]
        /// \brief Adjusts the reverb decay time of points inside the zone
        internal Spectrum rt60 = new Spectrum();
        [SerializeField]
        /// \brief Adjusts the reverb level of points inside the zone
        internal Spectrum reverbLevel = new Spectrum();
        [SerializeField]
        /// \brief A value in meters to describe the distance over which to fade out the Control Zone adjustments starting from the boundary
        internal float fadeDistance = 1.0f;
        internal void Clone(State other)
        {
            color = other.color;
            reverbLevel.Clone(other.reverbLevel);
            rt60.Clone(other.rt60);
            fadeDistance = other.fadeDistance;

        }
    }

    [SerializeField]
    private State _state = new State();
    internal State state => _state;

    internal Color ZoneColor { get => _state.color; set => _state.color = value; }

    internal Spectrum Rt60 { get => _state.rt60; set => _state.rt60 = value; }

    internal Spectrum ReverbLevel { get => _state.reverbLevel; set => _state.reverbLevel = value; }

    internal float FadeDistance
    {
        get => _state.fadeDistance;
        set
        {
            _state.fadeDistance = value;
            ApplyTransform();
        }
    }

    private Vector3 NativeFadeDistance => new Vector3(_state.fadeDistance / transform.localScale.x, _state.fadeDistance / transform.localScale.y, _state.fadeDistance / transform.localScale.z);
    private Vector3 NativeBoxSize => new Vector3(2.0f + NativeFadeDistance.x, 2.0f + NativeFadeDistance.y, 2.0f + NativeFadeDistance.z);

    internal void Clone(State other)
    {
        _state.Clone(other);
    }

    //***********************************************************************
    // Private Fields

    private IntPtr _controlHandle = IntPtr.Zero;

    //***********************************************************************
    // Start / Destroy

    internal MetaXRAcousticControlZone()
    {
        Rt60.points = new List<Point>() { new Point(1000f, 0.0f) };
        ReverbLevel.points = new List<Point>() { new Point(1000f, 0.0f) };
    }

    /// Initialize the Control Zone. This is called after Awake() and before the first Update().
    void Start()
    {
        StartInternal();
    }

    internal void StartInternal()
    {
        // Ensure that the Control Zone is not initialized twice.
        if (_controlHandle != IntPtr.Zero)
            return;

        // Create the internal Control Zone.
        if (Native.Interface.CreateControlZone(out _controlHandle) != 0)
        {
            Debug.LogError("Unable to create internal Control Zone", gameObject);
            return;
        }

        // Run the updates to initialize the control.
        ApplyProperties();
    }

    /// Destroy the audio scene. This is called when the scene is deleted.
    void OnDestroy()
    {
        DestroyInternal();
    }

    internal void DestroyInternal()
    {
        if (_controlHandle != IntPtr.Zero)
        {
            // Destroy the control.
            Native.Interface.DestroyControlZone(_controlHandle);
            _controlHandle = IntPtr.Zero;
        }
    }

    //***********************************************************************

    /// Called when enabled.
    void OnEnable()
    {
        if (_controlHandle == IntPtr.Zero)
            return;

        Native.Interface.ControlZoneSetEnabled(_controlHandle, true);
    }

    /// Called when disabled.
    void OnDisable()
    {
        if (_controlHandle == IntPtr.Zero)
            return;

        Native.Interface.ControlZoneSetEnabled(_controlHandle, false);
    }

    //***********************************************************************
    // Updates

    void LateUpdate()
    {
        if (_controlHandle == IntPtr.Zero)
            return;

        if (transform.hasChanged)
        {
            ApplyTransform();

            // Reset dirty bit.
            transform.hasChanged = false;
        }
    }

    //***********************************************************************
    // Upload

    private void ApplyTransform()
    {
        if (_controlHandle == IntPtr.Zero)
            return;

        Native.Interface.ControlZoneSetBox(
            _controlHandle, NativeBoxSize.x, NativeBoxSize.y, NativeBoxSize.z);
        Native.Interface.ControlZoneSetFadeDistance(
            _controlHandle, NativeFadeDistance.x, NativeFadeDistance.y, NativeFadeDistance.z);

        Native.Interface.ControlZoneSetTransform(_controlHandle, transform.localToWorldMatrix);
    }

    internal void ApplyProperties()
    {
        if (_controlHandle == IntPtr.Zero)
            return;

        ApplyTransform();

        // RT60
        Native.Interface.ControlZoneReset(_controlHandle, ControlZoneProperty.RT60);
        foreach (Point p in Rt60.points)
        {
            Native.Interface.ControlZoneSetFrequency(
                _controlHandle, ControlZoneProperty.RT60, p.frequency, p.data);
        }

        // Reverb level
        Native.Interface.ControlZoneReset(_controlHandle, ControlZoneProperty.REVERB_LEVEL);
        foreach (Point p in ReverbLevel.points)
        {
            Native.Interface.ControlZoneSetFrequency(
                _controlHandle, ControlZoneProperty.REVERB_LEVEL, p.frequency, p.data);
        }
    }

    //***********************************************************************
    // Debug Drawing

#if UNITY_EDITOR
    /// Draw the editor debug view of the control.
    void OnDrawGizmos()
    {
        drawDebug(false);
    }

    /// Draw the editor debug view of the control when selected
    void OnDrawGizmosSelected()
    {
        drawDebug(true);
    }

    private void drawDebug(bool selected)
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Color baseColor = ZoneColor;

        // Box outline
        Gizmos.color = baseColor;
        Gizmos.DrawWireCube(new Vector3(), NativeBoxSize);

        // Box shading
        Gizmos.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0.1f);
        Gizmos.DrawCube(new Vector3(), NativeBoxSize);

        // Inner box
        Gizmos.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0.2f);
        Vector3 innerSize = Vector3.Max(Vector3.one * 2.0f, Vector3.zero);
        Gizmos.DrawCube(new Vector3(), innerSize);
    }
#endif // UNITY_EDITOR

}
