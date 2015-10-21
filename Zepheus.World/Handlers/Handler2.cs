
using Zepheus.FiestaLib;
using Zepheus.FiestaLib.Networking;
using Zepheus.World.Networking;
using Zepheus.Util;
using System;
using System.Text;
using System.Globalization;

namespace Zepheus.World.Handlers
{
    public sealed class Handler2
    {
        //this is incorrect, somehow?
        [PacketHandler(CH2Type.Pong)]
        public static void Pong(WorldClient client, Packet packet)
        {
            client.Pong = true;
        }

        public static void SendPing(WorldClient client)
        {
            using (var packet = new Packet(SH2Type.Ping))
            {
                client.SendPacket(packet);
            }

            SendInterfaceClock(client);
        }

        [PacketHandler(CH2Type.BackToSeverSelect)]
        public static void UNK213(WorldClient client, Packet packet)
        {
            sendUnk14(client);
        }

        public static void sendUnk14(WorldClient client)
        {
            using (var packet = new Packet(SH2Type.Unk14))
            {
                packet.WriteHexAsBytes("07 2C 2A");
                client.SendPacket(packet);
            }
        }

        public static void SendInterfaceClock(WorldClient client)
        {
            DateTime dt = DateTime.Now;

            using (var packet = new Packet(SH2Type.InterfaceClock))
            {
                packet.WriteInt(DateTime.Now.DayOfYear); //unk
                packet.WriteInt(DateTime.Now.Minute);
                packet.WriteInt(DateTime.Now.Hour);
                packet.WriteInt(DateTime.Now.Day);
                packet.WriteInt((DateTime.Now.Month - 1));
                packet.WriteInt((DateTime.Now.Year - 1900));
                packet.WriteInt((int)DateTime.Now.DayOfWeek);
                packet.WriteInt((DateTime.Now.DayOfYear - 1));
                packet.Fill(4, 0); //unk
                packet.WriteSByte(Convert.ToSByte((DateTime.Now - DateTime.UtcNow).ToString().Split(':')[0])); //Timezone
                client.SendPacket(packet);
            }
        }
    }
}
