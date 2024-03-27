using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarkRoles.Modules.Customs;
public static class CustomTags
{
    public static string Dir = @"./Dark Roles Data/Custom Tags";
    public static string Tags = @"./Dark Roles Data/Custom Tags/Tags.txt";
    public static Dictionary<string, string> Tag = [];
    public static Dictionary<string, string> color1 = [], color2 = [];
    public static void CreateDirectory(string path) => Directory.CreateDirectory(path);
    public static void FileCheck(string path)
    {
        switch (Directory.Exists(path))
        {
            case true:
                if (!File.Exists(path))
                    File.Create(path);
                break;
            case false:
                CreateDirectory(Dir);
                break;
        }
    }

    public static bool ReadTags(string name)
    {
        if (name is "") return false;
        FileCheck(Tags);
        var players = File.ReadAllLines(Tags);
        if (players.Any(tag => tag.Contains(name)))
        {
            using StreamReader reader = new(Tags);
            string[] split = reader.ReadToEnd().Split(":");
            Tag[split[0]] = split[1];
            color1[split[0]] = split[2];
            color2[split[0]] = split[3];
            return true;
        }
        return false;
    }
}
