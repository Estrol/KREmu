using Estrol.KREmu.Servers.Parser;
using Estrol.KREmu.Servers.Parser.Types;
using Estrol.KREmu.Servers.Payloads;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Estrol.KREmu.Servers {
    public class SessionHandler {
        private readonly MainWindow MainWindow;
        private readonly ServerSocket Server;
        private OJNList ojnlist;
        public SessionHandler(MainWindow MainWindow) {
            this.MainWindow = MainWindow;
            Server = new ServerSocket(15010);

            #if NETCOREAPP3_1
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            #endif
        }

        public void Start() {
            LoadOJNlist();

            Server.OnServerConnection += Server_OnConnection;
            Server.Start();
        }

        public void LoadOJNlist() {
            ojnlist = OJNListDecoder.Decode(AppDomain.CurrentDomain.BaseDirectory + @"\Image\OJNList.dat", true);
        }

        public void Server_OnConnection(object o, Connection state) {
            switch (state.opcode) {
                case Packets.LOGIN: new opcode_Login().GetData(state); break;
                case Packets.PLANET: new opcode_Planet().GetData(state); break;
                case Packets.CHANNEL: new opcode_Channel().GetData(state); break;
                case Packets.CHARACTER: new opcode_Character().GetData(state); break;
                case Packets.ROOMS: new opcode_Rooms().GetData(state); break;
                case Packets.MUSICLIST: new opcode_MusicList().GetData(state, this.ojnlist); break;
                case Packets.DISCONNECT: {
                    state.Destroy();
                    break;
                }
                default: {
                    Console.WriteLine("Unknown opcode: {0} or {1}", LittleEndian(state.opcode), state.opcode);
                    state.Read();
                    break;
                }
            }
        }

        private static string LittleEndian(ushort num) {
            byte[] bytes = BitConverter.GetBytes(num);
            string retval = "";
            foreach (byte b in bytes)
                retval += b.ToString("X2");
            return retval;
        }
    }

    public static class Packets {
        public const ushort LOGIN = 0x03e8;
        public const ushort DISCONNECT = 0xfff0;
        public const ushort MUSICLIST = 0x0fbe;
        public const ushort PLANET = 0x03ea;
        public const ushort CHANNEL = 0x03ec;
        public const ushort CHARACTER = 0x07d0;
        public const ushort ROOMS = 0x07d2;
    }
}
