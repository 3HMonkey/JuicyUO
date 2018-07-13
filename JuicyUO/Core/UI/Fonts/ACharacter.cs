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

namespace JuicyUO.Core.UI.Fonts
{
    abstract class ACharacter : ICharacter
    {
        protected bool HuePassedColor;
        protected uint[] m_PixelData;

        public int Width
        {
            get;
            set;
        }

        public int Height
        {
            get;
            set;
        }

        public int ExtraWidth
        {
            get;
            set;
        }

        public int XOffset
        {
            get;
            protected set;
        }

        public int YOffset
        {
            get;
            set;
        }

        public unsafe void WriteToBuffer(uint* dstPtr, int dx, int dy, int linewidth, int maxHeight, int baseLine, bool isBold, bool isItalic, bool isUnderlined, bool isOutlined, uint color, uint outline)
        {
            uint inColor = color;
            if (m_PixelData != null)
            {
                fixed (uint* srcPtr = m_PixelData)
                {
                    for (int y = 0; (y < Height) && (y + dy < maxHeight); y++)
                    {
                        uint* src = (srcPtr + (Width * y));
                        uint* dest = (dstPtr + (linewidth * (y + dy + YOffset)) + dx);
                        if (isItalic)
                        {
                            dest += (baseLine - YOffset - y - 1) / 2;
                        }
                        for (int x = 0; x < Width; x++)
                        {
                            if (*src != 0x00000000)
                            {
                                if (HuePassedColor)
                                {
                                    uint r = (uint)((*src & 0x000000ff) * ((float)(inColor & 0x000000ff) / 0xff));
                                    uint g = (uint)((*src & 0x0000ff00) * ((float)((inColor >> 08) & 0x000000ff) / 0xff)) & 0x0000ff00;
                                    uint b = (uint)((*src & 0x00ff0000) * ((float)((inColor >> 16) & 0x000000ff) / 0xff)) & 0x00ff0000;
                                    color = 0xff000000 + b + g + r;
                                }
                                if (isOutlined)
                                {
                                    for (int iy = -1; iy <= 1; iy++)
                                    {
                                        uint* idest = (dest + (iy * linewidth));
                                        if (*idest == 0x00000000)
                                        {
                                            *idest = outline;
                                        }
                                        if (iy == 0)
                                        {
                                            if (isBold)
                                            {
                                                if (*(src - 1) == 0x00000000)
                                                {
                                                    *(idest) = outline;
                                                    *(idest + 1) = color;
                                                }
                                                else
                                                {
                                                    *(idest + 1) = color;
                                                }
                                                *(idest + 2) = color;
                                            }
                                            else
                                            {
                                                *(idest + 1) = color;
                                            }
                                        }
                                        else
                                        {
                                            if (*(idest + 1) == 0x00000000)
                                            {
                                                *(idest + 1) = outline;
                                            }
                                        }
                                        if (*(idest + 2) == 0x00000000)
                                        {
                                            *(idest + 2) = outline;
                                        }
                                        if (isBold)
                                        {
                                            if (*(idest + 3) == 0x00000000)
                                            {
                                                *(idest + 3) = outline;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    *dest = color;
                                    if (isBold)
                                        *(dest + 1) = color;
                                }
                            }
                            dest++;
                            src++;
                        }
                    }
                }
            }

            if (isUnderlined)
            {
                int underlineAtY = dy + baseLine + 2;
                if (underlineAtY >= maxHeight)
                    return;
                uint* dest = (((uint*)dstPtr) + (linewidth * (underlineAtY)) + dx);
                int w = isBold ? Width + 2 : Width + 1;
                for (int k = 0; k < w; k++)
                {
                    if (isOutlined)
                    {
                        for (int iy = -1; iy <= 1; iy++)
                        {
                            uint* idest = dest + (iy * linewidth);
                            if (*idest == 0x00000000)
                                *idest = outline;
                            if (iy == 0)
                                *(idest + 1) = color;
                            else
                            {
                                if (*(idest + 1) == 0x00000000)
                                    *(idest + 1) = outline;
                            }

                            if (*(idest + 2) == 0x00000000)
                                *(idest + 2) = outline;
                        }
                    }
                    else
                    {
                        *dest = color;
                    }
                    dest++;
                }
            }
        }
    }
}