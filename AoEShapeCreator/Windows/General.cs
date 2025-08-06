using AoEShapeCreator.Helpers;
using C.ImGuiGLFW;
using C.ImGuiGLFW.Common;
using C.ImGuiGLFW.ImGuiMethods;
using C.ImGuiGLFW.Logging;
using C.ImGuiGLFW.Text.FontAwesome;
using Hexa.NET.ImGui;
using NativeFileDialog.Extended;
using System.Numerics;

namespace AoEShapeCreator.Windows;

internal partial class MainWindow : Window
{
    private static readonly Dictionary<ShapeType, string> names = new()
    {
        { ShapeType.Circle, "円" },
        { ShapeType.Annulus, "円環" },
        { ShapeType.Fan, "扇形" },
        { ShapeType.Rectangle, "四角形" },
        { ShapeType.AnnularSector, "環状扇形" },
    };
    private void GeneralTab()
    {
        ImGui.SetNextItemWidth(100f);
        ImGuiC.EnumCombo("タイプ", ref _settings.ShapeType, names: names);

        switch (_settings.ShapeType)
        {
            case ShapeType.Circle:
                Circle();
                break;
            case ShapeType.Annulus:
                Annulus();
                break;
            case ShapeType.Fan:
                Fan();
                break;
            case ShapeType.Rectangle:
                Rectangle();
                break;
            case ShapeType.AnnularSector:
                AnnularSector();
                break;
            default: break;
        }
        ImGui.Checkbox("中心に点を追加", ref _settings.AddCenterDot);
        ImGui.SetNextItemWidth(100f);
        ImGui.InputFloat("中心点の半径", ref _settings.CenterDotRadius);
        ImGui.PushItemWidth(200f);
        ImGui.ColorEdit4("色", ref _settings.Color);
        ImGui.SameLine();
        if (ImGui.Button("プリセットに追加##GeneralColor"))
        {
            if (!_settings.ColorPreset.Contains(_settings.Color)) _settings.ColorPreset.Add(_settings.Color);
        }
        var color = ColorPresetCombo("GeneralColor");
        if (color.HasValue) _settings.Color = color.Value;
        ImGui.ColorEdit4("中心点の色", ref _settings.CenterDotColor);
        ImGui.SameLine();
        if (ImGui.Button("プリセットに追加##CenterDotColor"))
        {
            if (!_settings.ColorPreset.Contains(_settings.CenterDotColor)) _settings.ColorPreset.Add(_settings.CenterDotColor);
        }
        var color2 = ColorPresetCombo("CenterDotColor");
        if (color2.HasValue) _settings.CenterDotColor = color2.Value;
        ImGui.PopItemWidth();
        ImGui.SetNextItemWidth(100f);
        ImGui.InputFloat("縮尺", ref _settings.ScaleFactor);
        CreateButton();
    }

    private void Circle()
    {
        ImGui.PushItemWidth(100f);
        ImGui.InputFloat("半径", ref _settings.CircleRadius);
        ImGui.PopItemWidth();
    }

    private void Annulus()
    {
        ImGui.PushItemWidth(100f);
        ImGui.InputFloat($"内径", ref _settings.InnerRadius);
        ImGui.InputFloat($"外径", ref _settings.OuterRadius);
        ImGui.PopItemWidth();
    }

    private void Fan()
    {
        ImGui.PushItemWidth(100f);
        ImGui.InputFloat("半径", ref _settings.FanRadius);
        ImGui.InputFloat("角度", ref _settings.FanAngle);
        ImGui.PopItemWidth();
    }

    private void Rectangle()
    {
        ImGui.PushItemWidth(100f);
        ImGui.InputFloat("幅", ref _settings.RectangleWidth);
        ImGui.InputFloat("高さ", ref _settings.RectangleHeight);
        ImGui.PopItemWidth();
    }

    private void AnnularSector()
    {
        ImGui.PushItemWidth(100f);
        ImGui.InputFloat("内径", ref _settings.HollowRadius);
        ImGui.InputFloat("外径", ref _settings.FanRadius);
        ImGui.InputFloat("角度", ref _settings.FanAngle);
        ImGui.PopItemWidth();
    }

    private string GetDefaultFileName() => _settings.ShapeType switch
    {
        ShapeType.Circle => $"{nameof(ShapeType.Circle)}_{_settings.CircleRadius}.png",
        ShapeType.Annulus => $"{nameof(ShapeType.Annulus)}_{_settings.InnerRadius}_{_settings.OuterRadius}.png",
        ShapeType.Fan => $"{nameof(ShapeType.Fan)}_{_settings.FanAngle}_{_settings.FanRadius}.png",
        ShapeType.Rectangle => $"{nameof(ShapeType.Rectangle)}_{_settings.RectangleWidth}_{_settings.RectangleHeight}.png",
        ShapeType.AnnularSector => $"{nameof(ShapeType.AnnularSector)}_{_settings.HollowRadius}_{_settings.FanRadius}_{_settings.FanAngle}.png",
        _ => string.Empty,
    };

    private static Vector2 selectableSize = new(120, 20);
    private Vector4? ColorPresetCombo(string label)
    {
        Vector4? ret = null;
        ImGui.SetNextItemWidth(150f);
        if (ImGui.BeginCombo($"##ColorPresetCombo-{label}", "色を選択"))
        {
            int i = 0;
            foreach (var color in _settings.ColorPreset)
            {
                var currentIndex = i;
                var c = color;
                ImGui.ColorEdit4($"##ColorEdit-{label}-{currentIndex}", ref c, ImGuiColorEditFlags.NoInputs);
                ImGui.SameLine();
                var num = ImGui.ColorConvertFloat4ToU32(c);
                if (ImGui.Selectable($"{num:X}", size: selectableSize)) ret = c;
                ImGui.BeginDisabled(!ImGuiC.KeyCtrl);
                ImGui.SameLine();
                if (ImGuiC.IconButton(FontAwesomeIcon.Trash.ToIconString(), $"Remove##Remove-{label}-{currentIndex}"))
                {
                    ImGuiController.AddPostdrawAction(() => _settings.ColorPreset.RemoveAt(currentIndex));
                }
                ImGui.EndDisabled();
                Tooltip("Ctrlを押しながらクリック", ImGuiHoveredFlags.AllowWhenDisabled);
                i++;
            }
            ImGui.EndCombo();
        }
        return ret;
    }

    private void CreateButton()
    {
        var name = names[_settings.ShapeType];
        if (ImGui.Button($"{name}を作成", ImGui.GetContentRegionAvail()))
        {
            var path = NFD.SaveDialog(string.Empty, GetDefaultFileName());
            if (string.IsNullOrEmpty(path))
            {
                InternalLog.Debug("Path is empty.");
                ImGuiController.FocusWindow();
                return;
            }
            try
            {
                switch (_settings.ShapeType)
                {
                    case ShapeType.Circle:
                        ShapeHelper.CreateCircle(path, _settings.CircleRadius * _settings.ScaleFactor, _settings.Color, _settings.AddCenterDot, _settings.CenterDotRadius, _settings.CenterDotColor);
                        break;
                    case ShapeType.Annulus:
                        ShapeHelper.CreateAnnulus(path, _settings.OuterRadius * _settings.ScaleFactor, _settings.InnerRadius * _settings.ScaleFactor, _settings.Color, _settings.AddCenterDot, _settings.CenterDotRadius, _settings.CenterDotColor);
                        break;
                    case ShapeType.Fan:
                        ShapeHelper.CreateFan(path, _settings.FanRadius * _settings.ScaleFactor, _settings.FanAngle, _settings.Color, _settings.AddCenterDot, _settings.CenterDotRadius, _settings.CenterDotColor);
                        break;
                    case ShapeType.Rectangle:
                        ShapeHelper.CreateRectangle(path, (int)(_settings.RectangleWidth * _settings.ScaleFactor), (int)(_settings.RectangleHeight * _settings.ScaleFactor), _settings.Color, _settings.AddCenterDot, _settings.CenterDotRadius, _settings.CenterDotColor);
                        break;
                    case ShapeType.AnnularSector:
                        ShapeHelper.CreateAnnularSector(path, _settings.FanRadius * _settings.ScaleFactor, _settings.FanAngle, _settings.HollowRadius * _settings.ScaleFactor, _settings.Color, _settings.AddCenterDot, _settings.CenterDotRadius, _settings.CenterDotColor);
                        break;
                    default: break;
                }
            }
            catch (Exception ex)
            {
                InternalLog.Debug($"{ex.Message} - {ex.StackTrace}");
                ModalHelper.ShowModal("Error", "Save Image failed.");
            }
            finally
            {
                ImGuiController.FocusWindow();
                ModalHelper.ShowModal("Success", "Save finished");
            }
        }
    }

    public static void Tooltip(string text, ImGuiHoveredFlags flags = ImGuiHoveredFlags.None)
    {
        if (ImGui.IsItemHovered(flags))
        {
            ImGuiC.SetTooltip(text);
        }
    }
}
