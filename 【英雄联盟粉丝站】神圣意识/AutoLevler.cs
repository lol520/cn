using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using LeagueSharp;
using LeagueSharp.Common;

namespace SAwareness
{
    internal class AutoLevler
    {
        private int[] _priority = {0, 0, 0, 0};
        private int[] _sequence;
        private int _useMode;

        public AutoLevler()
        {
            //LoadLevelFile();
            Game.OnGameUpdate += Game_OnGameUpdate;
        }

        ~AutoLevler()
        {
            Game.OnGameUpdate -= Game_OnGameUpdate;
        }

        public bool IsActive()
        {
            return Menu.Misc.GetActive() && Menu.AutoLevler.GetActive();
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (!IsActive())
                return;

            var stringList = Menu.AutoLevler.GetMenuItem("SAwarenessAutoLevlerSMode").GetValue<StringList>();
            if (stringList.SelectedIndex == 0)
            {
                _useMode = 0;
                _priority = new[]
                {
                    Menu.AutoLevler.GetMenuSettings("SAwarenessAutoLevlerPriority")
                        .GetMenuItem("SAwarenessAutoLevlerPrioritySliderQ").GetValue<Slider>().Value,
                    Menu.AutoLevler.GetMenuSettings("SAwarenessAutoLevlerPriority")
                        .GetMenuItem("SAwarenessAutoLevlerPrioritySliderW").GetValue<Slider>().Value,
                    Menu.AutoLevler.GetMenuSettings("SAwarenessAutoLevlerPriority")
                        .GetMenuItem("SAwarenessAutoLevlerPrioritySliderE").GetValue<Slider>().Value,
                    Menu.AutoLevler.GetMenuSettings("SAwarenessAutoLevlerPriority")
                        .GetMenuItem("SAwarenessAutoLevlerPrioritySliderR").GetValue<Slider>().Value
                };
            }
            else if (stringList.SelectedIndex == 1)
            {
                _useMode = 1;
            }
            else
            {
                _useMode = 2;
            }

            Obj_AI_Hero player = ObjectManager.Player;
            SpellSlot[] spellSlotst = GetSortedPriotitySlots();
            if (player.SpellTrainingPoints > 0)
            {
                //TODO: Add level logic// try levelup spell, if fails level another up etc.
                if (_useMode == 0 && Menu.AutoLevler.GetMenuSettings("SAwarenessAutoLevlerPriority")
                    .GetMenuItem("SAwarenessAutoLevlerPriorityActive").GetValue<bool>())
                {
                    if (Menu.AutoLevler.GetMenuSettings("SAwarenessAutoLevlerPriority")
                        .GetMenuItem("SAwarenessAutoLevlerPriorityFirstSpellsActive").GetValue<bool>())
                    {
                        player.Spellbook.LevelUpSpell(GetCurrentSpell());
                        return;
                    }
                    SpellSlot[] spellSlots = GetSortedPriotitySlots();
                    for (int slotId = 0; slotId <= 3; slotId++)
                    {
                        int spellLevel = player.Spellbook.GetSpell(spellSlots[slotId]).Level;
                        player.Spellbook.LevelUpSpell(spellSlots[slotId]);
                        if (player.Spellbook.GetSpell(spellSlots[slotId]).Level != spellLevel)
                            break;
                    }
                }
                else if (_useMode == 1)
                {
                    if (Menu.AutoLevler.GetMenuSettings("SAwarenessAutoLevlerSequence")
                        .GetMenuItem("SAwarenessAutoLevlerSequenceActive").GetValue<bool>())
                    {
                        SpellSlot spellSlot = GetSpellSlot(_sequence[player.Level - 1]);
                        player.Spellbook.LevelUpSpell(spellSlot);
                    }
                }
                else
                {
                    if (Menu.AutoLevler.GetMenuItem("SAwarenessAutoLevlerSMode").GetValue<StringList>().SelectedIndex == 2)
                    {
                        if (ObjectManager.Player.Level == 6 ||
                            ObjectManager.Player.Level == 11 ||
                            ObjectManager.Player.Level == 16)
                        {
                            player.Spellbook.LevelUpSpell(SpellSlot.R);
                        }
                    }
                }
            }
        }

        public void SetPriorities(int priorityQ, int priorityW, int priorityE, int priorityR)
        {
            _sequence[0] = priorityQ;
            _sequence[1] = priorityW;
            _sequence[2] = priorityE;
            _sequence[3] = priorityR;
        }

        private void LoadLevelFile()
        {
            //TODO: Read Level File for sequence leveling.
            string loc = Assembly.GetExecutingAssembly().Location;
            loc = loc.Remove(loc.LastIndexOf("\\", StringComparison.Ordinal));
            loc = loc + "\\Config\\SAwareness\\autolevel.conf";
            if (!File.Exists(loc))
            {
                //Download.DownloadFile("127.0.0.1", loc);
            }
            try
            {
                StreamReader sr = File.OpenText(loc);
                ReadLevelFile(sr);
            }
            catch (Exception)
            {
                Console.WriteLine("Couldn't load autolevel.conf. Using priority mode.");
                _useMode = 0;
            }
        }

        private void ReadLevelFile(StreamReader streamReader)
        {
            var sequence = new int[18];
            while (!streamReader.EndOfStream)
            {
                String line = streamReader.ReadLine();
                String champion = "";
                if (line != null && line.Length > line.IndexOf("="))
                    champion = line.Remove(line.IndexOf("="));
                if (!champion.Contains(ObjectManager.Player.ChampionName))
                    continue;
                if (line != null)
                {
                    string temp = line.Remove(0, line.IndexOf("=") + 2);
                    for (int i = 0; i < 18; i++)
                    {
                        sequence[i] = Int32.Parse(temp.Remove(1));
                        temp = temp.Remove(0, 1);
                    }
                }
                break;
            }
            _sequence = sequence;
        }

        private SpellSlot GetSpellSlot(int id)
        {
            var spellSlot = SpellSlot.Unknown;
            switch (id)
            {
                case 0:
                    spellSlot = SpellSlot.Q;
                    break;

                case 1:
                    spellSlot = SpellSlot.W;
                    break;

                case 2:
                    spellSlot = SpellSlot.E;
                    break;

                case 3:
                    spellSlot = SpellSlot.R;
                    break;
            }
            return spellSlot;
        }

        private SpellSlot[] GetSortedPriotitySlots()
        {
            int[] listOld = _priority;
            var listNew = new SpellSlot[4];

            listNew = ToSpellSlot(listOld, listNew);

            //listNew = listNew.OrderByDescending(c => c).ToList();


            return listNew;
        }

        private SpellSlot[] ToSpellSlot(int[] listOld, SpellSlot[] listNew)
        {
            for (int i = 0; i <= 3; i++)
            {
                switch (listOld[i])
                {
                    case 0:
                        listNew[0] = GetSpellSlot(i);
                        break;

                    case 1:
                        listNew[1] = GetSpellSlot(i);
                        break;

                    case 2:
                        listNew[2] = GetSpellSlot(i);
                        break;

                    case 3:
                        listNew[3] = GetSpellSlot(i);
                        break;
                }
            }
            return listNew;
        }

        private SpellSlot GetCurrentSpell()
        {
            SpellSlot[] spellSlot = null;
            switch (Menu.AutoLevler.GetMenuSettings("SAwarenessAutoLevlerPriority")
                .GetMenuItem("SAwarenessAutoLevlerPriorityFirstSpells").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    spellSlot = new[] {SpellSlot.Q, SpellSlot.W, SpellSlot.E};
                    break;
                case 1:
                    spellSlot = new[] { SpellSlot.Q, SpellSlot.E, SpellSlot.W };
                    break;
                case 2:
                    spellSlot = new[] { SpellSlot.W, SpellSlot.Q, SpellSlot.E };
                    break;
                case 3:
                    spellSlot = new[] { SpellSlot.W, SpellSlot.E, SpellSlot.Q };
                    break;
                case 4:
                    spellSlot = new[] { SpellSlot.E, SpellSlot.Q, SpellSlot.W };
                    break;
                case 5:
                    spellSlot = new[] { SpellSlot.E, SpellSlot.W, SpellSlot.Q };
                    break;
            }
            return spellSlot[ObjectManager.Player.Level - 1];
        }

        //private List<SpellSlot> SortAlgo(List<int> listOld, List<SpellSlot> listNew)
        //{
        //    int highestPriority = -1;
        //    for (int i = 0; i < listOld.Count; i++)
        //    {
        //        int prio = _priority[i];
        //        if (highestPriority < prio)
        //        {
        //            highestPriority = prio;
        //            listNew.Add(GetSpellSlot(i));
        //            listOld.Remove(_priority[i]);
        //        }
        //    }
        //    if (listOld.Count > 1)
        //        listNew = SortAlgo(listOld, listNew);
        //    return listNew;
        //}
    }
}