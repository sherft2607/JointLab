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
using UnityEngine;

namespace Meta.Voice.Hub
{
    public class MetaHubContext : ScriptableObject
    {
        [Header("Context Configuration")]
        [Tooltip("The title of the window if this is the primary context")]
        [SerializeField] private string _windowTitle;
        [Tooltip("The logo to use if this is the primary context")]
        [SerializeField] private Texture2D _logo;
        [Tooltip("The icon to use if this is the primary context")]
        [SerializeField] private Texture2D _icon;
        [Tooltip("The priority of this context. Lower number means higher probability that this will be the primary context.")]
        [SerializeField] private int _priority = 1000;
        [Tooltip("If there are no context filters you will have")]
        [SerializeField] private bool _allowWithoutContextFilter = true;

        [Header("Page Content")]
        [SerializeField] private bool showPageGroupTitle = true;
        [SerializeField] private MetaHubPage[] _pages;
        [SerializeField] private ScriptableObjectReflectionPage[] _scriptableObjectPages;
        [SerializeField] private string _defaultPage;

        [SerializeField] private string[] _parentContexts;

        public virtual string Name => name;
        public virtual int Priority => _priority;
        public virtual Texture2D LogoImage => _logo;
        public virtual Texture2D Icon => _icon;
        public virtual string DefaultPage => _defaultPage;
        public virtual string Title => _windowTitle;
        public virtual bool ShowPageGroupTitle => showPageGroupTitle;
        public virtual bool AllowWithoutContextFilter => _allowWithoutContextFilter;
        public virtual string[] ParentContexts => _parentContexts;

        public virtual ScriptableObjectReflectionPage[] ScriptableObjectReflectionPages => _scriptableObjectPages;

        [Serializable]
        public class ScriptableObjectReflectionPage
        {
            [SerializeField] public string scriptableObjectType;
            [SerializeField] public string namePrefix;
            [Tooltip("A modifier the priority of all pages of this type.")]
            [SerializeField] public int priorityModifier;
        }
    }
}
