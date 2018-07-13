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

namespace JuicyUO.Ultima.World.Entities.Mobiles
{
    public class CurrentMaxValue
    {
        public int Current;
        public int Max;

        public CurrentMaxValue()
        {
            Current = 1;
            Max = 1;
        }

        public CurrentMaxValue(int current, int max)
        {
            Current = current;
            Max = max;
        }

        public void Update(int current, int max)
        {
            Current = current;
            Max = max;
        }

        public override string ToString()
        {
            return string.Format("{0} / {1}", Current, Max);
        }
    }
}