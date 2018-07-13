#region license
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

#region usings
using System.Collections.Generic;
using JuicyUO.Core.Diagnostics.Tracing;
using JuicyUO.Ultima.Resources;
using JuicyUO.Core.Audio;
#endregion

namespace JuicyUO.Ultima.Audio
{
    public class AudioService
    {
        private readonly Dictionary<int, ASound> m_Sounds = new Dictionary<int, ASound>();
        private readonly Dictionary<int, ASound> m_Music = new Dictionary<int, ASound>();
        private UOMusic m_MusicCurrentlyPlaying;

        public void Update()
        {
            if (m_MusicCurrentlyPlaying != null)
                m_MusicCurrentlyPlaying.Update();
        }

        public void PlaySound(int soundIndex, AudioEffects effect = AudioEffects.None, float volume = 1.0f, bool spamCheck = false)
        {
            if (volume < 0.01f)
                return;
            if (Settings.Audio.SoundOn)
            {
                ASound sound;
                if (m_Sounds.TryGetValue(soundIndex, out sound))
                {
                    sound.Play(true, effect, volume, spamCheck);
                }
                else
                {
                    string name;
                    byte[] data;
                    if (SoundData.TryGetSoundData(soundIndex, out data, out name))
                    {
                        sound = new UOSound(name, data);
                        m_Sounds.Add(soundIndex, sound);
                        sound.Play(true, effect, volume, spamCheck);
                    }
                }
            }
        }

        public void PlayMusic(int id)
        {
            if (Settings.Audio.MusicOn)
            {
                if (id < 0) // not a valid id, used to stop music.
                {
                    StopMusic();
                    return;
                }

                if (!m_Music.ContainsKey(id))
                {
                    string name;
                    bool loops;
                    if (MusicData.TryGetMusicData(id, out name, out loops))
                    {
                        m_Music.Add(id, new UOMusic(id, name, loops));
                    }
                    else
                    {
                        Tracer.Error("Received unknown music id {0}", id);
                        return;
                    }
                }

                ASound toPlay = m_Music[id];
                if (toPlay != m_MusicCurrentlyPlaying)
                {
                    // stop the current song
                    StopMusic();
                    // play the new song!
                    m_MusicCurrentlyPlaying = toPlay as UOMusic;
                    m_MusicCurrentlyPlaying.Play(false);
                }
            }
        }

        public void StopMusic()
        {
            if (m_MusicCurrentlyPlaying != null)
            {
                m_MusicCurrentlyPlaying.Stop();
                m_MusicCurrentlyPlaying.Dispose();
                m_MusicCurrentlyPlaying = null;
            }
        }
    }
}
