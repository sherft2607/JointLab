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

using System.Linq;
using Meta.XR.Editor.Id;
using Meta.XR.Editor.Reflection;
using Meta.XR.Editor.Settings;
using Meta.XR.Editor.ToolingSupport;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;
using static Meta.XR.Editor.UserInterface.Styles.Constants;
using Utils = Meta.XR.Editor.UserInterface.Utils;

namespace Meta.XR.Editor.StatusMenu
{
    [InitializeOnLoad]
    [Reflection]
    internal static class StatusIcon
    {
        [Reflection(AssemblyTypeReference = typeof(UnityEditor.Editor), TypeName = "UnityEditor.Toolbar")]
        private static readonly TypeHandle ToolbarType = new();

        [Reflection(AssemblyTypeReference = typeof(UnityEditor.Editor), TypeName = "UnityEditor.Toolbar", Name = "m_Root")]
        private static readonly FieldInfoHandle<VisualElement> Root = new();

        private const string ElementClass = "unity-editor-toolbar-element";
        private const string PlayModeGroupId = "ToolbarZoneLeftAlign";
        private const string Title = "Meta XR Tools";

        private static bool Enabled { get; set; }

        private static readonly CustomBool StatusIconEnabled =
            new UserBool()
            {
                Owner = null,
                Uid = "StatusIcon.Enabled",
                Default = true
            };

        private static Object _toolbar;
        private static readonly EditorToolbarButton MetaIcon;
        private static readonly VisualElement Pill;
        private static Vector2 _rawPosition = Vector2.zero;

        static StatusIcon()
        {
            if (!Utils.ShouldRenderEditorUI()) return;

            MetaIcon = new EditorToolbarButton()
            {
                text = Title,
                style =
                {
                    paddingLeft = Margin,
                    paddingRight = Padding
                }
            };
            Styles.Contents.MetaIcon.RegisterToImageLoaded(loadedImage => MetaIcon.icon = loadedImage as Texture2D);
            MetaIcon.AddToClassList(ElementClass);
            MetaIcon.RegisterCallback<GeometryChangedEvent>(evt => RefreshRawPosition());
            MetaIcon.clicked += () =>
            {
                RefreshRawPosition();
                ShowDropdown();
            };

            var arrowIcon = new VisualElement();
            arrowIcon.AddToClassList("unity-icon-arrow");
            arrowIcon.style.marginLeft = Padding;
            MetaIcon.Add(arrowIcon);

            Pill = new VisualElement
            {
                style =
                {
                    position = Position.Absolute,
                    top = 3,
                    left = 18,
                    width = 6,
                    height = 6,
                    borderBottomLeftRadius = 50,
                    borderBottomRightRadius = 50,
                    borderTopLeftRadius = 50,
                    borderTopRightRadius = 50
                }
            };
            MetaIcon.Add(Pill);

            if (MetaIcon.Children().FirstOrDefault() is UnityEngine.UIElements.Image image)
            {
                image.style.marginRight = Padding;
                image.tintColor = UserInterface.Styles.Colors.UnselectedWhite;
            }

            EditorApplication.update += Update;
        }

        private static Rect ComputeCurrentRect()
        {
            var rect = MetaIcon.layout;
            var parentRect = MetaIcon.parent.layout;
            var position = _rawPosition + parentRect.position + rect.position;
            return new Rect(position, MetaIcon.layout.size);
        }

        private static void Update()
        {
            if (!StatusIconEnabled.Value)
            {
                Disable();
                return;
            }

            if (StatusIconEnabled.Value)
            {
                Enable();
            }

            UpdatePill();
        }

        private static VisualElement FetchParent()
        {
            if (_toolbar == null)
            {
                var toolbars = Resources.FindObjectsOfTypeAll(ToolbarType.Target);
                _toolbar = toolbars.FirstOrDefault();
            }

            if (_toolbar != null)
            {
                var root = Root.Get(_toolbar);
                return root?.Q(PlayModeGroupId);
            }

            return null;
        }

        private static void Enable()
        {
            var parent = FetchParent();

            if (MetaIcon.parent == parent && Enabled) return;

            Disable();

            parent?.Add(MetaIcon);

            Enabled = true;
        }


        private static void Disable()
        {
            if (!Enabled) return;

            MetaIcon.RemoveFromHierarchy();

            Enabled = false;
        }

        private static void UpdatePill()
        {
            var item = StatusMenu.GetHighestItem();
            if (item?.PillIcon == null)
            {
                Pill.style.opacity = 0.0f;
                return;
            }

            var (_, color, showNotification) = item.PillIcon();
            if (color == null || !showNotification)
            {
                Pill.style.opacity = 0.0f;
                return;
            }

            Pill.style.opacity = 1.0f;
            Pill.style.backgroundColor = color ?? Color.white;
        }

        public static void OnSettingsGUI()
        {
            StatusIconEnabled.DrawForGUI(Origins.UserSettings, null);
        }

        private static void RefreshRawPosition()
        {
            _rawPosition = GUIUtility.GUIToScreenPoint(Vector2.zero);
        }

        internal static void ShowDropdown()
        {
            StatusMenu.ShowDropdown(ComputeCurrentRect());
        }
    }
}
