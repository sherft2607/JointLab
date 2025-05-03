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
 * Filename    :   MetaXRAcousticControlZoneEditor.cs
 * Content     :   Editor for modifying acoustic properties within a 3D volume.
 ***********************************************************************************/
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Spectrum = Meta.XR.Acoustics.Spectrum;

[CustomEditor(typeof(MetaXRAcousticControlZone))]
internal sealed class MetaXRAcousticControlZoneEditor : Editor
{
    internal static readonly Color kRt60SpectrumColor = new Color(0.0f, 0.8f, 0.5f);
    internal static readonly Color kReverbLevelSpectrumColor = new Color(0.8f, 0.5f, 0.8f);
    private MetaXRAudioSpectrumEditor rt60;
    private MetaXRAudioSpectrumEditor reverbLevel;

    static MetaXRAcousticControlZoneEditor()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    class AcousticControlZoneSaveState
    {
        public MetaXRAcousticControlZone.State state = new MetaXRAcousticControlZone.State();
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 localScale;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            foreach (var change in playModeChanges)
            {
                string scenePath = change.Key;
                int separator = scenePath.IndexOf(':');
                string sceneName = scenePath.Substring(0, separator);
                UnityEngine.SceneManagement.Scene scene = UnityEditor.SceneManagement.EditorSceneManager.GetSceneByName(sceneName);
                GameObject[] children = scene.GetRootGameObjects();

                int prevSeparator = separator + 1;
                separator = scenePath.IndexOf('/', prevSeparator);
                bool isChild = true;
                if (separator == -1)
                {
                    separator = scenePath.Length;
                    isChild = false;
                }
                string goName = scenePath.Substring(prevSeparator, separator - prevSeparator);

                MetaXRAcousticControlZone control = null;
                foreach (GameObject go in children)
                {
                    if (go.name.Equals(goName))
                    {
                        Transform t;
                        if (isChild)
                        {
                            string childPath = scenePath.Substring(separator + 1);
                            t = go.transform.Find(childPath);
                        }
                        else
                        {
                            t = go.transform;
                        }
                        control = t.GetComponent<MetaXRAcousticControlZone>();

                        break;
                    }
                }

                AssetDatabase.Refresh();
                Undo.RegisterCompleteObjectUndo(control, "AcousticControlZone Playmode Edits");

                AcousticControlZoneSaveState save = change.Value;
                Debug.Log($"Applying PlayMode Edits to {scenePath}");
                control.Clone(save.state);

                Undo.RegisterCompleteObjectUndo(control.transform, "AcousticControlZone Playmode Edits");
                control.transform.position = save.position;
                control.transform.rotation = save.rotation;
                control.transform.localScale = save.localScale;
            }

            playModeChanges.Clear();
        }
    }

    static Dictionary<string, AcousticControlZoneSaveState> playModeChanges = new Dictionary<string, AcousticControlZoneSaveState>();

    private void OnEnable()
    {
        MetaXRAcousticControlZone control = target as MetaXRAcousticControlZone;

        if (rt60 == null)
        {
            rt60 = new MetaXRAudioSpectrumEditor("RT60 Adjustment", "Adjusts the reverb decay time inside the zone", MetaXRAudioSpectrumEditor.AxisScale.SquareCentered, kRt60SpectrumColor, -5.0f, 5.0f);
            rt60.dataUnits = " s";
        }

        if (reverbLevel == null)
        {
            reverbLevel = new MetaXRAudioSpectrumEditor("Reverb Level Adjustment", "Adjusts the reverb level inside the zone", MetaXRAudioSpectrumEditor.AxisScale.SquareCentered, kReverbLevelSpectrumColor, -24.0f, 24.0f);
            reverbLevel.dataUnits = " dB";
        }

        rt60.LoadFoldoutState();
        reverbLevel.LoadFoldoutState();
    }

    private void OnDisable()
    {
        rt60.SaveFoldoutState();
        reverbLevel.SaveFoldoutState();
    }

    public override void OnInspectorGUI()
    {
        MetaXRAcousticControlZone control = target as MetaXRAcousticControlZone;

        EditorGUI.BeginChangeCheck();

        Color newColor = EditorGUILayout.ColorField(control.ZoneColor);
        float newFadeDistance = EditorGUILayout.FloatField(new GUIContent("Fade Distance", "The distance margin that is used to fade out the control's influence near the box boundaries"), control.FadeDistance);

        // Draw spectrum editors.
        rt60.Draw(control.Rt60, Event.current);
        reverbLevel.Draw(control.ReverbLevel, Event.current);

        if (EditorGUI.EndChangeCheck())
        {
            string groupName = Undo.GetCurrentGroupName();
            Undo.RegisterCompleteObjectUndo(control, groupName);

            if (groupName == MetaXRAudioSpectrumEditor.pointAddedGroupName)
                Undo.CollapseUndoOperations(Undo.GetCurrentGroup() - 1);

            control.ZoneColor = newColor;
            control.FadeDistance = Mathf.Max(newFadeDistance, 0.0f);

            // Ensure that the gizmos are redrawn after editing the box.
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();

            if (Application.isPlaying)
                control.ApplyProperties();
        }

        if (Application.isPlaying)
        {
            if (GUILayout.Button("Save"))
            {
                string scenePath = control.gameObject.name;
                Transform t = control.transform.parent;
                while (t != null)
                {
                    scenePath = t.gameObject.name + "/" + scenePath;
                    t = t.parent;
                }
                scenePath = control.gameObject.scene.name + ":" + scenePath;
                Debug.Log($"log change path = {scenePath}");

                AcousticControlZoneSaveState saveState = null;
                saveState = new AcousticControlZoneSaveState();
                saveState.state.Clone(control.state);
                saveState.position = control.transform.position;
                saveState.rotation = control.transform.rotation;
                saveState.localScale = control.transform.localScale;
                playModeChanges[scenePath] = saveState;
            }
        }
    }
}
