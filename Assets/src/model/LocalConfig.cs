using System.IO;

public class LocalConfig
{
#if UNITY_EDITOR

    public static string CONFIG_PATH = ".localconfig";

    public bool SkipStartCameraScene = false;
    public bool FakeLevelGeneration = false;
    public bool FastStart = false;

    public LocalConfig()
    {
        if (!File.Exists(CONFIG_PATH))
            return;

        var data = new JSONObject(File.ReadAllText(CONFIG_PATH));
        data.GetField(ref SkipStartCameraScene, "SkipStartCameraScene");
        data.GetField(ref FakeLevelGeneration, "FakeLevelGeneration");
        data.GetField(ref FastStart, "FastStart");
    }

#endif
}