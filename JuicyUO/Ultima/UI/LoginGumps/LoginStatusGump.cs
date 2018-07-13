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
using JuicyUO.Core.Resources;
using System;
#endregion

namespace JuicyUO.Ultima.UI.LoginGumps {
    enum LoggingInGumpButtons {
        QuitButton,
        CancelLoginButton,
        OKNoLoginButton
    }

    class LoginStatusGump : Gump {
        public const int PageConnecting = 1;
        public const int PageCouldntConnect = 2;
        public const int PageIncorrectUsernamePassword = 3;
        public const int PageAccountInUse = 4;
        public const int PageAccountBlocked = 5;
        public const int PageCredentialsInvalid = 6;
        public const int PageConnectionLost = 7;
        public const int PageBadCommunication = 8;
        public const int PageVerifyingAccount = 9;

        event Action m_OnCancelLogin;

        public LoginStatusGump(Action onCancelLogin)
            : base(0, 0) {
            m_OnCancelLogin = onCancelLogin;

            // get the resource provider
            IResourceProvider provider = Service.Get<IResourceProvider>();

            int hue = 902;
            // backdrop
            AddControl(new GumpPicTiled(this, 0, 0, 800, 600, 9274));
            AddControl(new GumpPic(this, 0, 0, 5500, 0));
            // quit button
            AddControl(new Button(this, 554, 2, 5513, 5515, ButtonTypes.Activate, 0, (int)LoggingInGumpButtons.QuitButton));
            ((Button)LastControl).GumpOverID = 5514;
            // center message window backdrop
            AddControl(new ResizePic(this, 116, 95, 2600, 408, 288));

            // Page 1 - Connecting... with cancel login button
            AddControl(new TextLabelAscii(this, 166, 143, 2, hue, provider.GetString(3000002)), PageConnecting);
            AddControl(new Button(this, 305, 342, 1150, 1152, ButtonTypes.Activate, 0, (int)LoggingInGumpButtons.CancelLoginButton), PageConnecting);
            ((Button)LastControl).GumpOverID = 1151;

            // Page 2 - Couldn't connect to server
            AddControl(new TextLabelAsciiCropped(this, 166, 143, 308, 308, 2, hue, provider.GetString(3000016)), PageCouldntConnect);
            AddControl(new Button(this, 305, 342, 1153, 1155, ButtonTypes.Activate, 0, (int)LoggingInGumpButtons.OKNoLoginButton), PageCouldntConnect);
            ((Button)LastControl).GumpOverID = 1154;

            // Page 3 - Incorrect username and/or password.
            AddControl(new TextLabelAsciiCropped(this, 166, 143, 308, 308, 2, hue, provider.GetString(3000036)), PageIncorrectUsernamePassword);
            AddControl(new Button(this, 305, 342, 1153, 1155, ButtonTypes.Activate, 0, (int)LoggingInGumpButtons.OKNoLoginButton), PageIncorrectUsernamePassword);
            ((Button)LastControl).GumpOverID = 1154;

            // Page 4 - Someone is already using this account.
            AddControl(new TextLabelAsciiCropped(this, 166, 143, 308, 308, 2, hue, provider.GetString(3000034)), PageAccountInUse);
            AddControl(new Button(this, 305, 342, 1153, 1155, ButtonTypes.Activate, 0, (int)LoggingInGumpButtons.OKNoLoginButton), PageAccountInUse);
            ((Button)LastControl).GumpOverID = 1154;

            // Page 5 - Your account has been blocked / banned
            AddControl(new TextLabelAsciiCropped(this, 166, 143, 308, 308, 2, hue, provider.GetString(3000035)), PageAccountBlocked);
            AddControl(new Button(this, 305, 342, 1153, 1155, ButtonTypes.Activate, 0, (int)LoggingInGumpButtons.OKNoLoginButton), PageAccountBlocked);
            ((Button)LastControl).GumpOverID = 1154;

            // Page 6 - Your account credentials are invalid.
            AddControl(new TextLabelAsciiCropped(this, 166, 143, 308, 308, 2, hue, provider.GetString(3000036)), PageCredentialsInvalid);
            AddControl(new Button(this, 305, 342, 1153, 1155, ButtonTypes.Activate, 0, (int)LoggingInGumpButtons.OKNoLoginButton), PageCredentialsInvalid);
            ((Button)LastControl).GumpOverID = 1154;

            // Page 7 - Login idle period exceeded (I use "Connection lost")
            AddControl(new TextLabelAsciiCropped(this, 166, 143, 308, 308, 2, hue, provider.GetString(3000004)), PageConnectionLost);
            AddControl(new Button(this, 305, 342, 1153, 1155, ButtonTypes.Activate, 0, (int)LoggingInGumpButtons.OKNoLoginButton), PageConnectionLost);
            ((Button)LastControl).GumpOverID = 1154;

            // Page 8 - Communication problem.
            AddControl(new TextLabelAsciiCropped(this, 166, 143, 308, 308, 2, hue, provider.GetString(3000037)), PageBadCommunication);
            AddControl(new Button(this, 305, 342, 1153, 1155, ButtonTypes.Activate, 0, (int)LoggingInGumpButtons.OKNoLoginButton), PageBadCommunication);
            ((Button)LastControl).GumpOverID = 1154;

            // Page 9 - Verifying Account... with cancel login button
            AddControl(new TextLabelAscii(this, 166, 143, 2, hue, provider.GetString(3000003)), PageVerifyingAccount);
            AddControl(new Button(this, 305, 342, 1150, 1152, ButtonTypes.Activate, 0, (int)LoggingInGumpButtons.CancelLoginButton), PageVerifyingAccount);
            ((Button)LastControl).GumpOverID = 1151;

            IsUncloseableWithRMB = true;
        }

        public override void OnButtonClick(int buttonID) {
            switch ((LoggingInGumpButtons)buttonID) {
                case LoggingInGumpButtons.QuitButton:
                    Service.Get<UltimaGame>().Quit();
                    break;
                case LoggingInGumpButtons.CancelLoginButton:
                    m_OnCancelLogin();
                    break;
                case LoggingInGumpButtons.OKNoLoginButton:
                    m_OnCancelLogin();
                    break;
            }
        }
    }
}
