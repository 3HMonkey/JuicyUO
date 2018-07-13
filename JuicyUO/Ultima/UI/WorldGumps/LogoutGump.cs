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
using JuicyUO.Ultima.UI.Controls;
using JuicyUO.Ultima.World;
#endregion

namespace JuicyUO.Ultima.UI.WorldGumps
{
    class LogoutGump : Gump
    {
        public LogoutGump()
            : base(0, 0)
        {
            AddControl(new GumpPic(this, 0, 0, 0x0816, 0));
            AddControl(new TextLabelAscii(this, 40, 30, 1, 118, "Quit\nUltima Online?"));
            ((TextLabelAscii)LastControl).Hue = 997;
            AddControl(new Button(this, 40, 77, 0x817, 0x818, ButtonTypes.Activate, 0, 0));
            AddControl(new Button(this, 100, 77, 0x81A, 0x81B, ButtonTypes.Activate, 1, 1));

            IsMoveable = false;
            MetaData.IsModal = true;
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
            CenterThisControlOnScreen();
        }

        public override void OnButtonClick(int buttonID)
        {
            switch (buttonID)
            {
                case 0:
                    Dispose();
                    break;
                case 1:
                    WorldModel world = Service.Get<WorldModel>();
                    world.Disconnect();
                    Dispose();
                    break;
            }
        }
    }
}
