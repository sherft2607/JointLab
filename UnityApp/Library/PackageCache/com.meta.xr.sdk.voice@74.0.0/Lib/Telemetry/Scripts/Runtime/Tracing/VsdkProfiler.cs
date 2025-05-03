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

namespace Meta.Voice.TelemetryUtilities.PerformanceTracing
{
    /// <summary>
    /// Voice SDK profiler abstraction layer providing common sampling methods that can be rerouted to your profiler
    /// of choice.
    /// </summary>
    public static class VsdkProfiler
    {
        /// <summary>
        /// The active trace provider that will receive begin/end sample requests from VSDK logging and VsdkProfiler use
        /// </summary>
        public static ITraceProvider traceProvider = new UnityProfilerTraceProvider();

        /// <summary>
        /// Determines if profiling is enabled or not. In debug mode it will be on by default. Otherwise
        /// it will need to be turned on manually by the consuming application.
        /// </summary>
#if DEBUG
        public static bool profilingEnabled = true;
#else
        public static bool profilingEnabled = false;
#endif

        /// <summary>
        /// Begin sampling/tracking a profiler trace
        /// </summary>
        /// <param name="sampleName">The name/keyword of the trace being tracked</param>
        public static void BeginSample(string sampleName)
        {
            if (profilingEnabled)
            {
                traceProvider.BeginSample(sampleName);
            }
        }

        /// <summary>
        /// End an active trace
        /// </summary>
        /// <param name="sampleName">The name/keyword of the trace being tracked</param>
        public static void EndSample(string sampleName)
        {
            if (profilingEnabled)
            {
                traceProvider.EndSample(sampleName);
            }
        }
    }
}
