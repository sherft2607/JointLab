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
 * Filename    :   MetaXRAcousticSceneGroupEditor.cs
 * Content     :   Acoustic scene group editor class
 ***********************************************************************************/
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(MetaXRAcousticSceneGroup))]
internal class MetaXRAcousticSceneGroupEditor : Editor
{
    SerializedProperty sceneGuids;
    ReorderableList list;

    private void OnEnable()
    {
        sceneGuids = serializedObject.FindProperty("sceneGuids");

        list = new ReorderableList(serializedObject, sceneGuids, true, true, true, true)
        {
            drawElementCallback = DrawListItems,
            drawHeaderCallback = DrawHeader
        };
    }

    void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
        SceneAsset asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(AssetDatabase.GUIDToAssetPath(element.stringValue));
        SceneAsset newAsset = EditorGUI.ObjectField(rect, asset, typeof(SceneAsset), false) as SceneAsset;
        element.stringValue = AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(newAsset)).ToString();
    }

    void DrawHeader(Rect rect) => EditorGUI.LabelField(rect, "Scenes");

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        list.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}
