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
using Meta.XR.Util;
using UnityEditor;
using UnityEngine;


namespace Meta.XR.MRUtilityKit.SceneDecorator
{
    /// <summary>
    /// Base class for all modifiers.
    /// </summary>
    [Feature(Feature.Scene)]
    public abstract class Modifier : ScriptableObject
    {
        /// <summary>
        /// If the modifier is enabled.
        /// </summary>
        [SerializeField]
        public bool enabled = true;

        /// <summary>
        /// Applies the modifier to a decoration
        /// </summary>
        /// <param name="decorationGO">The gameobject</param>
        /// <param name="sceneAnchor">The SceneAnchor</param>
        /// <param name="sceneDecoration">The SceneDecoration</param>
        /// <param name="candidate">The candidate</param>
        public abstract void ApplyModifier(GameObject decorationGO, MRUKAnchor sceneAnchor, SceneDecoration sceneDecoration, Candidate candidate);

#if UNITY_EDITOR
        [ContextMenu("Delete")]
        private void Delete()
        {
            Undo.DestroyObjectImmediate(this);
            AssetDatabase.SaveAssets();
        }
#endif
    }
}
