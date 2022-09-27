using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Protocol.Commands
{

    public class CommandsHandler
    {
        public static ServerCommands serverCommands = new ServerCommands();

        public static Dictionary<string, Delegate> Commands = new()
        {
            { "01", serverCommands.SaveNewUser },
            { "02", serverCommands.SaveNewUserProfile },
            { "03", serverCommands.UploadUserProfileImage },
            { "04", serverCommands.GetAllProfiles },
            { "05", serverCommands.SaveNewMessage },
            { "06", serverCommands.GetNotReadedMessages },
            { "07", serverCommands.GetUserMessagesHistory },
            { "08", serverCommands.GetUserMessagesHistory },
            { "09", serverCommands.DownloadUserProfileImage },
            { "10", serverCommands.UpdateUser },
        };
    }
}
