using System;
using System.IO;
using System.Text;

namespace Estrol.KREmu.Servers.Payloads {
    public class opcode_Login : SendPacket {
        public override void GetData(Connection state) {
            byte[] byteUUID = new byte[30];
            Buffer.BlockCopy(state.Buffer, 19, byteUUID, 0, 30);
            Console.WriteLine("[Server] User {0} logged in.", Encoding.ASCII.GetString(byteUUID));

            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms)) {
                bw.Write(new byte[] { 0x26, 0x00 }); // Login header - packet length;
                bw.Write(new byte[] { 0xe9, 0x03 }); // Login header - opcode;
                bw.Write(new byte[4]);
                bw.Write(new byte[] { 0x20, 0x00, 0x00, 0x00 });

                string Date = DateTime.UtcNow.ToString("yyyy-dd-MM");
                string Time = DateTime.UtcNow.ToString("hh:mm:ss");
                string DateNTime = string.Format("{0} {1}", Date, Time);

                bw.Write(Encoding.UTF8.GetBytes(DateNTime));
                bw.Write(new byte[] {
                    0x00,
                    0x6e, 0xce,
                    0x04, 0x00,
                    0x00, 0x00
                });

                byte[] data = ms.ToArray();
                data[0] = (byte)data.Length;

                state.Send(data);
            }
        }
    }
}
