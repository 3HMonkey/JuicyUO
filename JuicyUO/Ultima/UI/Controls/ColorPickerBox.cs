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
using Microsoft.Xna.Framework.Graphics;
using JuicyUO.Core.Graphics;
using JuicyUO.Core.Input;
using JuicyUO.Core.UI;
using JuicyUO.Core.Resources;
using JuicyUO.Ultima.Resources;
#endregion

namespace JuicyUO.Ultima.UI.Controls
{
    class ColorPickerBox : AControl
    {
        protected Texture2D m_huesTexture;
        protected Texture2D m_selectedIndicator;
        protected Rectangle m_openArea;

        protected int m_hueWidth, m_hueHeight;
        protected int[] m_hues;
        protected int m_Index;

        protected ColorPickerBox m_ChildColorPicker;
        private Texture2D m_Inactive;

        public int Index
        {
            get;
            set;
        }

        public bool IsChild;
        public ColorPickerBox ParentColorPicker;

        public int HueValue
        {
            get { return m_hues[Index]; }
            set
            {
                for (int i = 0; i < m_hues.Length; i++)
                {
                    if (value == m_hues[i])
                    {
                        Index = i;
                        break;
                    }
                }
            }
        }

        ColorPickerBox(AControl parent)
            : base(parent)
        {
            HandlesMouseInput = true;
        }

        public ColorPickerBox(AControl parent, Rectangle area, int swatchWidth, int swatchHeight, int[] hues)
            : this(parent)
        {
            BuildGumpling(area, swatchWidth, swatchHeight, hues);
        }

        public ColorPickerBox(AControl parent, Rectangle closedArea, Rectangle openArea, int swatchWidth, int swatchHeight, int[] hues, int index)
            : this(parent)
        {
            m_openArea = openArea;
            m_Index = index;
            BuildGumpling(closedArea, swatchWidth, swatchHeight, hues);
        }

        void BuildGumpling(Rectangle area, int swatchWidth, int swatchHeight, int[] hues)
        {
            m_hueWidth = swatchWidth;
            m_hueHeight = swatchHeight;
            Position = new Point(area.X, area.Y);
            Size = new Point(area.Width, area.Height);
            m_hues = hues;
            Index = m_Index;
        }

        protected override void OnInitialize()
        {
            if (m_huesTexture == null)
            {
                if (IsChild) // is a child
                {
                    m_huesTexture = HueData.CreateHueSwatch(m_hueWidth, m_hueHeight, m_hues);
                    IResourceProvider provider = Service.Get<IResourceProvider>();
                    m_selectedIndicator = provider.GetUITexture(6000);
                }
                else
                {
                    m_huesTexture = HueData.CreateHueSwatch(1, 1, new int[1] { m_hues[Index] });
                }
            }
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position, double frameMS)
        {
            IResourceProvider provider = Service.Get<IResourceProvider>();
            m_Inactive = provider.GetUITexture(210);

            spriteBatch.Draw2D(m_Inactive, new Vector3(position.X, position.Y, 0), Vector3.Zero);
            spriteBatch.Draw2D(m_huesTexture, new Rectangle(position.X+3, position.Y+3, Width-2, Height-1), Vector3.Zero);

            if (IsChild && IsMouseOver)
            {
                spriteBatch.Draw2D(m_selectedIndicator, new Vector3(
                    (int)(position.X + (float)(Width / m_hueWidth) * ((Index % m_hueWidth) + 0.5f) - m_selectedIndicator.Width / 2),
                    (int)(position.Y + (float)(Height / m_hueHeight) * ((Index / m_hueWidth) + 0.5f) - m_selectedIndicator.Height / 2),
                    0), Vector3.Zero);
            }
            base.Draw(spriteBatch, position, frameMS);
        }

        protected override void OnMouseClick(int x, int y, MouseButton button)
        {
            if (IsChild) // is a child
            {
                ParentColorPicker.Index = this.Index;
                ParentColorPicker.CloseChildPicker();
            }
            else
            {
                if (m_ChildColorPicker == null)
                {
                    m_ChildColorPicker = new ColorPickerBox(this.Parent, m_openArea, m_hueWidth, m_hueHeight, m_hues);
                    m_ChildColorPicker.IsChild = true;
                    m_ChildColorPicker.ParentColorPicker = this;
                    Parent.AddControl(m_ChildColorPicker, this.Page);
                }
                else
                {
                    m_ChildColorPicker.Dispose();
                    m_ChildColorPicker = null;
                }

            }
        }

        protected override void OnMouseOver(int x, int y)
        {
            if (IsChild)
            {
                int clickRow = x / (Width / m_hueWidth);
                int clickColumn = y / (Height / m_hueHeight);
                ParentColorPicker.Index = Index = clickRow + clickColumn * m_hueWidth;
            }
        }

        protected override void OnMouseOut(int x, int y)
        {
        }

        protected void CloseChildPicker()
        {
            if (m_ChildColorPicker != null)
            {
                m_ChildColorPicker.Dispose();
                m_ChildColorPicker = null;
                m_huesTexture = HueData.CreateHueSwatch(1, 1, new int[1] { m_hues[Index] });
            }
        }
    }
}
