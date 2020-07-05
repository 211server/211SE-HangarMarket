using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces.Terminal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Utils;

namespace HangarStoreMod
{
    public class TerminalControls
    {

        public static bool controlsCreated = false;



        public static void CreateControls(IMyTerminalBlock block, List<IMyTerminalControl> controls)
        {
            if (block as IMyProjector == null || controlsCreated == true)
            {
                return;
            }

            controlsCreated = true;

            var controlList = new List<IMyTerminalControl>();
            MyAPIGateway.TerminalControls.GetControls<IMyProjector>(out controlList);
            controlList[9].Visible = Block => HideControls(Block);
            controlList[10].Visible = Block => HideControls(Block);
            controlList[11].Visible = Block => HideControls(Block);
            controlList[8].Visible = Block => HideControls(Block);

            controlList[29].Visible = Block => HideControls(Block);
            controlList[30].Visible = Block => HideControls(Block);
            controlList[31].Visible = Block => HideControls(Block);

            //controlList[28].Visible = Block => HideControls(Block);

            controlList[27].Visible = Block => HideControls(Block);
            controlList[26].Visible = Block => HideControls(Block);



            var SaleList = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlListbox, IMyProjector>("HangerStoreNext");
            SaleList.Title = MyStringId.GetOrCompute("Grids for sale: ");
            SaleList.SupportsMultipleBlocks = false;
            SaleList.VisibleRowsCount = 12;
            SaleList.Multiselect = false;
           
            SaleList.ListContent = ModCore.GetForSaleGrids;
            SaleList.ItemSelected = ModCore.SetSelectedGrid;
            SaleList.Enabled = Block => true;
            SaleList.Visible = Block => ControlVisibility(Block);
            MyAPIGateway.TerminalControls.AddControl<IMyProjector>(SaleList);
            controls.Add(SaleList);


            var PreviewButton = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlButton, IMyProjector>("PreviewButton");
            PreviewButton.Title = MyStringId.GetOrCompute("Preview Selected Grid");
            PreviewButton.Tooltip = MyStringId.GetOrCompute("Previews the selected Grid");
            PreviewButton.SupportsMultipleBlocks = false;
            PreviewButton.Action = Block => ModCore.PreviewSelectedGrid(Block);
            PreviewButton.Enabled = Block => ModCore.PreviewButtonVisibility(block);
            PreviewButton.Visible = Block => ControlVisibility(Block);
            
            MyAPIGateway.TerminalControls.AddControl<IMyProjector>(PreviewButton);
            controls.Add(PreviewButton);



            var DetailsButton = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlButton, IMyProjector>("Ship Details");
            DetailsButton.Title = MyStringId.GetOrCompute("Selected Grid Details");
            DetailsButton.Tooltip = MyStringId.GetOrCompute("Provides indepth ship details");
            DetailsButton.SupportsMultipleBlocks = false;
            DetailsButton.Action = Block => ModCore.ShipDetails(block);
            DetailsButton.Enabled = Block => ModCore.PreviewButtonVisibility(block);
            DetailsButton.Visible = Block => ControlVisibility(Block);

            MyAPIGateway.TerminalControls.AddControl<IMyProjector>(DetailsButton);
            controls.Add(DetailsButton);



            var PurchaseButton = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlButton, IMyProjector>("PurchaseButton");
            PurchaseButton.Title = MyStringId.GetOrCompute("Purchase Slected Grid");
            PurchaseButton.Tooltip = MyStringId.GetOrCompute("Purchases the selected grid");
            PurchaseButton.SupportsMultipleBlocks = false;

            PurchaseButton.Action = Block => ModCore.PurchaseSlectedGrid(Block);

            PurchaseButton.Enabled = Block => ModCore.PurchaseButtonVisibility(Block);
            PurchaseButton.Visible = Block => ControlVisibility(Block);
            MyAPIGateway.TerminalControls.AddControl<IMyProjector>(PurchaseButton);
            controls.Add(PurchaseButton);


        }


        public static bool HideControls(IMyTerminalBlock block)
        {
            if (block as IMyProjector != null)
            {
                var radar = block as IMyProjector;
                if (radar.BlockDefinition.SubtypeName.Contains("HangarStoreBlock"))
                {

                    return false;
                }
            }

            return true;
        }


        public static bool ControlVisibility(IMyTerminalBlock block)
        {

            if (block as IMyProjector != null)
            {

                var Hangar = block as IMyProjector;
                if (Hangar.BlockDefinition.SubtypeName.Contains("HangarStoreBlock"))
                {
                    return true;
                }

            }

            return false;
        }

        


        public static void ListControl(List<VRage.ModAPI.MyTerminalControlListBoxItem> A, List<VRage.ModAPI.MyTerminalControlListBoxItem> B)
        {

        }
    }
}
