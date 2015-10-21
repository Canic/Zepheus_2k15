using System;
using Zepheus.FiestaLib;
using Zepheus.FiestaLib.Networking;
using Zepheus.Services.DataContracts;
using Zepheus.Util;
using Zepheus.World.Data;
using Zepheus.World.Networking;

namespace Zepheus.World.Handlers
{
    public sealed class Handler3
    {
        [PacketHandler(CH3Type.WorldClientKey)]
        public static void TransferKey(WorldClient client, Packet packet)
        {
            string key;
            if (!packet.ReadSkip(18) || !packet.TryReadString(out key, 32))
            {
                Log.WriteLine(LogLevel.Warn, "Invalid connection request.");
                client.Disconnect();
                return;
            }
            ClientTransfer transfer = ClientManager.Instance.GetTransfer(key);
            if (transfer != null)
            {
                // Check if client does not connect from localhost or LAN, 
                // and if it's connecting from the correct IP.
                // When this check is not done, people can remote hack someone.
                if (!client.Host.StartsWith("127.0") && !client.Host.StartsWith("192.") && transfer.HostIP != client.Host)
                {
                    Log.WriteLine(LogLevel.Warn, "Remotehack from {0}", client.Host);
                    SendError(client, ServerError.INVALID_CREDENTIALS);
                }
                else
                {
                    if (ClientManager.Instance.RemoveTransfer(transfer.Hash) && (!Program.Maintenance || transfer.Admin > 0)) //admins can still login
                    {
                        client.Authenticated = true;
                        client.AccountID = transfer.AccountID;
                        client.Admin = transfer.Admin;
                        client.Username = transfer.Username;
                        client.lastPing = DateTime.Now; //this is so pongthread can start checking him
                        client.Pong = true;
                        client.AccountKey = key;
                        client.RandomID = MathUtils.RandomizeUShort(ushort.MaxValue);

                        Log.WriteLine(LogLevel.Debug, "{0} authenticated.", client.Username);
                        SendCharacterList(client);
                    }
                }
            }
            else
            {
                Log.WriteLine(LogLevel.Warn, "Invalid client authentication from {0}", client.Host);
                SendError(client, ServerError.INVALID_CREDENTIALS);
            }
        }

        [PacketHandler(CH3Type.BackToWorldSelect)]
        public static void BackToWorldSelect(WorldClient client, Packet packet)
        {
            SendBackToWorldSelect(client);
        }

        public static void SendBackToWorldSelect(WorldClient client)
        {
            using(Packet packet = new Packet(SH3Type.BackToWorldSelect))
            {
                packet.WriteUShort(7768);
                packet.WriteString(client.AccountKey);
                client.SendPacket(packet);
            }
        }

        public static void SendError(WorldClient client, ServerError error)
        {
            using (Packet pack = new Packet(SH3Type.Error))
            {
                pack.WriteShort((byte)error);
                client.SendPacket(pack);
            }
        }

        private static void SendCharacterList(WorldClient client)
        {
            if (!client.LoadCharacters())
            {
                SendError(client, ServerError.DATABASE_ERROR);
                return;
            }

            using (var packet = new Packet(SH3Type.CharacterList))
            {
                packet.WriteUShort(client.RandomID);
                packet.WriteByte((byte)client.Characters.Count);
                foreach (WorldCharacter ch in client.Characters.Values)
                {
                    PacketHelper.WriteBasicCharInfo(ch, packet);
                }
                client.SendPacket(packet);
            }
        }

        [PacketHandler(CH3Type.GameLogout)]
        public static void GameLogout(WorldClient client, Packet packet)
        {
            ClientManager.Instance.RemoveClient(client);
            SendCharacterList(client);
        }
    }
}
