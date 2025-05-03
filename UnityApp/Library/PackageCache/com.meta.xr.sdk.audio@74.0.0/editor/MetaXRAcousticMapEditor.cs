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
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad] // required to make playModeStateChanged capture all state changes
[CustomEditor(typeof(MetaXRAcousticMap))]
internal class MetaXRAcousticMapEditor : Editor
{
    private bool showAdvancedControls = false;
    private bool showMappingConfig = false;

    static MetaXRAcousticMapEditor()
    {
#if META_XR_ACOUSTIC_STALE_CHECK
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        SceneManager.sceneLoaded += OnSceneLoaded;
#endif
        EditorSceneManager.sceneOpened += OnSceneOpened;
    }

    private MetaXRAcousticMapEditor()
    {
        Selection.selectionChanged += OnSelectionChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
#if META_XR_ACOUSTIC_INFO
        Debug.LogWarning($"OnPlayModeStateChanged {state}");
#endif
        if (state == PlayModeStateChange.ExitingEditMode)
        {
#if META_XR_ACOUSTIC_INFO
            Debug.Log($"Validating AcousticMap hashes for {SceneManager.sceneCount} scenes");
#endif
            for (int i = 0; i < SceneManager.sceneCount; ++i)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                GameObject[] rootObjects = scene.GetRootGameObjects();
                foreach (GameObject go in rootObjects)
                {
                    MetaXRAcousticMap[] acousticMaps = go.GetComponentsInChildren<MetaXRAcousticMap>();
                    foreach (MetaXRAcousticMap acousticMap in acousticMaps)
                    {
                        if (acousticMap.Status != AcousticMapStatus.READY)
                            Debug.LogWarning($"AcousticMap not baked: {acousticMap.gameObject.name} in scene {acousticMap.gameObject.scene.name}", acousticMap);
                        if (acousticMap.IsDirty())
                            Debug.LogError($"AcousticMap out of date: {acousticMap.gameObject.name} in scene {acousticMap.gameObject.scene.name}", acousticMap);
                    }
                }
            }
        }
    }
    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
#if META_XR_ACOUSTIC_INFO
        Debug.LogWarning($"SceneLoaded {scene.name}");
#endif
        GameObject[] rootObjects = scene.GetRootGameObjects();
        foreach (GameObject go in rootObjects)
        {
            MetaXRAcousticMap[] acousticMaps = go.GetComponentsInChildren<MetaXRAcousticMap>();
            foreach (MetaXRAcousticMap acousticMap in acousticMaps)
            {
                if (acousticMap.IsDirty())
                    Debug.LogError($"AcousticMap out of date: {acousticMap.gameObject.name} in scene {acousticMap.gameObject.scene.name}", acousticMap);
            }
        }
    }

    private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
    {
#if META_XR_ACOUSTIC_INFO
        Debug.Log($"Open scene {scene.name}");
#endif
        GameObject[] rootObjects = scene.GetRootGameObjects();
        foreach (GameObject go in rootObjects)
        {
            MetaXRAcousticMap[] acousticMaps = go.GetComponentsInChildren<MetaXRAcousticMap>();
            foreach (MetaXRAcousticMap acousticMap in acousticMaps)
                acousticMap.StartInternal();
        }
    }

    public override void OnInspectorGUI()
    {
        acousticMap = target as MetaXRAcousticMap;

        string newFilePath = acousticMap.AbsoluteFilePath;
        bool pathChanged = false;
        bool compute = false;
        bool mapScene = false;
        bool addPoint = false;
        bool removePoint = false;
        bool staticOnly = acousticMap.StaticOnly;
        bool noFloating = acousticMap.NoFloating;
        bool diffraction = acousticMap.Diffraction;
        bool customPointsEnabled = acousticMap.customPointsEnabled;
        bool newCustomPointsEnabled = acousticMap.customPointsEnabled;
        float minSpacing = acousticMap.MinSpacing;
        float maxSpacing = acousticMap.MaxSpacing;
        float headHeight = acousticMap.HeadHeight;
        float maxHeight = acousticMap.MaxHeight;
        int reflectionCount = (int)acousticMap.ReflectionCount;
        MetaXRAcousticSceneGroup sceneGroup = acousticMap.SceneGroup;
        selectedPoint = acousticMap.SelectedPoint;

        bool changed = false;

        using (new EditorGUI.DisabledScope(Application.isPlaying))
        {
            using (new EditorGUI.DisabledScope(acousticMap.Computing))
            {
                EditorGUI.BeginChangeCheck();

                showAdvancedControls = EditorGUILayout.Foldout(showAdvancedControls, "Advanced Controls", true);
                if (showAdvancedControls)
                {
                    EditorGUI.indentLevel++;

                    //*****************************************************************************
                    // Scene Group
                    sceneGroup = EditorGUILayout.ObjectField(new GUIContent("Scene Group", "A collection of scenes to support additive loading, if not set will use parent scene"), acousticMap.SceneGroup, typeof(MetaXRAcousticSceneGroup), false) as MetaXRAcousticSceneGroup;
                    using (new EditorGUI.DisabledScope(true))
                    {
                        if (sceneGroup != null)
                        {
                            for (int i = 0; i < sceneGroup.sceneGuids.Length; ++i)
                            {
                                SceneAsset oldScene = null;
                                if (!string.IsNullOrEmpty(sceneGroup.sceneGuids[i]))
                                {
                                    string path = AssetDatabase.GUIDToAssetPath(sceneGroup.sceneGuids[i]);
                                    oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                                }
                                var newScene = EditorGUILayout.ObjectField("", oldScene, typeof(SceneAsset), false) as SceneAsset;
                            }
                        }
                        else
                        {
                            SceneAsset oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(acousticMap.gameObject.scene.path);
                            var newScene = EditorGUILayout.ObjectField("", oldScene, typeof(SceneAsset), false) as SceneAsset;
                        }
                    }

                    staticOnly = EditorGUILayout.Toggle(new GUIContent("Static Only", "Only bake data for game objects marked as static"), staticOnly);
                    diffraction = EditorGUILayout.Toggle(new GUIContent("Edge Diffraction", "Precompute edge diffraction data for smooth occlusion. If disabled, a lower-quality but faster fallback diffraction will be used."), acousticMap.Diffraction);
                    reflectionCount = EditorGUILayout.IntSlider(new GUIContent("Reflections", "The number of reflections generated for each data point"), reflectionCount, 0, 12);

                    MetaXRAcousticGeometryEditor.Separator();

                    //*****************************************************************************
                    // Serialized File Path
                    bool showFilePicker = false;
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.PrefixLabel(new GUIContent("File Path:", "The path to the serialized Acoustic Map, relative to the Assets directory"));

                        int indentLevelPrev = EditorGUI.indentLevel;
                        EditorGUI.indentLevel = 0;
                        EditorGUILayout.LabelField(acousticMap.RelativeFilePath, EditorStyles.wordWrappedLabel);

                        // defer displaying dialog to avoid layout error
                        if (GUILayout.Button("...", GUILayout.ExpandWidth(false)))
                            showFilePicker = true;

                        EditorGUI.indentLevel = indentLevelPrev;
                    }

                    if (showFilePicker)
                    {
                        if (!System.IO.Directory.Exists(Application.streamingAssetsPath))
                            System.IO.Directory.CreateDirectory(Application.streamingAssetsPath);

                        string directory = Application.streamingAssetsPath;
                        string fileName = acousticMap.gameObject.scene.name + "." + MetaXRAcousticMap.FILE_EXTENSION;

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
                            "Save baked Acoustic Map to file", System.IO.Path.GetDirectoryName(acousticMap.AbsoluteFilePath), fileName, MetaXRAcousticMap.FILE_EXTENSION);

                        // If the user canceled, use the old path.
                        if (string.IsNullOrEmpty(newFilePath))
                            newFilePath = acousticMap.AbsoluteFilePath;
                        else
                            pathChanged = true;
                    }

                    EditorGUI.indentLevel--;
                } // Advanced Controls

                showMappingConfig = EditorGUILayout.Foldout(showMappingConfig, "Mapping Configuration", true);
                if (showMappingConfig)
                {
                    EditorGUI.indentLevel++;

                    //*****************************************************************************
                    // Mapping Parameters
                    minSpacing = EditorGUILayout.FloatField(new GUIContent("Min Spacing", "The size in meters of the smallest space that will be precomputed"), minSpacing);
                    minSpacing = Mathf.Clamp(minSpacing, 0, maxSpacing);
                    maxSpacing = EditorGUILayout.FloatField(new GUIContent("Max Spacing", "The maximum distance in meters between precomputed data points"), maxSpacing);
                    maxSpacing = Mathf.Clamp(maxSpacing, minSpacing, MetaXRAcousticMap.DISTANCE_PARAMETER_MAX);
                    noFloating = EditorGUILayout.Toggle(new GUIContent("No Floating", "Don't automatically place data for points far above the floor"), noFloating);
                    headHeight = EditorGUILayout.FloatField(new GUIContent("Head Height", "The distance above the floor where data points are placed"), headHeight);
                    headHeight = Mathf.Clamp(headHeight, 0, MetaXRAcousticMap.DISTANCE_PARAMETER_MAX);
                    maxHeight = EditorGUILayout.FloatField(new GUIContent("Max Height", "The maximum height above the floor where data points are placed"), maxHeight);
                    maxHeight = Mathf.Clamp(maxHeight, 0, MetaXRAcousticMap.DISTANCE_PARAMETER_MAX);

                    newCustomPointsEnabled = EditorGUILayout.Toggle(new GUIContent("Custom Points", "Custom points allow you to move and place points"), customPointsEnabled);
                    if (customPointsEnabled && !newCustomPointsEnabled && acousticMap.HasCustomPoints)
                        mapScene = true;

                    using (new EditorGUI.DisabledGroupScope(!customPointsEnabled))
                    {
                        // Exclude "Map Scene" and "Enable Editing" from change check
                        changed = EditorGUI.EndChangeCheck();

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.PrefixLabel(" ");
                            string mapLabel = acousticMap.HasCustomPoints ? "Remap Scene" : "Map Scene";
                            if (GUILayout.Button(new GUIContent(mapLabel, "Automatically place points in the scene.\nNOTE: this will clear any previously baked data or custom points")))
                                mapScene = true;
                        }

                        //*****************************************************************************
                        // Point editing

                        EditorGUI.BeginChangeCheck();

                        using (new EditorGUI.DisabledGroupScope(acousticMap.Computing))
                        {
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                EditorGUILayout.PrefixLabel(" ");
                                if (GUILayout.Button(new GUIContent("Add Point", "Add a new point at the center of the current view.\nTo quickly create points by left mouse button, enable editing in the viewport.")))
                                    addPoint = true;

                                using (new EditorGUI.DisabledScope(!acousticMap.HasSelectedPoint))
                                {
                                    if (GUILayout.Button(new GUIContent("Remove Point", "Remove the currently-selected point.\nTo quickly delete points by pressing backspace, enable editing in the viewport")))
                                        removePoint = true;
                                }
                            }
                        }
                    }

                    EditorGUI.indentLevel--;
                    MetaXRAcousticGeometryEditor.Separator();
                } // Mapping Configuration
            }

            //*****************************************************************************
            // Baking controls

            // Exclude baking from change check
            changed |= EditorGUI.EndChangeCheck();

            bool cancel = false;
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel(" ");
                if (acousticMap.Computing && !acousticMap.ComputeCanceled)
                    cancel = GUILayout.Button("Cancel");
                else
                    compute = GUILayout.Button(new GUIContent("Bake Acoustics", "Simulate the scene, and also map it if custom points are not provided."));
            }

            if (cancel)
                acousticMap.CancelCompute();

            //*****************************************************************************
            // Statistics

            EditorGUILayout.LabelField(new GUIContent("Status:", "The current state of the precomputed data"), new GUIContent(acousticMap.Computing ? "COMPUTING" : (acousticMap.Status).ToString()));
            EditorGUILayout.LabelField(new GUIContent("Points:", "The number of precomputed data points"), new GUIContent((acousticMap.PointCount).ToString() + (acousticMap.HasCustomPoints ? " (Custom)" : "")));

            if (newFilePath != null)
            {
                FileInfo fileInfo = new FileInfo(newFilePath);
                long fileSize = fileInfo.Exists ? fileInfo.Length : 0;
                EditorGUILayout.LabelField(new GUIContent("Data Size:", "The total size of the serialized data"), new GUIContent(GetSizeString(fileSize)));
            }

            //*****************************************************************************
            // Progress

            // Show the progress bar if we are currently computing the IR asynchronously
            if (acousticMap.Computing)
            {
                double timeRemaining = acousticMap.ComputeProgress <= 0.0f ? 0.0 : acousticMap.ComputeTime / acousticMap.ComputeProgress - acousticMap.ComputeTime;
                EditorUtility.DisplayProgressBar(acousticMap.ComputeDescription, acousticMap.ComputeDescription + " ETA: " + getTimeString(timeRemaining), acousticMap.ComputeProgress);
            }
            else
            {
                EditorUtility.ClearProgressBar();
            }

            EditorGUI.BeginChangeCheck();
        }

        changed |= EditorGUI.EndChangeCheck();

        // Ask the user to confirm overwrite of previous data.
        bool overwritePoints = true;
        if (mapScene)
        {
            if (acousticMap.HasCustomPoints)
            {
                overwritePoints = EditorUtility.DisplayDialog("Overwrite Custom Points?",
                        "Are you sure you want to overwrite the custom points and re-map the scene?", "Overwrite", "Cancel");
            }
            else if (acousticMap.Status == AcousticMapStatus.READY)
            {
                overwritePoints = EditorUtility.DisplayDialog("Overwrite Baked Data?",
                        "Mapping the scene will clear previously baked data.\nDo you want to proceed?", "Overwrite", "Cancel");
            }

            // cancel toggle
            if (!overwritePoints && !newCustomPointsEnabled && customPointsEnabled)
                newCustomPointsEnabled = customPointsEnabled;
        }

        //*****************************************************************************
        // Apply changes

        if (changed)
        {
            Undo.RecordObject(acousticMap, "Edited MetaXRAcousticMap");

            acousticMap.StaticOnly = staticOnly;
            acousticMap.NoFloating = noFloating;
            acousticMap.Diffraction = diffraction;
            acousticMap.customPointsEnabled = newCustomPointsEnabled;
            acousticMap.MinSpacing = minSpacing;
            acousticMap.MaxSpacing = maxSpacing;
            acousticMap.HeadHeight = headHeight;
            acousticMap.MaxHeight = maxHeight;
            acousticMap.ReflectionCount = (uint)reflectionCount;
            acousticMap.SceneGroup = sceneGroup;

            if (pathChanged)
            {
                string prevPath = acousticMap.AbsoluteFilePath;
                string newRelativeFilePath = newFilePath.Substring(Application.dataPath.Length + 1).Replace('\\', '/');
                if (File.Exists(prevPath) && string.Compare(newRelativeFilePath, acousticMap.RelativeFilePath, true) != 0)
                {
                    Debug.Log($"Move/rename Acoustic Map file\n to:\t{newRelativeFilePath} from:\t{acousticMap.RelativeFilePath}\n");
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

                acousticMap.AbsoluteFilePath = newFilePath;
            }
        }

        if (compute || mapScene)
        {
            if (compute)
            {
                StartComputing(false);
            }
            else if (mapScene && overwritePoints)
            {
                // Set dirty so that the custom points are actually overwritten in the serialized data.
                // Without this, the custom points will be erroneously restored when loading project again.
                EditorUtility.SetDirty(acousticMap);
                StartComputing(true);
            }
        }
        else if (addPoint)
        {
            Undo.RecordObject(acousticMap, "Added Acoustic Map point");

            // Add a new point to the center of the view.
            // Trace a ray from the center into the scene to find the distance from camera.
            Camera camera = SceneView.lastActiveSceneView.camera;
            Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
            selectedPoint = acousticMap.AddPoint(GetNewPointForRay(acousticMap, ray, camera.farClipPlane));
            acousticMap.SelectedPoint = selectedPoint;

            SceneView.RepaintAll();
        }
        else if (removePoint)
        {
            Undo.RecordObject(acousticMap, "Deleted Acoustic Map point");
            acousticMap.RemovePoint(acousticMap.SelectedPoint);
            selectedPoint = INVALID_POINT;
            SceneView.RepaintAll();
        }
    }

    //***********************************************************************
    // Point Editing

    private MetaXRAcousticMap acousticMap;
    private const int INVALID_POINT = -1;
    private int selectedPoint = INVALID_POINT;
    private bool customMouseHandlingEnabled = false;
    private static float pointRadius = 0.2f;
    private bool freeEditEnabled = false;

    // Define a custom gizmo drawing function for the MetaXRAcousticMap component
    [DrawGizmo(GizmoType.Active | GizmoType.Selected | GizmoType.NotInSelectionHierarchy | GizmoType.Pickable)]
    internal static void DrawGizmos(MetaXRAcousticMap acousticMap, GizmoType gizmoType)
    {
        // Don't draw if the Acoustic Map inspector is collapsed.
        if (!InternalEditorUtility.GetIsInspectorExpanded(acousticMap))
            return;

        int pointCount = acousticMap.PointCount;
        for (int i = 0; i < pointCount; i++)
        {
            Gizmos.color = (i == acousticMap.SelectedPoint) ? Color.red : Color.yellow;
            Gizmos.DrawSphere(acousticMap.GetPoint(i), pointRadius);
        }
    }

    protected void OnSceneGUI()
    {
        MetaXRAcousticMap acousticMap = target as MetaXRAcousticMap;

        if (acousticMap.customPointsEnabled)
        {
            // Transform or remove the selected point.
            if (selectedPoint >= 0 && selectedPoint < acousticMap.PointCount)
            {
                EditorGUI.BeginChangeCheck();

                Vector3 oldPosition = acousticMap.GetPoint(selectedPoint);
                Vector3 newPosition = Handles.PositionHandle(oldPosition, Quaternion.identity);

                bool selectedPointMoved = EditorGUI.EndChangeCheck();

                if (selectedPointMoved)
                {
                    Undo.RecordObject(acousticMap, "Moved Acoustic Map point");
                    acousticMap.SetPoint(selectedPoint, newPosition);
                    return; // to not mess up with other mouse handling logic below
                }

                if (freeEditEnabled && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Backspace)
                {
                    // Remove selected point.
                    Undo.RecordObject(acousticMap, "Deleted Acoustic Map point");
                    acousticMap.RemovePoint(selectedPoint);
                    selectedPoint = INVALID_POINT;
                }
            }

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                bool shouldEnableCustomMouseHandling = false;
                selectedPoint = INVALID_POINT;
                if (PickPoint(Event.current.mousePosition, out selectedPoint))
                {
                    shouldEnableCustomMouseHandling = true;
                }
                else if (freeEditEnabled)
                {
                    // Create a new point in the scene at the mouse position.
                    Undo.RecordObject(acousticMap, "Added Acoustic Map point");

                    Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    acousticMap.AddPoint(GetNewPointForRay(acousticMap, ray, SceneView.lastActiveSceneView.camera.farClipPlane));

                    shouldEnableCustomMouseHandling = true;
                }

                if (shouldEnableCustomMouseHandling)
                {
                    // Eat the event to avoid the editor selecting objects behind the point.
                    Event.current.Use();
                    customMouseHandlingEnabled = true;

                    // This makes sure we receive mouse up events.
                    GUIUtility.hotControl = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);
                }

                // Inform the Acoustic Map which point is selected so that it can draw visual feedback.
                // This is ugly, but there doesn't seem to be a way to draw gizmos
                // from the editor and access non-static editor instance variables.
                acousticMap.SelectedPoint = selectedPoint;
            }
            else if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                if (customMouseHandlingEnabled)
                {
                    // Eat the event to avoid the editor selecting objects behind the point.
                    Event.current.Use();
                    customMouseHandlingEnabled = false;

                    // Mouse up cleanup
                    GUIUtility.hotControl = 0;
                }
            }
        }
    }

    /// Find the point that is under the specified mouse position.
    private bool PickPoint(Vector2 mousePosition, out int closestPoint)
    {
        // Convert from screen to world space.
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);

        MetaXRAcousticMap acousticMap = target as MetaXRAcousticMap;
        float minDistance = Single.MaxValue;
        closestPoint = INVALID_POINT;

        int pointCount = acousticMap.PointCount;
        for (int i = 0; i < pointCount; i++)
        {
            float distance = 0.0f;
            if (RayIntersectsSphere(ray, acousticMap.GetPoint(i), pointRadius, out distance) && distance < minDistance)
            {
                minDistance = distance;
                closestPoint = i;
            }
        }

        return closestPoint != INVALID_POINT;
    }

    private void OnSelectionChanged()
    {
        if (acousticMap != null && acousticMap.gameObject != Selection.activeGameObject)
            acousticMap.SelectedPoint = INVALID_POINT;
    }

    /// Determine whether or not a ray intersects a sphere.
    private bool RayIntersectsSphere(Ray ray, Vector3 center, float radius, out float distance)
    {
        Vector3 d = center - ray.origin;
        float dSquared = d.sqrMagnitude;
        float rSquared = radius * radius;

        // Find the closest point on the ray to the sphere's center.
        float t1 = Vector3.Dot(d, ray.direction);

        // If the ray starts inside the sphere there is an intersection.
        if (dSquared < rSquared)
        {
            // Find the distance from the closest point to the sphere's surface.
            float t2Squared = rSquared - dSquared + t1 * t1;

            // Compute the distance along the ray of the intersection.
            distance = t1 + Mathf.Sqrt(t2Squared);
            return true;
        }
        else
        {
            // Check to see if the ray is outside and points away from the sphere so there is no intersection.
            if (t1 < 0.0f)
            {
                distance = Single.MaxValue;
                return false;
            }

            // Find the distance from the closest point to the sphere's surface.
            // If the descriminant is negative, there is no intersection.
            float t2Squared = rSquared - dSquared + t1 * t1;
            if (t2Squared < 0.0f)
            {
                distance = Single.MaxValue;
                return false;
            }

            // Compute the distance along the ray of the intersection.
            distance = t1 - Mathf.Sqrt(t2Squared);
            return true;
        }
    }

    private Vector3 GetNewPointForRay(MetaXRAcousticMap acousticMap, Ray ray, float maxDistance, float defaultDistance = 2.0f)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            // Place the point slightly away from the hit point to avoid placing points on walls.
            float bias = 1.0f;
            Vector3 newPoint = ray.origin + ray.direction * Mathf.Max(hit.distance - bias, 0.0f);

            if (acousticMap.NoFloating == false)
            {
                return newPoint;
            }

            // Trace two rays down and up to place the point at the right height.
            Ray downRay = new Ray(newPoint, acousticMap.GravityVector.normalized);
            if (Physics.Raycast(downRay, out hit, Mathf.Infinity, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                float downDistance = hit.distance;
                float targetHeight = newPoint.y;

                if (downDistance > acousticMap.MaxHeight)
                {
                    targetHeight = acousticMap.HeadHeight;
                }
                else if (downDistance < acousticMap.HeadHeight)
                {
                    Ray upRay = new Ray(newPoint, -downRay.direction);
                    if (Physics.Raycast(upRay, out hit, Mathf.Infinity, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                    {
                        // Move point so that it is either at head height or is at the floor/ceiling midpoint if that is less.
                        float midHeight = 0.5f * (downDistance + hit.distance);
                        targetHeight = Mathf.Min(acousticMap.HeadHeight, midHeight);
                    }
                }

                // height adjustment
                newPoint += downRay.direction * (downDistance - targetHeight);
            }
            return newPoint;
        }
        else
        {
            // Camera is pointing at empty space.
            // Place the point a short default distance from the camera.
            return ray.origin + ray.direction * defaultDistance;
        }
    }

    Action<bool> onFinish;

    //***********************************************************************
    // Computation / Progress Bar

    internal bool StartComputing(bool mapOnly, Action<bool> OnFinish = null)
    {
        // Disable editing of custom points.
        freeEditEnabled = false;

        onFinish = OnFinish;

        // Subscribe to constant update ticks from the application.
        // This is the only way to always get updates on the main thread.
        EditorApplication.update += this.Update;

        MetaXRAcousticMap map = (MetaXRAcousticMap)target;

        // Trigger the computation.
        bool result = map.Compute(mapOnly);

        // Cause the editor to be redrawn to update the progress bar.
        Repaint();

        return result;
    }

    private void Update()
    {
        if (target == null)
            EditorApplication.update -= this.Update;

        MetaXRAcousticMap acousticMap = (MetaXRAcousticMap)target;
        if (acousticMap.Computing)
        {
            // Poll to see if the async compute was finished.
            if (acousticMap.ComputeFinished)
            {
                bool wasCancelled = acousticMap.ComputeCanceled;

                // Do final cleanup on the main thread after computing the IR.
                acousticMap.FinishCompute();

                // Unsubcribe from constant updates.
                EditorApplication.update -= this.Update;

                if (acousticMap.ComputeSucceeded)
                    acousticMap.UpdateHash();

                if (wasCancelled)
                {
                    Debug.Log($"Attempting to load previous bake {acousticMap.RelativeFilePath}", acousticMap);
                    acousticMap.DestroyInternal();
                    acousticMap.StartInternal();
                }

                if (onFinish != null)
                    onFinish(acousticMap.ComputeSucceeded);

                onFinish = null;
            }

            // Cause the editor to be redrawn to update the progress bar.
            Repaint();
        }
    }

    //***********************************************************************
    // Helpers

    /// Return a string suitable for display in a GUI for the specified data size in bytes.
    internal static string GetSizeString(long size)
    {
        if (size <= 0L)
            return "0 B";
        else if (size < 1024L)
            return $"{size} B";
        else if (size < 1024L * 1024L)
            return $"{size / (1024f):G3} KB";
        else if (size < 1024L * 1024L * 1024L)
            return $"{size / (1024f * 1024f):G3} MB";
        else if (size < 1024L * 1024L * 1024L * 1024L)
            return $"{size / (1024f * 1024f * 1024f):G3} GB";
        else
            return $"{size / (1024f * 1024f * 1024f * 1024f):0.##} TB";
    }

    /// Return a string suitable for display in a GUI for the specified time interval in seconds.
    private string getTimeString(double time)
    {
        return new TimeSpan((Int64)(time * 1.0e7)).ToString(@"hh\:mm\:ss");
    }
}
