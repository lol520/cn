using System;
using System.IO;
using LeagueSharp;
using LeagueSharp.Common;

namespace SAwareness
{
    internal class SurrenderVote
    {
        private int _lastNoVoteCount;

        public SurrenderVote()
        {
            Game.OnGameProcessPacket += Game_OnGameProcessPacket;
        }

        ~SurrenderVote()
        {
            Game.OnGameProcessPacket -= Game_OnGameProcessPacket;
        }

        public bool IsActive()
        {
            return Menu.Misc.GetActive() && Menu.SurrenderVote.GetActive();
        }

        private void Game_OnGameProcessPacket(GamePacketEventArgs args)
        {
            if (!IsActive())
                return;

            try
            {
                var reader = new BinaryReader(new MemoryStream(args.PacketData));
                byte packetId = reader.ReadByte(); //PacketId
                if (packetId != 201)
                    return;
                var gamePacket = new GamePacket(args.PacketData);
                gamePacket.Position = 6;
                int networkId = gamePacket.ReadInteger();
                gamePacket.Position = 11;
                byte noVote = gamePacket.ReadByte();
                byte allVote = gamePacket.ReadByte();
                byte team = gamePacket.ReadByte();

                foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>())
                {
                    if (hero.NetworkId == networkId)
                    {
                        if (noVote > _lastNoVoteCount)
                        {
                            if (
                                Menu.SurrenderVote.GetMenuItem("SAwarenessSurrenderVoteChatChoice")
                                    .GetValue<StringList>()
                                    .SelectedIndex == 1)
                            {
                                Game.PrintChat("{0} voted NO", hero.ChampionName);
                            }
                            else if (
                                Menu.SurrenderVote.GetMenuItem("SAwarenessSurrenderVoteChatChoice")
                                    .GetValue<StringList>()
                                    .SelectedIndex == 2 &&
                                Menu.GlobalSettings.GetMenuItem("SAwarenessGlobalSettingsServerChatPingActive")
                                    .GetValue<bool>())
                            {
                                Game.Say("{0} voted NO", hero.ChampionName);
                            }
                        }
                        else
                        {
                            if (
                                Menu.SurrenderVote.GetMenuItem("SAwarenessSurrenderVoteChatChoice")
                                    .GetValue<StringList>()
                                    .SelectedIndex == 1)
                            {
                                Game.PrintChat("{0} voted YES", hero.ChampionName);
                            }
                            else if (
                                Menu.SurrenderVote.GetMenuItem("SAwarenessSurrenderVoteChatChoice")
                                    .GetValue<StringList>()
                                    .SelectedIndex == 2 &&
                                Menu.GlobalSettings.GetMenuItem("SAwarenessGlobalSettingsServerChatPingActive")
                                    .GetValue<bool>())
                            {
                                Game.Say("{0} voted YES", hero.ChampionName);
                            }
                        }
                        break;
                    }
                }
                _lastNoVoteCount = noVote;
            }
            catch (Exception ex)
            {
                Console.WriteLine("SurrenderProcess: " + ex);
            }
        }
    }
}