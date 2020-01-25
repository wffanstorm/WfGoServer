using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace wfGoServer
{
    class Room
    {
        public int number;
        public string name;
        public bool state;
        public int playernum
        {
            get { return playerlist.Count; }
        }

        public List<FightPlayer> playerlist;

        private FightBoard board;



        public Room(int number, string name)
        {
            this.number = number;
            this.name = name;
            state = false;
            playerlist = new List<FightPlayer>();
            board = new FightBoard();

        }


        public string ToJson()
        {
            string str = JsonConvert.SerializeObject(this);
            return str;
        }

        public static Room Parse(string jsonstr)
        {
            Room room = JsonConvert.DeserializeObject<Room>(jsonstr);
            return room;
        }

        public void AddPlayer(FightPlayer p)
        {
            p.ComeInRoom(this);
            p.indexinroom = playerlist.Count;
            playerlist.Add(p);
        }
        public void SubPlayer(FightPlayer p)
        {
            p.ComeOutRoom();
            p.indexinroom = -1;
            playerlist.Remove(p);
        }
        public void SendRoomMSG()
        {

            foreach (FightPlayer p in playerlist)
            {
                p.SendRoomMsg();
            }
        }

        public void StartFight()
        {
            state = true;
        }
        public FightBoard GetBoard()
        {
            return board;
        }
        public void SetBoard(FightBoard b)
        {
            board = b;
        }

        public void SendToPeer(int indexinroom,MSG msg)
        {
            
            int targetindex;
            if (indexinroom == 0)
                targetindex = 1;
            else
                targetindex = 0;
            playerlist[targetindex].SendMSG(msg);
        }

    }
}
