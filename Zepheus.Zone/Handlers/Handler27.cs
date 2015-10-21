using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zepheus.FiestaLib;
using Zepheus.FiestaLib.Networking;
using Zepheus.Zone.Game;

namespace Zepheus.Zone.Handlers
{
    class Handler27
    {
        public static void SendGameMessage(ZoneCharacter character, string Message, string script)
        {
            using (var packet = new Packet(SH27Type.GameMessage))
            {
                packet.WriteString(script, 31);
                packet.WriteByte(24);
                packet.WriteInt(Message.Length);
                packet.WriteByte(0);
                packet.WriteString(Message, Message.Length);
                character.Client.SendPacket(packet);
            }
        }
    }
}
