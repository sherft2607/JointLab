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
 * Filename    :   MetaXRAcousticSerializationManager.cs
 * Content     :   Functionality for baking acoustics
 ***********************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using static MetaXRAcousticMap;

internal class MetaXRAcousticSerializationManager
{
    static MetaXRAcousticSerializationManager()
    {
        EditorSceneManager.sceneSaving += OnSceneSaving;
    }
    internal int callbackOrder => 0;

    internal void OnPreprocessBuild(BuildTarget target, string path)
    {
        Debug.Log($"MetaXRAudioNativeInterfaceSerializationManager.OnPreprocessBuild for target {target} at path {path}");
    }

    [MenuItem("Meta/Audio/Acoustics/Bake Current Scene")]
    internal static void BakeCurrentScene()
    {
        BakeAcousticsForScene(EditorSceneManager.GetActiveScene());
    }

    [MenuItem("Meta/Audio/Acoustics/Bake All Scenes")]
    internal static void BakeAllScenes()
    {
        Debug.Log("Baking acoustic map and geometry for all scenes");

        Stack<string> remainingSceneGuids = new Stack<string>(AssetDatabase.FindAssets("t:scene"));

        HashSet<MetaXRAcousticSceneGroup> bakedSceneGroups = new HashSet<MetaXRAcousticSceneGroup>();
        HashSet<string> bakedGeoPaths = new HashSet<string>();
        HashSet<string> bakedMapPaths = new HashSet<string>();

        BakeNextRemainingScene(remainingSceneGuids, bakedSceneGroups, bakedGeoPaths, bakedMapPaths);
    }

    static void BakeNextRemainingScene(Stack<string> remainingSceneGuids, HashSet<MetaXRAcousticSceneGroup> bakedSceneGroups, HashSet<string> bakedGeoPaths, HashSet<string> bakedMapPaths)
    {
        TemporaryLoadScenes(new[] { remainingSceneGuids.Pop() }, out List<TempSceneLoad> tempSceneLoads);

        // Note we cannot unload all scenes so we have to load temporaries first!
        List<Scene> originalScenes = new List<Scene>();

        BakeAcousticsForScene(tempSceneLoads[0].scene, bakedSceneGroups, bakedGeoPaths, bakedMapPaths,
        () =>
        {
            for (int i = 0; i < EditorSceneManager.sceneCount; ++i)
            {
                Scene scene = EditorSceneManager.GetSceneAt(i);
                if (scene.isLoaded)
                {
                    originalScenes.Add(scene);
                    EditorSceneManager.CloseScene(scene, false);
                    Debug.LogWarning($"CLOSING OPEN SCENE {scene.name}");
                }
            }
        },
        () =>
        {
            foreach (var scene in originalScenes)
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scene.path, UnityEditor.SceneManagement.OpenSceneMode.Additive);

            UnloadTemporaryScenes(tempSceneLoads);
            EditorUtility.ClearProgressBar();
            if (remainingSceneGuids.Count > 0)
                BakeNextRemainingScene(remainingSceneGuids, bakedSceneGroups, bakedGeoPaths, bakedMapPaths);
        });
    }

    [MenuItem("Meta/Audio/Acoustics/Delete Unreferenced Assets")]
    internal static void DeleteUnreferencedAcousticAssets()
    {
        DetermineUnreferencedAcousticAssets(out var unrefGeoFiles, out var unrefMapFiles);

        bool confirmed = false;
        if (unrefGeoFiles.Count == 0 && unrefMapFiles.Count == 0)
        {
            EditorUtility.DisplayDialog("No Unrefenced Assets", "No Unreferenced Acoustics Assets Found", "OK");
        }
        else
        {
            const int LINE_LIMIT = 10;
            Func<HashSet<string>, string> GenerateUnrefFilesMessage = unrefFiles =>
                unrefFiles.Count == 0 ? "" : $":\n  {string.Join("\n  ", unrefFiles.Take(LINE_LIMIT).Select(p => { p = Path.GetRelativePath(Application.streamingAssetsPath, p); return p; }))}{(unrefFiles.Count > LINE_LIMIT ? "\n..." : "")}\n";
            string unrefGeoFilesMessage = GenerateUnrefFilesMessage(unrefGeoFiles);
            string unrefMapFilesMessage = GenerateUnrefFilesMessage(unrefMapFiles);

            confirmed = EditorUtility.DisplayDialog("Delete Unreferenced Assets?",
               $"There are {unrefGeoFiles.Count} unreferenced geometries {unrefGeoFilesMessage}and {unrefMapFiles.Count} unreferenced maps{unrefMapFilesMessage}", "Delete", "Keep");
        }

        if (confirmed)
        {
            Debug.Log($"Deleting {unrefGeoFiles.Count()} acoustic map files");
            DeleteFiles(unrefMapFiles);

            Debug.Log($"Deleting {unrefGeoFiles.Count()} acoustic geometry files");
            DeleteFiles(unrefGeoFiles);

            AssetDatabase.Refresh();
        }
    }

    internal static void DeleteFiles(IEnumerable<string> unrefGeoFiles)
    {
        foreach (var path in unrefGeoFiles)
        {
            Debug.Log($"Deleting {path}");
            System.IO.File.Delete(path);
            System.IO.File.Delete(path + ".meta");
        }
    }

    internal static void DetermineUnreferencedAcousticAssets(out HashSet<string> unrefGeoFiles, out HashSet<string> unrefMapFiles)
    {
        string[] geoFileExtensions = { MetaXRAcousticGeometry.FILE_EXTENSION, "ovramesh" };
        IEnumerable<string> geoFiles = Enumerable.Empty<string>();
        foreach (string ext in geoFileExtensions)
            geoFiles = geoFiles.Concat(Directory.EnumerateFiles(Application.streamingAssetsPath, $"*.{ext}", SearchOption.AllDirectories));

        string[] mapFileExtensions = { MetaXRAcousticMap.FILE_EXTENSION, "ovramap", "scir" };
        IEnumerable<string> mapFiles = Enumerable.Empty<string>();
        foreach (string ext in mapFileExtensions)
            mapFiles = mapFiles.Concat(Directory.EnumerateFiles(Application.streamingAssetsPath, $"*.{ext}", SearchOption.AllDirectories));

        string[] sceneGuids = AssetDatabase.FindAssets("t:scene");

        HashSet<string> geoPaths = new HashSet<string>();
        HashSet<string> mapPaths = new HashSet<string>();
        foreach (string guid in sceneGuids)
        {
            TemporaryLoadScenes(new[] { guid }, out List<TempSceneLoad> tempSceneLoads);
            GatherGeometryAndMapsInScene(tempSceneLoads[0].scene, out MetaXRAcousticGeometry[] geos, out MetaXRAcousticMap[] maps);
            foreach (var geo in geos)
            {
                if (geo.FileEnabled)
                    geoPaths.Add(geo.AbsoluteFilePath);
            }

            foreach (var map in maps)
                mapPaths.Add(map.AbsoluteFilePath);

            UnloadTemporaryScenes(tempSceneLoads);
        }

        string[] prefabGuids = AssetDatabase.FindAssets("t:prefab");
        bool wasAutoValidate = MetaXRAcousticGeometry.AUTO_VALIDATE;
        foreach (string guid in prefabGuids)
        {
            MetaXRAcousticGeometry.AUTO_VALIDATE = false;
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
            MetaXRAcousticGeometry[] geos = prefab.GetComponentsInChildren<MetaXRAcousticGeometry>();
            foreach (var geo in geos)
            {
                if (geo.FileEnabled)
                    geoPaths.Add(Path.GetFullPath(geo.AbsoluteFilePath));
            }
            MetaXRAcousticMap[] maps = prefab.GetComponentsInChildren<MetaXRAcousticMap>();
            foreach (var map in maps)
                mapPaths.Add(Path.GetFullPath(map.AbsoluteFilePath));
        }
        MetaXRAcousticGeometry.AUTO_VALIDATE = wasAutoValidate;

        unrefGeoFiles = new HashSet<string>(geoFiles.Select(p => Path.GetFullPath(p)));
        unrefGeoFiles.ExceptWith(geoPaths);

        unrefMapFiles = new HashSet<string>(mapFiles.Select(p => Path.GetFullPath(p)));
        unrefMapFiles.ExceptWith(mapPaths);
    }

    internal static void OnSceneSaving(Scene scene, string path)
    {
        // Note: map baking can be slow so we don't automatically do it every time a scene saves
#if META_XR_ACOUSTIC_AUTO_BAKE
        BakeAudioAcoustics(scene);
#endif
    }

    private static void GatherGeometryAndMapsInScene(Scene scene, out MetaXRAcousticGeometry[] geos, out MetaXRAcousticMap[] maps)
    {
        List<MetaXRAcousticGeometry> allGeos = new List<MetaXRAcousticGeometry>();
        List<MetaXRAcousticMap> allMaps = new List<MetaXRAcousticMap>();

        GameObject[] rootObjects = scene.GetRootGameObjects();
        foreach (GameObject go in rootObjects)
        {
            allGeos.AddRange(go.GetComponentsInChildren<MetaXRAcousticGeometry>());
            allMaps.AddRange(go.GetComponentsInChildren<MetaXRAcousticMap>());
        }

        geos = allGeos.ToArray();
        maps = allMaps.ToArray();
    }

    private static void BakeAcousticsForScene(Scene scene, HashSet<MetaXRAcousticSceneGroup> sceneGroups = null, HashSet<string> geoPaths = null, HashSet<string> mapPaths = null, Action OnStart = null, Action OnFinish = null)
    {
        GatherGeometryAndMapsInScene(scene, out MetaXRAcousticGeometry[] geos, out MetaXRAcousticMap[] maps);
        foreach (var geo in geos)
        {
            if (geo.FileEnabled)
            {
                Debug.Log($"Baking acoustic geometry: {geo.name}");
                if (!geo.WriteFile())
                {
                    Debug.LogError($"Failed writing acoustic geometry for {geo.name}", geo.gameObject);
                }
                else
                {
                    if (geoPaths != null && !geoPaths.Add(geo.RelativeFilePath))
                        Debug.LogWarning($"Duplicate file name detected: {geo.RelativeFilePath}");
                }
            }
        }

        if (maps.Length > 1)
        {
            Debug.LogError("Multiple AcousticMaps in scene");
        }
        if (maps.Length == 0)
        {
            OnFinish.Invoke();
        }
        else
        {
            var map = maps[0];
            if (sceneGroups != null)
            {
                if (sceneGroups.Contains(map.SceneGroup))
                    return;

                sceneGroups.Add(map.SceneGroup);
            }

            Debug.Log($"Baking Scene {scene.name}");
            OnStart.Invoke();

            // Note: IsDirty isn't guaranteed to be 100% reliable so we always build
#if META_XR_ACOUSTIC_DIRTY_CHECK
            if (map.IsDirty())
#endif
            {
                MetaXRAcousticMapEditor mapEditor = Editor.CreateEditor(map, typeof(MetaXRAcousticMapEditor)) as MetaXRAcousticMapEditor;
                GameObject originalSelection = Selection.activeGameObject;
                Selection.activeGameObject = map.gameObject;
                if (!mapEditor.StartComputing(false, (bool success) =>
                {
                    Debug.Log($"[{scene.name}] baked {geoPaths.Count} geometries, acoustic map bake {(success ? "succeeded" : "failed")}");
                    Selection.activeGameObject = originalSelection;
                    if (OnFinish != null)
                        OnFinish.Invoke();
                }))
                {
                    Debug.LogError($"Failed writing acoustic map for {map.name}", map.gameObject);
                }

                if (mapPaths != null && !mapPaths.Add(map.RelativeFilePath))
                    Debug.LogWarning($"Duplicate file name detected: {map.RelativeFilePath}");
            }
        }
    }
}
