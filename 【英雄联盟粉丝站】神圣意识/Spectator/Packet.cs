using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAwareness.Spectator
{
    enum HeaderId
    {
        PlayerStats = 0x46
    }

    public class Packet
    {

        public Boolean isPacketType = true;
        public UInt32 param;
        public Byte header;
        public Single time;
        public Byte[] content;
        public Packet(UInt32 param, Byte header, Single time, Byte[] content)
        {
            this.param = param;
            this.header = header;
            this.time = time;
            this.content = new Byte[content.Length];
            Array.Copy(content, this.content, content.Length);
        }
    }

    class PlayerStats : Packet
    {
        public UInt32 NetId { get { return param; } }
        public UInt32 Assists { get { return BitConverter.ToUInt32(content, 0x0C); } }
        public UInt32 Kills { get { return BitConverter.ToUInt32(content, 0x14); } }
        public UInt32 DoubleKills { get { return BitConverter.ToUInt32(content, 0x1C); } }
        public UInt32 UnrealKills { get { return BitConverter.ToUInt32(content, 0x2C); } }
        public Single GoldEarned { get { return BitConverter.ToSingle(content, 0x30); } }
        public Single GoldSpent { get { return BitConverter.ToSingle(content, 0x34); } }
        public UInt32 CurrentKillingSpree { get { return BitConverter.ToUInt32(content, 0x60); } }
        public Single LargestCrit { get { return BitConverter.ToSingle(content, 0x64); } }
        public UInt32 LargestKillingSpree { get { return BitConverter.ToUInt32(content, 0x68); } }
        public UInt32 LargestMultiKill { get { return BitConverter.ToUInt32(content, 0x6C); } }
        public Single LargestTimeAlive { get { return BitConverter.ToSingle(content, 0x74); } }
        public Single TotalMagicDamage { get { return BitConverter.ToSingle(content, 0x78); } }
        public Single ChampMagicDamage { get { return BitConverter.ToSingle(content, 0x7C); } }
        public Single TakenMagicDamage { get { return BitConverter.ToSingle(content, 0x80); } }
        public UInt32 TotalMinionsKilled { get { return BitConverter.ToUInt32(content, 0x84); } }
        public UInt32 NeutralMinionsKilled { get { return BitConverter.ToUInt32(content, 0x8A); } }
        public UInt32 EnemyNeutralMinionsKilled { get { return BitConverter.ToUInt32(content, 0x8E); } }
        public UInt32 TeamNeutralMinionsKilled { get { return BitConverter.ToUInt32(content, 0x92); } }
        public UInt32 Deaths { get { return BitConverter.ToUInt32(content, 0x9A); } }
        public UInt32 PentaKills { get { return BitConverter.ToUInt32(content, 0x9E); } } //todo
        public Single TotalPhysicalDamage { get { return BitConverter.ToSingle(content, 0x9E); } }
        public Single ChampPhysicalDamage { get { return BitConverter.ToSingle(content, 0xA2); } }
        public Single TakenPhysicalDamage { get { return BitConverter.ToSingle(content, 0xA6); } }
        public UInt32 QuadraKills { get { return BitConverter.ToUInt32(content, 0xAE); } }
        public UInt32 Team { get { return BitConverter.ToUInt32(content, 0xD6); } }
        public Single TotalDamage { get { return BitConverter.ToSingle(content, 0xEA); } }
        public Single ChampDamage { get { return BitConverter.ToSingle(content, 0xEE); } }
        public Single TakenDamage { get { return BitConverter.ToSingle(content, 0xF2); } }
        public UInt32 TotalHeal { get { return BitConverter.ToUInt32(content, 0xF6); } }
        public Single TotalCCTimeDealt { get { return BitConverter.ToSingle(content, 0xFA); } }
        public Single TotalTimeDead { get { return BitConverter.ToSingle(content, 0xFE); } }
        public UInt32 TotalUnitsHealed { get { return BitConverter.ToUInt32(content, 0x102); } }
        public UInt32 TripleKills { get { return BitConverter.ToUInt32(content, 0x106); } }
        public Single TotalTrueDamage { get { return BitConverter.ToSingle(content, 0x10A); } }
        public Single ChampTrueDamage { get { return BitConverter.ToSingle(content, 0x10E); } }
        public Single TakenTrueDamage { get { return BitConverter.ToSingle(content, 0x112); } }
        public UInt32 TowersDestroyed { get { return BitConverter.ToUInt32(content, 0x116); } }
        public UInt32 InhibsDestroyed { get { return BitConverter.ToUInt32(content, 0x11A); } }
        public UInt32 WardsDestroyed { get { return BitConverter.ToUInt32(content, 0x122); } }
        public UInt32 WardsPlaced { get { return BitConverter.ToUInt32(content, 0x126); } }
        public PlayerStats(Packet p)
            : base(p.param, p.header, p.time, p.content)
        {
        }
    }
}
