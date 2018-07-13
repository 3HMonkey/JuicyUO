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
using System.Text;
using JuicyUO.Core.Network;
using JuicyUO.Core.Resources;
using JuicyUO.Core.UI.Fonts;
using JuicyUO.Ultima.Data;
using JuicyUO.Ultima.Network.Client;
using JuicyUO.Ultima.UI.Controls;
using JuicyUO.Core.Input;
#endregion

namespace JuicyUO.Ultima.UI.WorldGumps
{
    /// <summary>
    /// A context menu with a number of choices.
    /// </summary>
    class ContextMenuGump : Gump
    {
        private readonly ContextMenuData m_Data;

        private readonly ResizePic m_Background;
        private readonly HtmlGumpling m_MenuItems;

        public ContextMenuGump(ContextMenuData data)
            : base(0, 0)
        {
            MetaData.IsModal = true;
            MetaData.ModalClickOutsideAreaClosesThisControl = true;

            m_Data = data;

            IResourceProvider provider = Service.Get<IResourceProvider>();
            AFont font = (AFont)provider.GetUnicodeFont(1);

            m_Background = (ResizePic)AddControl(new ResizePic(this, 0, 0, 0x0A3C, 50, font.Height * m_Data.Count + 20));

            StringBuilder htmlContextItems = new StringBuilder();
            for (int i = 0; i < m_Data.Count; i++)
            {
                htmlContextItems.Append(string.Format("<a href='{0}' color='#DDD' hovercolor='#FFF' activecolor='#BBB' style='text-decoration:none;'>{1}</a><br/>", m_Data[i].ResponseCode, m_Data[i].Caption));
            }
            m_MenuItems = (HtmlGumpling)AddControl(new HtmlGumpling(this, 10, 10, 200, font.Height * m_Data.Count, 0, 0, htmlContextItems.ToString()));
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
            m_Background.Width = m_MenuItems.Width + 20;
        }

        protected override void OnMouseOut(int x, int y)
        {
            // Dispose();
        }

        public override void OnHtmlInputEvent(string href, MouseEvent e)
        {
            if (e != MouseEvent.Click)
                return;

            int contextMenuItemSelected;
            if (int.TryParse(href, out contextMenuItemSelected))
            {
                INetworkClient network = Service.Get<INetworkClient>();
                network.Send(new ContextMenuResponsePacket(m_Data.Serial, (short)contextMenuItemSelected));
                this.Dispose();
            }
        }
    }
}
