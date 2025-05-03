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

using System.Collections.Generic;
using System.Linq;
using Meta.Voice.Hub;
using Meta.Voice.Hub.Attributes;
using Meta.Voice.TelemetryUtilities;
using UnityEditor;

namespace Meta.Voice.VSDKHub
{
    [MetaHubContext(VoiceHubConstants.CONTEXT_VOICE)]
    public class VoiceSDKHubContext : MetaHubContext
    {
    }

    public class VoiceSDKHub : MetaHub
    {
        public static readonly List<string> Contexts = new List<string>
        {
            VoiceHubConstants.CONTEXT_VOICE
        };

        private List<string> _vsdkContexts;
        public override List<string> ContextFilter
        {
            get
            {
                if (null == _vsdkContexts || _vsdkContexts.Count == 0)
                {
                    _vsdkContexts = Contexts.ToList();
                    AddChildContexts(_vsdkContexts);
                }

                return _vsdkContexts;
            }
        }

        public static string GetPageId(string pageName)
        {
            return VoiceHubConstants.CONTEXT_VOICE + "::" + pageName;
        }

        [MenuItem("Meta/Voice SDK/Voice Hub", false, 1)]
        private static void ShowWindow()
        {
            MetaHub.ShowWindow<VoiceSDKHub>(Contexts.ToArray());
        }

        public static void ShowPage(string page)
        {
            Telemetry.Editor.LogInstantEvent(EditorTelemetry.TelemetryEventId.OpenUi, new Dictionary<EditorTelemetry.AnnotationKey, string>()
                {
                    {EditorTelemetry.AnnotationKey.PageId, page}
                });
            var window = MetaHub.ShowWindow<VoiceSDKHub>(Contexts.ToArray());
            window.SelectedPage = page;
        }

        protected override void OnEnable()
        {
            _vsdkContexts = null;
            base.OnEnable();
        }
    }
}
