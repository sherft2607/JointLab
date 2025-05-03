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

using System.IO;
using Meta.Voice.Net.WebSockets;
using Meta.WitAi.Configuration;
using Meta.WitAi.Data;
using Meta.WitAi.Data.Configuration;
using Meta.WitAi.Dictation.Events;
using Meta.WitAi.Events;
using Meta.WitAi.Interfaces;
using Meta.WitAi.Requests;
using UnityEngine;

namespace Meta.WitAi.Dictation
{
    public class WitDictation : DictationService, IWitRuntimeConfigProvider, IVoiceEventProvider, IVoiceServiceRequestProvider, IWitConfigurationProvider
    {
        [SerializeField] private WitRuntimeConfiguration witRuntimeConfiguration;

        private WitService witService;

        public WitRuntimeConfiguration RuntimeConfiguration
        {
            get => witRuntimeConfiguration;
            set => witRuntimeConfiguration = value;
        }
        public WitConfiguration Configuration => RuntimeConfiguration?.witConfiguration;

        #region Voice Service Properties

        public override bool Active => null != witService && witService.Active;
        public override bool IsRequestActive => null != witService && witService.IsRequestActive;

        public override ITranscriptionProvider TranscriptionProvider
        {
            get => witService.TranscriptionProvider;
            set => witService.TranscriptionProvider = value;

        }

        public override bool MicActive => null != witService && witService.MicActive;

        protected override bool ShouldSendMicData => witRuntimeConfiguration.sendAudioToWit ||
                                                     null == TranscriptionProvider;


        /// <summary>
        /// Events specific to wit voice activation.
        /// </summary>
        public VoiceEvents VoiceEvents => _voiceEvents;
        private readonly VoiceEvents _voiceEvents = new VoiceEvents();

        public override DictationEvents DictationEvents
        {
            get => dictationEvents;
            set
            {
                DictationEvents oldEvents = dictationEvents;
                dictationEvents = value;
                if (gameObject.activeSelf)
                {
                    VoiceEvents.RemoveListener(oldEvents);
                    VoiceEvents.AddListener(dictationEvents);
                }
            }
        }

        #endregion

        #region IVoiceServiceRequestProvider
        /// <summary>
        /// Create request using configuration, request options & events
        /// </summary>
        public VoiceServiceRequest CreateRequest(WitRuntimeConfiguration requestSettings, WitRequestOptions requestOptions, VoiceServiceRequestEvents requestEvents)
        {
            var config = requestSettings?.witConfiguration;
            if (config != null && config.RequestType == WitRequestType.WebSocket)
            {
                return WitSocketRequest.GetDictationRequest(config, GetComponent<WitWebSocketAdapter>(), AudioBuffer.Instance, requestOptions, requestEvents);
            }
            return config.CreateDictationRequest(requestOptions, requestEvents);
        }
        #endregion

        #region Voice Service Methods
        /// <summary>
        /// Activates and waits for the user to exceed the min wake threshold before data is sent to the server.
        /// </summary>
        /// <param name="requestOptions">Additional options such as custom request id</param>
        /// <param name="requestEvents">Events specific to the request's lifecycle</param>
        public override VoiceServiceRequest Activate(WitRequestOptions requestOptions, VoiceServiceRequestEvents requestEvents)
        {
            SetupRequestParameters(ref requestOptions, ref requestEvents);
            return witService.Activate(requestOptions, requestEvents);
        }

        /// <summary>
        /// Activates immediately and starts sending data to the server. This will not wait for min wake threshold
        /// </summary>
        /// <param name="requestOptions">Additional options such as custom request id</param>
        /// <param name="requestEvents">Events specific to the request's lifecycle</param>
        public override VoiceServiceRequest ActivateImmediately(WitRequestOptions requestOptions, VoiceServiceRequestEvents requestEvents)
        {
            SetupRequestParameters(ref requestOptions, ref requestEvents);
            return witService.ActivateImmediately(requestOptions, requestEvents);
        }

        /// <summary>
        /// Deactivates. If a transcription is in progress the network request will complete and any additional
        /// transcription values will be returned.
        /// </summary>
        public override void Deactivate()
        {
            witService.Deactivate();
        }

        /// <summary>
        /// Deactivates and ignores any pending transcription content.
        /// </summary>
        public override void Cancel()
        {
            witService.DeactivateAndAbortRequest();
        }
        #endregion

        protected override void Awake()
        {
            base.Awake();
            witService = gameObject.AddComponent<WitService>();
            witService.VoiceEventProvider = this;
            witService.ConfigurationProvider = this;
            witService.RequestProvider = this;
            witService.TelemetryEventsProvider = this;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            VoiceEvents.AddListener(DictationEvents);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            VoiceEvents.RemoveListener(DictationEvents);
        }

        public void TranscribeFile(string fileName)
        {
            var request = CreateRequest(witRuntimeConfiguration, new WitRequestOptions(), new VoiceServiceRequestEvents());
            if (request is WitRequest wr)
            {
                var data = File.ReadAllBytes(fileName);
                wr.postData = data;
                witService.ExecuteRequest(wr);
            }
        }
    }
}
