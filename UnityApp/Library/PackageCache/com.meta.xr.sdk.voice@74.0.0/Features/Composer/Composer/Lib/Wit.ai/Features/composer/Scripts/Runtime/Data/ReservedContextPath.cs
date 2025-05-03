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

namespace Meta.WitAi.Composer.Data
{
    /// <summary>
    /// This class represents a specific path within the Composer graph.
    /// It can be extended to any specific path ID.
    /// </summary>
    public abstract class ReservedContextPath : BaseReservedContextPath
    {
        protected ReservedContextPath() { }

        /// <summary>
        /// The value of the path field.
        /// </summary>
        private string _value;

        /// <returns>the string representation of the value at this reserved path.</returns>
        public string GetValue()
        {
            return _value;
        }

        /// <summary>
        /// Sets the value of the reserved path
        /// </summary>
        /// <param name="value">The value you want to set, generally transient descriptive state.</param>
        public void Set(string value)
        {
            _value = value;
            UpdateContextMap();
        }

        /// <summary>
        /// Sets the current context map's value for this reserved path.
        /// </summary>
        protected internal override void UpdateContextMap()
        {
            if (Map == null)
            {
                VLog.W($"Missing Composer map for { this }");
            }
            Map?.SetData(ReservedPath, _value);
        }

        /// <summary>
        /// Removes all context of this type
        /// </summary>
        public override void Clear()
        {
            _value = string.Empty;
            base.Clear();
        }

        public override string ToString()
        {
            return $"{ReservedPath} : {_value}";
        }
    }
}
