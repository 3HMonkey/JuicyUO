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
using Microsoft.Xna.Framework.Input;
using JuicyUO.Core.Graphics;
using JuicyUO.Core.Input;
using JuicyUO.Ultima.UI.Controls;
using JuicyUO.Ultima.World;
#endregion

namespace JuicyUO.Ultima.UI.WorldGumps
{
    public class DebugGump : Gump
    {
        readonly WorldModel m_World;
        HtmlGumpling m_Debug;

        public DebugGump()
            : base(0, 0)
        {
            m_World = Service.Get<WorldModel>();
            IsMoveable = true;
            AddControl(new ResizePic(this, 0, 0, 0x2436, 256 + 16, 256 + 16));
            AddControl(m_Debug = new HtmlGumpling(this, 0, 0, 256, 256, 0, 0, string.Empty));
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
            IInputService input = Service.Get<IInputService>();
            bool lmb = input.IsMouseButtonDown(MouseButton.Left);
            bool rmb = input.IsMouseButtonDown(MouseButton.Right);
            m_Debug.Text = $"{(lmb ? "LMB" : string.Empty)}:{(rmb ? "RMB" : string.Empty)}";
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position, double frameMS)
        {
            base.Draw(spriteBatch, position, frameMS);

            spriteBatch.Draw2D(((WorldView)m_World.GetView()).MiniMap.Texture, new Vector3(position.X + 8, position.Y + 8, 0), Vector3.Zero);
        }
    }
}