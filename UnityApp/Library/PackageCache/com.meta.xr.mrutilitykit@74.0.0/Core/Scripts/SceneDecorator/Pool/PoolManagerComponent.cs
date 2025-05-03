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
using Meta.XR.Util;
using UnityEngine;

namespace Meta.XR.MRUtilityKit.SceneDecorator
{
    /// <summary>
    /// This component is responsible for managing the pooling of GameObjects.
    /// </summary>
    [Feature(Feature.Scene)]
    public class PoolManagerComponent : MonoBehaviour
    {
        /// <summary>
        /// Pool Callbacks
        /// </summary>
        [Serializable]
        public abstract class CallbackProvider : MonoBehaviour
        {
            /// <summary>
            /// Gets the callbacks for the GameObject pool.
            /// </summary>
            /// <returns>An instance of Pool<GameObject>.Callbacks containing the callback methods for the GameObject pool.</returns>
            public abstract Pool<GameObject>.Callbacks GetPoolCallbacks();
        }

        [Serializable]
        internal class PoolableData : MonoBehaviour
        {
            internal Pool<GameObject> Pool;
            internal Vector3 Scale;
            internal MRUKAnchor Anchor;
        }

        [Serializable]
        internal struct PoolDesc
        {
            public enum PoolType
            {
                CIRCULAR
                , FIXED
            }

            public PoolType poolType;
            public GameObject primitive;
            public int size;
            public CallbackProvider callbackProviderOverride;
        }

        private static class DefaultCallbacks
        {
            public static GameObject Create(GameObject primitive)
            {
                var e = primitive.activeSelf;
                primitive.SetActive(false);
                var go = Instantiate(primitive, Vector3.zero, Quaternion.identity);
                primitive.SetActive(e);
                return go;
            }

            public static void OnGet(GameObject go)
            {
                go.SetActive(true);
            }

            public static void OnRelease(GameObject go)
            {
                go.SetActive(false);
            }
        }

        public static readonly Pool<GameObject>.Callbacks DEFAULT_CALLBACKS = new()
        {
            Create = DefaultCallbacks.Create,
            OnGet = DefaultCallbacks.OnGet,
            OnRelease = DefaultCallbacks.OnRelease,
        };

        [SerializeField] internal PoolDesc[] defaultPools;

        [NonSerialized] public PoolManager<GameObject, Pool<GameObject>> poolManager = new();

        protected internal virtual void InitDefaultPools(Pool<GameObject>.Callbacks? defaultCallbacks = null)
        {
            if (defaultCallbacks == null)
            {
                defaultCallbacks = DEFAULT_CALLBACKS;
            }

            foreach (PoolDesc pd in defaultPools)
            {
                Pool<GameObject>.Callbacks callbacks = defaultCallbacks.Value;
                CallbackProvider cp = pd.callbackProviderOverride == null
                    ? pd.primitive.GetComponent<CallbackProvider>()
                    : pd.callbackProviderOverride;
                if (cp != null)
                {
                    callbacks = cp.GetPoolCallbacks();
                }

                Pool<GameObject> pool;
                switch (pd.poolType)
                {
                    case PoolDesc.PoolType.FIXED:
                        pool = new FixedPool<GameObject>(pd.primitive, pd.size, callbacks);
                        break;
                    case PoolDesc.PoolType.CIRCULAR: //circular is default for now
                    default:
                        pool = new CircularPool<GameObject>(pd.primitive, pd.size, callbacks);
                        break;
                }

                poolManager.AddPool(pd.primitive, pool);
            }
        }

        /// <summary>
        /// Create is a drop-in replacement for Instantiate that uses a pool if available.
        /// Note that it is not named Instantiate so that it is easy to find & replace Instantiate
        /// calls with Create calls when switching to using Pools.
        /// </summary>
        /// <param name="primitive">The primitive to use</param>
        /// <param name="position">Position of the object to instatiate</param>
        /// <param name="rotation">Rotation of the object to instatiate</param>
        /// <param name="anchor">The MRUKAnchor to associate with.</param>
        /// <param name="parent">The Parent object to attach to.</param>
        /// <returns>The instantiated gameobject</returns>
        public GameObject Create(GameObject primitive, Vector3 position, Quaternion rotation, MRUKAnchor anchor, Transform parent = null)
        {
            Pool<GameObject> pool = poolManager.GetPool(primitive);
            if (pool == null)
            {
                return Instantiate(primitive, position, rotation, parent);
            }

            //Temporarily disable the OnGet callback,
            //as we only want to call it after we've adjusted the GameObject's transform
            Action<GameObject> onGet = pool.callbacks.OnGet;
            pool.callbacks.OnGet = null;

            GameObject go = pool.Get();
            if (go == null)
            {
                //If we are using a FixedPool, we have run out of pooled GameObjects
                pool.callbacks.OnGet = onGet;
                return null;
            }

            if (!go.TryGetComponent<PoolableData>(out var poolableData))
            {
                poolableData = go.AddComponent<PoolableData>();
            }

            poolableData.Scale = go.transform.localScale;
            poolableData.Anchor = anchor;
            poolableData.Pool = pool;

            go.transform.SetParent(parent);
            go.transform.SetPositionAndRotation(position, rotation);

            onGet(go);

            //ensure the OnGet callback gets restored
            pool.callbacks.OnGet = onGet;

            return go;
        }

        /// <summary>
        /// Create is a drop-in replacement for Instantiate that uses a pool if available.
        /// Note that it is not named Instantiate so that it is easy to find & replace Instantiate
        /// calls with Create calls when switching to using Pools.
        /// </summary>
        /// <param name="primitive">The primitive to use</param>
        /// <param name="anchor">The MRUKAnchor to associate with.</param>
        /// <param name="parent">The Parent object to attach to.</param>
        /// <param name="instantiateInWorldSpace">Define if it should staz in worldspace</param>
        /// <returns>The instantiated gameobject</returns>
        public GameObject Create(GameObject primitive, MRUKAnchor anchor, Transform parent = null, bool instantiateInWorldSpace = false)
        {
            Pool<GameObject> pool = poolManager.GetPool(primitive);
            if (pool == null)
            {
                return Instantiate(primitive, parent, instantiateInWorldSpace);
            }

            //Temporarily disable the OnGet callback,
            //as we only want to call it after we've adjusted the GameObject's transform
            Action<GameObject> onGet = pool.callbacks.OnGet;
            pool.callbacks.OnGet = null;

            GameObject go = pool.Get();
            if (go == null)
            {
                //if we are using a FixedPool, we have run out of pooled GameObjects
                pool.callbacks.OnGet = onGet;
                return null;
            }

            if (!go.TryGetComponent<PoolableData>(out var poolableData))
            {
                poolableData = go.AddComponent<PoolableData>();
            }

            poolableData.Scale = go.transform.localScale;
            poolableData.Anchor = anchor;
            poolableData.Pool = pool;

            go.transform.SetParent(parent);
            if (parent)
            {
                if (instantiateInWorldSpace)
                {
                    go.transform.SetPositionAndRotation(parent.position, parent.rotation);
                }
                else
                {
                    go.transform.localRotation = parent.localRotation;
                    go.transform.localPosition = parent.localPosition;
                }
            }

            onGet(go);

            //ensure the OnGet callback gets restored
            pool.callbacks.OnGet = onGet;
            return go;
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
            GameObject go = Create(primitive.gameObject, position, rotation, anchor, parent);
            return go == null ? null : go.GetComponent<T>();
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
            GameObject go = Create(primitive.gameObject, anchor, parent, instantiateInWorldSpace);
            return go == null ? null : go.GetComponent<T>();
        }

        /// <summary>
        /// Releases a previously created GameObject back to its pool, or destroys it if it's not poolable.
        /// This method is used to return a GameObject to its original state and make it available for reuse.
        /// </summary>
        /// <param name="go">The GameObject to release.</param>
        public void Release(GameObject go)
        {
            if (go.TryGetComponent<PoolableData>(out var poolableData) &&
                poolableData.Pool != null)
            {
                go.transform.localScale = poolableData.Scale;
                poolableData.Anchor = null;
                poolableData.Pool.Release(go);
            }
            else
            {
                Destroy(go);
            }
        }
    }
}
