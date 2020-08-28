using System;
using System.IO;

namespace Estrol.KREmu.Servers.Payloads {
    public class opcode_Rooms : SendPacket {
        public override void GetData(Connection state) {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms)) {
                bw.Write(new byte[] { 0x1e, 0x00 }); // Packet length: 30 byte
                bw.Write(new byte[] { 0xd3, 0x07 }); // Packet opcode: 0x07d3
                bw.Write(new byte[4]);
                bw.Write(new byte[] { 0x16, 0x00 });
                bw.Write(new byte[] { 0xea, 0x07, 0x02, 0x00, 0x00, 0x00 });
                bw.Write(new byte[] { 0xe8, 0x02 });
                bw.Write(new byte[5]);
                bw.Write(new byte[] { 0xf4, 0x02 });
                bw.Write(new byte[5]);

                state.Send(ms.ToArray());
            }
        }
    }
}
