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
using JuicyUO.Ultima.World.EntityViews;
using JuicyUO.Ultima.World.Maps;
#endregion

namespace JuicyUO.Ultima.World.Entities.Effects
{
    class LightningEffect : AEffect
    {
        public LightningEffect(Map map, int hue)
            : base (map)
        {
            Hue = hue;
        }

        public LightningEffect(Map map, AEntity Source, int hue)
            : this(map, hue)
        {
            SetSource(Source);
        }

        public LightningEffect(Map map, int xSource, int ySource, int zSource, int hue)
            : this(map, hue)
        {
            SetSource(xSource, ySource, zSource);
        }

        public LightningEffect(Map map, int sourceSerial, int xSource, int ySource, int zSource, int hue)
            : this(map, hue)
        {
            AEntity source = WorldModel.Entities.GetObject<AEntity>(sourceSerial, false);
            if (source != null)
            {
                SetSource(source);
            }
            else
            {
                SetSource(xSource, ySource, zSource);
            }
        }

        public override void Update(double frameMS)
        {
            base.Update(frameMS);
            if (FramesActive >= 10)
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

        public override string ToString()
        {
            return string.Format("LightningEffect");
        }

        protected override AEntityView CreateView()
        {
            return new LightningEffectView(this);
        }
    }
}
