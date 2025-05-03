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
 * Filename    :   MetaXRAcousticSettingsEditor.cs
 * Content     :   Custom editor for acoustic setttings
 ***********************************************************************************/
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

class MetaXRAcousticSettingsProvider : SettingsProvider
{
    private SerializedObject serializableSettings;
    private MetaXRAcousticMaterialMappingEditor mappingEditor;
    private bool showMapping = false;

    internal MetaXRAcousticSettingsProvider(string path, SettingsScope scope = SettingsScope.User)
        : base(path, scope) { }

    internal static bool IsSettingsAvailable() => true;

    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        // This function is called when the user clicks on the MyCustom element in the Settings window.
        serializableSettings = new SerializedObject(MetaXRAcousticSettings.Instance);
    }

    static System.Type tagEnumType;
    static System.Type TagEnumType
    {
        get
        {
            string[] systemTags = UnityEditorInternal.InternalEditorUtility.tags;
            string[] tagEnumNames = tagEnumType != null ? System.Enum.GetNames(tagEnumType) : new string[0];
            if (tagEnumType == null || tagEnumNames.Length != systemTags.Length || !tagEnumNames.SequenceEqual(systemTags))
            {
                System.Reflection.AssemblyName aName = new System.Reflection.AssemblyName("TempAssembly");
                System.Reflection.Emit.AssemblyBuilder ab = System.AppDomain.CurrentDomain.DefineDynamicAssembly(
                    aName, System.Reflection.Emit.AssemblyBuilderAccess.RunAndSave);

                System.Reflection.Emit.ModuleBuilder mb = ab.DefineDynamicModule(aName.Name, aName.Name + ".dll");
                System.Reflection.Emit.EnumBuilder eb = mb.DefineEnum("UnityTags", System.Reflection.TypeAttributes.Public, typeof(int));
                int i = 0;
                foreach (string tag in systemTags)
                {
                    eb.DefineLiteral(tag, 1 << i);
                    i++;
                }
                tagEnumType = eb.CreateType();
                ab.Save(aName.Name + ".dll");
            }

            return tagEnumType;
        }
    }

    static System.Enum GenerateEnum(string[] tags)
    {
        int value = 0;
        string[] systemTags = UnityEditorInternal.InternalEditorUtility.tags;
#if META_XR_ACOUSTIC_INFO
        Debug.Log($"system={systemTags}, input={tags}");
#endif

        if (tags != null)
        {
            int i = 0;
            foreach (string tag in systemTags)
            {
                if (tags.Contains(tag))
                    value |= 1 << i;

                i++;
            }
        }

        return System.Enum.ToObject(TagEnumType, value) as System.Enum;
    }

    static string[] GenerateTagStrings(System.Enum tagsEnum)
    {
        List<string> tags = new List<string>();

        string[] systemTags = UnityEditorInternal.InternalEditorUtility.tags;
        int i = 0;
        foreach (string tag in systemTags)
        {
            int flag = 1 << i;
            if (tagsEnum.HasFlag(System.Enum.ToObject(TagEnumType, flag) as System.Enum))
            {
#if META_XR_ACOUSTIC_INFO
                Debug.Log($"append {tag}, strings={tags}");
#endif
                tags.Add(tag);
            }
            i++;
        }

        string[] tags1 = tags.ToArray();
#if META_XR_ACOUSTIC_INFO
        Debug.Log($"enum={tagsEnum}, strings={tags1.Length}");
#endif
        return tags1;
    }

    internal static string[] ExcludeTagAsFlagsField(string[] excludeTagNames)
    {
        System.Enum excludeTags = GenerateEnum(excludeTagNames);
        System.Enum modifiedexcludeTags = EditorGUILayout.EnumFlagsField("Exclude Tags", excludeTags);
        if (modifiedexcludeTags != excludeTags)
            return GenerateTagStrings(modifiedexcludeTags);

        return null;
    }

    public override void OnGUI(string searchContext)
    {
        MetaXRAcousticSettings settings = MetaXRAcousticSettings.Instance;

        // Make the label column wider because some of the labels extend slightly past the default
        EditorGUIUtility.labelWidth = 180;

        EditorGUILayout.PropertyField(serializableSettings.FindProperty("acousticModel"), new GUIContent("Acoustic Model"));
        bool propagationEnabled = settings.AcousticModel == Meta.XR.Acoustics.AcousticModel.Automatic ||
            settings.AcousticModel == Meta.XR.Acoustics.AcousticModel.AcousticRayTracing;
        EditorGUI.BeginDisabledGroup(!propagationEnabled);
        EditorGUILayout.PropertyField(serializableSettings.FindProperty("diffractionEnabled"), new GUIContent("Diffraction Enabled"));
        string[] newTags = ExcludeTagAsFlagsField(settings.ExcludeTags);
        EditorGUI.EndDisabledGroup();

        if (newTags != null)
        {
            // Note: we don't need to call ApplyAllSettings since this only exists in editor
            settings.ExcludeTags = newTags;

            // Force write settings to disk
            serializableSettings.Update();
        }

        EditorGUILayout.PropertyField(serializableSettings.FindProperty("mapBakeWriteGeo"), new GUIContent("Map Bake Writes Geometry"));

        showMapping = EditorGUILayout.Foldout(showMapping, "Physic Material Mapping");
        if (showMapping)
            mappingEditor.OnInspectorGUI();

        bool wasModified = serializableSettings.hasModifiedProperties;
        serializableSettings.ApplyModifiedPropertiesWithoutUndo();
        if (wasModified)
            settings.ApplyAllSettings();
    }

    // Register the SettingsProvider
    [SettingsProvider]
    internal static SettingsProvider CreateMetaXRAcousticSettingsProvider()
    {
        if (IsSettingsAvailable())
        {
            var provider = new MetaXRAcousticSettingsProvider("Project/Meta XR Acoustics", SettingsScope.Project);

            // Automatically extract all keywords from the Styles.
            provider.keywords = new HashSet<string>(new[] { "Meta", "XR", "Audio", "Acoustics", "Propagation", "Diffraction", "Dynamic", "Quality" });

            // Create a mapping editor to display in settings dialog
            provider.mappingEditor = Editor.CreateEditor(MetaXRAcousticMaterialMapping.Instance) as MetaXRAcousticMaterialMappingEditor;
            return provider;
        }

        // Settings Asset doesn't exist yet; no need to display anything in the Settings window.
        return null;
    }
}
