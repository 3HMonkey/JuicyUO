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
using System.Collections.Generic;
using JuicyUO.Ultima.World.Entities;
using JuicyUO.Ultima.World.Entities.Effects;
using JuicyUO.Ultima.World.Entities.Items;
using JuicyUO.Ultima.World.Entities.Mobiles;
#endregion

namespace JuicyUO.Ultima.World.Maps
{
    static class TileSorter
    {
        public static void Sort(List<AEntity> items)
        {
            for (int i = 0; i < items.Count - 1; i++)
            {
                int j = i + 1;

                while (j > 0)
                {
                    int result = Compare(items[j - 1], items[j]);
                    if (result > 0)
                    {
                        AEntity temp = items[j - 1];
                        items[j - 1] = items[j];
                        items[j] = temp;

                    }
                    j--;
                }
            }
        }

        public static int Compare(AEntity x, AEntity y)
        {
            int xZ, xType, xThreshold, xTiebreaker;
            int yZ, yType, yThreshold, yTiebreaker;

            GetSortValues(x, out xZ, out xType, out xThreshold, out xTiebreaker);
            GetSortValues(y, out yZ, out yType, out yThreshold, out yTiebreaker);

            xZ += xThreshold;
            yZ += yThreshold;

            int comparison = xZ - yZ;
            if (comparison == 0)
            {
                comparison = xType - yType;
            }
            if (comparison == 0)
            {
                comparison = xThreshold - yThreshold;
            }
            if (comparison == 0)
            {
                comparison = xTiebreaker - yTiebreaker;
            }
            return comparison;
        }

        public static void GetSortValues(AEntity e, out int z, out int type, out int threshold, out int tiebreaker)
        {
            if (e is AEffect)
            {
                AEffect effect = e as AEffect;
                z = effect.Z;
                type = 4;
                threshold = 2;
                tiebreaker = 0;
            }
            else if (e is DeferredEntity)
            {
                DeferredEntity mobile = (DeferredEntity)e;
                z = mobile.Z;
                type = 2;
                threshold = 1;
                tiebreaker = 0;
            }
            else if (e is Mobile)
            {
                Mobile mobile = (Mobile)e;
                z = mobile.Z;
                type = mobile.IsSitting ? 0 : 3;
                threshold = 2;
                tiebreaker = mobile.IsClientEntity ? 0x40000000 : (int)mobile.Serial;
            }
            else if (e is Ground)
            {
                Ground tile = (Ground)e;
                z = tile.GetView().SortZ;
                type = 0;
                threshold = 0;
                tiebreaker = 0;
            }
            else if (e is StaticItem)
            {
                StaticItem item = (StaticItem)e;
                z = item.Z;
                type = 1;
                threshold = (item.ItemData.Height > 0 ? 1 : 0) + (item.ItemData.IsBackground ? 0 : 1);
                tiebreaker = item.SortInfluence;
            }
            else if (e is Item)
            {
                Item item = (Item)e;
                z = item.Z;
                type = ((item.ItemID & 0x3fff) == 0x2006) ? 4 : 2; // corpses show on top of mobiles and items.
                threshold = (item.ItemData.Height > 0 ? 1 : 0) + (item.ItemData.IsBackground ? 0 : 1);
                tiebreaker = item.Serial;
            }
            else
            {
                z = 0;
                threshold = 0;
                type = 0;
                tiebreaker = 0;
            }
        }
    }
}
