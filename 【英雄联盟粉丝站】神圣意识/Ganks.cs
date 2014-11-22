using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;

namespace SAwareness
{
    internal class GankPotentialTracker
    {
        private readonly Dictionary<Obj_AI_Hero, double> _enemies = new Dictionary<Obj_AI_Hero, double>();
        private readonly Line _line;
        private bool _drawActive = true;

        public GankPotentialTracker()
        {
            _line = new Line(Drawing.Direct3DDevice);
            _line.Antialias = true;
            _line.Width = 2;
            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (hero.IsEnemy)
                {
                    _enemies.Add(hero, 0);
                }
            }
            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnPreReset += Drawing_OnPreReset;
            Drawing.OnPostReset += Drawing_OnPostReset;
            AppDomain.CurrentDomain.DomainUnload += delegate { Drawing_OnPreReset(new EventArgs()); };
            AppDomain.CurrentDomain.ProcessExit += delegate { Drawing_OnPreReset(new EventArgs()); };
            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        ~GankPotentialTracker()
        {
            Game.OnGameUpdate -= Game_OnGameUpdate;
            Drawing.OnPreReset -= Drawing_OnPreReset;
            Drawing.OnPostReset -= Drawing_OnPostReset;
            Drawing.OnEndScene -= Drawing_OnEndScene;
        }

        public bool IsActive()
        {
            return Menu.Ganks.GetActive() && Menu.GankTracker.GetActive();
        }

        private void Drawing_OnPostReset(EventArgs args)
        {
            if (Drawing.Direct3DDevice == null || Drawing.Direct3DDevice.IsDisposed)
                return;
            _line.OnResetDevice();
            _drawActive = true;
        }

        private void Drawing_OnPreReset(EventArgs args)
        {
            _line.OnLostDevice();
            _drawActive = false;
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            Obj_AI_Hero player = ObjectManager.Player;
            foreach (var enemy in _enemies.ToList())
            {
                double dmg = 0;
                try
                {
                    if (player.Spellbook.CanUseSpell(SpellSlot.Q) == SpellState.Ready)
                        dmg += player.GetSpellDamage(enemy.Key, SpellSlot.Q);
                }
                catch (InvalidOperationException)
                {
                }
                try
                {
                    if (player.Spellbook.CanUseSpell(SpellSlot.W) == SpellState.Ready)
                        dmg += player.GetSpellDamage(enemy.Key, SpellSlot.W);
                }
                catch (InvalidOperationException)
                {
                }
                try
                {
                    if (player.Spellbook.CanUseSpell(SpellSlot.E) == SpellState.Ready)
                        dmg += player.GetSpellDamage(enemy.Key, SpellSlot.E);
                }
                catch (InvalidOperationException)
                {
                }
                try
                {
                    if (player.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready)
                        dmg += player.GetSpellDamage(enemy.Key, SpellSlot.R);
                }
                catch (InvalidOperationException)
                {
                }
                try
                {
                    dmg += player.GetAutoAttackDamage(enemy.Key);
                }
                catch (InvalidOperationException)
                {
                }
                _enemies[enemy.Key] = dmg;
            }
        }

        private void Drawing_OnEndScene(EventArgs args)
        {
            if (!IsActive() || !_drawActive)
                return;
            Vector2 myPos = Drawing.WorldToScreen(ObjectManager.Player.ServerPosition);
            foreach (var enemy in _enemies)
            {
                if (enemy.Key.IsDead || ObjectManager.Player.IsDead)
                    continue;
                if(Vector3.Distance(ObjectManager.Player.ServerPosition, enemy.Key.ServerPosition) >
                    Menu.GankTracker.GetMenuItem("SAwarenessGankTrackerTrackRange").GetValue<Slider>().Value)
                    continue;
                Vector2 ePos = Drawing.WorldToScreen(enemy.Key.ServerPosition);
                _line.Begin();
                if (enemy.Value > enemy.Key.Health)
                {
                    //Drawing.DrawLine(myPos.X, myPos.Y, ePos.X, ePos.Y, 2.0f, System.Drawing.Color.OrangeRed);
                    //DirectXDrawer.DrawLine(line, ObjectManager.Player.ServerPosition, enemy.Key.ServerPosition,
                    //    Color.OrangeRed);
                    _line.Draw(new[] {myPos, ePos}, Color.OrangeRed);
                    //DirectXDrawer.DrawLine(ObjectManager.Player.ServerPosition, enemy.Key.ServerPosition,
                    //    System.Drawing.Color.OrangeRed);
                }
                if (enemy.Value < enemy.Key.Health && !Menu.GankTracker.GetMenuItem("SAwarenessGankTrackerKillable").GetValue<bool>())
                {
                    //Drawing.DrawLine(myPos.X, myPos.Y, ePos.X, ePos.Y, 2.0f, System.Drawing.Color.GreenYellow);
                    //DirectXDrawer.DrawLine(line, ObjectManager.Player.ServerPosition, enemy.Key.ServerPosition,
                    //    Color.GreenYellow);
                    _line.Draw(new[] {myPos, ePos}, Color.GreenYellow);
                    //DirectXDrawer.DrawLine(ObjectManager.Player.ServerPosition, enemy.Key.ServerPosition,
                    //    System.Drawing.Color.GreenYellow);
                }
                else if (enemy.Key.Health/enemy.Key.MaxHealth < 0.1)
                {
                    //Drawing.DrawLine(myPos.X, myPos.Y, ePos.X, ePos.Y, 2.0f, System.Drawing.Color.Red);
                    //DirectXDrawer.DrawLine(line, ObjectManager.Player.ServerPosition, enemy.Key.ServerPosition, Color.Red);
                    _line.Draw(new[] {myPos, ePos}, Color.Red);
                    //DirectXDrawer.DrawLine(ObjectManager.Player.ServerPosition, enemy.Key.ServerPosition,
                    //    System.Drawing.Color.Red);
                }
                _line.End();
            }
        }
    }

    internal class GankDetector
    {
        private static readonly Dictionary<Obj_AI_Hero, Time> Enemies = new Dictionary<Obj_AI_Hero, Time>();
        private readonly Render.Text _textF = new Render.Text("", 0, 0, 24, SharpDX.Color.Red);
        private bool _drawActive = true;

        public GankDetector()
        {
            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (hero.IsEnemy)
                {
                    Enemies.Add(hero, new Time());
                }
            }
            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Drawing.OnPreReset += Drawing_OnPreReset;
            Drawing.OnPostReset += Drawing_OnPostReset;
        }

        ~GankDetector()
        {
            Game.OnGameUpdate -= Game_OnGameUpdate;
            Drawing.OnEndScene -= Drawing_OnEndScene;
            Drawing.OnPreReset -= Drawing_OnPreReset;
            Drawing.OnPostReset -= Drawing_OnPostReset;
        }

        public bool IsActive()
        {
            return Menu.Ganks.GetActive() && Menu.GankDetector.GetActive();
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (!IsActive())
                return;
            foreach (var enemy in Enemies)
            {
                UpdateTime(enemy);
            }
        }

        private void Drawing_OnPostReset(EventArgs args)
        {
            _textF.OnPostReset();
            _drawActive = true;
        }

        private void Drawing_OnPreReset(EventArgs args)
        {
            _textF.OnPreReset();
            _drawActive = false;
        }

        private void Drawing_OnEndScene(EventArgs args)
        {
            if (!IsActive() || !_drawActive || !Menu.GankDetector.GetMenuItem("SAwarenessGankDetectorShowJungler").GetValue<bool>())
                return;

            foreach (var enemy in Enemies)
            {
                Obj_AI_Hero hero = enemy.Key;
                if(!hero.IsValid)
                    continue;
                bool hasSmite = false;
                foreach (SpellDataInst spell in hero.SummonerSpellbook.Spells)
                {
                    if (spell.Name.ToLower().Contains("smite"))
                    {
                        hasSmite = true;
                        break;
                    }
                }
                if (enemy.Key.IsVisible && !enemy.Key.IsDead &&
                    Vector3.Distance(ObjectManager.Player.ServerPosition, hero.ServerPosition) >
                    Menu.GankDetector.GetMenuItem("SAwarenessGankDetectorTrackRangeMin").GetValue<Slider>().Value &&
                    Vector3.Distance(ObjectManager.Player.ServerPosition, hero.ServerPosition) <
                    Menu.GankDetector.GetMenuItem("SAwarenessGankDetectorTrackRangeMax").GetValue<Slider>().Value &&
                    hasSmite)
                {
                    String killText = "Enemy jungler approaching";
                    _textF.Centered = true;
                    _textF.text = killText;
                    _textF.X = (int)Drawing.WorldToScreen(ObjectManager.Player.ServerPosition).X;
                    _textF.Y = (int)Drawing.WorldToScreen(ObjectManager.Player.ServerPosition).Y;
                    _textF.OutLined = true;
                    _textF.OnEndScene();
                }
            }
        }

        private void ChatAndPing(KeyValuePair<Obj_AI_Hero, Time> enemy)
        {
            Obj_AI_Hero hero = enemy.Key;
            var pingType = Packet.PingType.Normal;
            var t = Menu.GankDetector.GetMenuItem("SAwarenessGankDetectorPingType").GetValue<StringList>();
            pingType = (Packet.PingType) t.SelectedIndex + 1;
            Vector3 pos = hero.ServerPosition;
            GamePacket gPacketT;
            for (int i = 0;
                i < Menu.GankDetector.GetMenuItem("SAwarenessGankDetectorPingTimes").GetValue<Slider>().Value;
                i++)
            {
                if (Menu.GankDetector.GetMenuItem("SAwarenessGankDetectorLocalPing").GetValue<bool>())
                {
                    gPacketT = Packet.S2C.Ping.Encoded(new Packet.S2C.Ping.Struct(pos[0], pos[1], 0, 0, pingType));
                    gPacketT.Process();
                }
                else if (!Menu.GankDetector.GetMenuItem("SAwarenessGankDetectorLocalPing").GetValue<bool>() &&
                         Menu.GlobalSettings.GetMenuItem("SAwarenessGlobalSettingsServerChatPingActive")
                             .GetValue<bool>())
                {
                    gPacketT = Packet.C2S.Ping.Encoded(new Packet.C2S.Ping.Struct(pos[0], pos[1], 0, pingType));
                    gPacketT.Send();
                }
            }

            if (
                Menu.GankDetector.GetMenuItem("SAwarenessGankDetectorChatChoice").GetValue<StringList>().SelectedIndex ==
                1)
            {
                Game.PrintChat("Gank: {0}", hero.ChampionName);
            }
            else if (
                Menu.GankDetector.GetMenuItem("SAwarenessGankDetectorChatChoice")
                    .GetValue<StringList>()
                    .SelectedIndex == 2 &&
                Menu.GlobalSettings.GetMenuItem("SAwarenessGlobalSettingsServerChatPingActive").GetValue<bool>())
            {
                Game.Say("Gank: {0}", hero.ChampionName);
            }

            //TODO: Check for Teleport etc.                    
        }

        private void HandleGank(KeyValuePair<Obj_AI_Hero, Time> enemy)
        {
            Obj_AI_Hero hero = enemy.Key;
            if (enemy.Value.InvisibleTime > 5)
            {
                if (!enemy.Value.CalledInvisible && hero.IsValid && !hero.IsDead && hero.IsVisible &&
                    Vector3.Distance(ObjectManager.Player.ServerPosition, hero.ServerPosition) >
                    Menu.GankDetector.GetMenuItem("SAwarenessGankDetectorTrackRangeMin").GetValue<Slider>().Value &&
                    Vector3.Distance(ObjectManager.Player.ServerPosition, hero.ServerPosition) <
                    Menu.GankDetector.GetMenuItem("SAwarenessGankDetectorTrackRangeMax").GetValue<Slider>().Value)
                {
                    ChatAndPing(enemy);
                    enemy.Value.CalledInvisible = true;
                }
            }
            if (!enemy.Value.CalledVisible && hero.IsValid && !hero.IsDead &&
                enemy.Key.GetWaypoints().Last().Distance(ObjectManager.Player.ServerPosition) >
                Menu.GankDetector.GetMenuItem("SAwarenessGankDetectorTrackRangeMin").GetValue<Slider>().Value &&
                enemy.Key.GetWaypoints().Last().Distance(ObjectManager.Player.ServerPosition) <
                Menu.GankDetector.GetMenuItem("SAwarenessGankDetectorTrackRangeMax").GetValue<Slider>().Value)
            {
                ChatAndPing(enemy);
                enemy.Value.CalledVisible = true;
            }
        }

        private void UpdateTime(KeyValuePair<Obj_AI_Hero, Time> enemy)
        {
            Obj_AI_Hero hero = enemy.Key;
            if (!hero.IsValid)
                return;
            if (hero.IsVisible)
            {
                HandleGank(enemy);
                Enemies[hero].InvisibleTime = 0;
                Enemies[hero].VisibleTime = (int) Game.Time;
                enemy.Value.CalledInvisible = false;
            }
            else
            {
                if (Enemies[hero].VisibleTime != 0)
                {
                    Enemies[hero].InvisibleTime = (int) (Game.Time - Enemies[hero].VisibleTime);
                }
                else
                {
                    Enemies[hero].InvisibleTime = 0;
                }
                enemy.Value.CalledVisible = false;
            }
        }

        public class Time
        {
            public bool CalledInvisible;
            public bool CalledVisible;
            public int InvisibleTime;
            public int VisibleTime;
        }
    }
}