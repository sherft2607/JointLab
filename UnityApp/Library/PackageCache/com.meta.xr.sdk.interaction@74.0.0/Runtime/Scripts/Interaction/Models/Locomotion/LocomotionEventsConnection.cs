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
using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
    /// <summary>
    /// This component serves as a nexus point between LocomotionEventBroadcasters
    /// and a LocomotionEventHandler. Use it to group several LocomotionEventBroadcasters
    /// and re-route them to your player controller, or invoke it directly to get the event
    /// forwarded.
    /// </summary>
    public class LocomotionEventsConnection : MonoBehaviour
        , ILocomotionEventHandler, ILocomotionEventBroadcaster
    {
        [SerializeField, Interface(typeof(ILocomotionEventBroadcaster))]
        [Optional(OptionalAttribute.Flag.DontHide)]
        private List<UnityEngine.Object> _broadcasters;
        private IEnumerable<ILocomotionEventBroadcaster> Broadcasters { get; set; }

        [SerializeField, Interface(typeof(ILocomotionEventHandler))]
        private UnityEngine.Object _handler;
        private ILocomotionEventHandler Handler { get; set; }

        private bool _started;

        public event Action<LocomotionEvent> WhenLocomotionPerformed = delegate { };

        public event Action<LocomotionEvent, Pose> WhenLocomotionEventHandled
        {
            add
            {
                Handler.WhenLocomotionEventHandled += value;
            }
            remove
            {
                Handler.WhenLocomotionEventHandled -= value;
            }
        }

        protected virtual void Awake()
        {
            Broadcasters = _broadcasters.ConvertAll(b => b as ILocomotionEventBroadcaster);
            Handler = _handler as ILocomotionEventHandler;
        }

        protected virtual void Start()
        {
            this.BeginStart(ref _started);
            this.AssertCollectionItems(Broadcasters, nameof(Broadcasters));
            this.AssertField(Handler, nameof(Handler));
            this.EndStart(ref _started);
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
                foreach (var eventRaiser in Broadcasters)
                {
                    eventRaiser.WhenLocomotionPerformed += HandleLocomotionEvent;
                }
            }
        }

        protected virtual void OnDisable()
        {
            if (_started)
            {
                foreach (var eventRaiser in Broadcasters)
                {
                    eventRaiser.WhenLocomotionPerformed -= HandleLocomotionEvent;
                }
            }
        }

        public void HandleLocomotionEvent(LocomotionEvent locomotionEvent)
        {
            if (_started && this.isActiveAndEnabled)
            {
                WhenLocomotionPerformed.Invoke(locomotionEvent);
                Handler.HandleLocomotionEvent(locomotionEvent);
            }
        }

        #region Inject
        [Obsolete("Broadcasters is Optional, use with " + nameof(InjectOptionalBroadcasters))]
        public void InjectAllLocomotionBroadcastersHandlerConnection(
            IEnumerable<ILocomotionEventBroadcaster> broadcasters,
            ILocomotionEventHandler handler)
        {
            InjectOptionalBroadcasters(broadcasters);
            InjectHandler(handler);
        }

        public void InjectAllLocomotionBroadcastersHandlerConnection(
            ILocomotionEventHandler handler)
        {
            InjectHandler(handler);
        }

        public void InjectOptionalBroadcasters(IEnumerable<ILocomotionEventBroadcaster> broadcasters)
        {
            Broadcasters = broadcasters;
        }

        public void InjectHandler(ILocomotionEventHandler handler)
        {
            _handler = handler as UnityEngine.Object;
            Handler = handler;
        }

        #endregion
    }
}
