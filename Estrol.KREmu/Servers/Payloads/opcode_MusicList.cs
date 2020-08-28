using Estrol.KREmu.Servers;
using Estrol.KREmu.Servers.Parser.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Estrol.KREmu.Servers.Payloads {
    public class opcode_MusicList : SendPacket {
        public void GetData(Connection state, OJNList ojnlist) {
            OJN[] headers = ojnlist.GetHeaders();

            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms)) {
                short length = (short)(6 + (headers.Length * 12) + 12);
                bw.Write(length);
                bw.Write(new byte[] { 0xBF, 0x0F });
                bw.Write((short)ojnlist.Count);

                foreach (OJN ojn in headers) {
                    bw.Write((short)ojn.Id);
                    bw.Write((short)ojn.NoteCountEx);
                    bw.Write((short)ojn.NoteCountNx);
                    bw.Write((short)ojn.NoteCountHx);
                    bw.Write(new byte[4]);
                }

                bw.Write(new byte[12]);

                state.Send(ms.ToArray());
            }
        }
    }
}
