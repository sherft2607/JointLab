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

namespace Oculus.Interaction
{
    /// <summary>
    /// Exposes Unity events that broadcast state changes from an <see cref="IInteractorView"/> (an Interactor).
    /// </summary>
    public class InteractorUnityEventWrapper : MonoBehaviour
    {
        /// <summary>
        /// The <see cref="IInteractorView"/> (Interactor) component to wrap.
        /// </summary>
        [Tooltip("The IInteractorView (Interactor) component to wrap.")]
        [SerializeField, Interface(typeof(IInteractorView))]
        private UnityEngine.Object _interactorView;
        private IInteractorView InteractorView;

        /// <summary>
        /// Raised when the Interactor is enabled.
        /// </summary>
        [Tooltip("Raised when the Interactor is enabled.")]
        [SerializeField]
        private UnityEvent _whenEnabled;

        /// <summary>
        /// Raised when the Interactor is disabled.
        /// </summary>
        [Tooltip("Raised when the Interactor is disabled.")]
        [SerializeField]
        private UnityEvent _whenDisabled;

        /// <summary>
        /// Raised when the Interactor is hovering over an Interactable.
        /// </summary>
        [Tooltip("Raised when the Interactor is hovering over an Interactable.")]
        [SerializeField]
        private UnityEvent _whenHover;

        /// <summary>
        /// Raised when the Interactor stops hovering over an Interactable.
        /// </summary>
        [Tooltip("Raised when the stops hovering over an Interactable.")]
        [SerializeField]
        private UnityEvent _whenUnhover;

        /// <summary>
        /// Raised when the Interactor selects an Interactable.
        /// </summary>
        [Tooltip("Raised when the Interactor selects an Interactable.")]
        [SerializeField]
        private UnityEvent _whenSelect;

        /// <summary>
        /// Raised when the Interactor stops selecting an Interactable.
        /// </summary>
        [Tooltip("Raised when the Interactor stops selecting an Interactable.")]
        [SerializeField]
        private UnityEvent _whenUnselect;

        [Space]

        /// <summary>
        /// Raised when the Interactor preprocesses
        /// </summary>
        [Tooltip("Raised when the Interactor preprocesses.")]
        [SerializeField]
        private UnityEvent _whenPreprocessed;

        /// <summary>
        /// Raised when the Interactor processes
        /// </summary>
        [Tooltip("Raised when the Interactor processes.")]
        [SerializeField]
        private UnityEvent _whenProcessed;
        /// <summary>
        /// Raised when the Interactor processes
        /// </summary>
        [Tooltip("Raised when the Interactor processes.")]
        [SerializeField]
        private UnityEvent _whenPostprocessed;

        public UnityEvent WhenDisabled => _whenDisabled;
        public UnityEvent WhenEnabled => _whenEnabled;
        public UnityEvent WhenHover => _whenHover;
        public UnityEvent WhenUnhover => _whenUnhover;
        public UnityEvent WhenSelect => _whenSelect;
        public UnityEvent WhenUnselect => _whenUnselect;

        public UnityEvent WhenPreprocessed => _whenPreprocessed;
        public UnityEvent WhenProcessed => _whenProcessed;
        public UnityEvent WhenPostprocessed => _whenPostprocessed;

        protected bool _started = false;

        protected virtual void Awake()
        {
            InteractorView = _interactorView as IInteractorView;
        }

        protected virtual void Start()
        {
            this.BeginStart(ref _started);
            this.AssertField(InteractorView, nameof(InteractorView));
            this.EndStart(ref _started);
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
                InteractorView.WhenStateChanged += HandleStateChanged;
                InteractorView.WhenPreprocessed += HandlePreprocessed;
                InteractorView.WhenProcessed += HandleProcessed;
                InteractorView.WhenPostprocessed += HandlePostprocessed;
            }
        }

        protected virtual void OnDisable()
        {
            if (_started)
            {
                InteractorView.WhenStateChanged -= HandleStateChanged;
                InteractorView.WhenPreprocessed -= HandlePreprocessed;
                InteractorView.WhenProcessed -= HandleProcessed;
                InteractorView.WhenPostprocessed -= HandlePostprocessed;
            }
        }

        private void HandleStateChanged(InteractorStateChangeArgs args)
        {
            switch (args.NewState)
            {
                case InteractorState.Disabled:
                    _whenDisabled.Invoke();
                    break;
                case InteractorState.Normal:
                    if (args.PreviousState == InteractorState.Hover)
                    {
                        _whenUnhover.Invoke();
                    }
                    else if (args.PreviousState == InteractorState.Disabled)
                    {
                        _whenEnabled.Invoke();
                    }
                    break;
                case InteractorState.Hover:
                    if (args.PreviousState == InteractorState.Normal)
                    {
                        _whenHover.Invoke();
                    }
                    else if (args.PreviousState == InteractorState.Select)
                    {
                        _whenUnselect.Invoke();
                    }

                    break;
                case InteractorState.Select:
                    if (args.PreviousState == InteractorState.Hover)
                    {
                        _whenSelect.Invoke();
                    }

                    break;
            }
        }

        private void HandlePreprocessed()
        {
            _whenPreprocessed.Invoke();
        }

        private void HandleProcessed()
        {
            _whenProcessed.Invoke();
        }

        private void HandlePostprocessed()
        {
            _whenPostprocessed.Invoke();
        }



        #region Inject

        public void InjectAllInteractorUnityEventWrapper(IInteractorView interactorView)
        {
            InjectInteractorView(interactorView);
        }

        public void InjectInteractorView(IInteractorView interactorView)
        {
            _interactorView = interactorView as UnityEngine.Object;
            InteractorView = interactorView;
        }

        #endregion
    }
}
