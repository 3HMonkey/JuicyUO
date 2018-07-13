﻿#region license
//  Copyright (C) 2018 JuicyUO Development Community on Github
//
//	This project is an alternative client for the game Ultima Online.
//	The goal of this is to develop a lightweight client considering 
//	new technologies such as DirectX (MonoGame included). The foundation
//	is originally licensed (GNU) on JuicyUO and the JuicyUO Development
//	Team. (Copyright (c) 2015 JuicyUO Development Team)
//    
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <https://www.gnu.org/licenses/>.
#endregion

using System.Collections.Generic;
using JuicyUO.Ultima.Audio;

namespace JuicyUO.Ultima.World.Entities.Mobiles
{
    public static class MobileSounds
    {
        private static AudioService m_Audio = Service.Get<AudioService>();

        private static Dictionary<Serial, MobileSoundData> m_Data = new Dictionary<Serial,MobileSoundData>();

        private static int[] m_StepSFX = { 0x12B, 0x12C };
        private static int[] m_StepMountedSFX = { 0x129, 0x12A };

        public static void ResetFootstepSounds(Mobile mobile)
        {
            if (m_Data.ContainsKey(mobile.Serial))
            {
                m_Data[mobile.Serial].LastFrame = 1.0f;
            }
        }

        public static void DoFootstepSounds(Mobile mobile, double frame)
        {
            if (!mobile.Body.IsHumanoid || mobile.Flags.IsHidden)
                return;

            MobileSoundData data;
            if (!m_Data.TryGetValue(mobile.Serial, out data))
            {
                data = new MobileSoundData(mobile);
                m_Data.Add(mobile.Serial, data);
            }

            bool play = (data.LastFrame < 0.5d && frame >= 0.5d) || (data.LastFrame > 0.5d && frame < 0.5d);
            if (mobile.IsMounted && !mobile.IsRunning && frame > 0.5d)
                play = false;

            if (play)
            {
                float volume = 1f;
                int distanceFromPlayer = Utility.DistanceBetweenTwoPoints(mobile.DestinationPosition.Tile, WorldModel.Entities.GetPlayerEntity().DestinationPosition.Tile);
                if (distanceFromPlayer > 4)
                    volume = 1f - (distanceFromPlayer - 4) * 0.05f;


                if (mobile.IsMounted && mobile.IsRunning)
                {
                    int sfx = Utility.RandomValue(0, m_StepMountedSFX.Length - 1);
                    m_Audio.PlaySound(m_StepMountedSFX[sfx], Core.Audio.AudioEffects.PitchVariation, volume);
                }
                else
                {
                    int sfx = Utility.RandomValue(0, m_StepSFX.Length - 1);
                    m_Audio.PlaySound(m_StepSFX[sfx], Core.Audio.AudioEffects.PitchVariation, volume);
                }
            }
            data.LastFrame = frame;
        }

        private class MobileSoundData
        {
            public Mobile Mobile
            {
                get;
                private set;
            }

            public double LastFrame = 1d;

            public MobileSoundData(Mobile mobile)
            {
                Mobile = mobile;
            }
        }
    }
}
