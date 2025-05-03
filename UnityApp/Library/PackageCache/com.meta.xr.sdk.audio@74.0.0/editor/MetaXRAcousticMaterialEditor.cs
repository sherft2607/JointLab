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
 * Filename    :   MetaXRAcousticMaterialEditor.cs
 * Content     :   Propagation acoustic material editor class
                Attach to geometry to define material properties
 ***********************************************************************************/
using UnityEditor;
using UnityEngine;
using static MetaXRAcousticMaterialProperties;
using Spectrum = Meta.XR.Acoustics.Spectrum;

[CanEditMultipleObjects]
[CustomEditor(typeof(MetaXRAcousticMaterial))]
internal class MetaXRAcousticMaterialEditor : Editor
{
    internal static readonly Color kAbsorptionSpectrumColor = new Color(0.52f, 0.68f, 1f);
    internal static readonly Color kTransmissionSpectrumColor = AudioCurveRendering.kAudioOrange;
    internal static readonly Color kScatteringSpectrumColor = new Color(1f, 0.25f, 0.25f);
    private MetaXRAudioSpectrumEditor absorption;
    private MetaXRAudioSpectrumEditor transmission;
    private MetaXRAudioSpectrumEditor scattering;

    private void OnEnable()
    {
        MetaXRAcousticMaterial mat = target as MetaXRAcousticMaterial;
        if (absorption == null)
            absorption = new MetaXRAudioSpectrumEditor("Absorption", "Acoustic energy absorbed by the material", MetaXRAudioSpectrumEditor.AxisScale.Cube, kAbsorptionSpectrumColor);

        if (transmission == null)
            transmission = new MetaXRAudioSpectrumEditor("Transmission", "Acoustic energy transmitted through the material", MetaXRAudioSpectrumEditor.AxisScale.Cube, kTransmissionSpectrumColor);

        if (scattering == null)
            scattering = new MetaXRAudioSpectrumEditor("Scattering", "Acoustic energy scattered by material", MetaXRAudioSpectrumEditor.AxisScale.Linear, kScatteringSpectrumColor);

        absorption.LoadFoldoutState();
        transmission.LoadFoldoutState();
        scattering.LoadFoldoutState();
    }

    private void OnDisable()
    {
        absorption.SaveFoldoutState();
        transmission.SaveFoldoutState();
        scattering.SaveFoldoutState();
    }

    public override void OnInspectorGUI()
    {
        MetaXRAcousticMaterial mat0 = target as MetaXRAcousticMaterial;
        MetaXRAcousticMaterialProperties props0 = mat0.Properties;
        bool hadCustomData0 = false;
        bool hasCustomData = false;
        Meta.XR.Acoustics.MaterialData data0 = mat0.Data;
        Color color0 = mat0.Color;

        bool propsIsMixed = false;
        bool customDataIsMixed = false;

        foreach (Object t in targets)
        {
            MetaXRAcousticMaterial mat = t as MetaXRAcousticMaterial;
            if (mat.Properties != props0)
                propsIsMixed = true;
        }

        EditorGUI.BeginChangeCheck();

        Rect r = EditorGUILayout.GetControlRect();
        r.width -= MetaXRAudioSpectrumEditor.GetRightMargin();

        bool isInvalid = (props0 == null && !hadCustomData0);

        EditorGUI.showMixedValue = propsIsMixed;
        MetaXRAcousticMaterialProperties props = EditorGUI.ObjectField(r, props0, typeof(MetaXRAcousticMaterialProperties), false) as MetaXRAcousticMaterialProperties;
        if (props != props0)
            propsIsMixed = false;

        r = EditorGUILayout.GetControlRect();
        r.width -= MetaXRAudioSpectrumEditor.GetRightMargin();
        EditorGUI.showMixedValue = customDataIsMixed;

        // Make sure to reset this to false at the end otherwise other components will be displayed incorrectly
        EditorGUI.showMixedValue = false;

        bool shouldCopyFromPreset = !hadCustomData0 && hasCustomData && mat0.customData.IsEmpty && props != null;

        // first time we "override" we copy current shared properties
        if (shouldCopyFromPreset)
            Debug.Log($"Copying custom acoustic material properties from {props}");

        BuiltinPreset oldPreset = hasCustomData ? BuiltinPreset.Custom : (props != null ? props.Preset : BuiltinPreset.Custom);
        BuiltinPreset newPreset = oldPreset;
        Color newColor = color0;
        Spectrum newAbsorption = data0?.absorption;
        Spectrum newTransmission = data0?.transmission;
        Spectrum newScattering = data0?.scattering;

        bool shouldShow = data0 != null && !customDataIsMixed;
        if (!hasCustomData)
        {
            if (propsIsMixed)
                shouldShow = false;

            if (newAbsorption == null || newTransmission == null || newScattering == null)
                shouldShow = false;
        }

        if (shouldShow)
        {
            EditorGUI.BeginDisabledGroup(data0 != mat0.customData);

            r = EditorGUILayout.GetControlRect();
            r.width -= MetaXRAudioSpectrumEditor.GetRightMargin();

            newPreset = (BuiltinPreset)EditorGUI.EnumPopup(r, "Preset", oldPreset);

            newColor = EditorGUILayout.ColorField(color0);

            Event e = Event.current;
            Debug.Assert(mat0?.customData != mat0?.Properties?.Data);
            absorption.Draw(mat0.Data.absorption, e);
            transmission.Draw(mat0.Data.transmission, e);
            scattering.Draw(mat0.Data.scattering, e);
            EditorGUI.EndDisabledGroup();
        }
        else if (isInvalid)
        {
            char warning = '\u26A0';
            EditorGUILayout.LabelField($"{warning} No material properties specified, you must select a preset!", new GUIStyle
            {
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Bold,
                wordWrap = true,
                normal = new GUIStyleState { textColor = Color.yellow },
            });
        }

        if (EditorGUI.EndChangeCheck())
        {
            foreach (Object t in targets)
            {
                MetaXRAcousticMaterial mat = t as MetaXRAcousticMaterial;
                if (!propsIsMixed)
                    mat.Properties = props;
            }
        }
    }
}
