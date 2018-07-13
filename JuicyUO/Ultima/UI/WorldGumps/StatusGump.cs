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
using JuicyUO.Core.UI;
using JuicyUO.Ultima.UI.Controls;
using JuicyUO.Ultima.World;
using JuicyUO.Ultima.World.Entities.Mobiles;
using JuicyUO.Core.Network;
using JuicyUO.Ultima.Network.Client;
using JuicyUO.Ultima.Player;
#endregion

namespace JuicyUO.Ultima.UI.WorldGumps
{
    class StatusGump : Gump
    {
        public static void Toggle(Serial serial)
        {
            UserInterfaceService ui = Service.Get<UserInterfaceService>();
            if (ui.GetControl<StatusGump>() == null)
            {
                INetworkClient client = Service.Get<INetworkClient>();
                client.Send(new MobileQueryPacket(MobileQueryPacket.StatusType.BasicStatus, serial));
                ui.AddControl(new StatusGump(), 200, 400);
            }
            else
                ui.RemoveControl<StatusGump>();
        }

        private Mobile m_Mobile = WorldModel.Entities.GetPlayerEntity();
        double m_RefreshTime;

        private TextLabelAscii[] m_Labels = new TextLabelAscii[(int)MobileStats.Max];

        private enum MobileStats
        {
            Name,
            Strength,
            Dexterity,
            Intelligence,
            HealthCurrent,
            HealthMax,
            StaminaCurrent,
            StaminaMax,
            ManaCurrent,
            ManaMax,
            Followers,
            WeightCurrent,
            WeightMax,
            StatCap,
            Luck,
            Gold,
            AR,
            RF,
            RC,
            RP,
            RE,
            Damage,
            Sex,
            Max
        }

        public StatusGump()
            : base(0, 0)
        {
            IsMoveable = true;

            if (PlayerState.ClientFeatures.AOS)
            {
                AddControl(new GumpPic(this, 0, 0, 0x2A6C, 0));

                m_Labels[(int)MobileStats.Name] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 54, 48, 1, 997, string.Format("<center>{0}", m_Mobile.Name))); // center doesn't work because textlabelascii shrinks to fit.
                m_Labels[(int)MobileStats.Strength] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 88, 76, 1, 997, m_Mobile.Strength.ToString()));
                m_Labels[(int)MobileStats.Dexterity] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 88, 104, 1, 997, m_Mobile.Dexterity.ToString()));
                m_Labels[(int)MobileStats.Intelligence] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 88, 132, 1, 997, m_Mobile.Intelligence.ToString()));

                m_Labels[(int)MobileStats.HealthCurrent] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 147, 71, 1, 997, m_Mobile.Health.Current.ToString()));
                m_Labels[(int)MobileStats.HealthMax] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 147, 82, 1, 997, m_Mobile.Health.Max.ToString()));

                m_Labels[(int)MobileStats.StaminaCurrent] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 147, 99, 1, 997, m_Mobile.Stamina.Current.ToString()));
                m_Labels[(int)MobileStats.StaminaMax] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 147, 110, 1, 997, m_Mobile.Stamina.Max.ToString()));

                m_Labels[(int)MobileStats.ManaCurrent] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 148, 127, 1, 997, m_Mobile.Mana.Current.ToString()));
                m_Labels[(int)MobileStats.ManaMax] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 148, 138, 1, 997, m_Mobile.Mana.Max.ToString()));

                m_Labels[(int)MobileStats.Followers] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 289, 132, 1, 997, ConcatCurrentMax(m_Mobile.Followers.Current, m_Mobile.Followers.Max)));

                m_Labels[(int)MobileStats.WeightCurrent] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 212, 127, 1, 997, m_Mobile.Weight.Current.ToString()));
                m_Labels[(int)MobileStats.WeightMax] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 212, 138, 1, 997, m_Mobile.Weight.Max.ToString()));

                m_Labels[(int)MobileStats.StatCap] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 219, 74, 1, 997, m_Mobile.StatCap.ToString()));
                m_Labels[(int)MobileStats.Luck] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 218, 102, 1, 997, m_Mobile.Luck.ToString()));
                m_Labels[(int)MobileStats.Gold] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 282, 102, 1, 997, m_Mobile.Gold.ToString()));

                m_Labels[(int)MobileStats.AR] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 352, 73, 1, 997, m_Mobile.ArmorRating.ToString()));
                m_Labels[(int)MobileStats.RF] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 352, 90, 1, 997, m_Mobile.ResistFire.ToString()));
                m_Labels[(int)MobileStats.RC] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 352, 105, 1, 997, m_Mobile.ResistCold.ToString()));
                m_Labels[(int)MobileStats.RP] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 352, 119, 1, 997, m_Mobile.ResistPoison.ToString()));
                m_Labels[(int)MobileStats.RE] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 352, 135, 1, 997, m_Mobile.ResistEnergy.ToString()));

                m_Labels[(int)MobileStats.Damage] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 277, 75, 1, 997, ConcatCurrentMax(m_Mobile.DamageMin, m_Mobile.DamageMax)));
            }
            else
            {
                AddControl(new GumpPic(this, 0, 0, 0x802, 0));

                m_Labels[(int)MobileStats.Name] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 84, 37, 6, 0, m_Mobile.Name));
                m_Labels[(int)MobileStats.Strength] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 84, 57, 6, 0, m_Mobile.Strength.ToString()));
                m_Labels[(int)MobileStats.Dexterity] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 84, 69, 6, 0, m_Mobile.Dexterity.ToString()));
                m_Labels[(int)MobileStats.Intelligence] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 84, 81, 6, 0, m_Mobile.Intelligence.ToString()));
                m_Labels[(int)MobileStats.Sex] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 84, 93, 6, 0, m_Mobile.Flags.IsFemale ? "F" : "M"));
                m_Labels[(int)MobileStats.AR] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 84, 105, 6, 0, m_Mobile.ArmorRating.ToString()));

                m_Labels[(int)MobileStats.HealthCurrent] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 172, 57, 6, 0, m_Mobile.Health.Current.ToString() + '/' + m_Mobile.Health.Max.ToString()));
                m_Labels[(int)MobileStats.ManaCurrent] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 172, 69, 6, 0, m_Mobile.Mana.Current.ToString() + '/' + m_Mobile.Mana.Max.ToString()));
                m_Labels[(int)MobileStats.StaminaCurrent] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 172, 81, 6, 0, m_Mobile.Stamina.Current.ToString() + '/' + m_Mobile.Stamina.Max.ToString()));
                m_Labels[(int)MobileStats.Gold] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 172, 93, 6, 0, m_Mobile.Gold.ToString()));
                m_Labels[(int)MobileStats.WeightCurrent] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 172, 105, 6, 0, m_Mobile.Weight.Current.ToString() + '/' + m_Mobile.Weight.Max.ToString()));
            }
        }

        protected override void OnInitialize()
        {
            SetSavePositionName("status");
            base.OnInitialize();
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (m_RefreshTime + 0.5d < totalMS) //need to update
            {
                m_RefreshTime = totalMS;
                // we can just set these without checking if they've changed.
                // The label will only update if the value has changed.
                if (PlayerState.ClientFeatures.AOS)
                {
                    m_Labels[(int)MobileStats.Name].Text = string.Format("<center>{0}", m_Mobile.Name);
                    m_Labels[(int)MobileStats.Strength].Text = m_Mobile.Strength.ToString();
                    m_Labels[(int)MobileStats.Dexterity].Text = m_Mobile.Dexterity.ToString();
                    m_Labels[(int)MobileStats.Intelligence].Text = m_Mobile.Intelligence.ToString();

                    m_Labels[(int)MobileStats.HealthCurrent].Text = m_Mobile.Health.Current.ToString();
                    m_Labels[(int)MobileStats.HealthMax].Text = m_Mobile.Health.Max.ToString();

                    m_Labels[(int)MobileStats.StaminaCurrent].Text = m_Mobile.Stamina.Current.ToString();
                    m_Labels[(int)MobileStats.StaminaMax].Text = m_Mobile.Stamina.Max.ToString();

                    m_Labels[(int)MobileStats.ManaCurrent].Text = m_Mobile.Mana.Current.ToString();
                    m_Labels[(int)MobileStats.ManaMax].Text = m_Mobile.Mana.Max.ToString();

                    m_Labels[(int)MobileStats.Followers].Text = ConcatCurrentMax(m_Mobile.Followers.Current, m_Mobile.Followers.Max);

                    m_Labels[(int)MobileStats.WeightCurrent].Text = m_Mobile.Weight.Current.ToString();
                    m_Labels[(int)MobileStats.WeightMax].Text = m_Mobile.Weight.Max.ToString();

                    m_Labels[(int)MobileStats.StatCap].Text = m_Mobile.StatCap.ToString();
                    m_Labels[(int)MobileStats.Luck].Text = m_Mobile.Luck.ToString();
                    m_Labels[(int)MobileStats.Gold].Text = m_Mobile.Gold.ToString();

                    m_Labels[(int)MobileStats.AR].Text = m_Mobile.ArmorRating.ToString();
                    m_Labels[(int)MobileStats.RF].Text = m_Mobile.ResistFire.ToString();
                    m_Labels[(int)MobileStats.RC].Text = m_Mobile.ResistCold.ToString();
                    m_Labels[(int)MobileStats.RP].Text = m_Mobile.ResistPoison.ToString();
                    m_Labels[(int)MobileStats.RE].Text = m_Mobile.ResistEnergy.ToString();

                    m_Labels[(int)MobileStats.Damage].Text = ConcatCurrentMax(m_Mobile.DamageMin, m_Mobile.DamageMax);
                }
                else
                {
                    m_Labels[(int)MobileStats.Name].Text = m_Mobile.Name;
                    m_Labels[(int)MobileStats.Strength].Text = m_Mobile.Strength.ToString();
                    m_Labels[(int)MobileStats.Dexterity].Text = m_Mobile.Dexterity.ToString();
                    m_Labels[(int)MobileStats.Intelligence].Text = m_Mobile.Intelligence.ToString();

                    m_Labels[(int)MobileStats.HealthCurrent].Text = ConcatCurrentMax(m_Mobile.Health.Current, m_Mobile.Health.Max);
                    m_Labels[(int)MobileStats.StaminaCurrent].Text = ConcatCurrentMax(m_Mobile.Stamina.Current, m_Mobile.Stamina.Max);
                    m_Labels[(int)MobileStats.ManaCurrent].Text = ConcatCurrentMax(m_Mobile.Mana.Current, m_Mobile.Mana.Max);

                    m_Labels[(int)MobileStats.WeightCurrent].Text = m_Mobile.Weight.Current.ToString();

                    m_Labels[(int)MobileStats.Gold].Text = m_Mobile.Gold.ToString();

                    m_Labels[(int)MobileStats.AR].Text = m_Mobile.ArmorRating.ToString();

                    m_Labels[(int)MobileStats.Sex].Text = m_Mobile.Flags.IsFemale ? "F" : "M";
                }
            }

            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position, double frameMS)
        {
            base.Draw(spriteBatch, position, frameMS);
        }

        private string ConcatCurrentMax(int min, int max)
        {
            return string.Format("{0}/{1}", min, max);
        }
    }
}