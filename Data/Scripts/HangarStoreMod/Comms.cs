using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Timers;
using Sandbox.ModAPI;
using VRage.Game;
using VRage.Game.ModAPI;
using VRage.Utils;
using VRageMath;
using System.Linq;
using Sandbox.ModAPI.Interfaces.Terminal;
using VRage.Game.Components;

namespace HangarStoreMod
{
    [MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
    public class Comms : MySessionComponentBase
    {
        private bool _init;
        public const ushort NETWORK_ID = 2934;

        public override void UpdateAfterSimulation()
        {
            
            if (MyAPIGateway.Session != null)
                if (!_init)
                {
                    _init = true;

                    MyAPIGateway.Multiplayer.RegisterMessageHandler(NETWORK_ID, MessageHandler);
                    Debug.Write("Creating message handler!");

                    //Need to request all data!

                    Message Message = new Message();
                    Message.Type = MessageType.RequestAllItems;

                    Comms.SendMessageToServer(Message);

                }
        }


        protected override void UnloadData()
        {
            try
            {
                MyAPIGateway.Multiplayer.UnregisterMessageHandler(NETWORK_ID, MessageHandler);
            }
            catch (Exception a)
            {
                //MyLog.Default.WriteLineAndConsole("Cannot remove event Handlers! Are they already removed?1" + a);
            }
        }

        private static void MessageHandler(byte[] bytes)
        {
            
            try
            {

                Debug.Write("Message Recieved!");

                Message RecievedMessage = MyAPIGateway.Utilities.SerializeFromBinary<Message>(bytes);
                //MyLog.Default.WriteLineAndConsole("Recieved Message from server: "+ RecievedMessage.Type.ToString());

                if (RecievedMessage.Type == MessageType.RequestAllItems)
                {
                    //ModCore.MarketGrids.Clear();

                    ModCore.MarketGrids = RecievedMessage.MarketBoxItmes;


                    //Do we need to update GUI?

                    //ModCore.UpdateGUI(ModCore.m_block);

                }else if (RecievedMessage.Type == MessageType.AddOne)
                {
                    ModCore.MarketGrids.Add(RecievedMessage.MarketBoxItmes[0]);

                }else if(RecievedMessage.Type == MessageType.RemoveOne)
                {

                    //MyLog.Default.WriteLineAndConsole("MessageType: RemoveOne");
                    MarketList list = ModCore.MarketGrids.First(x => x.Name == RecievedMessage.MarketBoxItmes[0].Name);

                    for(int i = 0; i < ModCore.MarketGrids.Count; i++)
                    {
                        if(ModCore.MarketGrids[i].Name == RecievedMessage.MarketBoxItmes[0].Name)
                        {
                            ModCore.MarketGrids.RemoveAt(i);
                            ModCore.RemovedMarketGrids = RecievedMessage.MarketBoxItmes[0];
                            MyLog.Default.WriteLineAndConsole("MessageType: RemovedOne "+ ModCore.RemovedMarketGrids.Name);
                            break;
                        }
                    }

                    try
                    {
                       ModCore.m_block.RefreshCustomInfo();
                       ModCore.UpdateGUI(ModCore.m_block);
                    }
                    catch
                    {
                        //Experiement to see if this 

                    }

                    //Remove frome list
                }
                else {
                    //Do nothing... 
                }

                //Update GUI
                //ModCore.UpdateGUI();



                //MyLog.Default.WriteLineAndConsole("Message Recieved");


                //Array.Copy(bytes, 1, data, 0, data.Length);
            }
            catch (Exception ex)
            {
                //MyLog.Default.WriteLineAndConsole($"WTF is this shit " + ex);
                
            }
        }

        public static void SendMessageToServer(Message message)
        {
            //MyLog.Default.WriteLineAndConsole("Sending request to server to update market list");
            byte[] barr = MyAPIGateway.Utilities.SerializeToBinary(message);


            MyAPIGateway.Multiplayer.SendMessageToServer(NETWORK_ID, barr);
        }
    }





}
