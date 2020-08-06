using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DAPEmulator
{

    class DAPEmulatorComm
    {
        public DelegateCollection.updateUIControl threadDelegate;

        DAPEmulatorUI emulatorUI = new DAPEmulatorUI();
        Utils utils = new Utils();
        DAPParameters dapParameters = new DAPParameters();
        public CommThread commThread;

        IPEndPoint point;

        //declare sockets for listening and sending packets
        Socket commSocket;
        Socket listenSocket;

        //declare threads for listening connection created by client and accepting packets from client 
        Thread serverListenThread;
        Thread packetReceiveThread;

        public DAPEmulatorComm()
        { 
        }

        public DAPEmulatorComm(DAPEmulatorUI emulatorUI, object obj)
        {
            this.listenSocket = obj as Socket;
            this.emulatorUI = emulatorUI;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="port"></param>
        public void Listen(object endPoint) 
        {
            this.point = endPoint as IPEndPoint;
            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            listenSocket.Bind(point);
            listenSocket.Listen(10);

            //create an instance of thread for listening
            //serverListenThread = new Thread(new ParameterizedThreadStart(StartListenMethod));
            //serverListenThread.IsBackground = true;
            //serverListenThread.Start(listenSocket);


            StartListenMethod(listenSocket);

        }

        /// <summary>
        /// method for waiting for connection from client and communicating with client
        /// <param name="obj"></param>
        private void StartListenMethod(object obj)
        {
            Socket listenSocket = obj as Socket;
            //emulatorUI.DisplayLog("listen successfully!", true, point.ToString());



            if (threadDelegate != null)
            {
                threadDelegate("listen successfully!", true, point.ToString());
            }

            while (true)
            {
                commSocket = listenSocket.Accept();

                string ipStr = commSocket.RemoteEndPoint.ToString();
                emulatorUI.ip.Text = ipStr;
                Application.DoEvents();

                threadDelegate("connect successfully!", true, ipStr);

                commThread = new CommThread(emulatorUI, commSocket);

                //create a thread for receiving packet from client
                Thread threadReceive = new Thread(new ThreadStart(commThread.RunThread));
                threadReceive.IsBackground = true;
                threadReceive.Start();
            }
        }



    }
}
