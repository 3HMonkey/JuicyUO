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

using System;
using Microsoft.Xna.Framework;

namespace JuicyUO.Ultima.World
{
    [Flags]
    public enum Direction : byte
    {
        North = 0x0,
        Right = 0x1,
        East = 0x2,
        Down = 0x3,
        South = 0x4,
        Left = 0x5,
        West = 0x6,
        Up = 0x7,
        FacingMask = 0x7,
        Running = 0x80,
        ValueMask = 0x87,
        Nothing = 0xED
    }

    public static class DirectionHelper
    {
        public static Direction DirectionFromPoints(Point from, Point to)
        {
            return DirectionFromVectors(new Vector2(from.X, from.Y), new Vector2(to.X, to.Y));
        }

        public static Direction DirectionFromVectors(Vector2 fromPosition, Vector2 toPosition)
        {
            double Angle = Math.Atan2(toPosition.Y - fromPosition.Y, toPosition.X - fromPosition.X);
            if (Angle < 0)
                Angle = Math.PI + (Math.PI + Angle);
            double piPerSegment = (Math.PI * 2f) / 8f;
            double segmentValue = (Math.PI * 2f) / 16f;
            int direction = int.MaxValue;

            for (int i = 0; i < 8; i++)
            {
                if (Angle >= segmentValue && Angle <= (segmentValue + piPerSegment))
                {
                    direction = i + 1;
                    break;
                }
                segmentValue += piPerSegment;
            }

            if (direction == int.MaxValue)
                direction = 0;

            direction = (direction >= 7) ? (direction - 7) : (direction + 1);

            return (Direction)direction;
        }

        public static Direction GetCardinal(Direction inDirection)
        {
            return inDirection & (Direction)0x6; // contains bitmasks for 0x0, 0x2, 0x4, and 0x6
        }

        public static Direction Reverse(Direction inDirection)
        {
            return (Direction)((int)inDirection + 0x04) & Direction.FacingMask;
        }
    }
}
