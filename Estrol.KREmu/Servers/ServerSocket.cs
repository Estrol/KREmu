using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Estrol.KREmu.Servers {
    public class ServerSocket {
        private readonly int Port;
        private Socket _socket;

        public delegate void OnServerConnectionEvent(object sender, Connection state);
        public event OnServerConnectionEvent OnServerConnection;

        public ServerSocket(short Port) {
            this.Port = Port;

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(new IPEndPoint(IPAddress.Any, this.Port));
        }

        public void Start() {
            _socket.Listen(Port);
            Console.WriteLine("[Server] Planet {0} now listening at port {1}", 1, Port);

            _socket.BeginAccept(new AsyncCallback(ServerSocket_OnAsyncConnection), _socket);
        }

        public void Send(Connection state, byte[] data) {
            Socket socket = state.Socket;
            ushort dataLength = BitConverter.ToUInt16(data, 0);

            string payload = state.payload;

            socket.BeginSend(data, 0, dataLength, 0, new AsyncCallback(ServerSocket_OnAsyncSend), state);
        }

        public void CloseSocket(Connection state) {
            Console.WriteLine("[Server] A client disconnected!");
            state.Socket.Close();
            state.Dispose();
        }

        public void Read(Connection state) {
            state.raw = new byte[Connection.MAX_SOCKET_BUFFER];
            state.Buffer = null;
            state.Socket.BeginReceive(state.raw, 0, Connection.MAX_SOCKET_BUFFER, SocketFlags.None, new AsyncCallback(ServerSocket_OnAsyncData), state);
        }

        private void ServerSocket_OnAsyncSend(IAsyncResult result) {
            try {
                Connection state = (Connection)result.AsyncState;

                state.raw = new byte[Connection.MAX_SOCKET_BUFFER];
                state.Buffer = null;
                state.Socket.BeginReceive(state.raw, 0, Connection.MAX_SOCKET_BUFFER, SocketFlags.None, ServerSocket_OnAsyncData, state);
            } catch (Exception e) {
                if (e is ObjectDisposedException) {
                    Console.WriteLine("[C# Exception] A thread tried to access disposed object.");
                } else if (e is SocketException) {
                    SocketException err = (SocketException)e;
                    if (err.ErrorCode == 10054) {
                        Console.WriteLine("[C# Exception] A thread tried to access socket that already disconnected");
                    }
                }

                Console.WriteLine(e.Message);
            }
        }

        private void Thread_CheckConnection(Connection state) {
            Socket socket = state.Socket;

            while (true) {
                bool status = Socket_CheckConnection(socket);
                if (!status) {
                    CloseSocket(state);
                    break;
                }
            }
        }

        private bool Socket_CheckConnection(Socket socket) {
            try {
                return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
            } catch (Exception) {
                return false;
            }
        }

        private void SendDataToEvent(Connection state) {
            if (OnServerConnection == null) return;
            OnServerConnection(this, state);
        }

        private void ServerSocket_OnAsyncConnection(IAsyncResult result) {
            Connection state;

            try {
                Socket socket = (Socket)result.AsyncState;

                state = new Connection() {
                    Socket = socket.EndAccept(result),
                    Server = this,
                    raw = new byte[Connection.MAX_SOCKET_BUFFER]
                };

                Thread check = new Thread(() => Thread_CheckConnection(state)) {
                    IsBackground = true
                };
                check.Start();

                Console.WriteLine("[Server] A Client connected");
                state.Socket.BeginReceive(state.raw, 0, Connection.MAX_SOCKET_BUFFER, SocketFlags.None, ServerSocket_OnAsyncData, state);
                _socket.BeginAccept(new AsyncCallback(ServerSocket_OnAsyncConnection), _socket);
            } catch (Exception e) {
                if (e is ObjectDisposedException) {
                    Console.WriteLine("[C# Exception] A thread tried to access disposed object.");
                } else if (e is SocketException) {
                    SocketException err = (SocketException)e;
                    if (err.ErrorCode == 10054) {
                        Console.WriteLine("[C# Exception] A thread tried to access socket that already disconnected");
                    }
                }

                Console.WriteLine(e.Message);
            }
        }

        private void ServerSocket_OnAsyncData(IAsyncResult result) {
            try {
                Connection state = (Connection)result.AsyncState;

                ushort opcode = BitConverter.ToUInt16(state.raw, 2);
                state.opcode = opcode;

                ushort PacketLength = BitConverter.ToUInt16(state.raw, 0);
                state.Buffer = new byte[PacketLength];
                Buffer.BlockCopy(state.raw, 0, state.Buffer, 0, PacketLength);

                state.raw = null;

                SendDataToEvent(state);
            } catch (Exception e) {
                if (e is ObjectDisposedException) {
                    Console.WriteLine("[C# Exception] A thread tried to access disposed object.");
                } else if (e is SocketException) {
                    SocketException err = (SocketException)e;
                    if (err.ErrorCode == 10054) {
                        Console.WriteLine("[C# Exception] A thread tried to access socket that already disconnected");
                    }
                }

                Console.WriteLine(e.Message);
            }
        }
    }

    public class Connection : IDisposable {
        public const int MAX_SOCKET_BUFFER = 10248;
        public ServerSocket Server;
        public Socket Socket;

        public string payload = "login";
        public ushort opcode;
        public byte[] Buffer;
        public byte[] raw;

        ~Connection() => Destroy();

        public void Dispose() { }

        public void Send(byte[] data) {
            Server.Send(this, data);
        }

        public void Read() {
            Server.Read(this);
        }

        public void Destroy() {
            Socket.Close();
            Dispose();
        }
    }
}
