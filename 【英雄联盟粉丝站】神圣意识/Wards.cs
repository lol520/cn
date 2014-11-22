using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace SAwareness
{
    internal class Wards
    {
        public enum WardType
        {
            Stealth,
            Vision,
            Temp,
            TempVision
        }

        public static readonly List<WardItem> WardItems = new List<WardItem>();

        static Wards()
        {
            WardItems.Add(new WardItem(3360, "Feral Flare", "", 1000, 180, WardType.Stealth));
            WardItems.Add(new WardItem(2043, "Vision Ward", "VisionWard", 600, 180, WardType.Vision));
            WardItems.Add(new WardItem(2044, "Stealth Ward", "SightWard", 600, 180, WardType.Stealth));
            WardItems.Add(new WardItem(3154, "Wriggle's Lantern", "WriggleLantern", 600, 180, WardType.Stealth));
            WardItems.Add(new WardItem(2045, "Ruby Sightstone", "ItemGhostWard", 600, 180, WardType.Stealth));
            WardItems.Add(new WardItem(2049, "Sightstone", "ItemGhostWard", 600, 180, WardType.Stealth));
            WardItems.Add(new WardItem(2050, "Explorer's Ward", "ItemMiniWard", 600, 60, WardType.Stealth));
            WardItems.Add(new WardItem(3340, "Greater Stealth Totem", "", 600, 120, WardType.Stealth));
            WardItems.Add(new WardItem(3361, "Greater Stealth Totem", "", 600, 180, WardType.Stealth));
            WardItems.Add(new WardItem(3362, "Greater Vision Totem", "", 600, 180, WardType.Vision));
            WardItems.Add(new WardItem(3366, "Bonetooth Necklace", "", 600, 120, WardType.Stealth));
            WardItems.Add(new WardItem(3367, "Bonetooth Necklace", "", 600, 120, WardType.Stealth));
            WardItems.Add(new WardItem(3368, "Bonetooth Necklace", "", 600, 120, WardType.Stealth));
            WardItems.Add(new WardItem(3369, "Bonetooth Necklace", "", 600, 120, WardType.Stealth));
            WardItems.Add(new WardItem(3371, "Bonetooth Necklace", "", 600, 180, WardType.Stealth));
            WardItems.Add(new WardItem(3375, "Head of Kha'Zix", "", 600, 180, WardType.Stealth));
            WardItems.Add(new WardItem(3205, "Quill Coat", "", 600, 180, WardType.Stealth));
            WardItems.Add(new WardItem(3207, "Spirit of the Ancient Golem", "", 600, 180, WardType.Stealth));
            WardItems.Add(new WardItem(3342, "Scrying Orb", "", 2500, 2, WardType.Temp));
            WardItems.Add(new WardItem(3363, "Farsight Orb", "", 4000, 2, WardType.Temp));
            WardItems.Add(new WardItem(3187, "Hextech Sweeper", "", 800, 5, WardType.TempVision));
            WardItems.Add(new WardItem(3159, "Grez's Spectral Lantern", "", 800, 5, WardType.Temp));
            WardItems.Add(new WardItem(3364, "Oracle's Lens", "", 600, 10, WardType.TempVision));
        }

        public static WardItem GetWardItem()
        {
            return WardItems.FirstOrDefault(x => Items.HasItem(x.Id) && Items.CanUseItem(x.Id));
        }

        public static InventorySlot GetWardSlot()
        {
            foreach (WardItem ward in WardItems)
            {
                if (Items.CanUseItem(ward.Id))
                {
                    return ObjectManager.Player.InventoryItems.FirstOrDefault(slot => slot.Id == (ItemId) ward.Id);
                }
            }
            return null;
        }

        public class WardItem
        {
            public readonly int Id;
            public int Duration;
            public String Name;
            public int Range;
            public String SpellName;
            public WardType Type;

            public WardItem(int id, string name, string spellName, int range, int duration, WardType type)
            {
                Id = id;
                Name = name;
                SpellName = spellName;
                Range = range;
                Duration = duration;
                Type = type;
            }
        }
    }

    internal class InvisibleRevealer //TODO: Check for other Wards
    {
        private readonly List<String> _spellList = new List<string>();
        private int _lastTimeVayne;
        private int _lastTimeWarded;

        public InvisibleRevealer() //Passive Evelynn, Teemo Missing
        {
            _spellList.Add("AkaliSmokeBomb"); //Akali W
            _spellList.Add("RengarR"); //Rengar R
            _spellList.Add("KhazixR"); //Kha R
            _spellList.Add("khazixrlong"); //Kha R Evolved
            _spellList.Add("Deceive"); //Shaco Q
            _spellList.Add("TalonShadowAssault"); //Talon R
            _spellList.Add("HideInShadows"); //Twitch Q
            _spellList.Add("VayneTumble");
                //Vayne Q -> Check before if args.SData.Name == "vayneinquisition" then ability.ExtraTicks = (int)Game.Time + 6 + 2 * args.Level; if (Game.Time >= ability.ExtraTicks) return;
            _spellList.Add("MonkeyKingDecoy"); //Wukong W

            Obj_AI_Base.OnProcessSpellCast += ObjAiBase_OnProcessSpellCast;
        }

        ~InvisibleRevealer()
        {
            Obj_AI_Base.OnProcessSpellCast -= ObjAiBase_OnProcessSpellCast;
        }

        public bool IsActive()
        {
            return Menu.Wards.GetActive() && Menu.InvisibleRevealer.GetActive();
        }

        private void ObjAiBase_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!IsActive())
                return;

            var mode =
                Menu.InvisibleRevealer.GetMenuItem("SAwarenessInvisibleRevealerMode")
                    .GetValue<StringList>();

            if (sender.IsEnemy && sender.IsValid && !sender.IsDead)
            {
                if (args.SData.Name.ToLower().Contains("vayneinquisition"))
                {
                    _lastTimeVayne = Environment.TickCount + 6000 + 2000*args.Level;
                }
                if (mode.SelectedIndex == 0 &&
                    Menu.InvisibleRevealer.GetMenuItem("SAwarenessInvisibleRevealerKey").GetValue<KeyBind>().Active ||
                    mode.SelectedIndex == 1)
                {
                    if (_spellList.Exists(x => x.ToLower().Contains(args.SData.Name.ToLower())))
                    {
                        if (_lastTimeWarded == 0 || Environment.TickCount - _lastTimeWarded > 500)
                        {
                            Wards.WardItem wardItem =
                                Wards.WardItems.First(
                                    x =>
                                        Items.HasItem(x.Id) && Items.CanUseItem(x.Id) && (x.Type == Wards.WardType.Vision || x.Type == Wards.WardType.TempVision));
                            if (wardItem == null)
                                return;
                            if (sender.ServerPosition.Distance(ObjectManager.Player.ServerPosition) > wardItem.Range)
                                return;

                            InventorySlot invSlot =
                                ObjectManager.Player.InventoryItems.FirstOrDefault(
                                    slot => slot.Id == (ItemId) wardItem.Id);
                            if (invSlot == null)
                                return;

                            if (args.SData.Name.ToLower().Contains("vaynetumble") &&
                                Environment.TickCount >= _lastTimeVayne)
                                return;

                            invSlot.UseItem(args.End);
                            _lastTimeWarded = Environment.TickCount;
                        }
                    }
                }
            }
        }
    }

    internal class BushRevealer //By Beaving & Blm95
    {
        private readonly List<PlayerInfo> _playerInfo = new List<PlayerInfo>();
        private int _lastTimeWarded;

        public BushRevealer()
        {
            _playerInfo = ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy).Select(x => new PlayerInfo(x)).ToList();
            Game.OnGameUpdate += Game_OnGameUpdate;
        }

        ~BushRevealer()
        {
            Game.OnGameUpdate -= Game_OnGameUpdate;
        }

        public bool IsActive()
        {
            return Menu.Wards.GetActive() && Menu.BushRevealer.GetActive();
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (!IsActive())
                return;

            int time = Environment.TickCount;

            foreach (PlayerInfo playerInfo in _playerInfo.Where(x => x.Player.IsVisible))
                playerInfo.LastSeen = time;

            Wards.WardItem ward = Wards.GetWardItem();
            if (ward == null)
                return;

            if (Menu.BushRevealer.GetMenuItem("SAwarenessBushRevealerKey").GetValue<KeyBind>().Active)
            {
                foreach (Obj_AI_Hero enemy in _playerInfo.Where(x =>
                    x.Player.IsValid &&
                    !x.Player.IsVisible &&
                    !x.Player.IsDead &&
                    x.Player.Distance(ObjectManager.Player.ServerPosition) < 1000 && //check real ward range later
                    time - x.LastSeen < 2500).Select(x => x.Player))
                {
                    Vector3 bestWardPos = GetWardPos(enemy.ServerPosition, 165, 2);

                    if (bestWardPos != enemy.ServerPosition && bestWardPos != Vector3.Zero &&
                        bestWardPos.Distance(ObjectManager.Player.ServerPosition) < ward.Range)
                    {
                        if (_lastTimeWarded == 0 || Environment.TickCount - _lastTimeWarded > 500)
                        {
                            InventorySlot wardSlot = Wards.GetWardSlot();

                            if (wardSlot != null && wardSlot.Id != ItemId.Unknown)
                            {
                                wardSlot.UseItem(bestWardPos);
                                _lastTimeWarded = Environment.TickCount;
                            }
                        }
                    }
                }
            }
        }

        private Vector3 GetWardPos(Vector3 lastPos, int radius = 165, int precision = 3)
        {
            //Vector3 averagePos = Vector3.Zero;

            int count = precision;
            //int calculated = 0;

            while (count > 0)
            {
                int vertices = radius;

                var wardLocations = new WardLocation[vertices];
                double angle = 2*Math.PI/vertices;

                for (int i = 0; i < vertices; i++)
                {
                    double th = angle*i;
                    var pos = new Vector3((float) (lastPos.X + radius*Math.Cos(th)),
                        (float) (lastPos.Y + radius*Math.Sin(th)), 0); //wardPos.Z
                    wardLocations[i] = new WardLocation(pos, NavMesh.IsWallOfGrass(pos));
                }

                var grassLocations = new List<GrassLocation>();

                for (int i = 0; i < wardLocations.Length; i++)
                {
                    if (wardLocations[i].Grass)
                    {
                        if (i != 0 && wardLocations[i - 1].Grass)
                            grassLocations.Last().Count++;
                        else
                            grassLocations.Add(new GrassLocation(i, 1));
                    }
                }

                GrassLocation grassLocation = grassLocations.OrderByDescending(x => x.Count).FirstOrDefault();

                if (grassLocation != null) //else: no pos found. increase/decrease radius?
                {
                    var midelement = (int) Math.Ceiling(grassLocation.Count/2f);
                    //averagePos += wardLocations[grassLocation.Index + midelement - 1].Pos; //uncomment if using averagePos
                    lastPos = wardLocations[grassLocation.Index + midelement - 1].Pos; //comment if using averagePos
                    radius = (int) Math.Floor(radius/2f); //precision recommended: 2-3; comment if using averagePos

                    //calculated++; //uncomment if using averagePos
                }

                count--;
            }

            return lastPos; //averagePos /= calculated; //uncomment if using averagePos
        }

        private class GrassLocation
        {
            public readonly int Index;
            public int Count;

            public GrassLocation(int index, int count)
            {
                Index = index;
                Count = count;
            }
        }

        private class PlayerInfo
        {
            public readonly Obj_AI_Hero Player;
            public int LastSeen;

            public PlayerInfo(Obj_AI_Hero player)
            {
                Player = player;
            }
        }

        private class WardLocation
        {
            public readonly bool Grass;
            public readonly Vector3 Pos;

            public WardLocation(Vector3 pos, bool grass)
            {
                Pos = pos;
                Grass = grass;
            }
        }
    }

    internal class WardCorrector
    {
        private static readonly List<WardSpot> WardSpots = new List<WardSpot>();

        private bool _drawSpots;
        private SpellSlot _latestSpellSlot = SpellSlot.Unknown;
        private WardSpot _latestWardSpot;
        private bool _wardAlreadyCorrected;

        public WardCorrector() //TODO: Add SpellNames for WardItems
        {
            WardSpots.Add(new WardSpot("BlueGolem", new Vector3(3274f, 7772f, 52f)));
            WardSpots.Add(new WardSpot("BlueLizard", new Vector3(7422f, 3282f, 46.53f)));
            WardSpots.Add(new WardSpot("BlueTriBush", new Vector3(10148f, 2839f, 44.41f)));
            WardSpots.Add(new WardSpot("BluePassBush", new Vector3(6269f, 4445f, 42.51f)));
            WardSpots.Add(new WardSpot("BlueRiver", new Vector3(7151.64f, 4719.66f, 51.67f)));
            WardSpots.Add(new WardSpot("BlueRiverRoundBush", new Vector3(4728f, 8336f, -51.29f)));
            WardSpots.Add(new WardSpot("BlueRiverSplitBush", new Vector3(6762.52f, 2918.75f, 55.68f)));
            WardSpots.Add(new WardSpot("PurpleGolem", new Vector3(11217.39f, 6841.89f, 54.87f)));
            WardSpots.Add(new WardSpot("PurpleLizard", new Vector3(6610.35f, 11064.61f, 54.45f)));
            WardSpots.Add(new WardSpot("PurpleTriBush", new Vector3(3883f, 11577f, 39.87f)));
            WardSpots.Add(new WardSpot("PurplePassBush", new Vector3(7775f, 10046.49f, 43.14f)));
            WardSpots.Add(new WardSpot("PurpleRiver", new Vector3(6867.68f, 9567.63f, 57.01f)));
            WardSpots.Add(new WardSpot("PurpleRoundBush", new Vector3(9720.86f, 7501.50f, 54.85f)));
            WardSpots.Add(new WardSpot("PurpleRiverRoundBush", new Vector3(9233.13f, 6094.48f, -44.63f)));
            WardSpots.Add(new WardSpot("PurpleRiverSplitPush", new Vector3(7282.69f, 11482.53f, 52.59f)));
            WardSpots.Add(new WardSpot("Dragon", new Vector3(10180.18f, 4969.32f, -62.32f)));
            WardSpots.Add(new WardSpot("DragonBush", new Vector3(8875.13f, 5390.57f, -64.07f)));
            WardSpots.Add(new WardSpot("Baron", new Vector3(3920.88f, 9477.78f, -60.42f)));
            WardSpots.Add(new WardSpot("BaronBush", new Vector3(5017.27f, 8954.09f, -62.70f)));
            WardSpots.Add(new WardSpot("PurpleBotTower2", new Vector3(12731.25f, 9132.66f, 50.32f)));
            WardSpots.Add(new WardSpot("PurpleTopTower2", new Vector3(8036.52f, 12882.94f, 45.19f)));
            WardSpots.Add(new WardSpot("PurpleMidTower1", new Vector3(9260.02f, 8582.67f, 54.62f)));
            WardSpots.Add(new WardSpot("BlueMidTower1", new Vector3(4749.79f, 5890.76f, 53.59f)));
            WardSpots.Add(new WardSpot("BlueBotTower2", new Vector3(5983.58f, 1547.98f, 52.99f)));
            WardSpots.Add(new WardSpot("BlueTopTower2", new Vector3(1213.70f, 5324.73f, 58.77f)));

            WardSpots.Add(new WardSpot("NoName", new Vector3(9641.65f, 6368.74f, 53.01f)));
            WardSpots.Add(new WardSpot("NoName", new Vector3(8081.43f, 4683.44f, 55.94f)));
            WardSpots.Add(new WardSpot("NoName", new Vector3(5943.51f, 9792.40f, 53.18f)));
            WardSpots.Add(new WardSpot("NoName", new Vector3(4379.51f, 8093.74f, 42.73f)));
            WardSpots.Add(new WardSpot("NoName", new Vector3(4222.72f, 7038.58f, 53.61f)));
            WardSpots.Add(new WardSpot("NoName", new Vector3(9068.02f, 11186.68f, 53.22f)));
            WardSpots.Add(new WardSpot("NoName", new Vector3(7970.82f, 10005.07f, 53.52f)));
            WardSpots.Add(new WardSpot("NoName", new Vector3(4978.19f, 3042.69f, 54.34f)));
            WardSpots.Add(new WardSpot("NoName", new Vector3(7907.63f, 11629.32f, 49.94f)));
            WardSpots.Add(new WardSpot("NoName", new Vector3(7556.06f, 11739.62f, 50.61f)));
            WardSpots.Add(new WardSpot("NoName", new Vector3(5973.48f, 11115.68f, 54.34f)));
            WardSpots.Add(new WardSpot("NoName", new Vector3(5732.81f, 10289.76f, 53.39f)));
            WardSpots.Add(new WardSpot("NoName", new Vector3(7969.15f, 3307.56f, 56.94f)));
            WardSpots.Add(new WardSpot("NoName", new Vector3(12073.18f, 4795.50f, 52.32f)));
            WardSpots.Add(new WardSpot("NoName", new Vector3(4044.13f, 11600.50f, 48.59f)));
            WardSpots.Add(new WardSpot("NoName", new Vector3(5597.66f, 12491.04f, 39.73f)));
            WardSpots.Add(new WardSpot("NoName", new Vector3(10070.20f, 4132.45f, -60.33f)));
            WardSpots.Add(new WardSpot("NoName", new Vector3(8320.28f, 4292.80f, 56.47f)));
            WardSpots.Add(new WardSpot("NoName", new Vector3(9603.52f, 7872.23f, 54.71f)));

            WardSpots.Add(new WardSpot("Dragon->TriBush", new Vector3(9695f, 3465f, 43.02f),
                new Vector3(9843.38f, 3125.16f, 43.02f), new Vector3(9946.10f, 3064.81f, 43.02f),
                new Vector3(9595f, 3665f, 43.02f)));
            WardSpots.Add(new WardSpot("Nashor->TriBush", new Vector3(4346.10f, 10964.81f, 36.62f),
                new Vector3(4214.93f, 11202.01f, 36.62f), new Vector3(4146.10f, 11314.81f, 36.62f),
                new Vector3(4384.36f, 10680.41f, 36.62f)));
            WardSpots.Add(new WardSpot("BlueTop->SoloBush", new Vector3(2349f, 10387f, 44.20f),
                new Vector3(2267.97f, 10783.37f, 44.20f), new Vector3(2446.10f, 10914.81f, 44.20f),
                new Vector3(2311f, 10185f, 44.20f)));
            WardSpots.Add(new WardSpot("BlueMid->RoundBush", new Vector3(4946.52f, 6474.56f, 54.71f),
                new Vector3(4891.98f, 6639.05f, 53.62f), new Vector3(4546.10f, 6864.81f, 53.78f),
                new Vector3(5217f, 6263f, 54.95f)));
            WardSpots.Add(new WardSpot("BlueMid->RiverLaneBush", new Vector3(5528.96f, 7615.20f, 45.64f),
                new Vector3(5688.96f, 7825.20f, 45.64f), new Vector3(5796.10f, 7914.81f, 45.64f),
                new Vector3(5460.13f, 7469.77f, 45.64f)));
            WardSpots.Add(new WardSpot("BlueLizard->DragonPassBush", new Vector3(7745f, 4065f, 47.71f),
                new Vector3(7927.65f, 4239.77f, 47.71f), new Vector3(8146.10f, 4414.81f, 47.71f),
                new Vector3(7645f, 4015f, 47.71f)));
            WardSpots.Add(new WardSpot("PurpleMid->RoundBush", new Vector3(9057f, 8245f, 45.73f),
                new Vector3(9230.77f, 7897.22f, 66.39f), new Vector3(9446.10f, 7814.81f, 54.66f),
                new Vector3(8895f, 8313f, 54.89f)));
            WardSpots.Add(new WardSpot("PurpleMid->RiverRoundBush", new Vector3(9025.78f, 6591.64f, 46.27f),
                new Vector3(9200.08f, 6425.05f, 43.21f), new Vector3(9396.10f, 6264.81f, 23.72f),
                new Vector3(8795f, 6815f, 56.11f)));
            WardSpots.Add(new WardSpot("PurpleMid->RiverLaneBush", new Vector3(8530.27f, 6637.38f, 46.98f),
                new Vector3(8539.27f, 6637.38f, 46.98f), new Vector3(8396.10f, 6464.81f, 46.98f),
                new Vector3(8779.17f, 6804.70f, 46.98f)));
            WardSpots.Add(new WardSpot("PurpleBot->SoloBush", new Vector3(11889f, 4205f, 42.84f),
                new Vector3(11974.23f, 3807.21f, 42.84f), new Vector3(11646.10f, 3464.81f, 42.84f),
                new Vector3(11939f, 4255f, 42.84f)));
            WardSpots.Add(new WardSpot("PurpleLizard->NashorPassBush", new Vector3(6299f, 10377.75f, 45.47f),
                new Vector3(6030.24f, 10292.37f, 54.29f), new Vector3(5846.10f, 10164.81f, 53.94f),
                new Vector3(6447f, 10463f, 54.63f)));

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnGameUpdate += Game_OnGameUpdate;
            Game.OnWndProc += Game_OnWndProc;
            Game.OnGameSendPacket += Game_OnGameSendPacket;
        }

        ~WardCorrector()
        {
            Game.OnWndProc -= Game_OnWndProc;
            Game.OnGameUpdate -= Game_OnGameUpdate;
            Game.OnGameSendPacket -= Game_OnGameSendPacket;
            Drawing.OnDraw -= Drawing_OnDraw;
        }

        public bool IsActive()
        {
            return Menu.Wards.GetActive() && Menu.WardCorrector.GetActive();
        }


        private void Game_OnGameSendPacket(GamePacketEventArgs args) //TODO: Add Packetsupp for wards
        {
            if (!IsActive())
                return;

            var gPacket = new GamePacket(args.PacketData);
            var reader = new BinaryReader(new MemoryStream(args.PacketData));

            byte packetId = reader.ReadByte(); //PacketId
            if (packetId == 0x9A) //OLD 0x9A
            {
                int mNetworkId = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                byte spellId = reader.ReadByte();
                byte unknown = reader.ReadByte();
                float fromX = BitConverter.ToSingle(reader.ReadBytes(4), 0);
                float fromY = BitConverter.ToSingle(reader.ReadBytes(4), 0);
                float toX = BitConverter.ToSingle(reader.ReadBytes(4), 0);
                float toY = BitConverter.ToSingle(reader.ReadBytes(4), 0);
                int tNetworkId = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                PacketSpellId nSpellId = PacketSpellId.ConvertPacketCastToId(spellId);
                if (/*_latestSpellSlot == nSpellId.SSpellSlot && */_latestSpellSlot != SpellSlot.Unknown)
                {
                    _drawSpots = false;
                    foreach (WardSpot wardSpot in WardSpots)
                    {
                        if (!wardSpot.SafeWard &&
                            Vector3.Distance(wardSpot.Pos,
                                new Vector3(fromX, fromY, ObjectManager.Player.ServerPosition.Z)) <= 250 &&
                            !_wardAlreadyCorrected)
                        {
                            args.Process = false;
                            _wardAlreadyCorrected = true;
                            //SendPacket
                            var sCastPacket = new byte[28];
                            var writer = new BinaryWriter(new MemoryStream(sCastPacket));
                            writer.Write((byte) 0x9A);
                            writer.Write(mNetworkId);
                            writer.Write(spellId);
                            writer.Write(unknown);
                            writer.Write(wardSpot.Pos.X);
                            writer.Write(wardSpot.Pos.Y);
                            writer.Write(wardSpot.Pos.X);
                            writer.Write(wardSpot.Pos.Y);
                            writer.Write(tNetworkId);
                            Game.SendPacket(sCastPacket, PacketChannel.C2S, PacketProtocolFlags.Reliable);
                            //TODO: Check if its correct
                            _wardAlreadyCorrected = false;
                            return;
                        }
                        if (wardSpot.SafeWard &&
                            Vector3.Distance(wardSpot.MagneticPos,
                                new Vector3(fromX, fromY, ObjectManager.Player.ServerPosition.Z)) <=
                            250 &&
                            !_wardAlreadyCorrected)
                        {
                            args.Process = false;
                            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo,
                                new Vector3(wardSpot.MovePos.X, wardSpot.MovePos.Y, wardSpot.MovePos.Z));
                            _latestWardSpot = wardSpot;
                            return;
                        }
                    }
                }
            }
        }

        private void Game_OnWndProc(WndEventArgs args)
        {
            //http://msdn.microsoft.com/en-us/library/windows/desktop/ms632585(v=vs.85).aspx
            const int WM_KEYDOWN = 0x100;
            const int WM_LBUTTONUP = 0x202;
            const int WM_LBUTTONDOWN = 0x201;
            const int WM_RBUTTONUP = 0x205;
            const int WM_RBUTTONDOWN = 0x204;

            if (!IsActive())
                return;
            if (MenuGUI.IsChatOpen)
                return;
            uint trinketKey =
                Menu.WardCorrector.GetMenuItem("SAwarenessWardCorrectorKey").GetValue<KeyBind>().Key;
            if (args.Msg == WM_KEYDOWN)
            {
                InventorySlot inventoryItem = null;
                int inventoryItemId = -1;
                if (trinketKey == args.WParam)
                {
                    _latestSpellSlot = SpellSlot.Trinket;
                    inventoryItemId = 6;
                }
                else
                {
                    _latestSpellSlot = SpellSlot.Unknown;
                }
                if(_latestSpellSlot == SpellSlot.Unknown)
                {
                    switch (args.WParam)
                    {
                        case '1':
                            _latestSpellSlot = SpellSlot.Item1;
                            inventoryItemId = 0;
                            break;

                        case '2':
                            _latestSpellSlot = SpellSlot.Item2;
                            inventoryItemId = 1;
                            break;

                        case '3':
                            _latestSpellSlot = SpellSlot.Item3;
                            inventoryItemId = 2;
                            break;

                        //case trinketKey:
                        //    _latestSpellSlot = SpellSlot.Trinket;
                        //    inventoryItemId = 6;
                        //    break;

                        case '5':
                            _latestSpellSlot = SpellSlot.Item5;
                            inventoryItemId = 3;
                            break;

                        case '6':
                            _latestSpellSlot = SpellSlot.Item6;
                            inventoryItemId = 4;
                            break;

                        case '7':
                            _latestSpellSlot = SpellSlot.Item4;
                            inventoryItemId = 5;
                            break;

                        default:
                            _drawSpots = false;
                            _latestSpellSlot = SpellSlot.Unknown;
                            break;
                    }
                }

                foreach (InventorySlot inventorySlot in ObjectManager.Player.InventoryItems)
                {
                    if (inventorySlot.Slot == inventoryItemId)
                    {
                        inventoryItem = inventorySlot;
                        break;
                    }
                }

                if (_latestSpellSlot != SpellSlot.Unknown)
                {
                    if (inventoryItem != null)
                    {
                        foreach (Wards.WardItem wardItem in Wards.WardItems)
                        {
                            if ((int) inventoryItem.Id == wardItem.Id &&
                                wardItem.Type != Wards.WardType.Temp &&
                                wardItem.Type != Wards.WardType.TempVision &&
                                ObjectManager.Player.Spellbook.CanUseSpell(_latestSpellSlot) == SpellState.Ready ||
                                ObjectManager.Player.Spellbook.CanUseSpell(_latestSpellSlot) == (SpellState)1)
                            {
                                _drawSpots = true;
                            }
                        }
                        if (_drawSpots == false)
                        {
                            _latestSpellSlot = SpellSlot.Unknown;
                        }
                    }
                }
            }
            else if (args.Msg == WM_LBUTTONUP && _drawSpots)
            {
                _drawSpots = false;
            }
            else if (args.Msg == WM_RBUTTONDOWN && _drawSpots)
            {
                _drawSpots = false;
            }
            else if (args.Msg == WM_RBUTTONDOWN)
            {
                _latestWardSpot = null;
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (!IsActive())
                return;
            if (_latestWardSpot != null && _latestSpellSlot != SpellSlot.Unknown)
            {
                if (Vector3.Distance(ObjectManager.Player.ServerPosition, _latestWardSpot.ClickPos) <= 650)
                {
                    ObjectManager.Player.Spellbook.CastSpell(_latestSpellSlot, _latestWardSpot.ClickPos);
                    _latestWardSpot = null;
                }
            }
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (!IsActive())
                return;
            if (!_drawSpots)
                return;
            foreach (WardSpot ward in WardSpots)
            {
                if (Common.IsOnScreen(ward.Pos))
                    Utility.DrawCircle(ward.Pos, 50, Color.GreenYellow);
                if (ward.SafeWard)
                {
                    if (Common.IsOnScreen(ward.MagneticPos))
                    {
                        Utility.DrawCircle(ward.MagneticPos, 30, Color.Red);
                        DrawArrow(ward.MagneticPos, ward.Pos, Color.RoyalBlue);
                    }
                }
            }
        }

        private void DrawArrow(Vector3 start, Vector3 end, Color color) //TODO. Check if its correct calculated
        {
            Vector2 mPos1 = Drawing.WorldToScreen(start);
            Vector2 mPos2 = Drawing.WorldToScreen(end);
            Drawing.DrawLine(mPos1[0], mPos1[1], mPos2[0], mPos2[1], 1.0f, color);
            //Vector2 mmPos2 = new Vector2(mPos2[0], mPos2[1]);
            //Vector2 end1 = Geometry.Rotated(mmPos2, Geometry.DegreeToRadian(45));
            //Vector2 end2 = Geometry.Rotated(mmPos2, Geometry.DegreeToRadian(315));
            //end1.Normalize();
            //end2.Normalize();
            //end1 = Vector2.Multiply(mmPos2, end1);
            //Drawing.DrawLine(mPos2[0], mPos2[1], mmPos2.X * end1.X, mmPos2.Y * end1.Y, 1.0f, color);
            //Drawing.DrawLine(mPos2[0], mPos2[1], mmPos2.X * end2.X, mmPos2.Y * end2.Y, 1.0f, color);
            //float rad1 = Geometry.DegreeToRadian(45);
            //float cos1 = (float)Math.Cos(rad1);
            //float sin1 = (float)Math.Sin(rad1);
            //float rad2 = Geometry.DegreeToRadian(315);
            //float cos2 = (float)Math.Cos(rad2);
            //float sin2 = (float)Math.Sin(rad2);
            //Drawing.DrawLine(mPos2[0], mPos2[1], mPos1[0] * cos1 - mPos1[1] * sin1, mPos1[0] * sin1 + mPos1[1] * cos1, 1.0f, color);
            //Drawing.DrawLine(mPos2[0], mPos2[1], mPos2[0] * cos2 - mPos2[1] * sin2, mPos2[0] * sin2 + mPos2[1] * cos2, 1.0f, color);
            //double r1 = Math.Sqrt(mPos1[0] * mPos1[0] + mPos1[1] + mPos1[1]);
            //double r2 = Math.Sqrt(mPos2[0] * mPos2[0] + mPos2[1] + mPos2[1]);
            //Vector2 mPos2P = new Vector2((float)(r2 * Math.Cos(45)), (float)(r2 * Math.Sin(45)));
            //Vector2 mPos2N = new Vector2((float)(r2 * Math.Cos(-45)), (float)(r2 * Math.Sin(-45)));
            //Drawing.DrawLine(mPos2[0], mPos2[1], mPos2P.X, mPos2P.Y, 1.0f, color);
            //Drawing.DrawLine(mPos2[0], mPos2[1], mPos2N.X, mPos2N.Y, 1.0f, color);
        }

        private class PacketSpellId
        {
            public readonly SpellSlot SSpellSlot;
            public bool IsSummoner;

            public PacketSpellId(SpellSlot spellSlot, bool isSummoner)
            {
                SSpellSlot = spellSlot;
                IsSummoner = isSummoner;
            }

            public static PacketSpellId ConvertPacketCastToId(int id)
            {
                switch (id)
                {
                    case 128:
                        return new PacketSpellId(SpellSlot.Q, false);

                    case 129:
                        return new PacketSpellId(SpellSlot.W, false);

                    case 130:
                        return new PacketSpellId(SpellSlot.E, false);

                    case 131:
                        return new PacketSpellId(SpellSlot.R, false);

                    case 132:
                        return new PacketSpellId(SpellSlot.Item1, false);

                    case 133:
                        return new PacketSpellId(SpellSlot.Item2, false);

                    case 134:
                        return new PacketSpellId(SpellSlot.Item3, false);

                    case 145:
                        return new PacketSpellId(SpellSlot.Item4, false);

                    case 136:
                        return new PacketSpellId(SpellSlot.Item5, false);

                    case 137:
                        return new PacketSpellId(SpellSlot.Item6, false);

                    case 138:
                        return new PacketSpellId(SpellSlot.Trinket, false);

                    case 64:
                        return new PacketSpellId(SpellSlot.Q, true);

                    case 65:
                        return new PacketSpellId(SpellSlot.W, true);

                    case 192:
                        return new PacketSpellId(SpellSlot.Q, true);

                    case 193:
                        return new PacketSpellId(SpellSlot.W, true);

                    case 10:
                        return new PacketSpellId(SpellSlot.Recall, false);
                }
                return new PacketSpellId(SpellSlot.Unknown, false);
            }
        }

        private class WardSpot
        {
            public readonly Vector3 ClickPos;
            public readonly Vector3 MagneticPos;
            public readonly bool SafeWard;
            public Vector3 MovePos;
            public String Name;
            public Vector3 Pos;

            public WardSpot(string name, Vector3 pos)
            {
                Name = name;
                Pos = pos;
                MagneticPos = new Vector3();
                ClickPos = new Vector3();
                MovePos = new Vector3();
                SafeWard = false;
            }

            public WardSpot(string name, Vector3 magneticPos, Vector3 clickPos, Vector3 pos, Vector3 movePos)
            {
                Name = name;
                Pos = pos;
                MagneticPos = magneticPos;
                ClickPos = clickPos;
                MovePos = movePos;
                SafeWard = true;
            }
        }
    }
}