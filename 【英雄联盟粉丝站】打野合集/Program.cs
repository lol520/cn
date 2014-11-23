using System;
using System.Linq;
using System.Collections.Generic;
using Color = System.Drawing.Color;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using LX_Orbwalker;

namespace Master
{
    class Program
    {
        public static Obj_AI_Hero Player = ObjectManager.Player, targetObj = null;
        private static TargetSelector selectTarget;
        public static Spell SkillQ, SkillW, SkillE, SkillR;
        private static SpellDataInst FData, SData, IData;
        public static Int32 Tiamat = 3077, Hydra = 3074, Blade = 3153, Bilge = 3144, Rand = 3143, Youmuu = 3142;
        public static Menu Config;
        public static String Name;
        public static Boolean PacketCast = false;

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad(EventArgs args)
        {
            Game.PrintChat("<font color = \"#00bfff\">鎵撻噹鍚堥泦</font> by <font color = \"#9370db\">Brian</font>");
            Game.PrintChat("<font color = \"#ffa500\">杞藉叆鎴愬姛,</font> <font color = \"#ff4500\">鑻遍泟鑱旂洘绮変笣绔欙紙LOL520.cc鍔╂偍娓告垙鎰夊揩锛侊級</font>");
            Name = Player.ChampionName;
            Config = new Menu("鎵撻噹鍚堥泦- " + Name, "Master_" + Name, true);

            Config.AddSubMenu(new Menu("鐩爣閫夋嫨", "TSSettings"));
            Config.SubMenu("TSSettings").AddItem(new MenuItem("tsMode", "妯″紡").SetValue(new StringList(new[] { "|鑷姩|", "|鏈€澶欰D|", "|鏈€澶欰P|", "|鏈€灏戞敾鍑粅", "|鏈€灏戝钩A|", "|鏈€浣嶩P|", "|鏈€杩憒", "|榧犳爣闄勮繎|" })));
            Config.SubMenu("TSSettings").AddItem(new MenuItem("tsFocus", "寮哄埗鐩爣").SetValue(true));
            Config.SubMenu("TSSettings").AddItem(new MenuItem("tsDraw", "鏄剧ず鐩爣").SetValue(true));
            selectTarget = new TargetSelector(2000, TargetSelector.TargetingMode.AutoPriority);

            var OWMenu = new Menu("璧扮爫", "Orbwalker");
            LXOrbwalker.AddToMenu(OWMenu);
            Config.AddSubMenu(OWMenu);

            try
            {
                if (Activator.CreateInstance(null, "Master." + Name) != null)
                {
                    var QData = Player.Spellbook.GetSpell(SpellSlot.Q);
                    var WData = Player.Spellbook.GetSpell(SpellSlot.W);
                    var EData = Player.Spellbook.GetSpell(SpellSlot.E);
                    var RData = Player.Spellbook.GetSpell(SpellSlot.R);
                    //Game.PrintChat("{0}/{1}/{2}/{3}", QData.SData.CastRange[0], WData.SData.CastRange[0], EData.SData.CastRange[0], RData.SData.CastRange[0]);
                    FData = Player.SummonerSpellbook.GetSpell(Player.GetSpellSlot("summonerflash"));
                    SData = Player.SummonerSpellbook.GetSpell(Player.GetSpellSlot("summonersmite"));
                    IData = Player.SummonerSpellbook.GetSpell(Player.GetSpellSlot("summonerdot"));
                    Game.OnGameUpdate += OnGameUpdate;
                    Drawing.OnDraw += OnDraw;
                    SkinChanger(null, null);
                }
            }
            catch
            {
                Game.PrintChat("[Master Series] => {0} Not Support !", Name);
            }
            Config.AddToMainMenu();
        }

        private static void OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead) return;
            targetObj = GetTarget();
            var newTarget = Hud.SelectedUnit;
            if (newTarget is Obj_AI_Hero && (newTarget as Obj_AI_Hero).IsValidTarget(2000) && (newTarget as Obj_AI_Hero).Health > 0) targetObj = (Obj_AI_Hero)newTarget;
            LXOrbwalker.ForcedTarget = Config.Item("tsFocus").GetValue<bool>() ? targetObj : null;
        }

        private static void OnDraw(EventArgs args)
        {
            if (Player.IsDead || !Config.Item("tsDraw").GetValue<bool>() || targetObj == null) return;
            Utility.DrawCircle(targetObj.Position, 130, Color.Red);
        }

        private static Obj_AI_Hero GetTarget()
        {
            switch (Config.Item("tsMode").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    selectTarget.SetTargetingMode(TargetSelector.TargetingMode.AutoPriority);
                    break;
                case 1:
                    selectTarget.SetTargetingMode(TargetSelector.TargetingMode.MostAD);
                    break;
                case 2:
                    selectTarget.SetTargetingMode(TargetSelector.TargetingMode.MostAP);
                    break;
                case 3:
                    selectTarget.SetTargetingMode(TargetSelector.TargetingMode.LessAttack);
                    break;
                case 4:
                    selectTarget.SetTargetingMode(TargetSelector.TargetingMode.LessCast);
                    break;
                case 5:
                    selectTarget.SetTargetingMode(TargetSelector.TargetingMode.LowHP);
                    break;
                case 6:
                    selectTarget.SetTargetingMode(TargetSelector.TargetingMode.Closest);
                    break;
                case 7:
                    selectTarget.SetTargetingMode(TargetSelector.TargetingMode.NearMouse);
                    break;
            }
            return selectTarget.Target;
        }

        public static void Orbwalk(Obj_AI_Base target)
        {
            LXOrbwalker.Orbwalk(Game.CursorPos, (target != null && LXOrbwalker.InAutoAttackRange(target)) ? target : null);
        }

        public static bool CanKill(Obj_AI_Base target, Spell Skill, int Stage = 0)
        {
            return (Skill.GetHealthPrediction(target) + 35 < Skill.GetDamage(target, Stage)) ? true : false;
        }

        public static void SkinChanger(object sender, OnValueChangeEventArgs e)
        {
            Utility.DelayAction.Add(35, () => Packet.S2C.UpdateModel.Encoded(new Packet.S2C.UpdateModel.Struct(Player.NetworkId, Config.Item(Name + "SkinID").GetValue<Slider>().Value, Name)).Process());
        }

        public static List<Obj_AI_Base> CheckingCollision(Obj_AI_Base from, Obj_AI_Base target, Spell Skill, bool Mid = true, bool OnlyHero = false)
        {
            var ListCol = new List<Obj_AI_Base>();
            foreach (var Obj in ObjectManager.Get<Obj_AI_Base>().Where(i => i.IsValidTarget(Skill.Range) && (!Mid || (Mid && Skill.GetPrediction(i).Hitchance >= HitChance.Medium)) && ((!OnlyHero && i is Obj_AI_Minion) || (OnlyHero && i is Obj_AI_Hero)) && i != target))
            {
                var Segment = (Mid ? Obj : target).Position.To2D().ProjectOn(from.Position.To2D(), Mid ? target.Position.To2D() : Obj.Position.To2D());
                if (Segment.IsOnSegment)
                {
                    if (Mid)
                    {
                        if (Obj.Position.Distance(new Vector3(Segment.SegmentPoint.X, Obj.Position.Y, Segment.SegmentPoint.Y)) <= Obj.BoundingRadius + Skill.Width) ListCol.Add(Obj);
                    }
                    else
                    {
                        if (Obj.Distance(Segment.LinePoint) <= target.BoundingRadius + Skill.Width) ListCol.Add(Obj);
                    }
                }
            }
            return ListCol.Distinct().ToList();
        }

        public static bool SmiteCollision(Obj_AI_Hero target, Spell Skill)
        {
            if (!SmiteReady()) return false;
            var Col1 = CheckingCollision(Player, target, Skill);
            if (Col1.Count == 0 || Col1.Count > 1) return false;
            if (Skill.InRange(target.Position) && Col1.First() is Obj_AI_Minion)
            {
                if (CastSmite(Col1.First()))
                {
                    Skill.Cast(Skill.GetPrediction(target).CastPosition, PacketCast);
                    return true;
                }
            }
            return false;
        }

        public static bool FlashReady()
        {
            return (FData != null && FData.Slot != SpellSlot.Unknown && FData.State == SpellState.Ready);
        }

        public static bool SmiteReady()
        {
            return (SData != null && SData.Slot != SpellSlot.Unknown && SData.State == SpellState.Ready);
        }

        public static bool IgniteReady()
        {
            return (IData != null && IData.Slot != SpellSlot.Unknown && IData.State == SpellState.Ready);
        }

        public static bool CastFlash(Vector3 pos)
        {
            return (FlashReady() && Player.SummonerSpellbook.CastSpell(FData.Slot, pos));
        }

        public static bool CastSmite(Obj_AI_Base target)
        {
            return (SmiteReady() && target.IsValidTarget(SData.SData.CastRange[0]) && target.Health <= Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Smite) && Player.SummonerSpellbook.CastSpell(SData.Slot, target));
        }

        public static bool CastIgnite(Obj_AI_Hero target)
        {
            return (IgniteReady() && target.IsValidTarget(IData.SData.CastRange[0]) && HealthPrediction.GetHealthPrediction(target, (int)(Player.Distance(target) / 1500 * 1000 + 250)) + 35 < Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) && Player.SummonerSpellbook.CastSpell(IData.Slot, target));
        }

        public static InventorySlot GetWardSlot()
        {
            Int32[] wardId = { 3340, 3361, 3205, 3207, 3154, 3160, 2049, 2045, 2050, 2044 };
            foreach (var Id in wardId.Where(i => Items.CanUseItem(i))) return Player.InventoryItems.First(i => i.Id == (ItemId)Id);
            return null;
        }
    }
}