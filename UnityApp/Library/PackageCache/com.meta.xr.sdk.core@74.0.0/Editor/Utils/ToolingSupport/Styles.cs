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

using Meta.XR.Editor.UserInterface;
using UnityEditor;
using UnityEngine;
using static Meta.XR.Editor.UserInterface.Styles.Constants;
using static Meta.XR.Editor.UserInterface.Styles.Colors;
using static Meta.XR.Editor.UserInterface.Utils;

namespace Meta.XR.Editor.ToolingSupport
{
    internal static class Styles
    {
        public class GUIStylesContainer
        {
            internal readonly GUIStyle BackgroundAreaStyle = new GUIStyle()
            {
                stretchHeight = true,
                padding = new RectOffset(Border, Border, Border, 0),
                normal =
                {
                    background = CharcoalGray.ToTexture()
                }
            };

            internal readonly GUIStyle DescriptionAreaStyle = new GUIStyle()
            {
                stretchHeight = true,
                fixedHeight = ItemHeight,
                padding = new RectOffset(MiniMargin + Padding, Padding, Padding, Padding),
                margin = new RectOffset(0, 0, 0, Border),

                normal =
                {
                    background = DarkGray.ToTexture()
                },
                hover =
                {
                    background = DarkGrayHover.ToTexture()
                }
            };

            internal readonly GUIStyle DescriptionDarkerAreaStyle = new GUIStyle()
            {
                stretchHeight = true,
                fixedHeight = ItemHeight,
                padding = new RectOffset(MiniMargin + Padding, Padding, Padding, Padding),
                margin = new RectOffset(0, 0, 0, Border),
                normal =
                {
                    background = DarkerGray.ToTexture()
                },
                hover =
                {
                    background = DarkGray.ToTexture()
                }
            };

            public readonly GUIStyle BoldLabel = new GUIStyle(UserInterface.Styles.GUIStyles.BoldLabel)
            {
                alignment = TextAnchor.LowerLeft,
            };

            public readonly GUIStyle BoldLabelHover = new GUIStyle(UserInterface.Styles.GUIStyles.BoldLabel)
            {
                alignment = TextAnchor.LowerLeft,
                normal = { textColor = Color.white },
            };

            internal readonly GUIStyle SubtitleStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.UpperLeft,
            };

            internal readonly GUIStyle IconStyle = new GUIStyle(EditorStyles.label)
            {
                fixedWidth = ItemHeight - Padding * 2,
                fixedHeight = ItemHeight - Padding * 2,
                stretchHeight = true,
                padding = new RectOffset(Margin, Margin, Margin, Margin),
            };

            internal readonly GUIStyle PillIconStyle = new GUIStyle(EditorStyles.label)
            {
                fixedWidth = 22,
                fixedHeight = 22,
                stretchHeight = true,
                padding = new RectOffset(0, 0, 0, 0),
            };

            internal readonly GUIStyle StatusIconStyle = new GUIStyle("StatusBarIcon");

            internal readonly GUIStyle StatusPillIconStyle = new GUIStyle(EditorStyles.label)
            {
                fixedWidth = 10,
                fixedHeight = 10,
                stretchHeight = true,
                padding = new RectOffset(0, 0, 0, 0),
            };
        }

        public static class Contents
        {
            internal static readonly TextureContent StatusIcon =
                TextureContent.CreateContent("ovr_icon_meta.png", TextureContent.Categories.Generic, null);

            internal static readonly TextureContent StatusPillIcon =
                TextureContent.CreateContent("ovr_icon_pill.png", TextureContent.Categories.Generic, null);

            public static readonly TextureContent MetaIcon =
                TextureContent.CreateContent("ovr_icon_meta_white.png", TextureContent.Categories.Generic);
        }

        public static class Constants
        {
            internal const float Width = 360;
        }

        private static GUIStylesContainer _guiStyles;
        public static GUIStylesContainer GUIStyles => _guiStyles ??= new GUIStylesContainer();
    }
}
