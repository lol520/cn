using System;
using System.Linq;
using Color = System.Drawing.Color;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using LX_Orbwalker;

namespace Master
{
    class LeeSin : Program
    {
        private const String Version = "1.1.6";
        private Obj_AI_Base allyObj = null;
        private bool WardCasted = false, JumpCasted = false, KickCasted = false, FlyCasted = false, InsecJumpCasted = false, QCasted = false, WCasted = false, ECasted = false, RCasted = false;

        public LeeSin()
        {
            SkillQ = new Spell(SpellSlot.Q, 1100);//1300
            SkillW = new Spell(SpellSlot.W, 700);
            SkillE = new Spell(SpellSlot.E, 425);//575
            SkillR = new Spell(SpellSlot.R, 375);
            SkillQ.SetSkillshot(SkillQ.Instance.SData.SpellCastTime, SkillQ.Instance.SData.LineWidth, SkillQ.Instance.SData.MissileSpeed, true, SkillshotType.SkillshotLine);
            SkillW.SetTargetted(SkillW.Instance.SData.SpellCastTime, SkillW.Instance.SData.MissileSpeed);
            SkillE.SetSkillshot(SkillE.Instance.SData.SpellCastTime, SkillE.Instance.SData.LineWidth, SkillE.Instance.SData.MissileSpeed, false, SkillshotType.SkillshotCircle);
            SkillR.SetTargetted(SkillR.Instance.SData.SpellCastTime, SkillR.Instance.SData.MissileSpeed);

            Config.SubMenu("Orbwalker").SubMenu("lxOrbwalker_Modes").AddItem(new MenuItem(Name + "starActive", "鏄庢槦杩炴嫑").SetValue(new KeyBind("X".ToCharArray()[0], KeyBindType.Press)));
            Config.SubMenu("Orbwalker").SubMenu("lxOrbwalker_Modes").AddItem(new MenuItem(Name + "insecMake", "鍙兘鏄洖鏃嬭涪").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            Config.SubMenu("Orbwalker").SubMenu("lxOrbwalker_Modes").AddItem(new MenuItem(Name + "ksbrdr", "|鎶㈠ぇ榫?灏忛緳|").SetValue(new KeyBind("Z".ToCharArray()[0], KeyBindType.Press)));

            Config.AddSubMenu(new Menu("杩炴嫑", "csettings"));
            Config.SubMenu("csettings").AddItem(new MenuItem(Name + "pusage", "浣跨敤琚姩").SetValue(false));
            Config.SubMenu("csettings").AddItem(new MenuItem(Name + "qusage", "浣跨敤 Q").SetValue(true));
            Config.SubMenu("csettings").AddItem(new MenuItem(Name + "wusage", "浣跨敤 W").SetValue(true));
            Config.SubMenu("csettings").AddItem(new MenuItem(Name + "autowusage", "浣跨敤W濡傛灉HP鍦▅").SetValue(new Slider(40, 1)));
            Config.SubMenu("csettings").AddItem(new MenuItem(Name + "eusage", "浣跨敤 E").SetValue(true));
            Config.SubMenu("csettings").AddItem(new MenuItem(Name + "rusage", "浣跨敤R鏉ュ嚮鏉€").SetValue(true));
            Config.SubMenu("csettings").AddItem(new MenuItem(Name + "ignite", "濡傛灉鍙嚮鏉€鑷姩浣跨敤鐐圭噧").SetValue(true));
            Config.SubMenu("csettings").AddItem(new MenuItem(Name + "iusage", "浣跨敤椤圭洰").SetValue(true));

            Config.AddSubMenu(new Menu("楠氭壈", "hsettings"));
            Config.SubMenu("hsettings").AddItem(new MenuItem(Name + "harMode", "浣跨敤楠氭壈濡傛灉HP鍦ㄤ互涓妡").SetValue(new Slider(20, 1)));
            Config.SubMenu("hsettings").AddItem(new MenuItem(Name + "useHarE", "浣跨敤 E").SetValue(true));

            Config.AddSubMenu(new Menu("娓呯嚎/娓呴噹", "LaneJungClear"));
            Config.SubMenu("LaneJungClear").AddItem(new MenuItem(Name + "useClearQ", "浣跨敤 Q").SetValue(true));
            Config.SubMenu("LaneJungClear").AddItem(new MenuItem(Name + "useClearW", "浣跨敤 W").SetValue(true));
            Config.SubMenu("LaneJungClear").AddItem(new MenuItem(Name + "useClearE", "浣跨敤 E").SetValue(true));
            Config.SubMenu("LaneJungClear").AddItem(new MenuItem(Name + "useClearI", "浣跨敤 鎻愪簹鐜涚壒/璐涔濆ご").SetValue(true));

            Config.AddSubMenu(new Menu("涓栫晫绗竴鎵撻噹", "insettings"));
            Config.SubMenu("insettings").AddItem(new MenuItem(Name + "insecMode", "妯″紡").SetValue(new StringList(new[] { "|闄勮繎鐨勭洰鏍噟", "|閫夋嫨鐨勭洰鏍噟", "|榧犳爣鐨勪綅缃畖" })));
            Config.SubMenu("insettings").AddItem(new MenuItem(Name + "insecFlash", "濡傛灉鏃犵溂浣跨敤闂幇").SetValue(true));
            Config.SubMenu("insettings").AddItem(new MenuItem(Name + "insecTowerR", "濡傛灉娌℃湁鑻遍泟鍒欒涪鍚戝").SetValue(new Slider(1100, 500, 1500)));
            Config.SubMenu("insettings").AddItem(new MenuItem(Name + "drawInsec", "|鍥炴棆韪㈢嚎鏉℃柟鍚憒").SetValue(true));
            Config.SubMenu("insettings").AddItem(new MenuItem(Name + "drawInsecTower", "|鏄剧ず澶ф嫑濉旇寖鍥磡").SetValue(true));

            Config.AddSubMenu(new Menu("澶ф嫑璁剧疆", "useUlt"));
            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(i => i.IsEnemy))
            {
                Config.SubMenu("useUlt").AddItem(new MenuItem(Name + "ult" + enemy.ChampionName, "浣跨敤澶ф嫑 " + enemy.ChampionName).SetValue(true));
            }

            Config.AddSubMenu(new Menu("鏉傞」", "miscs"));
            Config.SubMenu("miscs").AddItem(new MenuItem(Name + "lasthitQ", "|浣跨敤Q鏈€鍚庝竴鍑粅").SetValue(true));
            Config.SubMenu("miscs").AddItem(new MenuItem(Name + "surviveW", "|灏濊瘯浣跨敤W鐢熷瓨|").SetValue(true));
            Config.SubMenu("miscs").AddItem(new MenuItem(Name + "smite", "|浣跨敤鎯╂垝灏忓叺鎺|").SetValue(true));
            Config.SubMenu("miscs").AddItem(new MenuItem(Name + "SkinID", "|鎹㈢毊鑲").SetValue(new Slider(4, 0, 6))).ValueChanged += SkinChanger;
            Config.SubMenu("miscs").AddItem(new MenuItem(Name + "packetCast", "浣跨敤灏佸寘").SetValue(true));

            Config.AddSubMenu(new Menu("鎶€鑳借寖鍥撮€夐」", "DrawSettings"));
            Config.SubMenu("DrawSettings").AddItem(new MenuItem(Name + "drawKillable", "鍑绘潃鏂囨湰").SetValue(true));
            Config.SubMenu("DrawSettings").AddItem(new MenuItem(Name + "DrawQ", "Q 鑼冨洿").SetValue(false));
            Config.SubMenu("DrawSettings").AddItem(new MenuItem(Name + "DrawW", "W 鑼冨洿").SetValue(true));
            Config.SubMenu("DrawSettings").AddItem(new MenuItem(Name + "DrawE", "E 鑼冨洿").SetValue(false));
            Config.SubMenu("DrawSettings").AddItem(new MenuItem(Name + "DrawR", "R 鑼冨洿").SetValue(false));
Config.AddSubMenu(new Menu("鑻遍泟鑱旂洘绮変笣绔檌", "LOL520.CC"));
				Config.SubMenu("LOL520.CC").AddItem(new MenuItem("qunhao", "浜ゆ祦缇わ細21422249"));
				Config.SubMenu("LOL520.CC").AddItem(new MenuItem("qunhao2", "浜ゆ祦2缇わ細397773763"));

            Game.OnGameUpdate += OnGameUpdate;
            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Game.OnWndProc += OnWndProc;
            Obj_SpellMissile.OnCreate += OnCreate;
            Game.PrintChat("<font color = \"#33CCCC\">Master of {0}</font> <font color = \"#00ff00\">v{1}</font>", Name, Version);
        }

        private void OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead) return;
            //if (SkillR.IsReady() && targetObj != null && SkillR.InRange(targetObj.Position))
            //{
            //    Game.PrintChat(CheckingCollision(Player, targetObj, new Spell(SpellSlot.R, 1150), false, true).Count.ToString());
            //    if (CheckingCollision(Player, targetObj, new Spell(SpellSlot.R, 1150), false, true).Count >= 2)SkillR.CastOnUnit(targetObj, PacketCast);
            //}
            PacketCast = Config.Item(Name + "packetCast").GetValue<bool>();
            switch (Config.Item(Name + "insecMode").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    allyObj = SkillR.IsReady() ? GetInsecAlly() : null;
                    break;
                case 1:
                    if (!SkillR.IsReady() && allyObj != null) allyObj = null;
                    break;
            }
            switch (LXOrbwalker.CurrentMode)
            {
                case LXOrbwalker.Mode.Combo:
                    NormalCombo();
                    break;
                case LXOrbwalker.Mode.Harass:
                    Harass();
                    break;
                case LXOrbwalker.Mode.LaneClear:
                    LaneJungClear();
                    break;
                case LXOrbwalker.Mode.LaneFreeze:
                    LaneJungClear();
                    break;
                case LXOrbwalker.Mode.Lasthit:
                    if (Config.Item(Name + "lasthitQ").GetValue<bool>()) LastHit();
                    break;
                case LXOrbwalker.Mode.Flee:
                    WardJump(Game.CursorPos);
                    break;
            }
            LXOrbwalker.CustomOrbwalkMode = false;
            if (Config.Item(Name + "insecMake").GetValue<KeyBind>().Active)
            {
                LXOrbwalker.CustomOrbwalkMode = true;
                InsecCombo();
            }
            else InsecJumpCasted = false;
            if (Config.Item(Name + "starActive").GetValue<KeyBind>().Active)
            {
                LXOrbwalker.CustomOrbwalkMode = true;
                StarCombo();
            }
            if (Config.Item(Name + "ksbrdr").GetValue<KeyBind>().Active)
            {
                LXOrbwalker.CustomOrbwalkMode = true;
                KillStealBrDr();
            }
        }

        private void OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            if (Config.Item(Name + "DrawQ").GetValue<bool>() && SkillQ.Level > 0) Utility.DrawCircle(Player.Position, (SkillQ.Instance.Name == "BlindMonkQOne") ? SkillQ.Range : 1300, SkillQ.IsReady() ? Color.Green : Color.Red);
            if (Config.Item(Name + "DrawW").GetValue<bool>() && SkillW.Level > 0) Utility.DrawCircle(Player.Position, SkillW.Range, SkillW.IsReady() ? Color.Green : Color.Red);
            if (Config.Item(Name + "DrawE").GetValue<bool>() && SkillE.Level > 0) Utility.DrawCircle(Player.Position, (SkillE.Instance.Name == "BlindMonkEOne") ? SkillE.Range : 575, SkillE.IsReady() ? Color.Green : Color.Red);
            if (Config.Item(Name + "DrawR").GetValue<bool>() && SkillR.Level > 0) Utility.DrawCircle(Player.Position, SkillR.Range, SkillR.IsReady() ? Color.Green : Color.Red);
            if (Config.Item(Name + "drawInsec").GetValue<bool>() && SkillR.IsReady())
            {
                Byte validTargets = 0;
                if (targetObj != null)
                {
                    Utility.DrawCircle(targetObj.Position, 70, Color.FromArgb(0, 204, 0));
                    validTargets += 1;
                }
                if (Config.Item(Name + "insecMode").GetValue<StringList>().SelectedIndex == 2 || (Config.Item(Name + "insecMode").GetValue<StringList>().SelectedIndex != 2 && allyObj != null))
                {
                    Utility.DrawCircle((Config.Item(Name + "insecMode").GetValue<StringList>().SelectedIndex != 2) ? allyObj.Position : Game.CursorPos, 70, Color.FromArgb(0, 204, 0));
                    if (Config.Item(Name + "insecMode").GetValue<StringList>().SelectedIndex != 2) validTargets += 1;
                }
                if ((Config.Item(Name + "insecMode").GetValue<StringList>().SelectedIndex == 2 && validTargets == 1) || (Config.Item(Name + "insecMode").GetValue<StringList>().SelectedIndex != 2 && validTargets == 2))
                {
                    var posDraw = targetObj.Position + Vector3.Normalize(((Config.Item(Name + "insecMode").GetValue<StringList>().SelectedIndex != 2) ? allyObj.Position : Game.CursorPos) - targetObj.Position) * 600;
                    Drawing.DrawLine(Drawing.WorldToScreen(targetObj.Position), Drawing.WorldToScreen(posDraw), 2, Color.White);
                }
            }
            if (Config.Item(Name + "insecMode").GetValue<StringList>().SelectedIndex == 0 && SkillR.IsReady() && Config.Item(Name + "drawInsecTower").GetValue<bool>()) Utility.DrawCircle(Player.Position, Config.Item(Name + "insecTowerR").GetValue<Slider>().Value, Color.White);
            if (Config.Item(Name + "drawKillable").GetValue<bool>())
            {
                foreach (var killableObj in ObjectManager.Get<Obj_AI_Hero>().Where(i => i.IsValidTarget()))
                {
                    var dmgTotal = Player.GetAutoAttackDamage(killableObj);
                    if (SkillQ.IsReady() && SkillQ.Instance.Name == "BlindMonkQOne") dmgTotal += SkillQ.GetDamage(killableObj);
                    if (SkillR.IsReady() && Config.Item(Name + "ult" + killableObj.ChampionName).GetValue<bool>()) dmgTotal += SkillR.GetDamage(killableObj);
                    if (SkillE.IsReady() && SkillQ.Instance.Name == "BlindMonkEOne") dmgTotal += SkillE.GetDamage(killableObj);
                    if (SkillQ.IsReady() && (killableObj.HasBuff("BlindMonkQOne", true) || killableObj.HasBuff("blindmonkqonechaos", true))) dmgTotal += GetQ2Dmg(killableObj, dmgTotal);
                    if (killableObj.Health < dmgTotal)
                    {
                        var posText = Drawing.WorldToScreen(killableObj.Position);
                        Drawing.DrawText(posText.X - 30, posText.Y - 5, Color.White, "Killable");
                    }
                }
            }
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;
            if (args.SData.Name == "BlindMonkQOne")
            {
                QCasted = true;
                Utility.DelayAction.Add(2500, () => QCasted = false);
            }
            if (args.SData.Name == "BlindMonkEOne")
            {
                ECasted = true;
                Utility.DelayAction.Add(2500, () => ECasted = false);
            }
            if (args.SData.Name == "BlindMonkRKick")
            {
                RCasted = true;
                Utility.DelayAction.Add(700, () => RCasted = false);
                if (Config.Item(Name + "insecMake").GetValue<KeyBind>().Active || Config.Item(Name + "starActive").GetValue<KeyBind>().Active)
                {
                    KickCasted = true;
                    Utility.DelayAction.Add(1000, () => KickCasted = false);
                }
            }
            if (args.SData.Name == "blindmonkqtwo" && Config.Item(Name + "insecMake").GetValue<KeyBind>().Active && Config.Item(Name + "insecFlash").GetValue<bool>() && FlashReady())
            {
                FlyCasted = true;
                Utility.DelayAction.Add(1000, () => FlyCasted = false);
            }
        }

        private void OnWndProc(WndEventArgs args)
        {
            if (MenuGUI.IsChatOpen || Player.IsDead) return;
            if (Config.Item(Name + "insecMode").GetValue<StringList>().SelectedIndex == 1 && args.Msg == (uint)WindowsMessages.WM_LBUTTONDOWN)
            {
                allyObj = null;
                if (SkillR.IsReady()) foreach (var obj in ObjectManager.Get<Obj_AI_Base>().Where(i => i.IsAlly && !i.IsMe && !i.IsDead && i.Distance(Game.CursorPos) <= 130)) allyObj = obj;
            }
        }

        private void OnCreate(GameObject sender, EventArgs args)
        {
            if (sender == null || !sender.IsValid) return;
            if (Config.Item(Name + "surviveW").GetValue<bool>() && SkillW.IsReady() && SkillW.Instance.Name == "BlindMonkWOne")
            {
                var missle = (Obj_SpellMissile)sender;
                var unit = missle.SpellCaster;
                if (unit.IsEnemy)
                {
                    if (LXOrbwalker.IsAutoAttack(missle.SData.Name))
                    {
                        if (missle.Target.IsMe && Player.Health <= unit.GetAutoAttackDamage(Player, true))
                        {
                            SkillW.Cast(PacketCast);
                            return;
                        }
                    }
                    else if (missle.Target.IsMe || Player.Distance(missle.Position) <= 200)
                    {
                        if (missle.SData.Name == "summonerdot")
                        {
                            if (Player.Health <= (unit as Obj_AI_Hero).GetSummonerSpellDamage(Player, Damage.SummonerSpell.Ignite))
                            {
                                SkillW.Cast(PacketCast);
                                return;
                            }
                        }
                        else if (Player.Health <= (unit as Obj_AI_Hero).GetSpellDamage(Player, (unit as Obj_AI_Hero).GetSpellSlot(missle.SData.Name, false), 1))
                        {
                            SkillW.Cast(PacketCast);
                            return;
                        }
                    }
                }
            }
        }

        private void NormalCombo()
        {
            if (targetObj == null) return;
            if (Config.Item(Name + "pusage").GetValue<bool>() && Player.HasBuff("blindmonkpassive_cosmetic", true) && LXOrbwalker.InAutoAttackRange(targetObj) && LXOrbwalker.CanAttack()) return;
            if (Config.Item(Name + "eusage").GetValue<bool>() && SkillE.IsReady() && SkillE.Instance.Name == "BlindMonkEOne" && SkillE.InRange(targetObj.Position)) SkillE.Cast(PacketCast);
            if (Config.Item(Name + "qusage").GetValue<bool>() && SkillQ.IsReady())
            {
                if (SkillQ.Instance.Name == "BlindMonkQOne" && SkillQ.InRange(targetObj.Position))
                {
                    if (Config.Item(Name + "smite").GetValue<bool>() && SkillQ.GetPrediction(targetObj).Hitchance == HitChance.Collision)
                    {
                        if (!SmiteCollision(targetObj, SkillQ)) SkillQ.CastIfHitchanceEquals(targetObj, HitChance.VeryHigh, PacketCast);
                    }
                    else SkillQ.CastIfHitchanceEquals(targetObj, HitChance.VeryHigh, PacketCast);
                }
                else if (targetObj.IsValidTarget(1300) && (targetObj.HasBuff("BlindMonkQOne", true) || targetObj.HasBuff("blindmonkqonechaos", true)))
                {
                    if (Player.Distance(targetObj) > 500 || CanKill(targetObj, SkillQ, 1) || (targetObj.HasBuff("BlindMonkEOne", true) && SkillE.InRange(targetObj.Position)) || !QCasted) SkillQ.Cast(PacketCast);
                }
            }
            if (Config.Item(Name + "eusage").GetValue<bool>() && SkillE.IsReady() && targetObj.IsValidTarget(575) && targetObj.HasBuff("BlindMonkEOne", true) && (Player.Distance(targetObj) > 450 || !ECasted)) SkillE.Cast(PacketCast);
            if (Config.Item(Name + "rusage").GetValue<bool>() && Config.Item(Name + "ult" + targetObj.ChampionName).GetValue<bool>() && SkillR.IsReady() && SkillR.InRange(targetObj.Position))
            {
                if (CanKill(targetObj, SkillR) || (targetObj.Health - SkillR.GetDamage(targetObj) < GetQ2Dmg(targetObj, SkillR.GetDamage(targetObj)) && SkillQ.IsReady() && (targetObj.HasBuff("BlindMonkQOne", true) || targetObj.HasBuff("blindmonkqonechaos", true)) && Player.Mana >= 50)) SkillR.CastOnUnit(targetObj, PacketCast);
            }
            if (Config.Item(Name + "wusage").GetValue<bool>() && SkillW.IsReady() && SkillE.InRange(targetObj.Position) && Player.Health * 100 / Player.MaxHealth <= Config.Item(Name + "autowusage").GetValue<Slider>().Value)
            {
                if (SkillW.Instance.Name == "BlindMonkWOne" && !WCasted)
                {
                    SkillW.Cast(PacketCast);
                    WCasted = true;
                    Utility.DelayAction.Add(1000, () => WCasted = false);
                }
                else if (!Player.HasBuff("blindmonkwoneshield", true) && !WCasted) SkillW.Cast(PacketCast);
            }
            if (Config.Item(Name + "iusage").GetValue<bool>()) UseItem(targetObj);
            if (Config.Item(Name + "ignite").GetValue<bool>()) CastIgnite(targetObj);
        }

        private void Harass()
        {
            if (targetObj == null) return;
            var jumpObj = ObjectManager.Get<Obj_AI_Base>().Where(i => !i.IsMe && i.IsAlly && !(i is Obj_AI_Turret) && i.Distance(Player) <= SkillW.Range + i.BoundingRadius).OrderByDescending(i => i.Distance(Player)).OrderBy(i => i.Distance(ObjectManager.Get<Obj_AI_Turret>().Where(a => a.IsAlly && !a.IsDead).OrderBy(a => a.Distance(Player)).First()));
            if (SkillQ.IsReady())
            {
                if (SkillQ.Instance.Name == "BlindMonkQOne" && SkillQ.InRange(targetObj.Position))
                {
                    if (Config.Item(Name + "smite").GetValue<bool>() && SkillQ.GetPrediction(targetObj).Hitchance == HitChance.Collision)
                    {
                        if (!SmiteCollision(targetObj, SkillQ)) SkillQ.CastIfHitchanceEquals(targetObj, HitChance.VeryHigh, PacketCast);
                    }
                    else SkillQ.CastIfHitchanceEquals(targetObj, HitChance.VeryHigh, PacketCast);
                }
                else if ((targetObj.HasBuff("BlindMonkQOne", true) || targetObj.HasBuff("blindmonkqonechaos", true)) && targetObj.IsValidTarget(1300) && (CanKill(targetObj, SkillQ, 1) || (SkillW.IsReady() && SkillW.Instance.Name == "BlindMonkWOne" && Player.Mana >= (Config.Item(Name + "useHarE").GetValue<bool>() ? 130 : 80) && Player.Health * 100 / Player.MaxHealth >= Config.Item(Name + "harMode").GetValue<Slider>().Value && jumpObj.Count() >= 1))) SkillQ.Cast(PacketCast);
            }
            if (Config.Item(Name + "useHarE").GetValue<bool>() && SkillE.IsReady() && SkillE.Instance.Name == "BlindMonkEOne" && SkillE.InRange(targetObj.Position)) SkillE.Cast(PacketCast);
            if (SkillW.IsReady() && SkillW.Instance.Name == "BlindMonkWOne" && ((SkillE.Level == 0 && !(targetObj.HasBuff("BlindMonkQOne", true) || targetObj.HasBuff("blindmonkqonechaos", true)) && Player.Distance(targetObj) < 200) || (Config.Item(Name + "useHarE").GetValue<bool>() && targetObj.HasBuff("BlindMonkEOne", true)) || (!Config.Item(Name + "useHarE").GetValue<bool>() && !(targetObj.HasBuff("BlindMonkQOne", true) || targetObj.HasBuff("blindmonkqonechaos", true)) && Player.Distance(targetObj) < 200)) && !JumpCasted)
            {
                SkillW.CastOnUnit(jumpObj.First(), PacketCast);
                JumpCasted = true;
                Utility.DelayAction.Add(300, () => JumpCasted = false);
            }
        }

        private void LaneJungClear()
        {
            foreach (var minionObj in MinionManager.GetMinions(Player.Position, 1300, MinionTypes.All, MinionTeam.NotAlly))
            {
                var Passive = Player.HasBuff("blindmonkpassive_cosmetic", true);
                if (Config.Item(Name + "useClearQ").GetValue<bool>() && SkillQ.IsReady())
                {
                    if (SkillQ.Instance.Name == "BlindMonkQOne")
                    {
                        if (!Passive) SkillQ.CastIfHitchanceEquals(minionObj, HitChance.VeryHigh, PacketCast);
                    }
                    else if ((minionObj.HasBuff("BlindMonkQOne", true) || minionObj.HasBuff("blindmonkqonechaos", true)) && (CanKill(minionObj, SkillQ, 1) || !QCasted || Player.Distance(minionObj) > 450 || !Passive)) SkillQ.Cast(PacketCast);
                }
                if (Config.Item(Name + "useClearE").GetValue<bool>() && SkillE.IsReady())
                {
                    if (SkillE.Instance.Name == "BlindMonkEOne" && SkillE.InRange(minionObj.Position))
                    {
                        if (!Passive) SkillE.Cast(PacketCast);
                    }
                    else if (minionObj.HasBuff("BlindMonkEOne", true) && minionObj.IsValidTarget(575) && (!ECasted || !Passive)) SkillE.Cast(PacketCast);
                }
                if (Config.Item(Name + "useClearW").GetValue<bool>() && SkillW.IsReady() && LXOrbwalker.InAutoAttackRange(minionObj) && !WCasted && !Passive)
                {
                    if (SkillW.Instance.Name == "BlindMonkWOne")
                    {
                        SkillW.Cast(PacketCast);
                        WCasted = true;
                        Utility.DelayAction.Add(300, () => WCasted = false);
                    }
                    else SkillW.Cast(PacketCast);
                }
                if (Config.Item(Name + "useClearI").GetValue<bool>() && Player.Distance(minionObj) <= 350)
                {
                    if (Items.CanUseItem(Tiamat)) Items.UseItem(Tiamat);
                    if (Items.CanUseItem(Hydra)) Items.UseItem(Hydra);
                }
            }
        }

        private void LastHit()
        {
            var minionObj = MinionManager.GetMinions(Player.Position, SkillQ.Range, MinionTypes.All, MinionTeam.NotAlly).FirstOrDefault(i => CanKill(i, SkillQ));
            if (minionObj != null && SkillQ.IsReady() && SkillQ.Instance.Name == "BlindMonkQOne") SkillQ.CastIfHitchanceEquals(minionObj, HitChance.VeryHigh, PacketCast);
        }

        private void WardJump(Vector3 Pos)
        {
            if (!SkillW.IsReady() || SkillW.Instance.Name != "BlindMonkWOne") return;
            bool IsWard = false;
            var posJump = (Player.Distance(Pos) > SkillW.Range) ? Player.Position + Vector3.Normalize(Pos - Player.Position) * 600 : Pos;
            foreach (var jumpObj in ObjectManager.Get<Obj_AI_Base>().Where(i => !i.IsMe && i.IsAlly && !(i is Obj_AI_Turret) && i.Distance(Player) <= SkillW.Range + i.BoundingRadius && i.Distance(posJump) <= 230 && (!Config.Item(Name + "insecMake").GetValue<KeyBind>().Active || (Config.Item(Name + "insecMake").GetValue<KeyBind>().Active && !Config.Item(Name + "insecFlash").GetValue<bool>()) || (Config.Item(Name + "insecMake").GetValue<KeyBind>().Active && Config.Item(Name + "insecFlash").GetValue<bool>() && i.Name.ToLower().Contains("ward")))))
            {
                if (jumpObj.Name.ToLower().Contains("ward")) IsWard = true;
                SkillW.CastOnUnit(jumpObj, PacketCast);
                if (!jumpObj.Name.ToLower().Contains("ward")) return;
            }
            if (!IsWard && GetWardSlot() != null && !WardCasted)
            {
                GetWardSlot().UseItem(posJump);
                WardCasted = true;
                Utility.DelayAction.Add(1000, () => WardCasted = false);
            }
        }

        private void StarCombo()
        {
            Orbwalk(targetObj);
            if (targetObj == null) return;
            UseItem(targetObj);
            if (SkillE.IsReady() && SkillE.Instance.Name == "BlindMonkEOne" && SkillE.InRange(targetObj.Position)) SkillE.Cast(PacketCast);
            if (SkillQ.IsReady())
            {
                if (SkillQ.Instance.Name == "BlindMonkQOne" && SkillQ.InRange(targetObj.Position))
                {
                    if (Config.Item(Name + "smite").GetValue<bool>() && SkillQ.GetPrediction(targetObj).Hitchance == HitChance.Collision)
                    {
                        if (!SmiteCollision(targetObj, SkillQ)) SkillQ.CastIfHitchanceEquals(targetObj, HitChance.VeryHigh, PacketCast);
                    }
                    else SkillQ.CastIfHitchanceEquals(targetObj, HitChance.VeryHigh, PacketCast);
                }
                else if (targetObj.IsValidTarget(1300) && (targetObj.HasBuff("BlindMonkQOne", true) || targetObj.HasBuff("blindmonkqonechaos", true)) && (CanKill(targetObj, SkillQ, 1) || (!SkillR.IsReady() && !RCasted && KickCasted) || (!SkillR.IsReady() && !RCasted && !KickCasted && !QCasted))) SkillQ.Cast(PacketCast);
            }
            if (SkillE.IsReady() && targetObj.IsValidTarget(575) && targetObj.HasBuff("BlindMonkEOne", true) && (Player.Distance(targetObj) > 450 || !ECasted)) SkillE.Cast(PacketCast);
            if (!SkillR.InRange(targetObj.Position) && SkillR.IsReady() && SkillQ.IsReady() && (targetObj.HasBuff("BlindMonkQOne", true) || targetObj.HasBuff("blindmonkqonechaos", true)) && Player.Distance(targetObj) < SkillW.Range + SkillR.Range - 170 && SkillW.IsReady() && SkillW.Instance.Name == "BlindMonkWOne") WardJump(targetObj.Position);
            if (SkillR.IsReady() && SkillQ.IsReady() && (targetObj.HasBuff("BlindMonkQOne", true) || targetObj.HasBuff("blindmonkqonechaos", true)) && SkillR.InRange(targetObj.Position) && Player.Mana >= 50) SkillR.CastOnUnit(targetObj, PacketCast);
        }

        private void InsecCombo()
        {
            Orbwalk(targetObj);
            if (targetObj == null) return;
            Vector3 posJumpTo = default(Vector3);
            if (SkillR.IsReady() && (Config.Item(Name + "insecMode").GetValue<StringList>().SelectedIndex == 2 || (Config.Item(Name + "insecMode").GetValue<StringList>().SelectedIndex != 2 && allyObj != null)))
            {
                var posKickTo = (Config.Item(Name + "insecMode").GetValue<StringList>().SelectedIndex != 2) ? allyObj.Position : Game.CursorPos;
                posJumpTo = posKickTo + Vector3.Normalize(targetObj.Position - posKickTo) * (targetObj.Distance(posKickTo) + 250);
                if (SkillR.InRange(targetObj.Position))
                {
                    var posKick = Player.Position + Vector3.Normalize(targetObj.Position - Player.Position) * (targetObj.Distance(Player) + 250);
                    var distKick = (Config.Item(Name + "insecMode").GetValue<StringList>().SelectedIndex != 2) ? allyObj.Distance(targetObj) - allyObj.Distance(posKick) : Game.CursorPos.Distance(targetObj.Position) - Game.CursorPos.Distance(posKick);
                    if (distKick > 0 && distKick / 250 > 0.7)
                    {
                        SkillR.CastOnUnit(targetObj, PacketCast);
                        return;
                    }
                }
                if (Config.Item(Name + "insecFlash").GetValue<bool>())
                {
                    if (SkillW.IsReady() && SkillW.Instance.Name == "BlindMonkWOne" && Player.Distance(posJumpTo) < 600 && GetWardSlot() != null)
                    {
                        WardJump(posJumpTo);
                        InsecJumpCasted = true;
                        return;
                    }
                    else if (FlashReady() && !InsecJumpCasted)
                    {
                        var Obj = ObjectManager.Get<Obj_AI_Base>().Where(i => !i.IsMe && i.IsAlly && !(i is Obj_AI_Turret) && i.Distance(Player) <= SkillW.Range + i.BoundingRadius && i.Distance(posJumpTo) < 550).OrderBy(i => i.Distance(posJumpTo)).FirstOrDefault();
                        if (Obj != null)
                        {
                            if (Player.Distance(posJumpTo) < 1000 && !FlyCasted)
                            {
                                if (SkillW.IsReady() && SkillW.Instance.Name == "BlindMonkWOne" && !JumpCasted)
                                {
                                    SkillW.CastOnUnit(Obj, PacketCast);
                                    JumpCasted = true;
                                    Utility.DelayAction.Add(1000, () => JumpCasted = false);
                                }
                                if (SkillW.IsReady() && SkillW.Instance.Name != "BlindMonkWOne" && JumpCasted)
                                {
                                    Utility.DelayAction.Add(400, () =>
                                    {
                                        CastFlash(posJumpTo);
                                        SkillR.CastOnUnit(targetObj, PacketCast);
                                    });
                                    return;
                                }
                            }
                        }
                        else if (!JumpCasted && Player.Distance(posJumpTo) < 600)
                        {
                            CastFlash(posJumpTo);
                            return;
                        }
                    }
                }
                else if (SkillW.IsReady() && SkillW.Instance.Name == "BlindMonkWOne" && Player.Distance(posJumpTo) < 600)
                {
                    WardJump(posJumpTo);
                    return;
                }
            }
            if (SkillQ.IsReady())
            {
                if (SkillQ.Instance.Name == "BlindMonkQOne" && SkillQ.InRange(targetObj.Position))
                {
                    if (Config.Item(Name + "smite").GetValue<bool>() && SkillQ.GetPrediction(targetObj).Hitchance == HitChance.Collision)
                    {
                        if (!SmiteCollision(targetObj, SkillQ)) SkillQ.CastIfHitchanceEquals(targetObj, HitChance.VeryHigh, PacketCast);
                    }
                    else
                    {
                        if (posJumpTo != default(Vector3) && SkillR.IsReady())
                        {
                            var enemyObj = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(i => i.IsValidTarget(SkillQ.Range) && i.Distance(posJumpTo) < 550 && SkillQ.GetPrediction(i).Hitchance >= HitChance.Medium && !CanKill(i, SkillQ));
                            SkillQ.CastIfHitchanceEquals((enemyObj != null) ? enemyObj : targetObj, (enemyObj != null) ? HitChance.Medium : HitChance.VeryHigh, PacketCast);
                        }
                        else SkillQ.CastIfHitchanceEquals(targetObj, HitChance.VeryHigh, PacketCast);
                    }
                }
                else
                {
                    if ((targetObj.HasBuff("BlindMonkQOne", true) || targetObj.HasBuff("blindmonkqonechaos", true)) && targetObj.IsValidTarget(1300))
                    {
                        if (CanKill(targetObj, SkillQ, 1) || (!SkillR.IsReady() && !RCasted && KickCasted) || Player.Distance(SkillR.IsReady() ? posJumpTo : targetObj.Position) > 600 || !QCasted) SkillQ.Cast(PacketCast);
                    }
                    else if (posJumpTo != default(Vector3) && SkillR.IsReady())
                    {
                        if (ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(i => (i.HasBuff("BlindMonkQOne", true) || i.HasBuff("blindmonkqonechaos", true)) && i.IsValidTarget(1300) && i.Distance(posJumpTo) < 550) != null) SkillQ.Cast(PacketCast);
                    }
                }
            }
        }

        private void KillStealBrDr()
        {
            var minionObj = MinionManager.GetMinions(Player.Position, 1300, MinionTypes.All, MinionTeam.NotAlly).FirstOrDefault(i => i.Name == "Worm12.1.1" || i.Name == "Dragon6.1.1");
            Orbwalk(minionObj);
            if (minionObj == null) return;
            if (SkillQ.IsReady() && !SmiteReady() && minionObj.Health - SkillQ.GetDamage(minionObj) < GetQ2Dmg(minionObj, SkillQ.GetDamage(minionObj)))
            {
                if (SkillQ.Instance.Name == "BlindMonkQOne" && SkillQ.InRange(minionObj.Position))
                {
                    SkillQ.CastIfHitchanceEquals(minionObj, HitChance.VeryHigh, PacketCast);
                }
                else if ((minionObj.HasBuff("BlindMonkQOne", true) || minionObj.HasBuff("blindmonkqonechaos", true)) && minionObj.IsValidTarget(1300))
                {
                    SkillQ.Cast(PacketCast);
                    return;
                }
            }
            if (SkillQ.IsReady() && SmiteReady() && minionObj.Health - (SkillQ.GetDamage(minionObj) + Player.GetSummonerSpellDamage(minionObj, Damage.SummonerSpell.Smite)) < GetQ2Dmg(minionObj, SkillQ.GetDamage(minionObj) + Player.GetSummonerSpellDamage(minionObj, Damage.SummonerSpell.Smite)))
            {
                if (SkillQ.Instance.Name == "BlindMonkQOne" && SkillQ.InRange(minionObj.Position))
                {
                    SkillQ.CastIfHitchanceEquals(minionObj, HitChance.VeryHigh, PacketCast);
                }
                else if ((minionObj.HasBuff("BlindMonkQOne", true) || minionObj.HasBuff("blindmonkqonechaos", true)) && minionObj.IsValidTarget(1300))
                {
                    SkillQ.Cast(PacketCast);
                    Utility.DelayAction.Add(400, () => CastSmite(minionObj));
                    return;
                }
            }
            CastSmite(minionObj);
        }

        private Obj_AI_Base GetInsecAlly()
        {
            Obj_AI_Base nearObj = null;
            if (ObjectManager.Get<Obj_AI_Hero>().FirstOrDefault(i => i.IsAlly && !i.IsDead && !i.IsMe && i.Distance(Player) < Config.Item(Name + "insecTowerR").GetValue<Slider>().Value) != null)
            {
                nearObj = ObjectManager.Get<Obj_AI_Hero>().Where(i => i.IsAlly && !i.IsDead && !i.IsMe).OrderBy(i => i.Distance(Player)).First();
            }
            else
            {
                nearObj = ObjectManager.Get<Obj_AI_Turret>().Where(i => i.IsAlly && !i.IsDead).OrderBy(i => i.Distance(Player)).FirstOrDefault();
                var nearMinion = (targetObj != null) ? ObjectManager.Get<Obj_AI_Minion>().Where(i => i.IsAlly && !i.IsDead && i.Distance(Player) < 1600 && i.Distance(targetObj) > 600).OrderByDescending(i => i.Distance(targetObj)).OrderBy(i => i.Distance(nearObj)).FirstOrDefault() : null;
                if (Player.Distance(nearObj) > 1600 && nearMinion != null) nearObj = nearMinion;
            }
            return nearObj;
        }

        private void UseItem(Obj_AI_Hero target)
        {
            if (Items.CanUseItem(Bilge) && Player.Distance(target) <= 450) Items.UseItem(Bilge, target);
            if (Items.CanUseItem(Blade) && Player.Distance(target) <= 450) Items.UseItem(Blade, target);
            if (Items.CanUseItem(Tiamat) && Player.CountEnemysInRange(350) >= 1) Items.UseItem(Tiamat);
            if (Items.CanUseItem(Hydra) && (Player.CountEnemysInRange(350) >= 2 || (Player.GetAutoAttackDamage(target) < target.Health && Player.CountEnemysInRange(350) == 1))) Items.UseItem(Hydra);
            if (Items.CanUseItem(Rand) && Player.CountEnemysInRange(450) >= 1) Items.UseItem(Rand);
        }

        private double GetQ2Dmg(Obj_AI_Base target, double dmgPlus)
        {
            var Dmg = Player.CalcDamage(target, Damage.DamageType.Physical, new Int32[] { 50, 80, 110, 140, 170 }[SkillQ.Level - 1] + 0.9 * Player.FlatPhysicalDamageMod + 0.08 * (target.MaxHealth - (target.Health - dmgPlus)));
            return (target is Obj_AI_Minion && Dmg > 400) ? Player.CalcDamage(target, Damage.DamageType.Physical, 400) : Dmg;
        }
    }
}