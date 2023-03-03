using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels.Http;
using Interfaces;

namespace ClienteConsole
{
    class MsgBox : MarshalByRefObject, IMsgBox
    {

        #region IMsgBox Members

        public void acceptMsg(string userName, string msg)
        {
            //throw new NotImplementedException();
            Console.WriteLine("Msg received from "+userName+":" + msg);
        }

        #endregion
    }


    class Program
    {
        static void Main(string[] args)
        {
            BinaryServerFormatterSinkProvider serverProv = new BinaryServerFormatterSinkProvider();
            //SoapServerFormatterSinkProvider serverProv = new SoapServerFormatterSinkProvider();
            serverProv.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
            BinaryClientFormatterSinkProvider clientProv = new BinaryClientFormatterSinkProvider();
            //SoapClientFormatterSinkProvider clientProv = new SoapClientFormatterSinkProvider();
            IDictionary props = new Hashtable();
            props["port"] = 0;
            TcpChannel ch = new TcpChannel(props, clientProv, serverProv);
            //HttpChannel ch = new HttpChannel(props,clientProv,serverProv);
            ChannelServices.RegisterChannel(ch, false); 
            ICenterDifusion svc = (ICenterDifusion) Activator.GetObject(
                typeof(ICenterDifusion),
                "tcp://localhost:1234/RemoteCenterDifusor");
            
            MsgBox myMsgBox = new MsgBox();
            Console.Write("User Name? ");
            string userName = Console.ReadLine();
            svc.registerMsgBox(userName, myMsgBox);
            
            Console.WriteLine("Escreva mensagens e <end> para terminar");
            for (; ; )
            {
               string msg=Console.ReadLine();
               //string  msg = "Sou o cliente consola";
               //System.Threading.Thread.Sleep(2 * 1000);
                if (string.Compare(msg,"end") == 0) break;
                svc.sendMessage(userName,msg);
            }
            svc.unRegisterMsgBox(userName,myMsgBox);
           

        }
    }
}
