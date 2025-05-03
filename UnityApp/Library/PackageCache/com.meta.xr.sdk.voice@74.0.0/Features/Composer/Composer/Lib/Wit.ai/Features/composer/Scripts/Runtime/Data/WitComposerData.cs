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
using Meta.WitAi.Data.Configuration;
using UnityEngine;

namespace Meta.WitAi.Composer.Data.Info
{
    [Serializable]
    public class WitComposerData : WitConfigurationAssetData
    {
        /// <summary>
        /// List of canvases in the app
        /// </summary>
        [Tooltip("Represents the canvas of the given name.")]
        public ComposerGraph[] canvases;
    }

    [Serializable]
    public struct ComposerGraph
    {
        [HideInInspector]
        public string canvasName;
        [Tooltip("The Context Map is a JSON object passed between the the server and the client. \n" +
            "These are the JSON paths and values present in the Context Map on the server")]
        public ContextMapPaths contextMap;
        [Tooltip("A listing of all the actions sent by the Responses in this canvas.")]
        public string[] actions;
    }

    /// <summary>
    /// Path names and values of variables referenced in a context map.
    /// </summary>
    [Serializable]
    public struct ContextMapPaths
    {
        [Tooltip("The path names and values which are written by the Composer graph for the client to read. Composer does not read these values.")]
        public ComposerGraphValues[] server;

        [Tooltip("The path names which the Composer graph references but does not modify. The values of these must be supplied by the client.")]
        public string[] client;

        [Tooltip("The paths which the Composer graph both modifies and references. The client read or modify these.")]
        public ComposerGraphValues[] shared;
    }

    [Serializable]
    public struct ComposerGraphValues
    {
        [Tooltip("The path name referenced in Composer")]
        public string path;
        [Tooltip("The values assigned to this path in Composer")]
        public string[] values;
    }

    public static class WitAppInfoComposerExtensions
    {
        public static WitComposerData Composer(this IWitRequestConfiguration configuration)
        {
            return (WitComposerData)Array.Find(configuration.GetConfigData(), d => d is WitComposerData);
        }
    }
}
