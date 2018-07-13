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
using JuicyUO.Core.UI;

namespace JuicyUO.Core.Resources
{
    public interface IResourceProvider
    {
        AAnimationFrame[] GetAnimation(int body, ref int hue, int action, int direction);
        Texture2D GetUITexture(int textureID, bool replaceMask080808 = false);
        Texture2D GetItemTexture(int textureID);
        Texture2D GetLandTexture(int textureID);
        Texture2D GetTexmapTexture(int textureID);

        bool IsPointInUITexture(int textureID, int x, int y);
        bool IsPointInItemTexture(int textureID, int x, int y, int extraRange = 0);
        void GetItemDimensions(int textureID, out int width, out int height);

        ushort GetWebSafeHue(Color color);
        IFont GetUnicodeFont(int fontIndex);
        IFont GetAsciiFont(int fontIndex);
        string GetString(int strIndex);

        void RegisterResource<T>(IResource<T> resource);
        T GetResource<T>(int resourceIndex);
    }
}
