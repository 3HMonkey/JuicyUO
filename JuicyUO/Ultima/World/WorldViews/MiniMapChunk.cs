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

using JuicyUO.Ultima.Resources;
using JuicyUO.Ultima.World.Entities;
using JuicyUO.Ultima.World.Entities.Items;
using JuicyUO.Ultima.World.Maps;

namespace JuicyUO.Ultima.World.WorldViews
{
    class MiniMapChunk
    {
        public uint X, Y;

        public uint[] Colors;
        private static sbyte[] m_Zs = new sbyte[64]; // shared between all instances of MiniMapChunk.

        public MiniMapChunk(uint x, uint y, TileMatrixData tileData)
        {
            X = x;
            Y = y;
            Colors = new uint[64];

            // get data from the tile Matrix
            byte[] groundData = tileData.GetLandChunk(x, y);
            int staticLength;
            byte[] staticsData = tileData.GetStaticChunk(x, y, out staticLength);

            // get the ground colors
            int groundDataIndex = 0;
            for (int i = 0; i < 64; i++)
            {
                Colors[i] = RadarColorData.Colors[groundData[groundDataIndex++] + (groundData[groundDataIndex++] << 8)];
                m_Zs[i]= (sbyte)groundData[groundDataIndex++];
            }

            // get the static colors
            int countStatics = staticLength / 7;
            int staticDataIndex = 0;
            for (int i = 0; i < countStatics; i++)
            {
                int itemID = staticsData[staticDataIndex++] + (staticsData[staticDataIndex++] << 8);
                int tile = staticsData[staticDataIndex++] + staticsData[staticDataIndex++] * 8;
                sbyte z = (sbyte)staticsData[staticDataIndex++];
                int hue = staticsData[staticDataIndex++] + (staticsData[staticDataIndex++] * 256); // is this used?

                ItemData data = TileData.ItemData[itemID];
                int iz = z + data.Height + (data.IsRoof || data.IsSurface ? 1 : 0);

                if ((x * 8 + (tile % 8) == 1480) && (y * 8 + (tile / 8) == 1608))
                {
                    if (iz > m_Zs[tile])
                    {
                        Colors[tile] = RadarColorData.Colors[itemID + 0x4000];
                        m_Zs[tile] = (sbyte)iz;
                    }
                }

                if (iz > m_Zs[tile])
                {
                    Colors[tile] = RadarColorData.Colors[itemID + 0x4000];
                    m_Zs[tile] = (sbyte)iz;
                }
            }
        }

        public MiniMapChunk(MapChunk block)
        {
            X = (uint)block.ChunkX;
            Y = (uint)block.ChunkY;
            Colors = new uint[64];

            for (uint tile = 0; tile < 64; tile++)
            {
                uint color = 0xffff00ff;
                // get the topmost static item or ground
                int eIndex = block.Tiles[tile].Entities.Count - 1;
                while (eIndex >= 0)
                {
                    AEntity e = block.Tiles[tile].Entities[eIndex];
                    if (e is Ground)
                    {
                        color = RadarColorData.Colors[(e as Ground).LandDataID];
                        break;
                    }
                    else if (e is StaticItem)
                    {
                        color = RadarColorData.Colors[(e as StaticItem).ItemID + 0x4000];
                        break;
                    }
                    eIndex--;
                }
                Colors[tile] = color;
            }
        }
    }
}
