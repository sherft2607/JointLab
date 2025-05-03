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
 * Filename    :   MetaXRAcousticMaterialPropertiesEditor.cs
 * Content     :   Acoustic material properties editor class
 ***********************************************************************************/
using UnityEditor;
using UnityEngine;
using static MetaXRAcousticMaterialProperties;
using Spectrum = Meta.XR.Acoustics.Spectrum;

[CustomEditor(typeof(MetaXRAcousticMaterialProperties))]
internal class MetaXRAcousticMaterialPropertiesEditor : Editor
{
    private MetaXRAudioSpectrumEditor absorption;
    private MetaXRAudioSpectrumEditor transmission;
    private MetaXRAudioSpectrumEditor scattering;

    private void OnEnable()
    {
        MetaXRAcousticMaterialProperties props = target as MetaXRAcousticMaterialProperties;
        if (absorption == null)
            absorption = new MetaXRAudioSpectrumEditor("Absorption", "Acoustic energy absorbed by the material", MetaXRAudioSpectrumEditor.AxisScale.Cube, MetaXRAcousticMaterialEditor.kAbsorptionSpectrumColor);

        if (transmission == null)
            transmission = new MetaXRAudioSpectrumEditor("Transmission", "Acoustic energy transmitted through the material", MetaXRAudioSpectrumEditor.AxisScale.Cube, MetaXRAcousticMaterialEditor.kTransmissionSpectrumColor);

        if (scattering == null)
            scattering = new MetaXRAudioSpectrumEditor("Scattering", "Acoustic energy scattered by material", MetaXRAudioSpectrumEditor.AxisScale.Linear, MetaXRAcousticMaterialEditor.kScatteringSpectrumColor);

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
        //serializedObject.Update();

        MetaXRAcousticMaterialProperties props = target as MetaXRAcousticMaterialProperties;

        EditorGUI.BeginChangeCheck();

        Rect r = EditorGUILayout.GetControlRect();
        r.width -= MetaXRAudioSpectrumEditor.GetRightMargin();

        BuiltinPreset newPreset = (BuiltinPreset)EditorGUI.EnumPopup(r, "Preset", props.Preset);

        Color newColor = EditorGUILayout.ColorField(props.Data.color);

        Event e = Event.current;
        absorption.Draw(props.Data.absorption, e);
        transmission.Draw(props.Data.transmission, e);
        scattering.Draw(props.Data.scattering, e);

        if (EditorGUI.EndChangeCheck())
        {
            ApplyChanges(props, newPreset, newColor);
        }
    }

    internal void ApplyChanges(MetaXRAcousticMaterialProperties props, BuiltinPreset newPreset, Color newColor)
    {
#if META_XR_ACOUSTIC_INFO
        Debug.Log("Apply changes");
#endif
        if (newPreset != props.Preset)
        {
            Undo.SetCurrentGroupName($"Change to {newPreset} preset");
        }

        string groupName = Undo.GetCurrentGroupName();
        Undo.RegisterCompleteObjectUndo(props, groupName);
#if META_XR_ACOUSTIC_INFO
        Debug.Log($"Log undo {groupName}: {props}");
#endif

        if (groupName == MetaXRAudioSpectrumEditor.pointAddedGroupName)
        {
#if META_XR_ACOUSTIC_INFO
            Debug.Log($"Collapsing undo operations");
#endif
            Undo.CollapseUndoOperations(Undo.GetCurrentGroup() - 1);
        }

        if (props.Preset != newPreset)
        {
#if META_XR_ACOUSTIC_INFO
            Debug.Log($"Preset changed from {props.Preset} to {newPreset}");
#endif
            props.Preset = newPreset;
        }
        else
        {
#if META_XR_ACOUSTIC_INFO
            Debug.Log($"Data changed");
#endif
            props.Preset = BuiltinPreset.Custom;
            props.Data.color = newColor;
        }

        // HACK: We shouldn't have to call SetDirty here but if we don't these ScriptableObject changes won't get written to disk! (tested in Unity 2021.3.11f1)
        EditorUtility.SetDirty(props);
        AssetDatabase.SaveAssetIfDirty(props);

        if (Application.isPlaying)
        {
            MetaXRAcousticMaterial[] mats = FindObjectsOfType<MetaXRAcousticMaterial>();
            foreach (MetaXRAcousticMaterial mat in mats)
            {
                if (mat.Properties = props)
                {
                    mat.ApplyMaterialProperties();
                }
            }
        }
    }
}
