using System;
using System.Linq;
using Color = System.Drawing.Color;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using LX_Orbwalker;

namespace Master
{
    class Riven : Program
    {
        private const String Version = "1.0.0";

        public Riven()
        {
            SkillQ = new Spell(SpellSlot.Q, 1100);//1300
            SkillW = new Spell(SpellSlot.W, 700);
            SkillE = new Spell(SpellSlot.E, 425);//575
            SkillR = new Spell(SpellSlot.R, 375);
            SkillE.SetSkillshot(SkillE.Instance.SData.SpellCastTime, SkillE.Instance.SData.LineWidth, SkillE.Instance.SData.MissileSpeed, false, SkillshotType.SkillshotLine);

            Config.AddSubMenu(new Menu("杩炴嫑", "csettings"));
            Config.SubMenu("csettings").AddItem(new MenuItem(Name + "qusage", "浣跨敤 Q").SetValue(true));
            Config.SubMenu("csettings").AddItem(new MenuItem(Name + "wusage", "浣跨敤 W").SetValue(false));
            Config.SubMenu("csettings").AddItem(new MenuItem(Name + "eusage", "浣跨敤 E").SetValue(true));
            Config.SubMenu("csettings").AddItem(new MenuItem(Name + "rusage", "浣跨敤 R").SetValue(true));
            Config.SubMenu("csettings").AddItem(new MenuItem(Name + "ignite", "濡傛灉鍙嚮鏉€鑷姩浣跨敤鐐圭噧").SetValue(true));
            Config.SubMenu("csettings").AddItem(new MenuItem(Name + "iusage", "浣跨敤椤圭洰").SetValue(true));

            Config.AddSubMenu(new Menu("Extra Combo", "cexsettings"));
            Config.SubMenu("cexsettings").AddSubMenu(new Menu("Skill W", "configW"));
            Config.SubMenu("cexsettings").SubMenu("configW").AddItem(new MenuItem(Name + "itemW", "Use Item After W").SetValue(true));
            Config.SubMenu("cexsettings").SubMenu("configW").AddItem(new MenuItem(Name + "autoW", "Auto W If Enemy Above").SetValue(new Slider(2, 1, 4)));
            Config.SubMenu("cexsettings").AddSubMenu(new Menu("Skill R (First)", "configR1"));
            Config.SubMenu("cexsettings").SubMenu("configR1").AddItem(new MenuItem(Name + "modeR1", "Mode").SetValue(new StringList(new[] { "# Enemy", "Target Health", "Target Range", "All" }, 1)));
            Config.SubMenu("cexsettings").SubMenu("configR1").AddItem(new MenuItem(Name + "countR1", "Use R If Enemy Above").SetValue(new Slider(1, 1, 4)));
            Config.SubMenu("cexsettings").SubMenu("configR1").AddItem(new MenuItem(Name + "healthR1", "Use R If Target Health Under").SetValue(new Slider(65, 1)));
            Config.SubMenu("cexsettings").SubMenu("configR1").AddItem(new MenuItem(Name + "rangeR1", "Use R If Target In Range").SetValue(new Slider(400, 125, 550)));
            Config.SubMenu("cexsettings").AddSubMenu(new Menu("Skill R (Second)", "configR2"));
            Config.SubMenu("cexsettings").SubMenu("configR2").AddItem(new MenuItem(Name + "modeR2", "Mode").SetValue(new StringList(new[] { "Max Damage", "To Kill" }, 1)));
            Config.SubMenu("cexsettings").SubMenu("configR2").AddItem(new MenuItem(Name + "ksR2", "Use R To Kill Steal").SetValue(true));

            Config.AddSubMenu(new Menu("楠氭壈", "hsettings"));
            Config.SubMenu("hsettings").AddItem(new MenuItem(Name + "harMode", "濡傛灉浠ヤ笂HP浣跨敤楠氭壈").SetValue(new Slider(20, 1)));
            Config.SubMenu("hsettings").AddItem(new MenuItem(Name + "useHarQ", "浣跨敤 Q").SetValue(true));
            Config.SubMenu("hsettings").AddItem(new MenuItem(Name + "useHarW", "浣跨敤 W").SetValue(true));
            Config.SubMenu("hsettings").AddItem(new MenuItem(Name + "useHarE", "浣跨敤 E").SetValue(true));

            Config.AddSubMenu(new Menu("鏉傞」", "miscs"));
            Config.SubMenu("miscs").AddItem(new MenuItem(Name + "cancelAni", "|鍔ㄧ敾鍙栨秷妯″紡|").SetValue(new StringList(new[] { "Dance", "Laugh", "Move" }, 2)));
            Config.SubMenu("miscs").AddItem(new MenuItem(Name + "useDodgeE", "|浣跨敤e闂翰鎶€鑳絴").SetValue(true));
            Config.SubMenu("miscs").AddItem(new MenuItem(Name + "useAntiW", "|浣跨敤W鍙嶅樊璺濇帴杩憒").SetValue(true));
            Config.SubMenu("miscs").AddItem(new MenuItem(Name + "useInterW", "|浣跨敤W鎵撴柇|").SetValue(true));
            Config.SubMenu("miscs").AddItem(new MenuItem(Name + "SkinID", "|鎹㈢毊鑲").SetValue(new Slider(4, 0, 6))).ValueChanged += SkinChanger;
            Config.SubMenu("miscs").AddItem(new MenuItem(Name + "packetCast", "浣跨敤灏佸寘").SetValue(true));

            Config.AddSubMenu(new Menu("娓呯嚎/娓呴噹", "LaneJungClear"));
            Config.SubMenu("LaneJungClear").AddItem(new MenuItem(Name + "useClearQ", "浣跨敤 Q").SetValue(true));
            Config.SubMenu("LaneJungClear").AddItem(new MenuItem(Name + "useClearW", "浣跨敤 W").SetValue(true));
            Config.SubMenu("LaneJungClear").AddItem(new MenuItem(Name + "useClearE", "浣跨敤 E").SetValue(true));
            Config.SubMenu("LaneJungClear").AddItem(new MenuItem(Name + "useClearI", "浣跨敤 鎻愪簹鐜涚壒/璐涔濆ご").SetValue(true));

            Config.AddSubMenu(new Menu("鎶€鑳借寖鍥撮€夐」", "DrawSettings"));
            Config.SubMenu("DrawSettings").AddItem(new MenuItem(Name + "DrawQ", "Q 鑼冨洿").SetValue(false));
            Config.SubMenu("DrawSettings").AddItem(new MenuItem(Name + "DrawW", "W 鑼冨洿").SetValue(true));
            Config.SubMenu("DrawSettings").AddItem(new MenuItem(Name + "DrawE", "E 鑼冨洿").SetValue(true));
            Config.SubMenu("DrawSettings").AddItem(new MenuItem(Name + "DrawR", "R 鑼冨洿").SetValue(false));
Config.AddSubMenu(new Menu("鑻遍泟鑱旂洘绮変笣绔檌", "LOL520.CC"));
				Config.SubMenu("LOL520.CC").AddItem(new MenuItem("qunhao", "浜ゆ祦缇わ細21422249"));
				Config.SubMenu("LOL520.CC").AddItem(new MenuItem("qunhao2", "浜ゆ祦2缇わ細397773763"));
            Game.OnGameUpdate += OnGameUpdate;
            Drawing.OnDraw += OnDraw;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter.OnPossibleToInterrupt += OnPossibleToInterrupt;
            Game.OnGameProcessPacket += OnGameProcessPacket;
            Game.OnGameSendPacket += OnGameSendPacket;
            Game.PrintChat("<font color = \"#33CCCC\">Master of {0}</font> <font color = \"#00ff00\">v{1}</font>", Name, Version);
        }

        private void OnGameProcessPacket(GamePacketEventArgs args)
        {
            if (targetObj != null && PacketCast && (LXOrbwalker.CurrentMode == LXOrbwalker.Mode.Combo || LXOrbwalker.CurrentMode == LXOrbwalker.Mode.Harass))
            {
                if (args.PacketData[0] != 100) return;
                var PacketData = new GamePacket(args.PacketData);
                PacketData.Position = 1;
                if (PacketData.ReadInteger() != targetObj.NetworkId) return;
                var PacketType = PacketData.ReadByte();
                PacketData.Position += 4;
                if (PacketData.ReadInteger() != Player.NetworkId) return;
                if (PacketType == 12) SkillQ.Cast(targetObj.Position, PacketCast);
                return;
            }
        }

        private void OnGameSendPacket(GamePacketEventArgs args)
        {
            if (!PacketCast) return;
            if (args.PacketData[0] == 153)
            {
                var PacketData = new GamePacket(args.PacketData[0]);
                PacketData.Position = 1;
                if (PacketData.ReadFloat() != Player.NetworkId) return;
                if (PacketData.ReadByte() != 0) return;
                switch (Config.Item(Name + "cancelAni").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        Packet.C2S.Emote.Encoded(new Packet.C2S.Emote.Struct(0));
                        break;
                    case 1:
                        Packet.C2S.Emote.Encoded(new Packet.C2S.Emote.Struct(1));
                        break;
                    case 2:
                        if (targetObj != null)
                        {
                            var pos = targetObj.Position + Vector3.Normalize(Player.Position - targetObj.Position) * (Player.Distance(targetObj) + 50);
                            Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(pos.X, pos.Y)).Process();
                        }
                        break;
                }
            }
        }

        private void OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead) return;
            PacketCast = Config.Item(Name + "packetCast").GetValue<bool>();
            switch (LXOrbwalker.CurrentMode)
            {
                case LXOrbwalker.Mode.Combo:
                    //NormalCombo();
                    break;
                case LXOrbwalker.Mode.Harass:
                    //Harass();
                    break;
                case LXOrbwalker.Mode.LaneClear:
                    //LaneJungClear();
                    break;
                case LXOrbwalker.Mode.LaneFreeze:
                    break;
            }
        }

        private void OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            if (Config.Item(Name + "DrawQ").GetValue<bool>() && SkillQ.Level > 0) Utility.DrawCircle(Player.Position, SkillQ.Range, SkillQ.IsReady() ? Color.Green : Color.Red);
            if (Config.Item(Name + "DrawW").GetValue<bool>() && SkillW.Level > 0) Utility.DrawCircle(Player.Position, SkillW.Range, SkillW.IsReady() ? Color.Green : Color.Red);
            if (Config.Item(Name + "DrawE").GetValue<bool>() && SkillE.Level > 0) Utility.DrawCircle(Player.Position, SkillE.Range, SkillE.IsReady() ? Color.Green : Color.Red);
            if (Config.Item(Name + "DrawR").GetValue<bool>() && SkillR.Level > 0) Utility.DrawCircle(Player.Position, SkillR.Range, SkillR.IsReady() ? Color.Green : Color.Red);
        }

        private void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!Config.Item(Name + "useAntiW").GetValue<bool>()) return;
            if (gapcloser.Sender.IsValidTarget(SkillW.Range) && SkillW.IsReady()) SkillW.Cast();
        }

        private void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (!Config.Item(Name + "useInterW").GetValue<bool>()) return;
            if (SkillW.IsReady() && SkillE.IsReady() && !unit.IsValidTarget(SkillW.Range) && unit.IsValidTarget(SkillE.Range)) SkillE.Cast(unit.Position, PacketCast);
            if (unit.IsValidTarget(SkillW.Range) && SkillW.IsReady()) SkillW.Cast();
        }
    }
}