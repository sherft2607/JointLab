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

using System.Collections.Generic;
using Meta.Voice.Hub.Content;
using Meta.Voice.Hub.Interfaces;
using Meta.Voice.Hub.Utilities;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Meta.Voice.Hub.Inspectors
{
    [CustomEditor(typeof(SamplesPage))]
    public class SamplesPageInspector : Editor, IOverrideSize
    {
#if VSDK_INTERNAL
        private bool edit = true;
#else
        private bool edit = false;
#endif

        const float _tileSize = 200;

        private List<Sample> _samples = new List<Sample>();
        private SamplesPage _samplePage;
        public float OverrideWidth { get; set; } = -1;
        public float OverrideHeight { get; set; } = -1;

        private Vector2 _scrollOffset;
        private float _minTileMargin = 15f;

        private void OnEnable()
        {
            _samples.Clear();
            _samplePage = (SamplesPage)target;
            var samples = PageFinder.FindPages(typeof(Sample));

            foreach (var so in samples)
            {
                var sample = (Sample)so;
                if (sample.sampleSetId == _samplePage.SampleSetId)
                {
                    _samples.Add(sample);
                }
            }

            _samples.Sort((a, b) =>
            {
                int cmp = a.priority.CompareTo(b.priority);
                if (cmp == 0) cmp = a.name.CompareTo(b.name);
                return cmp;
            });
        }

        public override void OnInspectorGUI()
        {
            _scrollOffset = GUILayout.BeginScrollView(_scrollOffset);
            var windowWidth = (OverrideWidth > 0 ? OverrideWidth : EditorGUIUtility.currentViewWidth) - _minTileMargin;
            var columns = (int)(windowWidth / (_tileSize + _minTileMargin));
            int col = 0;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            for (int i = 0; i < _samples.Count; i++)
            {
                if (col >= columns)
                {
                    col = 0;
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                }

                var sample = _samples[i];
                SampleEditor.DrawSample(sample, _tileSize);
                GUILayout.FlexibleSpace();
                col++;
            }
            GUILayout.EndHorizontal();

#if VSDK_INTERNAL
            GUILayout.Space(16);
            edit = EditorGUILayout.Foldout(edit, "Edit");
#endif
            if (edit)
            {
                base.OnInspectorGUI();
            }
            GUILayout.EndScrollView();
        }

        public static void OpenSceneFromGUID(string scenePath)
        {
            if (!string.IsNullOrEmpty(scenePath))
            {
                SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                if (sceneAsset != null)
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene(scenePath);
                    }
                }
                else
                {
                    Debug.LogError($"Scene asset not found at path: {scenePath}");
                }
            }
            else
            {
                Debug.LogError($"Scene not found {scenePath}");
            }
        }
    }
}
