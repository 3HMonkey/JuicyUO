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
using JuicyUO.Core.Graphics;
using JuicyUO.Core.Input;
using JuicyUO.Core.UI;
using JuicyUO.Ultima.Data;
using JuicyUO.Ultima.UI.Controls;
using JuicyUO.Ultima.World;
#endregion

namespace JuicyUO.Ultima.UI.WorldGumps
{
    public class UseSpellButtonGump : Gump
    {
        // private variables
        private SpellDefinition m_Spell;
        private GumpPic m_SpellButton;
        // services
        private readonly WorldModel m_World;

        public UseSpellButtonGump(SpellDefinition spell)
            : base(spell.ID, 0)
        {
            while (UserInterface.GetControl<UseSpellButtonGump>(spell.ID) != null)
            {
                UserInterface.GetControl<UseSpellButtonGump>(spell.ID).Dispose();
            }

            m_Spell = spell;
            m_World = Service.Get<WorldModel>();

            IsMoveable = true;
            HandlesMouseInput = true;

            m_SpellButton = (GumpPic)AddControl(new GumpPic(this, 0, 0, spell.GumpIconSmallID, 0));
            m_SpellButton.HandlesMouseInput = true;
            m_SpellButton.MouseDoubleClickEvent += EventMouseDoubleClick;
        }

        public override void Dispose()
        {
            m_SpellButton.MouseDoubleClickEvent -= EventMouseDoubleClick;
            base.Dispose();
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position, double frameMS)
        {
            base.Draw(spriteBatch, position, frameMS);
        }

        private void EventMouseDoubleClick(AControl sender, int x, int y, MouseButton button)
        {
            if (button != MouseButton.Left)
                return;
            m_World.Interaction.CastSpell(m_Spell.ID);
        }
    }
}