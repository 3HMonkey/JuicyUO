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
using JuicyUO.Ultima.World.Entities.Items;
using JuicyUO.Ultima.World.Entities.Mobiles;
using JuicyUO.Ultima.World.EntityViews;
using JuicyUO.Ultima.World.Maps;
#endregion

namespace JuicyUO.Ultima.World.Entities.Effects
{
    public class AnimatedItemEffect : AEffect
    {
        public int ItemID;
        public int Duration;

        public AnimatedItemEffect(Map map, int itemID, int hue, int duration)
            : base(map)
        {
            ItemID = (itemID & 0x3fff);
            Hue = hue;
            Duration = duration;
        }

        public AnimatedItemEffect(Map map, AEntity Source, int ItemID, int Hue, int duration)
            : this(map, ItemID, Hue, duration)
        {
            SetSource(Source);
        }

        public AnimatedItemEffect(Map map, int Source, int ItemID, int Hue, int duration)
            : this(map, Source, 0, 0, 0, ItemID, Hue, duration)
        {

        }

        public AnimatedItemEffect(Map map, int xSource, int ySource, int zSource, int ItemID, int Hue, int duration)
            : this(map, ItemID, Hue, duration)
        {
            SetSource(xSource, ySource, zSource);
        }

        public AnimatedItemEffect(Map map, int sourceSerial, int xSource, int ySource, int zSource, int ItemID, int Hue, int duration)
            : this(map, ItemID, Hue, duration)
        {
            sbyte zSrcByte = (sbyte)zSource;

            AEntity source = WorldModel.Entities.GetObject<AEntity>(sourceSerial, false);
            if (source != null)
            {
                if (source is Mobile)
                {
                    Mobile mobile = source as Mobile;
                    if ((!mobile.IsClientEntity && !mobile.IsMoving) && (((xSource != 0) || (ySource != 0)) || (zSource != 0)))
                    {
                        mobile.Position.Set(xSource, ySource, zSrcByte);
                    }
                    SetSource(mobile);
                }
                else if (source is Item)
                {
                    Item item = source as Item;
                    if (((xSource != 0) || (ySource != 0)) || (zSource != 0))
                    {
                        item.Position.Set(xSource, ySource, zSrcByte);
                    }
                    SetSource(item);
                }
                else
                {
                    SetSource(xSource, ySource, zSource);
                }
            }
            else
            {
                SetSource(xSource, ySource, zSource);
            }
        }

        public override void Update(double frameMS)
        {
            base.Update(frameMS);
            if (FramesActive >= Duration)
            {
                Dispose();
            }
            else
            {
                int x, y, z;
                GetSource(out x, out y, out z);
                Position.Set(x, y, z);
            }
        }

        protected override AEntityView CreateView()
        {
            return new AnimatedItemEffectView(this);
        }

        public override string ToString()
        {
            return string.Format("AnimatedItemEffect");
        }
    }
}
