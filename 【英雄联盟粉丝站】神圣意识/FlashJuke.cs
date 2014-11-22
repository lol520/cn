using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace SAwareness
{
    class FlashJuke
    {
        List<Vector3> spotsStart = new List<Vector3>();
        List<Vector3> spotsEnd = new List<Vector3>();

        private double flashedTime = Game.Time;

         public FlashJuke()
        {
             if(Game.MapId != GameMapId.SummonersRift)
                 return;
            spotsStart.Add(new Vector3(5976, 12584, 40));
            spotsStart.Add(new Vector3(8145, 1915, 54));
            spotsStart.Add(new Vector3(5815, 11397, 53));
            spotsStart.Add(new Vector3(8135, 3058, 57));
            spotsStart.Add(new Vector3(11530, 4733, 53));
            spotsStart.Add(new Vector3(2485, 9683, 53));
            spotsStart.Add(new Vector3(9963, 6435, 55));
            spotsStart.Add(new Vector3(4123, 7979, 52));
            spotsStart.Add(new Vector3(3935, 7214, 53));
            spotsStart.Add(new Vector3(8926, 2402, 64));
            spotsStart.Add(new Vector3(5718, 3502, 53));
            spotsStart.Add(new Vector3(7052, 3223, 55));
            spotsStart.Add(new Vector3(7059, 3081, 55));
            spotsStart.Add(new Vector3(5054, 11998, 41));
            spotsStart.Add(new Vector3(6966, 11282, 53));
            spotsStart.Add(new Vector3(6959, 11416, 53));
            spotsStart.Add(new Vector3(8265, 11103, 50));
            spotsStart.Add(new Vector3(9949, 7249, 55));
            spotsStart.Add(new Vector3(11361, 4257, -62));
            spotsStart.Add(new Vector3(5240, 9233, -65));
            spotsStart.Add(new Vector3(5405, 9926, 55));
            spotsStart.Add(new Vector3(8655, 5195, -64));
            spotsStart.Add(new Vector3(8583, 4408, 55));
            spotsStart.Add(new Vector3(2788, 10204, -65));  
            spotsStart.Add(new Vector3(3513, 7555, 55));
            spotsStart.Add(new Vector3(10595, 6915, 54));
            spotsStart.Add(new Vector3(5924, 4975, 51));
            spotsStart.Add(new Vector3(8042, 9542, 53));

            spotsEnd.Add(new Vector3(5571, 12561, 40));
            spotsEnd.Add(new Vector3(8537, 1963, 55));
            spotsEnd.Add(new Vector3(6109, 11204, 54));
            spotsEnd.Add(new Vector3(7867, 3293, 56));
            spotsEnd.Add(new Vector3(11885, 4933, 45));
            spotsEnd.Add(new Vector3(2171, 9569, 53));
            spotsEnd.Add(new Vector3(9614, 6339, 51));
            spotsEnd.Add(new Vector3(4446, 8148, 34));
            spotsEnd.Add(new Vector3(4273, 7017, 54));
            spotsEnd.Add(new Vector3(8875, 2061, 55));
            spotsEnd.Add(new Vector3(5375, 3293, 54));
            spotsEnd.Add(new Vector3(7391, 3297, 55));
            spotsEnd.Add(new Vector3(6729, 2911, 55));
            spotsEnd.Add(new Vector3(5082, 12374, 40));
            spotsEnd.Add(new Vector3(6619, 11165, 54));
            spotsEnd.Add(new Vector3(7303, 11525, 52));
            spotsEnd.Add(new Vector3(8604, 11161, 54));
            spotsEnd.Add(new Vector3(9665, 7497, 54));
            spotsEnd.Add(new Vector3(11435, 3881, -55));
            spotsEnd.Add(new Vector3(4942, 8970, -63));
            spotsEnd.Add(new Vector3(5772, 10033, 54));
            spotsEnd.Add(new Vector3(8935, 5417, -64));
            spotsEnd.Add(new Vector3(8267, 4415, 55));
            spotsEnd.Add(new Vector3(2685, 10567, -64));
            spotsEnd.Add(new Vector3(3153, 7534, 56));
            spotsEnd.Add(new Vector3(10936, 6865, 54));
            spotsEnd.Add(new Vector3(6101, 4621, 51));
            spotsEnd.Add(new Vector3(7917, 9885, 53));

            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnEndScene += Drawing_OnEndScene;
        }

         ~FlashJuke()
        {
            Game.OnGameUpdate -= Game_OnGameUpdate;
            Drawing.OnEndScene -= Drawing_OnEndScene;
        }

        public bool IsActive()
        {
            return Menu.Misc.GetActive() && Menu.FlashJuke.GetActive();
        }

        void Drawing_OnEndScene(EventArgs args)
        {
            if (!IsActive())
                return;

            SpellSlot spell = Activator.GetFlashSlot();
            if (ObjectManager.Player.SummonerSpellbook.CanUseSpell(spell) != SpellState.Ready)
                return;

            for (int i = 0; i < spotsStart.Count; i++)
            {
                if (ObjectManager.Player.ServerPosition.Distance(spotsStart[i]) < 2000)
                {
                    Utility.DrawCircle(spotsStart[i], 50, System.Drawing.Color.Red);
                    Utility.DrawCircle(spotsEnd[i], 100, System.Drawing.Color.Green);
                    Drawing.DrawLine(Drawing.WorldToScreen(spotsStart[i]), Drawing.WorldToScreen(spotsEnd[i]), 2, Color.Gold);
                }
            }
        }

        void Game_OnGameUpdate(EventArgs args)
        {
            if (!IsActive() || !Menu.FlashJuke.GetMenuItem("SAwarenessFlashJukeKeyActive").GetValue<KeyBind>().Active)
                return;

            SpellSlot spell = Activator.GetFlashSlot();
            if(ObjectManager.Player.SummonerSpellbook.CanUseSpell(spell) != SpellState.Ready)
                return;

            spell = Activator.GetPacketSlot(spell);
            Vector3 nearestPosStart = GetNearestPos(spotsStart);

            if (Game.Time > flashedTime + 5)
            {
                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, nearestPosStart);
            }

            if (nearestPosStart.X != 0 && ObjectManager.Player.Distance(nearestPosStart) < 50 && !NavMesh.IsWallOfGrass(ObjectManager.Player.ServerPosition) && Game.Time > flashedTime + 5 &&
                !AnyEnemyInBush())
            {
                Vector3 nearestPosEnd = GetNearestPos(spotsEnd);
                if (nearestPosEnd.X != 0)
                {
                    Activator.FixedSummonerCast.Encoded(new Packet.C2S.Cast.Struct(0, spell, -1, nearestPosEnd.X, nearestPosEnd.Y, nearestPosEnd.X, nearestPosEnd.Y)).Send();
                    flashedTime = Game.Time;
                    if (Menu.FlashJuke.GetMenuItem("SAwarenessFlashJukeRecall").GetValue<bool>())
                    {
                        Utility.DelayAction.Add(200, () =>
                        {
                            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, nearestPosEnd);
                            Packet.C2S.Cast.Encoded(new Packet.C2S.Cast.Struct(0, SpellSlot.Recall)).Send();
                        });
                    }
                }                
            }
        }

        bool AnyEnemyInBush()
        {
            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (hero.IsValid && hero.IsEnemy && hero.IsVisible && !hero.IsDead)
                {
                    if (NavMesh.IsWallOfGrass(hero.ServerPosition) &&
                        ObjectManager.Player.ServerPosition.Distance(hero.ServerPosition) < 650)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        Vector3 GetNearestPos(List<Vector3> vecs)
        {
            Vector3 near = new Vector3();
            double lastDist = 999999999;
            foreach (var vec in vecs)
            {
                if (ObjectManager.Player.Distance(vec) < lastDist)
                {
                    lastDist = ObjectManager.Player.Distance(vec);
                    near = vec;
                }
            }
            return near;
        }
    }
}
