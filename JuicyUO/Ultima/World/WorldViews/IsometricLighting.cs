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

using System;
using Microsoft.Xna.Framework;

namespace JuicyUO.Ultima.World.WorldViews
{
    public class IsometricLighting
    {
        private int m_LightLevelPersonal = 9;
        private int m_LightLevelOverall = 9;
        private float m_LightDirection = 4.12f;
        private float m_LightHeight = -0.75f;

        public IsometricLighting()
        {
            RecalculateLightningValues();
        }

        public int PersonalLightning
        {
            set { m_LightLevelPersonal = value; RecalculateLightningValues(); }
            get { return m_LightLevelPersonal; }
        }

        public int OverallLightning
        {
            set { m_LightLevelOverall = value; RecalculateLightningValues(); }
            get { return m_LightLevelOverall; }
        }

        public float LightDirection
        {
            set { m_LightDirection = value; RecalculateLightningValues(); }
            get { return m_LightDirection; }
        }

        public float LightHeight
        {
            set { m_LightHeight = value; RecalculateLightningValues(); }
            get { return m_LightHeight; }
        }

        private void RecalculateLightningValues()
        {
            float light = Math.Min(30 - OverallLightning + PersonalLightning, 30f);
            light = Math.Max(light, 0);
            IsometricLightLevel = light / 30; // bring it between 0-1

            // i'd use a fixed lightning direction for now - maybe enable this effect with a custom packet?
            m_LightDirection = 1.2f;
            IsometricLightDirection = Vector3.Normalize(new Vector3((float)Math.Cos(m_LightDirection), (float)Math.Sin(m_LightDirection), 1f));
        }

        public float IsometricLightLevel
        {
            get;
            private set;
        }

        public Vector3 IsometricLightDirection
        {
            get;
            private set;
        }
    }
}
