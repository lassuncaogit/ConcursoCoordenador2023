using System;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Lifetime;
using Interfaces;

namespace Server
{
    class CenterDifusor : MarshalByRefObject, ICenterDifusion
    {
        private object lck = new object();
        //private List<IMsgBox> tab = new List<IMsgBox>();
        private Dictionary<string, IMsgBox> tab = new Dictionary<string, IMsgBox>();

        public CenterDifusor()
        {
            //Console.WriteLine("ctor Server");

        }
        #region ICenterDifusion Members

        public void registerMsgBox(string uid,IMsgBox mbox)
        {
            ILease lease=(ILease)this.GetLifetimeService();
            Console.WriteLine("TTL:"+lease.CurrentLeaseTime);
            lock (lck)
            {
                tab.Add(uid,mbox);
            }
        }

        public void unRegisterMsgBox(string uid, IMsgBox mbox)
        {
            lock (lck)
            {
                tab.Remove(uid);
            }
        }
        public int sendMessage(string userName, string msgText)
        //public int sendMessage(ChatMessage msg)
        {
            ChatMessage msg = new ChatMessage();
            msg.Uid = userName; msg.Msg = msgText;
            //System.Threading.Thread.Sleep(2 * 1000);
            ILease lease = (ILease)this.GetLifetimeService();
            if (lease != null)
                Console.WriteLine("TTL:" + lease.CurrentLeaseTime);

            Console.WriteLine("Message received from " + msg.Uid + " : " + msg.Msg);
             lock (lck)
             {
                foreach (string key in tab.Keys)
                try
                {
                        Console.WriteLine("Message sent to " + msg.Uid + " : " + key);
                        tab[key].acceptMsg(msg.Uid, msg.Msg);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
             }
             return msg.Msg.Length;
        }

        #endregion
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Inicio do Server");
            BinaryServerFormatterSinkProvider serverProv = new BinaryServerFormatterSinkProvider();
            //SoapServerFormatterSinkProvider serverProv = new SoapServerFormatterSinkProvider();
            serverProv.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
            BinaryClientFormatterSinkProvider clientProv = new BinaryClientFormatterSinkProvider();
            //SoapClientFormatterSinkProvider clientProv = new SoapClientFormatterSinkProvider();
            IDictionary props = new Hashtable();
            props["port"] = 1234;
            //HttpChannel ch = new HttpChannel(props, clientProv, serverProv);
            TcpChannel ch = new TcpChannel(props, clientProv, serverProv);
            ChannelServices.RegisterChannel(ch, false);
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(CenterDifusor),
                "RemoteCenterDifusor",
            WellKnownObjectMode.Singleton); // pedidos servidos pelo mesmo objecto
            Console.WriteLine("Espera pedidos");
            Console.ReadLine();
        }
    }
}
