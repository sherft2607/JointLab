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

using Meta.Voice.Hub.Attributes;
using Meta.Voice.Hub.Interfaces;
using UnityEngine;

namespace Meta.Voice.Hub.Content
{
    [MetaHubPageScriptableObject]
    public class SamplesPage : ScriptableObject, IPageInfo
    {
        [Tooltip("The name to be shown to the user in the samples display")]
        [SerializeField] private string _displayName;

        [Tooltip("The hierarchy location in the left-hand menu bar where this page will appear. If left empty, it will be a top-level item. Note that the prefix must end in a forward slash ( / )")]
        [SerializeField] private string _prefix;

        [Tooltip("A reference to the main tab in which this page will be shown")]
        [SerializeField] private MetaHubContext _context;

        [Tooltip("The ordering priority of this page")]
        [SerializeField] private int _priority = 0;

        [Tooltip("The unique identifier for this page. It is used by individual samples to denote in which page they'll be shown.")]
        [SerializeField] private string _sampleSetId;

        public string SampleSetId => _sampleSetId;
        public string Name => _displayName ?? name;
        public string Context => _context.Name;
        public int Priority => _priority;
        public string Prefix => _prefix;
    }
}
