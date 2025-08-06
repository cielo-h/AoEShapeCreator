using C.ImGuiGLFW.Common;
using Hexa.NET.ImGui;
using System.Numerics;

namespace AoEShapeCreator.Windows;

internal partial class MainWindow : Window
{
    private readonly Settings _settings = Settings.Get();
    private readonly List<Action> postDraw = [];

    public MainWindow()
    {
        Flags = ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDecoration;
        SizeCondition = ImGuiCond.Always;
    }

    public override void PreDraw()
    {
        var size = ImGui.GetMainViewport();
        WindowSize = size.WorkSize;
        ImGui.SetNextWindowPos(size.WorkPos);
    }

    public override void PostDraw()
    {
        if (postDraw.Count > 0)
        {
            foreach (Action action in postDraw)
                action();
            postDraw.Clear();
        }
    }

    public override void OnDispose() => _settings.Write(false);

    protected override void Draw() => GeneralTab();
}
