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

namespace Meta.Voice.TelemetryUtilities
{
    /// <summary>
    /// Provides a facade to emit telemetry to various targets.
    /// </summary>
    public class RuntimeTelemetry : ITelemetryWriter
    {
        private readonly List<ITelemetryWriter> _writers = new List<ITelemetryWriter>();

        internal RuntimeTelemetry()
        { }

        public static RuntimeTelemetry Instance { get; } = new RuntimeTelemetry();

        public void RegisterWriter(ITelemetryWriter writer)
        {
            _writers.Add(writer);
        }

        public void StartEvent(OperationID operationId, RuntimeTelemetryEventType runtimeTelemetryEventType)
        {
            foreach (var writer in _writers)
            {
                writer.StartEvent(operationId, runtimeTelemetryEventType);
            }
        }

        public void LogEventTermination(OperationID operationId, TerminationReason reason = TerminationReason.Successful,
            string message = "")
        {
            foreach (var writer in _writers)
            {
                writer.LogEventTermination(operationId, reason, message);
            }
        }

        public void LogInstantaneousEvent(OperationID operationId, RuntimeTelemetryEventType runtimeTelemetryEventType, Dictionary<string, string> annotations = null)
        {
            foreach (var writer in _writers)
            {
                writer.LogInstantaneousEvent(operationId, runtimeTelemetryEventType, annotations);
            }
        }

        public void LogPoint(OperationID operationId, RuntimeTelemetryPoint point)
        {
            foreach (var writer in _writers)
            {
                writer.LogPoint(operationId, point);
            }
        }

        public void AnnotateEvent(OperationID operationID, string annotationKey, string annotationValue)
        {
            foreach (var writer in _writers)
            {
                writer.AnnotateEvent(operationID, annotationKey, annotationValue);
            }
        }
    }
}
