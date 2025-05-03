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

using System.IO;
using System;
using UnityEngine;
using System.Text;

using UnityEditor.AssetImporters;

namespace Oculus.Haptics.Editor
{
    /// <summary>
    /// Importer for a <c>HapticClip</c>.
    /// </summary>
    ///
    /// <remarks>
    /// A <c>HapticClip</c> contains haptic playback data, designed in <see cref="https://developer.oculus.com/resources/haptics-overview/">Meta Haptics Studio</see>.
    /// Haptic playback data is UTF-8 encoded and JSON formatted and is described by amplitude and frequency
    /// curves in addition to emphasis points.
    /// </remarks>
    [ScriptedImporter(version: 3, ext: "haptic", AllowCaching = true)]
    public class HapticClipImporter : ScriptedImporter
    {
        /// <summary>
        /// Loads JSON data from a <c>.haptic</c> file into a <c>HapticClip</c> and imports
        /// the <c>HapticClip</c> into the <c>AssetDatabase</c> so that it can be assigned to a
        /// <c>HapticClipPlayer</c> for playback.
        /// </summary>
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var fileName = System.IO.Path.GetFileNameWithoutExtension(ctx.assetPath);
            var jsonString = File.ReadAllText(ctx.assetPath);
            var hapticClip = HapticClip.CreateInstance<HapticClip>();
            hapticClip.json = jsonString;

            ctx.AddObjectToAsset("com.meta.xr.sdk.HapticClip", hapticClip);
            ctx.SetMainObject(hapticClip);
        }
    }
}
