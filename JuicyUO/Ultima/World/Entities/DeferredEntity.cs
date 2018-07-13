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
using JuicyUO.Core.Diagnostics.Tracing;
using JuicyUO.Ultima.World.Entities.Effects;
using JuicyUO.Ultima.World.Entities.Mobiles;
using JuicyUO.Ultima.World.EntityViews;
using JuicyUO.Ultima.World.Entities.Items.Containers;

namespace JuicyUO.Ultima.World.Entities
{
    public class DeferredEntity : AEntity
    {
        Vector3 m_DrawPosition;
        AEntityView m_BaseView;

        public DeferredEntity(AEntity entity, Vector3 drawPosition, int z)
            : base(entity.Serial, entity.Map)
        {
            m_BaseView = GetBaseView(entity);
            m_DrawPosition = drawPosition;
            Position.Set(int.MinValue, int.MinValue, z);
        }

        AEntityView GetBaseView(AEntity entity)
        {
            if (entity is Mobile)
                return (MobileView)entity.GetView();
            if (entity is Corpse)
                return (MobileView)entity.GetView();
            if (entity is LightningEffect)
                return (LightningEffectView)entity.GetView();
            if (entity is AnimatedItemEffect)
                return (AnimatedItemEffectView)entity.GetView();
            if (entity is MovingEffect)
                return (MovingEffectView)entity.GetView();
            Tracer.Critical("Cannot defer this type of object.");
            return null;
        }

        protected override AEntityView CreateView()
        {
            return new DeferredView(this, m_DrawPosition, m_BaseView);
        }

        public override string ToString() => $"{base.ToString()} | deferred";
    }
}
