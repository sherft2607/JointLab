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

using System.Collections.Generic;
using Meta.XR.Util;

namespace Meta.XR.MRUtilityKit.SceneDecorator
{
    /// <summary>
    /// A pool of objects that can be reused.
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    [Feature(Feature.Scene)]
    public class FixedPool<T> : Pool<T> where T : class
    {
        /// <summary>
        /// Returns the length of the pool.
        /// </summary>
        protected override int CountAll
        {
            get => pool.Length;
        }

        /// <summary>
        /// Returns the active objects in the pool.
        /// </summary>
        protected override int CountActive
        {
            get => index;
        }

        /// <summary>
        /// Provides a fixed pool with the given size and the type of primitive. Use callbacks to register specific callbacks
        /// </summary>
        /// <param name="primitive">The primitive (gameobject)</param>
        /// <param name="size">Size of the pool</param>
        /// <param name="callbacks">Custom or generic callbacks to attach for create, onGet, onRelease</param>
        public FixedPool(T primitive, int size, Callbacks callbacks)
        {
            pool = new Entry[size];
            indices = new Dictionary<T, int>(size);
            index = 0;
            this.callbacks = callbacks;

            for (int i = 0; i < size; ++i)
            {
                T t = callbacks.Create(primitive);
                pool[i].t = t;
                indices[t] = i;
            }
        }

        public override T Get()
        {
            if (index >= pool.Length)
            {
                return null;
            }

            T t = pool[index].t;
            pool[index].active = true;
            if (callbacks.OnGet != null)
            {
                callbacks.OnGet(t);
            }

            ++index;

            return t;
        }

        public override void Release(T t)
        {
            var eIndex = indices[t];
            if (!pool[eIndex].active) //Protect against double releasing
            {
                return;
            }

            pool[eIndex].active = false;

            --index;
            Swap(eIndex, index);
            if (callbacks.OnRelease != null)
            {
                callbacks.OnRelease(t);
            }
        }
    }
}
