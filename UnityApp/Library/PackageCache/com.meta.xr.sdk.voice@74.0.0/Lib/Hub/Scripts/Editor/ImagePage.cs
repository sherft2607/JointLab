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
using Meta.Voice.Hub.UIComponents;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Meta.Voice.Hub
{
    [MetaHubPageScriptableObject]
    public class ImagePage : ScriptableObject, IPageInfo
    {
        [SerializeField] private string _displayName;
        [SerializeField] private string _prefix;
        [SerializeField] private MetaHubContext _context;
        [SerializeField] private int _priority = 0;
        [SerializeField]
        [FormerlySerializedAs("image")]
        private Texture2D _image;

        public string Name => _displayName ?? name;
        public string Context => _context?.Name;
        public int Priority => _priority;
        public string Prefix => _prefix;
        internal Texture2D Image => _image;
    }

    [CustomEditor(typeof(ImagePage))]
    public class ImageDisplayScriptableObjectEditor : Editor
    {
        private ImagePage _imageDisplay;
        private ImageView _imageView;

        private void OnEnable()
        {
            _imageDisplay = (ImagePage)target;
            _imageView = new ImageView(this);
        }

        public override void OnInspectorGUI()
        {
            if (_imageDisplay.Image)
            {
                _imageView.Draw(_imageDisplay.Image);
            }
            else
            {
                // Draw the default properties
                base.OnInspectorGUI();
            }
        }
    }
}
