﻿#region LICENSE

// Copyright 2014 - 2014 Support
// Thresh.cs is part of Support.
// Support is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// Support is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with Support. If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Collision = LeagueSharp.Common.Collision;

#endregion

namespace Support.Plugins
{
    public class Thresh : PluginBase
    {
        private const int QFollowTime = 3000;
        private Obj_AI_Hero _qTarget;
        private int _qTick;

        public Thresh()
        {
            Q = new Spell(SpellSlot.Q, 1025);
            W = new Spell(SpellSlot.W, 950);
            E = new Spell(SpellSlot.E, 400);
            R = new Spell(SpellSlot.R, 400);

            Q.SetSkillshot(0.5f, 70f, 1900, true, SkillshotType.SkillshotCircle);
        }

        private bool FollowQ
        {
            get { return Environment.TickCount <= _qTick + QFollowTime; }
        }

        private bool FollowQBlock
        {
            get { return Environment.TickCount - _qTick >= QFollowTime; }
        }

        public override void OnUpdate(EventArgs args)
        {
            if (_qTarget != null)
                if (Environment.TickCount - _qTick >= QFollowTime)
                    _qTarget = null;

            if (ComboMode)
            {
                if (Q.CastCheck(Target, "ComboQ") && FollowQBlock)
                {
                    if (Q.Cast(Target, UsePackets) == Spell.CastStates.SuccessfullyCasted)
                    {
                        _qTick = Environment.TickCount;
                        _qTarget = Target;
                    }
                }
                if (Q.CastCheck(_qTarget, "ComboQFollow"))
                {
                    if (FollowQ)
                        Q.Cast();
                }

                if (W.CastCheck(Target, "ComboW"))
                {
                    EngageFriendLatern();
                }

                if (E.CastCheck(Target, "ComboE"))
                {
                    if (Helpers.AllyBelowHp(ConfigValue<Slider>("ComboHealthE").Value, E.Range) != null)
                    {
                        E.Cast(Target.Position, UsePackets);
                    }
                    else
                    {
                        E.Cast(Helpers.ReversePosition(ObjectManager.Player.Position, Target.Position), UsePackets);
                    }
                }

                if (R.CastCheck(Target, "ComboR"))
                {
                    if (Helpers.EnemyInRange(ConfigValue<Slider>("ComboCountR").Value, R.Range))
                        R.Cast();
                }
            }

            if (HarassMode)
            {
                if (Q.CastCheck(Target, "HarassQ") && FollowQBlock)
                {
                    Q.Cast(Target, UsePackets);
                }

                if (W.CastCheck(Target, "HarassW"))
                {
                    SafeFriendLatern();
                }

                if (E.CastCheck(Target, "HarassE"))
                {
                    if (Helpers.AllyBelowHp(ConfigValue<Slider>("HarassHealthE").Value, E.Range) != null)
                    {
                        E.Cast(Target.Position, UsePackets);
                    }
                    else
                    {
                        E.Cast(Helpers.ReversePosition(ObjectManager.Player.Position, Target.Position), UsePackets);
                    }
                }
            }
        }

        public override void OnBeforeEnemyAttack(BeforeEnemyAttackEventArgs args)
        {
            if (Q.CastCheck(args.Caster, "Misc.Q.OnAttack") && (ComboMode || HarassMode) &&
                args.Caster == Target && args.Type == Packet.AttackTypePacket.TargetedAA)
            {
                var collision = Collision.GetCollision(new List<Vector3> {args.Caster.Position},
                    new PredictionInput {Delay = 0.5f, Radius = 70, Speed = 1900});

                if (collision.Count == 0)
                {
                    Q.Cast(args.Caster.Position, UsePackets);
                }
            }
        }

        public override void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsAlly)
                return;

            if (E.CastCheck(gapcloser.Sender, "GapcloserE"))
            {
                E.Cast(gapcloser.Start, UsePackets);
            }
        }

        public override void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (spell.DangerLevel < InterruptableDangerLevel.High || unit.IsAlly)
                return;

            if (E.CastCheck(unit, "InterruptE"))
            {
                E.Cast(unit.Position, UsePackets);
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "浣跨敤 Q", true);
            config.AddBool("ComboQFollow", "浣跨敤 Q 璺熼殢", true);
            config.AddBool("ComboW", "浣跨敤 W 鎺ラ槦鍙嬨劎", true);
            config.AddBool("ComboE", "浣跨敤 E", true);
            config.AddSlider("ComboHealthE", "濡傛灉鐩爣琛€閲忎綆浜庛劎", 20, 1, 100);
            config.AddBool("ComboR", "浣跨敤 R", true);
            config.AddSlider("ComboCountR", "鏁屼汉鏁伴噺浣跨敤澶ф嫑", 2, 1, 5);
        }

        public override void HarassMenu(Menu config)
        {
            config.AddBool("HarassQ", "浣跨敤 Q", true);
            config.AddBool("HarassW", "浣跨敤 W 鏁戦槦鍙嬨劎", true);
            config.AddBool("HarassE", "浣跨敤 E", true);
            config.AddSlider("HarassHealthE", "濡傛灉鐩爣琛€閲忎綆浜庛劎", 20, 1, 100);
        }

        public override void MiscMenu(Menu config)
        {
            config.AddBool("Misc.Q.OnAttack", "瀵圭洰鏍囦娇鐢ㄥ嬀", true);
        }

        public override void InterruptMenu(Menu config)
        {
            config.AddBool("GapcloserE", "浣跨敤 E 闃茬獊杩涖劎", true);

            config.AddBool("InterruptE", "浣跨敤 E 鎵撴柇", true);
        }

        /// <summary>
        ///     Credit
        ///     https://github.com/LXMedia1/UltimateCarry2/blob/master/LexxersAIOCarry/Thresh.cs
        /// </summary>
        private void EngageFriendLatern()
        {
            if (!W.IsReady())
                return;

            var bestcastposition = new Vector3(0f, 0f, 0f);

            foreach (var friend in ObjectManager.Get<Obj_AI_Hero>()
                .Where(hero => hero.IsAlly && !hero.IsMe && hero.Distance(Player) <= W.Range + 300 &&
                               hero.Distance(Player) <= W.Range - 300 && hero.Health/hero.MaxHealth*100 >= 20 &&
                               Utility.CountEnemysInRange(150) >= 1))
            {
                var center = Player.Position;
                const int points = 36;
                var radius = W.Range;
                const double slice = 2*Math.PI/points;

                for (var i = 0; i < points; i++)
                {
                    var angle = slice*i;
                    var newX = (int) (center.X + radius*Math.Cos(angle));
                    var newY = (int) (center.Y + radius*Math.Sin(angle));
                    var p = new Vector3(newX, newY, 0);
                    if (p.Distance(friend.Position) <= bestcastposition.Distance(friend.Position))
                        bestcastposition = p;
                }

                if (friend.Distance(ObjectManager.Player) <= W.Range)
                {
                    W.Cast(bestcastposition, true);
                    return;
                }
            }

            if (bestcastposition.Distance(new Vector3(0f, 0f, 0f)) >= 100)
                W.Cast(bestcastposition, true);
        }

        /// <summary>
        ///     Credit
        ///     https://github.com/LXMedia1/UltimateCarry2/blob/master/LexxersAIOCarry/Thresh.cs
        /// </summary>
        private void SafeFriendLatern()
        {
            if (!W.IsReady())
                return;

            var bestcastposition = new Vector3(0f, 0f, 0f);

            foreach (var friend in ObjectManager.Get<Obj_AI_Hero>()
                .Where(hero => hero.IsAlly && !hero.IsMe && hero.Distance(ObjectManager.Player) <= W.Range + 300 &&
                               hero.Distance(ObjectManager.Player) <= W.Range - 200 &&
                               hero.Health/hero.MaxHealth*100 >= 20 && !hero.IsDead))
            {
                foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(h => h.IsEnemy))
                {
                    if (friend == null || !(friend.Distance(enemy) <= 300))
                        continue;

                    var center = ObjectManager.Player.Position;
                    const int points = 36;
                    var radius = W.Range;
                    const double slice = 2*Math.PI/points;

                    for (var i = 0; i < points; i++)
                    {
                        var angle = slice*i;
                        var newX = (int) (center.X + radius*Math.Cos(angle));
                        var newY = (int) (center.Y + radius*Math.Sin(angle));
                        var p = new Vector3(newX, newY, 0);
                        if (p.Distance(friend.Position) <= bestcastposition.Distance(friend.Position))
                            bestcastposition = p;
                    }

                    if (friend.Distance(ObjectManager.Player) <= W.Range)
                    {
                        W.Cast(bestcastposition, true);
                        return;
                    }
                }

                if (bestcastposition.Distance(new Vector3(0f, 0f, 0f)) >= 100)
                    W.Cast(bestcastposition, true);
            }
        }
    }
}