﻿#region LICENSE

// Copyright 2014 - 2014 Support
// AntiGapcloser.cs is part of Support.
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

#endregion

namespace Support
{
    public delegate void OnGapcloseH(ActiveGapcloser gapcloser);

    public enum GapcloserType
    {
        Skillshot,
        Targeted
    }

    public struct Gapcloser
    {
        public string ChampionName;
        public GapcloserType SkillType;
        public SpellSlot Slot;
        public int Speed;
        public string SpellName;
    }

    public struct ActiveGapcloser
    {
        public Vector3 End;
        public int EndTick;
        public Obj_AI_Base Sender;
        public GapcloserType SkillType;
        public Vector3 Start;
        public int StartTick;
    }

    public static class AntiGapcloser
    {
        public static List<Gapcloser> Spells = new List<Gapcloser>();
        public static List<ActiveGapcloser> ActiveGapclosers = new List<ActiveGapcloser>();

        static AntiGapcloser()
        {
            #region Aatrox

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Aatrox",
                    Slot = SpellSlot.Q,
                    SpellName = "aatroxq",
                    SkillType = GapcloserType.Skillshot
                });

            #endregion

            #region Akali

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Akali",
                    Slot = SpellSlot.R,
                    SpellName = "akalishadowdance",
                    SkillType = GapcloserType.Targeted
                });

            #endregion

            #region Alistar

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Alistar",
                    Slot = SpellSlot.E,
                    SpellName = "headbutt",
                    SkillType = GapcloserType.Targeted
                });

            #endregion

            #region LeeSin

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "LeeSin",
                    Slot = SpellSlot.Q,
                    SpellName = "blindmonkqtwo",
                    SkillType = GapcloserType.Skillshot
                });

            #endregion

            #region Diana

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Diana",
                    Slot = SpellSlot.R,
                    SpellName = "dianateleport",
                    SkillType = GapcloserType.Targeted
                });

            #endregion

            #region Elise

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Elise",
                    Slot = SpellSlot.Q,
                    SpellName = "elisespiderqcast",
                    SkillType = GapcloserType.Skillshot
                });

            #endregion

            #region Fiora

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Fiora",
                    Slot = SpellSlot.Q,
                    SpellName = "fioraq",
                    SkillType = GapcloserType.Targeted
                });

            #endregion

            #region Fizz

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Fizz",
                    Slot = SpellSlot.Q,
                    SpellName = "fizzpiercingstrike",
                    SkillType = GapcloserType.Targeted
                });

            #endregion

            #region Gnar

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Gnar",
                    Slot = SpellSlot.E,
                    SpellName = "gnarbige",
                    SkillType = GapcloserType.Skillshot
                });

            #endregion

            #region Gragas

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Gragas",
                    Slot = SpellSlot.E,
                    SpellName = "gragase",
                    SkillType = GapcloserType.Skillshot
                });

            #endregion

            #region Hecarim

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Hecarim",
                    Slot = SpellSlot.R,
                    SpellName = "hecarimult",
                    SkillType = GapcloserType.Skillshot
                });

            #endregion

            #region Irelia

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Irelia",
                    Slot = SpellSlot.Q,
                    SpellName = "ireliagatotsu",
                    SkillType = GapcloserType.Targeted
                });

            #endregion

            #region JarvanIV

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "JarvanIV",
                    Slot = SpellSlot.Q,
                    SpellName = "jarvanivdragonstrike",
                    SkillType = GapcloserType.Skillshot
                });

            #endregion

            #region Jax

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Jax",
                    Slot = SpellSlot.Q,
                    SpellName = "jaxleapstrike",
                    SkillType = GapcloserType.Targeted
                });

            #endregion

            #region Khazix

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Khazix",
                    Slot = SpellSlot.E,
                    SpellName = "khazixe",
                    SkillType = GapcloserType.Skillshot
                });

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Khazix",
                    Slot = SpellSlot.E,
                    SpellName = "khazixelong",
                    SkillType = GapcloserType.Skillshot
                });

            #endregion

            #region LeBlanc

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "LeBlanc",
                    Slot = SpellSlot.W,
                    SpellName = "leblancslide",
                    SkillType = GapcloserType.Skillshot
                });

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "LeBlanc",
                    Slot = SpellSlot.R,
                    SpellName = "leblancslidem",
                    SkillType = GapcloserType.Skillshot
                });

            #endregion

            #region Leona

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Leona",
                    Slot = SpellSlot.Q,
                    SpellName = "leonazenithblade",
                    SkillType = GapcloserType.Skillshot
                });

            #endregion

            #region MonkeyKing

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "MonkeyKing",
                    Slot = SpellSlot.E,
                    SpellName = "monkeykingnimbus",
                    SkillType = GapcloserType.Targeted
                });

            #endregion

            #region Pantheon

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Pantheon",
                    Slot = SpellSlot.W,
                    SpellName = "pantheon_leapbash",
                    SkillType = GapcloserType.Targeted
                });

            #endregion

            #region Poppy

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Poppy",
                    Slot = SpellSlot.E,
                    SpellName = "poppyheroiccharge",
                    SkillType = GapcloserType.Targeted
                });

            #endregion

            #region Renekton

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Renekton",
                    Slot = SpellSlot.E,
                    SpellName = "renektonsliceanddice",
                    SkillType = GapcloserType.Skillshot
                });

            #endregion

            #region Riven

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Riven",
                    Slot = SpellSlot.Q,
                    SpellName = "riventricleave",
                    SkillType = GapcloserType.Skillshot
                });

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Riven",
                    Slot = SpellSlot.E,
                    SpellName = "rivenfeint",
                    SkillType = GapcloserType.Skillshot
                });

            #endregion

            #region Sejuani

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Sejuani",
                    Slot = SpellSlot.Q,
                    SpellName = "sejuaniarcticassault",
                    SkillType = GapcloserType.Skillshot
                });

            #endregion

            #region Malphite

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Malphite",
                    Slot = SpellSlot.R,
                    SpellName = "ufslash",
                    SkillType = GapcloserType.Skillshot
                });

            #endregion

            #region Vi

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Vi",
                    Slot = SpellSlot.Q,
                    SpellName = "viq",
                    SkillType = GapcloserType.Skillshot
                });

            #endregion

            #region XinZhao

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "XinZhao",
                    Slot = SpellSlot.E,
                    SpellName = "xenzhaosweep",
                    SkillType = GapcloserType.Targeted
                });

            #endregion

            #region Yasuo

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Yasuo",
                    Slot = SpellSlot.E,
                    SpellName = "yasuodashwrapper",
                    SkillType = GapcloserType.Targeted
                });

            #endregion

            #region Tryndamere

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Tryndamere",
                    Slot = SpellSlot.E,
                    SpellName = "slashcast",
                    SkillType = GapcloserType.Skillshot
                });

            #endregion

            #region Lucian

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Lucian",
                    Slot = SpellSlot.E,
                    SpellName = "luciane",
                    SkillType = GapcloserType.Skillshot
                });

            #endregion

            #region Tristana

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Tristana",
                    Slot = SpellSlot.W,
                    SpellName = "RocketJump",
                    SkillType = GapcloserType.Skillshot
                });

            #endregion

            #region Zac

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Zac",
                    Slot = SpellSlot.E,
                    SpellName = "zace",
                    SkillType = GapcloserType.Skillshot
                });

            #endregion

            #region Ziggs

            Spells.Add(
                new Gapcloser
                {
                    ChampionName = "Ziggs",
                    Slot = SpellSlot.W,
                    SpellName = "ziggswtoggle",
                    SkillType = GapcloserType.Skillshot
                });

            #endregion

            Game.OnGameUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        public static event OnGapcloseH OnEnemyGapcloser;

        private static void Game_OnGameUpdate(EventArgs args)
        {
            ActiveGapclosers.RemoveAll(entry => Environment.TickCount > entry.EndTick);

            if (OnEnemyGapcloser == null)
            {
                return;
            }

            foreach (var gapcloser in ActiveGapclosers)
            {
                if (gapcloser.SkillType == GapcloserType.Targeted ||
                    (gapcloser.SkillType == GapcloserType.Skillshot &&
                     ObjectManager.Get<Obj_AI_Hero>()
                         .Any(
                             h =>
                                 h.IsAlly && !h.IsDead &&
                                 (h.Distance(gapcloser.Start) < 800 || h.Distance(gapcloser.End) < 800))))
                {
                    if (gapcloser.Sender.IsValidTarget())
                    {
                        OnEnemyGapcloser(gapcloser);
                    }
                }
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!SpellIsGapcloser(args))
            {
                return;
            }

            ActiveGapclosers.Add(
                new ActiveGapcloser
                {
                    Start = args.Start,
                    End = args.End,
                    Sender = sender,
                    StartTick = Environment.TickCount,
                    EndTick = Environment.TickCount + 900, // TODO: calc duration with speed
                    SkillType = GetGapcloser(args).SkillType
                });
        }

        private static bool SpellIsGapcloser(GameObjectProcessSpellCastEventArgs args)
        {
            return Spells.Any(spell => spell.SpellName == args.SData.Name.ToLower());
        }

        private static Gapcloser GetGapcloser(GameObjectProcessSpellCastEventArgs args)
        {
            return Spells.SingleOrDefault(spell => spell.SpellName == args.SData.Name.ToLowerInvariant());
        }
    }
}