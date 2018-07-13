﻿#region license
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
using Microsoft.Xna.Framework.Graphics;
using JuicyUO.Core.Diagnostics;
using JuicyUO.Core.IO;
using JuicyUO.Core.Resources;
#endregion

namespace JuicyUO.Ultima.Resources
{
    public sealed class AnimationFrame : AAnimationFrame
    {
        const int DoubleXor = (0x200 << 22) | (0x200 << 12);
        
        readonly int m_AnimationIndex;

        public override bool IsPointInTexture(int x, int y) => m_Picking.Get(m_AnimationIndex, x, y);

        public static readonly AnimationFrame NullFrame = new AnimationFrame();
        public static readonly AnimationFrame[] NullFrames = { NullFrame };
        static PixelPicking m_Picking = new PixelPicking();

        AnimationFrame()
        {
            IResourceProvider provider = Service.Get<IResourceProvider>();
            Texture = provider.GetItemTexture(1);
            Center = new Point(0, 0);
        }

        public unsafe AnimationFrame(int uniqueAnimationIndex, GraphicsDevice graphics, ushort[] palette, BinaryFileReader reader, SittingTransformation sitting)
        {
            m_AnimationIndex = uniqueAnimationIndex;
            int xCenter = reader.ReadShort();
            int yCenter = reader.ReadShort();
            int width = reader.ReadUShort();
            int height = reader.ReadUShort();
            // Fix for animations with no pixels.
            if ((width == 0) || (height == 0))
            {
                Texture = null;
                return;
            }
            if (sitting == SittingTransformation.StandSouth)
            {
                xCenter += 8;
                width += 8;
                height += 4;
            }
            ushort[] data = new ushort[width * height];
            // for sitting:
            // somewhere around the waist of a typical mobile animation, we take twelve rows of pixels,
            // discard every third, and shift every remaining row (total of eight) one pixel to the left
            // or right (depending on orientation), for a total skew of eight pixels.
            fixed (ushort* pData = data)
            {
                ushort* dataRef = pData;

                int dataRead = 0;

                int header;
                while ((header = reader.ReadInt()) != 0x7FFF7FFF)
                {
                    header ^= DoubleXor;
                    int x = ((header >> 22) & 0x3FF) + xCenter - 0x200;
                    int y = ((header >> 12) & 0x3FF) + yCenter + height - 0x200;
                    if (sitting == SittingTransformation.StandSouth)
                    {
                        const int skew_start = -17;
                        const int skew_end = skew_start - 16;
                        int iy = y - height - yCenter;
                        if (iy > skew_start)
                        {
                            // pixels below the skew
                            x -= 8;
                            y -= 4;
                        }
                        else if (iy > skew_end)
                        {
                            // pixels within the skew
                            if ((iy - skew_end) % 4 == 0)
                            {
                                reader.Position += (header & 0xFFF);
                                continue;
                            }
                            x -= (iy - skew_end) / 2;
                            y -= (iy - skew_end) / 4;
                        }
                    }
                    ushort* cur = dataRef + y * width + x;
                    ushort* end = cur + (header & 0xFFF);
                    int filecounter = 0;
                    byte[] filedata = reader.ReadBytes(header & 0xFFF);
                    while (cur < end)
                    {
                        *cur++ = palette[filedata[filecounter++]];
                    }
                    dataRead += header & 0xFFF;
                }
                Metrics.ReportDataRead(dataRead);
            }
            Center = new Point(xCenter, yCenter);
            Texture = new Texture2D(graphics, width, height, false, SurfaceFormat.Bgra5551);
            Texture.SetData(data);
            m_Picking.Set(m_AnimationIndex, width, height, data);
        }

        public enum SittingTransformation
        {
            None,
            StandSouth,
            MountNorth
        }
    }
}
