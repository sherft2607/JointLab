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
    /// A pool manager that manages pools of primitives.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="P"></typeparam>
    [Feature(Feature.Scene)]
    public class PoolManager<K, P> where K : class
        where P : Pool<K>
    {
        private Dictionary<K, P> pools = new Dictionary<K, P>();

        /// <summary>
        /// Adds a pool to the collection, associated with the given primitive.
        /// </summary>
        /// <param name="primitive">The primitive object used as a key for the pool.</param>
        /// <param name="pool">The pool to add to the collection.</param>
        public void AddPool(K primitive, P pool)
        {
            pools.Add(primitive, pool);
        }

        /// <summary>
        /// Checks if a pool exists in the collection for the given primitive.
        /// </summary>
        /// <param name="primitive">The primitive object used as a key for the pool.</param>
        /// <returns>True if a pool exists for the given primitive, false otherwise.</returns>
        public bool ContainsPool(K primitive)
        {
            return pools.ContainsKey(primitive);
        }

        /// <summary>
        /// Retrieves the pool associated with the given primitive from the collection.
        /// </summary>
        /// <param name="primitive">The primitive object used as a key for the pool.</param>
        /// <returns>The pool associated with the given primitive, or null if no such pool exists.</returns>
        public P GetPool(K primitive)
        {
            pools.TryGetValue(primitive, out var pool);
            return pool;
        }
    }
}
