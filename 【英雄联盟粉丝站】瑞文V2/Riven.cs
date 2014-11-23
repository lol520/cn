using System;
using System.Collections.Generic;
using System.Linq;

using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;
using Rive;
using SharpDX;

namespace RivenSharp
{
    class Riven
    {
        public static Obj_AI_Hero Player = ObjectManager.Player;

        public static Spellbook sBook = Player.Spellbook;

        public static SpellDataInst Qdata = sBook.GetSpell(SpellSlot.Q);
        public static SpellDataInst Wdata = sBook.GetSpell(SpellSlot.W);
        public static SpellDataInst Edata = sBook.GetSpell(SpellSlot.E);
        public static SpellDataInst Rdata = sBook.GetSpell(SpellSlot.R);
        public static Spell Q = new Spell(SpellSlot.Q, 280);
        public static Spell Q2 = new Spell(SpellSlot.Q, 280);
        public static Spell Q3 = new Spell(SpellSlot.Q, 280);
        public static Spell W = new Spell(SpellSlot.W, 260);
        public static Spell E = new Spell(SpellSlot.E, 390);
        public static Spell R = new Spell(SpellSlot.R, 900);


        public static SummonerItems sumItems;


        public static int qStage = 0;

        public static bool rushDown = false;

        public static bool rushDownQ = false;

        public static bool forceQ = false;

        public static void setSkillshots()
        {
            R.SetSkillshot(0.25f, 300f, 1400f, false, SkillshotType.SkillshotCone);
            sumItems = new SummonerItems(Player);
        }


        public static void doCombo(Obj_AI_Base target)
        {
            if (target == null)
                return;
            if (target.Distance(Player) < 500)
            {
                sumItems.cast(SummonerItems.ItemIds.Ghostblade);
            }
            if (target.Distance(Player) < 500 && (Player.Health/Player.MaxHealth)*100<85)
            {
                sumItems.cast(SummonerItems.ItemIds.BotRK, target);

            }

            igniteIfKIllable(target);
            rushDownQ = rushDmgBasedOnDist(target) * 0.7f > target.Health;
            rushDown = rushDmgBasedOnDist(target)*1.1f > target.Health;
            useRSmart(target);
            if (rushDown)
                sumItems.castIgnite((Obj_AI_Hero)target);
            useESmart(target);
            useWSmart(target);
            useHydra(target);
            gapWithQ(target);
        }

        public static void doHarasQ(Obj_AI_Base target)
        {
            if (target == null)
                return;
            float dist = Player.Distance(target.ServerPosition);


            //W loic
            if (dist < (W.Range+50) && W.IsReady())
            {
                W.Cast();
                Riven.useHydra(target);
            }
            //QLogic
            if (getQJumpCount() > 0)
            {   
                Player.IssueOrder(GameObjectOrder.AttackUnit, target);

                if (!Player.IsChanneling && Q.IsReady() && dist > Player.AttackRange+Player.BoundingRadius/* && (getQJumpCount()*175 + target.BoundingRadius + 100) < dist*/)
                {
                    Q.Cast(target.Position);
                }
            }//Get away logic
            else if (dist < (target.AttackRange + target.BoundingRadius+150))
            {
                Obj_AI_Turret closest_tower =
                           ObjectManager.Get<Obj_AI_Turret>()
                               .Where(tur => tur.IsAlly)
                               .OrderBy(tur => tur.Distance(Player.Position))
                               .First();
                Player.IssueOrder(GameObjectOrder.MoveTo, closest_tower.ServerPosition);
                if (E.IsReady())
                {
                    E.Cast(closest_tower.Position);
                }
            }
        }

        public static void doHarasE(Obj_AI_Base target)
        {
            if (target == null)
                return;
            float dist = Player.Distance(target.ServerPosition);
            if (E.IsReady() && dist <540)
                E.Cast(target.Position);
            if (dist < (W.Range + 50) && W.IsReady() && !E.IsReady())
            {
                W.Cast();
                Riven.useHydra(target);
            }
            if (getQJumpCount() > 1 && !E.IsReady())
            {
                Player.IssueOrder(GameObjectOrder.AttackUnit, target);

                if (!Player.IsChanneling && Q.IsReady() && dist > Player.AttackRange + Player.BoundingRadius/* && (getQJumpCount()*175 + target.BoundingRadius + 100) < dist*/)
                {
                    Q.Cast(target.Position);
                }
            }//Get away
            else if (getQJumpCount() == 1 && !W.IsReady())
            {
                Obj_AI_Turret closest_tower =
                          ObjectManager.Get<Obj_AI_Turret>()
                              .Where(tur => tur.IsAlly)
                              .OrderBy(tur => tur.Distance(Player.Position))
                              .First();
                Player.IssueOrder(GameObjectOrder.MoveTo, closest_tower.ServerPosition);
                if (Q.IsReady() && isRunningTo(closest_tower))
                {
                    Q.Cast(closest_tower.Position);
                }
            }
        }

        public static void gapWithQ(Obj_AI_Base target)
        {
            if ((E.IsReady() || !Q.IsReady() || Player.IsAutoAttacking || !LXOrbwalker.CanAttack() || target.Distance(Player.ServerPosition) < Player.AttackRange) && !rushDownQ)
                return;
            reachWithQ(target);
        }

        public static void AfterAttack(Obj_AI_Base unit, Obj_AI_Base target)
        {
        }

        public static  void useWSmart(Obj_AI_Base target, bool aaRange =false)
        {
            if (!W.IsReady())
                return;
            float range = 0;
            if (aaRange)
                range = Player.AttackRange + target.BoundingRadius;
            else
                range = W.Range + target.BoundingRadius - 40;
            if (W.IsReady() && target.Distance(Player.ServerPosition) <range)
            {
                W.Cast();
            }

        }

        public static void useESmart(Obj_AI_Base target)
        {
            if (!E.IsReady())
                return;

           

            float trueAARange = Player.AttackRange + target.BoundingRadius;
            float trueERange = target.BoundingRadius + E.Range;



            float dist = Player.Distance(target); 

            var path = Player.GetPath(target.Position);
            if (!target.IsMoving && dist < trueERange)
            {
                Player.IssueOrder(GameObjectOrder.MoveTo, target.Position);
                E.Cast(path.Count() > 1 ? path[1] : target.ServerPosition);
            }
            if ((dist > trueAARange && dist < trueERange )|| rushDown)
            {
                
                E.Cast(path.Count() > 1 ? path[1] : target.ServerPosition);
            }
        }

        public static void useRSmart(Obj_AI_Base target)
        {
            if (!R.IsReady())
                return;
            if (!ultIsOn() && RivenSharp.Config.Item("useR").GetValue<KeyBind>().Active && !E.IsReady() && target.Distance(Player.ServerPosition) < (Q.Range + target.BoundingRadius))
            {
                R.Cast();
            }
            else if (canUseWindSlash() && target is Obj_AI_Hero && (!(E.IsReady() && Player.IsDashing()) || Player.Distance(target) > 150))
            {   
                var targ = target as Obj_AI_Hero;   
                PredictionOutput po = R.GetPrediction(targ, true);
                if (getTrueRDmgOn(targ) > ((targ.Health)) || rushDown)
                {
                    if (po.Hitchance > HitChance.Medium && Player.Distance(po.UnitPosition) > 30)
                    {
                        R.Cast(Player.Distance(po.UnitPosition) < 150 ? target.Position : po.UnitPosition);
                    }
                }
            }
        }


        public static bool useHydra(Obj_AI_Base target)
        {
            if (target.Distance(Player.ServerPosition) < (400 + target.BoundingRadius-20))
            {
                sumItems.cast(SummonerItems.ItemIds.Tiamat);
                sumItems.cast(SummonerItems.ItemIds.Hydra);
                return true;
            }
            return false;
        }

        public static Vector3 difPos()
        {
            Vector3 pPos = Player.ServerPosition;
            return pPos + new Vector3(300, 300, 0);
        }

        public static void reachWithQ(Obj_AI_Base target)
        {
            if (!Q.IsReady() || Player.IsDashing())
                return;

            float trueAARange = Player.AttackRange + target.BoundingRadius+20;
            float trueQRange = target.BoundingRadius + Q.Range+30;

            float dist = Player.Distance(target);
            Vector2 walkPos = new Vector2();
            if (target.IsMoving && target.Path.Count() != 0)
            {
                Vector2 tpos = target.Position.To2D();
                Vector2 path = target.Path[0].To2D() - tpos;
                path.Normalize();
                walkPos = tpos + (path * 100);
            }
            float targ_ms = (target.IsMoving && Player.Distance(walkPos) > dist) ? target.MoveSpeed : 0;
            float msDif = (Player.MoveSpeed - targ_ms) == 0 ? 0.0001f : (Player.MoveSpeed - targ_ms);
            float timeToReach = (dist - trueAARange) / msDif;
            if ((dist > trueAARange && dist < trueQRange) || rushDown)
            {
                if (timeToReach > 2.5 || timeToReach < 0.0f || rushDown)
                {
                    Q.Cast(target.ServerPosition);
                }
            }
        }

        public static void reachWithE(Obj_AI_Base target)
        {
            if (!E.IsReady())
                return;
            float trueAARange = Player.AttackRange + target.BoundingRadius;
            float trueERange = target.BoundingRadius + E.Range;

            float dist = Player.Distance(target);
            Vector2 walkPos = new Vector2();
            if (target.IsMoving)
            {
                Vector2 tpos = target.Position.To2D();
                Vector2 path = target.Path[0].To2D() - tpos;
                path.Normalize();
                walkPos = tpos + (path * 100);
            }
            float targ_ms = (target.IsMoving && Player.Distance(walkPos) > dist) ? target.MoveSpeed : 0;
            float msDif = (Player.MoveSpeed - targ_ms) == 0 ? 0.0001f : (Player.MoveSpeed - targ_ms);
            float timeToReach = (dist - trueAARange) / msDif;
            if (dist > trueAARange && (dist < trueERange || rushDown))
            {
                if (timeToReach > 1.7f || timeToReach < 0.0f)
                {
                    E.Cast(target.ServerPosition);
                }
            }
        }


        public static float getTrueRDmgOn(Obj_AI_Base target, float minus = 0)
        {
            float baseDmg = 40 + 40 * R.Level + 0.6f * Player.FlatPhysicalDamageMod;
            float eneMissHpProc =  ((((target.MaxHealth - target.Health - minus)/target.MaxHealth)*100f)>75f)?75f:(((target.MaxHealth - target.Health)/target.MaxHealth)*100f);

            float multiplier = 1+(eneMissHpProc*2.66f)/100;

            return (float)Player.CalcDamage(target,Damage.DamageType.Physical, baseDmg * multiplier);
        }

        public static void moveTo(Vector3 pos)
        {
            if (RivenSharp.Config.Item("forceQE").GetValue<bool>())
                Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(pos.X, pos.Y)).Send();
            else
                Player.IssueOrder(GameObjectOrder.MoveTo,pos);
        }

        public static void cancelAnim(bool aaToo=false)
        {
            if (aaToo)
            {
                LXOrbwalker.ResetAutoAttackTimer();
            }
            if (LXOrbwalker.GetPossibleTarget() != null && !Riven.useHydra(LXOrbwalker.GetPossibleTarget()))
            {
                if (W.IsReady())
                    Riven.useWSmart(LXOrbwalker.GetPossibleTarget());

            }
            moveTo(Game.CursorPos);
            //Game.Say("/l");


           //  Packet.C2S.Cast.Encoded(new Packet.C2S.Cast.Struct(fill iterator up)).Send();
        }

        public static bool isRunningTo(Obj_AI_Base target)
        {
            Obj_AI_Turret closest_tower =
                             ObjectManager.Get<Obj_AI_Turret>()
                                 .Where(tur => tur.IsAlly)
                                 .OrderBy(tur => tur.Distance(Player.Position))
                                 .First();
            float dist = Player.Distance(closest_tower);
            Vector2 walkPos = new Vector2();
            if (Player.Path.Length > 0)
            {
               
                Vector3 run = Player.Position + Vector3.Normalize(Player.Path[0] - Player.Position)*100;

                return (dist > run.Distance(closest_tower.Position));
            }
            return false;
        }

        public static bool isImmobileTarg(Obj_AI_Base target)
        {
            foreach (BuffInstance buff in target.Buffs)
            {
                if (buff.Type == BuffType.Knockback || buff.Type == BuffType.Knockup || buff.Type == BuffType.Fear
                     || buff.Type == BuffType.Stun || buff.Type == BuffType.Taunt)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ultIsOn()
        {
            foreach (var buf in Player.Buffs)
            {
                if (buf.Name == "RivenFengShuiEngine")
                {
                    return true;
                }
            }
            return false;
        }

        public static bool canUseWindSlash()
        {
            foreach (var buf in Player.Buffs)
            {
                if (buf.Name == "rivenwindslashready")
                {
                    return true;
                }
            }
            return false;
        }

        public static int getQJumpCount()
        {
            try
            {
                var buff = Player.Buffs.First(buf => buf.Name == "RivenTriCleave");

                return 3-buff.Count;
            }
            catch (Exception ex)
            {
                if (!Q.IsReady())
                    return 0;
                return 3;
            }
        }

        public static float getTrueQDmOn(Obj_AI_Base target)
        {
            return (float)Player.CalcDamage(target, Damage.DamageType.Physical, -10 + (Q.Level * 20) +
                                                                          (0.35 + (Q.Level*0.05))*
                                                                          (Player.FlatPhysicalDamageMod +
                                                                           Player.BaseAttackDamage));
        }

        public static float rushDmgBasedOnDist(Obj_AI_Base target)
        {
            float multi = 1.0f;
            if (!ultIsOn() && R.IsReady())
                multi = 1.2f;
            float Qdmg = getTrueQDmOn(target);
            float Wdmg = (E.IsReady())?(float)Player.GetSpellDamage(target, SpellSlot.W):0;
            float ADdmg = (float)Player.GetAutoAttackDamage(target);
            float Rdmg = (R.IsReady() && (canUseWindSlash() || !ultIsOn())) ? getTrueRDmgOn(target) : 0;

            float trueAARange = Player.AttackRange + target.BoundingRadius-15;
            float dist = Player.Distance(target.ServerPosition);
            float Ecan = (E.IsReady()) ? E.Range : 0;
            int Qtimes = getQJumpCount();
            int ADtimes = 0;

            if (E.IsReady())
                ADtimes++;
            

            dist -= Ecan;
            dist -= trueAARange;
            while (dist > 0 && Qtimes>0)
            {
                dist -= Player.AttackRange+50;
                Qtimes--;
            }
            if (dist<0)
                ADtimes++;
            
            //Console.WriteLine("times: "+Qtimes);
            //Console.WriteLine("Q: " + Qdmg );
            //Console.WriteLine("W: " + Wdmg);
            //Console.WriteLine("AD: " + ADdmg * ADtimes);
            return (Qdmg * Qtimes + Wdmg + ADdmg * ADtimes + Rdmg) * multi;
        }

        public static void igniteIfKIllable(Obj_AI_Base target)
        {
           // Console.WriteLine("cast ignite");
            if (target is Obj_AI_Hero)
            {
                if (target.Health < 50 + 20*Player.Level)
                    sumItems.castIgnite((Obj_AI_Hero)target);
            }
        }

    }
}
