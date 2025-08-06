using AoEShapeCreator.Windows;
using C.ImGuiGLFW;

internal class Program
{
    private static void Main()
    {
        ImGuiController.Initialize(nameof(AoEShapeCreator), 450, 430);
        ImGuiController.AddWindow(new MainWindow());
        ImGuiController.Run();
        Console.WriteLine($"{nameof(AoEShapeCreator)} has exited...");
    }
}