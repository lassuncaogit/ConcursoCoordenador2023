using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels.Http;
using Interfaces;
using System.Runtime.Remoting.Messaging;


namespace ClienteWinForms
{

    
    public partial class Form1 : Form
    {
        //HttpChannel ch;
        ICenterDifusion svc;
        MsgBox myMsgBox;

        public Form1()
        {
            InitializeComponent();

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
            svc = (ICenterDifusion)Activator.GetObject(
                typeof(ICenterDifusion),
                "tcp://localhost:1234/RemoteCenterDifusor");

            myMsgBox = new MsgBox(this);

        }

        int nreq = 0;
        delegate int DelSend(string userName, string msg);
        private void button1_Click(object sender, EventArgs e)
        {
            //svc.sendMessage(textBox2.Text, textBox1.Text);
            DelSend del = new DelSend(svc.sendMessage);
            AsyncCallback callback = new AsyncCallback(SendCompleted);
            del.BeginInvoke(textBox2.Text, textBox1.Text, callback, ++nreq);

        }
        private void SendCompleted(IAsyncResult ar)
        {
            AsyncResult ar2 = (AsyncResult)ar;
            int nreq=(int)ar2.AsyncState;
            DelSend del = (DelSend)ar2.AsyncDelegate;
            int size=del.EndInvoke(ar2);
            ActualizaEndInvoke("pedido n. " + nreq + "com size" + size.ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            svc.registerMsgBox(textBox2.Text, myMsgBox);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            svc.unRegisterMsgBox(textBox2.Text,myMsgBox);
        }

        private delegate void AddTextDel(string txt);
        public void msgReceived(string txt)
        {
             if (this.InvokeRequired)
             {
                AddTextDel del = new AddTextDel(this.msgReceived);
                object[] args = new object[] { txt };
                this.Invoke(del, args);
             } else
                this.richTextBox1.AppendText(txt+"\n");
        }

        private delegate void AddTextDel2(string txt);
        public void ActualizaEndInvoke(string txt)
        {
            if (this.InvokeRequired)
            {
                AddTextDel2 del = new AddTextDel2(this.ActualizaEndInvoke);
                object[] args = new object[] { txt };
                this.Invoke(del, args);
            }
            else
                this.richTextBox2.AppendText(txt + "\n");
        }
    }

    public class MsgBox : MarshalByRefObject, IMsgBox
    {
        Form1 myForm;
        public MsgBox(Form1 f)
        {
            myForm = f;

        }
        #region IMsgBox Members

        public void acceptMsg(string userName, string msg)
        {
            myForm.msgReceived("Msg received from " + userName + ": " + msg);

        }

        #endregion
    }
    
}
