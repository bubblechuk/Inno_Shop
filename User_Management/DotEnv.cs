using System.IO;
using System.Net;

namespace User_Management;

public static class DotEnv
{
    public static void Load(string path)
    {
        if (!File.Exists(path))
        {
            return;
        }

        foreach (var line in File.ReadAllLines(path))
        {
            var parts = line.Split('=', 2);
            if (parts.Length != 2) continue;
            Environment.SetEnvironmentVariable(parts[0], parts[1]);
        }
    }
    
}