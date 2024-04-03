using System.IO;
using DarkRoles.Modules.Customs.Json;

namespace DarkRoles.Modules.Customs;
public static class CustomTags
{
    public static void CreateDirectory(string path) => Directory.CreateDirectory(path);

    public static bool DoesPlayerHaveTags(string name)
    {
        var hasTags = false;
        hasTags = JsonReader.ReadExternalJson(@"./Dark Roles Data/Custom Tags/Tags.json", name) is not null;
        return hasTags;
    }

    public static string GetPlayerTags(string name)
    {
        var path = @"./Dark Roles Data/Custom Tags/Tags.json";
        return Utils.GradientColorText(JsonReader.ReadExternalMultiLineJson(path, name, "color1"), JsonReader.ReadExternalMultiLineJson(path, name, "color2"), JsonReader.ReadExternalMultiLineJson(path, name, "tag"));
    }
}