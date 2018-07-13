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
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using JuicyUO.Core.Diagnostics.Tracing;
using JuicyUO.Core.Resources;
using JuicyUO.Core.UI;
using JuicyUO.Ultima.Resources;
#endregion

namespace JuicyUO.Ultima
{
    class ResourceProvider : IResourceProvider
    {
        AnimationResource m_Anim;
        ArtMulResource m_Art;
        ClilocResource m_Cliloc;
        EffectDataResource m_Effects;
        FontsResource m_Fonts;
        GumpMulResource m_Gumps;
        TexmapResource m_Texmaps;
        readonly Dictionary<Type, object> m_Resources = new Dictionary<Type, object>();

        public ResourceProvider(Game game)
        {
            m_Anim = new AnimationResource(game.GraphicsDevice);
            m_Art = new ArtMulResource(game.GraphicsDevice);
            m_Cliloc = new ClilocResource("enu");
            m_Effects = new EffectDataResource();
            m_Fonts = new FontsResource(game.GraphicsDevice);
            m_Gumps = new GumpMulResource(game.GraphicsDevice);
            m_Texmaps = new TexmapResource(game.GraphicsDevice);
        }

        public AAnimationFrame[] GetAnimation(int body, ref int hue, int action, int direction)
        {
            return m_Anim.GetAnimation(body, ref hue, action, direction);
        }

        public Texture2D GetUITexture(int textureID, bool replaceMask080808 = false)
        {
            return m_Gumps.GetGumpTexture(textureID, replaceMask080808);
        }

        public bool IsPointInUITexture(int textureID, int x, int y)
        {
            return m_Gumps.IsPointInGumpTexture(textureID, x, y);
        }

        public Texture2D GetItemTexture(int itemIndex)
        {
            return m_Art.GetStaticTexture(itemIndex);
        }

        public bool IsPointInItemTexture(int textureID, int x, int y, int extraRange = 0)
        {
            return m_Art.IsPointInItemTexture(textureID, x, y, extraRange);
        }

        public Texture2D GetLandTexture(int landIndex)
        {
            return m_Art.GetLandTexture(landIndex);
        }

        public void GetItemDimensions(int itemIndex, out int width, out int height)
        {
            m_Art.GetStaticDimensions(itemIndex, out width, out height);
        }

        public Texture2D GetTexmapTexture(int textureIndex)
        {
            return m_Texmaps.GetTexmapTexture(textureIndex);
        }

        /// <summary>
        /// Returns a Ultima Online Hue index that approximates the passed color.
        /// </summary>
        public ushort GetWebSafeHue(Color color)
        {
            return (ushort)HueData.GetWebSafeHue(color);
        }

        public IFont GetUnicodeFont(int fontIndex)
        {
            return m_Fonts.GetUniFont(fontIndex);
        }

        public IFont GetAsciiFont(int fontIndex)
        {
            return m_Fonts.GetAsciiFont(fontIndex);
        }

        public string GetString(int clilocIndex)
        {
            return m_Cliloc.GetString(clilocIndex);
        }

        

        public void RegisterResource<T>(IResource<T> resource)
        {
            Type type = typeof(T);

            if (m_Resources.ContainsKey(type))
            {
                Tracer.Critical(string.Format("Attempted to register resource provider of type {0} twice.", type));
                m_Resources.Remove(type);
            }

            m_Resources.Add(type, resource);
        }

        public T GetResource<T>(int resourceIndex)
        {
            Type type = typeof(T);

            if (m_Resources.ContainsKey(type))
            {
                IResource<T> resource = (IResource<T>)m_Resources[type];
                return (T)resource.GetResource(resourceIndex);
            }
            else
            {
                Tracer.Critical(string.Format("Attempted to get resource provider of type {0}, but no provider with this type is registered.", type));
                return default(T);
            }
        }
    }
}
