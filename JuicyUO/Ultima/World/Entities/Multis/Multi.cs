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
using Microsoft.Xna.Framework;
using JuicyUO.Ultima.Resources;
using JuicyUO.Ultima.World.Entities.Items;
using JuicyUO.Ultima.World.Maps;
using JuicyUO.Ultima.World.Data;
#endregion

namespace JuicyUO.Ultima.World.Entities.Multis
{
    class Multi : AEntity
    {
        private static List<Multi> s_RegisteredMultis = new List<Multi>();

        private static void RegisterForMapBlockLoads(Multi multi)
        {
            if (!s_RegisteredMultis.Contains(multi))
                s_RegisteredMultis.Add(multi);
        }

        private static void UnregisterForMapBlockLoads(Multi multi)
        {
            if (s_RegisteredMultis.Contains(multi))
                s_RegisteredMultis.Remove(multi);
        }

        public static void AnnounceMapChunkLoaded(MapChunk chunk)
        {
            for (int i = 0; i < s_RegisteredMultis.Count; i++)
                if (!s_RegisteredMultis[i].IsDisposed)
                    s_RegisteredMultis[i].PlaceTilesIntoNewlyLoadedChunk(chunk);
        }

        MultiComponentList m_Components;

        int m_customHouseRevision = 0x7FFFFFFF;
        StaticTile[] m_customHouseTiles;
        public int CustomHouseRevision { get { return m_customHouseRevision; } }

        // bool m_hasCustomTiles = false;
        CustomHouse m_customHouse;
        public void AddCustomHousingTiles(CustomHouse house)
        {
            // m_hasCustomTiles = true;
            m_customHouse = house;
            m_customHouseTiles = house.GetStatics(m_Components.Width, m_Components.Height);
        }

        int m_MultiID = -1;
        public int MultiID
        {
            get { return m_MultiID; }
            set
            {
                if (m_MultiID != value)
                {
                    m_MultiID = value;
                    m_Components = MultiData.GetComponents(m_MultiID);
                    InitialLoadTiles();
                }
            }
        }

        public Multi(Serial serial, Map map)
            : base(serial, map)
        {
            RegisterForMapBlockLoads(this);
        }

        public override void Dispose()
        {
            UnregisterForMapBlockLoads(this);
            base.Dispose();
        }

        private void InitialLoadTiles()
        {
            int px = Position.X;
            int py = Position.Y;

            foreach (MultiComponentList.MultiItem item in m_Components.Items)
            {
                int x = px + item.OffsetX;
                int y = py + item.OffsetY;

                MapTile tile = Map.GetMapTile((uint)x, (uint)y);
                if (tile != null)
                {
                    if (tile.ItemExists(item.ItemID, item.OffsetZ))
                        continue;

                    StaticItem staticItem = new StaticItem(item.ItemID, 0, 0, Map);
                    if (staticItem.ItemData.IsDoor)
                        continue;
                    staticItem.Position.Set(x, y, Z + item.OffsetZ);
                }
            }
        }

        private void PlaceTilesIntoNewlyLoadedChunk(MapChunk chunk)
        {
            int px = Position.X;
            int py = Position.Y;

            Rectangle bounds = new Rectangle((int)chunk.ChunkX * 8, (int)chunk.ChunkY * 8, 8, 8);

            foreach (MultiComponentList.MultiItem item in m_Components.Items)
            {
                int x = px + item.OffsetX;
                int y = py + item.OffsetY;

                if (bounds.Contains(x, y))
                {
                    // would it be faster to get the tile from the chunk?
                    MapTile tile = Map.GetMapTile(x, y);
                    if (tile != null)
                    {
                        if (!tile.ItemExists(item.ItemID, item.OffsetZ))
                        {
                            StaticItem staticItem = new StaticItem(item.ItemID, 0, 0, Map);
                            staticItem.Position.Set(x, y, Z + item.OffsetZ);
                        }
                    }
                }
            }
        }

        public override int GetMaxUpdateRange()
        {
            if (m_customHouse == null)
                return 22;
            else
                return 24;
        }
    }
}
