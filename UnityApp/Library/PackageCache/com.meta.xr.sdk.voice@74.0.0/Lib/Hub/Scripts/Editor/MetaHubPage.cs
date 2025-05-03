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

using Meta.Voice.Hub.Interfaces;
using UnityEngine;

namespace Meta.Voice.Hub
{
    public class MetaHubPage : ScriptableObject, IMetaHubPage, IPageInfo
    {
        /// <summary>
        /// The context this page will fall under
        /// </summary>
        [SerializeField] private string _context;
        /// <summary>
        /// A prefix that will show up before the name of the page. This is a good place to insert page hierarchy etc.
        /// </summary>
        [SerializeField] private string _prefix;
        /// <summary>
        /// The sorting priority of the page
        /// </summary>
        [SerializeField] private int _priority;

        public virtual string Name => name;
        public virtual string Context => _context;
        public virtual int Priority => _priority;
        public virtual string Prefix => _context;

        public virtual void OnGUI()
        {

        }
    }
}
