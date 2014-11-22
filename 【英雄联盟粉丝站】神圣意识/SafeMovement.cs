using System;
using System.IO;
using LeagueSharp;
using LeagueSharp.Common;

namespace SAwareness
{
    internal class SafeMovement
    {
        private decimal _lastSend;

        public SafeMovement()
        {
            Game.OnGameSendPacket += Game_OnGameSendPacket;
        }

        ~SafeMovement()
        {
            Game.OnGameSendPacket -= Game_OnGameSendPacket;
        }

        public bool IsActive()
        {
            return Menu.Misc.GetActive() && Menu.SafeMovement.GetActive();
        }

        private void Game_OnGameSendPacket(GamePacketEventArgs args)
        {
            if (!IsActive())
                return;

            try
            {
                decimal milli = DateTime.Now.Ticks/(decimal) TimeSpan.TicksPerMillisecond;
                var reader = new BinaryReader(new MemoryStream(args.PacketData));
                byte packetId = reader.ReadByte();
                if (packetId != Packet.C2S.Move.Header)
                    return;
                Packet.C2S.Move.Struct move = Packet.C2S.Move.Decoded(args.PacketData);
                if (move.MoveType == 2)
                {
                    if (move.SourceNetworkId == ObjectManager.Player.NetworkId)
                    {
                        if (milli - _lastSend <
                            Menu.SafeMovement.GetMenuItem("SAwarenessSafeMovementBlockIntervall")
                                .GetValue<Slider>()
                                .Value)
                        {
                            args.Process = false;
                        }
                        else
                        {
                            _lastSend = milli;
                        }
                    }
                }
                else if (move.MoveType == 3)
                {
                    _lastSend = 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("MovementProcess: " + ex);
            }
        }
    }
}