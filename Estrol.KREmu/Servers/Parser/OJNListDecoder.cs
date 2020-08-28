using Estrol.KREmu.Servers.Parser.Types;
using System.IO;
using System.Text;

namespace Estrol.KREmu.Servers.Parser {
    public static class OJNListDecoder {
        private const string KoreanEncoding = "ks_c_5601-1987";
        private const string ChineseEncoding = "big5";

        public static OJNList Decode(string FileName, bool newFormat) {
            return Decode(File.Open(FileName, FileMode.Open), false, newFormat);
        }

        public static OJNList Decode(Stream stream, bool keepOpen = false, bool newFormat = false) {
            var headers = new OJNList();
            byte[] inputData = new byte[0];

            using (var reader = new BinaryReader(stream, Encoding.Unicode, keepOpen)) {
                stream.Seek(0, SeekOrigin.Begin);
                int songCount = reader.ReadInt32();

                headers.Version = newFormat ? FileFormat.New : FileFormat.Old; // (stream.Length > 4 + (songCount * 300) ? FileFormat.New : FileFormat.Old);
                var charset = headers.Version == FileFormat.New ? KoreanEncoding : ChineseEncoding;

                for (int i = 0; i < songCount; i++) {
                    headers.Add(OJNDecoder.Decode(reader.ReadBytes(300), Encoding.GetEncoding(charset)));
                }

                if (headers.Version == FileFormat.New) {
                    songCount = reader.ReadInt32();
                    for (int i = 0; i < songCount; i++) {
                        int id = reader.ReadInt32();
                        int val1 = reader.ReadInt32();
                        int val2 = reader.ReadInt32();
                        int val3 = reader.ReadInt32();
                    }

                    songCount = reader.ReadInt32();
                    for (int i = 0; i < songCount; i++) {
                        int id = reader.ReadInt32();
                        int state = reader.ReadInt32();
                        int val1 = reader.ReadInt32();
                        int val2 = reader.ReadInt32();
                        int val3 = reader.ReadInt32();
                    }

                    songCount = reader.ReadInt32();
                    for (int i = 0; i < songCount; i++) {
                        int id = reader.ReadInt32();
                        int val1 = reader.ReadInt32();
                        int val2 = reader.ReadInt32();
                        int val3 = reader.ReadInt32();
                    }

                    songCount = reader.ReadInt32();
                    for (int i = 0; i < songCount; i++) {
                        int id = reader.ReadInt32();
                        int val1 = reader.ReadInt32();
                        int val2 = reader.ReadInt32();
                    }

                    songCount = reader.ReadInt32();
                    for (int i = 0; i < songCount; i++) {
                        int id = reader.ReadInt32();
                        int val1 = reader.ReadInt32();
                    }

                    songCount = reader.ReadInt32();
                    for (int i = 0; i < songCount; i++) {
                        int id = reader.ReadInt32();
                        int val1 = reader.ReadInt32();
                        int val2 = reader.ReadInt32();
                    }

                    songCount = reader.ReadInt32();

                    songCount = reader.ReadInt32();
                    for (int i = 0; i < songCount; i++) {
                        int id = reader.ReadInt32();
                        if (id > 0 && headers.Contains(id)) {
                            var header = headers[id];
                            header.KeyMode = reader.ReadByte();
                            reader.ReadInt16();
                            reader.ReadByte();

                        } else {
                            reader.ReadInt32();
                        }
                    }
                }

                return headers;
            }
        }
    }
}
