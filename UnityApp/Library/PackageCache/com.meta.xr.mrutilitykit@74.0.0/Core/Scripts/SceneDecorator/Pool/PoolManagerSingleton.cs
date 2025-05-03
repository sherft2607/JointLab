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

using Meta.XR.Util;
using UnityEngine;

namespace Meta.XR.MRUtilityKit.SceneDecorator
{
    /// <summary>
    /// This class is a singleton that manages the pool of GameObjects used by Scene Decorator.
    /// </summary>
    [Feature(Feature.Scene)]
    [SingletonMonoBehaviour.InstantiationSettings(dontDestroyOnLoad = false)]
    public class PoolManagerSingleton : SingletonMonoBehaviour<PoolManagerSingleton>
    {
        /// <summary>
        /// Gets the pool manager for GameObjects.
        /// </summary>
        public PoolManager<GameObject, Pool<GameObject>> poolManager => poolManagerComponent.poolManager;

        private PoolManagerComponent poolManagerComponent;

        /// <summary>
        /// Creates a new instance of the specified GameObject at the given position and rotation, attached to the provided MRUKAnchor.
        /// The new GameObject will be parented to the optional parent Transform if provided.
        /// </summary>
        /// <param name="primitive">The primitive GameObject to instantiate.</param>
        /// <param name="position">The position of the new GameObject in world space.</param>
        /// <param name="rotation">The rotation of the new GameObject in world space.</param>
        /// <param name="anchor">The MRUKAnchor to which the new GameObject will be attached.</param>
        /// <param name="parent">An optional parent Transform for the new GameObject. Defaults to null.</param>
        /// <returns>The newly created GameObject instance, or null if creation failed.</returns>
        public GameObject Create(GameObject primitive, Vector3 position, Quaternion rotation, MRUKAnchor anchor, Transform parent = null)
        {
            if (poolManagerComponent == null)
            {
                poolManagerComponent = GetComponent<PoolManagerComponent>();
            }

            return poolManagerComponent.Create(primitive, position, rotation, anchor, parent);
        }

        /// <summary>
        /// Creates a new instance of the specified GameObject at the position and rotation defined by the provided MRUKAnchor.
        /// The new GameObject will be parented to the optional parent Transform if provided.
        /// </summary>
        /// <param name="primitive">The primitive GameObject to instantiate.</param>
        /// <param name="anchor">The MRUKAnchor that defines the position and rotation of the new GameObject.</param>
        /// <param name="parent">An optional parent Transform for the new GameObject. Defaults to null.</param>
        /// <param name="instantiateInWorldSpace">If true, the new GameObject will be instantiated in world space instead of local space. Defaults to false.</param>
        /// <returns>The newly created GameObject instance, or null if creation failed.</returns>
        public GameObject Create(GameObject primitive, MRUKAnchor anchor, Transform parent = null, bool instantiateInWorldSpace = false)
        {
            return poolManagerComponent.Create(primitive, anchor, parent, instantiateInWorldSpace);
        }

        /// <summary>
        /// Creates a new instance of the specified component type, attached to a GameObject created from the given primitive.
        /// The GameObject is positioned and rotated according to the provided parameters, and optionally parented to another Transform.
        /// </summary>
        /// <param name="primitive">The primitive object used to create the new GameObject.</param>
        /// <param name="position">The position of the new GameObject in world space.</param>
        /// <param name="rotation">The rotation of the new GameObject in world space.</param>
        /// <param name="anchor">The MRUKAnchor to which the new GameObject will be attached.</param>
        /// <param name="parent">An optional parent Transform for the new GameObject. Defaults to null.</param>
        /// <typeparam name="T">The type of component to create and attach to the new GameObject.</typeparam>
        /// <returns>The newly created component instance, or null if creation failed.</returns>
        public T Create<T>(T primitive, Vector3 position, Quaternion rotation, MRUKAnchor anchor, Transform parent = null) where T : Component
        {
            return poolManagerComponent.Create<T>(primitive, position, rotation, anchor, parent);
        }

        /// <summary>
        /// Creates a new instance of the specified component type, attached to a GameObject created from the given primitive.
        /// The GameObject is positioned and rotated according to the provided MRUKAnchor, and optionally parented to another Transform.
        /// </summary>
        /// <param name="primitive">The primitive object used to create the new GameObject.</param>
        /// <param name="anchor">The MRUKAnchor to which the new GameObject will be attached.</param>
        /// <param name="parent">An optional parent Transform for the new GameObject. Defaults to null.</param>
        /// <param name="instantiateInWorldSpace">If true, the new GameObject will be instantiated in world space instead of local space. Defaults to false.</param>
        /// <typeparam name="T">The type of component to create and attach to the new GameObject.</typeparam>
        /// <returns>The newly created component instance, or null if creation failed.</returns>
        public T Create<T>(T primitive, MRUKAnchor anchor, Transform parent = null, bool instantiateInWorldSpace = false) where T : Component
        {
            return poolManagerComponent.Create<T>(primitive, anchor, parent, instantiateInWorldSpace);
        }

        /// <summary>
        /// Releases a GameObject back to its pool or destroys it if it's not pooled.
        /// </summary>
        /// <param name="go">The GameObject to release.</param>
        public void Release(GameObject go)
        {
            if (Instance == null)
            {
                Destroy(go);
                return;
            }

            if (go == null)
            {
                return;
            }

            Instance.poolManagerComponent.Release(go);
        }

        void Start()
        {
            poolManagerComponent = GetComponent<PoolManagerComponent>();
        }
    }
}
