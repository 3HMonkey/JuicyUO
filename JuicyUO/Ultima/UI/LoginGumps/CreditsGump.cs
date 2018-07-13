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
using JuicyUO.Core.Input;
using System.IO;
#endregion

namespace JuicyUO.Ultima.UI.LoginGumps
{
    public class CreditsGump : Gump {
        public CreditsGump()
            : base(0, 0) {
            AddControl(new GumpPicTiled(this, 0, 0, 800, 600, 0x0588));
            AddControl(new HtmlGumpling(this, 96, 64, 400, 400, 1, 1, ReadCreditsFile()));
            HandlesMouseInput = true;
        }

        protected override void OnMouseClick(int x, int y, MouseButton button) {
            Dispose();
        }

        private string ReadCreditsFile() {
            string path = @"Data/credits.txt";
            if (!File.Exists(path))
                return "<span color='#000'>Credits file not found.";
            try {
                string text = File.ReadAllText(@"Data\credits.txt");
                return text;
            }
            catch {
                return "<span color='#000'>Could not read credits file.";
            }

        }
    }
}
