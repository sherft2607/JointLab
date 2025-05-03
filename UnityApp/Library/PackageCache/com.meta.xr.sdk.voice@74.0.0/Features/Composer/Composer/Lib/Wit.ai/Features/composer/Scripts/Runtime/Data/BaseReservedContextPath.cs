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
    /// This class represents a specific path within the Composer graph which
    /// has specific meaning -- it is reserved for a specific use.
    /// This class is intended to be extended so as to represent specific path IDs.
    ///
    /// Note also that it is a IContextMapReservedPathExtension, so it will be
    /// automatically registered with the composer's context map.
    /// </summary>
    public abstract class BaseReservedContextPath : IContextMapReservedPathExtension
    {
        protected ComposerContextMap Map => _composer.CurrentContextMap;
        protected abstract string ReservedPath { get; }

        private ComposerService _composer;

        /// <summary>
        /// Should only be true if this extension has been successfully integrated
        /// with the composer service.
        /// </summary>
        public bool HasComposer;

        protected BaseReservedContextPath() { }

        /// <summary>
        /// Ensures that the data in this object is present to the Composer
        /// Context Map.
        /// </summary>
        protected internal abstract void UpdateContextMap();


        /// <summary>
        /// Assigns these path and value to the map of the given composer to which this will belong and
        /// does whatever setup is required for this plugin.
        /// </summary>
        /// <param name="composer">the composer object to to which this collection is related.</param>
        public virtual void AssignTo(ComposerService composer)
        {
            if (_composer == composer) return;
            _composer = composer;
            ComposerContextMap.ReservedPaths.Add(ReservedPath);

            HasComposer = true;
        }

        /// <summary>
        /// Removes all context of this type
        /// </summary>
        public virtual void Clear()
        {
            Map?.ClearData(ReservedPath);
        }
    }
}
