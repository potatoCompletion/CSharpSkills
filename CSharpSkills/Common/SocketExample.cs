using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Socket;

namespace DotNet5_Tester
{
    class Program
    {
        static SocketManager.SocketServer socketServer = null;
        static Thread thread = null;
        static void Main(string[] args)
        {

            try
            {
                thread = new Thread(socketInit);
                thread.Start();

                while (true) { }    // loop
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message);
            }
        }

        private static void socketInit(object obj)
        {
            try
            {
                socketServer = new SocketManager.SocketServer(5005);

                socketServer.OnAccept += SocketServer_OnAccept;
                socketServer.OnReceive += SocketServer_OnReceive;
                socketServer.ListenerStart();
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message);
            }
        }

        private static void SocketServer_OnReceive(object sender, SocketManager.SocketReceiveEventArgs e)
        {
            // Do Something
        }

        private static void SocketServer_OnAccept(object sender, SocketManager.SocketAcceptEventArgs e)
        {
            Console.WriteLine("==================== Socket Client Connected. ====================");
            Console.WriteLine("Client Info : " + e.Worker.RemoteEndPoint);
            socketServer.DataReceive();
        }
    }
}
