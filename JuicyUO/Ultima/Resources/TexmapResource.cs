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
using Microsoft.Xna.Framework.Graphics;
using JuicyUO.Core.Diagnostics;
using JuicyUO.Core.Diagnostics.Tracing;
using JuicyUO.Core.IO;
using JuicyUO.Ultima.IO;
#endregion

namespace JuicyUO.Ultima.Resources
{
    class TexmapResource
    {
        private Texture2D[] m_Cache = new Texture2D[0x4000];
        private readonly AFileIndex m_Index = FileManager.CreateFileIndex("texidx.mul", "texmaps.mul", 0x4000, -1); // !!! must find patch file reference for texmap.
        private readonly GraphicsDevice m_Graphics;

        private const int DEFAULT_TEXTURE = 0x007F; // index 127 is the first 'unused' texture.

        public TexmapResource(GraphicsDevice graphics)
        {
            m_Graphics = graphics;
        }

        public Texture2D GetTexmapTexture(int index)
        {
            index &= 0x3FFF;

            if (m_Cache[index] == null)
            {
                m_Cache[index] = readTexmapTexture(index);
                if (m_Cache[index] == null)
                    m_Cache[index] = GetTexmapTexture(127);
            }

            return m_Cache[index];
        }

        private unsafe Texture2D readTexmapTexture(int index)
        {
            int length, extra;
            bool is_patched;

            BinaryFileReader reader = m_Index.Seek(index, out length, out extra, out is_patched);
            if (reader == null)
                return null;
            if (reader.Stream.Length == 0)
            {
                Tracer.Warn("Requested texmap texture #{0} does not exist. Replacing with 'unused' graphic.", index);
                return GetTexmapTexture(DEFAULT_TEXTURE);
            }

            int metrics_dataread_start = (int)reader.Position;

            int textureSize = (extra == 0) ? 64 : 128;

            ushort[] pixelData = new ushort[textureSize * textureSize];
            ushort[] fileData = reader.ReadUShorts(textureSize * textureSize);

            fixed (ushort* pData = pixelData)
            {
                ushort* pDataRef = pData;

                int count = 0;
                int max = textureSize * textureSize;

                while (count < max)
                {
                    ushort color = (ushort)(fileData[count] | 0x8000);
                    *pDataRef++ = color;
                    count++;
                }
            }

            Texture2D texture = new Texture2D(m_Graphics, textureSize, textureSize, false, SurfaceFormat.Bgra5551);

            texture.SetData<ushort>(pixelData);

            Metrics.ReportDataRead((int)reader.Position - metrics_dataread_start);

            return texture;
        }
    }
}