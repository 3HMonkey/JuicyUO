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
using JuicyUO.Core.Resources;
using JuicyUO.Core.UI;
#endregion

namespace JuicyUO.Ultima.UI.Controls
{
    /// <summary>
    /// A base scrollbar with methods that control min, max, and value
    /// </summary>
    class ScrollFlag : AControl, IScrollBar
    {
        // ============================================================================================================
        // Private variables
        // ============================================================================================================
        private Texture2D m_GumpSlider;

        private int m_SliderExtentTop, m_SliderExtentHeight;
        private float m_SliderPosition;
        private float m_Value;
        private int m_Max, m_Min;

        private bool m_BtnSliderClicked;
        private Point m_ClickPosition;

        // ============================================================================================================
        // Public properties
        // ============================================================================================================
        public int Value
        {
            get
            {
                return (int)m_Value;
            }
            set
            {
                m_Value = value;
                if (m_Value < MinValue)
                    m_Value = MinValue;
                if (m_Value > MaxValue)
                    m_Value = MaxValue;
            }
        }

        public int MinValue
        {
            get
            {
                return m_Min;
            }
            set
            {
                m_Min = value;
                if (m_Value < m_Min)
                    m_Value = m_Min;
            }
        }

        public int MaxValue
        {
            get
            {
                return m_Max;
            }
            set
            {
                if (value < 0)
                    value = 0;
                m_Max = value;
                if (m_Value > m_Max)
                    m_Value = m_Max;
            }
        }

        // ============================================================================================================
        // Ctor, Initialize, Update, and Draw
        // ============================================================================================================
        public ScrollFlag(AControl parent)
            : base(parent)
        {
            HandlesMouseInput = true;
        }

        public ScrollFlag(AControl parent, int x, int y, int height, int minValue, int maxValue, int value)
            : this(parent)
        {
            Position = new Point(x, y);
            m_SliderExtentTop = y;
            m_SliderExtentHeight = height;

            MinValue = minValue;
            MaxValue = maxValue;
            Value = value;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            IResourceProvider provider = Service.Get<IResourceProvider>();
            m_GumpSlider = provider.GetUITexture(0x0828);
            Size = new Point(m_GumpSlider.Width, m_GumpSlider.Height);
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
            
            if (MaxValue <= MinValue || MinValue >= MaxValue)
            {
                Value = MaxValue = MinValue;
            }

            m_SliderPosition = CalculateSliderYPosition();
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position, double frameMS)
        {
            // draw slider
            if (MaxValue == MinValue)
            {
                // do nothing.
            }
            else
            {
                spriteBatch.Draw2D(m_GumpSlider, new Vector3(position.X - 5, position.Y + m_SliderPosition, 0), Vector3.Zero);
            }

            base.Draw(spriteBatch, position, frameMS);
        }

        private float CalculateSliderYPosition()
        {
            if (!IsInitialized)
                return 0f;
            if (MaxValue - MinValue == 0)
                return 0f;
            return CalculateScrollableArea() * ((m_Value - MinValue) / (MaxValue - MinValue));
        }

        private float CalculateScrollableArea()
        {
            if (!IsInitialized)
                return 0f;
            return Height - m_GumpSlider.Height;
        }

        protected override bool IsPointWithinControl(int x, int y)
        {
            x -= 5;
            Rectangle slider = new Rectangle(0, (int)m_SliderPosition, m_GumpSlider.Width, m_GumpSlider.Height);
            return slider.Contains(x, y);
        }

        protected override void OnMouseDown(int x, int y, MouseButton button)
        {
            if (IsPointWithinControl(x, y))
            {
                // clicked on the slider
                m_BtnSliderClicked = true;
                m_ClickPosition = new Point(x, y);
            }
        }

        protected override void OnMouseUp(int x, int y, MouseButton button)
        {
            m_BtnSliderClicked = false;
        }

        protected override void OnMouseOver(int x, int y)
        {
            if (m_BtnSliderClicked)
            {
                if (y != m_ClickPosition.Y)
                {
                    float sliderY = m_SliderPosition + (y - m_ClickPosition.Y);

                    if (sliderY < 0)
                        sliderY = 0;

                    float scrollableArea = CalculateScrollableArea();
                    if (sliderY > scrollableArea)
                        sliderY = scrollableArea;

                    m_ClickPosition = new Point(x, y);

                    m_Value = ((sliderY / scrollableArea) * (float)((MaxValue - MinValue))) + MinValue;
                    m_SliderPosition = sliderY;
                }
            }
        }

        public bool PointWithinControl(int x, int y)
        {
            return IsPointWithinControl(x, y);
        }
    }
}
