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
using Meta.Voice.VSDKHub;
using Meta.Voice.Windows;
using Oculus.Voice.Utility;
using UnityEngine;

namespace Meta.Voice.Hub
{
    [MetaHubPage("Settings", VoiceHubConstants.CONTEXT_VOICE, priority: 800)]
    public class SettingsWindowPage : SettingsWindow, IMetaHubPage
    {
        protected override GUIContent Title => new GUIContent("Voice SDK Settings");
        protected override Texture2D HeaderIcon => null;
        protected override string DocsUrl => VoiceSDKStyles.Texts.VoiceDocsUrl;

        public new void OnGUI()
        {
            base.OnGUI();
        }
    }
}
