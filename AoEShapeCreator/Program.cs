using AoEShapeCreator.Windows;
using C.ImGuiGLFW;

internal class Program
{
    private static void Main()
    {
        ImGuiController.Initialize(nameof(AoEShapeCreator), 450, 450, false);
        ImGuiController.AddWindow(new MainWindow());
        ImGuiController.Run();
        Console.WriteLine($"{nameof(AoEShapeCreator)} has exited...");
    }
}