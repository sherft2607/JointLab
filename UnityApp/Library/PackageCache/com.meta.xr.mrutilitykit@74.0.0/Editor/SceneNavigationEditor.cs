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
using UnityEngine;
using UnityEditor;
using Meta.XR.MRUtilityKit;

[CustomEditor(typeof(SceneNavigation))]
public class SceneNavigationEditor : Editor
{
    private SerializedProperty buildOnSceneLoadedProp;
    private SerializedProperty trackUpdatesProp;
    private SerializedProperty collectGeometryProp;
    private SerializedProperty collectObjectsProp;
    private SerializedProperty agentRadiusProp;
    private SerializedProperty agentHeightProp;
    private SerializedProperty agentClimbProp;
    private SerializedProperty agentMaxSlopeProp;
    private SerializedProperty agentsProp;
    private SerializedProperty navigableSurfacesProp;
    private SerializedProperty sceneObstaclesProp;
    private SerializedProperty layersProp;
    private SerializedProperty onNavMeshInitializedProp;
    private SerializedProperty connectRoomsInNavMeshProp;
    private SerializedProperty useSceneDataProp;
    private SerializedProperty customAgentProp;
    private SerializedProperty agentIndexProp;
    private SerializedProperty generateLinksProp;
    private SerializedProperty overrideVoxelSizeProp;
    private SerializedProperty voxelSizeProp;
    private SerializedProperty overrideTileSizeProp;
    private SerializedProperty tileSizeProp;

    private void OnEnable()
    {
        // Bind the properties
        buildOnSceneLoadedProp = serializedObject.FindProperty("BuildOnSceneLoaded");
        collectGeometryProp = serializedObject.FindProperty("CollectGeometry");
        collectObjectsProp = serializedObject.FindProperty("CollectObjects");
        agentRadiusProp = serializedObject.FindProperty("AgentRadius");
        agentHeightProp = serializedObject.FindProperty("AgentHeight");
        agentClimbProp = serializedObject.FindProperty("AgentClimb");
        agentMaxSlopeProp = serializedObject.FindProperty("AgentMaxSlope");
        agentsProp = serializedObject.FindProperty("Agents");
        navigableSurfacesProp = serializedObject.FindProperty("NavigableSurfaces");
        sceneObstaclesProp = serializedObject.FindProperty("SceneObstacles");
        layersProp = serializedObject.FindProperty("Layers");
        agentIndexProp = serializedObject.FindProperty("AgentIndex");
        onNavMeshInitializedProp = serializedObject.FindProperty("<OnNavMeshInitialized>k__BackingField");
        connectRoomsInNavMeshProp = serializedObject.FindProperty("connectRoomsInNavMesh");
        generateLinksProp = serializedObject.FindProperty("GenerateLinks");
        overrideVoxelSizeProp = serializedObject.FindProperty("OverrideVoxelSize");
        voxelSizeProp = serializedObject.FindProperty("VoxelSize");
        overrideTileSizeProp = serializedObject.FindProperty("OverrideTileSize");
        tileSizeProp = serializedObject.FindProperty("TileSize");
        useSceneDataProp = serializedObject.FindProperty("UseSceneData");
        customAgentProp = serializedObject.FindProperty("CustomAgent");
    }

    public override void OnInspectorGUI()
    {
        var sceneNavigation = (SceneNavigation)target;

        if (!sceneNavigation)
        {
            return;
        }

        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(),
                false);
        }

        serializedObject.Update();
        EditorGUILayout.PropertyField(buildOnSceneLoadedProp);
        EditorGUILayout.Space(8);
        EditorGUILayout.PropertyField(useSceneDataProp);
        EditorGUILayout.Space(2);
        if (!sceneNavigation.UseSceneData)
        {
            // Conditionally display CollectGeometry and CollectObjects based on _useSceneData
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(collectGeometryProp);
            EditorGUILayout.PropertyField(collectObjectsProp);
#if UNITY_2022_3_OR_NEWER
            EditorGUILayout.PropertyField(generateLinksProp);
#endif
            EditorGUI.indentLevel--;
        }
        else
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(navigableSurfacesProp);
            EditorGUILayout.PropertyField(sceneObstaclesProp);
            EditorGUILayout.Space(4);
            EditorGUILayout.PropertyField(overrideVoxelSizeProp);
            EditorGUILayout.Space(2);
            if (sceneNavigation.OverrideVoxelSize)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(voxelSizeProp);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(2);
            EditorGUILayout.PropertyField(overrideTileSizeProp);
            if (sceneNavigation.OverrideTileSize)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(tileSizeProp);
                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space(8);
        EditorGUILayout.PropertyField(customAgentProp);
        EditorGUILayout.Space(2);
        // Conditionally display agent properties based on _customAgent
        if (sceneNavigation.CustomAgent)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(agentRadiusProp);
            EditorGUILayout.PropertyField(agentHeightProp);
            EditorGUILayout.PropertyField(agentClimbProp);
            EditorGUILayout.PropertyField(agentMaxSlopeProp);
            EditorGUI.indentLevel--;
        }
        else
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(agentIndexProp);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space(8);

        EditorGUILayout.PropertyField(layersProp);
        EditorGUILayout.Space(8);

        EditorGUILayout.PropertyField(agentsProp, true); // True to handle array size
        EditorGUILayout.PropertyField(onNavMeshInitializedProp);
        EditorGUILayout.Space(4);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Build NavMesh"))
        {
            if (EnsurePlayMode())
            {
                ExecuteAction(() => sceneNavigation.BuildSceneNavMesh(),
                    "NavMesh successfully built.");
            }
        }

        GUILayout.Space(8);
        if (GUILayout.Button("Clear NavMesh Data"))
        {
            if (EnsurePlayMode())
            {
                ExecuteAction(() => sceneNavigation.RemoveNavMeshData(),
                    "NavMesh data removed.");
            }
        }

        GUILayout.EndHorizontal();
        EditorGUILayout.Space(4);

        serializedObject.ApplyModifiedProperties();
    }

    private bool EnsurePlayMode()
    {
        if (Application.isPlaying)
        {
            return true;
        }

        EditorUtility.DisplayDialog("Operation Not Available",
            "This operation is only available in Play Mode.", "OK");
        return false;
    }

    private void ExecuteAction(Action action, string successMessage)
    {
        try
        {
            action();
            Debug.Log(successMessage);
        }
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("Operation Not Available", e.Message, "OK");
        }
    }
}
