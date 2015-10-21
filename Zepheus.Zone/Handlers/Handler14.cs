using System;
using Zepheus.FiestaLib;
using Zepheus.FiestaLib.Data;
using Zepheus.FiestaLib.Networking;
using Zepheus.Services.DataContracts;
using Zepheus.Util;
using Zepheus.Zone.Data;
using Zepheus.Zone.Game;
using Zepheus.Zone.Networking;
using Zepheus.Database;
using System.Linq;
using System.Collections.Generic;

namespace Zepheus.Zone.Handlers
{
    class Handler14
    {
        [PacketHandler(CH14Type.PartyInformation)]
        public static void PartyInformation(ZoneClient client, Packet packet)
        {
            SendPartyInformation(client);
        }

        public static void SendPartyInformation(ZoneClient client)
        {
            if (Program.Entity.Parties.Where(c => c.CharNo == client.Character.ID).Count() == 1)
            {
                Party PartyNo = Program.Entity.Parties.First(c => c.CharNo == client.Character.ID);
                foreach (Party party in Program.Entity.Parties.Where(c => c.PartyNo == PartyNo.PartyNo))
                {
                    Character Pcharacter = Program.Entity.Characters.First(c => c.ID == party.CharNo);
                    ZoneClient otherLiveInfo = ClientManager.Instance.GetClientByName(Pcharacter.Name);
                    if (otherLiveInfo != null)
                    {
                        if (otherLiveInfo.Character.MapID == client.Character.MapID)
                        {
                            using (var ppacket = new Packet(SH14Type.PartyInformationShort))
                            {
                                ppacket.WriteByte(1);
                                ppacket.WriteString(otherLiveInfo.Character.Name, 16);
                                ppacket.WriteUInt(otherLiveInfo.Character.HP);
                                ppacket.WriteUInt(otherLiveInfo.Character.SP);
                                ppacket.WriteUInt(otherLiveInfo.Character.LP);
                                client.SendPacket(ppacket);
                            }

                            using (var ppacket = new Packet(SH14Type.PartyInformation))
                            {
                                ppacket.WriteByte(1); //unk
                                ppacket.WriteString(otherLiveInfo.Character.Name, 16);
                                ppacket.WriteByte((byte)otherLiveInfo.Character.Job);
                                ppacket.WriteByte(otherLiveInfo.Character.Level);
                                ppacket.WriteUInt(otherLiveInfo.Character.HP);
                                ppacket.WriteUInt(otherLiveInfo.Character.SP);
                                ppacket.WriteUInt(otherLiveInfo.Character.LP);
                                ppacket.WriteByte(1); //unk
                                client.SendPacket(ppacket);
                            }

                            using (var ppacket = new Packet(SH14Type.PartyLoginCord))
                            {
                                ppacket.WriteByte(1); //unk
                                ppacket.WriteString(otherLiveInfo.Character.Name, 16);
                                ppacket.WriteInt(otherLiveInfo.Character.Position.X);
                                ppacket.WriteInt(otherLiveInfo.Character.Position.Y);
                                client.SendPacket(ppacket);
                            }
                        }
                    }
                }
            }
        }

        public static void SendPartyMemberCordChange(ZoneClient client)
        {
            if (Program.Entity.Parties.Where(c => c.CharNo == client.Character.ID).Count() == 1)
            {
                Party PartyNo = Program.Entity.Parties.First(c => c.CharNo == client.Character.ID);
                foreach (Party party in Program.Entity.Parties.Where(c => c.PartyNo == PartyNo.PartyNo))
                {
                    using (var ppacket = new Packet(SH14Type.PartyLoginCord))
                    {
                        Character Pcharacter = Program.Entity.Characters.First(c => c.ID == party.CharNo);
                        ZoneClient otherLiveInfo = ClientManager.Instance.GetClientByName(Pcharacter.Name);
                        if (otherLiveInfo != null)
                        {
                            if (otherLiveInfo.Character.MapID == client.Character.MapID)
                            {
                                ppacket.WriteByte(1); //unk
                                ppacket.WriteString(client.Character.Name, 16);
                                ppacket.WriteInt(client.Character.Position.X);
                                ppacket.WriteInt(client.Character.Position.Y);
                                otherLiveInfo.SendPacket(ppacket);
                            }
                        }
                    }
                }
            }
        }

        public static void SendPartyMemberHPSPLP(ZoneCharacter character)
        {
            if (Program.Entity.Parties.Where(c => c.CharNo == character.Client.Character.ID).Count() == 1)
            {
                Party PartyNo = Program.Entity.Parties.First(c => c.CharNo == character.Client.Character.ID);
                foreach (Party party in Program.Entity.Parties.Where(c => c.PartyNo == PartyNo.PartyNo))
                {
                    Character Pcharacter = Program.Entity.Characters.First(c => c.ID == party.CharNo);
                    ZoneClient otherLiveInfo = ClientManager.Instance.GetClientByName(Pcharacter.Name);
                    if (otherLiveInfo != null)
                    {
                        if (otherLiveInfo.Character.MapID == character.Client.Character.MapID)
                        {
                            using (var ppacket = new Packet(SH14Type.PartyInformationShort))
                            {
                                ppacket.WriteByte(1);
                                ppacket.WriteString(character.Client.Character.Name, 16);
                                ppacket.WriteUInt(character.Client.Character.HP);
                                ppacket.WriteUInt(character.Client.Character.SP);
                                ppacket.WriteUInt(character.Client.Character.LP);
                                otherLiveInfo.SendPacket(ppacket);
                            }
                        }
                    }
                }
            }
        }
    }
}
