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

using UnityEngine;
using UnityEngine.Events;
using Oculus.Interaction.Input;

namespace Oculus.Interaction
{
    class MicroGestureUnityEventWrapper : MonoBehaviour
    {
        [SerializeField]
        private OVRMicrogestureEventSource _ovrMicrogestureEventSource;

        [SerializeField]
        private UnityEvent _whenTapCenter;
        public UnityEvent WhenTapCenter => _whenTapCenter;

        [SerializeField]
        private UnityEvent _whenSwipeUp;
        public UnityEvent WhenSwipeUp => _whenSwipeUp;

        [SerializeField]
        private UnityEvent _whenSwipeDown;
        public UnityEvent WhenSwipeDown => _whenSwipeDown;

        [SerializeField, Interface(typeof(IHand))]
        [Optional(OptionalAttribute.Flag.DontHide)]
        [Tooltip("Reference a Hand if you want to emit swipe events that disambiguate" +
            " between the left and the right direction.")]
        private UnityEngine.Object _hand;
        private IHand Hand { get; set; }

        [SerializeField]
        private UnityEvent _whenSwipeLeft;
        public UnityEvent WhenSwipeLeft => _whenSwipeLeft;

        [SerializeField]
        private UnityEvent _whenSwipeRight;
        public UnityEvent WhenSwipeRight => _whenSwipeRight;

        private bool _started = false;

        private void Awake()
        {
            Hand = _hand as IHand;
        }

        protected virtual void Start()
        {
            this.BeginStart(ref _started);

            if (_hand != null)
            {
                this.AssertField(Hand, nameof(_hand));
            }
            this.AssertField(_ovrMicrogestureEventSource, nameof(_ovrMicrogestureEventSource));

            this.EndStart(ref _started);
        }
        private void OnEnable()
        {
            _ovrMicrogestureEventSource.GestureRecognizedEvent.AddListener(HandleGesture);
        }

        private void OnDisable()
        {
            _ovrMicrogestureEventSource.GestureRecognizedEvent.RemoveListener(HandleGesture);
        }

        private void HandleGesture(OVRHand.MicrogestureType gesture)
        {
            if (gesture == OVRHand.MicrogestureType.SwipeRight)
            {
                _whenSwipeRight.Invoke();
            }
            else if (gesture == OVRHand.MicrogestureType.SwipeLeft)
            {
                _whenSwipeLeft.Invoke();
            }
            else if (gesture == OVRHand.MicrogestureType.SwipeForward)
            {
                _whenSwipeUp.Invoke();
            }
            else if (gesture == OVRHand.MicrogestureType.SwipeBackward)
            {
                _whenSwipeDown.Invoke();
            }
            else if (gesture == OVRHand.MicrogestureType.ThumbTap)
            {
                _whenTapCenter.Invoke();
            }
        }

        #region Inject

        public void InjectAllMicroGestureUnityEventWrapper(
            OVRMicrogestureEventSource ovrMicrogestureEventSource)
        {
            InjectOvrMicrogestureEventSource(ovrMicrogestureEventSource);
        }

        public void InjectOvrMicrogestureEventSource(OVRMicrogestureEventSource ovrMicrogestureEventSource)
        {
            _ovrMicrogestureEventSource = ovrMicrogestureEventSource;
        }

        public void InjectOptionalHand(IHand hand)
        {
            _hand = hand as UnityEngine.Object;
            Hand = hand;
        }
        #endregion
    }
}
