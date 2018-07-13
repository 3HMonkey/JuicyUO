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
using System.Drawing;
using System.Linq;
using JuicyUO.Core.Diagnostics;
using JuicyUO.Core.IO;
using JuicyUO.Ultima.IO;
#endregion

namespace JuicyUO.Ultima.Resources
{
    public class MultiData
    {
        private static MultiComponentList[] m_Components = new MultiComponentList[0x4000];
        public static MultiComponentList[] Cache { get { return m_Components; } }

        private static readonly AFileIndex m_FileIndex = FileManager.CreateFileIndex("Multi.idx", "Multi.mul", 0x4000, 14);
        public static AFileIndex FileIndex { get { return m_FileIndex; } }

        public static MultiComponentList GetComponents(int index)
        {
            MultiComponentList mcl;

            index &= FileManager.ItemIDMask;

            if (index >= 0 && index < m_Components.Length)
            {
                mcl = m_Components[index];

                if (mcl == null)
                    m_Components[index] = mcl = Load(index);
            }
            else
            {
                mcl = MultiComponentList.Empty;
            }

            return mcl;
        }

        public static MultiComponentList Load(int index)
        {
            try
            {
                int length, extra;
                bool patched;
                BinaryFileReader reader = m_FileIndex.Seek(index, out length, out extra, out patched);
                if (reader == null)
                    return MultiComponentList.Empty;

                return new MultiComponentList(reader, length / 12);
            }
            catch
            {
                return MultiComponentList.Empty;
            }
        }
    }

    public sealed class MultiComponentList
    {
        private Point m_Min, m_Max, m_Center;
        private int m_Width, m_Height;

        public static readonly MultiComponentList Empty = new MultiComponentList();

        public Point Min { get { return m_Min; } }
        public Point Max { get { return m_Max; } }
        public Point Center { get { return m_Center; } }
        public int Width { get { return m_Width; } }
        public int Height { get { return m_Height; } }

        public MultiItem[] Items
        {
            get;
            private set;
        }

        public struct MultiItem
        {
            public short ItemID;
            public short OffsetX, OffsetY, OffsetZ;
            public int Flags;

            public override string ToString()
            {
                return string.Format("{0:X4} {1} {2} {3} {4:X4}", ItemID, OffsetX, OffsetY, OffsetZ, Flags);
            }
        }

        public MultiComponentList(BinaryFileReader reader, int count)
        {
            int metrics_dataread_start = (int)reader.Position;

            m_Min = m_Max = Point.Empty;

            Items = new MultiItem[count];

            for (int i = 0; i < count; ++i)
            {
                Items[i].ItemID = reader.ReadShort();
                Items[i].OffsetX = reader.ReadShort();
                Items[i].OffsetY = reader.ReadShort();
                Items[i].OffsetZ = reader.ReadShort();
                Items[i].Flags = reader.ReadInt();

                if (Items[i].OffsetX < m_Min.X)
                    m_Min.X = Items[i].OffsetX;

                if (Items[i].OffsetY < m_Min.Y)
                    m_Min.Y = Items[i].OffsetY;

                if (Items[i].OffsetX > m_Max.X)
                    m_Max.X = Items[i].OffsetX;

                if (Items[i].OffsetY > m_Max.Y)
                    m_Max.Y = Items[i].OffsetY;
            }

            m_Center = new Point(-m_Min.X, -m_Min.Y);
            m_Width = (m_Max.X - m_Min.X) + 1;
            m_Height = (m_Max.Y - m_Min.Y) + 1;

            // SortMultiComponentList();

            Metrics.ReportDataRead((int)reader.Position - metrics_dataread_start);
        }

        private void SortMultiComponentList()
        {
            Items = Items.OrderBy(a => a.OffsetY).ThenBy(a => a.OffsetX).ToArray();
        }

        private MultiComponentList()
        {
            Items = new MultiItem[0];
        }
    }
}