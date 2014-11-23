using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

using LeagueSharp;
using LeagueSharp.Common;
/*TODO
 * Combo calc and choose best <-- kinda
 * Farming
 * Interupt
 * 
 * gap close with q < -- done
 * 
 * mash q if les hp < -- done
 * 
 * smart cancel combos < -- yup
 * 
 * gap kill <-- yup
 * 
 * overkill 
 * 
 * harass to trade good <-- done
 * 
 * 
 * fix ignite
 * 
 * R KS
 * 
 */
using Rive;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;

namespace RivenSharp
{
    class RivenSharp
    {

        public const string CharName = "Riven";

        public static Menu Config;


        public static HpBarIndicator hpi = new HpBarIndicator();
        

        public RivenSharp()
        {
            Console.WriteLine("Riven sharp starting...");
            try
            {
                // if (ObjectManager.Player.BaseSkinName != CharName)
                //    return;
                /* CallBAcks */
                CustomEvents.Game.OnGameLoad += onLoad;
            }
            catch (Exception ex)
            {
               Console.WriteLine(ex.Message);
            }

        }

        private static void onLoad(EventArgs args)
        {
            try
            {

            Game.PrintChat("RivenSharp by DeTuKs");
            Config = new Menu("放逐之刃─瑞文", "Riven", true);
            //Orbwalkervar menu = new Menu("My Mainmenu", "my_mainmenu", true);
            var orbwalkerMenu = new Menu("LX 走砍", "my_Orbwalker");
            LXOrbwalker.AddToMenu(orbwalkerMenu);
            Config.AddSubMenu(orbwalkerMenu);
           //TS
           var TargetSelectorMenu = new Menu("目标选择", "Target Selector");
           SimpleTs.AddToMenu(TargetSelectorMenu);
           Config.AddSubMenu(TargetSelectorMenu);
            //Combo
            Config.AddSubMenu(new Menu("连招", "combo"));
            Config.SubMenu("combo").AddItem(new MenuItem("useR", "使用 R")).SetValue(new KeyBind('Z', KeyBindType.Toggle, true));
            Config.SubMenu("combo").AddItem(new MenuItem("forceQE", "使用Q后E")).SetValue(true);
            Config.SubMenu("combo").AddItem(new MenuItem("packets", "使用封包")).SetValue(true);

            //Haras
            Config.AddSubMenu(new Menu("骚扰", "haras"));
            Config.SubMenu("haras").AddItem(new MenuItem("doHarasE", "使用 E")).SetValue(new KeyBind('G', KeyBindType.Press, false));
            Config.SubMenu("haras").AddItem(new MenuItem("doHarasQ", "使用 Q")).SetValue(new KeyBind('T', KeyBindType.Press, false));

            //Drawing
            Config.AddSubMenu(new Menu("绘制", "draw"));
            Config.SubMenu("draw").AddItem(new MenuItem("doDraw", "清除绘制")).SetValue(false);
            Config.SubMenu("draw").AddItem(new MenuItem("drawHp", "显示伤害")).SetValue(true);

            //Debug
            Config.AddSubMenu(new Menu("调试", "debug"));
            Config.SubMenu("debug").AddItem(new MenuItem("db_targ", "调试目标��")).SetValue(new KeyBind('0', KeyBindType.Press, false));

            Config.AddToMainMenu();

            Drawing.OnDraw += onDraw;
            Drawing.OnEndScene += OnEndScene;
            Game.OnGameUpdate += OnGameUpdate;

            GameObject.OnCreate += OnCreateObject;
            GameObject.OnDelete += OnDeleteObject;
            GameObject.OnPropertyChange += OnPropertyChange;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;

            Game.OnGameSendPacket += OnGameSendPacket;
            Game.OnGameProcessPacket += OnGameProcessPacket;

            Riven.setSkillshots();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void OnEndScene(EventArgs args)
        {
            if (Config.Item("drawHp").GetValue<bool>())
            {
                foreach (
                    var enemy in
                        ObjectManager.Get<Obj_AI_Hero>()
                            .Where(ene => !ene.IsDead && ene.IsEnemy && ene.IsVisible))
                {
                    hpi.unit = enemy;
                    hpi.drawDmg(Riven.rushDmgBasedOnDist(enemy), Color.Yellow);

                }
            }
        }

        /*
         * 
         */
        private static void OnGameUpdate(EventArgs args)
        {
            /*
                RivenFengShuiEngine
                rivenwindslashready
             */
            try
            {

                if (Config.Item("doHarasE").GetValue<KeyBind>().Active)
                {
                    Obj_AI_Hero target = SimpleTs.GetTarget(1400, SimpleTs.DamageType.Physical);
                    LXOrbwalker.ForcedTarget = target;
                    Riven.doHarasE(target);
                }else if (Config.Item("doHarasQ").GetValue<KeyBind>().Active)
                {
                    Obj_AI_Hero target = SimpleTs.GetTarget(1400, SimpleTs.DamageType.Physical);
                    LXOrbwalker.ForcedTarget = target;
                    Riven.doHarasQ(target);
                }


                if (LXOrbwalker.CurrentMode == LXOrbwalker.Mode.Combo)
                {
                     Obj_AI_Hero target = SimpleTs.GetTarget(1400, SimpleTs.DamageType.Physical);
                     LXOrbwalker.ForcedTarget = target;
                     Riven.doCombo(target);
                     //Console.WriteLine(target.NetworkId);
                }
            }
            catch (Exception ex)
            {
               // Console.WriteLine(ex);
            }
        }


        private static void onDraw(EventArgs args)
        {
            try
            {

                if (!Config.Item("doDraw").GetValue<bool>())
                {

                    if (Config.Item("drawHp").GetValue<bool>())
                    {
                        foreach (
                            var enemy in
                                ObjectManager.Get<Obj_AI_Hero>()
                                    .Where(ene => !ene.IsDead && ene.IsEnemy && ene.IsVisible))
                        {
                            hpi.unit = enemy;
                            hpi.drawDmg(Riven.rushDmgBasedOnDist(enemy), Color.Yellow);

                        }
                    }
                    foreach (
                        Obj_AI_Hero enHero in
                            ObjectManager.Get<Obj_AI_Hero>().Where(enHero => enHero.IsEnemy && enHero.Health > 0))
                    {
                        Utility.DrawCircle(enHero.Position,
                            enHero.BoundingRadius + Riven.E.Range + Riven.Player.AttackRange,
                            (Riven.rushDown) ? Color.Red : Color.Blue);
                        //Drawing.DrawCircle(enHero.Position, enHero.BoundingRadius + Riven.E.Range+Riven.Player.AttackRange, Color.Blue);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
           // if (sender.Name.Contains("missile") || sender.Name.Contains("Minion"))
           //     return;
           // Console.WriteLine("Object: " + sender.Name);
        }

        private static void OnDeleteObject(GameObject sender, EventArgs args)
        {

        }

        public static bool isComboing()
        {
            if (Config.Item("doHarasE").GetValue<KeyBind>().Active ||
                Config.Item("doHarasQ").GetValue<KeyBind>().Active
                || LXOrbwalker.CurrentMode == LXOrbwalker.Mode.Combo)
            {
                return true;
            }

            return false;
        }


        public static void OnProcessSpell(LeagueSharp.Obj_AI_Base sender, LeagueSharp.GameObjectProcessSpellCastEventArgs arg)
        {
            if (Config.Item("forceQE").GetValue<bool>() && sender.IsMe && arg.SData.Name.Contains("RivenFeint") && Riven.Q.IsReady() && LXOrbwalker.GetPossibleTarget() != null)
             {
                Console.WriteLine("force q");
                Riven.Q.Cast(LXOrbwalker.GetPossibleTarget());
                 Riven.forceQ = true;
                 // Riven.timer = new System.Threading.Timer(obj => { Riven.Player.IssueOrder(GameObjectOrder.MoveTo, Riven.difPos()); }, null, (long)100, System.Threading.Timeout.Infinite);
             }
        }

        public static void OnPropertyChange(LeagueSharp.GameObject obj, LeagueSharp.GameObjectPropertyChangeEventArgs prop)
        {
           // Console.WriteLine("obj: " + obj.Name + " - " + prop.NewValue);
        }

        public static void OnPlayAnimation(Obj_AI_Base value0, GameObjectPlayAnimationEventArgs value1)
        {
           // if (value1.Animation.Contains("Spell"))
           // {
           //     Console.WriteLine("Hydra");
           //     Utility.DelayAction.Add(Game.Ping + 150, delegate { Riven.useHydra(Riven.orbwalker.GetTarget()); });
           // }
        }


        public static void OnGameProcessPacket(GamePacketEventArgs args)
        {
            try
            {
                if (isComboing())
                {
                    if (args.PacketData[0] == 0x65 && Riven.Q.IsReady())
                    {
                        Packet.S2C.Damage.Struct dmg = Packet.S2C.Damage.Decoded(args.PacketData);

                       // LogPacket(args);
                        GamePacket gp = new GamePacket(args.PacketData);
                        gp.Position = 1;

                        int targetID = gp.ReadInteger();
                        int dType = (int)gp.ReadByte();
                        int Unknown = gp.ReadShort();
                        float DamageAmount = gp.ReadFloat();
                        int TargetNetworkIdCopy = gp.ReadInteger();
                        int SourceNetworkId = gp.ReadInteger();
                        if (Riven.Player.NetworkId != dmg.SourceNetworkId)
                            return;
                        Obj_AI_Base targ = ObjectManager.GetUnitByNetworkId<Obj_AI_Base>(dmg.TargetNetworkId);
                        if ((int)dmg.Type == 12 || (int)dmg.Type == 4 || (int)dmg.Type == 3 || (int)dmg.Type == 36 || (int)dmg.Type == 11)
                        {
                            Riven.Q.Cast(targ.Position);
                        }
                        else
                        {
                            Console.WriteLine("dtyoe: "+dType);
                        }
                    }
                    if (args.PacketData[0] == 0x34)// from yol0 :)
                    {
                        GamePacket packet = new GamePacket(args.PacketData);
                        packet.Position = 9;
                        int action = packet.ReadByte();
                        packet.Position = 1;
                        int sourceId = packet.ReadInteger();
                        if (action == 17 && sourceId == Riven.Player.NetworkId)
                        {
                            Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(Game.CursorPos.X, Game.CursorPos.Y)).Send();
                            if (LXOrbwalker.GetPossibleTarget() != null)
                            {
                                Riven.moveTo(LXOrbwalker.GetPossibleTarget().Position);
                                //Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(LXOrbwalker.GetPossibleTarget().Position.X, LXOrbwalker.GetPossibleTarget().Position.Y)).Send();

                               // LXOrbwalker.ResetAutoAttackTimer();
                                Riven.cancelAnim(true);
                            }
                        }
                    }
                    else if (args.PacketData[0] == 0x61) //move
                    {
                        GamePacket packet = new GamePacket(args.PacketData);
                        packet.Position = 12;
                        int sourceId = packet.ReadInteger();
                        if (sourceId == Riven.Player.NetworkId)
                        {
                            if (LXOrbwalker.GetPossibleTarget() != null)
                            {
                            //    Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(LXOrbwalker.GetPossibleTarget().Position.X, LXOrbwalker.GetPossibleTarget().Position.Y)).Send();
                                LXOrbwalker.ResetAutoAttackTimer();
                            }
                        }
                    }
                    else if (args.PacketData[0] == 0x38) //animation2
                    {
                        GamePacket packet = new GamePacket(args.PacketData);
                        packet.Position = 1;
                        int sourceId = packet.ReadInteger();
                        if (packet.Size() == 9 && sourceId == Riven.Player.NetworkId)
                        {
                            Riven.moveTo(Game.CursorPos);
                            Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(Game.CursorPos.X, Game.CursorPos.Y)).Send();
                            LXOrbwalker.ResetAutoAttackTimer();
                            Riven.cancelAnim();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void OnGameSendPacket(GamePacketEventArgs args)
        {
            try
            {
                if (args.PacketData[0] == 119)
                    args.Process = false;

                //if (Riven.orbwalker.ActiveMode.ToString() == "Combo")
                 //   LogPacket(args);
                if (args.PacketData[0] == 154 && LXOrbwalker.CurrentMode == LXOrbwalker.Mode.Combo)
                {
                    Packet.C2S.Cast.Struct cast = Packet.C2S.Cast.Decoded(args.PacketData);
                    if ((int) cast.Slot > -1 && (int) cast.Slot < 5)
                    {
                        Utility.DelayAction.Add(Game.Ping+LXOrbwalker.GetCurrentWindupTime(), delegate { Riven.cancelAnim(true); });

                        //Game.Say("/l");
                    }

                    if (cast.Slot == SpellSlot.E && Riven.R.IsReady())
                    {
                        Utility.DelayAction.Add(Game.Ping + 50, delegate { Riven.useRSmart(LXOrbwalker.GetPossibleTarget()); });
                    }
                    //Console.WriteLine(cast.Slot + " : " + Game.Ping);
                   /* if (cast.Slot == SpellSlot.Q)
                        Orbwalking.ResetAutoAttackTimer();
                    else if (cast.Slot == SpellSlot.W && Riven.Q.IsReady())
                        Utility.DelayAction.Add(Game.Ping+200, delegate { Riven.useHydra(Riven.orbwalker.GetTarget()); });
                    else if (cast.Slot == SpellSlot.E && Riven.W.IsReady())
                    {
                        Console.WriteLine("cast QQQQ");
                        Utility.DelayAction.Add(Game.Ping+200, delegate { Riven.useWSmart(Riven.orbwalker.GetTarget()); });
                    }
                    else if ((int)cast.Slot == 131 && Riven.W.IsReady())
                    {
                        Orbwalking.ResetAutoAttackTimer();
                        Utility.DelayAction.Add(Game.Ping +200, delegate { Riven.useWSmart(Riven.orbwalker.GetTarget()); });
                    }*/
                        // LogPacket(args);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

      


    }
}
