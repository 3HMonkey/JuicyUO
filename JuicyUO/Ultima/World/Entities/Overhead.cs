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
using JuicyUO.Ultima.Data;
using JuicyUO.Ultima.World.EntityViews;
#endregion

namespace JuicyUO.Ultima.World.Entities
{
    public class Overhead : AEntity
    {
        public AEntity Parent
        {
            get;
            private set;
        }

        public MessageTypes MessageType
        {
            get;
            private set;
        }

        public string Text
        {
            get;
            private set;
        }

        private int m_TimePersist;

        public Overhead(AEntity parent, MessageTypes msgType, string text)
            : base(parent.Serial, parent.Map)
        {
            Parent = parent;
            MessageType = msgType;
            Text = text;

            string plainText = text.Substring(text.IndexOf('>') + 1);
           
            // Every speech message lasts at least 2.5s, and increases by 100ms for every char, to a max of 10s
            m_TimePersist = 2500 + (plainText.Length * 100);
            if (m_TimePersist > 10000)
                m_TimePersist = 10000;
        }

        public void ResetTimer()
        {
            m_TimePersist = 5000;
        }

        public override void Update(double frameMS)
        {
            base.Update(frameMS);
            m_TimePersist -= (int)frameMS;
            if (m_TimePersist <= 0)
                Dispose();
        }

        // ============================================================================================================
        // View management
        // ============================================================================================================

        protected override AEntityView CreateView()
        {
            return new OverheadView(this);
        }
    }
}
