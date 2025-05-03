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
 * Filename    :   MetaXRAcousticMaterialMappingEditor.cs
 * Content     :   Acoustic scene group editor class
 ***********************************************************************************/
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(MetaXRAcousticMaterialMapping))]
internal class MetaXRAcousticMaterialMappingEditor : Editor
{
    SerializedProperty mapping;
    SerializedProperty fallbackMaterial;
    ReorderableList list;

    private void OnEnable()
    {
        mapping = serializedObject?.FindProperty("mapping");
        fallbackMaterial = serializedObject?.FindProperty("fallbackMaterial");
        Debug.Assert(fallbackMaterial != null);

        list = new ReorderableList(serializedObject, mapping, true, true, true, true)
        {
            drawElementCallback = DrawListItems,
            drawHeaderCallback = DrawHeader
        };
    }

    void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
        var propPhysics = element.FindPropertyRelative(nameof(MetaXRAcousticMaterialMapping.Pair.physicMaterial));

        float totalWidth = rect.width;

        // reserve a square for color box
        totalWidth -= rect.height;

        rect.width = totalWidth / 2f;
        var propAcoustic = element.FindPropertyRelative(nameof(MetaXRAcousticMaterialMapping.Pair.acousticMaterial));
        var acousticMat = propAcoustic?.objectReferenceValue as MetaXRAcousticMaterialProperties;

        PhysicMaterial newPhysicMaterial = EditorGUI.ObjectField(rect, propPhysics?.objectReferenceValue, typeof(PhysicMaterial), false) as PhysicMaterial;
        if (propPhysics != null)
            propPhysics.objectReferenceValue = newPhysicMaterial;

        rect.x += rect.width;
        MetaXRAcousticMaterialProperties newProps = DrawAcousticMaterialEditor(rect, acousticMat);
        if (propAcoustic != null)
            propAcoustic.objectReferenceValue = newProps;
    }

    void DrawHeader(Rect rect) => EditorGUI.LabelField(rect, "Mapping");

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        ShowFallbackMaterial();
        list.DoLayoutList();

        if (EditorGUI.EndChangeCheck())
        {
            if (UnityEditor.VersionControl.Provider.isActive)
            {
                var checkout = UnityEditor.VersionControl.Provider.Checkout(MetaXRAcousticMaterialMapping.FullPath(), UnityEditor.VersionControl.CheckoutMode.Asset);
                checkout.Wait();
                Debug.Log($"checkout {MetaXRAcousticMaterialMapping.FullPath()}: success = {checkout.success}");
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

    private static MetaXRAcousticMaterialProperties DrawAcousticMaterialEditor(Rect rect, MetaXRAcousticMaterialProperties acousticMat)
    {
        MetaXRAcousticMaterialProperties newAcousticMaterial = EditorGUI.ObjectField(rect, acousticMat, typeof(MetaXRAcousticMaterialProperties), false) as MetaXRAcousticMaterialProperties;

        float padding = 2.0f;
        rect.x += rect.width + padding;
        rect.width = rect.height - padding * 2;
        rect.height -= padding * 2;
        rect.y += padding;
        Color matColor = (acousticMat?.Data == null) ? GUI.backgroundColor : acousticMat.Data.color;
        EditorGUI.DrawRect(rect, matColor);

        return newAcousticMaterial;
    }

    private void ShowFallbackMaterial()
    {
        Rect rect = EditorGUILayout.GetControlRect();
        float totalWidth = rect.width;
        // reserve a square for color box
        totalWidth -= rect.height;
        rect.width = totalWidth / 2f;
        EditorGUI.LabelField(rect, "Fallback Material");
        rect.x += rect.width;
        fallbackMaterial.objectReferenceValue = DrawAcousticMaterialEditor(rect, fallbackMaterial.objectReferenceValue as MetaXRAcousticMaterialProperties);
    }
}
