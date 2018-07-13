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
using JuicyUO.Ultima.World.Maps;
#endregion

namespace JuicyUO.Ultima.World.Entities.Effects
{
    public abstract class AEffect : AEntity
    {
        private List<AEffect> m_Children;
        public List<AEffect> Children
        {
            get
            {
                if (m_Children == null)
                    m_Children = new List<AEffect>();
                return m_Children;
            }
        }

        public int ChildrenCount
        {
            get
            {
                return (m_Children == null) ? 0 : m_Children.Count;
            }
        }

        protected AEntity m_Source;
        protected AEntity m_Target;

        protected int m_xSource, m_ySource, m_zSource;
        protected int m_xTarget, m_yTarget, m_zTarget;

        private double m_TimeActiveMS;
        public int FramesActive
        {
            get
            {
                int frameOffset = (int)(m_TimeActiveMS / 50d); // one frame every 20ms
                return frameOffset;
            }
        }

        public int BlendMode;

        public AEffect(Map map)
            : base(Serial.Null, map)
        {

        }

        public override void Update(double frameMS)
        {
            base.Update(frameMS);
            if (!IsDisposed)
            {
                m_TimeActiveMS += frameMS;
            }
        }

        public override void Dispose()
        {
            m_Source = null;
            m_Target = null;
            base.Dispose();
        }

        /// <summary>
        /// Adds an effect which will display when the parent effect despawns.
        /// </summary>
        /// <param name="effect"></param>
        public void AddChildEffect(AEffect effect)
        {
            if (m_Children == null)
                m_Children = new List<AEffect>();
            m_Children.Add(effect);
        }

        /// <summary>
        /// Returns the world position that is the source of the effect.
        /// </summary>
        protected void GetSource(out int x, out int y, out int z)
        {
            if (m_Source == null)
            {
                x = m_xSource;
                y = m_ySource;
                z = m_zSource;
            }
            else
            {
                x = m_Source.X;
                y = m_Source.Y;
                z = m_Source.Z;
            }
        }

        /// <summary>
        /// Returns the world position that is the target of the effect.
        /// </summary>
        protected void GetTarget(out int X, out int Y, out int Z)
        {
            if (m_Target == null)
            {
                X = m_xTarget;
                Y = m_yTarget;
                Z = m_zTarget;
            }
            else
            {
                X = m_Target.X;
                Y = m_Target.Y;
                Z = m_Target.Z;
            }
        }

        public void SetSource(AEntity source)
        {
            m_Source = source;
        }

        public void SetSource(int x, int y, int z)
        {
            m_Source = null;
            m_xSource = x;
            m_ySource = y;
            m_zSource = z;
        }

        public void SetTarget(AEntity target)
        {
            m_Target = target;
        }

        public void SetTarget(int x, int y, int z)
        {
            m_Target = null;
            m_xTarget = x;
            m_yTarget = y;
            m_zTarget = z;
        }
    }
}
