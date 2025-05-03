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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
    /// <summary>
    /// This locomotion handler will respond to locomotion events by moving a character
    /// represented as a CapsuleCollider and keep it in sync with the VR player (represented
    /// by its root transform and the center eye transform).
    /// Appart from the locomotion event types it can also react to external calls for jumping,
    /// crouching or running.
    /// When moved using velocities, the character will collide with the world, preventing the
    /// capsule from penetrating through colliders or falling through the floor.
    ///
    /// Velocity is applied during Update, but other Locomotion events are not applied instantly, instead they are collected
    /// and applied all once at the end of the frame to avoid desyncs between interactions.
    ///
    /// Translation events:
    /// LocomotionEvent.Translation.Velocity: will add to the current velocity of the character and
    /// a delta will be applied every update to the capsule, checking for collisions and also dampening
    /// the velocity along time.
    /// LocomotionEvent.Translation.Relative: will move the character in the requested direction by also checking
    /// for collisions.
    /// LocomotionEvent.Translation.Absolute: will move the character's feet to the requested position
    /// while trying to snap it to the ground if found under the requested pose.
    /// LocomotionEvent.Translation.AbsoluteEyeLevel: will move the character so it's head ends up placed at the requested
    /// pose (not the feet). By doing this the capsule can potentially become missaligned with the ground so physics will be
    /// disabled until a new event is registered, the player physically moves _exitHotspotDistance or EnableMovement is called manually.
    ///
    /// Rotation events:
    /// LocomotionEvent.Rotation.Velocity: will rotate the character by this angular velocity using the provided delta time.
    /// LocomotionEvent.Rotation.Absolute: will force the character to face the requested absolute rotation.
    /// LocomotionEvent.Rotation.Relative: will force the character to rotate the requested amount.
    ///
    /// Other behaviors:
    /// When Player transforms are provided, the height of the capsule will stay in sync with the actual player height
    /// (distance from the top of their head to their floor), when the heigth increases it will also check for collisions
    /// above to avoid the character from clipping with low ceilings and other obstacles.
    /// When the character and player transforms difer more than _maxWallPenetrationDistance, the player will instantly be
    /// teleported to the character position if an input is registered. This can be manually controller by calling ResetPlayerToCharacter manually.
    /// When moving using Translation.Velocity or Translation.Relative, if a collision against a wall is registered, the capsule will try to rebound
    /// and slide along it using _maxReboundSteps iterations.
    /// </summary>
    [Obsolete("Use " + nameof(FirstPersonLocomotor) + " instead")]
    public class CapsuleLocomotionHandler : MonoBehaviour,
        ILocomotionEventHandler, IDeltaTimeConsumer
    {
        [Header("Character")]
        /// <summary>
        /// Capsule collider that represents the character
        /// and will be moved by the locomotor.
        /// </summary>
        [SerializeField]
        [Tooltip("Capsule collider that represents the character and will be moved by the locomotor.")]
        private CapsuleCollider _capsule;

        /// <summary>
        /// Extra offset added to the radius of the capsule for
        /// soft collisions.
        /// </summary>
        [SerializeField, Min(0f)]
        [Tooltip("Extra offset added to the radius of the capsule for soft collisions.")]
        private float _skinWidth = 0.02f;
        public float SkinWidth
        {
            get => _skinWidth;
            set => _skinWidth = value;
        }

        /// <summary>
        /// LayerMask check for collisions when moving.
        /// </summary>
        [SerializeField]
        [Tooltip("LayerMask check for collisions when moving.")]
        private LayerMask _layerMask = -1;
        public LayerMask LayerMask
        {
            get => _layerMask;
            set => _layerMask = value;
        }

        [Header("VR Player (Optional)")]
        /// <summary>
        /// Optional. Root of the actual VR player so it
        /// can be sync with with capsule.
        /// If you provided a _playerEyes you must also
        /// provide a _playerOrigin.
        /// </summary>
        [SerializeField, Optional]
        [Tooltip("Optional. Root of the actual VR player so it can be sync with with capsule. If you provided a _playerEyes you must also provide a _playerOrigin.")]
        private Transform _playerOrigin;

        /// <summary>
        /// Optional. Eyes of the actual VR player so it
        /// can be sync with the capsule.
        /// If you provided a _playerOrigin you must also
        /// provide a _playerEyes.
        /// </summary>
        [SerializeField, Optional]
        [Tooltip("Optional. Eyes of the actual VR player so it can be sync with the capsule. If you provided a _playerOrigin you must also provide a _playerEyes.")]
        private Transform _playerEyes;

        /// <summary>
        /// After the player penetrates the head inside a collider
        /// (for example a wall), the maximum distance before the player
        /// gets reset to the capsule position when trying to move synthetically.
        /// </summary>
        [SerializeField]
        [Tooltip("After the player penetrates the head inside a collider (for example a wall), the maximum distance before the player gets reset to the capsule position when trying to move synthetically.")]
        private float _maxWallPenetrationDistance = 0.3f;
        public float MaxWallPenetrationDistance
        {
            get => _maxWallPenetrationDistance;
            set => _maxWallPenetrationDistance = value;
        }

        /// <summary>
        /// After using LocomotionEvent.TranslationType.AbsoluteEyeLevel
        /// that disables the ground checks. What is the maximum deviation
        /// of the player before the physics are re-enabled.
        /// </summary>
        [SerializeField]
        [Tooltip("After using LocomotionEvent.TranslationType.AbsoluteEyeLevel that disables the ground checks. What is the maximum deviation of the player before the physics are re-enabled.")]
        private float _exitHotspotDistance = 0.3f;
        public float ExitHotspotDistance
        {
            get => _exitHotspotDistance;
            set => _exitHotspotDistance = value;
        }

        /// <summary>
        /// When _playerOrigin and _playerEyes are present. This will force
        /// the capsule height to update using the actual player height, instead
        /// of using _defaultHeight
        /// </summary>
        [SerializeField]
        [Tooltip("When _playerOrigin and _playerEyes are present. This will force the capsule height to update using the actual player height, instead of using _defaultHeight")]
        private bool _autoUpdateHeight = true;
        public bool AutoUpdateHeight
        {
            get => _autoUpdateHeight;
            set => _autoUpdateHeight = value;
        }

        [Header("Parameters")]
        /// <summary>
        /// Max climbable slope angle in degrees.
        /// </summary>
        [SerializeField, Range(0f, 90f)]
        [Tooltip("Max climbable slope angle in degrees.")]
        private float _maxSlopeAngle = 45f;
        public float MaxSlopeAngle
        {
            get => _maxSlopeAngle;
            set => _maxSlopeAngle = value;
        }

        /// <summary>
        /// Max climbable height for steps.
        /// </summary>
        [SerializeField, Min(0f)]
        [Tooltip("Max climbable height for steps.")]
        private float _maxStep = 0.1f;
        public float MaxStep
        {
            get => _maxStep;
            set => _maxStep = value;
        }

        /// <summary>
        /// Height of the character capsule
        /// when standing normally.
        /// This might be overriden by _autoUpdateHeight
        /// </summary>
        [SerializeField]
        [Tooltip("Height of the character capsule when standing normally. This might be overriden by _autoUpdateHeight")]
        private float _defaultHeight = 1.4f;
        public float DefaultHeight
        {
            get => _defaultHeight;
            set => _defaultHeight = value;
        }

        /// <summary>
        /// General height offset applied to the capsule.
        /// </summary>
        [SerializeField]
        [Tooltip("General height offset applied to the capsule.")]
        private float _heightOffset = 0f;
        public float HeightOffset
        {
            get => _heightOffset;
            set => _heightOffset = value;
        }

        /// <summary>
        /// Height offset added while crouching.
        /// </summary>
        [SerializeField]
        [Tooltip("Height offset added while crouching.")]
        private float _crouchHeightOffset = -0.5f;
        public float CrouchHeightOffset
        {
            get => _crouchHeightOffset;
            set => _crouchHeightOffset = value;
        }

        /// <summary>
        /// Speed multiplier applied while moving normally.
        /// </summary>
        [SerializeField]
        [Tooltip("Speed multiplier applied while moving normally.")]
        private float _speedFactor = 1.0f;
        public float SpeedFactor
        {
            get => _speedFactor;
            set => _speedFactor = value;
        }

        /// <summary>
        /// Speed multiplier applied while crouching.
        /// </summary>
        [SerializeField]
        [Tooltip("Speed multiplier applied while crouching.")]
        private float _crouchSpeedFactor = 0.5f;
        public float CrouchSpeedFactor
        {
            get => _crouchSpeedFactor;
            set => _crouchSpeedFactor = value;
        }

        /// <summary>
        /// Speed multiplier applied while running.
        /// </summary>
        [SerializeField]
        [Tooltip("Speed multiplier applied while running.")]
        private float _runningSpeedFactor = 2f;
        public float RunningSpeedFactor
        {
            get => _runningSpeedFactor;
            set => _runningSpeedFactor = value;
        }

        /// <summary>
        /// The rate of acceleration during movement.
        /// </summary>
        [SerializeField]
        [Tooltip("The rate of acceleration during movement.")]
        private float _acceleration = 70f;
        public float Acceleration
        {
            get => _acceleration;
            set => _acceleration = value;
        }

        /// <summary>
        /// The rate of damping on movement while grounded.
        /// </summary>
        [SerializeField]
        [Tooltip("The rate of damping on movement while grounded.")]
        private float _groundDamping = 30f;
        public float GroundDamping
        {
            get => _groundDamping;
            set => _groundDamping = value;
        }

        /// <summary>
        /// The rate of damping on the vertical movement while jumping.
        /// </summary>
        [SerializeField]
        [Tooltip("The rate of damping on the vertical movement while jumping.")]
        private float _jumpDamping = 30f;
        public float JumpDamping
        {
            get => _jumpDamping;
            set => _jumpDamping = value;
        }
        /// <summary>
        /// The rate of damping on the horizontal movement while in the air.
        /// </summary>
        [SerializeField]
        [Tooltip("The rate of damping on the horizontal movement while in the air.")]
        private float _airDamping = 5f;
        public float AirDamping
        {
            get => _airDamping;
            set => _airDamping = value;
        }

        /// <summary>
        /// The force applied to the character when jumping.
        /// </summary>
        [SerializeField]
        [Tooltip("The force applied to the character when jumping.")]
        private float _jumpForce = 100f;
        public float JumpForce
        {
            get => _jumpForce;
            set => _jumpForce = value;
        }

        /// <summary>
        /// Modifies the strength of gravity.
        /// </summary>
        [SerializeField]
        [Tooltip("Modifies the strength of gravity.")]
        private float _gravityFactor = 1f;
        public float GravityFactor
        {
            get => _gravityFactor;
            set => _gravityFactor = value;
        }

        /// <summary>
        /// Max iterations for sliding the delta movement
        /// after colliding with an obstacle.
        /// </summary>
        [SerializeField, Min(1)]
        [Tooltip("Max iterations for sliding the delta movement after colliding with an obstacle.")]
        private int _maxReboundSteps = 3;
        public int MaxReboundSteps
        {
            get => _maxReboundSteps;
            set => _maxReboundSteps = value;
        }

        /// <summary>
        /// When Velocity is ignored the character will not try to catch up
        /// to the player and the character won't slide or fall.
        /// It is preferred to re-enable the movement by calling EnableMovement
        /// instead of setting this variable to false directly.
        /// </summary>
        [SerializeField]
        [Tooltip("When Velocity is ignored the character will not try to catch up " +
            "to the player and the character won't slide or fall." +
            "It is preferred to re-enable the movement by calling " + nameof(EnableMovement) +
            " instead of setting this variable to false directly.")]
        private bool _velocityDisabled;

        [Header("Anchors")]
        /// <summary>
        /// Optional. This transform pose will be updated
        /// with the pose of the character head.
        /// </summary>
        [SerializeField, Optional]
        [Tooltip("Optional. This transform pose will be updated with the pose of the character head.")]
        private Transform _logicalHead;

        /// <summary>
        /// Optional. This transform pose will be updated
        /// with the pose of the character feet.
        /// </summary>
        [SerializeField, Optional]
        [Tooltip("Optional. This transform pose will be updated with the pose of the character feet.")]
        private Transform _logicalFeet;

        private Func<float> _deltaTimeProvider = () => Time.deltaTime;
        public void SetDeltaTimeProvider(Func<float> deltaTimeProvider)
        {
            _deltaTimeProvider = deltaTimeProvider;
        }

        protected Action<LocomotionEvent, Pose> _whenLocomotionEventHandled = delegate { };
        public event Action<LocomotionEvent, Pose> WhenLocomotionEventHandled
        {
            add
            {
                _whenLocomotionEventHandled += value;
            }
            remove
            {
                _whenLocomotionEventHandled -= value;
            }
        }

        /// <summary>
        /// Indicates whether the character was detected as grounded after
        /// the last move.
        /// </summary>
        public bool IsGrounded => _isGrounded;
        /// <summary>
        /// Indicates whether the character is running
        /// </summary>
        public bool IsRunning => _isRunning;
        /// <summary>
        /// Indicates whether the character is crouching
        /// </summary>
        public bool IsCrouching => _isCrouching;
        /// <summary>
        /// Velocity movements can be disabled either by calling DisableMovement or
        /// teleporting to a Hostpot that sets the Head (instead of the feet) to a specific pose.
        /// </summary>
        public bool IgnoringVelocity => _velocityDisabled || _isHeadInHotspot;

        private bool ControllingPlayer => _playerOrigin != null && _playerEyes != null;

        private Pose _accumulatedDeltaFrame;
        private Vector3 _velocity;
        private bool _isHeadInHotspot;
        private Vector3? _headHotspotCenter;
        private RaycastHit _groundHit;
        private bool _isGrounded;
        private bool _isRunning;
        private bool _isCrouching;

        //Distace from the Sellion to the Top of the Head.
        //Percentile 50 averaged between Male and Female
        private const float _sellionToTopOfHead = 0.1085f;

        //Half distace from the Sellion to the Back of the Head.
        //Percentile 50 averaged between Male and Female
        private const float _sellionToBackOfHeadHalf = 0.0965f;

        private const float _cornerHitEpsilon = 0.001f;

        private Queue<LocomotionEvent> _deferredLocomotionEvent = new Queue<LocomotionEvent>();
        private YieldInstruction _endOfFrame = new WaitForEndOfFrame();
        private Coroutine _endOfFrameRoutine = null;

        protected bool _started;

        protected virtual void Start()
        {
            this.BeginStart(ref _started);

            this.AssertField(_capsule, nameof(_capsule));

            if (_playerOrigin != null || _playerEyes != null)
            {
                this.AssertField(_playerOrigin, nameof(_playerOrigin));
                this.AssertField(_playerEyes, nameof(_playerEyes));
            }

            this.EndStart(ref _started);
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
                _endOfFrameRoutine = StartCoroutine(EndOfFrameCoroutine());
            }
        }

        protected virtual void OnDisable()
        {
            if (_started)
            {
                _accumulatedDeltaFrame = Pose.identity;
                StopCoroutine(_endOfFrameRoutine);
                _endOfFrameRoutine = null;
            }
        }

        protected virtual void Update()
        {
            TryExitHotspot();

            UpdateCharacterHeight();

            if (!IgnoringVelocity)
            {
                CatchUpCharacterToPlayer();

                UpdateVelocity();

                Pose startPose = _capsule.transform.GetPose();
                Vector3 delta = _velocity * _deltaTimeProvider.Invoke();
                MoveCharacter(delta);
                Pose endPose = _capsule.transform.GetPose();
                AccumulateDelta(ref _accumulatedDeltaFrame, startPose, endPose);
            }

            UpdateAnchorPoints();
        }

        protected virtual void LateUpdate()
        {
            ConsumeDeferredLocomotionEvents();
        }

        protected virtual void LastUpdate()
        {
            CatchUpPlayerToCharacter(_accumulatedDeltaFrame, GetCharacterFeet().y);
            _accumulatedDeltaFrame = Pose.identity;
        }

        /// <summary>
        /// Jump vertically if it the character grounded.
        /// If the character is crouching it will stand up instead.
        /// </summary>
        public void Jump()
        {
            if (!_isGrounded)
            {
                return;
            }

            if (_isCrouching)
            {
                Crouch(false);
                return;
            }

            TryExitHotspot(true);

            _velocity += Vector3.up * _jumpForce;
        }

        /// <summary>
        /// If the character is crouching it will stand up
        /// and vice versa
        /// </summary>
        public void ToggleCrouch()
        {
            Crouch(!_isCrouching);
        }

        /// <summary>
        /// When true, it will enable crouching by applying the crouch height offset
        /// to the character and using the crouch speed factor for movement
        /// </summary>
        /// <param name="crouch">True to crouch, false to stand up</param>
        public void Crouch(bool crouch)
        {
            if (_isCrouching != crouch)
            {
                _isCrouching = crouch;
            }
        }

        /// <summary>
        /// If the character is walking it will run
        /// and vice versa
        /// </summary>
        public void ToggleRun()
        {
            Run(!_isRunning);
        }
        /// <summary>
        /// When true, it will enable running by applying
        /// the running speed factor to the movement
        /// </summary>
        /// <param name="run"></param>
        public void Run(bool run)
        {
            if (_isRunning != run)
            {
                _isRunning = run;
                TryExitHotspot(true);
            }
        }

        #region Locomotion Events Handling

        public void HandleLocomotionEvent(LocomotionEvent locomotionEvent)
        {
            //Velocity translations get added directly (but will be applied during update)
            if (locomotionEvent.Translation == LocomotionEvent.TranslationType.Velocity)
            {
                AddVelocity(locomotionEvent.Pose.position);
                if (IsHeadFarFromPoint(GetCharacterHead(), _maxWallPenetrationDistance))
                {
                    ResetPlayerToCharacter();
                }
                _whenLocomotionEventHandled.Invoke(locomotionEvent, locomotionEvent.Pose);
            }
            else //other events are stored to be excuted at the end of the frame
            {
                if (locomotionEvent.Translation == LocomotionEvent.TranslationType.Absolute
                    || locomotionEvent.Translation == LocomotionEvent.TranslationType.AbsoluteEyeLevel
                    || locomotionEvent.Translation == LocomotionEvent.TranslationType.Relative)
                {
                    _velocity = Vector3.zero;
                }
                _deferredLocomotionEvent.Enqueue(locomotionEvent);
            }
        }

        private void ConsumeDeferredLocomotionEvents()
        {
            if (_deferredLocomotionEvent.Count == 0)
            {
                return;
            }

            Pose startPose = _capsule.transform.GetPose();
            while (_deferredLocomotionEvent.Count > 0)
            {
                LocomotionEvent locomotionEvent = _deferredLocomotionEvent.Dequeue();
                HandleDeferredLocomotionEvent(locomotionEvent);
            }
            Pose endPose = _capsule.transform.GetPose();
            AccumulateDelta(ref _accumulatedDeltaFrame, startPose, endPose);
        }

        private void HandleDeferredLocomotionEvent(LocomotionEvent locomotionEvent)
        {
            Pose startPose = _capsule.transform.GetPose();

            //Teleport
            if (locomotionEvent.Translation == LocomotionEvent.TranslationType.Absolute)
            {
                MoveAbsoluteFeet(locomotionEvent.Pose.position);
            }
            else if (locomotionEvent.Translation == LocomotionEvent.TranslationType.AbsoluteEyeLevel)
            {
                MoveAbsoluteHead(locomotionEvent.Pose.position);
            }
            else if (locomotionEvent.Translation == LocomotionEvent.TranslationType.Relative)
            {
                MoveRelative(locomotionEvent.Pose.position);
            }

            //Rotation
            if (locomotionEvent.Rotation == LocomotionEvent.RotationType.Absolute)
            {
                RotateAbsolute(locomotionEvent.Pose.rotation);
            }
            else if (locomotionEvent.Rotation == LocomotionEvent.RotationType.Relative)
            {
                RotateRelative(locomotionEvent.Pose.rotation);
            }
            else if (locomotionEvent.Rotation == LocomotionEvent.RotationType.Velocity)
            {
                RotateVelocity(locomotionEvent.Pose.rotation);
            }

            Pose endPose = _capsule.transform.GetPose();
            Pose delta = Pose.identity;
            AccumulateDelta(ref delta, startPose, endPose);
            _whenLocomotionEventHandled.Invoke(locomotionEvent, delta);
        }

        private void AccumulateDelta(ref Pose accumulator, in Pose from, in Pose to)
        {
            accumulator.position = accumulator.position + to.position - from.position;
            accumulator.rotation = (Quaternion.Inverse(from.rotation) * to.rotation) * accumulator.rotation;
        }

        #endregion Locomotion Events Handling

        private void AddVelocity(Vector3 velocity)
        {
            TryExitHotspot(true);
            _velocity += velocity * GetModifiedSpeedFactor();
        }

        private void MoveAbsoluteFeet(Vector3 target)
        {
            TryExitHotspot(true);
            //Move to target
            Vector3 characterFeet = GetCharacterFeet();
            Vector3 offset = target - characterFeet;
            _capsule.transform.position += offset;

            //Ground it
            if (CheckMoveCharacter(Vector3.down * _maxStep, out Vector3 movement))
            {
                _capsule.transform.position += movement;
                UpdateGrounded(true);
            }
            else
            {
                UpdateGrounded(false);
            }
        }

        private void MoveAbsoluteHead(Vector3 target)
        {
            Vector3 characterHead = GetCharacterHead();
            Vector3 offset = target - characterHead;
            _capsule.transform.position += offset;

            _isHeadInHotspot = true;
            _headHotspotCenter = GetCharacterHead();
        }

        private void MoveRelative(Vector3 offset)
        {
            if (_isGrounded)
            {
                TryExitHotspot(true);
                _velocity = Vector3.zero;
                MoveCharacter(offset);
            }
        }

        private void RotateAbsolute(Quaternion target)
        {
            _capsule.transform.rotation = target;
        }

        private void RotateRelative(Quaternion target)
        {
            _capsule.transform.rotation = target * _capsule.transform.rotation;
        }

        private void RotateVelocity(Quaternion target)
        {
            target.ToAngleAxis(out float angle, out Vector3 axis);
            angle *= _deltaTimeProvider.Invoke();
            _capsule.transform.rotation = Quaternion.AngleAxis(angle, axis) * _capsule.transform.rotation;
        }

        /// <summary>
        /// Disables grounding and velocity movements. Preventing the character
        /// from catching up to the player, sliding or falling.
        /// </summary>
        public void DisableMovement()
        {
            _velocityDisabled = true;
        }

        /// <summary>
        /// Re-enables movement if it was disabled. Making sure grounding is working again.
        /// </summary>
        public void EnableMovement()
        {
            if (!IgnoringVelocity)
            {
                return;
            }

            _velocityDisabled = false;
            _isHeadInHotspot = false;
            _headHotspotCenter = null;
            _velocity = Vector3.zero;

            bool grounded = CalculateGround(out RaycastHit groundHit);
            if (grounded && IsFlat(groundHit.normal))
            {
                Vector3 capsulePosition = _capsule.transform.position;
                RaycastHitPlane(groundHit, capsulePosition, Vector3.down, out float enter);
                capsulePosition.y = capsulePosition.y - enter
                    + _capsule.height * 0.5f + _skinWidth;
                _capsule.transform.position = capsulePosition;
            }
        }

        private bool TryExitHotspot(bool force = false)
        {
            if (_isHeadInHotspot && _headHotspotCenter.HasValue
                && (force || IsHeadFarFromPoint(_headHotspotCenter.Value, _exitHotspotDistance)))
            {
                EnableMovement();
                return true;
            }
            return false;
        }
        private void UpdateCharacterHeight()
        {
            float characterHeight = _capsule.height;
            float playerHeight = _heightOffset
                + (_isCrouching ? _crouchHeightOffset : 0f)
                + (ControllingPlayer && _autoUpdateHeight ?
                    GetPlayerHeadTop().y - _playerOrigin.position.y : _defaultHeight);

            float deltaThreshold = _skinWidth;
            float desiredHeight = Mathf.Max(playerHeight, _capsule.radius * 2f);
            float heightDelta = desiredHeight - characterHeight;

            if (heightDelta > deltaThreshold
                && CheckMoveCharacter(Vector3.up * heightDelta, out Vector3 movement))
            {
                heightDelta = Mathf.Max(0f, movement.y - _skinWidth);
            }

            if (Mathf.Abs(heightDelta) <= deltaThreshold)
            {
                return;
            }

            _capsule.height = characterHeight + heightDelta;
            _capsule.transform.position += Vector3.up * heightDelta * 0.5f;
        }

        private void CatchUpCharacterToPlayer()
        {
            if (!ControllingPlayer)
            {
                return;
            }

            Vector3 head = GetPlayerHead();
            Vector3 delta = Vector3.ProjectOnPlane(head - _capsule.transform.position, Vector3.up);
            MoveCharacter(delta);
            Vector3 forward = Vector3.ProjectOnPlane(_playerEyes.forward, Vector3.up);
            _capsule.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }

        private void CatchUpPlayerToCharacter(Pose delta, float feetHeight)
        {
            if (!ControllingPlayer)
            {
                return;
            }

            Pose capsulePose = _capsule.transform.GetPose();
            Vector3 originalHeadPosition = GetPlayerHead();
            _playerOrigin.rotation = delta.rotation * _playerOrigin.rotation;
            _playerOrigin.position = _playerOrigin.position + originalHeadPosition - GetPlayerHead();

            Vector3 xzDelta = Vector3.ProjectOnPlane(delta.position, Vector3.up);
            Vector3 finalPosition = _playerOrigin.position + xzDelta;
            finalPosition.y = feetHeight
                + (_isCrouching ? _crouchHeightOffset : 0f)
                + _heightOffset;
            _playerOrigin.position = finalPosition;
            _capsule.transform.SetPose(capsulePose);

        }

        /// <summary>
        /// Instantly moves the Player to the character position
        /// </summary>
        public void ResetPlayerToCharacter()
        {
            if (!ControllingPlayer)
            {
                return;
            }

            Pose capsulePose = _capsule.transform.GetPose();
            Vector3 characterFeet = GetCharacterFeet();
            Vector3 playerHeadOffset = _playerOrigin.position - GetPlayerHead();
            playerHeadOffset.y = 0f;
            _playerOrigin.position = characterFeet + playerHeadOffset;
            _accumulatedDeltaFrame = Pose.identity;
            _capsule.transform.SetPose(capsulePose);
        }

        private void UpdateVelocity()
        {
            float deltaTime = _deltaTimeProvider.Invoke();

            if (_isGrounded
                && _velocity.y <= 0f)
            {
                _velocity *= 1f / (1f + _groundDamping * deltaTime);
                _velocity.y = 0f;
            }
            else
            {
                float airDamp = 1f / (1f + _airDamping * deltaTime);
                _velocity.x *= airDamp;
                _velocity.z *= airDamp;

                if (_velocity.y > 0f)
                {
                    _velocity.y *= 1f / (1f + _jumpDamping * deltaTime);
                }
                _velocity += Physics.gravity * _gravityFactor * deltaTime;
            }
        }

        private void MoveCharacter(Vector3 delta)
        {
            if (_isGrounded)
            {
                Vector3 flatDelta = Vector3.ProjectOnPlane(delta, Vector3.up);
                Vector3 slopedFlatDelta = Vector3.ProjectOnPlane(flatDelta, _groundHit.normal);
                delta = slopedFlatDelta + Vector3.up * delta.y;
            }

            Vector3 movement = Rebound(delta, _maxReboundSteps);
            _capsule.transform.position += movement;
            UpdateGrounded(delta.y < 0f
                && delta.y < movement.y
                && Mathf.Abs(movement.y) < 0.001f);
        }

        private Vector3 Rebound(Vector3 delta, int bounces)
        {
            Vector3 capsuleHalfSegment = Vector3.up * Mathf.Max(0f, _capsule.height * 0.5f - _capsule.radius);
            Vector3 capsuleTop = _capsule.transform.position + capsuleHalfSegment;
            Vector3 capsuleBase = _capsule.transform.position - capsuleHalfSegment;

            Vector3 originalFlatDelta = Vector3.ProjectOnPlane(delta, Vector3.up);
            return ReboundRecursive(capsuleBase, capsuleTop, _capsule.radius, delta, originalFlatDelta, bounces);

            Vector3 ReboundRecursive(Vector3 capsuleBase, Vector3 capsuleTop, float radius, Vector3 delta, Vector3 originalFlatDelta, int bounceStep)
            {
                if (bounceStep <= 0
                    || Mathf.Approximately(delta.sqrMagnitude, 0f))
                {
                    return Vector3.zero;
                }

                Vector3 accumulatedDelta = Vector3.zero;
                Vector3 originalDelta = delta;
                Vector3 extraDelta = Vector3.zero;

                RaycastHit? moveHit = null;
                RaycastHit? stepHit = null;

                //check if we collided against a wall
                if (MoveCapsuleCollides(capsuleBase, capsuleTop, radius, delta, out moveHit))
                {
                    (delta, extraDelta) = DecomposeDelta(delta, moveHit.Value);
                }

                capsuleBase += delta;
                capsuleTop += delta;
                accumulatedDelta += delta;

                //check when we collided, whether it was a step that we can climb
                if (_isGrounded
                    && moveHit.HasValue
                    //early exit if the hitpoint is already too high
                    && moveHit.Value.point.y - (capsuleBase.y - radius - _skinWidth) <= _maxStep)
                {
                    bool stepClimbed = ClimbStep(capsuleBase, capsuleTop, radius, extraDelta,
                        out Vector3 climbDelta, out stepHit);
                    if (stepClimbed)
                    {
                        if (stepHit.HasValue)
                        {
                            (_, extraDelta) = DecomposeDelta(extraDelta, stepHit.Value);
                            extraDelta = SlideDelta(extraDelta, originalFlatDelta, stepHit.Value);
                        }
                        else
                        {
                            extraDelta = Vector3.zero;
                        }

                        capsuleBase += climbDelta;
                        capsuleTop += climbDelta;
                        accumulatedDelta += climbDelta;
                    }
                }

                if (moveHit.HasValue && !stepHit.HasValue)
                {
                    extraDelta = SlideDelta(extraDelta, originalFlatDelta, moveHit.Value);
                }

                return accumulatedDelta + ReboundRecursive(capsuleBase, capsuleTop, radius, extraDelta, originalFlatDelta, bounceStep - 1);
            }
        }

        private bool ClimbStep(Vector3 capsuleBase, Vector3 capsuleTop, float radius, Vector3 delta, out Vector3 climbDelta, out RaycastHit? stepHit)
        {
            stepHit = null;
            climbDelta = Vector3.zero;

            float baseOffset = Mathf.Min(_maxStep, capsuleTop.y - capsuleBase.y);
            float topOffset = Mathf.Max(0f, _maxStep - baseOffset);
            Vector3 capsuleMaxStepBase = capsuleBase + Vector3.up * baseOffset;
            Vector3 capsuleMaxStepTop = capsuleTop + Vector3.up * topOffset;

            //then move the capsule forward but with the base floating the _maxStep amount
            if (MoveCapsuleCollides(capsuleMaxStepBase, capsuleMaxStepTop, radius, delta, out RaycastHit? hit))
            {
                stepHit = hit;
                //check if we hit one of the caps
                //if so, correct the surface normal in case it is a corner
                Vector3 capsuleDir = capsuleTop - capsuleBase;
                if (Mathf.Approximately(capsuleDir.sqrMagnitude, 0f)
                    || Mathf.Abs(Vector3.Dot(hit.Value.normal, capsuleDir.normalized)) > _cornerHitEpsilon)
                {
                    Vector3 rayDir = -hit.Value.normal;
                    Ray cornerRay = new Ray(hit.Value.point - rayDir * hit.Value.distance, rayDir);
                    if (hit.Value.collider.Raycast(cornerRay, out RaycastHit capHit, hit.Value.distance + _cornerHitEpsilon))
                    {
                        //replace only hit, not stephit, for correcting the delta
                        //but not overriding the capsule normal
                        hit = capHit;
                    }
                }

                (delta, _) = DecomposeDelta(delta, hit.Value);
            }

            //then we try to grow the base back to its original height to check for the ground
            bool groundCollided = CalculateGround(capsuleTop + delta, radius, _capsule.height - radius,
                out RaycastHit stepDownHit);

            if (groundCollided)
            {
                bool hitBase = RaycastSphere(stepDownHit.point, Vector3.up,
                    capsuleMaxStepBase + delta, radius + _skinWidth,
                    out float verticalDistance);

                if (hitBase
                    && stepDownHit.point.y - (capsuleBase.y - radius) <= _maxStep
                    && IsFlat(stepDownHit.normal))
                {
                    delta.y = Mathf.Max(delta.y, baseOffset - verticalDistance);
                    //now that we know the size of the step, we must also check that the capsule
                    //can move up that amount before going forward
                    Vector3 stepUp = Vector3.up * delta.y;
                    if (MoveCapsuleCollides(capsuleBase, capsuleTop, radius, stepUp, out _))
                    {
                        return false;
                    }

                    climbDelta = delta;
                    return true;
                }
            }

            return false;
        }

        private bool CheckMoveCharacter(Vector3 delta, out Vector3 movement)
        {
            Vector3 capsuleHalfSegment = Vector3.up * Mathf.Max(0f, _capsule.height * 0.5f - _capsule.radius);
            Vector3 capsuleTop = _capsule.transform.position + capsuleHalfSegment;
            Vector3 capsuleBase = _capsule.transform.position - capsuleHalfSegment;
            float radius = _capsule.radius;
            if (MoveCapsuleCollides(capsuleBase, capsuleTop, radius, delta, out RaycastHit? hit))
            {
                (delta, _) = DecomposeDelta(delta, hit.Value);
                movement = delta;
                return true;
            }
            movement = Vector3.zero;
            return false;
        }

        private bool MoveCapsuleCollides(Vector3 capsuleBase, Vector3 capsuleTop, float radius, Vector3 delta, out RaycastHit? moveHit)
        {
            float sqrMagnitude = delta.sqrMagnitude;
            if (Mathf.Approximately(sqrMagnitude, 0f))
            {
                moveHit = null;
                return false;
            }

            float magnitude = sqrMagnitude < _skinWidth * _skinWidth ? _skinWidth : Mathf.Sqrt(sqrMagnitude);
            bool collided = Physics.CapsuleCast(capsuleBase, capsuleTop, radius, delta.normalized,
                       out RaycastHit hit, magnitude, _layerMask.value, QueryTriggerInteraction.Ignore);
            moveHit = collided ? hit : null;
            return collided;
        }

        private (Vector3, Vector3) DecomposeDelta(Vector3 delta, RaycastHit hit)
        {
            Vector3 deltaDir = delta.normalized;
            float projectedSkin = Mathf.Max(0f, Vector3.Dot(deltaDir, -hit.normal)) * _skinWidth;
            Vector3 freeDelta = deltaDir * Mathf.Max(0f, hit.distance - projectedSkin);
            Vector3 remainingDelta = delta - freeDelta;
            return (freeDelta, remainingDelta);
        }

        private Vector3 SlideDelta(Vector3 delta, Vector3 originalFlatDelta, RaycastHit hit)
        {
            //if the hit a steep ground slope
            //consider it a wall to avoid climbing attempts when sliding
            Vector3 hitNormal = hit.normal;
            if (!IsFlat(hitNormal))
            {
                hitNormal = Vector3.ProjectOnPlane(hit.normal, Vector3.up).normalized;
            }

            Vector3 flatDelta = Vector3.ProjectOnPlane(delta, Vector3.up);
            flatDelta = Vector3.ProjectOnPlane(flatDelta, hitNormal);
            if (Vector3.Dot(flatDelta, originalFlatDelta) <= 0)
            {
                flatDelta = Vector3.zero;
            }

            Vector3 verticalDelta = Vector3.up * delta.y;
            verticalDelta = Vector3.ProjectOnPlane(verticalDelta, hit.normal);

            return flatDelta + verticalDelta;
        }

        private bool IsFlat(Vector3 groundNormal)
        {
            float angle = Vector3.Angle(Vector3.up, groundNormal);
            return angle <= _maxSlopeAngle;
        }

        private void UpdateGrounded(bool forceGrounded = false)
        {
            _isGrounded = CalculateGround(out _groundHit) && IsFlat(_groundHit.normal);
            if (!_isGrounded && forceGrounded)
            {
                _isGrounded = true;
                _groundHit.normal = Vector3.up;
                _groundHit.point = _capsule.transform.position + Vector3.down * (_capsule.height * 0.5f + _skinWidth);
            }
        }

        private bool CalculateGround(out RaycastHit groundHit)
        {
            //check just the feet
            Vector3 capsuleBase = _capsule.transform.position + Vector3.down * (_capsule.height * 0.5f - _capsule.radius);
            bool feetHit = CalculateGround(capsuleBase,
                _capsule.radius + _skinWidth,
                _capsule.radius + _skinWidth,
                out groundHit);

            if (feetHit)
            {
                return true;
            }

            //check half the body
            bool halfBodyHit = CalculateGround(_capsule.transform.position,
                _capsule.radius + _skinWidth,
                _capsule.height * 0.5f + _skinWidth,
                out groundHit);

            return halfBodyHit;
        }

        private bool CalculateGround(Vector3 origin, float radius, float distance, out RaycastHit groundHit)
        {
            Vector3 downDir = Vector3.down;

            bool rayCollided = Physics.Raycast(
                origin, downDir,
                out RaycastHit rayHit, distance, _layerMask.value, QueryTriggerInteraction.Ignore);

            bool sphereCollided = Physics.SphereCast(origin, radius, downDir,
                out RaycastHit sphereHit, distance - radius,
                 _layerMask.value, QueryTriggerInteraction.Ignore);

            if (sphereCollided)
            {
                bool collided = Physics.Raycast(
                    sphereHit.point - downDir * 0.01f, downDir,
                    out RaycastHit preciseHit, 0.011f, _layerMask.value, QueryTriggerInteraction.Ignore);
                if (collided)
                {
                    sphereHit.normal = preciseHit.normal;
                }
            }

            if (sphereCollided && rayCollided)
            {
                //pick the flattest one
                groundHit = sphereHit.normal.y > rayHit.normal.y ? sphereHit : rayHit;
                groundHit.distance = Vector3.Project(groundHit.point - origin, downDir).magnitude;
                return true;
            }
            else if (sphereCollided || rayCollided)
            {
                groundHit = sphereCollided ? sphereHit : rayHit;
                groundHit.normal = rayCollided ? rayHit.normal : sphereHit.normal;
                groundHit.distance = Vector3.Project(groundHit.point - origin, downDir).magnitude;
                return true;
            }

            groundHit = default;
            return false;
        }

        private void UpdateAnchorPoints()
        {
            if (_logicalHead != null)
            {
                _logicalHead.transform.SetPositionAndRotation(
                    GetCharacterHead(), _capsule.transform.rotation);
            }

            if (_logicalFeet != null)
            {
                _logicalFeet.transform.SetPositionAndRotation(
                    GetCharacterFeet(), _capsule.transform.rotation);
            }
        }

        private float GetModifiedSpeedFactor()
        {
            if (!_isGrounded || _velocity.y > 0f)
            {
                return 0f;
            }

            float speedFactor = _acceleration
                * (_isCrouching ? _crouchSpeedFactor : _isRunning ? _runningSpeedFactor : _speedFactor)
                * _deltaTimeProvider.Invoke();
            return speedFactor;
        }

        private Vector3 GetCharacterFeet()
        {
            Vector3 characterFeet = _capsule.transform.position
                + Vector3.down * (_capsule.height * 0.5f + _skinWidth);
            return characterFeet;
        }

        private Vector3 GetCharacterHead()
        {
            Vector3 characterHead = _capsule.transform.position
                + Vector3.up * (_capsule.height * 0.5f - _sellionToTopOfHead + _skinWidth);
            return characterHead;
        }

        private Vector3 GetPlayerHead()
        {
            return _playerEyes.position - _playerEyes.forward * _sellionToBackOfHeadHalf;
        }

        private Vector3 GetPlayerHeadTop()
        {
            return GetPlayerHead() + Vector3.up * _sellionToTopOfHead;
        }

        private bool IsHeadFarFromPoint(Vector3 point, float maxDistance)
        {
            Vector3 head = ControllingPlayer ? GetPlayerHead() : GetCharacterHead();
            Vector3 vector = Vector3.ProjectOnPlane(head - point, Vector3.up);
            return vector.sqrMagnitude >= maxDistance * maxDistance;
        }

        private IEnumerator EndOfFrameCoroutine()
        {
            while (true)
            {
                yield return _endOfFrame;
                LastUpdate();
            }
        }

        #region Capsule Physics

        private static bool RaycastSphere(Vector3 origin, Vector3 direction, Vector3 sphereCenter, float radius, out float distance)
        {
            distance = float.MaxValue;
            Vector3 os = origin - sphereCenter;
            float a = Vector3.Dot(direction, direction);
            float b = 2.0f * Vector3.Dot(os, direction);
            float c = Vector3.Dot(os, os) - radius * radius;
            float discriminant = b * b - 4.0f * a * c;

            if (discriminant < 0)
            {
                return false;
            }
            else
            {
                distance = (-b - (float)Math.Sqrt(discriminant)) / (2.0f * a);
                return true;
            }
        }

        private bool RaycastHitPlane(RaycastHit hit, Vector3 origin, Vector3 direction, out float enter)
        {
            enter = 0f;
            float pointAlignment = Vector3.Dot(hit.normal, hit.point) - Vector3.Dot(origin, hit.normal);
            float normalAlignment = Vector3.Dot(direction, hit.normal);
            if (!Mathf.Approximately(normalAlignment, 0f))
            {
                enter = pointAlignment / normalAlignment;
                return true;
            }
            return false;
        }

        #endregion Capsule Physics

        #region Inject

        public void InjectAllCapsuleLocomotionHandler(CapsuleCollider capsule)
        {
            InjectCapsule(capsule);
        }

        public void InjectCapsule(CapsuleCollider capsule)
        {
            _capsule = capsule;
        }

        public void InjectOptionalPlayerEyes(Transform playerEyes)
        {
            _playerEyes = playerEyes;
        }

        public void InjectOptionalPlayerOrigin(Transform playerOrigin)
        {
            _playerOrigin = playerOrigin;
        }

        #endregion
    }
}
