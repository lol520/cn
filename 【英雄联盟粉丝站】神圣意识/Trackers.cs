using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using LeagueSharp;
using LeagueSharp.Common;
using SAwareness.Spectator;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using Font = SharpDX.Direct3D9.Font;
using Packet = LeagueSharp.Common.Packet;

namespace SAwareness
{
    internal class CloneTracker
    {
        public CloneTracker()
        {
            Drawing.OnDraw += Drawing_OnDraw;
        }

        ~CloneTracker()
        {
            Drawing.OnDraw -= Drawing_OnDraw;
        }

        public bool IsActive()
        {
            return Menu.Tracker.GetActive() && Menu.CloneTracker.GetActive();
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (!IsActive())
                return;
            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (hero.IsEnemy && !hero.IsDead && hero.IsVisible)
                {
                    if (hero.ChampionName.Contains("Shaco") ||
                        hero.ChampionName.Contains("Leblanc") ||
                        hero.ChampionName.Contains("MonkeyKing") ||
                        hero.ChampionName.Contains("Yorick"))
                    {
                        Utility.DrawCircle(hero.ServerPosition, 100, Color.Red);
                        Utility.DrawCircle(hero.ServerPosition, 110, Color.Red);
                    }
                    
                }
            }
        }
    }

    internal class HiddenObject
    {
        public enum ObjectType
        {
            Vision,
            Sight,
            Trap,
            Unknown
        }

        private const int WardRange = 1200;
        private const int TrapRange = 300;
        public List<ObjectData> HidObjects = new List<ObjectData>();
        public List<Object> Objects = new List<Object>();

        public HiddenObject()
        {
            Objects.Add(new Object(ObjectType.Vision, "Vision Ward", "VisionWard", "VisionWard", float.MaxValue, 8,
                6424612, Color.BlueViolet));
            Objects.Add(new Object(ObjectType.Sight, "Stealth Ward", "SightWard", "SightWard", 180.0f, 161, 234594676,
                Color.Green));
            Objects.Add(new Object(ObjectType.Sight, "Warding Totem (Trinket)", "YellowTrinket", "TrinketTotemLvl1", 60.0f,
                56, 263796881, Color.Green));
            Objects.Add(new Object(ObjectType.Sight, "Warding Totem (Trinket)", "YellowTrinketUpgrade", "TrinketTotemLvl2", 120.0f,
                56, 263796882, Color.Green));
            Objects.Add(new Object(ObjectType.Sight, "Greater Stealth Totem (Trinket)", "SightWard", "TrinketTotemLvl3",
                180.0f, 56, 263796882, Color.Green));
            Objects.Add(new Object(ObjectType.Sight, "Greater Vision Totem (Trinket)", "VisionWard", "TrinketTotemLvl3B",
                9999.9f, 137, 194218338, Color.BlueViolet));
            Objects.Add(new Object(ObjectType.Sight, "Wriggle's Lantern", "SightWard", "wrigglelantern", 180.0f, 73,
                177752558, Color.Green));
            Objects.Add(new Object(ObjectType.Sight, "Quill Coat", "SightWard", "", 180.0f, 73, 135609454, Color.Green));
            Objects.Add(new Object(ObjectType.Sight, "Ghost Ward", "SightWard", "ItemGhostWard", 180.0f, 229, 101180708,
                Color.Green));

            Objects.Add(new Object(ObjectType.Trap, "Yordle Snap Trap", "Cupcake Trap", "CaitlynYordleTrap", 240.0f, 62,
                176176816, Color.Red));
            Objects.Add(new Object(ObjectType.Trap, "Jack In The Box", "Jack In The Box", "JackInTheBox", 60.0f, 2,
                44637032, Color.Red));
            Objects.Add(new Object(ObjectType.Trap, "Bushwhack", "Noxious Trap", "Bushwhack", 240.0f, 9, 167611995,
                Color.Red));
            Objects.Add(new Object(ObjectType.Trap, "Noxious Trap", "Noxious Trap", "BantamTrap", 600.0f, 48, 176304336,
                Color.Red));

            Game.OnGameProcessPacket += Game_OnGameProcessPacket;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            GameObject.OnDelete += Obj_AI_Base_OnDelete;
            Drawing.OnDraw += Drawing_OnDraw;
            GameObject.OnCreate += GameObject_OnCreate;
            Game.OnGameUpdate += Game_OnGameUpdate;
            foreach (var obj in ObjectManager.Get<GameObject>())
            {
                GameObject_OnCreate(obj, new EventArgs());
            }
        }

        void Game_OnGameUpdate(EventArgs args)
        {
            if (!IsActive())
                return;
            List<ObjectData> objects = HidObjects.FindAll(x => x.ObjectBase.Name == "Unknown");
            foreach (var obj1 in HidObjects.ToArray())
            {
                if(obj1.ObjectBase.Name.Contains("Unknown"))
                    continue;
                foreach (var obj2 in objects)
                {
                    if (Geometry.ProjectOn(obj1.EndPosition.To2D(), obj2.StartPosition.To2D(), obj2.EndPosition.To2D()).IsOnSegment)
                    {
                        HidObjects.Remove(obj2);
                    }
                }
            }
            
        }

        ~HiddenObject()
        {
            Game.OnGameProcessPacket -= Game_OnGameProcessPacket;
            Obj_AI_Base.OnProcessSpellCast -= Obj_AI_Base_OnProcessSpellCast;
            GameObject.OnCreate -= GameObject_OnCreate;
            GameObject.OnDelete -= Obj_AI_Base_OnDelete;
            Drawing.OnDraw -= Drawing_OnDraw;
        }

        public bool IsActive()
        {
            return Menu.Tracker.GetActive() && Menu.VisionDetector.GetActive();
        }

        private void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (!IsActive())
                return;
            try
            {
                if(!sender.IsValid)
                    return;
                if (sender is Obj_AI_Base && ObjectManager.Player.Team != sender.Team)
                {   
                    foreach (Object obj in Objects)
                    {
                        if (((Obj_AI_Base)sender).BaseSkinName == obj.ObjectName && !ObjectExist(sender.Position))
                        {
                            HidObjects.Add(new ObjectData(obj, sender.Position, Game.Time + ((Obj_AI_Base)sender).Mana, sender.Name,
                                null, sender.NetworkId));
                            break;
                        }
                    }
                }
                
                if (sender is Obj_SpellLineMissile && ObjectManager.Player.Team != ((Obj_SpellMissile)sender).SpellCaster.Team)
                {
                    if (((Obj_SpellMissile)sender).SData.Name.Contains("itemplacementmissile"))
                    {
                        Utility.DelayAction.Add(10, () =>
                        {
                            if (!ObjectExist(((Obj_SpellMissile)sender).EndPosition))
                            {

                                HidObjects.Add(new ObjectData(new Object(ObjectType.Unknown, "Unknown", "Unknown", "Unknown", 180.0f, 0, 0, Color.Yellow), ((Obj_SpellMissile)sender).EndPosition, Game.Time + 180.0f, sender.Name, null,
                                    sender.NetworkId, ((Obj_SpellMissile)sender).StartPosition));
                            }
                        });
                        
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HiddenObjectCreate: " + ex);
            }
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (!IsActive())
                return;
            try
            {
                for (int i = 0; i < HidObjects.Count; i++)
                {
                    ObjectData obj = HidObjects[i];
                    if (Game.Time > obj.EndTime)
                    {
                        HidObjects.RemoveAt(i);
                        break;
                    }
                    Vector2 objMPos = Drawing.WorldToMinimap(obj.EndPosition);
                    Vector2 objPos = Drawing.WorldToScreen(obj.EndPosition);
                    var posList = new List<Vector3>();
                    switch (obj.ObjectBase.Type)
                    {
                        case ObjectType.Sight:
                            if (Menu.VisionDetector.GetMenuItem("SAwarenessVisionDetectorDrawRange").GetValue<bool>())
                            {
                                Utility.DrawCircle(obj.EndPosition, WardRange, obj.ObjectBase.Color);
                            }
                            posList = GetVision(obj.EndPosition, WardRange);
                            for (int j = 0; j < posList.Count; j++)
                            {
                                Vector2 visionPos1 = Drawing.WorldToScreen(posList[i]);
                                Vector2 visionPos2 = Drawing.WorldToScreen(posList[i]);
                                Drawing.DrawLine(visionPos1[0], visionPos1[1], visionPos2[0], visionPos2[1], 1.0f,
                                    obj.ObjectBase.Color);
                            }
                            Drawing.DrawText(objMPos[0], objMPos[1], obj.ObjectBase.Color, "S");
                            break;

                        case ObjectType.Trap:
                            if (Menu.VisionDetector.GetMenuItem("SAwarenessVisionDetectorDrawRange").GetValue<bool>())
                            {
                                Utility.DrawCircle(obj.EndPosition, TrapRange, obj.ObjectBase.Color);
                            }
                            posList = GetVision(obj.EndPosition, TrapRange);
                            for (int j = 0; j < posList.Count; j++)
                            {
                                Vector2 visionPos1 = Drawing.WorldToScreen(posList[i]);
                                Vector2 visionPos2 = Drawing.WorldToScreen(posList[i]);
                                Drawing.DrawLine(visionPos1[0], visionPos1[1], visionPos2[0], visionPos2[1], 1.0f,
                                    obj.ObjectBase.Color);
                            }
                            Drawing.DrawText(objMPos[0], objMPos[1], obj.ObjectBase.Color, "T");
                            break;

                        case ObjectType.Vision:
                            if (Menu.VisionDetector.GetMenuItem("SAwarenessVisionDetectorDrawRange").GetValue<bool>())
                            {
                                Utility.DrawCircle(obj.EndPosition, WardRange, obj.ObjectBase.Color);
                            }
                            posList = GetVision(obj.EndPosition, WardRange);
                            for (int j = 0; j < posList.Count; j++)
                            {
                                Vector2 visionPos1 = Drawing.WorldToScreen(posList[i]);
                                Vector2 visionPos2 = Drawing.WorldToScreen(posList[i]);
                                Drawing.DrawLine(visionPos1[0], visionPos1[1], visionPos2[0], visionPos2[1], 1.0f,
                                    obj.ObjectBase.Color);
                            }
                            Drawing.DrawText(objMPos[0], objMPos[1], obj.ObjectBase.Color, "V");
                            break;

                        case ObjectType.Unknown:
                            Drawing.DrawLine(Drawing.WorldToScreen(obj.StartPosition), Drawing.WorldToScreen(obj.EndPosition), 1, obj.ObjectBase.Color);
                            break;
                    }
                    Utility.DrawCircle(obj.EndPosition, 50, obj.ObjectBase.Color);
                    float endTime = obj.EndTime - Game.Time;
                    if (!float.IsInfinity(endTime) && !float.IsNaN(endTime) && endTime.CompareTo(float.MaxValue) != 0)
                    {
                        var m = (float) Math.Floor(endTime/60);
                        var s = (float) Math.Ceiling(endTime%60);
                        String ms = (s < 10 ? m + ":0" + s : m + ":" + s);
                        Drawing.DrawText(objPos[0], objPos[1], obj.ObjectBase.Color, ms);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HiddenObjectDraw: " + ex);
            }
        }

        private List<Vector3> GetVision(Vector3 viewPos, float range) //TODO: ADD IT
        {
            var list = new List<Vector3>();
            //double qual = 2*Math.PI/25;
            //for (double i = 0; i < 2*Math.PI + qual;)
            //{
            //    Vector3 pos = new Vector3(viewPos.X + range * (float)Math.Cos(i), viewPos.Y - range * (float)Math.Sin(i), viewPos.Z);
            //    for (int j = 1; j < range; j = j + 25)
            //    {
            //        Vector3 nPos = new Vector3(viewPos.X + j * (float)Math.Cos(i), viewPos.Y - j * (float)Math.Sin(i), viewPos.Z);
            //        if (NavMesh.GetCollisionFlags(nPos).HasFlag(CollisionFlags.Wall))
            //        {
            //            pos = nPos;
            //            break;
            //        }
            //    }
            //    list.Add(pos);
            //    i = i + 0.1;
            //}
            return list;
        }

        private Object HiddenObjectById(int id)
        {
            return Objects.FirstOrDefault(vision => id == vision.Id2);
        }

        private bool ObjectExist(Vector3 pos)
        {
            return HidObjects.Any(obj => pos.Distance(obj.EndPosition) < 30);
        }

        private void Game_OnGameProcessPacket(GamePacketEventArgs args)
        {
            if (!IsActive())
                return;
            try
            {
                var reader = new BinaryReader(new MemoryStream(args.PacketData));
                byte packetId = reader.ReadByte(); //PacketId
                if (packetId == 181) //OLD 180
                {
                    int networkId = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                    var creator = ObjectManager.GetUnitByNetworkId<Obj_AI_Base>(networkId);
                    if (creator != null && creator.Team != ObjectManager.Player.Team)
                    {
                        reader.ReadBytes(7);
                        int id = reader.ReadInt32();
                        reader.ReadBytes(21);
                        networkId = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                        reader.ReadBytes(12);
                        float x = BitConverter.ToSingle(reader.ReadBytes(4), 0);
                        float y = BitConverter.ToSingle(reader.ReadBytes(4), 0);
                        float z = BitConverter.ToSingle(reader.ReadBytes(4), 0);
                        var pos = new Vector3(x, y, z);
                        Object obj = HiddenObjectById(id);
                        if (obj != null && !ObjectExist(pos))
                        {
                            if (obj.Type == ObjectType.Trap)
                                pos = new Vector3(x, z, y);
                            networkId = networkId + 2;
                            Utility.DelayAction.Add(1, () =>
                            {
                                for (int i = 0; i < HidObjects.Count; i++)
                                {
                                    ObjectData objectData = HidObjects[i];
                                    if (objectData != null && objectData.NetworkId == networkId)
                                    {
                                        var objNew = ObjectManager.GetUnitByNetworkId<Obj_AI_Base>(networkId);
                                        if (objNew != null && objNew.IsValid)
                                            objectData.EndPosition = objNew.Position;
                                    }
                                }
                            });
                            HidObjects.Add(new ObjectData(obj, pos, Game.Time + obj.Duration, creator.Name, null,
                                networkId));
                        }
                    }
                }
                else if (packetId == 178)
                {
                    int networkId = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                    var gObject = ObjectManager.GetUnitByNetworkId<GameObject>(networkId);
                    if (gObject != null)
                    {
                        for (int i = 0; i < HidObjects.Count; i++)
                        {
                            ObjectData objectData = HidObjects[i];
                            if (objectData != null && objectData.NetworkId == networkId)
                            {
                                objectData.EndPosition = gObject.Position;
                            }
                        }
                    }
                }
                else if (packetId == 50) //OLD 49
                {
                    int networkId = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                    for (int i = 0; i < HidObjects.Count; i++)
                    {
                        ObjectData objectData = HidObjects[i];
                        var gObject = ObjectManager.GetUnitByNetworkId<GameObject>(networkId);
                        if (objectData != null && objectData.NetworkId == networkId)
                        {
                            HidObjects.RemoveAt(i);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HiddenObjectProcess: " + ex);
            }
        }

        private void Obj_AI_Base_OnDelete(GameObject sender, EventArgs args)
        {
            if (!IsActive())
                return;
            try
            {
                if (!sender.IsValid)
                    return;
                for (int i = 0; i < HidObjects.Count; i++)
                {
                    ObjectData obj = HidObjects[i];
                    if ((obj.ObjectBase != null && sender.Name == obj.ObjectBase.ObjectName) ||
                        sender.Name.Contains("Ward") && sender.Name.Contains("Death"))
                        if (sender.Position.Distance(obj.EndPosition) < 30 || sender.Position.Distance(obj.StartPosition) < 30)
                        {
                            HidObjects.RemoveAt(i);
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HiddenObjectDelete: " + ex);
            }
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!IsActive())
                return;
            try
            {
                if (!sender.IsValid)
                    return;
                if (ObjectManager.Player.Team != sender.Team)
                {
                    foreach (Object obj in Objects)
                    {
                        if (args.SData.Name == obj.SpellName && !ObjectExist(args.End))
                        {
                            HidObjects.Add(new ObjectData(obj, args.End, Game.Time + obj.Duration, sender.Name, null,
                                sender.NetworkId));
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HiddenObjectSpell: " + ex);
            }
        }

        public class Object
        {
            public Color Color;
            public float Duration;
            public int Id;
            public int Id2;
            public String Name;
            public String ObjectName;
            public String SpellName;
            public ObjectType Type;

            public Object(ObjectType type, String name, String objectName, String spellName, float duration, int id,
                int id2, Color color)
            {
                Type = type;
                Name = name;
                ObjectName = objectName;
                SpellName = spellName;
                Duration = duration;
                Id = id;
                Id2 = id2;
                Color = color;
            }
        }

        public class ObjectData
        {
            public String Creator;
            public float EndTime;
            public int NetworkId;
            public Object ObjectBase;
            public List<Vector2> Points;
            public Vector3 EndPosition;
            public Vector3 StartPosition;

            public ObjectData(Object objectBase, Vector3 endPosition, float endTime, String creator, List<Vector2> points,
                int networkId, Vector3 startPosition = new Vector3())
            {
                ObjectBase = objectBase;
                EndPosition = endPosition;
                EndTime = endTime;
                Creator = creator;
                Points = points;
                NetworkId = networkId;
                StartPosition = startPosition;
            }
        }
    }

    internal class DestinationTracker
    {
        private static readonly Dictionary<Obj_AI_Hero, List<Ability>> Enemies =
            new Dictionary<Obj_AI_Hero, List<Ability>>();

        public DestinationTracker()
        {
            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (hero.IsEnemy)
                {
                    var abilities = new List<Ability>();
                    foreach (SpellDataInst spell in hero.SummonerSpellbook.Spells)
                    {
                        if (spell.Name.Contains("Flash"))
                        {
                            abilities.Add(new Ability("SummonerFlash", 400, 0, hero));
                            //AddObject(hero, abilities);
                        }
                    }

                    //abilities.Clear(); //TODO: Check if it delets the flash abilities

                    switch (hero.ChampionName)
                    {
                        case "Ezreal":
                            abilities.Add(new Ability("EzrealArcaneShift", 475, 0, hero));
                            //AddObject(hero, abilities);
                            break;

                        case "Fiora":
                            abilities.Add(new Ability("FioraDance", 700, 1, hero));
                            //AddObject(hero, abilities);
                            break;

                        case "Kassadin":
                            abilities.Add(new Ability("RiftWalk", 700, 0, hero));
                            //AddObject(hero, abilities);
                            break;

                        case "Katarina":
                            abilities.Add(new Ability("KatarinaE", 700, 0, hero));
                            //AddObject(hero, abilities);
                            break;

                        case "Leblanc":
                            abilities.Add(new Ability("LeblancSlide", 600, 0.5f, hero));
                            abilities.Add(new Ability("leblancslidereturn", 0, 0, hero));
                            abilities.Add(new Ability("LeblancSlideM", 600, 0.5f, hero));
                            abilities.Add(new Ability("leblancslidereturnm", 0, 0, hero));
                            //AddObject(hero, abilities);
                            break;

                        case "Lissandra":
                            abilities.Add(new Ability("LissandraE", 700, 0, hero));
                            //AddObject(hero, abilities);
                            break;

                        case "MasterYi":
                            abilities.Add(new Ability("AlphaStrike", 600, 0.9f, hero));
                            //AddObject(hero, abilities);
                            break;

                        case "Shaco":
                            abilities.Add(new Ability("Deceive", 400, 0, hero));
                            //AddObject(hero, abilities);
                            break;

                        case "Talon":
                            abilities.Add(new Ability("TalonCutthroat", 700, 0, hero));
                            //AddObject(hero, abilities);
                            break;

                        case "Vayne":
                            abilities.Add(new Ability("VayneTumble", 250, 0, hero));
                            //AddObject(hero, abilities);
                            break;

                        case "Zed":
                            abilities.Add(new Ability("ZedShadowDash", 999, 0, hero));
                            //AddObject(hero, abilities);
                            break;
                    }
                    if (abilities.Count > 0)
                        AddObject(hero, abilities);
                }
            }
            Game.OnGameUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Hero_OnProcessSpellCast;
            GameObject.OnCreate += Obj_AI_Base_OnCreate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private bool AddObject(Obj_AI_Hero hero, List<Ability> abilities)
        {
            if (Enemies.ContainsKey(hero))
                return false;
            Enemies.Add(hero, abilities);
            return true;
            //TODO:Add
        }

        ~DestinationTracker()
        {
            Game.OnGameUpdate -= Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast -= Obj_AI_Hero_OnProcessSpellCast;
            GameObject.OnCreate -= Obj_AI_Base_OnCreate;
            Drawing.OnDraw -= Drawing_OnDraw;
        }

        public bool IsActive()
        {
            return Menu.Tracker.GetActive() && Menu.DestinationTracker.GetActive();
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (!IsActive())
                return;
            foreach (var enemy in Enemies)
            {
                foreach (Ability ability in enemy.Value)
                {
                    if (ability.Casted)
                    {
                        Vector2 startPos = Drawing.WorldToScreen(ability.StartPos);
                        Vector2 endPos = Drawing.WorldToScreen(ability.EndPos);

                        if (ability.OutOfBush)
                        {
                            Utility.DrawCircle(ability.EndPos, ability.Range, Color.Red);
                        }
                        else
                        {
                            Utility.DrawCircle(ability.EndPos, ability.Range, Color.Red);
                            Drawing.DrawLine(startPos[0], startPos[1], endPos[0], endPos[1], 1.0f, Color.Red);
                        }
                        Drawing.DrawText(endPos[0], endPos[1], Color.Bisque,
                            enemy.Key.ChampionName + " " + ability.SpellName);
                    }
                }
            }
        }

        private void Obj_AI_Base_OnCreate(GameObject sender, EventArgs args)
        {
            if (!IsActive())
                return;
            foreach (var enemy in Enemies)
            {
                if (enemy.Key.ChampionName == "Shaco")
                {
                    if (sender.Type != GameObjectType.obj_LampBulb && sender.Name == "JackintheboxPoof2.troy" && !enemy.Value[0].Casted)
                    {
                        enemy.Value[0].StartPos = sender.Position;
                        enemy.Value[0].EndPos = sender.Position;
                        enemy.Value[0].Casted = true;
                        enemy.Value[0].TimeCasted = (int) Game.Time;
                        enemy.Value[0].OutOfBush = true;
                    }
                }
            }
        }

        private Vector3 CalculateEndPos(Ability ability, GameObjectProcessSpellCastEventArgs args)
        {
            float dist = Vector3.Distance(args.Start, args.End);
            if (dist <= ability.Range)
            {
                ability.EndPos = args.End;
            }
            else
            {
                Vector3 norm = args.Start - args.End;
                norm.Normalize();
                Vector3 endPos = args.Start - norm*ability.Range;

                //endPos = FindNearestNonWall(); TODO: Add FindNearestNonWall

                ability.EndPos = endPos;
            }
            return ability.EndPos;
        }

        private void Obj_AI_Hero_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!IsActive())
                return;
            if (sender.GetType() == typeof (Obj_AI_Hero))
            {
                var hero = (Obj_AI_Hero) sender;
                if (hero.IsEnemy)
                {
                    Obj_AI_Hero enemy = hero;
                    foreach (var abilities in Enemies)
                    {
                        if (abilities.Key.NetworkId != enemy.NetworkId)
                            continue;
                        int index = 0;
                        foreach (Ability ability in abilities.Value)
                        {
                            if (args.SData.Name == "vayneinquisition")
                            {
                                if (ability.ExtraTicks > 0)
                                {
                                    ability.ExtraTicks = (int) Game.Time + 6 + 2*args.Level;
                                    return;
                                }
                            }
                            if (args.SData.Name == ability.SpellName)
                            {
                                switch (ability.SpellName)
                                {
                                    case "VayneTumble":
                                        if (Game.Time >= ability.ExtraTicks)
                                            return;
                                        ability.StartPos = args.Start;
                                        ability.EndPos = CalculateEndPos(ability, args);
                                        break;

                                    case "Deceive":
                                        ability.OutOfBush = false;
                                        ability.StartPos = args.Start;
                                        ability.EndPos = CalculateEndPos(ability, args);
                                        break;

                                    case "LeblancSlideM":
                                        abilities.Value[index - 2].Casted = false;
                                        ability.StartPos = abilities.Value[index - 2].StartPos;
                                        ability.EndPos = CalculateEndPos(ability, args);
                                        break;

                                    case "leblancslidereturn":
                                    case "leblancslidereturnm":
                                        if (ability.SpellName == "leblancslidereturn")
                                        {
                                            abilities.Value[index - 1].Casted = false;
                                            abilities.Value[index + 1].Casted = false;
                                            abilities.Value[index + 2].Casted = false;
                                        }
                                        else
                                        {
                                            abilities.Value[index - 3].Casted = false;
                                            abilities.Value[index - 2].Casted = false;
                                            abilities.Value[index - 1].Casted = false;
                                        }
                                        ability.StartPos = args.Start;
                                        ability.EndPos = abilities.Value[index - 1].StartPos;
                                        break;

                                    case "FioraDance":
                                    case "AlphaStrike":
                                        //TODO: Get Target
                                        //ability.Target = args.Target;
                                        ability.TargetDead = false;
                                        ability.StartPos = args.Start;
                                        //ability.EndPos = args.Target.Position;
                                        break;

                                    default:
                                        ability.StartPos = args.Start;
                                        ability.EndPos = CalculateEndPos(ability, args);
                                        break;
                                }
                                ability.Casted = true;
                                ability.TimeCasted = (int) Game.Time;
                                return;
                            }
                            index++;
                        }
                    }
                }
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (!IsActive())
                return;
            foreach (var abilities in Enemies)
            {
                foreach (Ability ability in abilities.Value)
                {
                    if (ability.Casted)
                    {
                        if (ability.SpellName == "FioraDance" || ability.SpellName == "AlphaStrike" &&
                            !ability.TargetDead)
                        {
                            if (Game.Time > (ability.TimeCasted + ability.Delay + 0.2))
                                ability.Casted = false;
                                /*else if (ability.Target.IsDead()) TODO: Waiting for adding Target
                            {
                                Vector3 temp = ability.EndPos;
                                ability.EndPos = ability.StartPos;
                                ability.StartPos = temp;
                                ability.TargetDead = true;
                            }*/
                        }
                        else if (ability.Owner.IsDead ||
                                 (!ability.Owner.IsValid && Game.Time > (ability.TimeCasted + /*variable*/ 2)) ||
                                 (ability.Owner.IsVisible &&
                                  Game.Time > (ability.TimeCasted + /*variable*/ 5 + ability.Delay)))
                        {
                            ability.Casted = false;
                        }
                        else if (!ability.OutOfBush && ability.Owner.IsVisible &&
                                 Game.Time > (ability.TimeCasted + ability.Delay))
                        {
                            ability.EndPos = ability.Owner.ServerPosition;
                        }
                    }
                }
            }
        }

        public class Ability
        {
            public bool Casted;
            public float Delay;
            public Vector3 EndPos;
            public int ExtraTicks;
            public bool OutOfBush;
            public Obj_AI_Hero Owner;
            public int Range;
            public String SpellName;
            public Vector3 StartPos;
            public Obj_AI_Hero Target;
            public bool TargetDead;
            public int TimeCasted;

            public Ability(string spellName, int range, float delay, Obj_AI_Hero owner)
            {
                SpellName = spellName;
                Range = range;
                Delay = delay;
                Owner = owner;
            }
        }
    }

    internal class SsCaller
    {
        public static readonly Dictionary<Obj_AI_Hero, Time> Enemies = new Dictionary<Obj_AI_Hero, Time>();

        public SsCaller()
        {
            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (hero.IsEnemy)
                {
                    Enemies.Add(hero, new Time());
                }
            }
            Game.OnGameUpdate += Game_OnGameUpdate;
        }

        ~SsCaller()
        {
            Game.OnGameUpdate -= Game_OnGameUpdate;
        }

        public bool IsActive()
        {
            return Menu.Tracker.GetActive() && Menu.SsCaller.GetActive() &&
                   Game.Time < (Menu.SsCaller.GetMenuItem("SAwarenessSSCallerDisableTime").GetValue<Slider>().Value*60);
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (!IsActive())
                return;
            foreach (var enemy in Enemies)
            {
                UpdateTime(enemy);
                HandleSs(enemy);
            }
        }

        private void HandleSs(KeyValuePair<Obj_AI_Hero, Time> enemy)
        {
            Obj_AI_Hero hero = enemy.Key;
            if (enemy.Value.InvisibleTime > 5 && !enemy.Value.Called && Game.Time - enemy.Value.LastTimeCalled > 30)
            {
                var pos = new Vector2(hero.Position.X, hero.Position.Y);
                var pingType = Packet.PingType.Normal;
                var t = Menu.SsCaller.GetMenuItem("SAwarenessSSCallerPingType").GetValue<StringList>();
                pingType = (Packet.PingType) t.SelectedIndex + 1;
                GamePacket gPacketT;
                for (int i = 0;
                    i < Menu.SsCaller.GetMenuItem("SAwarenessSSCallerPingTimes").GetValue<Slider>().Value;
                    i++)
                {
                    if (Menu.SsCaller.GetMenuItem("SAwarenessSSCallerLocalPing").GetValue<bool>())
                    {
                        gPacketT =
                            Packet.S2C.Ping.Encoded(new Packet.S2C.Ping.Struct(pos[0], pos[1], 0, 0, pingType));
                        gPacketT.Process();
                    }
                    else if (!Menu.SsCaller.GetMenuItem("SAwarenessSSCallerLocalPing").GetValue<bool>() &&
                             Menu.GlobalSettings.GetMenuItem("SAwarenessGlobalSettingsServerChatPingActive")
                                 .GetValue<bool>())
                    {
                        gPacketT =
                            Packet.C2S.Ping.Encoded(new Packet.C2S.Ping.Struct(enemy.Value.LastPosition.X,
                                enemy.Value.LastPosition.Y, 0, pingType));
                        gPacketT.Send();
                    }
                }
                if (Menu.SsCaller.GetMenuItem("SAwarenessSSCallerChatChoice").GetValue<StringList>().SelectedIndex == 1)
                {
                    Game.PrintChat("ss {0}", hero.ChampionName);
                }
                else if (
                    Menu.SsCaller.GetMenuItem("SAwarenessSSCallerChatChoice").GetValue<StringList>().SelectedIndex ==
                    2 &&
                    Menu.GlobalSettings.GetMenuItem("SAwarenessGlobalSettingsServerChatPingActive").GetValue<bool>())
                {
                    Game.Say("ss {0}", hero.ChampionName);
                }
                enemy.Value.LastTimeCalled = (int) Game.Time;
                enemy.Value.Called = true;
            }
        }

        private void UpdateTime(KeyValuePair<Obj_AI_Hero, Time> enemy)
        {
            Obj_AI_Hero hero = enemy.Key;
            if (hero.IsVisible)
            {
                Enemies[hero].InvisibleTime = 0;
                Enemies[hero].VisibleTime = (int) Game.Time;
                enemy.Value.Called = false;
                Enemies[hero].LastPosition = hero.ServerPosition;
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
            }
        }

        public class Time
        {
            public bool Called;
            public int InvisibleTime;
            public Vector3 LastPosition;
            public int LastTimeCalled;
            public int VisibleTime;
        }
    }

    public class UiTracker
    {
        private readonly Dictionary<Obj_AI_Hero, ChampInfos> _allies = new Dictionary<Obj_AI_Hero, ChampInfos>();
        private readonly Dictionary<Obj_AI_Hero, ChampInfos> _enemies = new Dictionary<Obj_AI_Hero, ChampInfos>();
        private Font _champF;
        private Render.Rectangle _recB;
        private Font _recF;
        private Render.Rectangle _recS;
        private Render.Rectangle _recNS;
        private Sprite _s;
        private Font _spellF;
        private Font _sumF;
        private Font _champIF;
        private Texture _backBar;
        private Size _backBarSize = new Size(96, 10);
        private Size _champSize = new Size(64, 64);
        private Texture _healthBar;
        private Size _healthManaBarSize = new Size(96, 5);
        private Texture _manaBar;
        private Texture _overlayEmptyItem;
        private Texture _overlayRecall;
        private Texture _overlaySpellItem;
        private Texture _overlaySpellItemRed;
        private Texture _overlaySpellItemGreen;
        private Texture _overlaySummoner;
        private Texture _overlaySummonerSpell;
        private Texture _overlayGoldCsLvl;
        private Size _recSize = new Size(64, 12);
        private Vector2 _screen = new Vector2(Drawing.Width, Drawing.Height/2);
        private Size _spellSize = new Size(16, 16);
        private Size _sumSize = new Size(32, 32);
        private bool _drawActive = true;


        private Size _hudSize;
        private Vector2 _lastCursorPos;
        private bool _moveActive;
        private int _oldAx = 0;
        private int _oldAy = 0;
        private int _oldEx;
        private int _oldEy;
        private float _scalePc = 1.0f;
        private bool _shiftActive;

        public UiTracker()
        {
            if (!IsActive())
                return;
            bool loaded = false;
            int tries = 0;
            while (!loaded)
            {
                loaded = Init(tries >= 5);

                tries++;
                if (tries > 9)
                {
                    Console.WriteLine("Couldn't load Interface. It got disabled.");
                    Menu.UiTracker.ForceDisable = true;
                    Menu.UiTracker.Item = null;
                    return;
                }
                Thread.Sleep(10);
            }
            new System.Threading.Thread(() =>
            {
                SpecUtils.GetInfo();
            }).Start();
            Game.OnGameUpdate += Game_OnGameUpdate;
            //Game.OnGameProcessPacket += Game_OnGameProcessPacket; //TODO:Enable for Gold View currently bugged packet id never received
            Drawing.OnPreReset += Drawing_OnPreReset;
            Drawing.OnPostReset += Drawing_OnPostReset;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Game.OnWndProc += Game_OnWndProc;
            AppDomain.CurrentDomain.DomainUnload += delegate { Drawing_OnPreReset(new EventArgs()); };
            AppDomain.CurrentDomain.ProcessExit += delegate { Drawing_OnPreReset(new EventArgs()); };
        }

        private void Game_OnWndProc(WndEventArgs args)
        {
            if (!IsActive())
                return;
            HandleInput((WindowsMessages) args.Msg, Utils.GetCursorPos(), args.WParam);
        }

        private void HandleInput(WindowsMessages message, Vector2 cursorPos, uint key)
        {
            HandleUiMove(message, cursorPos, key);
            HandleChampClick(message, cursorPos, key);
        }

        private void HandleUiMove(WindowsMessages message, Vector2 cursorPos, uint key)
        {
            if (message != WindowsMessages.WM_LBUTTONDOWN && message != WindowsMessages.WM_MOUSEMOVE &&
                message != WindowsMessages.WM_LBUTTONUP || (!_moveActive && message == WindowsMessages.WM_MOUSEMOVE)
                )
            {
                return;
            }
            if (message == WindowsMessages.WM_LBUTTONDOWN)
            {
                _lastCursorPos = cursorPos;
            }
            if (message == WindowsMessages.WM_LBUTTONUP)
            {
                _lastCursorPos = new Vector2();
                _moveActive = false;
                return;
            }
            var firstEnemyHero = new KeyValuePair<Obj_AI_Hero, ChampInfos>();
            foreach (var enemy in _enemies.Reverse())
            {
                firstEnemyHero = enemy;
                break;
            }
            if (firstEnemyHero.Key != null &&
                Common.IsInside(cursorPos, firstEnemyHero.Value.SGui.SpellPassive.SizeSideBar,
                    _hudSize.Width, _hudSize.Height))
            {
                _moveActive = true;
                if (message == WindowsMessages.WM_MOUSEMOVE)
                {
                    var curSliderX =
                        Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker")
                            .GetMenuItem("SAwarenessUITrackerEnemyTrackerXPos")
                            .GetValue<Slider>();
                    var curSliderY =
                        Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker")
                            .GetMenuItem("SAwarenessUITrackerEnemyTrackerYPos")
                            .GetValue<Slider>();
                    Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker")
                        .GetMenuItem("SAwarenessUITrackerEnemyTrackerXPos")
                        .SetValue(new Slider((int) (curSliderX.Value + cursorPos.X - _lastCursorPos.X),
                            curSliderX.MinValue, curSliderX.MaxValue));
                    Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker")
                        .GetMenuItem("SAwarenessUITrackerEnemyTrackerYPos")
                        .SetValue(new Slider((int) (curSliderY.Value + cursorPos.Y - _lastCursorPos.Y),
                            curSliderY.MinValue, curSliderY.MaxValue));
                    _lastCursorPos = cursorPos;
                }
            }
            var firstAllyHero = new KeyValuePair<Obj_AI_Hero, ChampInfos>();
            foreach (var ally in _allies.Reverse())
            {
                firstAllyHero = ally;
                break;
            }
            if (firstAllyHero.Key != null &&
                Common.IsInside(cursorPos, firstAllyHero.Value.SGui.SpellPassive.SizeSideBar,
                    _hudSize.Width, _hudSize.Height))
            {
                _moveActive = true;
                if (message == WindowsMessages.WM_MOUSEMOVE)
                {
                    var curSliderX =
                        Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker")
                            .GetMenuItem("SAwarenessUITrackerAllyTrackerXPos")
                            .GetValue<Slider>();
                    var curSliderY =
                        Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker")
                            .GetMenuItem("SAwarenessUITrackerAllyTrackerYPos")
                            .GetValue<Slider>();
                    Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker")
                        .GetMenuItem("SAwarenessUITrackerAllyTrackerXPos")
                        .SetValue(new Slider((int) (curSliderX.Value + cursorPos.X - _lastCursorPos.X),
                            curSliderX.MinValue, curSliderX.MaxValue));
                    Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker")
                        .GetMenuItem("SAwarenessUITrackerAllyTrackerYPos")
                        .SetValue(new Slider((int) (curSliderY.Value + cursorPos.Y - _lastCursorPos.Y),
                            curSliderY.MinValue, curSliderY.MaxValue));
                    _lastCursorPos = cursorPos;
                }
            }
        }

        private void HandleChampClick(WindowsMessages message, Vector2 cursorPos, uint key)
        {
            if ((message != WindowsMessages.WM_KEYDOWN && key == 16) && message != WindowsMessages.WM_LBUTTONDOWN &&
                (message != WindowsMessages.WM_KEYUP && key == 16) ||
                (!_shiftActive && message == WindowsMessages.WM_LBUTTONDOWN))
            {
                return;
            }
            if (message == WindowsMessages.WM_KEYDOWN && key == 16)
            {
                _shiftActive = true;
            }
            if (message == WindowsMessages.WM_KEYUP && key == 16)
            {
                _shiftActive = false;
            }
            if (message == WindowsMessages.WM_LBUTTONDOWN)
            {
                foreach (var enemy in _enemies.Reverse())
                {
                    if (Common.IsInside(cursorPos, enemy.Value.SGui.Champ.SizeSideBar, _champSize.Width,
                        _champSize.Height))
                    {
                        //TODO: Add Camera move
                        if (Menu.UiTracker.GetMenuItem("SAwarenessUITrackerPingActive").GetValue<bool>())
                        {
                            Packet.S2C.Ping.Encoded(new Packet.S2C.Ping.Struct(enemy.Key.ServerPosition.X,
                                enemy.Key.ServerPosition.Y, 0, 0, Packet.PingType.Normal)).Process();
                        }
                    }
                }
            }
        }

        private void Drawing_OnPostReset(EventArgs args)
        {
            if (Drawing.Direct3DDevice == null || Drawing.Direct3DDevice.IsDisposed)
                return;
            _s.OnResetDevice();
            _champF.OnResetDevice();
            _spellF.OnResetDevice();
            _sumF.OnResetDevice();
            _recF.OnResetDevice();
            _recS.OnPostReset();
            _recB.OnPreReset();
            _recNS.OnPreReset();
            _drawActive = true;
        }

        private void Drawing_OnPreReset(EventArgs args)
        {
            _s.OnLostDevice();
            _champF.OnLostDevice();
            _spellF.OnLostDevice();
            _sumF.OnLostDevice();
            _recF.OnLostDevice();
            _recS.OnPreReset();
            _recB.OnPreReset();
            _recNS.OnPostReset();
            _drawActive = false;
        }

        ~UiTracker()
        {
            Game.OnGameUpdate -= Game_OnGameUpdate;
            Game.OnGameProcessPacket -= Game_OnGameProcessPacket;
            Drawing.OnPreReset -= Drawing_OnPreReset;
            Drawing.OnPostReset -= Drawing_OnPostReset;
            Drawing.OnEndScene -= Drawing_OnEndScene;
        }

        public bool IsActive()
        {
            return Menu.Tracker.GetActive() && Menu.UiTracker.GetActive();
        }

        private bool Init(bool force)
        {
            try
            {
                _s = new Sprite(Drawing.Direct3DDevice);
                _recF = new Font(Drawing.Direct3DDevice, new System.Drawing.Font("Times New Roman", 12));
                _spellF = new Font(Drawing.Direct3DDevice, new System.Drawing.Font("Times New Roman", 8));
                _champF = new Font(Drawing.Direct3DDevice, new System.Drawing.Font("Times New Roman", 24));
                //_champIF = new Font(Drawing.Direct3DDevice, new System.Drawing.Font("Times New Roman", 24));
                _sumF = new Font(Drawing.Direct3DDevice, new System.Drawing.Font("Times New Roman", 16));
                _recS = new Render.Rectangle(0, 0, 16, 16, SharpDX.Color.Green);
                _recB = new Render.Rectangle(0, 0, (int) (16*1.7), (int) (16*1.7), SharpDX.Color.Green);
                _recNS = new Render.Rectangle(0, 0, 32, 16, SharpDX.Color.Green);
            }
            catch (Exception)
            {
                return false;
                //throw;
            }
            if (
                Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker")
                    .GetMenuItem("SAwarenessUITrackerEnemyTrackerXPos")
                    .GetValue<Slider>()
                    .Value == -1)
            {
                Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker")
                    .GetMenuItem("SAwarenessUITrackerEnemyTrackerXPos")
                    .SetValue(new Slider((int) _screen.X, Drawing.Width, 0));
            }
            if (
                Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker")
                    .GetMenuItem("SAwarenessUITrackerEnemyTrackerYPos")
                    .GetValue<Slider>()
                    .Value == -1)
            {
                Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker")
                    .GetMenuItem("SAwarenessUITrackerEnemyTrackerYPos")
                    .SetValue(new Slider((int) _screen.Y, Drawing.Height, 0));
            }
            if (
                Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker")
                    .GetMenuItem("SAwarenessUITrackerAllyTrackerXPos")
                    .GetValue<Slider>()
                    .Value == -1)
            {
                Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker")
                    .GetMenuItem("SAwarenessUITrackerAllyTrackerXPos")
                    .SetValue(new Slider((int) _screen.X, Drawing.Width, 0));
            }
            if (
                Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker")
                    .GetMenuItem("SAwarenessUITrackerAllyTrackerYPos")
                    .GetValue<Slider>()
                    .Value == -1)
            {
                Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker")
                    .GetMenuItem("SAwarenessUITrackerAllyTrackerYPos")
                    .SetValue(new Slider((int) _screen.Y, Drawing.Height, 0));
            }

            //var loc = Assembly.GetExecutingAssembly().Location;
            //loc = loc.Remove(loc.LastIndexOf("\\", StringComparison.Ordinal));
            //loc = loc + "\\Sprites\\SAwareness\\";

            //SpriteHelper.LoadTexture("SummonerTint.dds", "SUMMONERS/", loc + "SUMMONERS\\SummonerTint.dds", ref _overlaySummoner);
            //SpriteHelper.LoadTexture("SummonerSpellTint.dds", "SUMMONERS/", loc + "SUMMONERS\\SummonerSpellTint.dds", ref _overlaySummonerSpell);
            //SpriteHelper.LoadTexture("SpellTint.dds", "SUMMONERS/", loc + "SUMMONERS\\SpellTint.dds", ref _overlaySpellItem);

            //SpriteHelper.LoadTexture("BarBackground.dds", "EXT/", loc + "EXT\\BarBackground.dds", ref _backBar);
            //SpriteHelper.LoadTexture("HealthBar.dds", "EXT/", loc + "EXT\\HealthBar.dds", ref _healthBar);
            //SpriteHelper.LoadTexture("ManaBar.dds", "EXT/", loc + "EXT\\ManaBar.dds", ref _manaBar);
            //SpriteHelper.LoadTexture("ItemSlotEmpty.dds", "EXT/", loc + "EXT\\ItemSlotEmpty.dds", ref _overlayEmptyItem);
            //SpriteHelper.LoadTexture("RecallBar.dds", "EXT/", loc + "EXT\\RecallBar.dds", ref _overlayRecall);

            SpriteHelper.LoadTexture("SummonerTint", ref _overlaySummoner, SpriteHelper.TextureType.Default);
            SpriteHelper.LoadTexture("SummonerSpellTint", ref _overlaySummonerSpell, SpriteHelper.TextureType.Default);
            SpriteHelper.LoadTexture("SpellTint", ref _overlaySpellItem, SpriteHelper.TextureType.Default);
            SpriteHelper.LoadTexture("SpellTintRed", ref _overlaySpellItemRed, SpriteHelper.TextureType.Default);
            SpriteHelper.LoadTexture("SpellTintGreen", ref _overlaySpellItemGreen, SpriteHelper.TextureType.Default);

            SpriteHelper.LoadTexture("BarBackground", ref _backBar, SpriteHelper.TextureType.Default);
            SpriteHelper.LoadTexture("HealthBar", ref _healthBar, SpriteHelper.TextureType.Default);
            SpriteHelper.LoadTexture("ManaBar", ref _manaBar, SpriteHelper.TextureType.Default);
            SpriteHelper.LoadTexture("ItemSlotEmpty", ref _overlayEmptyItem, SpriteHelper.TextureType.Default);
            SpriteHelper.LoadTexture("RecallBar", ref _overlayRecall, SpriteHelper.TextureType.Default);
            SpriteHelper.LoadTexture("GoldCsLvlBar", ref _overlayGoldCsLvl, SpriteHelper.TextureType.Default);


            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (hero.IsEnemy)
                {
                    var champ = new ChampInfos();
                    //SpriteHelper.LoadTexture(hero.ChampionName + ".dds", "CHAMP/", loc + "CHAMP\\" + hero.ChampionName + ".dds", ref champ.SGui.Champ.Texture);
                    SpriteHelper.LoadTexture(hero.ChampionName, ref champ.SGui.Champ.Texture,
                        SpriteHelper.TextureType.Default);
                    SpellDataInst[] s1 = hero.Spellbook.Spells;
                    //SpriteHelper.LoadTexture(s1[0].Name + ".dds", "PASSIVE/", loc + "PASSIVE\\" + s1[0].Name + ".dds", ref champ.SGui.Passive.Texture);
                    //SpriteHelper.LoadTexture(s1[0].Name + ".dds", "SPELLS/", loc + "SPELLS\\" + s1[0].Name + ".dds", ref champ.SGui.SpellQ.Texture);
                    //SpriteHelper.LoadTexture(s1[1].Name + ".dds", "SPELLS/", loc + "SPELLS\\" + s1[1].Name + ".dds", ref champ.SGui.SpellW.Texture);
                    //SpriteHelper.LoadTexture(s1[2].Name + ".dds", "SPELLS/", loc + "SPELLS\\" + s1[2].Name + ".dds", ref champ.SGui.SpellE.Texture);
                    //SpriteHelper.LoadTexture(s1[3].Name + ".dds", "SPELLS/", loc + "SPELLS\\" + s1[3].Name + ".dds", ref champ.SGui.SpellR.Texture);
                    SpriteHelper.LoadTexture(s1[0].Name, ref champ.SGui.SpellQ.Texture, SpriteHelper.TextureType.Default);
                    SpriteHelper.LoadTexture(s1[1].Name, ref champ.SGui.SpellW.Texture, SpriteHelper.TextureType.Default);
                    SpriteHelper.LoadTexture(s1[2].Name, ref champ.SGui.SpellE.Texture, SpriteHelper.TextureType.Default);
                    SpriteHelper.LoadTexture(s1[3].Name, ref champ.SGui.SpellR.Texture, SpriteHelper.TextureType.Default);

                    //var s2 = hero.SummonerSpellbook.Spells;
                    //SpriteHelper.LoadTexture(s2[0].Name + ".dds", "SUMMONERS/", loc + "SUMMONERS\\" + s2[0].Name + ".dds", ref champ.SGui.SpellSum1.Texture);
                    //SpriteHelper.LoadTexture(s2[1].Name + ".dds", "SUMMONERS/", loc + "SUMMONERS\\" + s2[1].Name + ".dds", ref champ.SGui.SpellSum2.Texture);
                    SpellDataInst[] s2 = hero.SummonerSpellbook.Spells;
                    SpriteHelper.LoadTexture(s2[0].Name + "1", ref champ.SGui.SpellSum1.Texture,
                        SpriteHelper.TextureType.Summoner);
                    SpriteHelper.LoadTexture(s2[1].Name + "1", ref champ.SGui.SpellSum2.Texture,
                        SpriteHelper.TextureType.Summoner);

                    _enemies.Add(hero, champ);
                }
            }

            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (!hero.IsEnemy && !hero.IsMe)
                {
                    var champ = new ChampInfos();
                    SpriteHelper.LoadTexture(hero.ChampionName, ref champ.SGui.Champ.Texture,
                        SpriteHelper.TextureType.Default);
                    SpellDataInst[] s1 = hero.Spellbook.Spells;
                    //SpriteHelper.LoadTexture(s1[0].Name + ".dds", "PASSIVE/", loc + "PASSIVE\\" + s1[0].Name + ".dds", ref champ.SGui.Passive.Texture);
                    SpriteHelper.LoadTexture(s1[0].Name, ref champ.SGui.SpellQ.Texture, SpriteHelper.TextureType.Default);
                    SpriteHelper.LoadTexture(s1[1].Name, ref champ.SGui.SpellW.Texture, SpriteHelper.TextureType.Default);
                    SpriteHelper.LoadTexture(s1[2].Name, ref champ.SGui.SpellE.Texture, SpriteHelper.TextureType.Default);
                    SpriteHelper.LoadTexture(s1[3].Name, ref champ.SGui.SpellR.Texture, SpriteHelper.TextureType.Default);

                    SpellDataInst[] s2 = hero.SummonerSpellbook.Spells;
                    SpriteHelper.LoadTexture(s2[0].Name, ref champ.SGui.SpellSum1.Texture,
                        SpriteHelper.TextureType.Summoner);
                    SpriteHelper.LoadTexture(s2[1].Name + "1", ref champ.SGui.SpellSum2.Texture,
                        SpriteHelper.TextureType.Summoner);

                    _allies.Add(hero, champ);
                }
            }
            UpdateItems(true);
            UpdateItems(false);
            CalculateSizes(true);
            CalculateSizes(false);

            return true;
        }

        private void CalculateSizes(bool calcEenemy)
        {
            Dictionary<Obj_AI_Hero, ChampInfos> heroes;
            float percentScale;
            StringList mode;
            StringList modeHead;
            StringList modeDisplay;
            int count;
            int xOffset;
            int yOffset;
            int yOffsetAdd;
            if (calcEenemy)
            {
                heroes = _enemies;
                percentScale = (float) Menu.UiTracker.GetMenuItem("SAwarenessUITrackerScale").GetValue<Slider>().Value/
                               100;
                mode =
                    Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker")
                        .GetMenuItem("SAwarenessUITrackerEnemyTrackerMode")
                        .GetValue<StringList>();
                modeHead =
                    Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker")
                        .GetMenuItem("SAwarenessUITrackerEnemyTrackerHeadMode")
                        .GetValue<StringList>();
                modeDisplay =
                    Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker")
                        .GetMenuItem("SAwarenessUITrackerEnemyTrackerSideDisplayMode")
                        .GetValue<StringList>();
                count = 0;
                xOffset =
                    Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker")
                        .GetMenuItem("SAwarenessUITrackerEnemyTrackerXPos")
                        .GetValue<Slider>()
                        .Value;
                _oldEx = xOffset;
                yOffset =
                    Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker")
                        .GetMenuItem("SAwarenessUITrackerEnemyTrackerYPos")
                        .GetValue<Slider>()
                        .Value;
                _oldEy = yOffset;
                yOffsetAdd = (int) (20*percentScale);
            }
            else
            {
                heroes = _allies;
                percentScale = (float) Menu.UiTracker.GetMenuItem("SAwarenessUITrackerScale").GetValue<Slider>().Value/
                               100;
                mode =
                    Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker")
                        .GetMenuItem("SAwarenessUITrackerAllyTrackerMode")
                        .GetValue<StringList>();
                modeHead =
                    Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker")
                        .GetMenuItem("SAwarenessUITrackerAllyTrackerHeadMode")
                        .GetValue<StringList>();
                modeDisplay =
                    Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker")
                        .GetMenuItem("SAwarenessUITrackerAllyTrackerSideDisplayMode")
                        .GetValue<StringList>();
                count = 0;
                xOffset =
                    Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker")
                        .GetMenuItem("SAwarenessUITrackerAllyTrackerXPos")
                        .GetValue<Slider>()
                        .Value;
                _oldAx = xOffset;
                yOffset =
                    Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker")
                        .GetMenuItem("SAwarenessUITrackerAllyTrackerYPos")
                        .GetValue<Slider>()
                        .Value;
                _oldAy = yOffset;
                yOffsetAdd = (int) (20*percentScale);
            }

            _hudSize = new Size();
            foreach (var hero in heroes)
            {
                if (mode.SelectedIndex == 0 || mode.SelectedIndex == 2)
                {
                    if (modeDisplay.SelectedIndex == 0)
                    {
                        hero.Value.SGui.SpellPassive.SizeSideBar =
                        new Size(
                            xOffset - (int)(_champSize.Width * percentScale) - (int)(_sumSize.Width * percentScale) -
                            (int)(_spellSize.Width * percentScale),
                            yOffset - (int)(_spellSize.Height * percentScale) * (count * 4 - 0) -
                            count * (int)(_backBarSize.Height * percentScale) -
                            count * (int)(_spellSize.Height * percentScale) - yOffsetAdd);
                        hero.Value.SGui.SpellQ.SizeSideBar = new Size(hero.Value.SGui.SpellPassive.SizeSideBar.Width,
                            hero.Value.SGui.SpellPassive.SizeSideBar.Height + (int)(_spellSize.Height * percentScale) * 1);
                        hero.Value.SGui.SpellW.SizeSideBar = new Size(hero.Value.SGui.SpellPassive.SizeSideBar.Width,
                            hero.Value.SGui.SpellPassive.SizeSideBar.Height + (int)(_spellSize.Height * percentScale) * 2);
                        hero.Value.SGui.SpellE.SizeSideBar = new Size(hero.Value.SGui.SpellPassive.SizeSideBar.Width,
                            hero.Value.SGui.SpellPassive.SizeSideBar.Height + (int)(_spellSize.Height * percentScale) * 3);
                        hero.Value.SGui.SpellR.SizeSideBar = new Size(hero.Value.SGui.SpellPassive.SizeSideBar.Width,
                            hero.Value.SGui.SpellPassive.SizeSideBar.Height + (int)(_spellSize.Height * percentScale) * 4);

                        hero.Value.SGui.Champ.SizeSideBar =
                            new Size(
                                hero.Value.SGui.SpellPassive.SizeSideBar.Width + (int)(_spellSize.Width * percentScale),
                                hero.Value.SGui.SpellPassive.SizeSideBar.Height);
                        hero.Value.SGui.SpellSum1.SizeSideBar =
                            new Size(hero.Value.SGui.Champ.SizeSideBar.Width + (int)(_champSize.Width * percentScale),
                                hero.Value.SGui.SpellPassive.SizeSideBar.Height);
                        hero.Value.SGui.SpellSum2.SizeSideBar = new Size(hero.Value.SGui.SpellSum1.SizeSideBar.Width,
                            hero.Value.SGui.SpellPassive.SizeSideBar.Height + (int)(_sumSize.Height * percentScale));

                        if (hero.Value.SGui.Item[0] == null)
                            hero.Value.SGui.Item[0] = new ChampInfos.Gui.SpriteInfos();
                        hero.Value.SGui.Item[0].SizeSideBar = new Size(hero.Value.SGui.SpellR.SizeSideBar.Width,
                            hero.Value.SGui.SpellR.SizeSideBar.Height + (int)(_spellSize.Height * percentScale));
                        for (int i = 1; i < hero.Value.SGui.Item.Length; i++)
                        {
                            if (hero.Value.SGui.Item[i] == null)
                                hero.Value.SGui.Item[i] = new ChampInfos.Gui.SpriteInfos();
                            hero.Value.SGui.Item[i].SizeSideBar =
                                new Size(
                                    hero.Value.SGui.Item[0].SizeSideBar.Width + (int)(_spellSize.Width * percentScale) * i,
                                    hero.Value.SGui.Item[0].SizeSideBar.Height);
                        }

                        hero.Value.SGui.SpellSum1.CoordsSideBar =
                            new Size(hero.Value.SGui.SpellSum1.SizeSideBar.Width + (int)(_sumSize.Width * percentScale) / 2,
                                hero.Value.SGui.SpellSum1.SizeSideBar.Height + (int)(_sumSize.Height * percentScale) / 8);
                        hero.Value.SGui.SpellSum2.CoordsSideBar =
                            new Size(hero.Value.SGui.SpellSum2.SizeSideBar.Width + (int)(_sumSize.Width * percentScale) / 2,
                                hero.Value.SGui.SpellSum2.SizeSideBar.Height + (int)(_sumSize.Height * percentScale) / 8);
                        hero.Value.SGui.Champ.CoordsSideBar =
                            new Size(hero.Value.SGui.Champ.SizeSideBar.Width + (int)(_champSize.Width * percentScale) / 2,
                                hero.Value.SGui.Champ.SizeSideBar.Height + (int)(_champSize.Height * percentScale) / 5);
                        hero.Value.SGui.SpellPassive.CoordsSideBar =
                            new Size(
                                hero.Value.SGui.SpellPassive.SizeSideBar.Width + (int)(_spellSize.Width * percentScale) / 2,
                                hero.Value.SGui.SpellPassive.SizeSideBar.Height + (int)(_spellSize.Height * percentScale) / 8);
                        hero.Value.SGui.SpellQ.CoordsSideBar =
                            new Size(hero.Value.SGui.SpellQ.SizeSideBar.Width + (int)(_spellSize.Width * percentScale) / 2,
                                hero.Value.SGui.SpellQ.SizeSideBar.Height + (int)(_spellSize.Height * percentScale) / 8);
                        hero.Value.SGui.SpellW.CoordsSideBar =
                            new Size(hero.Value.SGui.SpellW.SizeSideBar.Width + (int)(_spellSize.Width * percentScale) / 2,
                                hero.Value.SGui.SpellW.SizeSideBar.Height + (int)(_spellSize.Height * percentScale) / 8);
                        hero.Value.SGui.SpellE.CoordsSideBar =
                            new Size(hero.Value.SGui.SpellE.SizeSideBar.Width + (int)(_spellSize.Width * percentScale) / 2,
                                hero.Value.SGui.SpellE.SizeSideBar.Height + (int)(_spellSize.Height * percentScale) / 8);
                        hero.Value.SGui.SpellR.CoordsSideBar =
                            new Size(hero.Value.SGui.SpellR.SizeSideBar.Width + (int)(_spellSize.Width * percentScale) / 2,
                                hero.Value.SGui.SpellR.SizeSideBar.Height + (int)(_spellSize.Height * percentScale) / 8);

                        hero.Value.SGui.BackBar.SizeSideBar = new Size(hero.Value.SGui.Champ.SizeSideBar.Width,
                            hero.Value.SGui.SpellSum2.SizeSideBar.Height + (int)(_sumSize.Height * percentScale));
                        hero.Value.SGui.HealthBar.SizeSideBar = new Size(hero.Value.SGui.BackBar.SizeSideBar.Width,
                            hero.Value.SGui.BackBar.SizeSideBar.Height);
                        hero.Value.SGui.ManaBar.SizeSideBar = new Size(hero.Value.SGui.BackBar.SizeSideBar.Width,
                            hero.Value.SGui.BackBar.SizeSideBar.Height + (int)(_healthManaBarSize.Height * percentScale) + 3);
                        hero.Value.SGui.SHealth = ((int)hero.Key.Health) + "/" + ((int)hero.Key.MaxHealth);
                        hero.Value.SGui.SMana = ((int)hero.Key.Mana) + "/" + ((int)hero.Key.MaxMana);
                        hero.Value.SGui.HealthBar.CoordsSideBar =
                            new Size(
                                hero.Value.SGui.HealthBar.SizeSideBar.Width +
                                (int)(_healthManaBarSize.Width * percentScale) / 2,
                                hero.Value.SGui.HealthBar.SizeSideBar.Height -
                                (int)(_healthManaBarSize.Height * percentScale) / 2);
                        hero.Value.SGui.ManaBar.CoordsSideBar =
                            new Size(
                                hero.Value.SGui.ManaBar.SizeSideBar.Width + (int)(_healthManaBarSize.Width * percentScale) / 2,
                                hero.Value.SGui.ManaBar.SizeSideBar.Height -
                                (int)(_healthManaBarSize.Height * percentScale) / 2);

                        if (hero.Value.SGui.Item[0] == null)
                            hero.Value.SGui.Item[0] = new ChampInfos.Gui.SpriteInfos();
                        hero.Value.SGui.Item[0].CoordsSideBar = new Size(hero.Value.SGui.SpellR.CoordsSideBar.Width,
                            hero.Value.SGui.SpellR.CoordsSideBar.Height + (int)(_spellSize.Height * percentScale));
                        for (int i = 1; i < hero.Value.SGui.Item.Length; i++)
                        {
                            if (hero.Value.SGui.Item[i] == null)
                                hero.Value.SGui.Item[i] = new ChampInfos.Gui.SpriteInfos();
                            hero.Value.SGui.Item[i].CoordsSideBar =
                                new Size(
                                    hero.Value.SGui.Item[0].CoordsSideBar.Width + (int)(_spellSize.Width * percentScale) * i,
                                    hero.Value.SGui.Item[0].CoordsSideBar.Height);
                        }

                        hero.Value.SGui.RecallBar.SizeSideBar = new Size(hero.Value.SGui.Champ.SizeSideBar.Width,
                            hero.Value.SGui.BackBar.SizeSideBar.Height - (int)(_champSize.Height * percentScale) / 4);
                        hero.Value.SGui.RecallBar.CoordsSideBar =
                            new Size(hero.Value.SGui.RecallBar.SizeSideBar.Width + (int)(_recSize.Width * percentScale) / 2,
                                hero.Value.SGui.RecallBar.SizeSideBar.Height - (int)(_recSize.Height * percentScale) / 2);

                        hero.Value.SGui.Level.SizeSideBar = new Size(hero.Value.SGui.Champ.SizeSideBar.Width,
                            hero.Value.SGui.Champ.SizeSideBar.Height);
                        hero.Value.SGui.Level.CoordsSideBar =
                            new Size(hero.Value.SGui.Champ.SizeSideBar.Width + (int)(_champSize.Width * percentScale) / 8,
                                hero.Value.SGui.Champ.SizeSideBar.Height);

                        hero.Value.SGui.Cs.SizeSideBar = new Size(hero.Value.SGui.Champ.SizeSideBar.Width,
                            hero.Value.SGui.Champ.SizeSideBar.Height);
                        hero.Value.SGui.Cs.CoordsSideBar =
                            new Size(hero.Value.SGui.Champ.SizeSideBar.Width + (int)((_champSize.Width * percentScale) / 1.2),
                                hero.Value.SGui.Champ.SizeSideBar.Height);

                        hero.Value.SGui.Gold.SizeSideBar = new Size(hero.Value.SGui.Champ.SizeSideBar.Width,
                            hero.Value.SGui.Champ.SizeSideBar.Height);
                        hero.Value.SGui.Gold.CoordsSideBar =
                            new Size(hero.Value.SGui.Champ.SizeSideBar.Width + (int)(_champSize.Width * percentScale) / 2,
                                hero.Value.SGui.Champ.SizeSideBar.Height);

                        yOffsetAdd += (int)(5 * percentScale);
                        Size nSize = (hero.Value.SGui.Item[hero.Value.SGui.Item.Length - 1].SizeSideBar) -
                                     (hero.Value.SGui.SpellPassive.SizeSideBar);
                        nSize.Height += (int)(8 * percentScale);
                        _hudSize += nSize;
                        _hudSize.Width = nSize.Width;
                        _hudSize.Width += _spellSize.Width;
                        _hudSize.Height += (int)(20 * percentScale);
                        count++;
                    }
                    else
                    {
                        //yOffsetAdd = (int) (20*percentScale);
                        hero.Value.SGui.Champ.SizeSideBar =
                            new Size(
                                xOffset - (int)(_champSize.Width * percentScale),
                                yOffset - count * (int)(_champSize.Height * percentScale) -
                            count * (int)(_backBarSize.Height * percentScale) - yOffsetAdd);
                        hero.Value.SGui.SpellSum1.SizeSideBar =
                            new Size(hero.Value.SGui.Champ.SizeSideBar.Width - (int)(_sumSize.Width * percentScale),
                                hero.Value.SGui.Champ.SizeSideBar.Height);
                        hero.Value.SGui.SpellSum2.SizeSideBar = new Size(hero.Value.SGui.SpellSum1.SizeSideBar.Width,
                            hero.Value.SGui.Champ.SizeSideBar.Height + (int)(_sumSize.Height * percentScale));
                        hero.Value.SGui.SpellR.SizeSideBar = new Size(xOffset - (int)(_sumSize.Width * percentScale),
                            hero.Value.SGui.Champ.SizeSideBar.Height);

                        hero.Value.SGui.SpellSum1.CoordsSideBar =
                            new Size(hero.Value.SGui.SpellSum1.SizeSideBar.Width + (int)(_sumSize.Width * percentScale) / 2,
                                hero.Value.SGui.SpellSum1.SizeSideBar.Height + (int)(_sumSize.Height * percentScale) / 8);
                        hero.Value.SGui.SpellSum2.CoordsSideBar =
                            new Size(hero.Value.SGui.SpellSum2.SizeSideBar.Width + (int)(_sumSize.Width * percentScale) / 2,
                                hero.Value.SGui.SpellSum2.SizeSideBar.Height + (int)(_sumSize.Height * percentScale) / 8);
                        hero.Value.SGui.Champ.CoordsSideBar =
                            new Size(hero.Value.SGui.Champ.SizeSideBar.Width + (int)(_champSize.Width * percentScale) / 2,
                                hero.Value.SGui.Champ.SizeSideBar.Height + (int)(_champSize.Height * percentScale) / 8);
                        hero.Value.SGui.SpellR.CoordsSideBar =
                            new Size(hero.Value.SGui.SpellR.SizeSideBar.Width + (int)(_spellSize.Width * percentScale),
                                hero.Value.SGui.SpellR.SizeSideBar.Height + (int)(_spellSize.Height * percentScale) / 8);

                        hero.Value.SGui.BackBar.SizeSideBar = new Size(hero.Value.SGui.SpellSum1.SizeSideBar.Width,
                            hero.Value.SGui.SpellSum2.SizeSideBar.Height + (int)(_sumSize.Height * percentScale));
                        hero.Value.SGui.HealthBar.SizeSideBar = new Size(hero.Value.SGui.BackBar.SizeSideBar.Width,
                            hero.Value.SGui.BackBar.SizeSideBar.Height);
                        hero.Value.SGui.ManaBar.SizeSideBar = new Size(hero.Value.SGui.BackBar.SizeSideBar.Width,
                            hero.Value.SGui.BackBar.SizeSideBar.Height + (int)(_healthManaBarSize.Height * percentScale) + 3);
                        hero.Value.SGui.SHealth = ((int)hero.Key.Health) + "/" + ((int)hero.Key.MaxHealth);
                        hero.Value.SGui.SMana = ((int)hero.Key.Mana) + "/" + ((int)hero.Key.MaxMana);
                        hero.Value.SGui.HealthBar.CoordsSideBar =
                            new Size(
                                hero.Value.SGui.HealthBar.SizeSideBar.Width +
                                (int)(_healthManaBarSize.Width * percentScale) / 2,
                                hero.Value.SGui.HealthBar.SizeSideBar.Height -
                                (int)(_healthManaBarSize.Height * percentScale) / 2);
                        hero.Value.SGui.ManaBar.CoordsSideBar =
                            new Size(
                                hero.Value.SGui.ManaBar.SizeSideBar.Width + (int)(_healthManaBarSize.Width * percentScale) / 2,
                                hero.Value.SGui.ManaBar.SizeSideBar.Height -
                                (int)(_healthManaBarSize.Height * percentScale) / 2);

                        //For champ click/move
                        hero.Value.SGui.SpellPassive.SizeSideBar = hero.Value.SGui.SpellSum1.SizeSideBar;

                        yOffsetAdd += (int)(5 * percentScale);
                        Size nSize = (hero.Value.SGui.ManaBar.SizeSideBar) -
                                     (hero.Value.SGui.SpellSum1.SizeSideBar);
                        nSize.Height += (int)(8 * percentScale);
                        _hudSize += nSize;
                        _hudSize.Width = nSize.Width;
                        _hudSize.Width += _spellSize.Width;
                        _hudSize.Height += (int)(20 * percentScale);
                        count++;
                    }
                }
                if (mode.SelectedIndex == 1 || mode.SelectedIndex == 2)
                {
                    if (modeHead.SelectedIndex == 0)
                    {
                        const float hpPosScale = 0.8f;
                        Vector2 hpPos = hero.Key.HPBarPosition;
                        hero.Value.SGui.SpellSum1.SizeHpBar = new Size((int) hpPos.X - 20, (int) hpPos.Y);
                        hero.Value.SGui.SpellSum2.SizeHpBar = new Size(hero.Value.SGui.SpellSum1.SizeHpBar.Width,
                            hero.Value.SGui.SpellSum1.SizeHpBar.Height + (int) (_sumSize.Height*hpPosScale));
                        hero.Value.SGui.SpellPassive.SizeHpBar =
                            new Size(hero.Value.SGui.SpellSum1.SizeHpBar.Width + _sumSize.Width,
                                hero.Value.SGui.SpellSum2.SizeHpBar.Height + (int) ((_spellSize.Height*hpPosScale)/1.5));
                        hero.Value.SGui.SpellQ.SizeHpBar =
                            new Size(hero.Value.SGui.SpellPassive.SizeHpBar.Width + _spellSize.Width,
                                hero.Value.SGui.SpellPassive.SizeHpBar.Height);
                        hero.Value.SGui.SpellW.SizeHpBar =
                            new Size(hero.Value.SGui.SpellQ.SizeHpBar.Width + _spellSize.Width,
                                hero.Value.SGui.SpellQ.SizeHpBar.Height);
                        hero.Value.SGui.SpellE.SizeHpBar =
                            new Size(hero.Value.SGui.SpellW.SizeHpBar.Width + _spellSize.Width,
                                hero.Value.SGui.SpellW.SizeHpBar.Height);
                        hero.Value.SGui.SpellR.SizeHpBar =
                            new Size(hero.Value.SGui.SpellE.SizeHpBar.Width + _spellSize.Width,
                                hero.Value.SGui.SpellE.SizeHpBar.Height);

                        hero.Value.SGui.SpellSum1.CoordsHpBar =
                            new Size(hero.Value.SGui.SpellSum1.SizeHpBar.Width + _sumSize.Width/2,
                                hero.Value.SGui.SpellSum1.SizeHpBar.Height + _sumSize.Height/8);
                        hero.Value.SGui.SpellSum2.CoordsHpBar =
                            new Size(hero.Value.SGui.SpellSum2.SizeHpBar.Width + _sumSize.Width/2,
                                hero.Value.SGui.SpellSum2.SizeHpBar.Height + _sumSize.Height/8);
                        hero.Value.SGui.SpellPassive.CoordsHpBar =
                            new Size(hero.Value.SGui.SpellPassive.SizeHpBar.Width + _spellSize.Width/2,
                                hero.Value.SGui.SpellPassive.SizeHpBar.Height + _spellSize.Height/8);
                        hero.Value.SGui.SpellQ.CoordsHpBar =
                            new Size(hero.Value.SGui.SpellQ.SizeHpBar.Width + _spellSize.Width/2,
                                hero.Value.SGui.SpellQ.SizeHpBar.Height + _spellSize.Height/8);
                        hero.Value.SGui.SpellW.CoordsHpBar =
                            new Size(hero.Value.SGui.SpellW.SizeHpBar.Width + _spellSize.Width/2,
                                hero.Value.SGui.SpellW.SizeHpBar.Height + _spellSize.Height/8);
                        hero.Value.SGui.SpellE.CoordsHpBar =
                            new Size(hero.Value.SGui.SpellE.SizeHpBar.Width + _spellSize.Width/2,
                                hero.Value.SGui.SpellE.SizeHpBar.Height + _spellSize.Height/8);
                        hero.Value.SGui.SpellR.CoordsHpBar =
                            new Size(hero.Value.SGui.SpellR.SizeHpBar.Width + _spellSize.Width/2,
                                hero.Value.SGui.SpellR.SizeHpBar.Height + _spellSize.Height/8);
                    }
                    else
                    {
                        const float hpPosScale = 1.7f;
                        Vector2 hpPos = hero.Key.HPBarPosition;
                        hero.Value.SGui.SpellSum1.SizeHpBar = new Size((int) hpPos.X - 25, (int) hpPos.Y + 2);
                        hero.Value.SGui.SpellSum2.SizeHpBar = new Size(hero.Value.SGui.SpellSum1.SizeHpBar.Width,
                            hero.Value.SGui.SpellSum1.SizeHpBar.Height + (int) (_sumSize.Height*1.0f));
                        hero.Value.SGui.SpellPassive.SizeHpBar =
                            new Size(hero.Value.SGui.SpellSum1.SizeHpBar.Width + (int) (_spellSize.Width*hpPosScale),
                                hero.Value.SGui.SpellSum2.SizeHpBar.Height);
                        hero.Value.SGui.SpellQ.SizeHpBar =
                            new Size(
                                hero.Value.SGui.SpellPassive.SizeHpBar.Width + (int) (_spellSize.Width*hpPosScale),
                                hero.Value.SGui.SpellPassive.SizeHpBar.Height);
                        hero.Value.SGui.SpellW.SizeHpBar =
                            new Size(hero.Value.SGui.SpellQ.SizeHpBar.Width + (int) (_spellSize.Width*hpPosScale),
                                hero.Value.SGui.SpellQ.SizeHpBar.Height);
                        hero.Value.SGui.SpellE.SizeHpBar =
                            new Size(hero.Value.SGui.SpellW.SizeHpBar.Width + (int) (_spellSize.Width*hpPosScale),
                                hero.Value.SGui.SpellW.SizeHpBar.Height);
                        hero.Value.SGui.SpellR.SizeHpBar =
                            new Size(hero.Value.SGui.SpellE.SizeHpBar.Width + (int) (_spellSize.Width*hpPosScale),
                                hero.Value.SGui.SpellE.SizeHpBar.Height);

                        hero.Value.SGui.SpellSum1.CoordsHpBar =
                            new Size(hero.Value.SGui.SpellSum1.SizeHpBar.Width + _sumSize.Width/2,
                                hero.Value.SGui.SpellSum1.SizeHpBar.Height + _sumSize.Height/8);
                        hero.Value.SGui.SpellSum2.CoordsHpBar =
                            new Size(hero.Value.SGui.SpellSum2.SizeHpBar.Width + _sumSize.Width/2,
                                hero.Value.SGui.SpellSum2.SizeHpBar.Height + _sumSize.Height/8);
                        hero.Value.SGui.SpellPassive.CoordsHpBar =
                            new Size(hero.Value.SGui.SpellPassive.SizeHpBar.Width + (int) (_spellSize.Width/1.7),
                                hero.Value.SGui.SpellPassive.SizeHpBar.Height + _spellSize.Height/8);
                        hero.Value.SGui.SpellQ.CoordsHpBar =
                            new Size(hero.Value.SGui.SpellQ.SizeHpBar.Width + (int) (_spellSize.Width/1.3),
                                hero.Value.SGui.SpellQ.SizeHpBar.Height + _spellSize.Height/8);
                        hero.Value.SGui.SpellW.CoordsHpBar =
                            new Size(hero.Value.SGui.SpellW.SizeHpBar.Width + (int) (_spellSize.Width/1.3),
                                hero.Value.SGui.SpellW.SizeHpBar.Height + _spellSize.Height/8);
                        hero.Value.SGui.SpellE.CoordsHpBar =
                            new Size(hero.Value.SGui.SpellE.SizeHpBar.Width + (int) (_spellSize.Width/1.3),
                                hero.Value.SGui.SpellE.SizeHpBar.Height + _spellSize.Height/8);
                        hero.Value.SGui.SpellR.CoordsHpBar =
                            new Size(hero.Value.SGui.SpellR.SizeHpBar.Width + (int) (_spellSize.Width/1.3),
                                hero.Value.SGui.SpellR.SizeHpBar.Height + _spellSize.Height/8);
                    }
                }
            }
        }

        private void UpdateItems(bool enemy)
        {
            if (!Menu.UiTracker.GetMenuItem("SAwarenessItemPanelActive").GetValue<bool>())
                return;
            //var loc = Assembly.GetExecutingAssembly().Location;
            //loc = loc.Remove(loc.LastIndexOf("\\", StringComparison.Ordinal));
            //loc = loc + "\\Sprites\\SAwareness\\";

            Dictionary<Obj_AI_Hero, ChampInfos> heroes;

            if (enemy)
            {
                heroes = _enemies;
            }
            else
            {
                heroes = _allies;
            }

            foreach (var hero in heroes)
            {
                InventorySlot[] i1 = hero.Key.InventoryItems;
                ChampInfos champ = hero.Value;
                var slot = new List<int>();
                var unusedId = new List<int> {0, 1, 2, 3, 4, 5, 6};
                foreach (InventorySlot inventorySlot in i1)
                {
                    slot.Add(inventorySlot.Slot);
                    if (inventorySlot.Slot >= 0 && inventorySlot.Slot <= 6)
                    {
                        unusedId.Remove(inventorySlot.Slot);
                        if (champ.SGui.Item[inventorySlot.Slot] == null)
                            champ.SGui.Item[inventorySlot.Slot] = new ChampInfos.Gui.SpriteInfos();
                        if (champ.SGui.Item[inventorySlot.Slot].Texture == null ||
                            champ.SGui.ItemId[inventorySlot.Slot] != inventorySlot.Id)
                        {
                            //SpriteHelper.LoadTexture(inventorySlot.Id + ".dds", "ITEMS/",
                            //    loc + "ITEMS\\" + inventorySlot.Id + ".dds",
                            //    ref champ.SGui.Item[inventorySlot.Slot].Texture, true);
                            SpriteHelper.LoadTexture(inventorySlot.Id.ToString(),
                                ref champ.SGui.Item[inventorySlot.Slot].Texture, SpriteHelper.TextureType.Item);
                            if (champ.SGui.Item[inventorySlot.Slot].Texture != null)
                                champ.SGui.ItemId[inventorySlot.Slot] = inventorySlot.Id;
                        }
                    }
                }

                for (int i = 0; i < unusedId.Count; i++)
                {
                    int id = unusedId[i];
                    champ.SGui.ItemId[id] = 0;
                    if (champ.SGui.Item[id] == null)
                        champ.SGui.Item[id] = new ChampInfos.Gui.SpriteInfos();
                    champ.SGui.Item[id].Texture = null;
                    if ( /*id == i*/champ.SGui.Item[id].Texture == null &&
                                    champ.SGui.Item[id].Texture != _overlayEmptyItem)
                    {
                        champ.SGui.Item[id].Texture = _overlayEmptyItem;
                    }
                }
            }
        }

        private void UpdateCds(Dictionary<Obj_AI_Hero, ChampInfos> heroes)
        {
            try
            {
                UpdateItems(true);
                UpdateItems(false);

                foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>())
                {
                    foreach (var enemy in heroes)
                    {
                        enemy.Value.SGui.SHealth = ((int) enemy.Key.Health) + "/" + ((int) enemy.Key.MaxHealth);
                        enemy.Value.SGui.SMana = ((int) enemy.Key.Mana) + "/" + ((int) enemy.Key.MaxMana);
                        if (enemy.Key.NetworkId == hero.NetworkId)
                        {
                            foreach (var spell in hero.Spellbook.Spells)
                            {
                                if (spell.Slot == SpellSlot.Item1)
                                {
                                    if (spell.CooldownExpires - Game.Time > 0.0f)
                                    {
                                        enemy.Value.SGui.Item[0].Value = (int)(spell.CooldownExpires - Game.Time);
                                    }
                                    else if (spell.CooldownExpires - Game.Time <= 0.0f && enemy.Value.SGui.Item[0].Value != 0)
                                    {
                                        enemy.Value.SGui.Item[0].Value = 0;
                                    }
                                }
                                else if (spell.Slot == SpellSlot.Item2)
                                {
                                    if (spell.CooldownExpires - Game.Time > 0.0f)
                                    {
                                        enemy.Value.SGui.Item[1].Value = (int)(spell.CooldownExpires - Game.Time);
                                    }
                                    else if (spell.CooldownExpires - Game.Time <= 0.0f && enemy.Value.SGui.Item[1].Value != 0)
                                    {
                                        enemy.Value.SGui.Item[1].Value = 0;
                                    }
                                }
                                else if (spell.Slot == SpellSlot.Item3)
                                {
                                    if (spell.CooldownExpires - Game.Time > 0.0f)
                                    {
                                        enemy.Value.SGui.Item[2].Value = (int)(spell.CooldownExpires - Game.Time);
                                    }
                                    else if (spell.CooldownExpires - Game.Time <= 0.0f && enemy.Value.SGui.Item[2].Value != 0)
                                    {
                                        enemy.Value.SGui.Item[2].Value = 0;
                                    }
                                }
                                else if (spell.Slot == SpellSlot.Item4)
                                {
                                    if (spell.CooldownExpires - Game.Time > 0.0f)
                                    {
                                        enemy.Value.SGui.Item[3].Value = (int)(spell.CooldownExpires - Game.Time);
                                    }
                                    else if (spell.CooldownExpires - Game.Time <= 0.0f && enemy.Value.SGui.Item[3].Value != 0)
                                    {
                                        enemy.Value.SGui.Item[3].Value = 0;
                                    }
                                }
                                else if (spell.Slot == SpellSlot.Item5)
                                {
                                    if (spell.CooldownExpires - Game.Time > 0.0f)
                                    {
                                        enemy.Value.SGui.Item[4].Value = (int)(spell.CooldownExpires - Game.Time);
                                    }
                                    else if (spell.CooldownExpires - Game.Time <= 0.0f && enemy.Value.SGui.Item[4].Value != 0)
                                    {
                                        enemy.Value.SGui.Item[4].Value = 0;
                                    }
                                }
                                else if (spell.Slot == SpellSlot.Item6)
                                {
                                    if (spell.CooldownExpires - Game.Time > 0.0f)
                                    {
                                        enemy.Value.SGui.Item[5].Value = (int)(spell.CooldownExpires - Game.Time);
                                    }
                                    else if (spell.CooldownExpires - Game.Time <= 0.0f && enemy.Value.SGui.Item[5].Value != 0)
                                    {
                                        enemy.Value.SGui.Item[5].Value = 0;
                                    }
                                }
                                else if (spell.Slot == SpellSlot.Trinket)
                                {
                                    if (spell.CooldownExpires - Game.Time > 0.0f)
                                    {
                                        enemy.Value.SGui.Item[6].Value = (int)(spell.CooldownExpires - Game.Time);
                                    }
                                    else if (spell.CooldownExpires - Game.Time <= 0.0f && enemy.Value.SGui.Item[6].Value != 0)
                                    {
                                        enemy.Value.SGui.Item[6].Value = 0;
                                    }
                                }
                            }

                            SpellDataInst[] s1 = hero.Spellbook.Spells;
                            if (s1[0].CooldownExpires - Game.Time > 0.0f)
                            {
                                enemy.Value.SGui.SpellQ.Value = (int) (s1[0].CooldownExpires - Game.Time);
                            }
                            else if (s1[0].CooldownExpires - Game.Time <= 0.0f && enemy.Value.SGui.SpellQ.Value != 0)
                            {
                                enemy.Value.SGui.SpellQ.Value = 0;
                            }
                            if (s1[1].CooldownExpires - Game.Time > 0.0f)
                            {
                                enemy.Value.SGui.SpellW.Value = (int) (s1[1].CooldownExpires - Game.Time);
                            }
                            else if (s1[1].CooldownExpires - Game.Time <= 0.0f && enemy.Value.SGui.SpellW.Value != 0)
                            {
                                enemy.Value.SGui.SpellW.Value = 0;
                            }
                            if (s1[2].CooldownExpires - Game.Time > 0.0f)
                            {
                                enemy.Value.SGui.SpellE.Value = (int) (s1[2].CooldownExpires - Game.Time);
                            }
                            else if (s1[2].CooldownExpires - Game.Time <= 0.0f && enemy.Value.SGui.SpellE.Value != 0)
                            {
                                enemy.Value.SGui.SpellE.Value = 0;
                            }
                            if (s1[3].CooldownExpires - Game.Time > 0.0f)
                            {
                                enemy.Value.SGui.SpellR.Value = (int) (s1[3].CooldownExpires - Game.Time);
                            }
                            else if (s1[3].CooldownExpires - Game.Time <= 0.0f && enemy.Value.SGui.SpellR.Value != 0)
                            {
                                enemy.Value.SGui.SpellR.Value = 0;
                            }
                            SpellDataInst[] s2 = hero.SummonerSpellbook.Spells;
                            if (s2[0].CooldownExpires - Game.Time > 0.0f)
                            {
                                enemy.Value.SGui.SpellSum1.Value = (int) (s2[0].CooldownExpires - Game.Time);
                            }
                            else if (s2[0].CooldownExpires - Game.Time <= 0.0f && enemy.Value.SGui.SpellSum1.Value != 0)
                            {
                                enemy.Value.SGui.SpellSum1.Value = 0;
                            }
                            if (s2[1].CooldownExpires - Game.Time > 0.0f)
                            {
                                enemy.Value.SGui.SpellSum2.Value = (int) (s2[1].CooldownExpires - Game.Time);
                            }
                            else if (s2[1].CooldownExpires - Game.Time <= 0.0f && enemy.Value.SGui.SpellSum2.Value != 0)
                            {
                                enemy.Value.SGui.SpellSum2.Value = 0;
                            }
                            if (hero.IsVisible)
                            {
                                enemy.Value.SGui.InvisibleTime = 0;
                                enemy.Value.SGui.VisibleTime = (int) Game.Time;
                            }
                            else
                            {
                                if (enemy.Value.SGui.VisibleTime != 0)
                                {
                                    enemy.Value.SGui.InvisibleTime = (int) (Game.Time - enemy.Value.SGui.VisibleTime);
                                }
                                else
                                {
                                    enemy.Value.SGui.InvisibleTime = 0;
                                }
                            }

                            //Death
                            if (hero.IsDead && !enemy.Value.SGui.Dead)
                            {
                                enemy.Value.SGui.Dead = true;
                                float temp = enemy.Key.Level * 2.5f + 5 + 2;
                                if (Math.Floor(Game.Time / 60) >= 25)
                                {
                                    enemy.Value.SGui.DeathTime = (int)(temp + ((temp / 50) * (Math.Floor(Game.Time / 60) - 25))) + (int)Game.Time;
                                }
                                else
                                {
                                    enemy.Value.SGui.DeathTime = (int)temp + (int)Game.Time;
                                }
                                if (enemy.Key.ChampionName.Contains("KogMaw"))
                                {
                                    enemy.Value.SGui.DeathTime -= 4;
                                }
                            } 
                            else if (!hero.IsDead && enemy.Value.SGui.Dead)
                            {
                                enemy.Value.SGui.Dead = false;
                                enemy.Value.SGui.DeathTime = 0;
                            }
                            if (enemy.Value.SGui.DeathTime - Game.Time > 0.0f)
                            {
                                enemy.Value.SGui.DeathTimeDisplay = (int)(enemy.Value.SGui.DeathTime - Game.Time);
                            }
                            else if (enemy.Value.SGui.DeathTime - Game.Time <= 0.0f &&
                                     enemy.Value.SGui.DeathTimeDisplay != 0)
                            {
                                enemy.Value.SGui.DeathTimeDisplay = 0;
                            }
                            enemy.Value.SGui.Gold.Value = (int)enemy.Key.GoldEarned;//TODO: enable to get gold
                            enemy.Value.SGui.Cs.Value = enemy.Key.MinionsKilled;
                            enemy.Value.SGui.Level.Value = enemy.Key.Level;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("UITrackerUpdate: " + ex);
                throw;
            }
        }

        void Game_OnGameProcessPacket(GamePacketEventArgs args)
        {
            if (args.PacketData[0] == 0xC1 || args.PacketData[0] == 0xC2)
            {
                new System.Threading.Thread(() =>
                {
                    GetGold();
                }).Start();
            }
        }

        private void GetGold()
        {
            List<Spectator.Packet> packets = new List<Spectator.Packet>();
            if(SpecUtils.GameId == null)
                return;
            List<Byte[]> fullGameBytes = SpectatorDownloader.DownloadGameFiles(SpecUtils.GameId, SpecUtils.PlatformId, SpecUtils.Key, "KeyFrame");
            foreach (Byte[] chunkBytes in fullGameBytes)
            {
                packets.AddRange(SpectatorDecoder.DecodeBytes(chunkBytes));
            }
            foreach (Spectator.Packet p in packets)
            {
                if (p.header == (Byte)Spectator.HeaderId.PlayerStats)
                {
                    Spectator.PlayerStats playerStats = new Spectator.PlayerStats(p);
                    if (playerStats.GoldEarned <= 0.0f)
                        continue;
                    foreach (var ally in _allies)
                    {
                        if (ally.Key.NetworkId == playerStats.NetId)
                        {
                            //ally.Value.SGui.Gold = playerStats.GoldEarned;
                        }
                    }
                    foreach (var enemy in _enemies)
                    {
                        if (enemy.Key.NetworkId == playerStats.NetId)
                        {
                            //enemy.Value.SGui.Gold = playerStats.GoldEarned;
                        }
                    }
                }
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (!IsActive())
                return;

            UpdateCds(_enemies);
            UpdateCds(_allies);
            CalculateSizes(true);
            CalculateSizes(false);
        }

        private float CalcHpBar(Obj_AI_Hero hero)
        {
            float percent = (100/hero.MaxHealth*hero.Health);
            return percent/100;
        }

        private float CalcManaBar(Obj_AI_Hero hero)
        {
            float percent = (100/hero.MaxMana*hero.Mana);
            return (percent <= 0 || Single.IsNaN(percent) ? 0 : percent/100);
        }

        private float CalcRecallBar(RecallDetector.RecallInfo recall)
        {
            float maxTime = (recall.Recall.Duration/1000);
            float percent = (100/maxTime*(Game.Time - recall.StartTime));
            return (percent <= 100 ? percent/100 : 1);
        }

        private System.Drawing.Font CalcFont(int size, float scale)
        {
            double calcSize = (int) (size*scale);
            var newSize = (int) Math.Ceiling(calcSize);
            if (newSize%2 == 0 && newSize != 0)
                return new System.Drawing.Font("Times New Roman", (int) (size*scale));
            return null;
        }

        private void CheckValidSprite(ref Sprite sprite)
        {
            if (sprite.Device != Drawing.Direct3DDevice)
            {
                sprite = new Sprite(Drawing.Direct3DDevice);
            }
        }

        private void CheckValidFont(ref Font font)
        {
            if (font.Device != Drawing.Direct3DDevice)
            {
                AssingFonts(_scalePc, true);
            }
        }

        private void AssingFonts(float percentScale, bool force = false)
        {
            System.Drawing.Font font = CalcFont(12, percentScale);
            if (font != null || force)
                _recF = new Font(Drawing.Direct3DDevice, font);
            font = CalcFont(8, percentScale);
            if (font != null || force)
                _spellF = new Font(Drawing.Direct3DDevice, font);
            font = CalcFont(30, percentScale);
            if (font != null || force)
                _champF = new Font(Drawing.Direct3DDevice, font);
            font = CalcFont(16, percentScale);
            if (font != null || force)
                _sumF = new Font(Drawing.Direct3DDevice, font);
        }

        private RecallDetector.RecallInfo GetRecall(int networkId)
        {
            var t = Menu.RecallDetector.GetMenuItem("SAwarenessRecallDetectorMode").GetValue<StringList>();
            if (t.SelectedIndex == 1 || t.SelectedIndex == 2)
            {
                var recallDetector = (RecallDetector) Menu.RecallDetector.Item;
                if (recallDetector == null)
                    return null;
                foreach (RecallDetector.RecallInfo info in recallDetector.Recalls)
                {
                    if (info.NetworkId == networkId)
                    {
                        return info;
                    }
                }
            }
            return null;
        }

        private void DrawSideBarSimple(Dictionary<Obj_AI_Hero, ChampInfos> heroes, float percentScale)
        {
            _s.Begin();
            foreach (var hero in heroes)
            {
                float percentHealth = CalcHpBar(hero.Key);
                float percentMana = CalcManaBar(hero.Key);

                DirectXDrawer.DrawSprite(_s, hero.Value.SGui.Champ.Texture,
                    hero.Value.SGui.Champ.SizeSideBar,
                    new[] { 1.0f * percentScale, 1.0f * percentScale });
                DirectXDrawer.DrawSprite(_s, hero.Value.SGui.SpellSum1.Texture,
                    hero.Value.SGui.SpellSum1.SizeSideBar,
                    new[] { 1.0f * percentScale, 1.0f * percentScale });
                DirectXDrawer.DrawSprite(_s, hero.Value.SGui.SpellSum2.Texture,
                    hero.Value.SGui.SpellSum2.SizeSideBar,
                    new[] { 1.0f * percentScale, 1.0f * percentScale });

                DirectXDrawer.DrawSprite(_s, _backBar,
                    hero.Value.SGui.BackBar.SizeSideBar,
                    new[] { 1.0f * percentScale * 0.75f, 1.0f * percentScale });
                DirectXDrawer.DrawSprite(_s, _healthBar,
                    hero.Value.SGui.HealthBar.SizeSideBar,
                    new[] { 1.0f * percentHealth * percentScale * 0.75f, 1.0f * percentScale });
                DirectXDrawer.DrawSprite(_s, _manaBar,
                    hero.Value.SGui.ManaBar.SizeSideBar,
                    new[] { 1.0f * percentMana * percentScale * 0.75f, 1.0f * percentScale });

                if (hero.Value.SGui.DeathTimeDisplay > 0.0f)
                {
                    DirectXDrawer.DrawSprite(_s, _overlaySummoner,
                        hero.Value.SGui.Champ.SizeSideBar,
                        new ColorBGRA(Color3.White, 0.55f), new[] { 1.0f * percentScale, 1.0f * percentScale });
                }
                if (hero.Value.SGui.SpellSum1.Value > 0.0f)
                {
                    DirectXDrawer.DrawSprite(_s, _overlaySummonerSpell,
                        hero.Value.SGui.SpellSum1.SizeSideBar,
                        new ColorBGRA(Color3.White, 0.55f), new[] { 1.0f * percentScale, 1.0f * percentScale });
                }
                if (hero.Value.SGui.SpellSum2.Value > 0.0f)
                {
                    DirectXDrawer.DrawSprite(_s, _overlaySummonerSpell,
                        hero.Value.SGui.SpellSum2.SizeSideBar,
                        new ColorBGRA(Color3.White, 0.55f), new[] { 1.0f * percentScale, 1.0f * percentScale });
                }

                if (hero.Value.SGui.SpellR.Value > 0.0f || hero.Key.Spellbook.GetSpell(SpellSlot.R).Level < 1)
                {
                    DirectXDrawer.DrawSprite(_s, _overlaySpellItemRed,
                        hero.Value.SGui.SpellR.SizeSideBar,
                        new ColorBGRA(Color3.White, 0.65f), new[] { 2.0f * percentScale, 1.0f * percentScale });
                }
                else
                {
                    DirectXDrawer.DrawSprite(_s, _overlaySpellItemGreen,
                        hero.Value.SGui.SpellR.SizeSideBar,
                        new ColorBGRA(Color3.White, 0.65f), new[] { 2.0f * percentScale, 1.0f * percentScale });
                }
            }
            _s.End();

            foreach (var hero in heroes)
            {
                if (hero.Value.SGui.SpellR.Value > 0.0f)
                {
                    DirectXDrawer.DrawText(_spellF, hero.Value.SGui.SpellR.Value.ToString(),
                        hero.Value.SGui.SpellR.CoordsSideBar, SharpDX.Color.Orange);
                }
                if (hero.Value.SGui.DeathTimeDisplay > 0.0f && hero.Key.IsDead)
                {
                    DirectXDrawer.DrawText(_champF, hero.Value.SGui.DeathTimeDisplay.ToString(),
                        hero.Value.SGui.Champ.CoordsSideBar, SharpDX.Color.Orange);
                }
                else if (hero.Value.SGui.InvisibleTime > 0.0f && !hero.Key.IsVisible)
                {
                    DirectXDrawer.DrawText(_champF, hero.Value.SGui.InvisibleTime.ToString(),
                        hero.Value.SGui.Champ.CoordsSideBar, SharpDX.Color.Red);
                }
                if (hero.Value.SGui.SpellSum1.Value > 0.0f)
                {
                    DirectXDrawer.DrawText(_sumF, hero.Value.SGui.SpellSum1.Value.ToString(),
                        hero.Value.SGui.SpellSum1.CoordsSideBar, SharpDX.Color.Orange);
                }
                if (hero.Value.SGui.SpellSum2.Value > 0.0f)
                {
                    DirectXDrawer.DrawText(_sumF, hero.Value.SGui.SpellSum2.Value.ToString(),
                        hero.Value.SGui.SpellSum2.CoordsSideBar, SharpDX.Color.Orange);
                }
            }
        }

        private void DrawSideBarDefault(Dictionary<Obj_AI_Hero, ChampInfos> heroes, float percentScale)
        {
            _s.Begin();
            foreach (var hero in heroes)
            {
                float percentHealth = CalcHpBar(hero.Key);
                float percentMana = CalcManaBar(hero.Key);

                //DrawSprite(S, enemy.Value.PassiveTexture, nPassiveSize, Color.White);
                DirectXDrawer.DrawSprite(_s, hero.Value.SGui.SpellQ.Texture,
                    hero.Value.SGui.SpellQ.SizeSideBar,
                    new[] { 1.0f * percentScale, 1.0f * percentScale });
                DirectXDrawer.DrawSprite(_s, hero.Value.SGui.SpellW.Texture,
                    hero.Value.SGui.SpellW.SizeSideBar,
                    new[] { 1.0f * percentScale, 1.0f * percentScale });
                DirectXDrawer.DrawSprite(_s, hero.Value.SGui.SpellE.Texture,
                    hero.Value.SGui.SpellE.SizeSideBar,
                    new[] { 1.0f * percentScale, 1.0f * percentScale });
                DirectXDrawer.DrawSprite(_s, hero.Value.SGui.SpellR.Texture,
                    hero.Value.SGui.SpellR.SizeSideBar,
                    new[] { 1.0f * percentScale, 1.0f * percentScale });

                DirectXDrawer.DrawSprite(_s, hero.Value.SGui.Champ.Texture,
                    hero.Value.SGui.Champ.SizeSideBar,
                    new[] { 1.0f * percentScale, 1.0f * percentScale });
                DirectXDrawer.DrawSprite(_s, hero.Value.SGui.SpellSum1.Texture,
                    hero.Value.SGui.SpellSum1.SizeSideBar,
                    new[] { 1.0f * percentScale, 1.0f * percentScale });
                DirectXDrawer.DrawSprite(_s, hero.Value.SGui.SpellSum2.Texture,
                    hero.Value.SGui.SpellSum2.SizeSideBar,
                    new[] { 1.0f * percentScale, 1.0f * percentScale });

                DirectXDrawer.DrawSprite(_s, _backBar,
                    hero.Value.SGui.BackBar.SizeSideBar,
                    new[] { 1.0f * percentScale * 0.75f, 1.0f * percentScale });
                DirectXDrawer.DrawSprite(_s, _healthBar,
                    hero.Value.SGui.HealthBar.SizeSideBar,
                    new[] { 1.0f * percentHealth * percentScale * 0.75f, 1.0f * percentScale });
                DirectXDrawer.DrawSprite(_s, _manaBar,
                    hero.Value.SGui.ManaBar.SizeSideBar,
                    new[] { 1.0f * percentMana * percentScale * 0.75f, 1.0f * percentScale });

                if (Menu.UiTracker.GetMenuItem("SAwarenessItemPanelActive").GetValue<bool>())
                {
                    foreach (ChampInfos.Gui.SpriteInfos spriteInfo in hero.Value.SGui.Item)
                    {
                        DirectXDrawer.DrawSprite(_s, spriteInfo.Texture,
                            spriteInfo.SizeSideBar,
                            new[] { 1.0f * percentScale, 1.0f * percentScale });
                    }
                }

                if (hero.Value.SGui.SpellQ.Value > 0.0f || hero.Key.Spellbook.GetSpell(SpellSlot.Q).Level < 1)
                {
                    DirectXDrawer.DrawSprite(_s, _overlaySpellItem,
                        hero.Value.SGui.SpellQ.SizeSideBar,
                        new ColorBGRA(Color3.White, 0.55f), new[] { 1.0f * percentScale, 1.0f * percentScale });
                }
                if (hero.Value.SGui.SpellW.Value > 0.0f || hero.Key.Spellbook.GetSpell(SpellSlot.W).Level < 1)
                {
                    DirectXDrawer.DrawSprite(_s, _overlaySpellItem,
                        hero.Value.SGui.SpellW.SizeSideBar,
                        new ColorBGRA(Color3.White, 0.55f), new[] { 1.0f * percentScale, 1.0f * percentScale });
                }
                if (hero.Value.SGui.SpellE.Value > 0.0f || hero.Key.Spellbook.GetSpell(SpellSlot.E).Level < 1)
                {
                    DirectXDrawer.DrawSprite(_s, _overlaySpellItem,
                        hero.Value.SGui.SpellE.SizeSideBar,
                        new ColorBGRA(Color3.White, 0.55f), new[] { 1.0f * percentScale, 1.0f * percentScale });
                }
                if (hero.Value.SGui.SpellR.Value > 0.0f || hero.Key.Spellbook.GetSpell(SpellSlot.R).Level < 1)
                {
                    DirectXDrawer.DrawSprite(_s, _overlaySpellItem,
                        hero.Value.SGui.SpellR.SizeSideBar,
                        new ColorBGRA(Color3.White, 0.55f), new[] { 1.0f * percentScale, 1.0f * percentScale });
                }
                if (hero.Value.SGui.DeathTimeDisplay > 0.0f)
                {
                    DirectXDrawer.DrawSprite(_s, _overlaySummoner,
                        hero.Value.SGui.Champ.SizeSideBar,
                        new ColorBGRA(Color3.White, 0.55f), new[] { 1.0f * percentScale, 1.0f * percentScale });
                }
                if (hero.Value.SGui.SpellSum1.Value > 0.0f)
                {
                    DirectXDrawer.DrawSprite(_s, _overlaySummonerSpell,
                        hero.Value.SGui.SpellSum1.SizeSideBar,
                        new ColorBGRA(Color3.White, 0.55f), new[] { 1.0f * percentScale, 1.0f * percentScale });
                }
                if (hero.Value.SGui.SpellSum2.Value > 0.0f)
                {
                    DirectXDrawer.DrawSprite(_s, _overlaySummonerSpell,
                        hero.Value.SGui.SpellSum2.SizeSideBar,
                        new ColorBGRA(Color3.White, 0.55f), new[] { 1.0f * percentScale, 1.0f * percentScale });
                }
                if (Menu.UiTracker.GetMenuItem("SAwarenessItemPanelActive").GetValue<bool>())
                {
                    foreach (ChampInfos.Gui.SpriteInfos spriteInfo in hero.Value.SGui.Item)
                    {
                        if (spriteInfo.Value > 0.0f)
                        {
                            DirectXDrawer.DrawSprite(_s, _overlaySpellItem,
                                spriteInfo.SizeSideBar,
                                new ColorBGRA(Color3.White, 0.55f), new[] { 1.0f * percentScale, 1.0f * percentScale });
                        }
                    }
                }
                DirectXDrawer.DrawSprite(_s, _overlayGoldCsLvl,
                    hero.Value.SGui.Champ.SizeSideBar,
                    new ColorBGRA(Color3.White, 0.55f), new[] { 1.0f * percentScale, 1.0f * percentScale });
                if (Menu.RecallDetector.GetActive())
                {
                    RecallDetector.RecallInfo info = GetRecall(hero.Key.NetworkId);
                    if (info != null)
                    {
                        float percentRecall = CalcRecallBar(info);
                        if (info != null && info.StartTime != 0)
                        {
                            float time = Game.Time + info.Recall.Duration / 1000 - info.StartTime;
                            if (time > 0.0f &&
                                (info.Recall.Status == Packet.S2C.Recall.RecallStatus.TeleportStart ||
                                 info.Recall.Status == Packet.S2C.Recall.RecallStatus.RecallStarted))
                            {
                                DirectXDrawer.DrawSprite(_s, _overlayRecall,
                                    hero.Value.SGui.RecallBar.SizeSideBar,
                                    new ColorBGRA(Color3.White, 0.80f),
                                    new[] { 1.0f * percentRecall * percentScale, 1.0f * percentScale });
                            }
                            else if (time < 30.0f &&
                                     (info.Recall.Status == Packet.S2C.Recall.RecallStatus.TeleportEnd ||
                                      info.Recall.Status == Packet.S2C.Recall.RecallStatus.RecallFinished))
                            {
                                DirectXDrawer.DrawSprite(_s, _overlayRecall,
                                    hero.Value.SGui.RecallBar.SizeSideBar,
                                    new ColorBGRA(Color3.White, 0.80f),
                                    new[] { 1.0f * percentScale, 1.0f * percentScale });
                            }
                            else if (time < 30.0f &&
                                     (info.Recall.Status == Packet.S2C.Recall.RecallStatus.TeleportAbort ||
                                      info.Recall.Status == Packet.S2C.Recall.RecallStatus.RecallAborted))
                            {
                                DirectXDrawer.DrawSprite(_s, _overlayRecall,
                                    hero.Value.SGui.RecallBar.SizeSideBar,
                                    new ColorBGRA(Color3.White, 0.80f),
                                    new[] { 1.0f * percentScale, 1.0f * percentScale });
                            }
                        }
                    }
                }
            }
            _s.End();

            foreach (var hero in heroes)
            {
                DirectXDrawer.DrawText(_spellF, hero.Value.SGui.SHealth,
                    hero.Value.SGui.HealthBar.CoordsSideBar, SharpDX.Color.Orange);
                DirectXDrawer.DrawText(_spellF, hero.Value.SGui.SMana,
                    hero.Value.SGui.ManaBar.CoordsSideBar, SharpDX.Color.Orange);
                if (hero.Value.SGui.SpellQ.Value > 0.0f)
                {
                    DirectXDrawer.DrawText(_spellF, hero.Value.SGui.SpellQ.Value.ToString(),
                        hero.Value.SGui.SpellQ.CoordsSideBar, SharpDX.Color.Orange);
                }
                if (hero.Value.SGui.SpellW.Value > 0.0f)
                {
                    DirectXDrawer.DrawText(_spellF, hero.Value.SGui.SpellW.Value.ToString(),
                        hero.Value.SGui.SpellW.CoordsSideBar, SharpDX.Color.Orange);
                }
                if (hero.Value.SGui.SpellE.Value > 0.0f)
                {
                    DirectXDrawer.DrawText(_spellF, hero.Value.SGui.SpellE.Value.ToString(),
                        hero.Value.SGui.SpellE.CoordsSideBar, SharpDX.Color.Orange);
                }
                if (hero.Value.SGui.SpellR.Value > 0.0f)
                {
                    DirectXDrawer.DrawText(_spellF, hero.Value.SGui.SpellR.Value.ToString(),
                        hero.Value.SGui.SpellR.CoordsSideBar, SharpDX.Color.Orange);
                }
                if (hero.Value.SGui.DeathTimeDisplay > 0.0f && hero.Key.IsDead)
                {
                    DirectXDrawer.DrawText(_champF, hero.Value.SGui.DeathTimeDisplay.ToString(),
                        hero.Value.SGui.Champ.CoordsSideBar, SharpDX.Color.Orange);
                }
                else if (hero.Value.SGui.InvisibleTime > 0.0f && !hero.Key.IsVisible)
                {
                    DirectXDrawer.DrawText(_champF, hero.Value.SGui.InvisibleTime.ToString(),
                        hero.Value.SGui.Champ.CoordsSideBar, SharpDX.Color.Red);
                }
                if (hero.Value.SGui.SpellSum1.Value > 0.0f)
                {
                    DirectXDrawer.DrawText(_sumF, hero.Value.SGui.SpellSum1.Value.ToString(),
                        hero.Value.SGui.SpellSum1.CoordsSideBar, SharpDX.Color.Orange);
                }
                if (hero.Value.SGui.SpellSum2.Value > 0.0f)
                {
                    DirectXDrawer.DrawText(_sumF, hero.Value.SGui.SpellSum2.Value.ToString(),
                        hero.Value.SGui.SpellSum2.CoordsSideBar, SharpDX.Color.Orange);
                }
                if (Menu.UiTracker.GetMenuItem("SAwarenessItemPanelActive").GetValue<bool>())
                {
                    foreach (ChampInfos.Gui.SpriteInfos spriteInfo in hero.Value.SGui.Item)
                    {
                        if (spriteInfo.Value > 0.0f)
                        {
                            DirectXDrawer.DrawText(_spellF, spriteInfo.Value.ToString(),
                                spriteInfo.CoordsSideBar, SharpDX.Color.Orange);
                        }
                    }
                }
                DirectXDrawer.DrawText(_recF, hero.Value.SGui.Level.Value.ToString(),
                        hero.Value.SGui.Level.CoordsSideBar, SharpDX.Color.Orange);
                //DirectXDrawer.DrawText(_recF, hero.Value.SGui.Gold.Value.ToString(),
                //        hero.Value.SGui.Gold.CoordsSideBar, SharpDX.Color.Orange);
                DirectXDrawer.DrawText(_recF, hero.Value.SGui.Cs.Value.ToString(),
                        hero.Value.SGui.Cs.CoordsSideBar, SharpDX.Color.Orange);
                if (Menu.RecallDetector.GetActive())
                {
                    RecallDetector.RecallInfo info = GetRecall(hero.Key.NetworkId);
                    if (info != null && info.StartTime != 0)
                    {
                        float time = Game.Time + info.Recall.Duration / 1000 - info.StartTime;
                        if (time > 0.0f &&
                            (info.Recall.Status == Packet.S2C.Recall.RecallStatus.TeleportStart ||
                             info.Recall.Status == Packet.S2C.Recall.RecallStatus.RecallStarted))
                        {
                            DirectXDrawer.DrawText(_recF, "Porting",
                                hero.Value.SGui.RecallBar.CoordsSideBar,
                                SharpDX.Color.Chartreuse);
                        }
                        else if (time < 30.0f &&
                                 (info.Recall.Status == Packet.S2C.Recall.RecallStatus.TeleportEnd ||
                                  info.Recall.Status == Packet.S2C.Recall.RecallStatus.RecallFinished))
                        {
                            DirectXDrawer.DrawText(_recF, "Ported",
                                hero.Value.SGui.RecallBar.CoordsSideBar,
                                SharpDX.Color.Chartreuse);
                        }
                        else if (time < 30.0f &&
                                 (info.Recall.Status == Packet.S2C.Recall.RecallStatus.TeleportAbort ||
                                  info.Recall.Status == Packet.S2C.Recall.RecallStatus.RecallAborted))
                        {
                            DirectXDrawer.DrawText(_recF, "Canceled",
                                hero.Value.SGui.RecallBar.CoordsSideBar,
                                SharpDX.Color.Chartreuse);
                        }
                    }
                }
            }
        }

        private void DrawSideBar(Dictionary<Obj_AI_Hero, ChampInfos> heroes, float percentScale, StringList modeSideDisplayChoice)
        {
            if (modeSideDisplayChoice.SelectedIndex == 0)
            {
                DrawSideBarDefault(heroes, percentScale);
            }
            else
            {
                DrawSideBarSimple(heroes, percentScale);
            }
        }

        private void DrawOverHeadSimple(Dictionary<Obj_AI_Hero, ChampInfos> heroes, float percentScale, StringList modeHeadChoice)
        {
            if (modeHeadChoice.SelectedIndex == 0)
            {
                _s.Begin();
                foreach (var hero in heroes)
                {
                    if (!hero.Key.IsDead && hero.Key.IsVisible)
                    {
                        DirectXDrawer.DrawSprite(_s, hero.Value.SGui.SpellSum1.Texture,
                            hero.Value.SGui.SpellSum1.SizeHpBar,
                            new[] { 0.8f * percentScale, 0.8f * percentScale });
                        DirectXDrawer.DrawSprite(_s, hero.Value.SGui.SpellSum2.Texture,
                            hero.Value.SGui.SpellSum2.SizeHpBar,
                            new[] { 0.8f * percentScale, 0.8f * percentScale });

                        if (hero.Value.SGui.SpellQ.Value > 0.0f ||
                            hero.Key.Spellbook.GetSpell(SpellSlot.Q).Level < 1)
                        {
                            _recS.Color = SharpDX.Color.Red;
                        }
                        else
                        {
                            _recS.Color = SharpDX.Color.Green;
                        }
                        _recS.X = hero.Value.SGui.SpellQ.SizeHpBar.Width;
                        _recS.Y = hero.Value.SGui.SpellQ.SizeHpBar.Height;
                        _recS.OnEndScene();
                        if (hero.Value.SGui.SpellW.Value > 0.0f ||
                            hero.Key.Spellbook.GetSpell(SpellSlot.W).Level < 1)
                        {
                            _recS.Color = SharpDX.Color.Red;
                        }
                        else
                        {
                            _recS.Color = SharpDX.Color.Green;
                        }
                        _recS.X = hero.Value.SGui.SpellW.SizeHpBar.Width;
                        _recS.Y = hero.Value.SGui.SpellW.SizeHpBar.Height;
                        _recS.OnEndScene();
                        if (hero.Value.SGui.SpellE.Value > 0.0f ||
                            hero.Key.Spellbook.GetSpell(SpellSlot.E).Level < 1)
                        {
                            _recS.Color = SharpDX.Color.Red;
                        }
                        else
                        {
                            _recS.Color = SharpDX.Color.Green;
                        }
                        _recS.X = hero.Value.SGui.SpellE.SizeHpBar.Width;
                        _recS.Y = hero.Value.SGui.SpellE.SizeHpBar.Height;
                        _recS.OnEndScene();
                        if (hero.Value.SGui.SpellR.Value > 0.0f ||
                            hero.Key.Spellbook.GetSpell(SpellSlot.R).Level < 1)
                        {
                            _recS.Color = SharpDX.Color.Red;
                        }
                        else
                        {
                            _recS.Color = SharpDX.Color.Green;
                        }
                        _recS.X = hero.Value.SGui.SpellR.SizeHpBar.Width;
                        _recS.Y = hero.Value.SGui.SpellR.SizeHpBar.Height;
                        _recS.OnEndScene();
                        if (hero.Value.SGui.SpellSum1.Value > 0.0f)
                        {
                            DirectXDrawer.DrawSprite(_s, _overlaySummonerSpell,
                                hero.Value.SGui.SpellSum1.SizeHpBar,
                                new ColorBGRA(Color3.White, 0.55f),
                                new[] { 0.8f * percentScale, 0.8f * percentScale });
                        }
                        if (hero.Value.SGui.SpellSum2.Value > 0.0f)
                        {
                            DirectXDrawer.DrawSprite(_s, _overlaySummonerSpell,
                                hero.Value.SGui.SpellSum2.SizeHpBar,
                                new ColorBGRA(Color3.White, 0.55f),
                                new[] { 0.8f * percentScale, 0.8f * percentScale });
                        }
                    }
                }
                _s.End();
                foreach (var hero in heroes)
                {
                    if (!hero.Key.IsDead && hero.Key.IsVisible)
                    {
                        if (hero.Value.SGui.SpellQ.Value > 0.0f)
                        {
                            DirectXDrawer.DrawText(_spellF, hero.Value.SGui.SpellQ.Value.ToString(),
                                hero.Value.SGui.SpellQ.CoordsHpBar, SharpDX.Color.Orange);
                        }
                        else
                        {
                            DirectXDrawer.DrawText(_spellF, "Q",
                                hero.Value.SGui.SpellQ.CoordsHpBar, SharpDX.Color.Orange);
                        }
                        if (hero.Value.SGui.SpellW.Value > 0.0f)
                        {
                            DirectXDrawer.DrawText(_spellF, hero.Value.SGui.SpellW.Value.ToString(),
                                hero.Value.SGui.SpellW.CoordsHpBar, SharpDX.Color.Orange);
                        }
                        else
                        {
                            DirectXDrawer.DrawText(_spellF, "W",
                                hero.Value.SGui.SpellW.CoordsHpBar, SharpDX.Color.Orange);
                        }
                        if (hero.Value.SGui.SpellE.Value > 0.0f)
                        {
                            DirectXDrawer.DrawText(_spellF, hero.Value.SGui.SpellE.Value.ToString(),
                                hero.Value.SGui.SpellE.CoordsHpBar, SharpDX.Color.Orange);
                        }
                        else
                        {
                            DirectXDrawer.DrawText(_spellF, "E",
                                hero.Value.SGui.SpellE.CoordsHpBar, SharpDX.Color.Orange);
                        }
                        if (hero.Value.SGui.SpellR.Value > 0.0f)
                        {
                            DirectXDrawer.DrawText(_spellF, hero.Value.SGui.SpellR.Value.ToString(),
                                hero.Value.SGui.SpellR.CoordsHpBar, SharpDX.Color.Orange);
                        }
                        else
                        {
                            DirectXDrawer.DrawText(_spellF, "R",
                                hero.Value.SGui.SpellR.CoordsHpBar, SharpDX.Color.Orange);
                        }
                        if (hero.Value.SGui.SpellSum1.Value > 0.0f)
                        {
                            DirectXDrawer.DrawText(_sumF, hero.Value.SGui.SpellSum1.Value.ToString(),
                                hero.Value.SGui.SpellSum1.CoordsHpBar, SharpDX.Color.Orange);
                        }
                        if (hero.Value.SGui.SpellSum2.Value > 0.0f)
                        {
                            DirectXDrawer.DrawText(_sumF, hero.Value.SGui.SpellSum2.Value.ToString(),
                                hero.Value.SGui.SpellSum2.CoordsHpBar, SharpDX.Color.Orange);
                        }
                    }
                }
            }
            else
            {
                _s.Begin();
                foreach (var hero in heroes)
                {
                    if (!hero.Key.IsDead && hero.Key.IsVisible)
                    {
                        DirectXDrawer.DrawSprite(_s, hero.Value.SGui.SpellSum1.Texture,
                            hero.Value.SGui.SpellSum1.SizeHpBar,
                            new[] { 1.0f * percentScale, 1.0f * percentScale });
                        DirectXDrawer.DrawSprite(_s, hero.Value.SGui.SpellSum2.Texture,
                            hero.Value.SGui.SpellSum2.SizeHpBar,
                            new[] { 1.0f * percentScale, 1.0f * percentScale });

                        if (hero.Value.SGui.SpellQ.Value > 0.0f ||
                            hero.Key.Spellbook.GetSpell(SpellSlot.Q).Level < 1)
                        {
                            _recB.Color = SharpDX.Color.Red;
                        }
                        else
                        {
                            _recB.Color = SharpDX.Color.Green;
                        }
                        _recB.X = hero.Value.SGui.SpellQ.SizeHpBar.Width;
                        _recB.Y = hero.Value.SGui.SpellQ.SizeHpBar.Height;
                        _recB.OnEndScene();
                        if (hero.Value.SGui.SpellW.Value > 0.0f ||
                            hero.Key.Spellbook.GetSpell(SpellSlot.W).Level < 1)
                        {
                            _recB.Color = SharpDX.Color.Red;
                        }
                        else
                        {
                            _recB.Color = SharpDX.Color.Green;
                        }
                        _recB.X = hero.Value.SGui.SpellW.SizeHpBar.Width;
                        _recB.Y = hero.Value.SGui.SpellW.SizeHpBar.Height;
                        _recB.OnEndScene();
                        if (hero.Value.SGui.SpellE.Value > 0.0f ||
                            hero.Key.Spellbook.GetSpell(SpellSlot.E).Level < 1)
                        {
                            _recB.Color = SharpDX.Color.Red;
                        }
                        else
                        {
                            _recB.Color = SharpDX.Color.Green;
                        }
                        _recB.X = hero.Value.SGui.SpellE.SizeHpBar.Width;
                        _recB.Y = hero.Value.SGui.SpellE.SizeHpBar.Height;
                        _recB.OnEndScene();
                        if (hero.Value.SGui.SpellR.Value > 0.0f ||
                            hero.Key.Spellbook.GetSpell(SpellSlot.R).Level < 1)
                        {
                            _recB.Color = SharpDX.Color.Red;
                        }
                        else
                        {
                            _recB.Color = SharpDX.Color.Green;
                        }
                        _recB.X = hero.Value.SGui.SpellR.SizeHpBar.Width;
                        _recB.Y = hero.Value.SGui.SpellR.SizeHpBar.Height;
                        _recB.OnEndScene();
                    }
                }
                _s.End();
                foreach (var hero in heroes)
                {
                    if (!hero.Key.IsDead && hero.Key.IsVisible)
                    {
                        if (hero.Value.SGui.SpellQ.Value > 0.0f)
                        {
                            DirectXDrawer.DrawText(_sumF, hero.Value.SGui.SpellQ.Value.ToString(),
                                hero.Value.SGui.SpellQ.CoordsHpBar, SharpDX.Color.Orange);
                        }
                        else
                        {
                            DirectXDrawer.DrawText(_sumF, "Q",
                                hero.Value.SGui.SpellQ.CoordsHpBar, SharpDX.Color.Orange);
                        }
                        if (hero.Value.SGui.SpellW.Value > 0.0f)
                        {
                            DirectXDrawer.DrawText(_sumF, hero.Value.SGui.SpellW.Value.ToString(),
                                hero.Value.SGui.SpellW.CoordsHpBar, SharpDX.Color.Orange);
                        }
                        else
                        {
                            DirectXDrawer.DrawText(_sumF, "W",
                                hero.Value.SGui.SpellW.CoordsHpBar, SharpDX.Color.Orange);
                        }
                        if (hero.Value.SGui.SpellE.Value > 0.0f)
                        {
                            DirectXDrawer.DrawText(_sumF, hero.Value.SGui.SpellE.Value.ToString(),
                                hero.Value.SGui.SpellE.CoordsHpBar, SharpDX.Color.Orange);
                        }
                        else
                        {
                            DirectXDrawer.DrawText(_sumF, "E",
                                hero.Value.SGui.SpellE.CoordsHpBar, SharpDX.Color.Orange);
                        }
                        if (hero.Value.SGui.SpellR.Value > 0.0f)
                        {
                            DirectXDrawer.DrawText(_sumF, hero.Value.SGui.SpellR.Value.ToString(),
                                hero.Value.SGui.SpellR.CoordsHpBar, SharpDX.Color.Orange);
                        }
                        else
                        {
                            DirectXDrawer.DrawText(_sumF, "R",
                                hero.Value.SGui.SpellR.CoordsHpBar, SharpDX.Color.Orange);
                        }
                        if (hero.Value.SGui.SpellSum1.Value > 0.0f)
                        {
                            DirectXDrawer.DrawText(_sumF, hero.Value.SGui.SpellSum1.Value.ToString(),
                                hero.Value.SGui.SpellSum1.CoordsHpBar, SharpDX.Color.Orange);
                        }
                        if (hero.Value.SGui.SpellSum2.Value > 0.0f)
                        {
                            DirectXDrawer.DrawText(_sumF, hero.Value.SGui.SpellSum2.Value.ToString(),
                                hero.Value.SGui.SpellSum2.CoordsHpBar, SharpDX.Color.Orange);
                        }
                    }
                }
            }
        }

        private void DrawOverHeadDefault(Dictionary<Obj_AI_Hero, ChampInfos> heroes, float percentScale, StringList modeHeadChoice)
        {
            if (modeHeadChoice.SelectedIndex == 0)
            {
                _s.Begin();
                foreach (var hero in heroes)
                {
                    if (!hero.Key.IsDead && hero.Key.IsVisible)
                    {
                        DirectXDrawer.DrawSprite(_s, hero.Value.SGui.SpellPassive.Texture,
                            hero.Value.SGui.SpellPassive.SizeHpBar,
                            new[] { 1.0f * percentScale, 1.0f * percentScale });
                        DirectXDrawer.DrawSprite(_s, hero.Value.SGui.SpellQ.Texture,
                            hero.Value.SGui.SpellQ.SizeHpBar,
                            new[] { 1.0f * percentScale, 1.0f * percentScale });
                        DirectXDrawer.DrawSprite(_s, hero.Value.SGui.SpellW.Texture,
                            hero.Value.SGui.SpellW.SizeHpBar,
                            new[] { 1.0f * percentScale, 1.0f * percentScale });
                        DirectXDrawer.DrawSprite(_s, hero.Value.SGui.SpellE.Texture,
                            hero.Value.SGui.SpellE.SizeHpBar,
                            new[] { 1.0f * percentScale, 1.0f * percentScale });
                        DirectXDrawer.DrawSprite(_s, hero.Value.SGui.SpellR.Texture,
                            hero.Value.SGui.SpellR.SizeHpBar,
                            new[] { 1.0f * percentScale, 1.0f * percentScale });
                        DirectXDrawer.DrawSprite(_s, hero.Value.SGui.SpellSum1.Texture,
                            hero.Value.SGui.SpellSum1.SizeHpBar,
                            new[] { 0.8f * percentScale, 0.8f * percentScale });
                        DirectXDrawer.DrawSprite(_s, hero.Value.SGui.SpellSum2.Texture,
                            hero.Value.SGui.SpellSum2.SizeHpBar,
                            new[] { 0.8f * percentScale, 0.8f * percentScale });

                        if (hero.Value.SGui.SpellQ.Value > 0.0f ||
                            hero.Key.Spellbook.GetSpell(SpellSlot.Q).Level < 1)
                        {
                            DirectXDrawer.DrawSprite(_s, _overlaySpellItem,
                                hero.Value.SGui.SpellQ.SizeHpBar,
                                new ColorBGRA(Color3.White, 0.55f),
                                new[] { 1.0f * percentScale, 1.0f * percentScale });
                        }
                        if (hero.Value.SGui.SpellW.Value > 0.0f ||
                            hero.Key.Spellbook.GetSpell(SpellSlot.W).Level < 1)
                        {
                            DirectXDrawer.DrawSprite(_s, _overlaySpellItem,
                                hero.Value.SGui.SpellW.SizeHpBar,
                                new ColorBGRA(Color3.White, 0.55f),
                                new[] { 1.0f * percentScale, 1.0f * percentScale });
                        }
                        if (hero.Value.SGui.SpellE.Value > 0.0f ||
                            hero.Key.Spellbook.GetSpell(SpellSlot.E).Level < 1)
                        {
                            DirectXDrawer.DrawSprite(_s, _overlaySpellItem,
                                hero.Value.SGui.SpellE.SizeHpBar,
                                new ColorBGRA(Color3.White, 0.55f),
                                new[] { 1.0f * percentScale, 1.0f * percentScale });
                        }
                        if (hero.Value.SGui.SpellR.Value > 0.0f ||
                            hero.Key.Spellbook.GetSpell(SpellSlot.R).Level < 1)
                        {
                            DirectXDrawer.DrawSprite(_s, _overlaySpellItem,
                                hero.Value.SGui.SpellR.SizeHpBar,
                                new ColorBGRA(Color3.White, 0.55f),
                                new[] { 1.0f * percentScale, 1.0f * percentScale });
                        }
                        if (hero.Value.SGui.SpellSum1.Value > 0.0f)
                        {
                            DirectXDrawer.DrawSprite(_s, _overlaySummonerSpell,
                                hero.Value.SGui.SpellSum1.SizeHpBar,
                                new ColorBGRA(Color3.White, 0.55f),
                                new[] { 0.8f * percentScale, 0.8f * percentScale });
                        }
                        if (hero.Value.SGui.SpellSum2.Value > 0.0f)
                        {
                            DirectXDrawer.DrawSprite(_s, _overlaySummonerSpell,
                                hero.Value.SGui.SpellSum2.SizeHpBar,
                                new ColorBGRA(Color3.White, 0.55f),
                                new[] { 0.8f * percentScale, 0.8f * percentScale });
                        }
                    }
                }
                _s.End();
                foreach (var hero in heroes)
                {
                    if (!hero.Key.IsDead && hero.Key.IsVisible)
                    {
                        if (hero.Value.SGui.SpellQ.Value > 0.0f)
                        {
                            DirectXDrawer.DrawText(_spellF, hero.Value.SGui.SpellQ.Value.ToString(),
                                hero.Value.SGui.SpellQ.CoordsHpBar, SharpDX.Color.Orange);
                        }
                        if (hero.Value.SGui.SpellW.Value > 0.0f)
                        {
                            DirectXDrawer.DrawText(_spellF, hero.Value.SGui.SpellW.Value.ToString(),
                                hero.Value.SGui.SpellW.CoordsHpBar, SharpDX.Color.Orange);
                        }
                        if (hero.Value.SGui.SpellE.Value > 0.0f)
                        {
                            DirectXDrawer.DrawText(_spellF, hero.Value.SGui.SpellE.Value.ToString(),
                                hero.Value.SGui.SpellE.CoordsHpBar, SharpDX.Color.Orange);
                        }
                        if (hero.Value.SGui.SpellR.Value > 0.0f)
                        {
                            DirectXDrawer.DrawText(_spellF, hero.Value.SGui.SpellR.Value.ToString(),
                                hero.Value.SGui.SpellR.CoordsHpBar, SharpDX.Color.Orange);
                        }
                        if (hero.Value.SGui.SpellSum1.Value > 0.0f)
                        {
                            DirectXDrawer.DrawText(_sumF, hero.Value.SGui.SpellSum1.Value.ToString(),
                                hero.Value.SGui.SpellSum1.CoordsHpBar, SharpDX.Color.Orange);
                        }
                        if (hero.Value.SGui.SpellSum2.Value > 0.0f)
                        {
                            DirectXDrawer.DrawText(_sumF, hero.Value.SGui.SpellSum2.Value.ToString(),
                                hero.Value.SGui.SpellSum2.CoordsHpBar, SharpDX.Color.Orange);
                        }
                    }
                }
            }
            else
            {
                _s.Begin();
                foreach (var hero in heroes)
                {
                    if (!hero.Key.IsDead && hero.Key.IsVisible)
                    {
                        DirectXDrawer.DrawSprite(_s, hero.Value.SGui.SpellPassive.Texture,
                            hero.Value.SGui.SpellPassive.SizeHpBar,
                            new[] { 1.7f * percentScale, 1.7f * percentScale });
                        DirectXDrawer.DrawSprite(_s, hero.Value.SGui.SpellQ.Texture,
                            hero.Value.SGui.SpellQ.SizeHpBar,
                            new[] { 1.7f * percentScale, 1.7f * percentScale });
                        DirectXDrawer.DrawSprite(_s, hero.Value.SGui.SpellW.Texture,
                            hero.Value.SGui.SpellW.SizeHpBar,
                            new[] { 1.7f * percentScale, 1.7f * percentScale });
                        DirectXDrawer.DrawSprite(_s, hero.Value.SGui.SpellE.Texture,
                            hero.Value.SGui.SpellE.SizeHpBar,
                            new[] { 1.7f * percentScale, 1.7f * percentScale });
                        DirectXDrawer.DrawSprite(_s, hero.Value.SGui.SpellR.Texture,
                            hero.Value.SGui.SpellR.SizeHpBar,
                            new[] { 1.7f * percentScale, 1.7f * percentScale });
                        DirectXDrawer.DrawSprite(_s, hero.Value.SGui.SpellSum1.Texture,
                            hero.Value.SGui.SpellSum1.SizeHpBar,
                            new[] { 1.0f * percentScale, 1.0f * percentScale });
                        DirectXDrawer.DrawSprite(_s, hero.Value.SGui.SpellSum2.Texture,
                            hero.Value.SGui.SpellSum2.SizeHpBar,
                            new[] { 1.0f * percentScale, 1.0f * percentScale });

                        if (hero.Value.SGui.SpellQ.Value > 0.0f ||
                            hero.Key.Spellbook.GetSpell(SpellSlot.Q).Level < 1)
                        {
                            DirectXDrawer.DrawSprite(_s, _overlaySpellItem,
                                hero.Value.SGui.SpellQ.SizeHpBar,
                                new ColorBGRA(Color3.White, 0.55f),
                                new[] { 1.7f * percentScale, 1.7f * percentScale });
                        }
                        if (hero.Value.SGui.SpellW.Value > 0.0f ||
                            hero.Key.Spellbook.GetSpell(SpellSlot.W).Level < 1)
                        {
                            DirectXDrawer.DrawSprite(_s, _overlaySpellItem,
                                hero.Value.SGui.SpellW.SizeHpBar,
                                new ColorBGRA(Color3.White, 0.55f),
                                new[] { 1.7f * percentScale, 1.7f * percentScale });
                        }
                        if (hero.Value.SGui.SpellE.Value > 0.0f ||
                            hero.Key.Spellbook.GetSpell(SpellSlot.E).Level < 1)
                        {
                            DirectXDrawer.DrawSprite(_s, _overlaySpellItem,
                                hero.Value.SGui.SpellE.SizeHpBar,
                                new ColorBGRA(Color3.White, 0.55f),
                                new[] { 1.7f * percentScale, 1.7f * percentScale });
                        }
                        if (hero.Value.SGui.SpellR.Value > 0.0f ||
                            hero.Key.Spellbook.GetSpell(SpellSlot.R).Level < 1)
                        {
                            DirectXDrawer.DrawSprite(_s, _overlaySpellItem,
                                hero.Value.SGui.SpellR.SizeHpBar,
                                new ColorBGRA(Color3.White, 0.55f),
                                new[] { 1.7f * percentScale, 1.7f * percentScale });
                        }
                        if (hero.Value.SGui.SpellSum1.Value > 0.0f)
                        {
                            DirectXDrawer.DrawSprite(_s, _overlaySummonerSpell,
                                hero.Value.SGui.SpellSum1.SizeHpBar,
                                new ColorBGRA(Color3.White, 0.55f),
                                new[] { 1.0f * percentScale, 1.0f * percentScale });
                        }
                        if (hero.Value.SGui.SpellSum2.Value > 0.0f)
                        {
                            DirectXDrawer.DrawSprite(_s, _overlaySummonerSpell,
                                hero.Value.SGui.SpellSum2.SizeHpBar,
                                new ColorBGRA(Color3.White, 0.55f),
                                new[] { 1.0f * percentScale, 1.0f * percentScale });
                        }
                    }
                }
                _s.End();
                foreach (var hero in heroes)
                {
                    if (!hero.Key.IsDead && hero.Key.IsVisible)
                    {
                        if (hero.Value.SGui.SpellQ.Value > 0.0f)
                        {
                            DirectXDrawer.DrawText(_sumF, hero.Value.SGui.SpellQ.Value.ToString(),
                                hero.Value.SGui.SpellQ.CoordsHpBar, SharpDX.Color.Orange);
                        }
                        if (hero.Value.SGui.SpellW.Value > 0.0f)
                        {
                            DirectXDrawer.DrawText(_sumF, hero.Value.SGui.SpellW.Value.ToString(),
                                hero.Value.SGui.SpellW.CoordsHpBar, SharpDX.Color.Orange);
                        }
                        if (hero.Value.SGui.SpellE.Value > 0.0f)
                        {
                            DirectXDrawer.DrawText(_sumF, hero.Value.SGui.SpellE.Value.ToString(),
                                hero.Value.SGui.SpellE.CoordsHpBar, SharpDX.Color.Orange);
                        }
                        if (hero.Value.SGui.SpellR.Value > 0.0f)
                        {
                            DirectXDrawer.DrawText(_sumF, hero.Value.SGui.SpellR.Value.ToString(),
                                hero.Value.SGui.SpellR.CoordsHpBar, SharpDX.Color.Orange);
                        }
                        if (hero.Value.SGui.SpellSum1.Value > 0.0f)
                        {
                            DirectXDrawer.DrawText(_sumF, hero.Value.SGui.SpellSum1.Value.ToString(),
                                hero.Value.SGui.SpellSum1.CoordsHpBar, SharpDX.Color.Orange);
                        }
                        if (hero.Value.SGui.SpellSum2.Value > 0.0f)
                        {
                            DirectXDrawer.DrawText(_sumF, hero.Value.SGui.SpellSum2.Value.ToString(),
                                hero.Value.SGui.SpellSum2.CoordsHpBar, SharpDX.Color.Orange);
                        }
                    }
                }
            }
        }

        private void DrawOverHead(Dictionary<Obj_AI_Hero, ChampInfos> heroes, float percentScale, StringList modeHeadChoice, StringList modeHeadDisplayChoice)
        {           
            if (modeHeadDisplayChoice.SelectedIndex == 0)
            {
                DrawOverHeadDefault(heroes, percentScale, modeHeadChoice);
            }
            else
            {
                DrawOverHeadSimple(heroes, percentScale, modeHeadChoice);
            }
        }

        private void DrawInterface(bool enemy)
        {
            try
            {
                StringList modeChoice;
                StringList modeSideDisplayChoice;
                StringList modeHeadChoice;
                StringList modeHeadDisplayChoice;
                Dictionary<Obj_AI_Hero, ChampInfos> heroes;
                if (enemy)
                {
                    heroes = _enemies;
                    modeChoice =
                        Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker")
                            .GetMenuItem("SAwarenessUITrackerEnemyTrackerMode")
                            .GetValue<StringList>();
                    modeSideDisplayChoice =
                        Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker")
                            .GetMenuItem("SAwarenessUITrackerEnemyTrackerSideDisplayMode")
                            .GetValue<StringList>();
                    modeHeadChoice =
                        Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker")
                            .GetMenuItem("SAwarenessUITrackerEnemyTrackerHeadMode")
                            .GetValue<StringList>();
                    modeHeadDisplayChoice =
                        Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker")
                            .GetMenuItem("SAwarenessUITrackerEnemyTrackerHeadDisplayMode")
                            .GetValue<StringList>();
                }
                else
                {
                    heroes = _allies;
                    modeChoice =
                        Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker")
                            .GetMenuItem("SAwarenessUITrackerAllyTrackerMode")
                            .GetValue<StringList>();
                    modeSideDisplayChoice =
                        Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker")
                            .GetMenuItem("SAwarenessUITrackerAllyTrackerSideDisplayMode")
                            .GetValue<StringList>();
                    modeHeadChoice =
                        Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker")
                            .GetMenuItem("SAwarenessUITrackerAllyTrackerHeadMode")
                            .GetValue<StringList>();
                    modeHeadDisplayChoice =
                        Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker")
                            .GetMenuItem("SAwarenessUITrackerAllyTrackerHeadDisplayMode")
                            .GetValue<StringList>();
                }

                float percentScale =
                    (float) Menu.UiTracker.GetMenuItem("SAwarenessUITrackerScale").GetValue<Slider>().Value/100;
                if (
                    Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker")
                        .GetMenuItem("SAwarenessUITrackerEnemyTrackerXPos")
                        .GetValue<Slider>()
                        .Value != _oldEx ||
                    Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker")
                        .GetMenuItem("SAwarenessUITrackerEnemyTrackerYPos")
                        .GetValue<Slider>()
                        .Value != _oldEy
                    ||
                    Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker")
                        .GetMenuItem("SAwarenessUITrackerAllyTrackerXPos")
                        .GetValue<Slider>()
                        .Value != _oldAx ||
                    Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker")
                        .GetMenuItem("SAwarenessUITrackerAllyTrackerYPos")
                        .GetValue<Slider>()
                        .Value != _oldAy
                    || percentScale != _scalePc)
                {
                    CalculateSizes(true);
                    CalculateSizes(false);
                }

                if (percentScale != _scalePc)
                {
                    _scalePc = percentScale;
                    AssingFonts(percentScale);
                }

                StringList mode = modeChoice;
                if (_s == null || _s.IsDisposed)
                {
                    return;
                }                
                if (mode.SelectedIndex == 0 || mode.SelectedIndex == 2)
                {
                    DrawSideBar(heroes, percentScale, modeSideDisplayChoice);                    
                }
                if (mode.SelectedIndex == 1 || mode.SelectedIndex == 2)
                {
                    DrawOverHead(heroes, percentScale, modeHeadChoice, modeHeadDisplayChoice);
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                if (ex.GetType() == typeof (SharpDXException))
                {
                    Menu.UiTracker.SetActive(false);
                    Game.PrintChat("UITracker: An error occured. Please activate CDPanel in your menu again.");
                }
            }
        }

        private void Drawing_OnEndScene(EventArgs args)
        {
            if (!IsActive() || !_drawActive)
                return;

            if (Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerEnemyTracker").GetActive())
            {
                DrawInterface(true);
                int teamGoldEnemy = 0;
                foreach (var enemy in _enemies)
                {
                    teamGoldEnemy += enemy.Value.SGui.Gold.Value;
                }
                //DirectXDrawer.DrawText(_sumF, teamGoldEnemy.ToString(),
                //                _enemies.Last().Value.SGui.Champ.SizeSideBar, SharpDX.Color.Orange);
            }
            if (Menu.UiTracker.GetMenuSettings("SAwarenessUITrackerAllyTracker").GetActive())
            {
                DrawInterface(false);
                int teamGoldAlly = 0;
                foreach (var ally in ObjectManager.Get<Obj_AI_Hero>())
                {
                    if(ally.IsAlly)
                        teamGoldAlly += (int)ally.GoldEarned;
                }
                //DirectXDrawer.DrawText(_sumF, teamGoldAlly.ToString(),
                //                _allies.Last().Value.SGui.Champ.SizeSideBar, SharpDX.Color.Orange);
            }
        }

        private class ChampInfos
        {
            public readonly Gui SGui = new Gui();

            public class Gui
            {
                public readonly SpriteInfos BackBar = new SpriteInfos();
                public readonly SpriteInfos Champ = new SpriteInfos();
                public readonly SpriteInfos HealthBar = new SpriteInfos();
                public readonly SpriteInfos[] Item = new SpriteInfos[7];
                public readonly ItemId[] ItemId = new ItemId[7];
                public readonly SpriteInfos ManaBar = new SpriteInfos();
                public readonly SpriteInfos RecallBar = new SpriteInfos();
                public readonly SpriteInfos SpellE = new SpriteInfos();
                public readonly SpriteInfos SpellPassive = new SpriteInfos();
                public readonly SpriteInfos SpellQ = new SpriteInfos();
                public readonly SpriteInfos SpellR = new SpriteInfos();
                public readonly SpriteInfos SpellSum1 = new SpriteInfos();
                public readonly SpriteInfos SpellSum2 = new SpriteInfos();
                public readonly SpriteInfos SpellW = new SpriteInfos();
                public readonly SpriteInfos Gold = new SpriteInfos();
                public readonly SpriteInfos Level = new SpriteInfos();
                public readonly SpriteInfos Cs = new SpriteInfos();
                public int DeathTime;
                public int DeathTimeDisplay;
                public bool Dead;
                public int InvisibleTime;
                public Vector2 Pos = new Vector2();
                public String SHealth;
                public String SMana;
                public int VisibleTime;

                public class SpriteInfos
                {
                    public int Value;
                    public Size CoordsHpBar;
                    public Size CoordsSideBar;
                    public Size SizeHpBar;
                    public Size SizeSideBar;
                    public Texture Texture;
                }
            }
        }
    }

    public class UimTracker
    {
        private readonly Dictionary<Obj_AI_Hero, Texture> _enemies = new Dictionary<Obj_AI_Hero, Texture>();
        private Font _recF;
        private Sprite _s;
        private bool _drawActive = true;

        public UimTracker()
        {
            if (!IsActive())
                return;
            bool loaded = false;
            int tries = 0;
            while (!loaded)
            {
                loaded = Init(tries >= 5);

                tries++;
                if (tries > 9)
                {
                    Console.WriteLine("Couldn't load Interface. It got disabled.");
                    Menu.UimTracker.ForceDisable = true;
                    Menu.UimTracker.Item = null;
                    return;
                }
                Thread.Sleep(10);
            }

            Drawing.OnPreReset += Drawing_OnPreReset;
            Drawing.OnPostReset += Drawing_OnPostReset;
            Drawing.OnEndScene += Drawing_OnEndScene;
            AppDomain.CurrentDomain.DomainUnload += delegate { Drawing_OnPreReset(new EventArgs()); };
            AppDomain.CurrentDomain.ProcessExit += delegate { Drawing_OnPreReset(new EventArgs()); };
        }

        private void Drawing_OnPostReset(EventArgs args)
        {
            _s.OnResetDevice();
            _recF.OnResetDevice();
            _drawActive = true;
        }

        private void Drawing_OnPreReset(EventArgs args)
        {
            _s.OnLostDevice();
            _recF.OnLostDevice();
            _drawActive = false;
        }

        ~UimTracker()
        {
            Drawing.OnPreReset -= Drawing_OnPreReset;
            Drawing.OnPostReset -= Drawing_OnPostReset;
            Drawing.OnEndScene -= Drawing_OnEndScene;
        }

        public bool IsActive()
        {
            return Menu.Tracker.GetActive() && Menu.UimTracker.GetActive();
        }

        private bool Init(bool force)
        {
            try
            {
                _s = new Sprite(Drawing.Direct3DDevice);
                _recF = new Font(Drawing.Direct3DDevice, new System.Drawing.Font("Times New Roman", 8));
            }
            catch (Exception)
            {
                return false;
                //throw;
            }

            //var loc = Assembly.GetExecutingAssembly().Location;
            //loc = loc.Remove(loc.LastIndexOf("\\", StringComparison.Ordinal));
            //loc = loc + "\\Sprites\\SAwareness\\";

            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (hero.IsEnemy)
                {
                    Texture champ = null;
                    //SpriteHelper.LoadTexture(hero.ChampionName + ".dds", "CHAMP/", loc + "CHAMP\\" + hero.ChampionName + ".dds", ref champ);
                    SpriteHelper.LoadTexture(hero.ChampionName + "_MM", ref champ, SpriteHelper.TextureType.Default);
                    _enemies.Add(hero, champ);
                }
            }

            return true;
        }

        private RecallDetector.RecallInfo GetRecall(int networkId)
        {
            var t = Menu.RecallDetector.GetMenuItem("SAwarenessRecallDetectorMode").GetValue<StringList>();
            if (t.SelectedIndex == 1 || t.SelectedIndex == 2)
            {
                var recallDetector = (RecallDetector) Menu.RecallDetector.Item;
                if (recallDetector == null)
                    return null;
                foreach (RecallDetector.RecallInfo info in recallDetector.Recalls)
                {
                    if (info.NetworkId == networkId)
                    {
                        return info;
                    }
                }
            }
            return null;
        }

        private void Drawing_OnEndScene(EventArgs args)
        {
            if (!IsActive() || !_drawActive)
                return;
            try
            {
                float percentScale =
                    (float) Menu.UimTracker.GetMenuItem("SAwarenessUIMTrackerScale").GetValue<Slider>().Value/100;

                if (_s.IsDisposed)
                {
                    return;
                }
                _s.Begin();
                foreach (var enemy in _enemies)
                {
                    if (enemy.Key.IsVisible)
                        continue;
                    Vector2 serverPos = Drawing.WorldToMinimap(enemy.Key.ServerPosition);
                    var mPos = new Size((int) (serverPos[0] - 32*0.3f), (int) (serverPos[1] - 32*0.3f));
                    DirectXDrawer.DrawSprite(_s, enemy.Value,
                        mPos.ScaleSize(percentScale, new Vector2(mPos.Width, mPos.Height)),
                        new[] {0.7f*percentScale, 0.7f*percentScale});
                }
                _s.End();
                foreach (var enemy in _enemies)
                {
                    if (Menu.RecallDetector.GetActive())
                    {
                        RecallDetector.RecallInfo info = GetRecall(enemy.Key.NetworkId);
                        if (info != null && info.StartTime != 0)
                        {
                            float time = Game.Time + info.Recall.Duration/1000 - info.StartTime;
                            Vector2 vec = Drawing.WorldToMinimap(enemy.Key.ServerPosition);
                            var pos = new Size((int) vec.X, (int) vec.Y);
                            if (time > 0.0f &&
                                (info.Recall.Status == Packet.S2C.Recall.RecallStatus.TeleportStart ||
                                 info.Recall.Status == Packet.S2C.Recall.RecallStatus.RecallStarted))
                            {
                                DirectXDrawer.DrawText(_recF, enemy.Key.ChampionName, pos, SharpDX.Color.Chartreuse);
                            }
                        }
                    }
                }
                if (Menu.UimTracker.GetMenuItem("SAwarenessUIMTrackerShowSS").GetValue<bool>())
                { 
                    foreach (var enemy in SsCaller.Enemies)
                    {
                        if (Menu.SsCaller.GetActive())
                        {
                            if (!enemy.Key.IsVisible && enemy.Value.InvisibleTime > 0)
                            {
                                Vector2 vec = Drawing.WorldToMinimap(enemy.Key.ServerPosition);
                                var pos = new Size((int)vec.X, (int)vec.Y);
                                DirectXDrawer.DrawText(_recF, enemy.Value.InvisibleTime.ToString(), pos, SharpDX.Color.Chartreuse);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                if (ex.GetType() == typeof (SharpDXException))
                {
                    Menu.UimTracker.SetActive(false);
                    Game.PrintChat("UIM: An error occured. Please activate UI Minimap in your menu again.");
                }
            }
        }
    }

    internal class WaypointTracker
    {
        public WaypointTracker()
        {
            Drawing.OnDraw += Drawing_OnDraw;
        }

        ~WaypointTracker()
        {
            Drawing.OnDraw -= Drawing_OnDraw;
        }

        public bool IsActive()
        {
            return Menu.Tracker.GetActive() && Menu.WaypointTracker.GetActive();
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (!IsActive())
                return;
            foreach (Obj_AI_Hero enemy in ObjectManager.Get<Obj_AI_Hero>())
            {
                float arrivalTime = 0.0f;
                if (enemy.IsValid && enemy.IsVisible && !enemy.IsDead && enemy.IsEnemy)
                {
                    List<Vector2> waypoints = enemy.GetWaypoints();
                    for (int i = 0; i < waypoints.Count - 1; i++)
                    {
                        Vector2 oWp;
                        Vector2 nWp;
                        float time = 0;
                        oWp = Drawing.WorldToScreen(waypoints[i].To3D());
                        nWp = Drawing.WorldToScreen(waypoints[i + 1].To3D());
                        Drawing.DrawLine(oWp[0], oWp[1], nWp[0], nWp[1], 1, Color.White);
                        time =
                            ((Vector3.Distance(waypoints[i].To3D(), waypoints[i + 1].To3D())/
                              (ObjectManager.Player.MoveSpeed/1000))/1000);
                        time = (float) Math.Round(time, 2);
                        arrivalTime += time;
                        if (i == enemy.Path.Length - 1)
                        {
                            DrawCross(nWp[0], nWp[1], 1.0f, 3.0f, Color.Red);
                            Drawing.DrawText(nWp[0] - 15, nWp[1] + 10, Color.Red, arrivalTime.ToString());
                        }
                    }
                }
            }
        }

        private void DrawCross(float x, float y, float size, float thickness, Color color)
        {
            var topLeft = new Vector2(x - 10*size, y - 10*size);
            var topRight = new Vector2(x + 10*size, y - 10*size);
            var botLeft = new Vector2(x - 10*size, y + 10*size);
            var botRight = new Vector2(x + 10*size, y + 10*size);

            Drawing.DrawLine(topLeft.X, topLeft.Y, botRight.X, botRight.Y, thickness, color);
            Drawing.DrawLine(topRight.X, topRight.Y, botLeft.X, botLeft.Y, thickness, color);
        }
    }

    internal class Killable //TODO: Add more option for e.g. most damage first, add ignite spell
    {
        private readonly Render.Text _textF = new Render.Text("", 0, 0, 24, SharpDX.Color.Goldenrod);
        private bool _drawActive = true;
        Dictionary<Obj_AI_Hero, Combo> _enemies = new Dictionary<Obj_AI_Hero, Combo>();

        public Killable()
        {
            foreach (Obj_AI_Hero enemy in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (enemy.IsEnemy)
                {
                    _enemies.Add(enemy, CalculateKillable(enemy));
                }
            }
            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Drawing.OnPreReset += Drawing_OnPreReset;
            Drawing.OnPostReset += Drawing_OnPostReset;
        }

        ~Killable()
        {
            Game.OnGameUpdate -= Game_OnGameUpdate;
            Drawing.OnEndScene -= Drawing_OnEndScene;
            Drawing.OnPreReset -= Drawing_OnPreReset;
            Drawing.OnPostReset -= Drawing_OnPostReset;
        }

        public bool IsActive()
        {
            return Menu.Tracker.GetActive() && Menu.Killable.GetActive();
        }

        private Dictionary<Obj_AI_Hero, Combo> CalculateKillable()
        {
            foreach (var enemy in _enemies.ToArray())
            {
                _enemies[enemy.Key] = (CalculateKillable(enemy.Key));
            }
            return _enemies;
        }

        private Combo CalculateKillable(Obj_AI_Hero enemy)
        {
            var creationItemList = new Dictionary<Item, Damage.DamageItems>();
            var creationSpellList = new List<LeagueSharp.Common.Spell>();
            var tempSpellList = new List<Spell>();
            var tempItemList = new List<Item>();

            var ignite = new LeagueSharp.Common.Spell(Activator.GetIgniteSlot(), 1000);

            var q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1000);
            var w = new LeagueSharp.Common.Spell(SpellSlot.W, 1000);
            var e = new LeagueSharp.Common.Spell(SpellSlot.E, 1000);
            var r = new LeagueSharp.Common.Spell(SpellSlot.R, 1000);
            creationSpellList.Add(q);
            creationSpellList.Add(w);
            creationSpellList.Add(e);
            creationSpellList.Add(r);

            var dfg = new Item(3128, 1000, "Dfg");
            var bilgewater = new Item(3144, 1000, "Bilgewater");
            var hextechgun = new Item(3146, 1000, "Hextech");
            var blackfire = new Item(3188, 1000, "Blackfire");
            var botrk = new Item(3153, 1000, "Botrk");
            creationItemList.Add(dfg, Damage.DamageItems.Dfg);
            creationItemList.Add(bilgewater, Damage.DamageItems.Bilgewater);
            creationItemList.Add(hextechgun, Damage.DamageItems.Hexgun);
            creationItemList.Add(blackfire, Damage.DamageItems.BlackFireTorch);
            creationItemList.Add(botrk, Damage.DamageItems.Botrk);

            double enoughDmg = 0;
            double enoughMana = 0;

            foreach (var item in creationItemList)
            {
                if (item.Key.IsReady())
                {
                    enoughDmg += ObjectManager.Player.GetItemDamage(enemy, item.Value);
                    tempItemList.Add(item.Key);
                }
                if (enemy.Health < enoughDmg)
                {
                    return new Combo(null, tempItemList, true);
                }
            }

            foreach (LeagueSharp.Common.Spell spell in creationSpellList)
            {
                if (spell.IsReady())
                {
                    double spellDamage = spell.GetDamage(enemy, 0);
                    if (spellDamage > 0)
                    {
                        enoughDmg += spellDamage;
                        enoughMana += spell.Instance.ManaCost;
                        tempSpellList.Add(new Spell(spell.Slot.ToString(), spell.Slot));
                    }
                }
                if (enemy.Health < enoughDmg)
                {
                    if (ObjectManager.Player.Mana >= enoughMana)
                        return new Combo(tempSpellList, tempItemList, true);
                    return new Combo(null, null, false);
                }
            }

            if (Activator.GetIgniteSlot() != SpellSlot.Unknown && enemy.Health > enoughDmg)
            {
                enoughDmg += ObjectManager.Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
                tempSpellList.Add(new Spell("Ignite", ignite.Slot));
            }
            if (enemy.Health < enoughDmg)
            {
                return new Combo(tempSpellList, tempItemList, true);
            }

            return new Combo();
        }

        void Game_OnGameUpdate(EventArgs args)
        {
            CalculateKillable();
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
            if (!IsActive() || !_drawActive)
                return;

            int index = 0;
            foreach (var enemy in _enemies)
            {
                if (enemy.Value.Killable && enemy.Key.IsVisible && !enemy.Key.IsDead)
                {
                    String killText = "Killable " + enemy.Key.ChampionName + ": ";
                    if (enemy.Value.Spells != null && enemy.Value.Spells.Count > 0)
                        enemy.Value.Spells.ForEach(x => killText += x.Name + "/");
                    if (enemy.Value.Items != null && enemy.Value.Items.Count > 0)
                        enemy.Value.Items.ForEach(x => killText += x.Name + "/");
                    if (killText.Contains("/"))
                        killText = killText.Remove(killText.LastIndexOf("/"));
                    _textF.Centered = true;
                    _textF.text = killText;
                    _textF.X = Drawing.Width/2;
                    _textF.Y = (int) (Drawing.Height*0.80f - (17*index));
                    _textF.OutLined = true;
                    _textF.OnEndScene();
                    index++;
                }
            }
        }

        public class Combo
        {
            public List<Item> Items = new List<Item>();

            public bool Killable = false;
            public List<Spell> Spells = new List<Spell>();


            public Combo(List<Spell> spells, List<Item> items, bool killable)
            {
                Spells = spells;
                Items = items;
                Killable = killable;
            }

            public Combo()
            {
            }
        }

        public class Item : Items.Item
        {
            public String Name;

            public Item(int id, float range, String name) : base(id, range)
            {
                Name = name;
            }
        }

        public class Spell
        {
            public String Name;
            public SpellSlot SpellSlot;

            public Spell(String name, SpellSlot spellSlot)
            {
                Name = name;
                SpellSlot = spellSlot;
            }
        }
    }
}