using Zepheus.FiestaLib;
using Zepheus.FiestaLib.Networking;
using Zepheus.Util;
using Zepheus.Zone.Game;
using Zepheus.Zone.Networking;


namespace Zepheus.Zone.Handlers
{
    class Handler19
    {
        [PacketHandler (CH19Type.TradeRequest)]
        public static void HandleTrade(ZoneClient client, Packet packet)
        {
            ushort PlayerObjID;
            if (!packet.TryReadUShort(out PlayerObjID))
            {
                Packet ppacket = new Packet(SH19Type.TradeNotAccepted);
                ppacket.WriteUShort(client.Character.MapObjectID);
                client.SendPacket(ppacket);
                Log.WriteLine(LogLevel.Error, "TradeRequest :: Invalid Obj ID from {0}", client.Character.Name);
            }

            SendTradeRequest(client, PlayerObjID);
        }

        public static void SendTradeRequest(ZoneClient client, ushort ObjID)
        {
            using(var packet = new Packet(SH19Type.TradeRequest))
            {
                ZoneClient otherclient = ClientManager.Instance.GetClientByObj(ObjID);
                packet.WriteUShort(client.Character.MapObjectID);
                otherclient.SendPacket(packet);
            }
        }
    }
}
