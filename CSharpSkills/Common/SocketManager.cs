using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Socket {
    public class SocketManager
    {
        private static string Connention;
        public delegate void SocketAcceptEventHandler(object sender, SocketAcceptEventArgs e);
        public delegate void SocketReceiveEventHandler(object sender, SocketReceiveEventArgs e);
        public delegate void SocketErrorEventHandler(object sender, SocketErrorEventArgs e);
        public delegate void SocketSendEventHandler(object sender, SocketSendEventArgs e);
        public class SocketAcceptEventArgs : EventArgs
        {
            private readonly System.Net.Sockets.Socket conn;

            public SocketAcceptEventArgs(System.Net.Sockets.Socket conn)
            {
                this.conn = conn;
            }

            public System.Net.Sockets.Socket Worker
            {
                get { return this.conn; }
            }
        }
        public class SocketReceiveEventArgs : EventArgs
        {
            private readonly int receiveBytes;
            private readonly byte[] receiveData;

            public SocketReceiveEventArgs(int receiveBytes, byte[] receiveData)
            {
                this.receiveBytes = receiveBytes;
                this.receiveData = receiveData;
            }

            public int ReceiveBytes
            {
                get { return this.receiveBytes; }
            }

            public byte[] ReceiveData
            {
                get { return this.receiveData; }
            }
            public string Connection
            {
                get { return Connention; }
            }
        }
        public class SocketErrorEventArgs : EventArgs
        {
            private readonly Exception exception;
            private string message = string.Empty;

            public SocketErrorEventArgs(Exception exception, string MSG)
            {
                this.exception = exception;
                this.message = MSG;
            }

            public SocketErrorEventArgs(Exception exception)
            {
                this.exception = exception;
            }

            public Exception AsyncSocketException
            {
                get { return this.exception; }
            }

            public string Message
            {
                get { return this.message; }
                set { this.message = Message; }
            }
        }
        public class SocketSendEventArgs : EventArgs
        {
            private readonly int sendBytes;

            public SocketSendEventArgs(int sendBytes)
            {
                this.sendBytes = sendBytes;
            }

            public int SendBytes
            {
                get { return this.sendBytes; }
            }

        }





        public class SocketServer
        {
            public event SocketErrorEventHandler OnError;
            public event SocketAcceptEventHandler OnAccept;
            public event SocketReceiveEventHandler OnReceive;
            public event SocketSendEventHandler OnSend;
            protected virtual void Error(SocketErrorEventArgs e)
            {
                SocketErrorEventHandler handler = OnError;

                if (handler != null)
                    handler(this, e);
            }
            protected virtual void Accepted(SocketAcceptEventArgs e)
            {
                SocketAcceptEventHandler handler = OnAccept;

                if (handler != null)
                    handler(this, e);
            }
            protected virtual void Received(SocketReceiveEventArgs e)
            {
                SocketReceiveEventHandler handler = OnReceive;

                if (handler != null)
                    handler(this, e);
            }
            protected virtual void Send(SocketSendEventArgs e)
            {
                SocketSendEventHandler handler = OnSend;

                if (handler != null)
                    handler(this, e);
            }

            System.Net.Sockets.Socket _sock = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            System.Net.Sockets.Socket _clienSock = null;
            byte[] _buff = new byte[65536];

            public SocketServer(int p_port)
            {
                IPEndPoint ep = new IPEndPoint(IPAddress.Any, p_port);
                _sock.Bind(ep);
            }

            public void ListenerStart()
            {
                _sock.Listen(100);

                StartAccept();
            }

            private void StartAccept()
            {
                try
                {
                    _sock.BeginAccept(new AsyncCallback(OnListenCallBack), _sock);
                }
                catch (Exception ex)
                {
                    SocketErrorEventArgs errEV = new SocketErrorEventArgs(ex, "[StartAccept]");
                    Error(errEV);
                }
            }
            private void OnListenCallBack(IAsyncResult ar)
            {
                try
                {
                    if (_sock != null)
                    {
                        _clienSock = _sock.EndAccept(ar);

                        SocketAcceptEventArgs aev = new SocketAcceptEventArgs(_clienSock);
                        Connention = _clienSock.RemoteEndPoint.ToString();
                        Accepted(aev);

                        StartAccept();
                    }
                }
                catch (System.Exception e)
                {
                    SocketErrorEventArgs errEV = new SocketErrorEventArgs(e, "[OnListenCallBack]");
                    Error(errEV);
                }
            }

            public void DataReceive()
            {
                try
                {
                    _clienSock.BeginReceive(_buff, 0, _buff.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), _buff);
                }
                catch (Exception ex)
                {
                    SocketErrorEventArgs errEV = new SocketErrorEventArgs(ex, "[SERVER] dataReceive");
                    Error(errEV);
                }
            }

            private void ReceiveCallBack(IAsyncResult ar)
            {
                try
                {
                    if (_clienSock != null)
                    {
                        if (!_clienSock.Connected)
                            return;

                        byte[] lbuff = (byte[])ar.AsyncState;

                        int recv = _clienSock.EndReceive(ar);

                        if (recv == 0)
                        {
                            DisConnect();
                            return;
                        }
                        else
                        {
                            SocketReceiveEventArgs recvEV = new SocketReceiveEventArgs(recv, lbuff);
                            Received(recvEV);
                        }

                        DataReceive();
                    }
                }
                catch (Exception e)
                {
                    SocketErrorEventArgs errEV = new SocketErrorEventArgs(e, "[SERVER] RECEIVE_CALL_BACK");
                    Error(errEV);

                    if (e.Message != "현재 연결은 원격 호스트에 의해 강제로 끊겼습니다")
                        DataReceive();
                }
            }
            public void DisConnect()
            {
                try
                {
                    if (_clienSock != null)
                    {
                        if (!_clienSock.Connected)
                            return;

                        _clienSock.Shutdown(SocketShutdown.Both);
                        _clienSock.BeginDisconnect(false, new AsyncCallback(OnCloseCallBack), _clienSock);
                    }
                }
                catch (System.Exception e)
                {
                    SocketErrorEventArgs errEV = new SocketErrorEventArgs(e, "[SERVER] DISCONNECT");
                    Error(errEV);
                }
            }
            private void OnCloseCallBack(IAsyncResult ar)
            {
                try
                {
                    if (_clienSock != null)
                    {
                        _clienSock.EndDisconnect(ar);
                        _clienSock.Close();

                        _clienSock = null;
                    }
                }
                catch (System.Exception e)
                {
                    SocketErrorEventArgs errEV = new SocketErrorEventArgs(e, "[SERVER] OnCloseCallBack");
                    Error(errEV);
                }
            }


            public void Send(string msg)
            {
                try
                {
                    string strBuffer = msg;

                    byte[] lbuff = DecodeByteToStr(strBuffer);

                    _clienSock.BeginSend(lbuff, 0, lbuff.Length, SocketFlags.None, new AsyncCallback(SendCallBack), lbuff);

                }
                catch (System.Exception e)
                {
                    SocketErrorEventArgs errEV = new SocketErrorEventArgs(e, "[SERVER] SEND");
                    Error(errEV);
                }
            }
            private void SendCallBack(IAsyncResult ar)
            {
                try
                {
                    int intSend = _clienSock.EndSend(ar);

                    if (intSend == 0)
                    {

                    }
                    else
                    {
                        SocketSendEventArgs SendEV = new SocketSendEventArgs(intSend);
                        Send(SendEV);
                    }
                }
                catch (System.Exception e)
                {
                    SocketErrorEventArgs errEV = new SocketErrorEventArgs(e, "[SERVER] SEND_CALLBACK");
                    Error(errEV);
                }
            }
            private byte[] DecodeByteToStr(string strMsg)
            {
                return Encoding.Default.GetBytes(strMsg);
            }
        }

        public class SocketClient
        {
            private bool _bConn = false;

            public bool Connected
            {
                get { return Connect(); }
            }

            private int _intSize = 0;
            public event SocketErrorEventHandler OnError;
            public event SocketReceiveEventHandler OnReceive;
            public event SocketSendEventHandler OnSend;
            protected virtual void Error(SocketErrorEventArgs e)
            {
                SocketErrorEventHandler handler = OnError;

                if (handler != null)
                    handler(this, e);
            }

            protected virtual void Received(SocketReceiveEventArgs e)
            {
                SocketReceiveEventHandler handler = OnReceive;

                if (handler != null)
                    handler(this, e);
            }
            protected virtual void Send(SocketSendEventArgs e)
            {
                SocketSendEventHandler handler = OnSend;

                if (handler != null)
                    handler(this, e);
            }


            System.Net.Sockets.Socket _sock = null;
            IPEndPoint ep = null;
            private string sIP;
            private int sPort;
            byte[] _buff = new byte[65536];
            public SocketClient(string p_ip, int p_port)
            {
                ep = new IPEndPoint(IPAddress.Parse(p_ip), p_port);
                this.sIP = p_ip;
                this.sPort = p_port;
            }

            public bool Connect()
            {
                try
                {
                    _sock = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    IAsyncResult result = _sock.BeginConnect(sIP, sPort, null, null);
                    bool success = result.AsyncWaitHandle.WaitOne(5000, true);

                    if (_sock.Connected)
                    {
                        _sock.EndConnect(result);
                    }
                    else
                    {
                        // NOTE, MUST CLOSE THE SOCKET

                        _sock.Close();
                        throw new ApplicationException("Failed to connect server.");
                    }
                    //IAsyncResult result = _sock.BeginConnect(ep, new AsyncCallback(OnConnectCallback), _sock);                    
                    //_sock.Connect(ep);
                    Receive();
                    _bConn = true;
                }
                catch (Exception ex)
                {
                    SocketErrorEventArgs errEV = new SocketErrorEventArgs(ex, "[CLIENT] CONNECT");
                    Error(errEV);
                    _bConn = false;
                }


                return _bConn;
            }

            private void OnConnectCallback(IAsyncResult ar)
            {
                try
                {
                    _sock = (System.Net.Sockets.Socket)ar.AsyncState;

                    _sock.EndConnect(ar);

                    Receive();


                }
                catch (System.Exception e)
                {
                    SocketErrorEventArgs errEV = new SocketErrorEventArgs(e, "[SOCKET_CLIENT] OnConnectCallback");
                    Error(errEV);
                }
            }
            public void Receive()
            {
                byte[] _bufData = new byte[65536];

                try
                {
                    _sock.BeginReceive(_bufData, 0, _bufData.Length, SocketFlags.None, new AsyncCallback(OnReceiveCallBack), _bufData);

                }
                catch (System.Exception e)
                {
                    SocketErrorEventArgs errEV = new SocketErrorEventArgs(e, "[SOCKET_CLIENT] Receive");
                    Error(errEV);
                }
            }
            private void OnReceiveCallBack(IAsyncResult ar)
            {
                byte[] bytes = (byte[])ar.AsyncState;

                try
                {
                    _intSize = _sock.EndReceive(ar);

                    if (_intSize > 0)
                    {
                        SocketReceiveEventArgs REV = new SocketReceiveEventArgs(_intSize, bytes);
                        Received(REV);
                    }

                    Receive();
                }
                catch (System.Exception e)
                {
                    SocketErrorEventArgs errEV = new SocketErrorEventArgs(e, "[SOCKET_CLIENT] CallBack_ReceiveMsg");
                    Error(errEV);
                }
            }

            public bool Send(string strMsgData)
            {
                Thread.Sleep(10);
                try
                {
                    if (!_bConn)
                        Connect();

                    string strBuffer = strMsgData;

                    byte[] buffer = EncodeByteToStr(strBuffer);


                    _sock.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnSendCallBack), _sock);

                }
                catch (System.Exception e)
                {
                    _bConn = false;
                    SocketErrorEventArgs errEV = new SocketErrorEventArgs(e, "[SOCKET_CLIENT] Send");
                    Error(errEV);


                    return false;
                }

                return true;
            }

            private void OnSendCallBack(IAsyncResult ar)
            {
                _sock = (System.Net.Sockets.Socket)ar.AsyncState;


                try
                {
                    _intSize = _sock.EndSend(ar);


                    SocketSendEventArgs sev = new SocketSendEventArgs(_intSize);
                    Send(sev);
                }
                catch (System.Exception e)
                {

                    SocketErrorEventArgs eev = new SocketErrorEventArgs(e, "[SOCKET_CLIENT] OnSendCallBack");
                    Error(eev);

                }
            }
            public byte[] EncodeByteToStr(string strMsg)
            {
                return Encoding.Default.GetBytes(strMsg);
            }
            public void Disconnect()
            {
                try
                {
                    _sock.Shutdown(SocketShutdown.Both);
                    _sock.BeginDisconnect(false, new AsyncCallback(OnCloseCallBack), _sock);
                    _bConn = false;
                }
                catch (System.Exception e)
                {
                    SocketErrorEventArgs errEV = new SocketErrorEventArgs(e, "[SOCKET_CLIENT] Disconnect");
                    Error(errEV);
                }
            }

            private void OnCloseCallBack(IAsyncResult ar)
            {
                try
                {
                    _sock.EndDisconnect(ar);
                    _sock.Close();
                }
                catch (System.Exception e)
                {
                    SocketErrorEventArgs errEV = new SocketErrorEventArgs(e, "[SOCKET_CLIENT] OnCloseCallBack");
                    Error(errEV);
                }
            }
        }
    }
}
