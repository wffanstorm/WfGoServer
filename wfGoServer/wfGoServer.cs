using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;

namespace wfGoServer
{
    class wfGoServer
    {
        public List<Room> roomlist;
        public List<wfGoClient> clientlist;

        public bool state;

        public int roomnum;

        private TcpListener tcplistener;
        private Thread ListenThread;
        private Thread HeartCheckThread;
        private Thread BroadcastThread;

        public Form1 frm;

        public wfGoServer(Form1 f)
        {
            roomlist = new List<Room>();
            clientlist = new List<wfGoClient>();

            int port = ConstNumber.serverport;
            tcplistener = new TcpListener(IPAddress.Any, port);

            state = false;

            frm = f;

            roomnum = 0;
        }

        public void Start()
        {
            state = true;
            frm.LblState.BeginInvoke(new Action(() => frm.LblState.Text = "True"));
            StartAllThread();
        }
        public void End()
        {
            state = false;
            frm.LblState.BeginInvoke(new Action(() => frm.LblState.Text = "false"));
            Environment.Exit(0);
        }

        private void Listen()
        {
            tcplistener.Start();
            while (state)
            {
                wfGoClient client = new wfGoClient("null", this, frm);
                client.tcp = tcplistener.AcceptTcpClient();
                client.StartListenThread();
                client.StartHeartSubThread();
                clientlist.Add(client);
                Broadcast b = new Broadcast(roomlist, Convert.ToInt32(frm.LblOnLineNum.Text), frm.TxtInfo.Text);
                client.SendBroadcastMSG(b);
            }
        }

        public void StartListenThread()
        {
            ListenThread = new Thread(Listen);
            ListenThread.Start();
        }

        //每3s检测一遍所有在线玩家的心跳信息
        private void HeartSub()
        {
            while (state)
            {
                try
                {
                    foreach (wfGoClient client in clientlist)
                    {
                        //MessageBox.Show(client.heart.ToString(), "heart");
                        if (client.heart < 0) //离线
                        {
                            //listbox内删除
                            frm.ListBox1.BeginInvoke(new Action(() => frm.ListBox1.Items.Remove(client.player.name)));
                            //在线玩家数-1
                            frm.LblOnLineNum.BeginInvoke(new Action(() =>
                            {
                                int n = Convert.ToInt32(frm.LblOnLineNum.Text);
                                n--;
                                frm.LblOnLineNum.Text = n.ToString();
                            }));
                            //如果在房间内，从房间移除，若房间人数为0，删除房间
                            if(client.player.GetRoom()!=null)
                            {
                                foreach(Room r in roomlist)
                                {
                                    if(r==client.player.GetRoom())
                                    {
                                        r.SubPlayer(client.player);
                                        //检查人数
                                        if(r.playernum==0)
                                        {
                                            roomlist.Remove(r);
                                            UpdateListViewRoom();
                                        }
                                        else
                                        {
                                            r.SendRoomMSG();
                                        }
                                        break;
                                    }
                                }
                            }

                            clientlist.Remove(client);
                            client.Dispose();
                        }
                    }
                }
                catch (Exception ex)
                {

                }

                Thread.Sleep(3000);
            }

        }

        public void StartHeartSubThread()
        {
            HeartCheckThread = new Thread(HeartSub);
            HeartCheckThread.Start();
        }

        private void ShowPlayerNum()
        {
            while (true)
            {
                Thread.Sleep(5000);
                MessageBox.Show(clientlist.Count.ToString(), "playernum");
            }

        }
        public void StartPlayerNumThread()
        {
            Thread t = new Thread(ShowPlayerNum);
            t.Start();
        }

        private void SendBroadcast()
        {
            while (state)
            {
                Broadcast b = new Broadcast(roomlist, Convert.ToInt32(frm.LblOnLineNum.Text), frm.TxtInfo.Text);
                foreach (wfGoClient client in clientlist)
                {
                    client.SendBroadcastMSG(b);
                }
                Thread.Sleep(10000);
            }
        }
        public void StartSendBroadcastThread()
        {
            BroadcastThread = new Thread(SendBroadcast);
            BroadcastThread.Start();
        }

        public void StartAllThread()
        {
            StartHeartSubThread();
            StartListenThread();
            StartSendBroadcastThread();
            //StartPlayerNumThread();

        }

        public void UpdateListViewRoom()
        {
            frm.ListView1.BeginInvoke(new Action(() =>
            {
                frm.ListView1.Items.Clear();
                foreach (Room r in roomlist)
                {
                    ListViewItem item = new ListViewItem(new string[] { r.number.ToString(), r.name, r.state.ToString(), r.playernum.ToString() });
                    frm.ListView1.Items.Add(item);
                }

            }));
        }


    }
}
