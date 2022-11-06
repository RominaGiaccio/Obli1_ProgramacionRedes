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
            { "01", serverCommands.SaveNewUserAsync },
            { "02", serverCommands.SaveNewUserProfileAsync },
            { "03", serverCommands.UploadUserProfileImageAsync },
            { "04", serverCommands.GetAllProfilesAsync },
            { "05", serverCommands.SaveNewMessageAsync },
            { "06", serverCommands.GetNotReadedMessagesAsync },
            { "07", serverCommands.GetUserMessagesHistoryAsync },
            { "08", serverCommands.GetUserMessagesHistoryAsync },
            { "09", serverCommands.DownloadUserProfileImageAsync },
            { "10", serverCommands.SignInAsync },
            { "11", serverCommands.SignOutAsync },
        };
    }
}
