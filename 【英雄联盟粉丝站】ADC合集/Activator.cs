using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marksman
{
    internal class BuffList
    {
        public string ChampionName { get; set; }
        public string DisplayName { get; set; }
        public string BuffName { get; set; }
        public bool DefaultValue { get; set; }
    }

    internal class Activator
    {
        public readonly List<BuffList> BuffList = new List<BuffList>();

        public Activator()
        {
            BuffList.Add(new BuffList { ChampionName = "Darius", DisplayName = "Darius (W)", BuffName = "DariusNoxianTacticsONH", DefaultValue = true });
            BuffList.Add(new BuffList { ChampionName = "Diana", DisplayName = "Diana (Q)", BuffName = "DianaArc", DefaultValue = true });
            BuffList.Add(new BuffList { ChampionName = "Fizz", DisplayName = "Fizz (R)", BuffName = "FizzMarinerDoom", DefaultValue = true });
            BuffList.Add(new BuffList { ChampionName = "Galio", DisplayName = "Galio (R)", BuffName = "GalioIdolOfDurand", DefaultValue = true });
            BuffList.Add(new BuffList { ChampionName = "LeBlanc", DisplayName = "LeBlanc (E)", BuffName = "LeblancSoulShackle", DefaultValue = true });
            BuffList.Add(new BuffList { ChampionName = "Malzahar", DisplayName = "Malzahar (R)", BuffName = "AlZaharNetherGrasp", DefaultValue = true });
            BuffList.Add(new BuffList { ChampionName = "Mordekaiser", DisplayName = "Mordekaiser (R)", BuffName = "MordekaiserChildrenOfTheGrave", DefaultValue = true });
            BuffList.Add(new BuffList { ChampionName = "Nocturne", DisplayName = "Nocturne (R)", BuffName = "NocturneParanoia", DefaultValue = true });
            BuffList.Add(new BuffList { ChampionName = "Poppy", DisplayName = "Poppy (R)", BuffName = "PoppyDiplomaticImmunity", DefaultValue = true });
            //BuffList.Add(new BuffList { ChampionName = "Rammus", DisplayName = "Rammus (E)", BuffName = "PuncturingTaunt", DefaultValue = true });
            BuffList.Add(new BuffList { ChampionName = "TwistedFate", DisplayName = "Twisted Fate (R)", BuffName = "Destiny", DefaultValue = true });
            BuffList.Add(new BuffList { ChampionName = "Skarner", DisplayName = "Skarner (R)", BuffName = "SkarnerImpale", DefaultValue = true });
            BuffList.Add(new BuffList { ChampionName = "Urgot", DisplayName = "Urgot (R)", BuffName = "UrgotSwap2", DefaultValue = true });
            BuffList.Add(new BuffList { ChampionName = "Vladimir", DisplayName = "Vladimir (R)", BuffName = "VladimirHemoplague", DefaultValue = true });
            BuffList.Add(new BuffList { ChampionName = "Warwick", DisplayName = "Warwick (R)", BuffName = "InfiniteDuress", DefaultValue = true });
            //BuffList.Add(new BuffList { ChampionName = "Morgana", DisplayName = "Morgana (Q)", BuffName = "DarkBindingMissile", DefaultValue = true });
            BuffList.Add(new BuffList { ChampionName = "Zilean", DisplayName = "Zilean (Q)", BuffName = "timebombenemybuff", DefaultValue = false });
            BuffList.Add(new BuffList { ChampionName = "Zed", DisplayName = "Zed (R)", BuffName = "zedulttargetmark", DefaultValue = true });
        }
    }
    
}
