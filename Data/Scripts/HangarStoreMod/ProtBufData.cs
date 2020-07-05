using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace HangarStoreMod
{
    public enum MessageType
    {
        RequestAllItems,
        AddOne,
        RemoveOne,

        SendDefinition,
        PurchasedGrid
    }
    [ProtoContract]
    public class Message
    {
        [ProtoMember(1)] public GridsForSale GridDefinitions;
        [ProtoMember(2)] public List<MarketList> MarketBoxItmes;
        [ProtoMember(3)] public MessageType Type;
    }

    [ProtoContract]
    public class GridsForSale
    {
        //Items we send to block on request to preview the grid
        [ProtoMember(1)] public string name;
        [ProtoMember(2)] public byte[] GridDefinition;
        [ProtoMember(3)] public ulong SellerSteamid;
        [ProtoMember(4)] public ulong BuyerSteamid;
    }

    [ProtoContract]
    public class MarketList
    {
        //Items we will send to the block on load (Less lag)
        //Items we will send to the block on load (Less lag)

        [ProtoMember(1)] public string Name;
        [ProtoMember(2)] public string Description;
        [ProtoMember(3)] public string Seller = "Sold by Server";
        [ProtoMember(4)] public long Price;
        [ProtoMember(5)] public double MarketValue;
        [ProtoMember(6)] public ulong Steamid;


        //New items
        [ProtoMember(7)] public string SellerFaction = "N/A";
        [ProtoMember(8)] public float GridMass = 0;
        [ProtoMember(9)] public int SmallGrids = 0;
        [ProtoMember(10)] public int LargeGrids = 0;
        [ProtoMember(11)] public int StaticGrids = 0;
        [ProtoMember(12)] public int NumberofBlocks = 0;
        [ProtoMember(13)] public float MaxPowerOutput = 0;
        [ProtoMember(14)] public float GridBuiltPercent = 0;
        [ProtoMember(15)] public long JumpDistance = 0;
        [ProtoMember(16)] public int NumberOfGrids = 0;
        [ProtoMember(17)] public int PCU = 0;


        //Server blocklimits Block
        [ProtoMember(18)] public Dictionary<string, int> BlockTypeCount = new Dictionary<string, int>();


        //Grid Stored Materials
        [ProtoMember(19)] public Dictionary<string, double> StoredMaterials = new Dictionary<string, double>();

        [ProtoMember(20)] public byte[] GridDefinition;


    }

    [ProtoContract]
    public class MarketData
    {
        public List<GridsForSale> GridDefinition = new List<GridsForSale>();
        public List<MarketList> List = new List<MarketList>();
    }
}
