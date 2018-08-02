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
using JuicyUO.Ultima.Data;
using JuicyUO.Ultima.IO;
#endregion

namespace JuicyUO.Ultima.Resources
{
    public class GumpMulResource
    {
        AFileIndex m_FileIndex = ClientVersion.InstallationIsUopFormat ? FileManager.CreateFileIndex("gumpartLegacyMUL.uop", 0xFFFF, true, ".tga") : FileManager.CreateFileIndex("Gumpidx.mul", "Gumpart.mul", 0x10000, 12);
        GraphicsDevice m_graphicsDevice;
        readonly PixelPicking m_Picking = new PixelPicking();
        Texture2D[] m_TextureCache = new Texture2D[0x10000];

        public AFileIndex FileIndex => m_FileIndex;

        public GumpMulResource(GraphicsDevice graphics)
        {
            m_graphicsDevice = graphics;
        }

        public unsafe Texture2D GetGumpTexture(int textureID, bool replaceMask080808 = false)
        {
            if (textureID < 0)
            {
                return null;
            }

            if (m_TextureCache[textureID] == null)
            {
                int length, extra;
                bool patched;
                BinaryFileReader reader = m_FileIndex.Seek(textureID, out length, out extra, out patched);
                if (reader == null)
                {
                    return null;
                }
                int width = (extra >> 16) & 0xFFFF;
                int height = extra & 0xFFFF;
                if (width == 0 || height == 0)
                {
                    return null;
                }
                int shortsToRead = length - (height * 2);
                if (reader.Stream.Length - reader.Position < (shortsToRead * 2))
                {
                    Tracer.Error($"Could not read gump {textureID:X4}: not enough data. Gump texture file truncated?");
                    return null;
                }
                int[] lookups = reader.ReadInts(height);
                int metrics_dataread_start = (int)reader.Position;
                ushort[] fileData = reader.ReadUShorts(shortsToRead);
                ushort[] pixels = new ushort[width * height];
                fixed (ushort* line = &pixels[0])
                {
                    fixed (ushort* data = &fileData[0])
                    {
                        for (int y = 0; y < height; ++y)
                        {
                            ushort* dataRef = data + (lookups[y] - height) * 2;
                            ushort* cur = line + (y * width);
                            ushort* end = cur + width;
                            while (cur < end)
                            {
                                ushort color = *dataRef++;
                                ushort* next = cur + *dataRef++;
                                if (color == 0)
                                {
                                    cur = next;
                                }
                                else
                                {
                                    color |= 0x8000;
                                    while (cur < next)
                                        *cur++ = color;
                                }
                            }
                        }
                    }
                }
                Metrics.ReportDataRead(length);
                if (replaceMask080808)
                {
                    for (int i = 0; i < pixels.Length; i++)
                    {
                        if (pixels[i] == 0x8421)
                        {
                            pixels[i] = 0xFC1F;
                        }
                    }
                }
                Texture2D texture = new Texture2D(m_graphicsDevice, width, height, false, SurfaceFormat.Bgra5551);
                texture.SetData(pixels);
                m_TextureCache[textureID] = texture;
                m_Picking.Set(textureID, width, height, pixels);
            }
            return m_TextureCache[textureID];
        }

        public bool IsPointInGumpTexture(int textureID, int x, int y)
        {
            return m_Picking.Get(textureID, x, y);
        }
    }
}