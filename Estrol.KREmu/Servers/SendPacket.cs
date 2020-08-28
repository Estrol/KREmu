using System;
using System.Collections.Generic;
using System.Text;

namespace Estrol.KREmu.Servers {
    public abstract class SendPacket {
        public virtual void GetData(Connection state) {
            throw new Exception("This OPCODE handler is not constructed");
        }
    }
}
