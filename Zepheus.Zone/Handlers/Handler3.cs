
using Zepheus.FiestaLib;
using Zepheus.FiestaLib.Networking;
using Zepheus.Util;
using Zepheus.Zone.Game;
using Zepheus.Zone.Networking;

namespace Zepheus.Zone.Handlers
{
    public sealed class Handler3
    {
        public static void SendError(ZoneClient client, ServerError error)
        {
            using (Packet pack = new Packet(SH3Type.Error))
            {
                pack.WriteShort((byte)error);
                client.SendPacket(pack);
            }
        }

        [PacketHandler(CH3Type.GameLogout)]
        public static void GameLogout(ZoneClient client)
        {
            client.Disconnect();
        }
    }
}
