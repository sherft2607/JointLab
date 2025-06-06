/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * This source code is licensed under the license found in the
 * LICENSE file in the root directory of this source tree.
 */

using Meta.WitAi.TTS.Data;

namespace Meta.WitAi.TTS.Interfaces
{
    public interface ITTSVoiceProvider
    {
        /// <summary>
        /// Returns preset voice data if no voice data is selected.
        /// Useful for menu ai, etc.
        /// </summary>
        TTSVoiceSettings VoiceDefaultSettings { get; }

        /// <summary>
        /// Returns all preset voice settings
        /// </summary>
        TTSVoiceSettings[] PresetVoiceSettings { get; }
    }
}
