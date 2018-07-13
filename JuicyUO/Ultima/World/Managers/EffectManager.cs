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
using JuicyUO.Ultima.Network.Server;
using JuicyUO.Ultima.World.Entities.Effects;
#endregion

namespace JuicyUO.Ultima.World.Managers
{
    class EffectManager
    {
        WorldModel m_Model;
        readonly List<AEffect> m_Effects;

        public EffectManager(WorldModel model)
        {
            m_Model = model;
            m_Effects = new List<AEffect>();
        }

        public void Add(GraphicEffectPacket packet)
        {
            bool hasHueData = (packet as GraphicEffectHuedPacket != null);
            bool hasParticles = (packet as GraphicEffectExtendedPacket != null); // we don't yet handle these.
            if (hasParticles)
            {
                Tracer.Warn("Unhandled particles in an effects packet.");
            }

            AEffect effect = null;
            int hue = hasHueData ? ((GraphicEffectHuedPacket)packet).Hue : 0;
            int blend = hasHueData ? (int)((GraphicEffectHuedPacket)packet).BlendMode : 0;

            switch (packet.EffectType)
            {
                case GraphicEffectType.Moving:
                    if (packet.ItemID <= 0)
                        return;
                    effect = new MovingEffect(m_Model.Map, packet.SourceSerial, packet.TargetSerial,
                        packet.SourceX, packet.SourceY, packet.SourceZ,
                        packet.TargetX, packet.TargetY, packet.TargetZ, 
                        packet.ItemID, hue);
                    effect.BlendMode = blend;
                    if (packet.DoesExplode)
                    {
                        effect.Children.Add(new AnimatedItemEffect(m_Model.Map, packet.TargetSerial, 
                            packet.TargetX, packet.TargetY, packet.TargetZ,
                            0x36cb, hue, 9));
                    }
                    break;
                case GraphicEffectType.Lightning:
                    effect = new LightningEffect(m_Model.Map, packet.SourceSerial, 
                        packet.SourceX, packet.SourceY, packet.SourceZ, hue);
                    break;
                case GraphicEffectType.FixedXYZ:
                    if (packet.ItemID <= 0)
                        return;
                    effect = new AnimatedItemEffect(m_Model.Map, 
                        packet.SourceX, packet.SourceY, packet.SourceZ,
                        packet.ItemID, hue, packet.Duration);
                    effect.BlendMode = blend;
                    break;
                case GraphicEffectType.FixedFrom:
                    if (packet.ItemID <= 0)
                        return;
                    effect = new AnimatedItemEffect(m_Model.Map, packet.SourceSerial, 
                        packet.SourceX, packet.SourceY, packet.SourceZ,
                        packet.ItemID, hue, packet.Duration);
                    effect.BlendMode = blend;
                    break;
                case GraphicEffectType.ScreenFade:
                    Tracer.Warn("Unhandled screen fade effect.");
                    break;
                default:
                    Tracer.Warn("Unhandled effect.");
                    return;
            }

            if (effect != null)
                Add(effect);
        }

        public void Add(AEffect e)
        {
            m_Effects.Add(e);
        }

        public void Update(double frameMS)
        {
            for (int i = 0; i < m_Effects.Count; i++)
            {
                AEffect effect = m_Effects[i];
                effect.Update(frameMS);
                if (effect.IsDisposed)
                {
                    m_Effects.RemoveAt(i);
                    i--;
                    if (effect.ChildrenCount > 0)
                    {
                        for (int j = 0; j < effect.Children.Count; j++)
                        {
                            m_Effects.Add(effect.Children[j]);
                        }
                    }
                }
            }
        }
    }
}
