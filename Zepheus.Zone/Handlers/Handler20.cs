using Zepheus.FiestaLib;
using Zepheus.FiestaLib.Data;
using Zepheus.FiestaLib.Networking;
using Zepheus.Util;
using Zepheus.Zone.Data;
using Zepheus.Zone.Game;
using Zepheus.Zone.Networking;

namespace Zepheus.Zone.Handlers
{
    public sealed class Handler20
    {
        [PacketHandler(CH20Type.UseHPStone)]
        public static void UseHPStoneHandler(ZoneClient client, Packet packet)
        {
            if (client.Character.StonesHP == 0)
            {
                using (var p = new Packet(SH20Type.ErrorUseStone))
                {
                    client.SendPacket(p);
                }
            }
            else
            {
                client.Character.HealHP((uint)client.Character.BaseStats.SoulHP);

                using (var p = new Packet(SH20Type.StartHPStoneCooldown))
                {
                    client.SendPacket(p);
                }
            }
        }
        [PacketHandler(CH20Type.UseSPStone)]
        public static void UseSPStoneHandler(ZoneClient client, Packet packet)
        {
            if (client.Character.StonesSP == 0)
            {
                using (var p = new Packet(SH20Type.ErrorUseStone))
                {
                    client.SendPacket(p);
                }
            }
            else
            {
                client.Character.HealSP((uint)client.Character.BaseStats.SoulSP);

                using (var p = new Packet(SH20Type.StartSPStoneCooldown))
                {
                    client.SendPacket(p);
                }
            }
        }

        [PacketHandler(CH20Type.BuyHPStones)]
        public static void BuyHPStonesFromNPC(ZoneClient client, Packet packet)
        {
            short amount;
            ZoneCharacter character = client.Character;
            if (!packet.TryReadShort(out amount))
            {
                Log.WriteLine(LogLevel.Debug, "BuyHPStones :: Got unknown amount from {0}", character.Name);
            }

            using (var ppacket = new Packet(SH20Type.ChangeHPStones))
            {
                using (var pppacket = new Packet(SH4Type.MoneyChange))
                {
                    character.BuyHPStones(ppacket, pppacket, amount);
                    client.SendPacket(ppacket);
                    client.SendPacket(pppacket);
                }
            }
        }

        [PacketHandler(CH20Type.BuySPStones)]
        public static void BuySPStonesFromNPC(ZoneClient client, Packet packet)
        {
            short amount;
            ZoneCharacter character = client.Character;
            if (!packet.TryReadShort(out amount))
            {
                Log.WriteLine(LogLevel.Debug, "BuySPStones :: Got unknown amount from {0}", character.Name);
            }

            using (var ppacket = new Packet(SH20Type.ChangeSPStones))
            {
                using (var pppacket = new Packet(SH4Type.MoneyChange))
                {
                    character.BuySPStones(ppacket, pppacket, amount);
                    client.SendPacket(ppacket);
                    client.SendPacket(pppacket);
                }
            }
        }
    }
}
