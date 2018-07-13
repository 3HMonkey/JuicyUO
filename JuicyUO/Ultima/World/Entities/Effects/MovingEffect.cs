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

#region usings
using System;
using Microsoft.Xna.Framework;
using JuicyUO.Ultima.World.Entities.Items;
using JuicyUO.Ultima.World.Entities.Mobiles;
using JuicyUO.Ultima.World.EntityViews;
using JuicyUO.Ultima.World.Maps;
#endregion

namespace JuicyUO.Ultima.World.Entities.Effects
{
    public class MovingEffect : AEffect
    {
        public float AngleToTarget;

        int m_ItemID;
        public int ItemID
        {
            get { return m_ItemID; }
        }

        public MovingEffect(Map map,int itemID, int hue)
            : base(map)
        {
            Hue = hue;
            itemID &= 0x3fff;
            m_ItemID = itemID | 0x4000;
        }

        #region Constructors
        public MovingEffect(Map map,AEntity Source, AEntity Target, int itemID, int hue)
            : this(map, itemID, hue)
        {
            base.SetSource(Source);
            base.SetTarget(Target);
        }

        public MovingEffect(Map map,AEntity Source, int xTarget, int yTarget, int zTarget, int itemID, int hue)
            : this(map, itemID, hue)
        {
            base.SetSource(Source);
            base.SetTarget(xTarget, yTarget, zTarget);
        }

        public MovingEffect(Map map,int xSource, int ySource, int zSource, AEntity Target, int itemID, int hue)
            : this(map, itemID, hue)
        {
            base.SetSource(xSource, ySource, zSource);
            base.SetTarget(Target);
        }

        public MovingEffect(Map map,int xSource, int ySource, int zSource, int xTarget, int yTarget, int zTarget, int itemID, int hue)
            : this(map, itemID, hue)
        {
            base.SetSource(xSource, ySource, zSource);
            base.SetTarget(xTarget, yTarget, zTarget);
        }

        public MovingEffect(Map map,int sourceSerial, int targetSerial, int xSource, int ySource, int zSource, int xTarget, int yTarget, int zTarget, int itemID, int hue)
            : this(map, itemID, hue)
        {
            sbyte zSrcByte = (sbyte)zSource;
            sbyte zTarByte = (sbyte)zTarget;

            AEntity source = WorldModel.Entities.GetObject<AEntity>(sourceSerial, false);
            if (source != null)
            {
                if (source is Mobile)
                {
                    base.SetSource(source.X, source.Y, source.Z);
                    Mobile mobile = source as Mobile;
                    if ((!mobile.IsClientEntity && !mobile.IsMoving) && ((xSource | ySource | zSource) != 0))
                    {
                        source.Position.Set(xSource, ySource, zSrcByte);
                    }
                }
                else if (source is Item)
                {
                    base.SetSource(source.X, source.Y, source.Z);
                    Item item = source as Item;
                    if ((xSource | ySource | zSource) != 0)
                    {
                        item.Position.Set(xSource, ySource, zSrcByte);
                    }
                }
                else
                {
                    base.SetSource(xSource, ySource, zSrcByte);
                }
            }
            else
            {
                base.SetSource(xSource, ySource, zSource);
            }
            AEntity target = WorldModel.Entities.GetObject<AEntity>(targetSerial, false);
            if (target != null)
            {
                if (target is Mobile)
                {
                    base.SetTarget(target);
                    Mobile mobile = target as Mobile;
                    if ((!mobile.IsClientEntity && !mobile.IsMoving) && ((xTarget | yTarget | zTarget) != 0))
                    {
                        mobile.Position.Set(xTarget, yTarget, zTarByte);
                    }
                }
                else if (target is Item)
                {
                    base.SetTarget(target);
                    Item item = target as Item;
                    if ((xTarget | yTarget | zTarget) != 0)
                    {
                        item.Position.Set(xTarget, yTarget, zTarByte);
                    }
                }
                else
                {
                    base.SetSource(xTarget, yTarget, zTarByte);
                }
            }
        }
        #endregion

        public override void Update(double frameMS)
        {
            base.Update(frameMS);

            int sx, sy, sz, tx, ty, tz;
            GetSource(out sx, out sy, out sz);
            GetTarget(out tx, out ty, out tz);

            if (m_TimeUntilHit == 0f)
            {
                m_TimeActive = 0f;
                m_TimeUntilHit = (float)Math.Sqrt(Math.Pow((tx - sx), 2) + Math.Pow((ty - sy), 2) + Math.Pow((tz - sz), 2)) * 75f;
            }
            else
            {
                m_TimeActive += (float)frameMS;
            }

            if (m_TimeActive >= m_TimeUntilHit)
            {
                Dispose();
                return;
            }
            else
            {
                float x, y, z;
                x = (sx + (m_TimeActive / m_TimeUntilHit) * (float)(tx - sx));
                y = (sy + (m_TimeActive / m_TimeUntilHit) * (float)(ty - sy));
                z = (sz + (m_TimeActive / m_TimeUntilHit) * (float)(tz - sz));
                Position.Set((int)x, (int)y, (int)z);
                Position.Offset = new Vector3(x % 1, y % 1, z % 1);
                AngleToTarget = -((float)Math.Atan2((ty - sy), (tx - sx)) + (float)(Math.PI) * (1f / 4f)); // In radians
            }

            // m_RenderMode:
            // 2: Alpha = 1.0, Additive.
            // 3: Alpha = 1.5, Additive.
            // 4: Alpha = 0.5, AlphaBlend.

            // draw rotated.
            
        }

        private float m_TimeActive;
        private float m_TimeUntilHit;

        protected override AEntityView CreateView()
        {
            return new MovingEffectView(this);
        }

        public override string ToString()
        {
            return string.Format("MovingEffect");
        }
    }
}
