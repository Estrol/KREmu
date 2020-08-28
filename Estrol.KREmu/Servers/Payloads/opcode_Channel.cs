using Estrol.KREmu.Servers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Estrol.KREmu.Servers.Payloads {
    public class opcode_Channel : SendPacket {
        public override void GetData(Connection state) {
            state.payload = "channel";

            int ChannelID = state.Buffer[6];
            Console.WriteLine("[Server] Client entering channel {0}", ChannelID + 1);

            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms)) {
                bw.Write(new byte[] { 0x0c, 0x00 });
                bw.Write((byte)0xed);
                bw.Write((byte)0x03);
                bw.Write(new byte[8]);

                state.Send(ms.ToArray());
            }
        }
    }
}
