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
using JetBrains.Annotations;
using Meta.WitAi.Composer.Data.Info;
using Meta.WitAi.Data.Info;
namespace Meta.WitAi.Data.Configuration.Tabs
{

    [UsedImplicitly]
    public class WitConfigurationComposerTab : WitConfigurationEditorTab
    {
        public override Type DataType => typeof(WitComposerData);
        public override int TabOrder { get; } = 5;
        public override string TabID { get; } = "composer";
        public override string TabLabel { get; } = WitTexts.Texts.ConfigurationComposerTabLabel;
        public override string MissingLabel { get; } = WitTexts.Texts.ConfigurationComposerMissingLabel;
        public override string GetPropertyName(string tabID) => "canvases";


        public override bool ShouldTabShow(WitAppInfo appInfo) => false;

        public override bool ShouldTabShow(WitConfiguration configuration)
        {
            var composerData = configuration.Composer();
            return composerData != null && composerData.canvases?.Length>0;
        }
        public override string GetTabText(bool titleLabel)
        {
            return titleLabel ? WitTexts.Texts.ConfigurationComposerTabLabel : WitTexts.Texts.ConfigurationComposerMissingLabel;
        }
    }
}
