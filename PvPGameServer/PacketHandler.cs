using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer;

public class PacketHandler
{
    public static Func<string, byte[], bool> NetworkSendFunc;
    public static Func<string, bool> ForceSession;

    public static Action<MemoryPackBinaryRequestInfo> DistributeInnerPacket;

    protected UserManager _userMgr = null;


    public void Init(UserManager userMgr)
    {
        _userMgr = userMgr;
    }
}
