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
using JuicyUO.Core.Network.Packets;
using JuicyUO.Core.Resources;
#endregion

namespace JuicyUO.Ultima.Network.Server
{
    public class DeleteResultPacket : RecvPacket
    {
        public string Result
        {
            get
            {
                // get the resource provider
                IResourceProvider provider = Service.Get<IResourceProvider>();

                switch ((DeleteResultType)m_result)
                {
                    case DeleteResultType.PasswordInvalid:
                        return provider.GetString(3000018); // 3000018: That character password is invalid.
                    case DeleteResultType.CharNotExist:
                        return provider.GetString(3000019); // 3000019: That character does not exist.
                    case DeleteResultType.CharBeingPlayed:
                        return provider.GetString(3000020); // 3000020: That character is being played right now.
                    case DeleteResultType.CharTooYoung:
                        return provider.GetString(3000021); // 3000021: That character is not old enough to delete. The character must be 7 days old before it can be deleted.
                    case DeleteResultType.CharQueued:
                        return provider.GetString(3000022); // 3000022: That character is currently queued for backup and cannot be deleted.
                    case DeleteResultType.BadRequest:
                        return provider.GetString(3000023); // 3000023: Couldn't carry out your request.
                    default:
                        return "Could not delete character.";
                }
            }
        }
        // enum from RunUO. Other values may be possible
        enum DeleteResultType
        {
            PasswordInvalid, // never sent by RunUO
            CharNotExist,
            CharBeingPlayed,
            CharTooYoung,
            CharQueued, // never sent by RunUO
            BadRequest
        }

        readonly byte m_result;

        public DeleteResultPacket(PacketReader reader)
            : base(0x85, "Character Delete Result")
        {
            m_result = reader.ReadByte();
        }
    }
}
