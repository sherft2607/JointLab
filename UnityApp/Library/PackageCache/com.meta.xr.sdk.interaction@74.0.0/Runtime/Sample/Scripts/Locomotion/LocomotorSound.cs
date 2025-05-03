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

namespace Oculus.Interaction.Locomotion
{
    public class LocomotorSound : MonoBehaviour
    {
        [SerializeField, Interface(typeof(ILocomotionEventHandler))]
        private UnityEngine.Object _locomotor;
        private ILocomotionEventHandler Locomotor { get; set; }

        [SerializeField]
        private AdjustableAudio _translationSound;
        [SerializeField]
        private AdjustableAudio _translationDeniedSound;
        [SerializeField]
        private AdjustableAudio _snapTurnSound;

        [SerializeField]
        private AnimationCurve _translationCurve = AnimationCurve.EaseInOut(0f, 0f, 2f, 1f);
        [SerializeField]
        private AnimationCurve _rotationCurve = AnimationCurve.EaseInOut(0f, 0f, 180f, 1f);
        [SerializeField]
        private float _pitchVariance = 0.05f;

        protected bool _started;

        protected virtual void Awake()
        {
            Locomotor = _locomotor as ILocomotionEventHandler;
        }

        protected virtual void Start()
        {
            this.BeginStart(ref _started);
            this.AssertField(Locomotor, nameof(_locomotor));
            this.AssertField(_translationSound, nameof(_translationSound));
            this.AssertField(_translationDeniedSound, nameof(_translationDeniedSound));
            this.AssertField(_snapTurnSound, nameof(_snapTurnSound));
            this.EndStart(ref _started);
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
                Locomotor.WhenLocomotionEventHandled += HandleLocomotionEvent;
            }
        }

        protected virtual void OnDisable()
        {
            if (_started)
            {
                Locomotor.WhenLocomotionEventHandled -= HandleLocomotionEvent;
            }
        }

        private void HandleLocomotionEvent(LocomotionEvent locomotionEvent, Pose delta)
        {
            if (locomotionEvent.Translation == LocomotionEvent.TranslationType.Absolute
                || locomotionEvent.Translation == LocomotionEvent.TranslationType.AbsoluteEyeLevel
                || locomotionEvent.Translation == LocomotionEvent.TranslationType.Relative)
            {
                PlayTranslationSound(delta.position.magnitude);
            }
            if (locomotionEvent.Rotation == LocomotionEvent.RotationType.Relative)
            {
                PlayRotationSound(delta.rotation.y * delta.rotation.w);
            }
            if (locomotionEvent.Translation == LocomotionEvent.TranslationType.None
                && locomotionEvent.Rotation == LocomotionEvent.RotationType.None)
            {
                PlayDenialSound(delta.position.magnitude);
            }
        }

        private void PlayTranslationSound(float translationDistance)
        {
            float t = _translationCurve.Evaluate(translationDistance);
            float pitch = t + Random.Range(-_pitchVariance, _pitchVariance);
            _translationSound.PlayAudio(t, pitch);
        }

        private void PlayDenialSound(float translationDistance)
        {
            float t = _translationCurve.Evaluate(translationDistance);
            float pitch = t + Random.Range(-_pitchVariance, _pitchVariance);
            _translationDeniedSound.PlayAudio(t, pitch);
        }

        private void PlayRotationSound(float rotationLength)
        {
            float t = _rotationCurve.Evaluate(Mathf.Abs(rotationLength));
            float pitch = t + Random.Range(-_pitchVariance, _pitchVariance);
            _snapTurnSound.PlayAudio(t, pitch, rotationLength);
        }

        #region Inject

        public void InjectAllLocomotorSound(ILocomotionEventHandler locomotor)
        {
            InjectPlayerLocomotor(locomotor);
        }

        public void InjectPlayerLocomotor(ILocomotionEventHandler locomotor)
        {
            _locomotor = locomotor as UnityEngine.Object;
            Locomotor = locomotor;
        }
        #endregion
    }
}
