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
using System.Collections.Generic;
using Meta.XR.Util;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Meta.XR.MRUtilityKit.SceneDecorator
{
    /// <summary>
    /// A generic pool of objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Feature(Feature.Scene)]
    public abstract class Pool<T> where T : class
    {
        /// <summary>
        /// A generic pool entry with a flag indicating whether it is active or not and the object itself.
        /// </summary>
        protected struct Entry
        {
            public bool active;
            public T t;
        }

        /// <summary>
        /// The pool of objects.
        /// </summary>
        protected Entry[] pool;

        /// <summary>
        /// The indices of the objects in the pool.
        /// </summary>
        protected Dictionary<T, int> indices;

        /// <summary>
        /// The index of the next object to be retrieved from the pool.
        /// </summary>
        protected int index;

        /// <summary>
        /// Generic Callbacks
        /// </summary>
        public struct Callbacks
        {
            public Func<T, T> Create;
            public Action<T> OnGet, OnRelease;
        }

        /// <summary>
        /// The number of objects in the pool.
        /// </summary>
        protected abstract int CountAll
        {
            get;
        }

        /// <summary>
        /// The number of active objects in the pool.
        /// </summary>
        protected abstract int CountActive
        {
            get;
        }

        /// <summary>
        /// The number of inactive objects in the pool.
        /// </summary>
        public virtual int CountInactive => CountAll - CountActive;

        /// <summary>
        /// The callbacks to execute when an object is created, retrieved from the pool or released back to it.
        /// </summary>
        public Callbacks callbacks;

        /// <summary>
        /// Get an object from the pool.
        /// </summary>
        /// <returns></returns>
        public abstract T Get();

        /// <summary>
        /// Release an object back to the pool.
        /// </summary>
        /// <param name="t"></param>
        public abstract void Release(T t);

        /// <summary>
        /// Swap the two objects in the pool.
        /// </summary>
        /// <param name="i0">Object 1</param>
        /// <param name="i1">Object 2</param>
        protected void Swap(int i0, int i1)
        {
            indices[pool[i0].t] = i1;
            indices[pool[i1].t] = i0;

            (pool[i0], pool[i1]) = (pool[i1], pool[i0]);
        }
    }
}
