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
 * Filename    :   MetaXRAcousticMaterialMapping.cs
 * Content     :   Provides a way of mapping Unity PhysicMaterials to MetaXRAcousticMaterials
 ***********************************************************************************/

using Meta.XR.Acoustics;
using UnityEngine;
using System;
using System.Collections.Generic;
using Native = MetaXRAcousticNativeInterface;

/// \brief This class provides a way of mapping Unity PhysicMaterials to MetaXRAcousticMaterials
internal class MetaXRAcousticMaterialMapping : ScriptableObject
{
    internal MetaXRAcousticMaterialProperties findAcousticMaterial(PhysicMaterial pmat)
    {
        if (pmat == null || mapping == null || mapping.Length == 0)
            return fallbackMaterial;

        return Array.Find(mapping, pair => Equals(pair.physicMaterial, pmat))?.acousticMaterial;
    }

    [Serializable]
    internal class Pair
    {
        /// \brief The Unity PhysicMaterial to be mapped to be connected with the selected MetaXRAcousticMaterial
        [SerializeField]
        internal PhysicMaterial physicMaterial;
        /// \brief The MetaXRAcousticMaterialProperties to be connected with the selected Physic Material
        [SerializeField]
        internal MetaXRAcousticMaterialProperties acousticMaterial;
    }

    /// \brief The array of pairs that map Unity PhysicMaterials to MetaXRAcousticMaterialProperties
    [HideInInspector, SerializeField]
    internal Pair[] mapping;

    /// \brief The fallback material to use when there is no mapping for a given PhysicMaterial
    [HideInInspector, SerializeField, Tooltip("Acoustic material to apply when there is no physics material.")]
    internal MetaXRAcousticMaterialProperties fallbackMaterial;

    private static MetaXRAcousticMaterialMapping instance;
    internal static MetaXRAcousticMaterialMapping Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<MetaXRAcousticMaterialMapping>("MetaXRAcousticMaterialMapping");

                // Use a dummy object with defaults for the getters so we don't have a null pointer exception
                if (instance == null)
                {
                    instance = ScriptableObject.CreateInstance<MetaXRAcousticMaterialMapping>();

#if UNITY_EDITOR
                    // Only in the editor should we save it to disk
                    string properPath = System.IO.Path.Combine(UnityEngine.Application.dataPath, "Resources");
                    if (!System.IO.Directory.Exists(properPath))
                    {
                        UnityEditor.AssetDatabase.CreateFolder("Assets", "Resources");
                    }
                    string fullPath = FullPath();
                    UnityEditor.AssetDatabase.CreateAsset(instance, fullPath);
#endif
                }
            }

            return instance;
        }
    }

#if UNITY_EDITOR
    internal static string FullPath()
    {
        return System.IO.Path.Combine(
            "Assets", "Resources", "MetaXRAcousticMaterialMapping.asset");
    }
#endif
}
