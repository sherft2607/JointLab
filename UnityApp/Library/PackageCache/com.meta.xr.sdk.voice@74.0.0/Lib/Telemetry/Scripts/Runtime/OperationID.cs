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

namespace Meta.Voice.TelemetryUtilities
{
    /// <summary>
    /// An ID that identifies an operation. This could be a request ID or any custom ID.
    /// </summary>
    public readonly struct OperationID
    {
        private string Value { get; }

        private OperationID(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Returns true if the Operation ID has a value.
        /// </summary>
        public bool IsAssigned => Value != null;

        public override string ToString() => Value;

        public static implicit operator string(OperationID correlationId) => correlationId.Value;
        public static explicit operator OperationID(string value) => new OperationID(value);
        public static implicit operator OperationID(Guid value) => new OperationID(value.ToString());

        public override bool Equals(object obj) => obj is OperationID other && Value == other.Value;
        public override int GetHashCode() => Value.GetHashCode();
    }
}
