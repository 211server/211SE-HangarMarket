
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Game.Entities;
using Sandbox.Game.EntityComponents;
using Sandbox.Game.Gui;
using Sandbox.Game.World;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces.Terminal;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Utils;
using VRageMath;


namespace HangarStoreMod
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_Projector), true, "HangarStoreBlock")]
    public class ModCore : MyGameLogicComponent
    {
        //public Market Market;

        public static IMyTerminalBlock m_block = null;
        IMyProjector m_projector = null;

        public static bool PurchaseGridOption = false;
        public static bool ButtonPress = false;

        private bool _init = false;
        public static List<MarketList> MarketGrids = new List<MarketList>();
        public static MarketList RemovedMarketGrids;

        public static MarketList SelectedGrid = null;
        public static int TerminalCount;

        public static bool RefreshOnce = false;
        internal MyCubeBlock Cubeblock { get; set; }
        public bool HandlerOn { get; private set; }

        public static MyTerminalControlListBoxItem SelectedBoxItem;
        public static int SelectedItemNum = 0;



        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            //On block initilization, send request to server to get the entire market list



            // MyLog.Default.WriteLineAndConsole("Hello World!");


            m_block = Entity as IMyTerminalBlock;
            m_projector = m_block as IMyProjector;

            Cubeblock = m_projector as MyCubeBlock;

            var blockDef = m_projector.GetObjectBuilderCubeBlock() as MyObjectBuilder_Projector;

            // MyLog.Default.WriteLineAndConsole("Block: "+ m_projector.DisplayNameText);



            NeedsUpdate |= MyEntityUpdateEnum.BEFORE_NEXT_FRAME | MyEntityUpdateEnum.EACH_FRAME | MyEntityUpdateEnum.EACH_100TH_FRAME;
            /*
            var JumpButton = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlButton, IMyProjector>("HangerStoreNext");
            JumpButton.Title = MyStringId.GetOrCompute("Next grid");
            JumpButton.Tooltip = MyStringId.GetOrCompute("Iterates through list");
            
            JumpButton.Enabled = block => true;
            JumpButton.Visible = block => true;

            List<IMyTerminalControl> controls = new List<IMyTerminalControl>();
            MyAPIGateway.TerminalControls.GetControls<IMyProjector>(out controls);




            MyAPIGateway.TerminalControls.AddControl<IMyProjector>(JumpButton);


            

            //TerminalControls.AddHangarOptions<IMyProjector>();
            */

        }



        public override void Close()
        {
            if (_init)
            {

                //only client pls
                if (!MyAPIGateway.Multiplayer.IsServer)
                {
                    m_projector.AppendingCustomInfo -= OnAppendingCustomInfo;
                }

                _init = false;

                //MyLog.Default.WriteLineAndConsole("Closing Event Handlers for Hangar Market");
            }
            base.Close();
        }



        internal static bool PurchaseButtonVisibility(IMyTerminalBlock block)
        {
            IMyPlayer Buyer = MyAPIGateway.Session.LocalHumanPlayer;

            //Convert string. Apprently you cant use long out??? WTF keen. Making my job 10x as hard
            string myString = Buyer.GetBalanceShortString();
            myString = myString.Remove(myString.Length - 3, 3);
            myString = myString.Replace(",", "");
            long numVal = Int64.Parse(myString);

            if (SelectedGrid != null)
            {
                //Only enable the button if the selected item is not being sold by the buyer.
                if (SelectedGrid.Steamid == MyAPIGateway.Session.LocalHumanPlayer.SteamUserId)
                {
                    return false;
                }
                else
                {
                    if (numVal >= SelectedGrid.Price)
                    {

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

        }

        internal static bool PreviewButtonVisibility(IMyTerminalBlock block)
        {
            //IMyPlayer Buyer = MyAPIGateway.Session.LocalHumanPlayer;

            //Convert string. Apprently you cant use long out??? WTF keen. Making my job 10x as hard


            if (SelectedGrid != null)
            {
                //Only enable the button if the selected item is not being sold by the buyer.
                return true;
            }
            else
            {
                return false;
            }

        }

        private void OnAppendingCustomInfo(IMyTerminalBlock arg1, StringBuilder arg2)
        {
            //MyLog.Default.WriteLineAndConsole("Custom Info!");
            if (SelectedGrid != null)
            {
                //arg2.AppendLine("You can preview and buy the selected grid in the controls!");
                //string FactionTag;
                try
                {

                    var fc = MyAPIGateway.Session.Factions.GetObjectBuilder();


                    //MyPlayer player = MySession.Static.Players.GetPlayerByName(SelectedGrid.Seller);




                }
                catch (Exception e)
                {
                    //FactionTag = "N/A";
                }

                arg2.AppendLine("_______•°•(飞船市场)•°•_______");
                arg2.AppendLine("网格名称: " + SelectedGrid.Name);
                arg2.AppendLine();
                arg2.AppendLine("卖家: " + SelectedGrid.Seller);
                arg2.AppendLine("卖家阵营: " + SelectedGrid.SellerFaction);
                arg2.AppendLine("卖家定价: " + SelectedGrid.Price + " [sc]");
                arg2.AppendLine("估算价值: " + SelectedGrid.MarketValue + " [sc]");
                arg2.AppendLine();
                arg2.AppendLine("•°•°•°•°•°•°•°•°•°•°•°•°•°•°•°•°•°•°•°");
                arg2.AppendLine("描述: " + SelectedGrid.Description);
            }
            else
            {
                arg2.AppendLine("");

            }

        }


        public override void UpdateBeforeSimulation()
        {


            if (MyAPIGateway.Multiplayer.IsServer)//only client pls
                return;

            if (MyAPIGateway.Session == null)
                return;



            if (!_init)
            {
                //only client pls
                MyAPIGateway.TerminalControls.CustomControlGetter += CreateControlsNew;
                m_projector.AppendingCustomInfo += OnAppendingCustomInfo;

                //Request all data to be sent to the block!


                _init = true;

            }
            //MyLog.Default.WriteLineAndConsole("Hello World!");
        }


        public override void UpdateAfterSimulation10()
        {

            //Will update transparency of all grids
            var projectionEnt = m_projector.ProjectedGrid as IMyEntity;
            //var projectionGrid = Session.Instance.consoleBlocks[index].ProjectedGrid as IMyCubeGrid;
            if (projectionEnt == null) return;
            var transparency = .25f;



            if (projectionEnt.Render.Transparency != transparency)
            {
                //MyLog.Default.WriteLineAndConsole("Updating Projection transparency!");
                SetTransparency(projectionEnt, transparency);
            }
        }

        public static void CreateControlsNew(IMyTerminalBlock block, List<IMyTerminalControl> controls)
        {

            if (block as IMyProjector != null)
            {
                bool A;
                controls[0].Visible = B => A = B.Visible ;

                TerminalControls.CreateControls(block, controls);
            }
        }


        public static void GetForSaleGrids(IMyTerminalBlock block, List<MyTerminalControlListBoxItem> listItems, List<MyTerminalControlListBoxItem> selectedItems)
        {
            //We need to completley redo ALL this.

            try
            {
                foreach (MarketList grid in MarketGrids)
                {
                    //MyLog.Default.WriteLineAndConsole("Grid "+grid.Name+" is in the marked list grid out of "+MarketGrids.Count+" total grids");
                    if (!listItems.Any(x => x.Text == MyStringId.GetOrCompute(grid.Name)))
                    {
                        var dummy2 = new MyTerminalControlListBoxItem(MyStringId.GetOrCompute(grid.Name), MyStringId.GetOrCompute("PCU: " + grid.PCU), grid);
                        MyLog.Default.WriteLineAndConsole("Adding grid " + grid.Name + " to the block!");
                        listItems.Add(dummy2);
                    }

                }

                
                if (RemovedMarketGrids != null)
                {
                    try
                    {
                        var item = listItems.First(x => x.Text.String == RemovedMarketGrids.Name);
                        listItems.Remove(item);
                        MyLog.Default.WriteLineAndConsole("Removing grid from the block!");
                    }
                    catch
                    {
                        //Do freaking nothing. ffs
                    }



                    if (selectedItems.Count != 0 && selectedItems[0].Text.String == RemovedMarketGrids.Name)
                    {
                        selectedItems.Clear();
                    }




                    //Check to see if the selected item was our purchased item


                    RemovedMarketGrids = null;
                }



                //Reupdate selected items
                if (listItems.Count == 1)
                {
                    selectedItems.Clear();
                    selectedItems.Add(listItems[0]);
                    SelectedGrid = (MarketList)listItems[0].UserData;

                    MyTerminalControlListBoxItem item = listItems[0];
                    SelectedBoxItem = item;

                    block.RefreshCustomInfo();
                }
                else if (listItems.Count == 0)
                {
                    selectedItems.Clear();
                    block.RefreshCustomInfo();
                }
                else
                {
                    selectedItems.Clear();

                    if (listItems.Count == 0)
                        return;

                    selectedItems.Add(listItems.FirstOrDefault(x => x.Text == SelectedBoxItem.Text));
                }

                //var toList = new MyTerminalControlListBoxItem(MyStringId.GetOrCompute(grid.CustomName), MyStringId.GetOrCompute(grid.CustomName), objectText);


                //Can you do this?? Check to see if your saved selected item is in the new list?

                //Clear the selected grid
                //SelectedGrid = null;
                //block.RefreshCustomInfo();
            }
            catch (Exception e)
            {
                //Temp Debug
                MyLog.Default.WriteLineAndConsole("Exception Thrown! \n" + e);
            }

            //UpdateGUI(block);

        }

        public static void UpdateGUI(IMyTerminalBlock block)
        {

            if (MyAPIGateway.Gui.GetCurrentScreen == MyTerminalPageEnum.ControlPanel)
            {
                var myCubeBlock = block as MyCubeBlock;

                if (myCubeBlock.IDModule != null)
                {

                    var share = myCubeBlock.IDModule.ShareMode;
                    var owner = myCubeBlock.IDModule.Owner;
                    myCubeBlock.ChangeOwner(owner, share == MyOwnershipShareModeEnum.None ? MyOwnershipShareModeEnum.Faction : MyOwnershipShareModeEnum.None);
                    myCubeBlock.ChangeOwner(owner, share);
                }
            }


        }

        public static void SetSelectedGrid(IMyTerminalBlock block, List<MyTerminalControlListBoxItem> listItems)
        {

            //MyLog.Default.WriteLineAndConsole("Selected Item Changed to: "+listItems[0].Text);
            SelectedGrid = (MarketList)listItems[0].UserData;

            MyTerminalControlListBoxItem item = listItems[0];
            SelectedBoxItem = item;

            block.RefreshCustomInfo();
            UpdateGUI(block);
        }

        public static void UpdateTerminalClient(IMyTerminalBlock block)
        {
            MyOwnershipShareModeEnum shareMode;
            long ownerId;

            var myCubeBlock = block as MyCubeBlock;

            //MyLog.Default.WriteLineAndConsole("Refreshing block!");
            if (block == null)
            {
                MyLog.Default.WriteLineAndConsole("Block is null!");
            }

            if (myCubeBlock.IDModule != null)
            {
                ownerId = myCubeBlock.IDModule.Owner;
                shareMode = myCubeBlock.IDModule.ShareMode;
            }
            else return;
            MyLog.Default.WriteLineAndConsole("Changing block! owner");

            myCubeBlock.ChangeOwner(ownerId, shareMode == MyOwnershipShareModeEnum.None ? MyOwnershipShareModeEnum.Faction : MyOwnershipShareModeEnum.None);
            myCubeBlock.ChangeOwner(ownerId, shareMode);


        }


        public static void PreviewSelectedGrid(IMyTerminalBlock block)
        {
            m_block = block as IMyProjector;
            IMyProjector a = block as IMyProjector;



            a.SetProjectedGrid(null);

            if (SelectedGrid != null)
            {


                if (SelectedGrid.GridDefinition == null)
                {
                    MyLog.Default.WriteLineAndConsole("Grid needs to be re-sent to market! Ask admins to delete the market.json!");
                    return;
                }

                MyObjectBuilder_CubeGrid CubeGrid = MyAPIGateway.Utilities.SerializeFromBinary<MyObjectBuilder_CubeGrid>(SelectedGrid.GridDefinition);
                if (CubeGrid == null)
                {
                    MyLog.Default.WriteLineAndConsole("No Blueprints In file");

                    //No Blueprints in file!
                    return;
                }


                a.SetProjectedGrid(CubeGrid);

            }
            else
            {
                MyLog.Default.WriteLineAndConsole("Unable to preview grid");
            }
        }

        public static void PurchaseSlectedGrid(IMyTerminalBlock block)
        {
            if (SelectedGrid != null)
            {

                MyLog.Default.WriteLineAndConsole("Purchase selected grid!");
                Message message = new Message();
                GridsForSale grid = new GridsForSale();
                grid.SellerSteamid = SelectedGrid.Steamid;
                grid.name = SelectedGrid.Name;
                grid.BuyerSteamid = MyAPIGateway.Session.LocalHumanPlayer.SteamUserId;


                if (SelectedGrid.Steamid == grid.BuyerSteamid)
                {
                    return;
                }


                MyLog.Default.WriteLineAndConsole("BuyerID: " + grid.BuyerSteamid);


                message.GridDefinitions = grid;
                message.Type = MessageType.PurchasedGrid;

                //Check players price etc. if they dont have enough dont let them buy. Perhaps grey out button?


                Comms.SendMessageToServer(message);


                //remove item from list or force update etc.

                return;
            }


        }

        public static void ShipDetails(IMyTerminalBlock block)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Seller Description: " + SelectedGrid.Description);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine();

            stringBuilder.AppendLine("__________•°•[飞船属性]•°•__________");
            stringBuilder.AppendLine("网格质量: " + SelectedGrid.GridMass + "kg");
            stringBuilder.AppendLine("小网格数量: " + SelectedGrid.SmallGrids);
            stringBuilder.AppendLine("大网格数量: " + SelectedGrid.LargeGrids);
            stringBuilder.AppendLine("最大输出功率: " + SelectedGrid.MaxPowerOutput);
            stringBuilder.AppendLine("建造百分比: " + Math.Round(SelectedGrid.GridBuiltPercent * 100, 2) + "%");
            stringBuilder.AppendLine("最大跃迁距离: " + SelectedGrid.JumpDistance);
            stringBuilder.AppendLine("飞船 PCU: " + SelectedGrid.PCU);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine();

            stringBuilder.AppendLine("__________•°•[方块数量]•°•__________");
            foreach (KeyValuePair<string, int> pair in SelectedGrid.BlockTypeCount)
            {
                stringBuilder.AppendLine(pair.Key + ": " + pair.Value);
            }


            MyAPIGateway.Utilities.ShowMissionScreen(SelectedGrid.Name, "Ship Details", null, stringBuilder.ToString(), null);

            //ModCommunication.SendMessageTo(new DialogMessage("NPC-Takeover", $"All control blocks captured!", "Transfering authorship on " + Cubegrid.DisplayName + " to you! " + sb), UserSteamID);
        }


        private static void SetTransparency(IMyEntity myEntity, float transparency)
        {
            var ren = myEntity.Render;
            if (ren == null) return;
            ren.Transparency = transparency;
            ren.CastShadows = false;
            ren.UpdateTransparency();

            IMyCubeGrid cubeGrid = (IMyCubeGrid)myEntity;
            if (cubeGrid == null) return;

            List<IMySlimBlock> slimList = new List<IMySlimBlock>();
            cubeGrid.GetBlocks(slimList);

            foreach (var block in slimList)
            {
                if (block == null) continue;
                block.Dithering = transparency;
                block.UpdateVisual();

                IMyCubeBlock fatBlock = block.FatBlock;
                if (fatBlock != null)
                {
                    fatBlock.Render.CastShadows = false;
                    SetTransparencyForSubparts(fatBlock, transparency);
                }
            }

            ren.UpdateTransparency();
        }


        private static void SetTransparencyForSubparts(IMyCubeBlock renderEntity, float transparency)
        {
            if (renderEntity == null) return;
            var cubeBlock = renderEntity as MyEntity;
            cubeBlock.Render.CastShadows = false;
            if (cubeBlock.Subparts == null)
            {
                return;
            }
            foreach (KeyValuePair<string, MyEntitySubpart> current in cubeBlock.Subparts)
            {
                current.Value.Render.Transparency = 0f;
                current.Value.Render.CastShadows = false;
                //current.Value.Render.RemoveRenderObjects();
                //current.Value.Render.AddRenderObjects();
                current.Value.Render.UpdateTransparency();


                var subpartBlock = current.Value as MyEntity;
                if (subpartBlock == null) continue;

                if (subpartBlock.Subparts != null)
                {
                    // SetTransparencyForSubparts(current.Value as IMyCubeBlock, transparency);
                    foreach (KeyValuePair<string, MyEntitySubpart> current2 in subpartBlock.Subparts)
                    {
                        current2.Value.Render.Transparency = 0f;
                        current2.Value.Render.CastShadows = false;
                        current2.Value.Render.UpdateTransparency();

                    }
                }

            }
        }



    }




}
