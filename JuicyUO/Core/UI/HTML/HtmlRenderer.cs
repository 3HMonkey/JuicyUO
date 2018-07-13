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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JuicyUO.Core.Diagnostics.Tracing;
using JuicyUO.Core.Graphics;
using JuicyUO.Core.UI.HTML.Elements;
using JuicyUO.Core.UI.HTML.Styles;

namespace JuicyUO.Core.UI.HTML
{
    class HtmlRenderer
    {
        /// <summary>
        /// Renders all the elements in the root branch. At the same time, also sets areas for regions and href links.
        /// TODO: code for setting areas for regions / hrefs belongs in layout code in HtmlDocument.
        /// </summary>
        public Texture2D Render(BlockElement root, int ascender, HtmlLinkList links)
        {
            SpriteBatchUI sb = Service.Get<SpriteBatchUI>();
            GraphicsDevice graphics = sb.GraphicsDevice;

            if (root == null || root.Width == 0 || root.Height == 0) // empty text string
            {
                return new Texture2D(graphics, 1, 1);
            }
            uint[] pixels = new uint[root.Width * root.Height];

            if (root.Err_Cant_Fit_Children)
            {
                for (int i = 0; i < pixels.Length; i++)
                {
                    pixels[i] = 0xffffff00;
                }
                Tracer.Error("Err: Block can't fit children.");
            }
            else
            {
                unsafe
                {
                    fixed (uint* ptr = pixels)
                    {
                        DoRenderBlock(root, ascender, links, ptr, root.Width, root.Height);
                    }
                }
            }

            Texture2D texture = new Texture2D(graphics, root.Width, root.Height, false, SurfaceFormat.Color);
            texture.SetData(pixels);
            return texture;
        }

        unsafe void DoRenderBlock(BlockElement root, int ascender, HtmlLinkList links, uint* ptr, int width, int height)
        {
            foreach (AElement e in root.Children)
            {
                int x = e.Layout_X;
                int y = e.Layout_Y - ascender; // ascender is always negative.
                StyleState style = e.Style;
                if (e is CharacterElement)
                {
                    IFont font = style.Font;
                    ICharacter character = font.GetCharacter((e as CharacterElement).Character);
                    // HREF links should be colored white, because we will hue them at runtime.
                    uint color = style.IsHREF ? 0xFFFFFFFF : Utility.UintFromColor(style.Color);
                    character.WriteToBuffer(ptr, x, y, width, height, font.Baseline, style.IsBold, style.IsItalic, style.IsUnderlined, style.DrawOutline, color, 0xFF000008);
                    // offset y by ascender for links...
                    if (character.YOffset < 0)
                    {
                        y += character.YOffset;
                        height -= character.YOffset;
                    }
                }
                else if (e is ImageElement)
                {
                    ImageElement image = (e as ImageElement);
                    image.AssociatedImage.Area = new Rectangle(x, y, image.Width, image.Height);
                    if (style.IsHREF)
                    {
                        links.AddLink(style, new Rectangle(x, y, e.Width, e.Height));
                        image.AssociatedImage.LinkIndex = links.Count;
                    }
                }
                else if (e is BlockElement)
                {
                    DoRenderBlock(e as BlockElement, ascender, links, ptr, width, height);
                }
                // set href link regions
                if (style.IsHREF)
                {
                    links.AddLink(style, new Rectangle(x, y, e.Width, e.Height));
                }
            }
        }
    }
}
