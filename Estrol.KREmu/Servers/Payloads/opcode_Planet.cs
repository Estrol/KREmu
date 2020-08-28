using Estrol.KREmu.Servers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Estrol.KREmu.Servers.Payloads {
    public class opcode_Planet : SendPacket {
        public override void GetData(Connection state) {
            state.payload = "planet";

            int planetNumber = state.Buffer[3];
            Console.WriteLine("[Server] Client connected to planet no. {0}", planetNumber);

            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms)) {
                bw.Write(new byte[] { 0x0E, 0x01, 0xEB, 0x03 });    // Opcode and Packet length
                bw.Write(new byte[] {                               // Total channel?
                    0x2C, 0x01, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00
                });

                for (int i = 0; i < 20; i++) {
                    bw.Write(new byte[] {                           // Write channel data
                        0x78, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00,
                        0x01, 0x00, 0x00
                    });

                    bw.Write(new byte[] {                           // Channel position
                        (byte)(i + 1),
                        0x00
                    });
                }

                bw.Write(new byte[] {                               // Same channel data but the post goto one again. 
                    0x78, 0x00, 0x00, 0x00,                         // Idk why.
                    0x00, 0x00, 0x00, 0x00,
                    0x01, 0x00, 0x00
                });

                state.Send(ms.ToArray());
            }
        }
    }
}
