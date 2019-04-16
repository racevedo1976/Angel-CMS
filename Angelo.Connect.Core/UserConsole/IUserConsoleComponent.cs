using System;

namespace Angelo.Connect.UserConsole
{
    public interface IUserConsoleComponent
    {
        string ComponentType { get; }

        int ComponentOrder { get; }
    }
}
