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
using System;
using Oculus.Interaction.Input;

namespace Oculus.Interaction
{
    public class HandDebugGizmos : SkeletonDebugGizmos, IHandVisual
    {
        [Tooltip("The IHand that will drive the visuals.")]
        [SerializeField, Interface(typeof(IHand))]
        private UnityEngine.Object _hand;
        public IHand Hand { get; private set; }
        public bool ForceOffVisibility { get; set; }
        public bool IsVisible => _isVisible;

        public event Action WhenHandVisualUpdated = delegate { };

        private bool _isVisible = false;
        protected bool _started = false;

        protected virtual void Awake()
        {
            Hand = _hand as IHand;
        }

        protected virtual void Start()
        {
            this.BeginStart(ref _started);
            this.AssertField(Hand, nameof(Hand));
            this.EndStart(ref _started);
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
                Hand.WhenHandUpdated += HandleHandUpdated;
            }
        }

        protected virtual void OnDisable()
        {
            if (_started)
            {
                Hand.WhenHandUpdated -= HandleHandUpdated;
            }
        }

        public Pose GetJointPose(HandJointId jointId, Space space)
        {
            if (space == Space.Self)
            {
                if (Hand.GetJointPoseLocal(jointId, out Pose pose))
                {
                    return pose;
                }
            }
            else if (space == Space.World)
            {
                if (Hand.GetJointPose(jointId, out Pose pose))
                {
                    return pose;
                }
            }
            return new Pose();
        }

        private void HandleHandUpdated()
        {
            _isVisible = Hand.IsTrackedDataValid && !ForceOffVisibility;
            if (_isVisible)
            {
                for (var i = HandJointId.HandStart; i < HandJointId.HandEnd; ++i)
                {
                    Draw((int)i, Visibility);
                }
            }
            WhenHandVisualUpdated.Invoke();
        }

        protected override bool TryGetParentJointId(int jointId, out int parent)
        {
            if (jointId >= HandJointUtils.JointParentList.Length)
            {
                parent = (int)HandJointId.Invalid;
                return false;
            }
            parent = (int)HandJointUtils.JointParentList[jointId];
            return parent > (int)HandJointId.Invalid;
        }

        protected override bool TryGetWorldJointPose(int jointId, out Pose pose)
        {
            return Hand.GetJointPose((HandJointId)jointId, out pose);
        }

        #region Inject

        public void InjectAllHandDebugGizmos(IHand hand)
        {
            InjectHand(hand);
        }

        public void InjectHand(IHand hand)
        {
            _hand = hand as UnityEngine.Object;
            Hand = hand;
        }

        #endregion
    }

    [Obsolete("Use HandDebugGizmos instead.")]
    public class HandDebugVisual : HandDebugGizmos
    {
        [Obsolete("This method has been deprecated.", true)]
        public void UpdateSkeleton() => throw new System.NotImplementedException();
    }
}
