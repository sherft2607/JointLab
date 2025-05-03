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

using System.IO;
using Meta.WitAi.Dictation;
using UnityEditor;
using UnityEngine;

namespace Oculus.Voice.Dictation
{
    [CustomEditor(typeof(AppDictationExperience))]
    public class AppDictationExperienceEditor : Editor
    {
        [SerializeField] private string transcribeFile;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (EditorApplication.isPlaying)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("File Transcriber");
                GUILayout.BeginHorizontal();
                transcribeFile = EditorGUILayout.TextField(transcribeFile);
                if (GUILayout.Button("Browse", GUILayout.Width(75)))
                {
                    var pickedFile = EditorUtility.OpenFilePanel("Select File", "", "wav");
                    if (!string.IsNullOrEmpty(pickedFile))
                    {
                        transcribeFile = pickedFile;
                    }
                }

                GUILayout.EndHorizontal();
                if (File.Exists(transcribeFile) && GUILayout.Button("Transcribe"))
                {
                    var dictationService = ((AppDictationExperience)target).GetComponent<WitDictation>();
                    dictationService.TranscribeFile(transcribeFile);
                }

                GUILayout.EndVertical();
            }
        }
    }
}
