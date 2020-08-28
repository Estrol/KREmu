using Estrol.KREmu.Servers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Estrol.KREmu.Servers.Payloads {
    public class opcode_Character : SendPacket {
        public override void GetData(Connection state) {
            Console.WriteLine("[Server] Sending player details");

            string name = "Estrol";
            string gender = "Male";

            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms)) {
                if (name.Length > 5) {
                    int count = name.Length - 5;
                    int total = 433 + count;
                    bw.Write((short)total);
                } else {
                    bw.Write((short)433);
                }

                bw.Write(new byte[] { 0xd1, 0x07 });
                bw.Write(new byte[4]);

                bw.Write(Encoding.ASCII.GetBytes(name));
                bw.Write((byte)0x00); // Seperator

                if (gender == "Male") {
                    bw.Write((byte)0x00);
                } else {
                    bw.Write((byte)0x01);
                }

                bw.Write((byte)0x7f);
                bw.Write(new byte[] {
                    0x96, 0x98, 0x00, 0xFF,
                    0xFF, 0xFF, 0x00, 0x7F,
                    0x96, 0x98, 0x00, 0x64
                });

                bw.Write(new byte[] {
                    0x00, 0x00, 0x00, 0x10,
                    0x00, 0x00, 0x00, 0x0B,
                    0x00, 0x00, 0x00, 0x05,
                    0x00, 0x00, 0x00, 0x3A,
                    0x17, 0x59, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00,
                });

                for (int i = 0; i < 16; i++) {
                    bw.Write(new byte[] { // Item Data
                        0x00,
                        0x00
                    });

                    bw.Write(new byte[2]);
                }

                // Begin lazy data, first empty data that I lazy to make parser or it!
                // Probably inventory slot that not important in this KREmu.
                bw.Write(new byte[] {
                    0x3C, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00,
                });

                bw.Write(new byte[13 * 18]);
                bw.Write(new byte[] {   // Just bunch Inventory slot!
                    0x00, 0x26, 0x03, 0x00, 0x00, 0x24, 0x03, 0x00,
                    0x00, 0x22, 0x03, 0x00, 0x00, 0x20, 0x03, 0x00,
                    0x00, 0x1E, 0x03, 0x00, 0x00, 0x1C, 0x03, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00,
                    0x00, 0x1C, 0x03, 0x00, 0x00, 0x99, 0x99, 0x00,
                    0x00, 0x1E, 0x03, 0x00, 0x00, 0x99, 0x99, 0x00,
                    0x00, 0x20, 0x03, 0x00, 0x00, 0x99, 0x99, 0x00,
                    0x00, 0x22, 0x03, 0x00, 0x00, 0x99, 0x99, 0x00,
                    0x00, 0x24, 0x03, 0x00, 0x00, 0x99, 0x99, 0x00,
                    0x00, 0x26, 0x03, 0x00, 0x00, 0x99, 0x99, 0x00
                });

                bw.Write((byte)0x00); // End lazy data!


                state.Send(ms.ToArray());
            }
        }
    }
}
