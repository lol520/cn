using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace KurisuRiven
{
    internal class KurisuRiven
    {
        #region  Main

        public static Menu config;     
        private static Obj_AI_Hero enemy;
        private static readonly Obj_AI_Hero _player = ObjectManager.Player;
        private static Orbwalking.Orbwalker _orbwalker;

        private static int aa;
        private static int bb;
        private static int runiccount;
        public static int cleavecount;

        private static double ritems;
        private static double ua, uq, uw;
        private static double ra, rq, rw, rr, ri;
        private static float truerange;

        private static readonly Spell _e = new Spell(SpellSlot.E, 390f);
        private static readonly Spell _q = new Spell(SpellSlot.Q, 280f);
        private static readonly Spell _w = new Spell(SpellSlot.W, 260f);
        private static readonly Spell _r = new Spell(SpellSlot.R, 900f);

        private static double now;
        private static double killsteal;
        private static double extraqtime;
        private static double extraetime;

        private static bool ultion, useblade, use_auto_wind;
        private static bool use_combo, use_clear, use_cursor;
        private static int gaptime, wslash, wsneed, bladewhen;
        private static int cc, dd;

        private static float ee, ff;
        private static readonly int[] items = { 3144, 3153, 3142, 3112 };
        private static readonly int[] runicpassive =
        {
            20, 20, 25, 25, 25, 30, 30, 30, 35, 35, 35, 40, 40, 40, 45, 45, 45, 50, 50
        };

        private static readonly string[] jungleminions =
        {
            "AncientGolem", "GreatWraith", "Wraith", "LizardElder", "Golem", "Worm", "Dragon", "GiantWolf"       
        };

        #endregion

        public KurisuRiven()
        {
            Console.WriteLine("KurisuRiven is loaded!");
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        #region  OnGameLoad
        private void Game_OnGameLoad(EventArgs args)
        {
            if (_player.BaseSkinName != "Riven") return;     
            Initialize();

            Game.PrintChat("杞藉叆鎴愬姛锛岃嫳闆勮仈鐩熺矇涓濈珯锛圠OL520.cc鍔╂偍娓告垙鎰夊揩锛侊級");
            config = new Menu("KurisuRiven", "kriven", true);

            // Target Selector
            Menu menuTS = new Menu("鐩爣閫夋嫨", "tselect");
            SimpleTs.AddToMenu(menuTS);
            config.AddSubMenu(menuTS);

            // Orbwalker
            Menu menuOrb = new Menu("璧扮爫", "orbwalker");
            _orbwalker = new Orbwalking.Orbwalker(menuOrb);
            config.AddSubMenu(menuOrb);

            // Draw settings
            Menu menuD = new Menu("鏄剧ず", "dsettings");
            menuD.AddItem(new MenuItem("dsep1", "鏄剧ず"));
            menuD.AddItem(new MenuItem("drawrr", "R鑼冨洿")).SetValue(true);
            menuD.AddItem(new MenuItem("drawaa", "骞矨鑼冨洿")).SetValue(true);
            menuD.AddItem(new MenuItem("drawp", "鏄剧ず琚姩")).SetValue(true);
            menuD.AddItem(new MenuItem("drawengage", "W鑼冨洿")).SetValue(true);
            menuD.AddItem(new MenuItem("drawjumps", "鏄剧ず璺崇偣")).SetValue(true);
            menuD.AddItem(new MenuItem("drawkill", "鍑绘潃鎻愮ず")).SetValue(true);
            var dmgItem = new MenuItem("damageafter", "鏄剧ず浼ゅ").SetValue(true);
            Utility.HpBarDamageIndicator.DamageToUnit = _r.IsReady()
                ? (Utility.HpBarDamageIndicator.DamageToUnitDelegate)ComboDamageWUlt
                : ComboDamage;
            Utility.HpBarDamageIndicator.Enabled = dmgItem.GetValue<bool>();
            dmgItem.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                Utility.HpBarDamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
            };
            menuD.AddItem(dmgItem);
            menuD.AddItem(new MenuItem("dsep2", "==娴嬭瘯"));
            menuD.AddItem(new MenuItem("debugdmg", "杩炴嫑浼ゅ")).SetValue(false);
            menuD.AddItem(new MenuItem("debugtrue", "鐪熷疄鑼冨洿")).SetValue(false);
           
            config.AddSubMenu(menuD);

            Menu menuK = new Menu("鐑敭", "demkeys");
            menuK.AddItem(new MenuItem("combokey", "杩炴嫑")).SetValue(new KeyBind(32, KeyBindType.Press));
            menuK.AddItem(new MenuItem("clearkey", "娓呯嚎")).SetValue(new KeyBind(86, KeyBindType.Press));
            menuK.AddItem(new MenuItem("jumpkey", "绌垮")).SetValue(new KeyBind(88, KeyBindType.Press));
            menuK.AddItem(new MenuItem("ksep1", "====娴嬭瘯"));
            menuK.AddItem(new MenuItem("exportjump", "杈撳嚭浣嶇疆")).SetValue(new KeyBind(73, KeyBindType.Press));
            config.AddSubMenu(menuK);

            // Combo Settings
            Menu menuC = new Menu("杩炴嫑", "csettings");
            menuC.AddItem(new MenuItem("csep1", "==== E"));
            menuC.AddItem(new MenuItem("usevalor", "浣跨敤E")).SetValue(true);
            menuC.AddItem(new MenuItem("valorhealth", "鐢熷懡<%")).SetValue(new Slider(40));
            menuC.AddItem(new MenuItem("waitvalor", "绛塃(澶ф嫑)")).SetValue(true);
            menuC.AddItem(new MenuItem("csep2", "==== R"));
            menuC.AddItem(new MenuItem("useblade", "浣跨敤R")).SetValue(true);
            menuC.AddItem(new MenuItem("bladewhen", "==="))
                .SetValue(new StringList(new[] { "Easykill", "Normalkill", "Hardkill" }, 2));
            menuC.AddItem(new MenuItem("checkover", "鍑绘潃鎻愮ず")).SetValue(false);
            menuC.AddItem(new MenuItem("wslash", "浜屾R"))
                .SetValue(new StringList(new[] { "鍙潃", "Max浼ゅ" }, 1));
            menuC.AddItem(new MenuItem("csep3", "==== Q"));
            menuC.AddItem(new MenuItem("blockanim", "Q(濞变箰)")).SetValue(false);
            menuC.AddItem(new MenuItem("cancelanim", "Q绫诲瀷"))
			    .SetValue(new StringList(new[] { "Move", "Packet", "Delay" }));
            menuC.AddItem(new MenuItem("qqdelay", "Q绐佽繘寤惰繜 ")).SetValue(new Slider(1000, 0, 3000));
            config.AddSubMenu(menuC);

            // Extra Settings
            Menu menuO = new Menu("鏉傞」", "osettings");
            menuO.AddItem(new MenuItem("osep2", "====鏉傞」"));
            menuO.AddItem(new MenuItem("useignote", "鐐圭噧")).SetValue(true);
            menuO.AddItem(new MenuItem("useautow", "鑷姩W")).SetValue(true);
            menuO.AddItem(new MenuItem("autow", "Min")).SetValue(new Slider(3, 1, 5));
            menuO.AddItem(new MenuItem("osep1", "====浜屾R"));
            menuO.AddItem(new MenuItem("useautows", "鑷姩")).SetValue(true);
            menuO.AddItem(new MenuItem("autows", "浼ゅ>%")).SetValue(new Slider(65, 1));
            menuO.AddItem(new MenuItem("autows2", "浜烘暟>%")).SetValue(new Slider(3, 2, 5));
            menuO.AddItem(new MenuItem("osep3", "====鎵撴柇"));
            menuO.AddItem(new MenuItem("interrupter", "鎵撴柇")).SetValue(true);
            menuO.AddItem(new MenuItem("InterruptQ3", "涓夋Q鎵撴柇")).SetValue(true);
            menuO.AddItem(new MenuItem("InterruptW", "W鎵撴柇")).SetValue(true);
            config.AddSubMenu(menuO);

            // Farm/Clear Settings
            Menu menuJ = new Menu("娓呯嚎", "jsettings");
            menuJ.AddItem(new MenuItem("jsep1", "====娓呴噹"));
            menuJ.AddItem(new MenuItem("jungleE", "浣跨敤E ")).SetValue(true);
            menuJ.AddItem(new MenuItem("jungleW", "浣跨敤W ")).SetValue(true);
            menuJ.AddItem(new MenuItem("jungleQ", "浣跨敤Q")).SetValue(true);
            menuJ.AddItem(new MenuItem("jsep2", "====娓呯嚎"));
            menuJ.AddItem(new MenuItem("farmE", "浣跨敤E")).SetValue(true);
            menuJ.AddItem(new MenuItem("farmW", "浣跨敤W")).SetValue(true);
            menuJ.AddItem(new MenuItem("farmQ", "浣跨敤Q")).SetValue(true);
            config.AddSubMenu(menuJ);

            // Advance Settings
            Menu menuA = new Menu("鎺ㄨ繘", "asettings");
            menuA.AddItem(new MenuItem("asep1", "==== QA"));
            menuA.AddItem(new MenuItem("autoconfig", "浣跨敤鎺ㄨ崘")).SetValue(false);
            menuA.AddItem(new MenuItem("qcdelay", "寤惰繜 ")).SetValue(new Slider(0, 0, 1200));
            menuA.AddItem(new MenuItem("aareset", "閲嶇疆")).SetValue(new Slider(0, 0, 1200));
            menuA.AddItem(new MenuItem("asep4", "====绂佺敤"));
            menuA.AddItem(new MenuItem("cursormode", "鍏夐€烸A")).SetValue(false);
            menuA.AddItem(new MenuItem("asep2", "====鎹愯禒"));
            menuA.AddItem(new MenuItem("asep3", "xrobinsong@gmail.com"));
						
			Menu menuQ = new Menu("鑻遍泟鑱旂洘绮変笣绔檌", "LOL520.CC");
            menuQ.AddItem(new MenuItem("qun", "浜ゆ祦缇わ細21422249"));
            menuQ.AddItem(new MenuItem("qun2", "浜ゆ祦2缇わ細397773763"));


            config.AddSubMenu(menuA);
            config.AddToMainMenu();

            _r.SetSkillshot(0.25f, 300f, 120f, false, SkillshotType.SkillshotCone);
            

        }

        #endregion

        #region Initialize

        private void Initialize()
        {
            // initialize walljumps
            new KurisuLib();

            // On Game Draw
            Drawing.OnDraw += Game_OnDraw;

            // On Game Update
            Game.OnGameUpdate += Game_OnGameUpdate;

            // On Game Process Packet
            Game.OnGameProcessPacket += Game_OnGameProcessPacket;

            // On Possible Interrupter
            Interrupter.OnPossibleToInterrupt += Interrupter_OnPossibleToInterrupt;

            //On Enemy Gapcloser
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;

            // On Game Process Spell Cast
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

        }

        #endregion

        #region  OnGameUpdate
        private void Game_OnGameUpdate(EventArgs args)
        {
            CheckDamage(use_cursor ? _player : enemy);

            if (use_combo || killsteal + extraqtime > now)
            {
                CastCombo(use_cursor ? _player : enemy); 
            }

            if (config.Item("autoconfig").GetValue<bool>())
            {
                config.Item("aareset").SetValue(new Slider(Game.Ping + 70, 0, 1200));
                config.Item("qcdelay").SetValue(new Slider(Game.Ping + 8, 0, 1200));
            }

            Killsteal();
            Clear();
            AutoW();
            Requisites();
            RefreshBuffs();
            WindSlash();
        }

        #endregion

        #region  Requisites
        private void Requisites()
        {
            now = TimeSpan.FromMilliseconds(Environment.TickCount).TotalSeconds;
            enemy = SimpleTs.GetTarget(750, SimpleTs.DamageType.Physical);
            truerange = _player.AttackRange + _player.Distance(_player.BBox.Minimum) + 1;

            ee = (ff - Game.Time > 0) ? (ff - Game.Time) : 0;
            ultion = _player.HasBuff("RivenFengShuiEngine", true);

            wsneed = config.Item("autows").GetValue<Slider>().Value;
            gaptime = config.Item("qqdelay").GetValue<Slider>().Value;
            cc = config.Item("qcdelay").GetValue<Slider>().Value;
            dd = config.Item("aareset").GetValue<Slider>().Value;

            use_combo = config.Item("combokey").GetValue<KeyBind>().Active;
            use_clear = config.Item("clearkey").GetValue<KeyBind>().Active;
            use_auto_wind = config.Item("useautows").GetValue<bool>();

            bladewhen = config.Item("bladewhen").GetValue<StringList>().SelectedIndex;
            wslash = config.Item("wslash").GetValue<StringList>().SelectedIndex;

            use_cursor = config.Item("cursormode").GetValue<bool>();
            useblade = config.Item("useblade").GetValue<bool>();

            extraqtime = TimeSpan.FromMilliseconds(gaptime).TotalSeconds;
            extraetime = TimeSpan.FromMilliseconds(300).TotalSeconds;
        }

        #endregion

        #region  On Draw
        private void Game_OnDraw(EventArgs args)
        {

            if (config.Item("drawaa").GetValue<bool>() && !_player.IsDead)
                Utility.DrawCircle(_player.Position, _player.AttackRange + 25, Color.White, 1, 1);
            if (config.Item("drawrr").GetValue<bool>() && !_player.IsDead)
                Utility.DrawCircle(_player.Position, _r.Range, Color.White, 1, 1);
            if (config.Item("drawp").GetValue<bool>() && !_player.IsDead)
            {
                var wts = Drawing.WorldToScreen(_player.Position);
                Drawing.DrawText(wts[0] - 35, wts[1] + 30, Color.White, "Passive: " + runiccount);
                if (_player.Spellbook.CanUseSpell(SpellSlot.Q) == SpellState.NotLearned)
                    Drawing.DrawText(wts[0] - 35, wts[1] + 10, Color.White, "Q: Not Learned!");
                else if (ee <= 0)
                    Drawing.DrawText(wts[0] - 35, wts[1] + 10, Color.White, "Q: Ready");
                else
                    Drawing.DrawText(wts[0] - 35, wts[1] + 10, Color.White, "Q: " + ee.ToString("0.0"));

            }
            if (config.Item("debugtrue").GetValue<bool>())
            {
                if (!_player.IsDead)
                {
                    Utility.DrawCircle(_player.Position, truerange + 25, Color.Yellow, 1, 1);
                }
            }

            if (config.Item("drawengage").GetValue<bool>())
                if (!_player.IsDead)
                    Utility.DrawCircle(_player.Position, _e.Range + _player.AttackRange - 20, Color.White, 1, 1);

            if (config.Item("drawkill").GetValue<bool>())
            {
                if (enemy != null && !enemy.IsDead && !_player.IsDead)
                {
                    var ts = enemy;
                    var wts = Drawing.WorldToScreen(enemy.Position);
                    if ((float)(ra + rq * 2 + rw + ri + ritems) > ts.Health)
                        Drawing.DrawText(wts[0] - 20, wts[1] + 40, Color.OrangeRed, "Kill!");
                    else if ((float)(ra * 2 + rq * 2 + rw + ritems) > ts.Health)
                        Drawing.DrawText(wts[0] - 40, wts[1] + 40, Color.OrangeRed, "Easy Kill!");
                    else if ((float)(ua * 3 + uq * 2 + uw + ri + rr + ritems) > ts.Health)
                        Drawing.DrawText(wts[0] - 40, wts[1] + 40, Color.OrangeRed, "Full Combo Kill!");
                    else if ((float)(ua * 3 + uq * 3 + uw + rr + ri + ritems) > ts.Health)
                        Drawing.DrawText(wts[0] - 40, wts[1] + 40, Color.OrangeRed, "Full Combo Hard Kill!");
                    else if ((float)(ua * 3 + uq * 3 + uw + rr + ri +ritems) < ts.Health)
                        Drawing.DrawText(wts[0] - 40, wts[1] + 40, Color.OrangeRed, "Cant Kill!");

                }
            }

            if (config.Item("debugdmg").GetValue<bool>())
            {
                if (enemy != null && !enemy.IsDead && !_player.IsDead)
                {
                    var wts = Drawing.WorldToScreen(enemy.Position);
                    if (!_r.IsReady())
                        Drawing.DrawText(wts[0] - 75, wts[1] + 60, Color.Orange,
                            "Combo Damage: " + (float)(ra * 3 + rq * 3 + rw + rr + ri + ritems));
                    else
                        Drawing.DrawText(wts[0] - 75, wts[1] + 60, Color.Orange,
                            "Combo Damage: " + (float) (ua*3 + uq*3 + uw + rr + ri + ritems));
                }
            }

            if (config.Item("drawjumps").GetValue<bool>())
            {
                var jumplist = KurisuLib.Jumplist;
                if (jumplist.Any())
                {
                    foreach (var j in jumplist)
                    {
                        if (_player.Distance(j.pointA) <= 800 || _player.Distance(j.pointB) <= 800)
                        {
                            Utility.DrawCircle(j.pointA, 100, Color.White, 1, 1);
                            Utility.DrawCircle(j.pointB, 100, Color.White, 1, 1);
                        }
                    }
                }
            }
        }

        #endregion

        #region  Clear
        private void Clear()
        {
            if (!use_clear) return;

            var target = _orbwalker.GetTarget();                            
            if (target.IsValidTarget(_w.Range) && jungleminions.Any(name => target.Name.StartsWith(name)))
            {
                if (_w.IsReady() && config.Item("jungleW").GetValue<bool>())
                    _w.Cast();
            }

            else if (target.IsValidTarget(_q.Range) && target.Name.StartsWith("Minion"))
            {
                if (!_e.IsReady() || !config.Item("farmE").GetValue<bool>()) return;
                if (_q.IsReady() && cleavecount >= 1)
                    _e.Cast(Game.CursorPos);
            }

            if (_w.IsReady() && config.Item("farmW").GetValue<bool>())
            {
                var minions = ObjectManager.Get<Obj_AI_Minion>().Where(m => m.IsValidTarget(_w.Range)).ToList();

                if (minions.Count() > 2)
                {
                    if (Items.HasItem(3077) && Items.CanUseItem(3077))
                        Items.UseItem(3077);
                    if (Items.HasItem(3074) && Items.CanUseItem(3074))
                        Items.UseItem(3074);
                    _w.Cast();
                }
            }

        }

        #endregion

        #region  AntiGapcloser
        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.Type == _player.Type && gapcloser.Sender.IsValid)
                if (gapcloser.Sender.Distance(_player.Position) < _w.Range && _w.IsReady())
                    _w.Cast();
        }
        #endregion

        #region  Interrupter
        private void Interrupter_OnPossibleToInterrupt(Obj_AI_Base sender, InterruptableSpell spell)
        {
            if (!config.Item("interuppter").GetValue<bool>())
                return;

            if (sender.Type == _player.Type && sender.IsValid && sender.Distance(_player.Position) < _q.Range)
                if (_q.IsReady() && cleavecount == 2 && config.Item("InterruptQ3").GetValue<bool>())
                    _q.Cast(sender.Position, true);

            if (sender.Type == _player.Type && sender.IsValid && sender.Distance(_player.Position) < _w.Range)
                if (_w.IsReady() && config.Item("InterruptW").GetValue<bool>())
                    _w.Cast();
        }
        #endregion

        #region  OnProcessSpellCast

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var target = enemy;

            if (!sender.IsMe)
                return;

            switch (args.SData.Name)
            {
                case "RivenTriCleave":
                    bb = Environment.TickCount;   
                    if (cleavecount < 1)
                        ff = Game.Time + (13 + (13 * _player.PercentCooldownMod));
        
                    break;
                case "RivenMartyr":
                    Orbwalking.LastAATick = 0;
                    if (_q.IsReady() && (use_combo || killsteal + extraqtime > now))
                        Utility.DelayAction.Add(Game.Ping + 75, () => _q.Cast(use_cursor ? Game.CursorPos : target.Position, true));
                    if (_q.IsReady() && use_clear)
                        Utility.DelayAction.Add(Game.Ping + 75, () => _q.Cast(_orbwalker.GetTarget(), true));
                    break;
                case "ItemTiamatCleave":
                    Orbwalking.LastAATick = 0;
                    break;
                case "RivenFeint":
                    aa = Environment.TickCount;
                    _useitems(use_cursor ? _player : target);
                    if (!ultion && (use_combo || killsteal + extraqtime > now))
                    {
                        if (Items.HasItem(3077) && Items.CanUseItem(3077))
                            Items.UseItem(3077);
                        if (Items.HasItem(3074) && Items.CanUseItem(3074))
                            Items.UseItem(3074);
                    }
                    Orbwalking.LastAATick = 0;
                    if (_r.IsReady()  && (use_combo || killsteal + extraqtime > now) && useblade)
                    {
                        if (ultion && wslash == 1)
                                _r.Cast(use_cursor ? Game.CursorPos : target.Position, true);
                    }
                    break;
                case "RivenFengShuiEngine":
                    Orbwalking.LastAATick = 0;
                    break;
                case "rivenizunablade":
                    if (_q.IsReady())
                        _q.Cast(use_cursor ? Game.CursorPos : target.Position, true);
                    break;
            }
        }

        #endregion

        #region  OnProcessPacket
        private void Game_OnGameProcessPacket(GamePacketEventArgs args)
        {
            GamePacket packet = new GamePacket(args.PacketData);

            if (packet.Header == 0xb0)
            {
                packet.Position = 1;
                if (packet.ReadInteger() == _player.NetworkId && config.Item("blockanim").GetValue<bool>())
                    args.Process = false;
            }

            if (packet.Header == 0x65 && (use_combo || killsteal + extraqtime > now))
            {
                packet.Position = 16;
                int sourceId = packet.ReadInteger();

                packet.Position = 1;
                int targetId = packet.ReadInteger();
                int dmgType = packet.ReadByte();

                var trueTarget = ObjectManager.GetUnitByNetworkId<Obj_AI_Hero>(targetId);

                if (sourceId == _player.NetworkId && (dmgType == 4 || dmgType == 3) && _q.IsReady())
                {
                    _useitems(trueTarget);
                    _q.Cast(trueTarget.Position , true);
                }
            }

            if (packet.Header == 0x65 && use_clear)
            {
                packet.Position = 16;
                int sourceId = packet.ReadInteger();

                packet.Position = 1;
                int targetId = packet.ReadInteger();
                int dmgType = packet.ReadByte();

                Obj_AI_Minion trueTarget = ObjectManager.GetUnitByNetworkId<Obj_AI_Minion>(targetId);
                if (sourceId == _player.NetworkId && (dmgType == 4 || dmgType == 3))
                {
                    if (jungleminions.Any(name => trueTarget.Name.StartsWith(name)) && _q.IsReady() &&
                        config.Item("jungleQ").GetValue<bool>())
                    {
                        _q.Cast(trueTarget.Position, true);
                    }

                    var minionList =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(x => x.Name.StartsWith("Minion") && x.IsValidTarget(800))
                            .ToList();

                    if (minionList.Any() && _q.IsReady())
                    {
                        if (!config.Item("farmQ").GetValue<bool>()) return;

                        foreach (var minion in minionList)
                        {
                            if (_e.IsReady() && cleavecount >= 1 && config.Item("jungleE").GetValue<bool>())
                                _e.Cast(minion.Position);

                            _q.Cast(minion.Position, true);
                            _orbwalker.ForceTarget(minion);
                        }

                    }
                }
            }

            if (packet.Header == 0x38 && packet.Size() == 9 && (use_combo || killsteal + extraqtime > now))
            {
                packet.Position = 1;
                int sourceId = packet.ReadInteger();
                if (sourceId == _player.NetworkId)
                {
                    int targetId = _orbwalker.GetTarget().NetworkId;
                    int method = config.Item("cancelanim").GetValue<StringList>().SelectedIndex;

                    Obj_AI_Hero truetarget = ObjectManager.GetUnitByNetworkId<Obj_AI_Hero>(targetId);
                    if (_player.Distance(_orbwalker.GetTarget().Position) <= truerange + 25 && Orbwalking.Move)
                    {
                        Vector3 movePos = truetarget.Position + _player.Position -
                                            Vector3.Normalize(_player.Position) * (_player.Distance(truetarget.Position) + 57);

                        switch (method)
                        {
                            case 1:
                                Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(movePos.X, movePos.Y, 3, _orbwalker.GetTarget().NetworkId)).Send();
                                Orbwalking.LastAATick = 0;
                                break;
                            case 0:
                                _player.IssueOrder(GameObjectOrder.MoveTo, new Vector3(movePos.X, movePos.Y, movePos.Z));
                                Orbwalking.LastAATick = 0;
                                break;
                            case 2:
                                Utility.DelayAction.Add(cc, () => _player.IssueOrder(GameObjectOrder.MoveTo, new Vector3(movePos.X, movePos.Y, movePos.Z)));
                                Utility.DelayAction.Add(dd, () => Orbwalking.LastAATick = 0);
                                break;
                        }
                    }
                }
            }

            if (packet.Header == 0x38 && packet.Size() == 9 && use_clear)
            {
                packet.Position = 1;
                int sourceId = packet.ReadInteger();
                if (sourceId == _player.NetworkId)
                {
                    int targetId = _orbwalker.GetTarget().NetworkId;
                    Obj_AI_Minion truetarget = ObjectManager.GetUnitByNetworkId<Obj_AI_Minion>(targetId);

                    if (_player.Distance(_orbwalker.GetTarget().Position) <= truerange + 25 && Orbwalking.Move)
                    {
                        Vector3 movePos = truetarget.Position + _player.Position -
                                            Vector3.Normalize(_player.Position) * (_player.Distance(truetarget.Position) + 63);

                        if (jungleminions.Any(name => truetarget.Name.StartsWith(name)))
                        {
                            Utility.DelayAction.Add(cc, () => _player.IssueOrder(GameObjectOrder.MoveTo, new Vector3(movePos.X, movePos.Y, movePos.Z)));
                            Utility.DelayAction.Add(dd, () => Orbwalking.LastAATick = 0);
                        }

                        if (jungleminions.Any(name => truetarget.Name.StartsWith("Minion")))
                        {
                            Utility.DelayAction.Add(cc, () => _player.IssueOrder(GameObjectOrder.MoveTo, new Vector3(movePos.X, movePos.Y, movePos.Z)));
                            Utility.DelayAction.Add(dd, () => Orbwalking.LastAATick = 0);
                        }
                    }

                }
            }

            if (packet.Header == 0xfe && packet.Size() == 24)
            {
                packet.Position = 1;
                if (packet.ReadInteger() == _player.NetworkId)
                {
                    Orbwalking.LastAATick = Environment.TickCount;
                    Orbwalking.LastMoveCommandT = Environment.TickCount;
                }
            }
        }

        #endregion

        #region  Combo Logic
        private void CastCombo(Obj_AI_Base target)
        {
            var healthvalor = config.Item("valorhealth").GetValue<Slider>().Value;
            if (target.IsValidTarget())
            {
                if (_player.Distance(use_cursor ? Game.CursorPos : target.Position) > truerange + 25 ||
                    ((_player.Health / _player.MaxHealth) * 100) <= healthvalor)
                {
                    if (_e.IsReady() && config.Item("usevalor").GetValue<bool>())
                        _e.Cast(use_cursor ? Game.CursorPos : target.Position);
                    if (_q.IsReady() && cleavecount <= 1 && config.Item("waitvalor").GetValue<bool>())
                        CheckR(use_cursor ? _player : target);
                }

                if (_w.IsReady() && _q.IsReady() && _e.IsReady()
                    && _player.Distance(use_cursor ? _player.Position : target.Position) < _w.Range + 20)
                {
                    if (cleavecount <= 1)
                        if (!use_cursor) CheckR(target);
                }

                if (_r.IsReady() && _e.IsReady() && ultion)
                {
                    if (cleavecount == 2)
                        _e.Cast(use_cursor ? Game.CursorPos : target.Position);
                }


                if (_player.Distance(use_cursor ? Game.CursorPos : target.Position) < _w.Range)
                    if (_w.IsReady())
                        _w.Cast();

                if (_q.IsReady() && !_e.IsReady() &&
                    _player.Distance(use_cursor ? Game.CursorPos : target.Position) > _q.Range)
                {
                    if (TimeSpan.FromMilliseconds(bb).TotalSeconds + extraqtime < now &&
                        TimeSpan.FromMilliseconds(aa).TotalSeconds + extraetime < now)
                    {
                        _q.Cast(target.Position, true);
                    }
                }
            }
        }

        #endregion

        #region  Buff Handler

        private void RefreshBuffs()
        {
            var buffs = _player.Buffs;

            foreach (var b in buffs)
            {
                if (b.Name == "rivenpassiveaaboost")
                    runiccount = b.Count;
                if (b.Name == "RivenTriCleave")
                    cleavecount = b.Count;
            }

            if (!_player.HasBuff("rivenpassiveaaboost", true))
                runiccount = 0;
            if (!_q.IsReady())
                cleavecount = 0;
        }

        #endregion

        #region  Windlsash
        private static void WindSlash()
        {
            if (!ultion) return;
            foreach (var e in ObjectManager.Get<Obj_AI_Hero>().Where(e => e.IsValidTarget(_r.Range)))
            {                   
                var hitcount = config.Item("autows2").GetValue<Slider>().Value;

                PredictionOutput prediction = _r.GetPrediction(use_cursor ? _player : e, true);
                if (_r.IsReady() && use_auto_wind)
                {
                    if (wslash == 1)
                    {
                        if (prediction.AoeTargetsHitCount >= hitcount)
                            _r.Cast(prediction.CastPosition, true);
                        else if (rr / e.MaxHealth * 100 > e.Health / e.MaxHealth * wsneed)
                        {
                            if (prediction.Hitchance >= HitChance.Medium)
                                _r.Cast(prediction.CastPosition);
                        }
                        else if (e.Health < rr + ra * 2 + rq * 1)
                        {
                            if (prediction.Hitchance >= HitChance.Medium)
                                _r.Cast(prediction.CastPosition);
                        }
                    }
                    else if (wslash == 0 && e.Health < rr)
                    {
                        if (prediction.Hitchance >= HitChance.Medium)
                            _r.Cast(prediction.CastPosition);
                    }
                }
            }
        }

        #endregion

        #region Killsteal
        private void Killsteal()
        {
            foreach (var e in ObjectManager.Get<Obj_AI_Hero>().Where(e => e.IsValidTarget(_r.Range)))
            {
                if (_q.IsReady() && e.Health < rq && _player.Distance(enemy.Position) < _q.Range)
                    _q.Cast(enemy.Position, true);
                else if (_q.IsReady() && e.Health < rq + ra*2 + ri &&
                            _player.Distance(enemy.Position) < _q.Range)
                {
                    enemy = e;
                    killsteal = TimeSpan.FromMilliseconds(Environment.TickCount).TotalSeconds;

                }
                else if (_q.IsReady() && e.Health < rq*2 + ri && _player.Distance(enemy.Position) < _q.Range)
                {
                    enemy = e;
                    killsteal = TimeSpan.FromMilliseconds(Environment.TickCount).TotalSeconds;
                }
            }        
        }

        #endregion

        #region  Item Handler
        private static void _useitems(Obj_AI_Base target)
        {
            foreach (var i in items.Where(i => Items.CanUseItem(i) && Items.HasItem(i)))
            {
                if (target.IsValidTarget(_e.Range + _r.Range))
                    Items.UseItem(i);
            }
        }
        #endregion

        #region  Damage Handler
        private static void CheckDamage(Obj_AI_Base target)
        {
            if (target == null) return;

            var ignite = _player.GetSpellSlot("summonerdot");
            double aaa = _player.GetAutoAttackDamage(target);

            double tmt = Items.HasItem(3077) && Items.CanUseItem(3077) ? _player.GetItemDamage(target, Damage.DamageItems.Tiamat) : 0;
            double hyd = Items.HasItem(3074) && Items.CanUseItem(3074) ? _player.GetItemDamage(target, Damage.DamageItems.Hydra) : 0;
            double bwc = Items.HasItem(3144) && Items.CanUseItem(3144) ? _player.GetItemDamage(target, Damage.DamageItems.Bilgewater) : 0;
            double brk = Items.HasItem(3153) && Items.CanUseItem(3153) ? _player.GetItemDamage(target, Damage.DamageItems.Botrk) : 0;

            rr = _player.GetSpellDamage(target, SpellSlot.R);
            ra = aaa + (aaa * (runicpassive[_player.Level] / 100));
            rq = _q.IsReady() ? DamageQ(target) : 0;
            rw = _w.IsReady() ? _player.GetSpellDamage(target, SpellSlot.W) : 0;
            ri = _player.SummonerSpellbook.CanUseSpell(ignite) == SpellState.Ready ? _player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) : 0;

            ritems = tmt + hyd + bwc + brk;

            ua = _r.IsReady()
                ? ra +
                  _player.CalcDamage(target, Damage.DamageType.Physical,
                      _player.BaseAttackDamage + _player.FlatPhysicalDamageMod * 0.2)
                : ua;

            uq = _r.IsReady()
                ? rq +
                  _player.CalcDamage(target, Damage.DamageType.Physical,
                      _player.BaseAttackDamage + _player.FlatPhysicalDamageMod * 0.2 * 0.7)
                : uq;

            uw = _r.IsReady()
                ? rw +
                  _player.CalcDamage(target, Damage.DamageType.Physical,
                      _player.BaseAttackDamage + _player.FlatPhysicalDamageMod * 0.2 * 1)
                : uw;

            rr = _r.IsReady()
                ? rr +
                  _player.CalcDamage(target, Damage.DamageType.Physical,
                      _player.BaseAttackDamage + _player.FlatPhysicalDamageMod * 0.2)
                : rr;
        }

        public static float DamageQ(Obj_AI_Base target)
        {
            double dmg = 0;
            if (_q.IsReady())
            {
                dmg += _player.CalcDamage(target, Damage.DamageType.Physical,
                    -10 + (_q.Level * 20) +
                    (0.35 + (_q.Level * 0.05)) * (_player.FlatPhysicalDamageMod + _player.BaseAttackDamage));
            }

            return (float)dmg;
        }

        private static float ComboDamageWUlt(Obj_AI_Base hero)
        {
            return (float) (ua*3 + uq*3 + uw + rr + ri + ritems);
        }

        private static float ComboDamage(Obj_AI_Base hero)
        {
            return (float) (ra*3 + rq*3 + rw + rr + ri + ritems);
        }

        #endregion

        #region  Ultimate Handler
        private void CheckR(Obj_AI_Base target)
        {
            if (target.IsValidTarget() && useblade && use_combo)
            {
                switch (bladewhen)
                {
                    case 2:
                        if ((float) (ua*3 + uq*3 + uw + rr + ri + ritems) > target.Health && !ultion)
                        {
                            _r.Cast();
                            if (config.Item("useignote").GetValue<bool>() && _r.IsReady())
                                CastIgnite(target);
                        }
                        break;
                    case 1:
                        if ((float) (ra*3 + rq*3 + rw + rr + ri + ritems) > target.Health && !ultion)
                        {
                            _r.Cast();
                            if (config.Item("useignote").GetValue<bool>() && _r.IsReady())
                                CastIgnite(target);
                        }
                        break;
                    case 0:
                        if ((float) (ra*2 + rq*2 + rw + rr + ri + ritems) > target.Health && !ultion)
                        {
                            _r.Cast();
                        }
                        break;
                }
            }
        }
        
        #endregion

        #region  Ignote Handler
        private static void CastIgnite(Obj_AI_Base target)
        {
            if (target.IsValidTarget(600))
            {
                var ignote = _player.GetSpellSlot("summonerdot");
                if (_player.SummonerSpellbook.CanUseSpell(ignote) == SpellState.Ready)
                {
                    _player.SummonerSpellbook.CastSpell(ignote, target);
                }
            }
        }
        #endregion

        #region  AutoW
        private void AutoW()
        {
            var getenemies = ObjectManager.Get<Obj_AI_Hero>().Where(en => en.IsValidTarget(_w.Range));
            if (getenemies.Count() >= config.Item("autow").GetValue<Slider>().Value)
            {
                if (_w.IsReady() && config.Item("useautow").GetValue<bool>())
                {
                    _w.Cast();
                }
            }

        }

        #endregion

        #region WallJumps

        #endregion

    }
}
