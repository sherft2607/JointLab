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

// @lint-ignore-every LICENSELINT

using UnityEngine;

namespace Oculus.Haptics
{
    /// <summary>
    /// Represents an imported haptic clip asset.
    /// </summary>
    ///
    /// <remarks>
    /// A <c>HapticClip</c> is an asset containing the data of a <c>.haptic</c> file. It can be played back at
    /// runtime by a <c>HapticClipPlayer</c>. A <c>HapticClip</c> is created by <c>HapticClipImporter</c> on import.
    /// in the Unity editor.
    /// </remarks>
    public class HapticClip : ScriptableObject
    {
        /// <summary>
        /// The JSON representation of the haptic clip, stored as a string encoded in UTF-8.
        /// </summary>
        ///
        /// <remarks>
        /// This JSON data is imported from a <c>.haptic</c> file by the <c>HapticClipImporter</c>.
        /// The data contains the <c>HapticClip</c>'s metadata and haptic design pattern created in Meta Haptics Studio.
        /// </remarks>
        [SerializeField]
        public string json;
    }
}
