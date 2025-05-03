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

using Meta.Voice.Hub.UIComponents;
using UnityEngine;
using UnityEditor;

public class ImageViewer : EditorWindow
{
    private Texture2D _image;
    private ImageView _imageView;

    public static void ShowWindow(Texture2D image, string title)
    {
        if (image == null) return;

        ImageViewer window = CreateInstance<ImageViewer>();
        window._image = image;
        window.titleContent = new GUIContent(title);
        window.Show();
    }

    private void OnGUI()
    {
        if (null == _imageView) _imageView = new ImageView(this);
        _imageView.Draw(_image);
    }
}
