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
using Meta.WitAi.Json;
using Meta.WitAi.TTS.Data;

namespace Meta.WitAi.Composer.Data
{
    [Serializable]
    public class ComposerResponseData
    {
        /// <summary>
        /// Whether this response expects additional user input
        /// </summary>
        public bool expectsInput;

        /// <summary>
        /// The action id to be called automatically if desired
        /// </summary>
        public string actionID;

        /// <summary>
        /// Whether the response is from 'response' or 'partial_response'
        /// </summary>
        public bool responseIsFinal;

        /// <summary>
        /// Response text to be displayed for accessibility or visual purposes.
        /// </summary>
        public string responsePhrase;

        /// <summary>
        /// Response phrase returned from the composer for tts purposes
        /// </summary>
        public string responseTts;

        /// <summary>
        /// The voice settings to be used if desired
        /// </summary>
        public TTSVoiceSettings responseTtsSettings;

        /// <summary>
        /// The request ID for which this response was generated.
        /// </summary>
        public string requestId;

        /// <summary>
        /// Response for any errors
        /// </summary>
        public string error;

        /// <summary>
        /// The raw wit response
        /// </summary>
        [NonSerialized] public WitResponseNode witResponse;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ComposerResponseData() {}

        /// <summary>
        /// Error constructor
        /// </summary>
        public ComposerResponseData(string newError)
        {
            error = newError;
        }
    }
}
