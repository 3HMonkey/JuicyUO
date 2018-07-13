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
using Microsoft.Xna.Framework;
using System;
using JuicyUO.Ultima.Data;
using JuicyUO.Ultima.Resources;
using JuicyUO.Ultima.World.Entities.Multis;
#endregion

namespace JuicyUO.Ultima.World.Maps
{
    public class Map
    {
        private readonly MapChunk[] m_Chunks;
        public TileMatrixData MapData
        {
            get;
            private set;
        }

        private Point m_Center = new Point(int.MinValue, int.MinValue); // player position.

        public readonly uint Index;
        public readonly uint TileHeight, TileWidth;

        // Any mobile / item beyond this range is removed from the client. RunUO's range is 24 tiles, which would equal 3 cells.
        // We keep 4 cells in memory to allow for drawing further, and also as a safety precaution - don't want to unload an 
        // entity at the edge of what we keep in memory just because of being slightly out of sync with the server.
        private const int c_CellsInMemory = 5;
        private const int c_CellsInMemorySpan = c_CellsInMemory * 2 + 1;

        public Map(uint index)
        {
            Index = index;

            MapData = new TileMatrixData(Index);
            TileHeight = MapData.ChunkHeight * 8;
            TileWidth = MapData.ChunkWidth * 8;

            m_Chunks = new MapChunk[c_CellsInMemorySpan * c_CellsInMemorySpan];
        }

        public void Dispose()
        {
            for (int i = 0; i < m_Chunks.Length; i++)
            {
                if (m_Chunks[i] != null)
                {
                    m_Chunks[i].Dispose();
                    m_Chunks[i] = null;
                }
            }
        }

        public Point CenterPosition
        {
            get { return m_Center; }
            set
            {
                if (value != m_Center)
                {
                    m_Center = value;
                }

                InternalCheckCellsInMemory();
            }
        }

        public MapChunk GetMapChunk(uint x, uint y)
        {
            uint cellIndex = (y % c_CellsInMemorySpan) * c_CellsInMemorySpan + (x % c_CellsInMemorySpan);
            MapChunk cell = m_Chunks[cellIndex];
            if (cell == null)
                return null;
            if (cell.ChunkX != x || cell.ChunkY != y)
                return null;
            return cell;
        }

        public MapTile GetMapTile(int x, int y)
        {
            return GetMapTile((uint)x, (uint)y);
        }

        public MapTile GetMapTile(uint x, uint y)
        {
            uint cellX = (uint)x / 8, cellY = (uint)y / 8;
            uint cellIndex = (cellY % c_CellsInMemorySpan) * c_CellsInMemorySpan + (cellX % c_CellsInMemorySpan);

            MapChunk cell = m_Chunks[cellIndex];
            if (cell == null)
                return null;
            if (cell.ChunkX != cellX || cell.ChunkY != cellY)
                return null;
            return cell.Tiles[(y % 8) * 8 + (x % 8)];
        }

        private void InternalCheckCellsInMemory()
        {
            uint centerX = ((uint)CenterPosition.X / 8);
            uint centerY = ((uint)CenterPosition.Y / 8);
            for (int y = -c_CellsInMemory; y <= c_CellsInMemory; y++)
            {
                uint cellY = (uint)(centerY + y) % MapData.ChunkHeight;
                for (int x = -c_CellsInMemory; x <= c_CellsInMemory; x++)
                {
                    uint cellX = (uint)(centerX + x) % MapData.ChunkWidth;

                    uint cellIndex = (cellY % c_CellsInMemorySpan) * c_CellsInMemorySpan + (cellX % c_CellsInMemorySpan);
                    if (m_Chunks[cellIndex] == null || m_Chunks[cellIndex].ChunkX != cellX || m_Chunks[cellIndex].ChunkY != cellY)
                    {
                        if (m_Chunks[cellIndex] != null)
                            m_Chunks[cellIndex].Dispose();
                        m_Chunks[cellIndex] = new MapChunk(cellX, cellY);
                        m_Chunks[cellIndex].LoadStatics(MapData, this);
                        // if we have a translator and it's not spring, change some statics!
                        if (Season != Seasons.Spring && SeasonalTranslator != null)
                            SeasonalTranslator(m_Chunks[cellIndex], Season);
                        // let any active multis know that a new map chunk is ready, so they can load in their pieces.
                        Multi.AnnounceMapChunkLoaded(m_Chunks[cellIndex]);
                    }
                }
            }
        }

        public float GetTileZ(int x, int y)
        {
            MapTile t = GetMapTile(x, y);
            if (t != null)
                return t.Ground.Z;
            else
            {
                ushort tileID;
                sbyte alt;
                // THIS IS VERY INEFFICIENT :(
                MapData.GetLandTile((uint)x, (uint)y, out tileID, out alt);
                return alt;
            }
        }

        public int GetAverageZ(int top, int left, int right, int bottom, ref int low, ref int high)
        {
            high = top;
            if (left > high)
                high = left;
            if (right > high)
                high = right;
            if (bottom > high)
                high = bottom;

            low = high;
            if (left < low)
                low = left;
            if (right < low)
                low = right;
            if (bottom < low)
                low = bottom;

            if (Math.Abs(top - bottom) > Math.Abs(left - right))
                return FloorAverage(left, right);
            else
                return FloorAverage(top, bottom);
        }

        public int GetAverageZ(int x, int y, ref int low, ref int top)
        {
            return GetAverageZ(
                (int)GetTileZ(x, y),
                (int)GetTileZ(x, y + 1),
                (int)GetTileZ(x + 1, y),
                (int)GetTileZ(x + 1, y + 1),
                ref low, ref top);
        }

        private static int FloorAverage(int a, int b)
        {
            int v = a + b;

            if (v < 0)
                --v;

            return (v / 2);
        }

        private Seasons m_Season = Seasons.Summer;
        public Seasons Season
        {
            get { return m_Season; }
            set
            {
                if (m_Season != value)
                {
                    m_Season = value;
                    if (SeasonalTranslator != null)
                        foreach (MapChunk chunk in m_Chunks)
                            SeasonalTranslator(chunk, Season);
                }
            }
        }

        public static Action<MapChunk, Seasons> SeasonalTranslator;

        public void ReloadStatics()
        {
            foreach (MapChunk chunk in m_Chunks)
            {
                if (chunk != null)
                {
                    chunk.UnloadStatics();
                    chunk.LoadStatics(MapData, this);
                }
            }
        }
    }
}