﻿#region license
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
using System;
using System.Collections.Generic;
using JuicyUO.Core.Diagnostics.Tracing;
using JuicyUO.Core.Input;
using JuicyUO.Core.Network;
using JuicyUO.Core.Resources;
using JuicyUO.Core.UI;
using JuicyUO.Ultima.Network.Client;
using JuicyUO.Ultima.Network.Server;
using JuicyUO.Ultima.UI.Controls;
using JuicyUO.Ultima.World.Entities;
using JuicyUO.Ultima.World.Entities.Items;
using JuicyUO.Ultima.World.Entities.Items.Containers;
using JuicyUO.Ultima.World.Entities.Mobiles;
#endregion

namespace JuicyUO.Ultima.UI.WorldGumps
{
    class VendorBuyGump : Gump
    {
        ExpandableScroll m_Background;
        IScrollBar m_ScrollBar;
        HtmlGumpling m_TotalCost;
        Button m_OKButton;
        int m_VendorSerial;
        VendorItemInfo[] m_Items;
        RenderedTextList m_ShopContents;

        MouseState m_MouseState = MouseState.None;
        int m_MouseDownOnIndex;
        double m_MouseDownMS;

        public VendorBuyGump(AEntity vendorBackpack, VendorBuyListPacket packet)
            : base(0, 0)
        {

            // sanity checking: don't show buy gumps for empty containers.
            if (!(vendorBackpack is ContainerItem) || ((vendorBackpack as ContainerItem).Contents.Count <= 0) || (packet.Items.Count <= 0))
            {
                base.Dispose();
                return;
            }

            IsMoveable = true;
            // note: original gumplings start at index 0x0870.
            AddControl(m_Background = new ExpandableScroll(this, 0, 0, 360, false));
            AddControl(new HtmlGumpling(this, 0, 6, 300, 20, 0, 0, " <center><span color='#004' style='font-family:uni0;'>Shop Inventory"));

            m_ScrollBar = (IScrollBar)AddControl(new ScrollFlag(this));
            AddControl(m_ShopContents = new RenderedTextList(this, 22, 32, 250, 260, m_ScrollBar));
            BuildShopContents(vendorBackpack, packet);

            AddControl(m_TotalCost = new HtmlGumpling(this, 44, 334, 200, 30, 0, 0, string.Empty));
            UpdateEntryAndCost();

            AddControl(m_OKButton = new Button(this, 220, 333, 0x907, 0x908, ButtonTypes.Activate, 0, 0));
            m_OKButton.GumpOverID = 0x909;
            m_OKButton.MouseClickEvent += okButton_MouseClickEvent;

        }

        public override void Dispose()
        {
            if (m_OKButton != null)
                m_OKButton.MouseClickEvent -= okButton_MouseClickEvent;
            base.Dispose();
        }

        void okButton_MouseClickEvent(AControl control, int x, int y, MouseButton button)
        {
            if (button != MouseButton.Left)
                return;

            List<Tuple<int, short>> itemsToBuy = new List<Tuple<int, short>>();
            for (int i = 0; i < m_Items.Length; i++)
            {
                if (m_Items[i].AmountToBuy > 0)
                {
                    itemsToBuy.Add(new Tuple<int, short>(m_Items[i].Item.Serial, (short)m_Items[i].AmountToBuy));
                }
            }

            if (itemsToBuy.Count == 0)
                return;

            INetworkClient network = Service.Get<INetworkClient>();
            network.Send(new BuyItemsPacket(m_VendorSerial, itemsToBuy.ToArray()));
            this.Dispose();
        }

        public override void Update(double totalMS, double frameMS)
        {
            m_ShopContents.Height = Height - 69;
            base.Update(totalMS, frameMS);

            if (m_MouseState != MouseState.None)
            {
                m_MouseDownMS += frameMS;
                if (m_MouseDownMS >= 350d)
                {
                    m_MouseDownMS -= 120d;
                    if (m_MouseState == MouseState.MouseDownOnAdd)
                    {
                        AddItem(m_MouseDownOnIndex);
                    }
                    else
                    {
                        RemoveItem(m_MouseDownOnIndex);
                    }
                }
            }
        }

        void BuildShopContents(AEntity vendorBackpack, VendorBuyListPacket packet)
        {

            if (!(vendorBackpack is ContainerItem))
            {
                m_ShopContents.AddEntry("<span color='#800'>Err: vendorBackpack is not Container.");
                return;
            }

            ContainerItem contents = (vendorBackpack as ContainerItem);
            AEntity vendor = contents.Parent;
            if (vendor == null || !(vendor is Mobile))
            {
                m_ShopContents.AddEntry("<span color='#800'>Err: vendorBackpack item does not belong to a vendor Mobile.");
                return;
            }
            m_VendorSerial = vendor.Serial;

            m_Items = new VendorItemInfo[packet.Items.Count];

            for (int i = 0; i < packet.Items.Count; i++)
            {
                Item item = contents.Contents[packet.Items.Count - 1 - i];
                if (item.Amount > 0)
                {
                    string cliLocAsString = packet.Items[i].Description;
                    int price = packet.Items[i].Price;

                    int clilocDescription;
                    string description;
                    if (!(int.TryParse(cliLocAsString, out clilocDescription)))
                    {
                        description = cliLocAsString;
                    }
                    else
                    {
                        // get the resource provider
                        IResourceProvider provider = Service.Get<IResourceProvider>();
                        description = Utility.CapitalizeAllWords(provider.GetString(clilocDescription));
                    }

                    string html = string.Format(c_Format, description, price.ToString(), item.DisplayItemID, item.Amount, i);
                    m_ShopContents.AddEntry(html);

                    m_Items[i] = new VendorItemInfo(item, description, price, item.Amount);
                }
            }

            // list starts displaying first item.
            m_ScrollBar.Value = 0;
        }

        const string c_Format =
            "<right><a href='add={4}'><gumpimg src='0x9CF'/></a><div width='4'/><a href='remove={4}'><gumpimg src='0x9CE'/></a></right>" +
            "<left><itemimg src='{2}'  width='52' height='44'/></left><left><span color='#400'>{0}<br/>{1}gp, {3} available.</span></left>";

        public override void OnHtmlInputEvent(string href, MouseEvent e)
        {
            string[] hrefs = href.Split('=');
            bool isAdd;
            int index;

            // parse add/remove
            if (hrefs[0] == "add")
            {
                isAdd = true;
            }
            else if (hrefs[0] == "remove")
            {
                isAdd = false;
            }
            else
            {
                Tracer.Error("Bad HREF in VendorBuyGump: {0}", href);
                return;
            }

            // parse item index
            if (!(int.TryParse(hrefs[1], out index)))
            {
                Tracer.Error("Unknown vendor item index in VendorBuyGump: {0}", href);
                return;
            }

            if (e == MouseEvent.Down)
            {
                if (isAdd)
                {
                    AddItem(index);
                }
                else
                {
                    RemoveItem(index);
                }
                m_MouseState = isAdd ? MouseState.MouseDownOnAdd : MouseState.MouseDownOnRemove;
                m_MouseDownMS = 0;
                m_MouseDownOnIndex = index;
            }
            else if (e == MouseEvent.Up)
            {
                m_MouseState = MouseState.None;
            }

            UpdateEntryAndCost(index);
        }

        void AddItem(int index)
        {
            if (m_Items[index].AmountToBuy < m_Items[index].AmountTotal)
                m_Items[index].AmountToBuy++;
            UpdateEntryAndCost(index);
        }

        void RemoveItem(int index)
        {
            if (m_Items[index].AmountToBuy > 0)
                m_Items[index].AmountToBuy--;
            UpdateEntryAndCost(index);
        }

        void UpdateEntryAndCost(int index = -1)
        {
            if (index >= 0)
            {
                m_ShopContents.UpdateEntry(index, string.Format(c_Format,
                    m_Items[index].Description,
                    m_Items[index].Price.ToString(),
                    m_Items[index].Item.DisplayItemID,
                    m_Items[index].AmountTotal - m_Items[index].AmountToBuy, index));
            }

            int totalCost = 0;
            if (m_Items != null)
            {
                for (int i = 0; i < m_Items.Length; i++)
                {
                    totalCost += m_Items[i].AmountToBuy * m_Items[i].Price;
                }
            }
            m_TotalCost.Text = string.Format("<span style='font-family:uni0;' color='#008'>Total: </span><span color='#400'>{0}gp</span>", totalCost);
        }

        class VendorItemInfo
        {
            public readonly Item Item;
            public readonly string Description;
            public readonly int Price;
            public readonly int AmountTotal;
            public int AmountToBuy;

            public VendorItemInfo(Item item, string description, int price, int amount)
            {
                Item = item;
                Description = description;
                Price = price;
                AmountTotal = amount;
                AmountToBuy = 0;
            }
        }

        enum MouseState
        {
            None,
            MouseDownOnAdd,
            MouseDownOnRemove
        }
    }
}
