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
using System.Linq;
using System.Text;
using UnityEngine;

namespace Meta.WitAi.Composer.Data
{
    /// <summary>
    /// This class manages a specific path within the Composer graph as a dictionary
    /// of values.
    /// It can be extended to any specific path ID.
    /// </summary>
    public abstract class ReservedContextPathDictionary : BaseReservedContextPath
    {
        /// <summary>
        /// Key is a unique lookup, value is the item to add to the map.
        /// </summary>
        private readonly Dictionary<string, string> _runtimeDynamicContext = new Dictionary<string, string>();
        private ComposerService _composer;

        /// <summary>
        /// The separator used when joining the different dictionary values before
        /// sending to Composer.
        /// </summary>
        protected readonly string Separator = "\n";

        protected ReservedContextPathDictionary() { }

        /// <summary>
        /// Returns the backing dictionary for this context map path.
        /// </summary>
        public Dictionary<string, string> GetDictionary()
        {
            return _runtimeDynamicContext;
        }

        /// <summary>
        /// Standard overload to access the dictionary by index.
        /// </summary>
        public string this[string key]
        {
            get => _runtimeDynamicContext.ContainsKey(key) ? _runtimeDynamicContext[key] : string.Empty;
            set => Set(key, value);
        }

        /// <summary>
        /// Add a dynamic context value to the set of dynamic context strings. When sent
        /// to the server, it is sent as a set of text lines in a single context map field.
        /// </summary>
        /// <param name="key">the lookup key to reference this context again.
        ///     Used as the context, if context isn't set.</param>
        /// <param name="context">The value you want to set, generally transient descriptive state.
        ///     Defaults to the key value if unspecified.</param>
        /// <returns>true if successfully added, false if the key already existed</returns>
        public bool Add(string key, string context = null)
        {
            if (_runtimeDynamicContext.ContainsKey(key)) return false;

            Set(key, context);
            return true;
        }
        /// <summary>
        /// Sets the given context in the collection, adding it if it doesn't exist
        /// </summary>
        /// <param name="key">the lookup key to reference this context again.
        /// Used as the context, if context isn't set.</param>
        /// <param name="context">The value you want to set, generally transient descriptive state.
        /// Defaults to the key value if unspecified.</param>
        public void Set(string key, string context = null)
        {
            context ??= key;
            _runtimeDynamicContext[key] = context;
            UpdateContextMap();
        }


        /// <summary>
        /// Removes a line of context from the dynamic context state
        /// </summary>
        /// <param name="key">the key provided when adding the context.
        /// If no key was added, use the context.</param>
        public void Remove(string key)
        {
            _runtimeDynamicContext.Remove(key);

            var context = string.Join(Separator, _runtimeDynamicContext.Values);
        }

        /// <summary>
        /// Sets the current context map's value for this reserved path.
        /// </summary>
        protected internal override void UpdateContextMap()
        {
            string dynamicContext = string.Join(Separator, _runtimeDynamicContext.Values);
            Map.SetData(ReservedPath, dynamicContext);
            var context = string.Join(Separator, _runtimeDynamicContext.Values);
        }

        /// <summary>
        /// Removes all context of this type
        /// </summary>
        public override void Clear()
        {
            _runtimeDynamicContext.Clear();
            base.Clear();
        }

        /// <summary>
        /// Prints out the contents of the dictionary
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Reserved Path: {ReservedPath}");
            for (var i = 0; i < _runtimeDynamicContext.Keys.Count; i++)
            {
                var key = _runtimeDynamicContext.Keys.ElementAt(i);
                sb.Append($"\n\t[{i}] {key} : {_runtimeDynamicContext[key]}");

            }
            return sb.ToString();
        }
    }
}
