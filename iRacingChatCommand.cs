using iRacingSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.CornerSpeed
{
    public class iRacingChatCommand : iRacingMessaging
    {
        iRacingChatCommand() : base()
        {

        }

        void SendChat()
        {
            SendMessage(BroadcastMessage.ChatComand, (short)ChatCommandMode.Macro, 0);
        }
    }
}
