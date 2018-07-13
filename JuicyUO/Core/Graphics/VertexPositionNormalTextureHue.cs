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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JuicyUO.Core.Graphics
{
    public struct VertexPositionNormalTextureHue : IVertexType
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector3 TextureCoordinate;
        public Vector3 Hue;

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), // position
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0), // normal
            new VertexElement(sizeof(float) * 6, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0), // tex coord
            new VertexElement(sizeof(float) * 9, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 1) // hue
        );

        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }

        public VertexPositionNormalTextureHue(Vector3 position, Vector3 normal, Vector3 textureCoordinate)
        {
            Position = position;
            Normal = normal;
            TextureCoordinate = textureCoordinate;
            Hue = Vector3.Zero;
        }

        public static readonly VertexPositionNormalTextureHue[] PolyBuffer = {
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector3(0, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector3(1, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector3(0, 1, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector3(1, 1, 0))
            };

        public static readonly VertexPositionNormalTextureHue[] PolyBufferFlipped = {
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector3(0, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector3(0, 1, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector3(1, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector3(1, 1, 0))
            };

        public static int SizeInBytes { get { return sizeof( float ) * 12; } }

        public override string ToString()
        {
            return string.Format("VPNTH: <{0}> <{1}>", Position.ToString(), TextureCoordinate.ToString());
        }
    }
}
