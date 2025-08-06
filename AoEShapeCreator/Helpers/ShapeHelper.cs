using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Numerics;

namespace AoEShapeCreator.Helpers;

internal static class ShapeHelper
{
    internal static void CreateAnnulus(string fileName, float outerRadius, float innerRadius,
        Vector4 annulusColor, bool showCenter, float centerRadius, Vector4 centerColor)
    {
        // 画像サイズを計算（外側半径の2倍）
        int imageSize = (int)(outerRadius * 2);
        float centerX = imageSize / 2f;
        float centerY = imageSize / 2f;
        using Bitmap bitmap = new(imageSize, imageSize);
        using Graphics graphics = Graphics.FromImage(bitmap);
        // アンチエイリアスを有効にする
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.CompositingQuality = CompositingQuality.HighQuality;
        // 背景を透明にする
        graphics.Clear(Color.Transparent);

        // Vector4からColorに変換（X=R, Y=G, Z=B, W=A）
        Color annulusDrawColor = Color.FromArgb(
            (int)(Math.Clamp(annulusColor.W, 0f, 1f) * 255),
            (int)(Math.Clamp(annulusColor.X, 0f, 1f) * 255),
            (int)(Math.Clamp(annulusColor.Y, 0f, 1f) * 255),
            (int)(Math.Clamp(annulusColor.Z, 0f, 1f) * 255));

        using (SolidBrush brush = new(annulusDrawColor))
        {
            // GraphicsPathを使用してアニュラスを作成
            using GraphicsPath path = new();
            // 外側の円を追加
            path.AddEllipse(centerX - outerRadius, centerY - outerRadius,
                outerRadius * 2, outerRadius * 2);
            // 内側の円を追加（逆方向で穴を開ける）
            path.AddEllipse(centerX - innerRadius, centerY - innerRadius,
                innerRadius * 2, innerRadius * 2);
            // FillModeをAlternateに設定してドーナツ形状を作成
            path.FillMode = FillMode.Alternate;
            // アニュラスを描画
            graphics.FillPath(brush, path);
        }

        // 中心点を描画
        if (showCenter)
        {
            Color centerDrawColor = Color.FromArgb(
                (int)(Math.Clamp(centerColor.W, 0f, 1f) * 255),
                (int)(Math.Clamp(centerColor.X, 0f, 1f) * 255),
                (int)(Math.Clamp(centerColor.Y, 0f, 1f) * 255),
                (int)(Math.Clamp(centerColor.Z, 0f, 1f) * 255));

            using SolidBrush centerBrush = new(centerDrawColor);
            graphics.FillEllipse(centerBrush,
                centerX - centerRadius, centerY - centerRadius,
                centerRadius * 2, centerRadius * 2);
        }

        // PNG形式で保存
        bitmap.Save(fileName, ImageFormat.Png);
    }

    internal static void CreateCircle(string fileName, float radius,
    Vector4 Color, bool showCenter, float centerRadius, Vector4 centerColor)
    {
        // 画像サイズを計算（外側半径の2倍）
        int imageSize = (int)(radius * 2);
        float centerX = imageSize / 2f;
        float centerY = imageSize / 2f;
        using Bitmap bitmap = new(imageSize, imageSize);
        using Graphics graphics = Graphics.FromImage(bitmap);
        // アンチエイリアスを有効にする
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.CompositingQuality = CompositingQuality.HighQuality;
        // 背景を透明にする
        graphics.Clear(System.Drawing.Color.Transparent);

        // Vector4からColorに変換（X=R, Y=G, Z=B, W=A）
        Color annulusDrawColor = System.Drawing.Color.FromArgb(
            (int)(Math.Clamp(Color.W, 0f, 1f) * 255),
            (int)(Math.Clamp(Color.X, 0f, 1f) * 255),
            (int)(Math.Clamp(Color.Y, 0f, 1f) * 255),
            (int)(Math.Clamp(Color.Z, 0f, 1f) * 255));

        using (SolidBrush brush = new(annulusDrawColor))
        {
            // GraphicsPathを使用してアニュラスを作成
            using GraphicsPath path = new();
            // 外側の円を追加
            path.AddEllipse(centerX - radius, centerY - radius,
                radius * 2, radius * 2);
            // アニュラスを描画
            graphics.FillPath(brush, path);
        }

        // 中心点を描画
        if (showCenter)
        {
            Color centerDrawColor = System.Drawing.Color.FromArgb(
                (int)(Math.Clamp(centerColor.W, 0f, 1f) * 255),
                (int)(Math.Clamp(centerColor.X, 0f, 1f) * 255),
                (int)(Math.Clamp(centerColor.Y, 0f, 1f) * 255),
                (int)(Math.Clamp(centerColor.Z, 0f, 1f) * 255));

            using SolidBrush centerBrush = new(centerDrawColor);
            graphics.FillEllipse(centerBrush,
                centerX - centerRadius, centerY - centerRadius,
                centerRadius * 2, centerRadius * 2);
        }

        // PNG形式で保存
        bitmap.Save(fileName, ImageFormat.Png);
    }

    internal static void CreateFan(string fileName, float radius, float angle, Vector4 Color, bool showCenter, float centerRadius, Vector4 centerColor)
    {
        // 角度を1-360の範囲にクランプ
        angle = Math.Clamp(angle, 1f, 360f);

        // 元の画像サイズを計算（外側半径の2倍）
        int originalSize = (int)(radius * 2);
        float centerX = originalSize / 2f;
        float centerY = originalSize / 2f;

        using Bitmap originalBitmap = new(originalSize, originalSize);
        using Graphics graphics = Graphics.FromImage(originalBitmap);

        // アンチエイリアスを有効にする
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.CompositingQuality = CompositingQuality.HighQuality;

        // 背景を透明にする
        graphics.Clear(System.Drawing.Color.Transparent);

        // Vector4からColorに変換（X=R, Y=G, Z=B, W=A）
        Color sectorDrawColor = System.Drawing.Color.FromArgb(
            (int)(Math.Clamp(Color.W, 0f, 1f) * 255),
            (int)(Math.Clamp(Color.X, 0f, 1f) * 255),
            (int)(Math.Clamp(Color.Y, 0f, 1f) * 255),
            (int)(Math.Clamp(Color.Z, 0f, 1f) * 255));

        using (SolidBrush brush = new(sectorDrawColor))
        {
            // 扇形を描画
            // 開始角度を-90度（12時方向）に設定
            float startAngle = -90f;
            // 扇形の領域を定義
            RectangleF rect = new(
                centerX - radius,
                centerY - radius,
                radius * 2,
                radius * 2);

            // 扇形を描画
            graphics.FillPie(brush, rect, startAngle, angle);
        }

        // 中心点を描画
        if (showCenter)
        {
            Color centerDrawColor = System.Drawing.Color.FromArgb(
                (int)(Math.Clamp(centerColor.W, 0f, 1f) * 255),
                (int)(Math.Clamp(centerColor.X, 0f, 1f) * 255),
                (int)(Math.Clamp(centerColor.Y, 0f, 1f) * 255),
                (int)(Math.Clamp(centerColor.Z, 0f, 1f) * 255));

            using SolidBrush centerBrush = new(centerDrawColor);
            graphics.FillEllipse(centerBrush,
                centerX - centerRadius, centerY - centerRadius,
                centerRadius * 2, centerRadius * 2);
        }

        // 画像をangle/2の角度だけ左に回転
        float rotationAngle = -(angle / 2f); // 左回転なのでマイナス
        using Bitmap rotatedBitmap = new(originalSize, originalSize);
        using Graphics rotatedGraphics = Graphics.FromImage(rotatedBitmap);

        // アンチエイリアスを有効にする
        rotatedGraphics.SmoothingMode = SmoothingMode.AntiAlias;
        rotatedGraphics.CompositingQuality = CompositingQuality.HighQuality;

        // 背景を透明にする
        rotatedGraphics.Clear(System.Drawing.Color.Transparent);

        // 回転の中心を画像の中央に設定
        rotatedGraphics.TranslateTransform(centerX, centerY);
        rotatedGraphics.RotateTransform(rotationAngle);
        rotatedGraphics.TranslateTransform(-centerX, -centerY);

        // 元の画像を回転して描画
        rotatedGraphics.DrawImage(originalBitmap, 0, 0);

        // 描画内容の境界を計算
        Rectangle bounds = CalculateFanBounds(centerX, centerY, radius, angle, rotationAngle, showCenter, centerRadius);

        // 境界領域で切り抜いた新しいビットマップを作成
        using Bitmap croppedBitmap = new(bounds.Width, bounds.Height);
        using Graphics croppedGraphics = Graphics.FromImage(croppedBitmap);

        // 元のビットマップから境界領域をコピー
        croppedGraphics.DrawImage(rotatedBitmap,
            new Rectangle(0, 0, bounds.Width, bounds.Height),
            bounds,
            GraphicsUnit.Pixel);

        // PNG形式で保存
        croppedBitmap.Save(fileName, ImageFormat.Png);
    }

    private static Rectangle CalculateFanBounds(float centerX, float centerY, float radius, float angle, float rotationAngle, bool showCenter, float centerRadius)
    {
        // 扇形の各頂点を計算
        List<PointF> points =
        [
            // 中心点
            new PointF(centerX, centerY),
    ];

        // 開始角度（-90度 = 12時方向）に回転角度を適用
        float startAngleRad = (float)((-90 + rotationAngle) * Math.PI / 180);
        float endAngleRad = (float)((angle - 90 + rotationAngle) * Math.PI / 180);

        // 開始点
        points.Add(new PointF(
            centerX + radius * (float)Math.Cos(startAngleRad),
            centerY + radius * (float)Math.Sin(startAngleRad)));

        // 終了点
        points.Add(new PointF(
            centerX + radius * (float)Math.Cos(endAngleRad),
            centerY + radius * (float)Math.Sin(endAngleRad)));

        // 弧上の点を追加（より正確な境界計算のため）
        int arcPoints = Math.Max(1, (int)(angle / 10)); // 10度ごと
        for (int i = 1; i < arcPoints; i++)
        {
            float currentAngle = startAngleRad + (endAngleRad - startAngleRad) * i / arcPoints;
            points.Add(new PointF(
                centerX + radius * (float)Math.Cos(currentAngle),
                centerY + radius * (float)Math.Sin(currentAngle)));
        }

        // 中心円も考慮
        if (showCenter)
        {
            points.Add(new PointF(centerX - centerRadius, centerY - centerRadius));
            points.Add(new PointF(centerX + centerRadius, centerY - centerRadius));
            points.Add(new PointF(centerX - centerRadius, centerY + centerRadius));
            points.Add(new PointF(centerX + centerRadius, centerY + centerRadius));
        }

        // 最小・最大座標を計算
        float minX = points.Min(p => p.X);
        float maxX = points.Max(p => p.X);
        float minY = points.Min(p => p.Y);
        float maxY = points.Max(p => p.Y);

        // 小数点以下を考慮して少し余裕を持たせる
        int left = (int)Math.Floor(minX);
        int top = (int)Math.Floor(minY);
        int right = (int)Math.Ceiling(maxX);
        int bottom = (int)Math.Ceiling(maxY);

        // 境界が画像外に出ないようにクランプ
        int originalSize = (int)(Math.Max(centerX, centerY) * 2);
        left = Math.Max(0, left);
        top = Math.Max(0, top);
        right = Math.Min(originalSize, right);
        bottom = Math.Min(originalSize, bottom);

        return new Rectangle(left, top, right - left, bottom - top);
    }


    internal static void CreateRectangle(string fileName, float width, float height, Vector4 color, bool showCenter, float centerRadius, Vector4 centerColor)
    {
        // float値をintに変換（四捨五入または切り捨て）
        int intWidth = (int)Math.Round(Math.Max(1f, width));
        int intHeight = (int)Math.Round(Math.Max(1f, height));

        using Bitmap bitmap = new(intWidth, intHeight);
        using Graphics graphics = Graphics.FromImage(bitmap);

        // アンチエイリアスを有効にする
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.CompositingQuality = CompositingQuality.HighQuality;

        // 背景を透明にする
        graphics.Clear(Color.Transparent);

        // Vector4からColorに変換（X=R, Y=G, Z=B, W=A）
        // Vector4の値は0.0-1.0の範囲と仮定し、0-255にスケール
        Color rectangleColor = Color.FromArgb(
            (int)(Math.Clamp(color.W, 0f, 1f) * 255),
            (int)(Math.Clamp(color.X, 0f, 1f) * 255),
            (int)(Math.Clamp(color.Y, 0f, 1f) * 255),
            (int)(Math.Clamp(color.Z, 0f, 1f) * 255));

        using (SolidBrush brush = new(rectangleColor))
        {
            // 四角形を描画（画像全体を塗りつぶし）
            Rectangle rect = new(0, 0, intWidth, intHeight);
            graphics.FillRectangle(brush, rect);
        }

        // 中心点を描画
        if (showCenter)
        {
            // 中心座標を計算（元のfloat値を使用）
            float centerX = width / 2f;
            float centerY = height / 2f;

            Color centerDrawColor = Color.FromArgb(
                (int)(Math.Clamp(centerColor.W, 0f, 1f) * 255),
                (int)(Math.Clamp(centerColor.X, 0f, 1f) * 255),
                (int)(Math.Clamp(centerColor.Y, 0f, 1f) * 255),
                (int)(Math.Clamp(centerColor.Z, 0f, 1f) * 255));

            using SolidBrush centerBrush = new(centerDrawColor);
            graphics.FillEllipse(centerBrush,
                centerX - centerRadius, centerY - centerRadius,
                centerRadius * 2, centerRadius * 2);
        }

        // PNG形式で保存
        bitmap.Save(fileName, ImageFormat.Png);
    }

    internal static void CreateAnnularSector(string fileName, float radius, float angle, float hollowRadius, Vector4 Color, bool showCenter, float centerRadius, Vector4 centerColor)
    {
        // 角度を1-360の範囲にクランプ
        angle = Math.Clamp(angle, 1f, 360f);
        // くりぬき半径を0以上にクランプし、外側半径より小さくする
        hollowRadius = Math.Clamp(hollowRadius, 0f, radius - 1f);
        // 画像サイズを計算（外側半径の2倍）
        int imageSize = (int)(radius * 2);
        float centerX = imageSize / 2f;
        float centerY = imageSize / 2f;

        using Bitmap originalBitmap = new(imageSize, imageSize);
        using Graphics graphics = Graphics.FromImage(originalBitmap);

        // アンチエイリアスを有効にする
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.CompositingQuality = CompositingQuality.HighQuality;
        // 背景を透明にする
        graphics.Clear(System.Drawing.Color.Transparent);

        // 回転の設定（angle/2だけ左回転）
        float rotationAngle = -(angle / 2f); // 左回転なのでマイナス
        graphics.TranslateTransform(centerX, centerY);
        graphics.RotateTransform(rotationAngle);
        graphics.TranslateTransform(-centerX, -centerY);

        // Vector4からColorに変換（X=R, Y=G, Z=B, W=A）
        // Vector4の値は0.0-1.0の範囲と仮定し、0-255にスケール
        Color sectorDrawColor = System.Drawing.Color.FromArgb(
            (int)(Math.Clamp(Color.W, 0f, 1f) * 255),
            (int)(Math.Clamp(Color.X, 0f, 1f) * 255),
            (int)(Math.Clamp(Color.Y, 0f, 1f) * 255),
            (int)(Math.Clamp(Color.Z, 0f, 1f) * 255));

        // GraphicsPathを使用して環状扇形を作成
        using GraphicsPath path = new();
        // 外側の扇形を追加
        Rectangle outerRect = new(
            (int)(centerX - radius),
            (int)(centerY - radius),
            (int)(radius * 2),
            (int)(radius * 2));
        float startAngle = -90f;
        path.AddPie(outerRect, startAngle, angle);

        Rectangle innerRect = new(
            (int)(centerX - hollowRadius),
            (int)(centerY - hollowRadius),
            (int)(hollowRadius * 2),
            (int)(hollowRadius * 2));

        // 内側の扇形を作成
        using GraphicsPath innerPath = new();
        innerPath.AddPie(innerRect, startAngle, angle);

        // 内側の扇形を外側の扇形から除外
        Region region = new(path);
        region.Exclude(innerPath);

        // 領域を使用して描画
        using SolidBrush brush = new(sectorDrawColor);
        graphics.FillRegion(brush, region);

        // 中心点を描画（回転の影響を受けないように、Transform後に描画）
        if (showCenter)
        {
            Color centerDrawColor = System.Drawing.Color.FromArgb(
                (int)(Math.Clamp(centerColor.W, 0f, 1f) * 255),
                (int)(Math.Clamp(centerColor.X, 0f, 1f) * 255),
                (int)(Math.Clamp(centerColor.Y, 0f, 1f) * 255),
                (int)(Math.Clamp(centerColor.Z, 0f, 1f) * 255));
            using SolidBrush centerBrush = new(centerDrawColor);
            graphics.FillEllipse(centerBrush,
                centerX - centerRadius, centerY - centerRadius,
                centerRadius * 2, centerRadius * 2);
        }

        // 塗りつぶされた領域の境界を計算
        Rectangle bounds = CalculateContentBounds(originalBitmap);

        // 境界が見つからない場合（全て透明）は元の画像を保存
        if (bounds.IsEmpty)
        {
            originalBitmap.Save(fileName, ImageFormat.Png);
            return;
        }

        // クロップされた画像を作成
        using Bitmap croppedBitmap = new(bounds.Width, bounds.Height);
        using Graphics croppedGraphics = Graphics.FromImage(croppedBitmap);

        // 元の画像から境界領域をコピー
        croppedGraphics.DrawImage(originalBitmap,
            new Rectangle(0, 0, bounds.Width, bounds.Height),
            bounds,
            GraphicsUnit.Pixel);

        // PNG形式で保存
        croppedBitmap.Save(fileName, ImageFormat.Png);
    }

    private static Rectangle CalculateContentBounds(Bitmap bitmap)
    {
        int minX = bitmap.Width;
        int minY = bitmap.Height;
        int maxX = -1;
        int maxY = -1;

        // 全ピクセルをスキャンして非透明ピクセルの境界を見つける
        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                Color pixel = bitmap.GetPixel(x, y);
                // アルファ値が0より大きい（透明でない）ピクセルを検出
                if (pixel.A > 0)
                {
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;
                }
            }
        }

        // 非透明ピクセルが見つからない場合
        if (maxX == -1)
        {
            return Rectangle.Empty;
        }

        // 境界の矩形を返す
        return new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
    }
}
