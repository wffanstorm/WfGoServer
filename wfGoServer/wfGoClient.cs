using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Windows.Forms;

namespace wfGoServer
{
    class wfGoClient
    {
        public Board board;
        public FightPlayer player;

        public int heart;

        public TcpClient tcp;
        public NetworkStream netstream;
        public StreamReader streamreader;
        public Thread ListenThread;
        public Thread HeartSubThread;

        public Form1 frm;

        public wfGoServer server;

        public wfGoClient(string name, wfGoServer svr, Form1 f)
        {
            board = new FightBoard();
            player = new FightPlayer(name, this);
            tcp = new TcpClient();


            heart = ConstNumber.heart;

            frm = f;
            server = svr;

        }

        public void Connect()
        {
            string ip = ConstNumber.serverIP;
            int port = ConstNumber.serverport;

            tcp.Connect(ip, port);

        }

        public void SendMSG(MSG msg)
        {
            
            //try
            //{
                netstream = tcp.GetStream();
                byte[] data = Encoding.Unicode.GetBytes(msg.ToJson() + "\r\n");
                netstream.Write(data, 0, data.Length);
                if(msg.type== MSG_Type.ApplyFight)
                {
                    //MessageBox.Show("transfer apply fight to " + player.name, "server");
                }
                /*
            }
            catch (Exception ex)
            {
                return;
            }
            */
        }

        public void SendBroadcastMSG(Broadcast b)
        {
            try
            {
                netstream = tcp.GetStream();
                MSG msg = new MSG(MSG_Type.Broadcast, b);
                byte[] data = Encoding.Unicode.GetBytes(msg.ToJson() + "\r\n");
                netstream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                return;
            }

        }

        public void SendApplyFightMSG(int minute)
        {
            try
            {
                netstream = tcp.GetStream();
                MSG msg = new MSG(MSG_Type.ApplyFight, minute);
                byte[] data = Encoding.Unicode.GetBytes(msg.ToJson() + "\r\n");
                netstream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                return;
            }

        }

        public void SendIfAgreeMSG(bool ifagree, string target)
        {
            try
            {
                netstream = tcp.GetStream();
                string param = ifagree.ToString() + "_" + target;
                MSG msg = new MSG(MSG_Type.IfAgree, param);
                byte[] data = Encoding.Unicode.GetBytes(msg.ToJson() + "\r\n");
                netstream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                return;
            }

        }
        public void SendRoomMSG(Room room)
        {
            try
            {
                netstream = tcp.GetStream();
                MSG msg = new MSG(MSG_Type.Room, room);
                byte[] data = Encoding.Unicode.GetBytes(msg.ToJson() + "\r\n");
                netstream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show("err in send room msg: "+ex.Message);
                return;
            }
        }
        public void SendBoardMSG()
        {
            try
            {
                netstream = tcp.GetStream();
                MSG msg = new MSG(MSG_Type.Board, board);
                byte[] data = Encoding.Unicode.GetBytes(msg.ToJson() + "\r\n");
                netstream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                return;
            }

        }
        public void SendRegretMSG()
        {
            try
            {
                netstream = tcp.GetStream();
                MSG msg = new MSG(MSG_Type.Regret, board);
                byte[] data = Encoding.Unicode.GetBytes(msg.ToJson() + "\r\n");
                netstream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                return;
            }

        }
        public void SendSurrenderMSG()
        {
            try
            {
                netstream = tcp.GetStream();
                MSG msg = new MSG(MSG_Type.Surrender, null);
                byte[] data = Encoding.Unicode.GetBytes(msg.ToJson() + "\r\n");
                netstream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                return;
            }

        }
        public void SendFinishMSG()
        {
            try
            {
                netstream = tcp.GetStream();
                MSG msg = new MSG(MSG_Type.Finish, null);
                byte[] data = Encoding.Unicode.GetBytes(msg.ToJson() + "\r\n");
                netstream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                return;
            }

        }


        private void Listen()
        {
            streamreader = new StreamReader(tcp.GetStream(), Encoding.Unicode);
            while (tcp.Connected)
            {
                string jsonstr;
                try
                {
                    jsonstr = streamreader.ReadLine();
                }
                catch (Exception ex)
                {
                    break;
                }
                MSG msg = MSG.Parse(jsonstr);
                //消息处理
                DealMSG(msg);
            }
        }


        private void DealMSG(MSG msg)
        {
            if (msg == null)//中断连接
            {
                tcp.Close();
                return;
            }
            

            switch (msg.type)
            {
                case MSG_Type.Name:
                    DealNameMSG(msg);
                    break;
                case MSG_Type.Heartbeat:
                    DealHeartBeatMSG();
                    break;
                case MSG_Type.CreatRoom:
                    DealCreatRoomMSG(msg);
                    break;
                case MSG_Type.ComeInRoom:
                    DealComeInRoomMSG(msg);
                    break;
                case MSG_Type.QuitRoom:
                    DealQuitRoomMSG();
                    break;
                case MSG_Type.ApplyFight:
                    DealApplyFightMSG(msg);
                    break;
                case MSG_Type.Board:
                    DealBoardMSG(msg);
                    break;
                case MSG_Type.Regret:
                    DealRegretMSG(msg);
                    break;
                case MSG_Type.Finish:
                    DealFinishMSG(msg);
                    break;
                case MSG_Type.Surrender:
                    DealSurrenderMSG(msg);
                    break;
                case MSG_Type.IfAgree:
                    DealIfAgreeMSG(msg);
                    break;


            }
        }

        private void DealNameMSG(MSG msg)
        {
            player.name = (string)msg.parameter;
            frm.ListBox1.BeginInvoke(new Action(() => frm.ListBox1.Items.Add(player.name)));
            frm.LblOnLineNum.BeginInvoke(new Action(() =>
            {
                int n = Convert.ToInt32(frm.LblOnLineNum.Text);
                n++;
                frm.LblOnLineNum.Text = n.ToString();
            }));
        }
        private void DealHeartBeatMSG()
        {
            heart++;
        }
        private void DealCreatRoomMSG(MSG msg)
        {
            string roomname = (string)msg.parameter;
            //检测重名房间：
            foreach (Room rm in server.roomlist)
            {
                if (rm.name == roomname)
                {
                    //房间创建失败
                    SendIfAgreeMSG(false, "CreatRoom");
                    return;
                }
            }
            //无重名房间，可创建
            server.roomnum++;
            Room r = new Room(server.roomnum, roomname);
            r.AddPlayer(this.player);
            server.roomlist.Add(r);

            Broadcast b = new Broadcast(server.roomlist, Convert.ToInt32(frm.LblOnLineNum.Text), server.frm.TxtInfo.Text);
            SendBroadcastMSG(b);
            SendIfAgreeMSG(true, "CreatRoom_" + r.number.ToString());
            Thread.Sleep(1000);
            SendRoomMSG(r);
            server.UpdateListViewRoom();
        }
        private void DealComeInRoomMSG(MSG msg)
        {
            int roomnumber = Convert.ToInt32((string)msg.parameter);
            //检查该房间是否满人（2人）
            foreach (Room r in server.roomlist)
            {
                if (r.number == roomnumber)
                {
                    if (r.playernum >= 2)
                    {
                        SendIfAgreeMSG(false, "ComeInRoom");
                        return;
                    }
                    else//进入房间
                    {
                        r.AddPlayer(this.player);
                        SendIfAgreeMSG(true, "ComeInRoom");
                        Thread.Sleep(1000);
                        r.SendRoomMSG();

                        server.UpdateListViewRoom();
                        return;
                    }

                }
            }
        }
        private void DealQuitRoomMSG()
        {
            foreach(Room r in server.roomlist)
            {
                if(r==player.GetRoom())
                {
                    r.SubPlayer(player);
                    //检查人数
                    if (r.playernum == 0)
                    {
                        server.roomlist.Remove(r);
                    }
                    else
                    {
                        r.SendRoomMSG();
                    }
                    server.UpdateListViewRoom();
                    return;
                }
            }
        }

        private void DealApplyFightMSG(MSG msg)
        {
            //MessageBox.Show("rcv apply fight", "server");
            player.GetRoom().SendToPeer(player.indexinroom, msg);
        }
        private void DealRegretMSG(MSG msg)
        {
            player.GetRoom().SendToPeer(player.indexinroom, msg);
        }
        private void DealFinishMSG(MSG msg)
        {
            player.GetRoom().SendToPeer(player.indexinroom, msg);
        }
        private void DealSurrenderMSG(MSG msg)
        {
            player.GetRoom().SendToPeer(player.indexinroom, msg);
            player.GetRoom().state = false;
        }
        private void DealBoardMSG(MSG msg)
        {
            player.GetRoom().SendToPeer(player.indexinroom, msg);
        }
        private void DealIfAgreeMSG(MSG msg)
        {
            player.GetRoom().SendToPeer(player.indexinroom, msg);
            string para = (string)msg.parameter;
            string[] strs = para.Split('_');
            if(strs[0]=="True")
            {
                if(strs[1]=="Finish")
                {
                    player.GetRoom().state = false;
                }
                if(strs[1]=="ApplyFight")
                {
                    player.GetRoom().state = true;
                }
            }
        }



        public void StartListenThread()
        {
            ListenThread = new Thread(Listen);
            ListenThread.Start();
        }


        public void Dispose()
        {
            if (ListenThread != null && ListenThread.IsAlive)
            {
                ListenThread.Abort();
            }

        }

        private void HeartSub()
        {
            while (heart >= 0)
            {
                heart--;
                Thread.Sleep(3000);
            }
        }
        public void StartHeartSubThread()
        {
            HeartSubThread = new Thread(HeartSub);
            HeartSubThread.Start();
        }


    }
}
