using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wfGoServer
{
    class FightPlayer:Player
    {
        public string name;

        private Room room;

        public int indexinroom;

        private wfGoClient client;


        public FightPlayer(string Name,wfGoClient c)
        {
            name = Name;
            room = null;
            client = c;
            indexinroom = -1;
        }

        public void Regret(Board board)
        {
            board.Regret();
            board.Regret();
        }

        public void SendRoomMsg()
        {
            client.SendRoomMSG(room);
        }


        public void ComeInRoom(Room r)
        {
            room = r;
        }
        public void ComeOutRoom()
        {
            room = null;
        }

        public Room GetRoom()
        {
            return room;
        }
        public wfGoClient GetClient()
        {
            return client;
        }

        public void SendMSG(MSG msg)
        {
            client.SendMSG(msg);
        }

    }
}
