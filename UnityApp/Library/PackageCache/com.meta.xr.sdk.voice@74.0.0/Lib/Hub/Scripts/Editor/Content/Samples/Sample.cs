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

using UnityEditor;
using UnityEngine;

namespace Meta.Voice.Hub.Content
{
    public class Sample : ScriptableObject
    {
        [Header("Content")]
        [Tooltip("The human readable name of the sample.")]
        public string title;

        [TextArea]
        [Tooltip("A short description of the Sample")]
        public string description;
        [Tooltip("A 2D image representing the sample")]
        public Texture2D tileImage;

        [Tooltip("The scene file of this sample to be opened")]
        [Header("Resource Paths")]
        public SceneAsset sceneReference;

        [Tooltip("The name of the package in which the sample resides")]
        public string packageSampleName;

        [Tooltip("The grouping ID of the sample page in which this sample should be displayed.")]
        public string sampleSetId;

        [Tooltip("Relative ordering priority for display")]
        public float priority;
    }
}
