using Zepheus.FiestaLib;
using Zepheus.FiestaLib.Networking;
using Zepheus.Util;
using Zepheus.World.Networking;
using System.Linq;
using System;
using System.Collections.Generic;
using Zepheus.Database;

namespace Zepheus.World.Handlers
{
    public sealed class Handler14
    {
        [PacketHandler(CH14Type.PartyMatchingSystem)]
        public static void PartyMatchingSystem(WorldClient client, Packet packet)
        {
            sendPartyMatchingList(client);
        }
        
        public static void sendPartyMatchingList(WorldClient client)
        {
            using (var packet = new Packet(SH14Type.PartyMatchingSystem))
            {
                packet.WriteShort(0); // UNK
                packet.WriteShort(20);
                packet.WriteShort(1);
                packet.WriteInt(Program.Entity.PartyMatchingSystems.Count());
                foreach (PartyMatchingSystem v in Program.Entity.PartyMatchingSystems)
                {
                    packet.WriteInt(client.Character.ID); //unk
                    packet.WriteString(v.Name, 16);
                    packet.WriteString(v.Message, 128);
                    packet.WriteUShort(0); //unk
                }
                client.SendPacket(packet);
            }
        }

        [PacketHandler(CH14Type.PartyMatchingNewMessage)]
        public static void PartyMatchingSystemNewMessaage(WorldClient client, Packet packet)
        {
            WriteNewPartyMatchingMessage(client, packet);
        }

        public static void WriteNewPartyMatchingMessage(WorldClient client, Packet packet)
        {
            try
            {
                string inMessage;
                if (!packet.TryReadString(out inMessage, 128))
                {
                    Log.WriteLine(LogLevel.Error, "Party Matching Message not read propely from {0}", client.CharacterName);
                    return;
                }

                var latest = DateTime.Now.AddHours(1);

                if (Program.Entity.PartyMatchingSystems.Count(C => C.Datetime <= latest) != 0)
                {
                    foreach (PartyMatchingSystem party in Program.Entity.PartyMatchingSystems)
                    {
                        if (party.Datetime < latest)
                        {
                            Program.Entity.PartyMatchingSystems.DeleteObject(party);
                        }
                    }
                }

                if (Program.Entity.PartyMatchingSystems.Count(C => C.Owner == client.Character.ID) != 0)
                {
                    var deletept = Program.Entity.PartyMatchingSystems.First(x => x.Owner == client.Character.ID);
                    Program.Entity.PartyMatchingSystems.DeleteObject(deletept);
                }

                PartyMatchingSystem dbptmatch = new PartyMatchingSystem();
                dbptmatch.Owner = client.Character.ID;
                dbptmatch.Name = client.CharacterName;
                dbptmatch.Message = inMessage;
                dbptmatch.Datetime = DateTime.Now;
                Program.Entity.AddToPartyMatchingSystems(dbptmatch);
                Program.Entity.SaveChanges();
                sendPartyMatchingList(client);
            }catch(Exception ex)
            {
                Log.WriteLine(LogLevel.Debug, "PartyMatching something wrong -> {0} -> {1}", client.CharacterName, ex.ToString());
            }
        }


        [PacketHandler(CH14Type.PartyInviteToCharacter)]
        public static void PartyInviteToCharacter(WorldClient client, Packet packet)
        {
            string charname;
            if (!packet.TryReadString(out charname, 16))
            {
                Log.WriteLine(LogLevel.Error, "PartySystem :: Got unknown Party request from Character {0}", client.Character.Character.Name);
                return;
            }

            WorldClient otherclient = ClientManager.Instance.GetClientByCharname(charname);
            if (otherclient != null)
            {
                if (Program.Entity.Parties.Count(c => c.CharNo == otherclient.Character.Character.ID) == 1)
                {
                    using (var ppacket = new Packet(SH14Type.PartyInviteAnswer))
                    {
                        ppacket.WriteString(charname, 16);
                        ppacket.WriteByte(0);
                        ppacket.WriteShort(1220);
                        client.SendPacket(ppacket);
                    }
                }
                else
                {
                    using (var ppacket = new Packet(SH14Type.PartyRequestFromCharacter))
                    {
                        ppacket.WriteString(client.CharacterName, 16);
                        otherclient.SendPacket(ppacket);
                    }
                }
            }
            else
            {
                Log.WriteLine(LogLevel.Error, "PartySystem :: Client not found from {0}", client.CharacterName);
            }
        }

        [PacketHandler(CH14Type.PartyInviteFromCharacter)]
        public static void PartyInviteFromCharacter(WorldClient client, Packet packet)
        {
            //Charname => Who send the PT inv // Nexor
            //Client => Who got the request // xRapid
            string charname;
            if(!packet.TryReadString(out charname, 16))
            {
                Log.WriteLine(LogLevel.Error, "PartySystem :: Got unknown Character from {0}", client.CharacterName);
            }

            WorldClient otherclient = ClientManager.Instance.GetClientByCharname(charname);

            if (otherclient != null)
            {
                int PartyNo = 0;

                if (Program.Entity.Parties.Count() != 0)
                {
                    Party party;
                    party = Program.Entity.Parties.OrderByDescending(c => c.PartyNo).First();
                    PartyNo = (party.PartyNo + 1);
                }

                if (Program.Entity.Parties.Count(c => c.CharNo == otherclient.Character.Character.ID) == 1)
                {
                    Party party;
                    party = Program.Entity.Parties.First(c => c.CharNo == otherclient.Character.Character.ID);
                    PartyNo = party.PartyNo;

                    party = new Party();
                    party.CharNo = client.Character.Character.ID;
                    party.MasterNo = 0;
                    party.PartyNo = PartyNo;
                    party.PartyGroup = 1;
                    Program.Entity.AddToParties(party);
                    Program.Entity.SaveChanges();
                }
                else
                {
                    Party party = new Party();
                    party.CharNo = otherclient.Character.Character.ID;
                    party.MasterNo = 0;
                    party.PartyNo = PartyNo;
                    party.PartyGroup = 1;
                    Program.Entity.AddToParties(party);
                    Program.Entity.SaveChanges();

                    party = new Party();
                    party.CharNo = client.Character.Character.ID;
                    party.MasterNo = 1;
                    party.PartyNo = PartyNo;
                    party.PartyGroup = 1;
                    Program.Entity.AddToParties(party);
                    Program.Entity.SaveChanges();
                }

                //using (var ppacket = new Packet(SH14Type.PartyInvite))
                //{
                //    ppacket.WriteUShort(0);
                //    ppacket.WriteString(client.Character.Character.Name, 16);
                //    client.SendPacket(ppacket);
                //}

                using (var ppacket = new Packet(SH14Type.PartyInviteAnswer))
                {
                    ppacket.WriteString(charname, 16);
                    ppacket.WriteByte(otherclient.Character.Character.Job);
                    ppacket.WriteUShort(1217);
                    client.SendPacket(ppacket);
                }

                //All send to the request Person who send the request

                using (var ppacket = new Packet(SH14Type.PartyInvite))
                {
                    ppacket.WriteUShort(0);
                    ppacket.WriteString(charname, 16);
                    otherclient.SendPacket(ppacket);
                }

                SendPartyList(PartyNo);
            }
            else
            {
                Log.WriteLine(LogLevel.Error, "PartySystem :: Can not found requested client {0}", charname);
            }
        }

        [PacketHandler(CH14Type.PartyInviteDecline)]
        public static void PartyInviteDecline(WorldClient client, Packet packet)
        {
            string charname;
            if(!packet.TryReadString(out charname, 16))
            {
                Log.WriteLine(LogLevel.Error, "PartySystem :: Got unknown request from Client {0}", client.Character.Character.Name);
            }

            WorldClient otherclient = ClientManager.Instance.GetClientByCharname(charname);
            if (otherclient != null)
            {
                SendPartyInviteDecline(otherclient, client.Character.Character.Name);
            }
            else
            {
                Log.WriteLine(LogLevel.Error, "PartySystem :: Can not finde Client Handler (Offline?) for {0}", charname);
            }
        }

        public static void SendPartyInviteDecline(WorldClient client, string charactername)
        {
            using (var packet = new Packet(SH14Type.PartyInviteAnswer))
            {
                packet.WriteString(charactername, 16);
                packet.WriteByte(0);
                packet.WriteUShort(1222);
                client.SendPacket(packet);
            }
        }

        public static void SendPartyList(int PartyNo)
        {
            Packet ppacket = new Packet(SH14Type.PartyCharacterList);
            List<string> PartyMembers = new List<string>();
            int countofrows = Program.Entity.Parties.Where(x => x.PartyNo == PartyNo).Count();
            int currentrow = 1;
            ppacket.WriteByte(Convert.ToByte(countofrows));
            foreach (Party party in Program.Entity.Parties.Where(c => c.PartyNo == PartyNo).OrderBy(c => c.MasterNo))
            {
                Character character = Program.Entity.Characters.First(c => c.ID == party.CharNo);
                PartyMembers.Add(character.Name);
                ppacket.WriteString(character.Name, 16);
                if (currentrow != countofrows)
                {
                    ppacket.WriteByte(0);
                    currentrow++;
                }
                else
                    ppacket.WriteByte(1);
                ppacket.WriteByte(character.Job);
            }
            foreach (var party in PartyMembers)
            {
                WorldClient Partyclient = ClientManager.Instance.GetClientByCharname(party);
                if (Partyclient != null)
                {
                    Partyclient.SendPacket(ppacket);
                }
            }
        }

        [PacketHandler(CH14Type.LeaveParty)]
        public static void LeaveParty(WorldClient client, Packet packet)
        {
            Party party;
            party = Program.Entity.Parties.First(c => c.CharNo == client.Character.Character.ID);

            foreach (Party PartyListFromDB in Program.Entity.Parties.Where(c => c.PartyNo == party.PartyNo).OrderBy(c => c.MasterNo))
            {
                Character character;
                character = Program.Entity.Characters.First(c => c.ID == PartyListFromDB.CharNo);
                WorldClient Partyclient = ClientManager.Instance.GetClientByCharname(character.Name);
                using (var ppacket = new Packet(SH14Type.LeaveParty))
                {
                    ppacket.WriteString(client.Character.Character.Name, 16);
                    ppacket.WriteUShort(1281);
                    Partyclient.SendPacket(ppacket);
                }
            }

            if(Program.Entity.Parties.Where(c => c.PartyNo == party.PartyNo).Count() == 2){
                Party OtherClientParty;
                OtherClientParty = Program.Entity.Parties.First(c => c.CharNo != client.Character.Character.ID);
                Program.Entity.DeleteObject(OtherClientParty);
            }



            Program.Entity.DeleteObject(party);
            Program.Entity.SaveChanges();
        }
    }
}
