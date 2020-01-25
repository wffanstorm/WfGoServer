using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace wfGoServer
{
    enum MSG_Type { QuitRoom,Room,Broadcast,Name, CreatRoom, ComeInRoom, ApplyFight, IfAgree, Board, Surrender, Finish, Regret, RoomList, Heartbeat }
    //param state: Room,Broadcast,name,roomname , roomname, minutes, target_bool, ftboard,   null,    null ,  null, list<room>,  null
    class MSG
    {
        public MSG_Type type;
        public object parameter;

        public MSG(MSG_Type t, object param)
        {
            type = t;
            parameter = param;
        }

        public string ToJson()
        {
            string str = JsonConvert.SerializeObject(this);
            return str;
        }

        public static MSG Parse(string jsonstr)
        {
            MSG msg;
            try
            {
                msg = JsonConvert.DeserializeObject<MSG>(jsonstr);
            }
            catch (ArgumentNullException)
            {
                return null;
            }

            return msg;
        }

    }
}
