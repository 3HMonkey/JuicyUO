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
using JuicyUO.Ultima.World.Maps;
using JuicyUO.Ultima.World.Entities.Mobiles;
#endregion

namespace JuicyUO.Ultima.World.Entities.Items.Containers
{
    class Corpse : ContainerItem
    {
        public Serial MobileSerial = 0;

        public float Frame = 0.999f;
        public Body Body { get { return Amount; } }

        private Direction m_Facing = Direction.Nothing;
        public Direction Facing
        {
            get { return m_Facing & Direction.FacingMask; }
            set { m_Facing = value; }
        }

        public readonly bool DieForwards;

        public Corpse(Serial serial, Map map)
            : base(serial, map)
        {
            Equipment = new MobileEquipment(this);
            DieForwards = Utility.RandomValue(0, 1) == 0;
        }

        protected override AEntityView CreateView()
        {
            return new MobileView(this);
        }

        public override void Update(double frameMS)
        {
            base.Update(frameMS);
            Frame += ((float)frameMS / 500f);
            if (Frame >= 1f)
                Frame = 0.999f;
        }

        public void PlayDeathAnimation()
        {
            Frame = 0f;
        }

        public MobileEquipment Equipment;

        public override void RemoveItem(Serial serial)
        {
            base.RemoveItem(serial);
            Equipment.RemoveBySerial(serial);
            m_OnUpdated?.Invoke(this);
        }
    }
}
