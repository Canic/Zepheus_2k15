using System;
using Zepheus.FiestaLib;
using Zepheus.FiestaLib.Networking;
using Zepheus.Util;
using Zepheus.World.Data;
using Zepheus.World.Networking;
using Zepheus.World.Security;
using Zepheus.Database;
using System.Linq;

namespace Zepheus.World.Handlers
{
    class Handler8
    {
        [PacketHandler(CH8Type.Whisper)]
        public static void getWhisperMessage(WorldClient client, Packet packet)
        {
            string toChar;
            byte len;
            string mess;
            if (!packet.TryReadString(out toChar, 16) || !packet.TryReadByte(out len) || !packet.TryReadString(out mess, len))
            {
                Log.WriteLine(LogLevel.Error, "WhisperChat :: Can not parse Whisper request from {0}", client.Character.Character.Name);
                return;
            }

            WorldClient wclient = ClientManager.Instance.GetClientByCharname(toChar);

            if (wclient != null)
            {
                using (var ppacket = new Packet(SH8Type.WhisperTo))
                {
                    ppacket.WriteString(toChar, 16);
                    ppacket.WriteByte(len);
                    ppacket.WriteString(mess, len);
                    client.SendPacket(ppacket);
                }

                using (var ppacket = new Packet(SH8Type.WhisperFrom))
                {
                    ppacket.WriteString(client.Character.Character.Name, 16);
                    ppacket.WriteByte(0); // unk
                    ppacket.WriteByte(len);
                    ppacket.WriteString(mess, len);
                    wclient.SendPacket(ppacket);
                }
            }
            else
            {
                using (var ppacket = new Packet(SH8Type.WhisperErrAnswer))
                {
                    ppacket.WriteUShort(3945);
                    ppacket.WriteString(toChar, 16);
                    client.SendPacket(ppacket);
                }
            }
        }

        [PacketHandler(CH8Type.PartyChat)]
        public static void sendPartyMessage(WorldClient client, Packet packet)
        {
            string mess;
            byte len;
            if (!packet.TryReadByte(out len) || !packet.TryReadString(out mess, len))
            {
                Log.WriteLine(LogLevel.Error, "PartyChat :: Can not parse party chat from {0}", client.Character.Character.Name);
                return;
            }

            if(Program.Entity.Parties.Where(c => c.CharNo == client.Character.Character.ID).Count() == 1)
            {
                Party getPartyInfo = Program.Entity.Parties.First(c => c.CharNo == client.Character.Character.ID);
                foreach (Party party in Program.Entity.Parties.Where(c => c.PartyNo == getPartyInfo.PartyNo))
                {
                    Character character = Program.Entity.Characters.First(c => c.ID == party.CharNo);
                    WorldClient wclient = ClientManager.Instance.GetClientByCharname(character.Name);
                    using (var ppacket = new Packet(SH8Type.PartyChat))
                    {
                        ppacket.WriteString(client.Character.Character.Name, 16);
                        ppacket.WriteByte(len);
                        ppacket.WriteString(mess, len);
                        wclient.SendPacket(ppacket);
                    }
                }
            }
            else
            {
                using (var ppacket = new Packet(SH8Type.PartyChatErr))
                {
                    ppacket.WriteUShort(1985);
                    client.SendPacket(ppacket);
                }
            }
        }
    }
}
