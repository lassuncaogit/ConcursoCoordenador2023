using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interfaces
{

    [Serializable]
    public class ChatMessage {
        private string uid;
        private string msg;

        public string Uid
        {
            get
            {
                return uid;
            }

            set
            {
                uid = value;
            }
        }

        public string Msg
        {
            get
            {
                return msg;
            }

            set
            {
                msg = value;
            }
        }
    }


    public interface IMsgBox
    {
        void acceptMsg(string userName, string msg);
    }

    public interface ICenterDifusion
    {
        void registerMsgBox(string uid, IMsgBox mbox);
        void unRegisterMsgBox(string uid, IMsgBox mbox);
        int sendMessage(string userName,string msg);
        //int sendMessage(ChatMessage msg);
    }


}
