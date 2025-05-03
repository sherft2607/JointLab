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

using Meta.WitAi.Json;

namespace Meta.Voice.Composer.Data
{
    /// <summary>
    /// A class which can receive a Context Node from Composer, in JSON format, and parse it.
    /// </summary>
    internal interface IContextNodeParser
    {
        /// <param name="contextType"></param>
        /// <returns>true if it can parse a context node of the given type. False otherwise</returns>
        public bool HandlesType(string contextType);

        /// <summary>
        /// Processes the given node
        /// </summary>
        /// <param name="module">the Context node to parse</param>
        /// <param name="composerParser">The general composer parser through which results can be saved</param>
        public void ProcessNode(WitResponseNode module, ComposerParser composerParser);
    }
}
