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
 * Filename    :   MetaXRAcousticGeometryEditor.cs
 * Content     :   Geometry editor class
                Attach to geometry to define material properties
 ***********************************************************************************/

//#define ENABLE_DEBUG_EXPORT_OBJ

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MetaXRAcousticGeometry))]
public class MetaXRAcousticGeometryEditor : Editor
{
    private bool showAdvancedControls = false;
    private bool needsRebake = false;

    private void OnEnable()
    {
        MetaXRAcousticGeometry geo = target as MetaXRAcousticGeometry;

        // Determine if the geometry needs rebaking based on time stamp.
        bool isInvalid = geo.VertexCount == 0;
        DateTime timeStamp = DateTime.MinValue;
        string newFilePath = geo.AbsoluteFilePath;
        if (newFilePath != null)
        {
            // Get file time stamp.
            FileInfo fileInfo = new FileInfo(newFilePath);
            long fileSize = fileInfo.Exists ? fileInfo.Length : 0;
            timeStamp = System.IO.File.GetLastWriteTime(newFilePath);
            isInvalid = isInvalid && (fileSize == 0);
        }

        // The .IsOlder() function can take a long time. Call it just once when GUI is enabled.
        needsRebake = isInvalid ? true : geo.IsOlder(timeStamp);
    }

    public override void OnInspectorGUI()
    {
        MetaXRAcousticGeometry geo = (MetaXRAcousticGeometry)target;

        EditorGUI.BeginChangeCheck();

        bool newIncludeChildMeshes = EditorGUILayout.Toggle(new GUIContent("Include Child Meshes", "Include all child meshes into single geometry instance"), geo.IncludeChildMeshes);

        // Update controls to change Mesh Simplification Flags
        bool newEnableDiffraction = geo.EnableDiffraction;
        // Use default setting for advanced parameters if the settings are hidden
        float newMaxSimplifyError = geo.MaxSimplifyError;
        float newMinDiffractionEdgeAngle = geo.MinDiffractionEdgeAngle;
        float newMinDiffractionEdgeLength = geo.MinDiffractionEdgeLength;
        float newFlagLength = geo.FlagLength;
        bool newEnableMeshSimplification = geo.EnableSimplification;
        int newLODSelection = geo.LodSelection;
        bool newUseColliders = geo.UseColliders;
        bool newOverrideExcludeTagsEnabled = geo.OverrideExcludeTagsEnabled;
        string[] newOverrideExcludeTags = geo.ExcludeTags;
        bool newFileEnabled = geo.FileEnabled;
        bool pathChanged = false;
        string newFilePath = geo.AbsoluteFilePath;
        bool shouldWriteMesh = false;

        showAdvancedControls = EditorGUILayout.Foldout(showAdvancedControls, "Advanced Controls");
        if (showAdvancedControls)
        {
            EditorGUI.indentLevel++;
            newUseColliders = EditorGUILayout.Toggle(new GUIContent("Use Colliders", "Use the physics MeshColliders instead of the Visual geometry."), newUseColliders);
            newOverrideExcludeTagsEnabled = EditorGUILayout.Toggle(new GUIContent("Override Exclude Tags", "Override the Project Settings exclude tags to define custom exclude tags only for this geometry."), newOverrideExcludeTagsEnabled);

            using (new EditorGUI.DisabledGroupScope(!newOverrideExcludeTagsEnabled))
            {
                newOverrideExcludeTags = MetaXRAcousticSettingsProvider.ExcludeTagAsFlagsField(newOverrideExcludeTags);
            }
            Separator();

            EditorGUILayout.LabelField("Mesh Simplification", EditorStyles.boldLabel);
            newMaxSimplifyError = EditorGUILayout.Slider(new GUIContent("Max Error", "Maximum tolerable mesh simplification error in mesh-local units"), newMaxSimplifyError, 0.0f, 1.0f);
            newEnableMeshSimplification = (newMaxSimplifyError > 0.0f);
            const int LOD_LEVEL_MAX = 7; // https://docs.unity3d.com/Manual/LevelOfDetail.html
            newLODSelection = EditorGUILayout.IntSlider(new GUIContent("LOD", "Which LOD to use for the acoustic geometry when using an LOD Group. The lowest value of 0 corresponds to the highest quality mesh."), newLODSelection, 0, LOD_LEVEL_MAX);
            Separator();

            using (new EditorGUI.DisabledGroupScope(Application.isPlaying))
            {
                bool showFilePicker = false;
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PrefixLabel(new GUIContent("File Path:", "The path to the serialized mesh file, relative to the StreamingAssets directory"));

                    int indentLevelPrev = EditorGUI.indentLevel;
                    EditorGUI.indentLevel = 0;
                    EditorGUILayout.LabelField(geo.RelativeFilePath, EditorStyles.wordWrappedLabel);

                    if (GUILayout.Button("...", GUILayout.ExpandWidth(false)))
                    {
                        // defer displaying dialog to avoid layout error
                        showFilePicker = true;
                    }

                    EditorGUI.indentLevel = indentLevelPrev;
                }

                if (showFilePicker)
                {
                    if (!System.IO.Directory.Exists(Application.streamingAssetsPath))
                    {
                        System.IO.Directory.CreateDirectory(Application.streamingAssetsPath);
                    }

                    string directory = Application.streamingAssetsPath;
                    string fileName = geo.gameObject.name + "." + MetaXRAcousticGeometry.FILE_EXTENSION;

                    if (newFilePath != "")
                    {
                        string suggestedDirectory = System.IO.Path.GetDirectoryName(newFilePath);
                        while (!System.IO.Directory.Exists(suggestedDirectory))
                        {
                            suggestedDirectory = System.IO.Path.GetFullPath(suggestedDirectory + "/..");
                            Debug.Log($"suggest: {suggestedDirectory}");
                        }
                        directory = suggestedDirectory;
                        fileName = System.IO.Path.GetFileName(newFilePath);
                    }

                    newFilePath = EditorUtility.SaveFilePanel(
                        "Save baked mesh to file", directory, fileName,
                        MetaXRAcousticGeometry.FILE_EXTENSION);

                    // If the user canceled, use the old path.
                    if (string.IsNullOrEmpty(newFilePath))
                        newFilePath = geo.AbsoluteFilePath;
                    else
                        pathChanged = true;
                }

#if ENABLE_DEBUG_EXPORT_OBJ
                // this allows you to export the geometry to a .obj for viewing
                // in an external model viewer for debugging/validation
                if (GUILayout.Button("Write to .obj (debug)"))
                {
                    mesh.WriteToObj();
                }
#endif
            }

            EditorGUI.indentLevel--;
        } // Advanced Controls

        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.PrefixLabel(" ");
            shouldWriteMesh = GUILayout.Button("Bake Mesh");
        }

        //*****************************************************************************
        // Statistics
        bool isInvalid = geo.VertexCount == 0;
        DateTime timeStamp = DateTime.MinValue;
        EditorGUILayout.LabelField(new GUIContent("Vertices:", "The number of precomputed data points"), new GUIContent(geo.VertexCount.ToString()));
        if (newFilePath != null)
        {
            FileInfo fileInfo = new FileInfo(newFilePath);
            long fileSize = fileInfo.Exists ? fileInfo.Length : 0;
            timeStamp = System.IO.File.GetLastWriteTime(newFilePath);
            isInvalid = isInvalid && (fileSize == 0);
            EditorGUILayout.LabelField(new GUIContent("Data Size:", "The total size of the serialized data"), new GUIContent(MetaXRAcousticMapEditor.GetSizeString(fileSize)));
        }
        if (isInvalid)
        {
            char warning = '\u26A0';
            EditorGUILayout.LabelField($"{warning} Geometry contains no mesh data!", new GUIStyle
            {
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Bold,
                wordWrap = true,
                normal = new GUIStyleState { textColor = Color.yellow },
            });
        }
        else if (needsRebake)
        {
            char warning = '\u24D8';
            EditorGUILayout.LabelField($"{warning} Geometry stale, needs rebake!", new GUIStyle
            {
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Bold,
                wordWrap = true,
                normal = new GUIStyleState { textColor = Color.cyan },
            });
        }

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(geo, "Edited MetaXRAcousticGeometry");

            geo.IncludeChildMeshes = newIncludeChildMeshes;
            geo.FileEnabled = newFileEnabled;
            geo.EnableSimplification = newEnableMeshSimplification;
            geo.EnableDiffraction = newEnableDiffraction;
            geo.MaxSimplifyError = newMaxSimplifyError;
            geo.MinDiffractionEdgeAngle = newMinDiffractionEdgeAngle;
            geo.MinDiffractionEdgeLength = newMinDiffractionEdgeLength;
            geo.FlagLength = newFlagLength;
            geo.LodSelection = newLODSelection;
            geo.UseColliders = newUseColliders;
            geo.OverrideExcludeTagsEnabled = newOverrideExcludeTagsEnabled;
            if (newOverrideExcludeTags != null)
                geo.OverrideExcludeTags = newOverrideExcludeTags;

            if (pathChanged)
            {
                string prevPath = geo.AbsoluteFilePath;
                string newRelativeFilePath = newFilePath.Substring(Application.dataPath.Length + 1).Replace('\\', '/');
                if (File.Exists(prevPath) && string.Compare(newRelativeFilePath, geo.RelativeFilePath, true) != 0)
                {
                    Debug.Log($"Move/rename Geometry file\n to:\t{newRelativeFilePath} from:\t{geo.RelativeFilePath}\n");
                    if (UnityEditor.VersionControl.Provider.isActive)
                    {
                        UnityEditor.VersionControl.Provider.Move(prevPath, newFilePath);
                    }
                    else
                    {
                        File.Move(prevPath, newFilePath);
                        if (File.Exists(prevPath + ".meta"))
                            File.Move(prevPath + ".meta", newFilePath + ".meta");
                    }
                }

                geo.AbsoluteFilePath = newFilePath;
            }

            if (pathChanged || shouldWriteMesh)
            {
                if (geo.WriteFile())
                {
                    needsRebake = false;
                }
            }
        }

        if (Application.isPlaying && GUILayout.Button("Upload Mesh"))
        {
            geo.GatherGeometryRuntime();
        }
    }
    public static void Separator()
    {
        GUI.color = new Color(1, 1, 1, 0.25f);
        GUILayout.Box("", "HorizontalSlider", GUILayout.Height(16));
        GUI.color = Color.white;
    }
}
