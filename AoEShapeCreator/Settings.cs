using C.ImGuiGLFW.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO.Compression;
using System.Numerics;
using System.Text;

namespace AoEShapeCreator;

internal class Settings
{
    private readonly string FileLocation = Path.GetDirectoryName(AppContext.BaseDirectory) + "\\" + "config.gz";
    private static Settings? instance = null;
    public static Settings Get()
    {
        instance ??= new Settings();
        return instance;
    }

    private Settings()
    {
        if (!Read())
            Write();
    }

    public float ScaleFactor = 30f;
    public bool AddCenterDot = false;
    public float CenterDotRadius = 2;
    public Vector4 CenterDotColor = new(0, 0, 0, 1);
    public Vector4 Color = new(1f, 0.7058824f, 0.3647059f, 0.6509804f);
    public float OuterRadius = 10f;
    public float InnerRadius = 5f;
    public float CircleRadius = 10f;
    public float FanAngle = 60f;
    public float FanRadius = 30f;
    public float RectangleWidth = 10f;
    public float RectangleHeight = 10f;
    public float HollowRadius = 20f;
    public List<Vector4> ColorPreset = [];
    public ShapeType ShapeType = ShapeType.Circle;

    private bool Read()
    {
        try
        {
            var jsonStr = DecompressFileToString(FileLocation);
            var rootObj = JObject.Parse(jsonStr);

            ScaleFactor = rootObj.ContainsKey(nameof(ScaleFactor)) ? rootObj.Value<float>(nameof(ScaleFactor)) : 30f;
            AddCenterDot = !rootObj.ContainsKey(nameof(AddCenterDot)) || rootObj.Value<bool>(nameof(AddCenterDot));
            CenterDotRadius = rootObj.ContainsKey(nameof(CenterDotRadius)) ? rootObj.Value<float>(nameof(CenterDotRadius)) : 2f;
            CenterDotColor = rootObj[nameof(CenterDotColor)]?.ToObject<Vector4>() ?? new Vector4(0, 0, 0, 1);
            Color = rootObj[nameof(Color)]?.ToObject<Vector4>() ?? new(1f, 0.7058824f, 0.3647059f, 0.6509804f);
            OuterRadius = rootObj.ContainsKey(nameof(OuterRadius)) ? rootObj.Value<float>(nameof(OuterRadius)) : 10f;
            InnerRadius = rootObj.ContainsKey(nameof(InnerRadius)) ? rootObj.Value<float>(nameof(InnerRadius)) : 5f;
            CircleRadius = rootObj.ContainsKey(nameof(CircleRadius)) ? rootObj.Value<float>(nameof(CircleRadius)) : 10f;
            FanAngle = rootObj.ContainsKey(nameof(FanAngle)) ? rootObj.Value<float>(nameof(FanAngle)) : 60f;
            FanRadius = rootObj.ContainsKey(nameof(FanRadius)) ? rootObj.Value<float>(nameof(FanRadius)) : 30f;
            RectangleWidth = rootObj.ContainsKey(nameof(RectangleWidth)) ? rootObj.Value<float>(nameof(RectangleWidth)) : 10f;
            RectangleHeight = rootObj.ContainsKey(nameof(RectangleHeight)) ? rootObj.Value<float>(nameof(RectangleHeight)) : 10f;
            HollowRadius = rootObj.ContainsKey(nameof(HollowRadius)) ? rootObj.Value<float>(nameof(HollowRadius)) : 20f;
            ColorPreset = rootObj[nameof(ColorPreset)]?.ToObject<List<Vector4>>() ?? [];
            ShapeType = rootObj.ContainsKey(nameof(ShapeType)) ? (ShapeType)rootObj.Value<int>(nameof(ShapeType)) : ShapeType.Circle;

            InternalLog.Information("Configuration read successfully.");
        }
        catch (FileNotFoundException)
        {
            InternalLog.Error($"Settings file not found.\nCreate new settings to {FileLocation}");
            Write();
            return true;
        }
        catch (Exception ex2)
        {
            InternalLog.Error($"Could not read configuration file.\n{ex2.Message}");
            return false;
        }

        return true;
    }

    public void Reload() => Read();

    public void Write(bool outputMessage = true)
    {
        try
        {
            var rootObj = new JObject
            {
                [nameof(ScaleFactor)] = ScaleFactor,
                [nameof(AddCenterDot)] = AddCenterDot,
                [nameof(CenterDotRadius)] = CenterDotRadius,
                [nameof(OuterRadius)] = OuterRadius,
                [nameof(InnerRadius)] = InnerRadius,
                [nameof(CircleRadius)] = CircleRadius,
                [nameof(FanAngle)] = FanAngle,
                [nameof(FanRadius)] = FanRadius,
                [nameof(RectangleWidth)] = RectangleWidth,
                [nameof(RectangleHeight)] = RectangleHeight,
                [nameof(HollowRadius)] = HollowRadius,
                [nameof(ShapeType)] = (int)ShapeType,
            };

            string centerDotColor = JsonConvert.SerializeObject(CenterDotColor);
            rootObj[nameof(CenterDotColor)] = JToken.Parse(centerDotColor);

            string color = JsonConvert.SerializeObject(Color);
            rootObj[nameof(Color)] = JToken.Parse(color);

            string colorPreset = JsonConvert.SerializeObject(ColorPreset);
            rootObj[nameof(ColorPreset)] = JToken.Parse(colorPreset);

            CompressStringToFile(FileLocation, rootObj.ToString());

            if (outputMessage) InternalLog.Information("Configuration saved successfully.");
        }
        catch (Exception ex)
        {
            InternalLog.Error($"Could not write configuration file.\n{ex.Message}");
        }
    }

    #region Util

    private static void CompressStringToFile(string filePath, string inputString)
    {
        using FileStream fileStream = File.Create(filePath);
        using GZipStream gzipStream = new(fileStream, CompressionMode.Compress);
        byte[] bytes = Encoding.UTF8.GetBytes(inputString);
        gzipStream.Write(bytes, 0, bytes.Length);
    }

    private static string DecompressFileToString(string filePath)
    {
        using FileStream fileStream = File.OpenRead(filePath);
        using GZipStream gzipStream = new(fileStream, CompressionMode.Decompress);
        using MemoryStream memoryStream = new();
        byte[] buffer = new byte[4096];
        int count;
        while ((count = gzipStream.Read(buffer, 0, buffer.Length)) != 0)
        {
            memoryStream.Write(buffer, 0, count);
        }
        return Encoding.UTF8.GetString(memoryStream.ToArray());
    }

    #endregion
}