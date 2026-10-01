using System.ComponentModel;
using ModelContextProtocol.Server;

namespace LocalMCPServer;

[McpServerToolType]
public class RegisterUser
{
    [McpServerTool]
    [Description("Register a new user with the given username and password.")]
    public static string RegisterNewUser(
        [Description("The username of the new user.")]
        string username, 
        [Description("The password of the new user.")]
        string password)
    {
        return "";
    }
}