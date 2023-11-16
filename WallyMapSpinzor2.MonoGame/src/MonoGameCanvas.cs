using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WallyMapSpinzor2.MonoGame;

public class MonoGameCanvas : ICanvas<Texture2DWrapper>
{
    public static readonly Matrix CameraTransform = Matrix.CreateScale(0.1f) * Matrix.CreateTranslation(100, 100, 0);
    public string BrawlPath{get; set;}
    public SpriteBatch Batch{get; set;}
    public BucketPriorityQueue<Action<SpriteBatch>> DrawingQueue{get; set;} = new(Enum.GetValues<DrawPriorityEnum>().Length);
    public MonoGameCanvas(GraphicsDevice graphicsDevice, string brawlPath)
    {
        BrawlPath = brawlPath;
        Batch = new(graphicsDevice);
    }

    public void DrawCircle(double X, double Y, double R, Color c, Transform t, DrawPriorityEnum p)
    {
        
    }

    public void DrawDualColorLine(double X1, double Y1, double X2, double Y2, Color c1, Color c2, Transform t, DrawPriorityEnum p)
    {
        
    }

    public void DrawLine(double X1, double Y1, double X2, double Y2, Color c, Transform t, DrawPriorityEnum p)
    {
        
    }

    public void DrawRect(double X, double Y, double W, double H, bool filled, Color c, Transform t, DrawPriorityEnum p)
    {
        
    }

    public void DrawString(double X, double Y, string text, double fontSize, Color c, Transform t, DrawPriorityEnum p)
    {
        
    }

    public void DrawTexture(double X, double Y, Texture2DWrapper texture, Transform t, DrawPriorityEnum p)
    {
        if(texture.Texture is null) return;
        DrawingQueue.Push((SpriteBatch sb) =>
        {
            sb.Begin(blendState: BlendState.NonPremultiplied, transformMatrix: TransformToMatrix(t) * CameraTransform);
            sb.Draw(texture.Texture, new Vector2((float)X, (float)Y), Microsoft.Xna.Framework.Color.White);
            sb.End();
        }, (int)p);
    }

    public static Matrix TransformToMatrix(Transform t) => new(
        (float)t.ScaleX, (float)t.SkewY, 0, 0,
        (float)t.SkewX, (float)t.ScaleY, 0, 0,
        0, 0, 1, 0,
        (float)t.TranslateX, (float)t.TranslateY, 0, 1
    );

    public void DrawTextureRect(double X, double Y, double W, double H, Texture2DWrapper texture, Transform t, DrawPriorityEnum p)
    {
        if(texture.Texture is null) return;
        DrawingQueue.Push((SpriteBatch sb) =>
        {
            sb.Begin(blendState: BlendState.NonPremultiplied, transformMatrix: TransformToMatrix(t) * CameraTransform);
            sb.Draw(texture.Texture, new Rectangle((int)X, (int)Y, (int)W, (int)H), Microsoft.Xna.Framework.Color.White);
            sb.End();
        }, (int)p);
    }

    public Texture2DWrapper LoadTextureFromPath(string path)
    {
        if(Batch is null) return new(null);
        string finalPath = Path.Join(BrawlPath, "mapArt", path).ToString();
        return new(Texture2D.FromFile(Batch.GraphicsDevice, finalPath));
    }

    public Texture2DWrapper LoadTextureFromSWF(string filePath, string name)
    {
        return new(null);
    }

    public void FinalizeDraw()
    {
        if(Batch is null) return;
        while(DrawingQueue.Count > 0)
        {
            Action<SpriteBatch> drawAction = DrawingQueue.PopMin();
            drawAction(Batch);
        }
    }
}