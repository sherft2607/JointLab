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
using Oculus.Interaction.Input;
using System.Linq;
using UnityEngine.Serialization;

namespace Oculus.Interaction
{
    public class JoystickPoseMovementProvider : MonoBehaviour, IMovementProvider
    {
        [SerializeField, Interface(typeof(IInteractableView))]
        private MonoBehaviour _interactable;

        private IInteractableView _interactableView;

        [FormerlySerializedAs("moveSpeed")]
        [SerializeField, Optional]
        [Tooltip("The speed at which movement occurs.")]
        private float _moveSpeed = 0.04f;
        public float MoveSpeed
        {
            get => _moveSpeed;
            set => _moveSpeed = value;
        }

        [FormerlySerializedAs("rotationSpeed")]
        [SerializeField, Optional]
        [Tooltip("The speed at which rotation occurs.")]
        private float _rotationSpeed = 1.0f;
        public float RotationSpeed
        {
            get => _rotationSpeed;
            set => _rotationSpeed = value;
        }

        [SerializeField, Optional, Range(0f, 10f)]
        [Tooltip("The minimum distance along the Z-axis for the grabbed object.")]
        private float _minDistance = 0.1f;
        public float MinDistance
        {
            get => _minDistance;
            set => _minDistance = value;
        }

        [SerializeField, Optional, Range(1f, 10f)]
        [Tooltip("The maximum distance along the Z-axis for the grabbed object.")]
        private float _maxDistance = 3.0f;
        public float MaxDistance
        {
            get => _maxDistance;
            set => _maxDistance = value;
        }

        private IInteractorView _latestSelectingInteractor;

        private void Awake()
        {
            _interactableView = _interactable as IInteractableView;
        }

        private void OnEnable()
        {
            if (_interactableView != null)
            {
                _interactableView.WhenSelectingInteractorViewAdded += OnSelectingInteractorViewAdded;
                _interactableView.WhenSelectingInteractorViewRemoved += OnSelectingInteractorViewRemoved;
            }
        }

        private void OnDisable()
        {
            if (_interactableView != null)
            {
                _interactableView.WhenSelectingInteractorViewAdded -= OnSelectingInteractorViewAdded;
                _interactableView.WhenSelectingInteractorViewRemoved -= OnSelectingInteractorViewRemoved;
            }
        }

        private void OnSelectingInteractorViewAdded(IInteractorView interactor)
        {
            _latestSelectingInteractor = interactor;
        }

        private void OnSelectingInteractorViewRemoved(IInteractorView interactor)
        {
            if (_latestSelectingInteractor == interactor)
            {
                // If the latest interactor is removed, reset or find the next latest
                _latestSelectingInteractor = _interactableView.SelectingInteractorViews.LastOrDefault();
            }
        }

        public IMovement CreateMovement()
        {
            IController controller = null;

            if (_latestSelectingInteractor != null)
            {
                InteractorControllerDecorator.TryGetControllerForInteractor(_latestSelectingInteractor, out controller);
            }

            return new JoystickPoseMovement(controller, _moveSpeed, _rotationSpeed, _minDistance, _maxDistance);
        }
    }

    public class JoystickPoseMovement : IMovement
    {
        public Pose Pose => _currentPose;
        public bool Stopped => false;

        private Pose _currentPose;
        private Pose _targetPose;
        private Vector3 _localDirection;
        private IController _controller;
        private float _moveSpeed;
        private float _rotationSpeed;
        private float _minDistance;
        private float _maxDistance;


        public JoystickPoseMovement(IController controller, float moveSpeed, float rotationSpeed, float minDistance, float maxDistance)
        {
            _controller = controller;
            _moveSpeed = moveSpeed;
            _rotationSpeed = rotationSpeed;
            _minDistance = minDistance;
            _maxDistance = maxDistance;
        }

        public void MoveTo(Pose target)
        {
            _targetPose = target;
            _localDirection = Quaternion.Inverse(_targetPose.rotation)
                * (_currentPose.position - _targetPose.position).normalized;
        }

        public void UpdateTarget(Pose target)
        {
            _targetPose = target;
        }

        public void StopAndSetPose(Pose pose)
        {
            _currentPose = pose;
        }

        public void Tick()
        {
            AdjustPoseWithJoystickInput();
        }

        public void AdjustPoseWithJoystickInput()
        {
            if (_controller == null)
            {
                return;
            }

            Vector2 joystickInput = _controller.ControllerInput.Primary2DAxis;

            float moveDelta = joystickInput.y * _moveSpeed;
            float rotationDelta = -joystickInput.x * _rotationSpeed;

            Vector3 direction = _targetPose.rotation * _localDirection;
            Vector3 controllerPosition = _targetPose.position;
            Vector3 controllerToObject = _currentPose.position - controllerPosition;

            float currentDistanceAlongForward = Vector3.Project(controllerToObject, direction).magnitude;
            float newDistanceAlongForward = Mathf.Clamp(currentDistanceAlongForward + moveDelta, _minDistance, _maxDistance);

            Vector3 newPosition = controllerPosition + direction * newDistanceAlongForward;
            Quaternion newRotation = Quaternion.AngleAxis(rotationDelta, Vector3.up) * _currentPose.rotation;

            _currentPose = new Pose(newPosition, newRotation);
            UpdateTarget(_currentPose);
        }

        public void InjectController(IController controller)
        {
            _controller = controller;
        }
    }
}
