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
using JuicyUO.Ultima.Resources;
using JuicyUO.Ultima.World.EntityViews;
using JuicyUO.Ultima.World.Maps;
#endregion

namespace JuicyUO.Ultima.World.Entities
{
    public class Ground : AEntity
    {
        // !!! Don't forget to update surrounding Z values - code is in UpdateSurroundingsIfNecessary(map)

        private int m_LandDataID;
        public int LandDataID
        {
            get { return m_LandDataID; }
        }

        public LandData LandData;

        public bool IsIgnored
        {
            get
            {
                return 
                    m_LandDataID == 2 ||
                    m_LandDataID == 0x1DB ||
                    (m_LandDataID >= 0x1AE && m_LandDataID <= 0x1B5);
            }
        }

        public Ground(int tileID, Map map)
            : base(Serial.Null, map)
        {
            m_LandDataID = tileID;
            LandData = TileData.LandData[m_LandDataID & 0x3FFF];
        }

        protected override AEntityView CreateView()
        {
            return new GroundView(this);
        }
    }
}
