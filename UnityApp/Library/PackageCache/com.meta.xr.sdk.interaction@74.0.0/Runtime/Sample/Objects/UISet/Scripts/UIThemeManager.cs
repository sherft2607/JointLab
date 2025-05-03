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

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

namespace Oculus.Interaction
{
    /// <summary>
    /// Find and assign style properties defined by UITheme using tags.
    /// </summary>
    public class UIThemeManager : MonoBehaviour
    {
        [SerializeField]
        private UITheme[] _themes;
        public UITheme[] Themes => _themes;

        [SerializeField]
        private int _currentThemeIndex = 0;
        public int CurrentThemeIndex => _currentThemeIndex;

        void Start()
        {
            ApplyTheme(_currentThemeIndex);
        }

        public void ApplyCurrentTheme()
        {
            ApplyTheme(_currentThemeIndex);
        }

        public void ApplyTheme(int index)
        {
            if (index < 0 || index >= _themes.Length)
            {
                Debug.LogError("Theme index out of range.");
                return;
            }

            _currentThemeIndex = index;
            UITheme selectedTheme = _themes[index];

            // Assign Animator Controllers from current theme to all Animators in interactable UI components
            Animator[] animators = GetComponentsInChildren<Animator>();
            foreach (var animator in animators)
            {
                if (animator.CompareTag("QDSUIPrimaryButton"))
                {
                    animator.runtimeAnimatorController = selectedTheme.acPrimaryButton;
                }
                else if (animator.CompareTag("QDSUISecondaryButton"))
                {
                    animator.runtimeAnimatorController = selectedTheme.acSecondaryButton;
                }
                else if (animator.CompareTag("QDSUIBorderlessButton"))
                {
                    animator.runtimeAnimatorController = selectedTheme.acBorderlessButton;
                }
                else if (animator.CompareTag("QDSUIDestructiveButton"))
                {
                    animator.runtimeAnimatorController = selectedTheme.acDestructiveButton;
                }
                else if (animator.CompareTag("QDSUIToggleButton"))
                {
                    animator.runtimeAnimatorController = selectedTheme.acToggleButton;
                }
                else if (animator.CompareTag("QDSUIToggleBorderlessButton"))
                {
                    animator.runtimeAnimatorController = selectedTheme.acToggleBorderlessButton;
                }
                else if (animator.CompareTag("QDSUIToggleSwitch"))
                {
                    animator.runtimeAnimatorController = selectedTheme.acToggleSwitch;
                }
                else if (animator.CompareTag("QDSUIToggleCheckboxRadio"))
                {
                    animator.runtimeAnimatorController = selectedTheme.acToggleCheckboxRadio;
                }
                else if (animator.CompareTag("QDSUITextInputField"))
                {
                    animator.runtimeAnimatorController = selectedTheme.acTextInputField;
                }
                animator.Update(0);
            }

            // Apply color to all image objects under the canvas. For interactable elements, updates default normal state for editor view.
            Image[] images = GetComponentsInChildren<Image>();

            foreach (var image in images)
            {
                if (image.CompareTag("QDSUIIcon"))
                {
                    // Apply the text color to the icons as well
                    image.color = selectedTheme.textPrimaryColor;
                }
                else if (image.CompareTag("QDSUIAccentColor"))
                {
                    // Apply the accent color
                    image.color = selectedTheme.primaryButton.normal;
                }
                else if (image.CompareTag("QDSUISharedThemeColor") || image.CompareTag("QDSUIDestructiveButton") || image.CompareTag("QDSUIBorderlessButton") || image.CompareTag("QDSUIToggleBorderlessButton"))
                {
                    // Same color scheme for both themes, No changes. Or apply any future theme adjustments here.
                }
                else if (image.CompareTag("QDSUISecondaryButton") || image.CompareTag("QDSUIToggleButton"))
                {
                    // Colors are applied though animation clips.
                    // Apply the backplate color of the button for plated button.
                    // image.color = selectedTheme.buttonPlateColor;
                }
                else if (image.CompareTag("QDSUISection"))
                {
                    // Apply the section plate color.
                    image.color = selectedTheme.sectionPlateColor;
                }
                else if (image.CompareTag("QDSUITooltip"))
                {
                    // Apply the backplate color of the button for plated button.
                    image.color = selectedTheme.tooltipColor;
                }
                else if (image.CompareTag("QDSUITextInputField"))
                {
                    // Colors are applied though animation clips.
                }
                else
                {
                    image.color = selectedTheme.backplateColor;
                }

                if (selectedTheme.ThemeVersion == 2)
                {
                    // v2 visual design, optional: Apply the gradient material to the backplate
                    if (image.CompareTag("QDSUIBackplateGradient"))
                    {
                        image.color = selectedTheme.backplateColor;
                        image.material = selectedTheme.backplateGradientMaterial;
                    }

                    // v2 visual design, handle inverted colors on images
                    if (image.CompareTag("QDSUITextInvertedColor"))
                    {
                        image.color = selectedTheme.textPrimaryInvertedColor;
                    }
                }
            }

            SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

            foreach (var spriteRenderer in spriteRenderers)
            {
                spriteRenderer.color = selectedTheme.tooltipColor;
            }

            // Apply color and font to all TextMeshProUGUI components under the canvas
            TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();
            foreach (var text in texts)
            {
                text.font = selectedTheme.textFontMedium; // Set the font

                if (text.CompareTag("QDSUISharedThemeColor") || text.CompareTag("QDSUIDestructiveButton"))
                {
                    // Same color scheme for both themes, No changes. Or apply any future theme adjustments here.
                }
                else if (text.CompareTag("QDSUITextSecondaryColor"))
                {
                    text.color = selectedTheme.textSecondaryColor;
                }
                else
                {
                    // Apply the primary text color as default
                    text.color = selectedTheme.textPrimaryColor;
                }

                // v2 visual design, handle inverted text colors on buttons
                if (selectedTheme.ThemeVersion == 2)
                {
                    if (text.CompareTag("QDSUITextInvertedColor"))
                    {
                        text.color = selectedTheme.textPrimaryInvertedColor;
                    }

                    if (text.CompareTag("QDSUITextSecondaryInvertedColor"))
                    {
                        text.color = selectedTheme.textSecondaryInvertedColor;
                    }
                }
            }
        }
    }
}
