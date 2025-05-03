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

using JetBrains.Annotations;
using Meta.WitAi.Composer.Integrations;
using UnityEngine.Scripting;

namespace Meta.WitAi.Composer.Data
{
    /// <summary>
    /// The NamedPath path is the reserved path, awkwardly named 'path'.
    /// This is used within composer to choose which "named path" to use,
    /// notably when an intent isn't found.
    /// </summary>
    [UsedImplicitly]
    public class NamedPath : ReservedContextPath
    {
        protected override string ReservedPath => WitComposerConstants.CONTEXT_MAP_RESERVED_PATH;
        [Preserve]
        public NamedPath() { }

        public override string ToString()
        {
            return GetValue();
        }
    }
}
