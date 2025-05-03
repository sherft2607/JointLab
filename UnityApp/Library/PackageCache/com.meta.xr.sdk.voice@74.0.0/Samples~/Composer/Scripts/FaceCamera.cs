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

namespace Meta.WitAi.Composer.Samples
{
    /// <summary>
    /// Class for rotating object to face main camera on update
    /// </summary>
    public class FaceCamera : MonoBehaviour
    {
        // Main camera
        private static Camera _mainCamera;
        public bool OnlyY = true;

        // Update is called once per frame
        void Update()
        {
            // Obtain main camera
            if (_mainCamera == null)
            {
                // Obtain main camera
                _mainCamera = Camera.main;

                // Not found, log & destroy script
                if (_mainCamera == null)
                {
                    Debug.LogError($"{GetType().Name} - No Main Camera Found");
                    Destroy(this);
                    return;
                }
            }

            // Look at camera
            Vector3 direction = transform.position - _mainCamera.transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction.normalized);
            if (OnlyY)
            {
                rotation = Quaternion.Euler(0f, rotation.eulerAngles.y, 0f);
            }
            transform.rotation = rotation;
        }
    }
}
