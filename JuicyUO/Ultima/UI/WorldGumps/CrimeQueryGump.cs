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
using JuicyUO.Core.Network;
using JuicyUO.Ultima.Network.Client;
using JuicyUO.Ultima.UI.Controls;
using JuicyUO.Ultima.World.Entities.Mobiles;
#endregion

namespace JuicyUO.Ultima.UI.WorldGumps
{
    class CrimeQueryGump : Gump
    {
        public Mobile Mobile;

        public CrimeQueryGump(Mobile mobile)
            : base(0, 0)
        {
            Mobile = mobile;

            AddControl(new GumpPic(this, 0, 0, 0x0816, 0));
            AddControl(new TextLabelAscii(this, 40, 30, 1, 118, "This may flag you criminal!", 120));
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
                    INetworkClient m_Network = Service.Get<INetworkClient>();
                    m_Network.Send(new AttackRequestPacket(Mobile.Serial));
                    Dispose();
                    break;
            }
        }
    }
}
