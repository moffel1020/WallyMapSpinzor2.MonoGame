using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WallyMapSpinzor2.MonoGame;

public class MonoGameCanvas : ICanvas<Texture2DWrapper>
{
    private Texture2D? _pixelTexture = null;
    private BasicEffect? _lineShader = null;

    public string BrawlPath{get; set;}
    public SpriteBatch Batch{get; set;}
    public BucketPriorityQueue<Action> DrawingQueue{get; set;} = new(Enum.GetValues<DrawPriorityEnum>().Length);
    public Dictionary<string, Texture2DWrapper> TextureCache{get;} = new();
    public MonoGameCanvas(GraphicsDevice graphicsDevice, string brawlPath)
    {
        BrawlPath = brawlPath;
        Batch = new(graphicsDevice);
    }
    
    public static Matrix TransformToMatrix(Transform t) => new(
        (float)t.ScaleX, (float)t.SkewY, 0, 0,
        (float)t.SkewX, (float)t.ScaleY, 0, 0,
        0, 0, 1, 0,
        (float)t.TranslateX, (float)t.TranslateY, 0, 1
    );

    private static Texture2D Create1PixelTexture(GraphicsDevice gd)
    {
        Texture2D tpx = new(gd, 1, 1);
        tpx.SetData(new[]{Microsoft.Xna.Framework.Color.White});
        return tpx;
    }

    private static BasicEffect CreateLineShader(GraphicsDevice gd) =>
        new(gd)
        {
            VertexColorEnabled = true,
            Projection = Matrix.CreateOrthographicOffCenter
                (0, gd.Viewport.Width,
                gd.Viewport.Height, 0,
                0, 1)
        };

    public static Microsoft.Xna.Framework.Color ToXnaColor(Color c) => new(c.R/255f,c.G/255f,c.B/255f,c.A/255f);

    public void DrawCircle(double X, double Y, double R, Color c, Transform t, DrawPriorityEnum p)
    {

    }

    public void DrawLineMultiColor(double X1, double Y1, double X2, double Y2, Color[] cs, Transform t, DrawPriorityEnum p)
    {
        
    }

    public void DrawLine(double X1, double Y1, double X2, double Y2, Color c, Transform t, DrawPriorityEnum p)
    {
        //transform line points
        (X1, Y1) = t * new Position(X1, Y1);
        (X2, Y2) = t * new Position(X2, Y2);
        //create vertex array
        VertexPositionColor[] vertices = new VertexPositionColor[]
        {
            new(new Vector3((float)X1,(float)Y1,0), ToXnaColor(c)),
            new(new Vector3((float)X2,(float)Y2,0), ToXnaColor(c))
        };

        DrawingQueue.Push(() =>
        {
            //create line shader
            _lineShader ??= CreateLineShader(Batch.GraphicsDevice);
            //draw line
            _lineShader.CurrentTechnique.Passes[0].Apply();
            Batch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
        }, (int)p);
    }

    public void DrawRect(double X, double Y, double W, double H, bool filled, Color c, Transform t, DrawPriorityEnum p)
    {
        Matrix m = TransformToMatrix(t);
        //filled. use 1 pixel texture.
        if(filled)
        {
            DrawingQueue.Push(() =>
            {
                _pixelTexture ??= Create1PixelTexture(Batch.GraphicsDevice);
                //draw
                Batch.Begin(blendState: BlendState.NonPremultiplied, transformMatrix: m);
                Batch.Draw(_pixelTexture, new Vector2((float)X,(float)Y), null, ToXnaColor(c), 0, Vector2.Zero, new Vector2((float)W,(float)H), SpriteEffects.None, 0);
                Batch.End();
            }, (int)p);
        }
        //outline. use line draws.
        else
        {
            //transform lines points
            (double X1, double Y1) = t * new Position(X, Y);
            (double X2, double Y2) = t * new Position(X+W, Y);
            (double X3, double Y3) = t * new Position(X+W, Y+H);
            (double X4, double Y4) = t * new Position(X, Y+H);
            (double X5, double Y5) = t * new Position(X, Y);
            //create vertex array
            VertexPositionColor[] vertices = new VertexPositionColor[]
            {
                new(new Vector3((float)X1,(float)Y1,0), ToXnaColor(c)),
                new(new Vector3((float)X2,(float)Y2,0), ToXnaColor(c)),
                new(new Vector3((float)X3,(float)Y3,0), ToXnaColor(c)),
                new(new Vector3((float)X4,(float)Y4,0), ToXnaColor(c)),
                new(new Vector3((float)X5,(float)Y5,0), ToXnaColor(c))
            };
            DrawingQueue.Push(() =>
            {
                //create line shader
                _lineShader ??= CreateLineShader(Batch.GraphicsDevice);
                //draw line
                _lineShader.CurrentTechnique.Passes[0].Apply();
                Batch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, vertices, 0, vertices.Length-1);
            }, (int)p);
        }
    }

    public void DrawString(double X, double Y, string text, double fontSize, Color c, Transform t, DrawPriorityEnum p)
    {
        
    }

    public void DrawTexture(double X, double Y, Texture2DWrapper texture, Transform t, DrawPriorityEnum p)
    {
        if(texture.Texture is null) return;
        Matrix m = TransformToMatrix(t);
        DrawingQueue.Push(() =>
        {
            Batch.Begin(blendState: BlendState.NonPremultiplied, rasterizerState: RasterizerState.CullNone, transformMatrix: m);
            Batch.Draw(texture.Texture, new Vector2((float)X, (float)Y), Microsoft.Xna.Framework.Color.White);
            Batch.End();
        }, (int)p);
    }

    public void DrawTextureRect(double X, double Y, double W, double H, Texture2DWrapper texture, Transform t, DrawPriorityEnum p)
    {
        if(texture.Texture is null) return;
        Matrix m = TransformToMatrix(t);
        DrawingQueue.Push(() =>
        {
            Batch.Begin(blendState: BlendState.NonPremultiplied, rasterizerState: RasterizerState.CullNone, transformMatrix: m);
            Batch.Draw(texture.Texture, new Rectangle((int)X, (int)Y, (int)W, (int)H), Microsoft.Xna.Framework.Color.White);
            Batch.End();
        }, (int)p);
    }

    public Texture2DWrapper LoadTextureFromPath(string path)
    {
        if(Batch is null) return new(null);
        string finalPath = Path.Join(BrawlPath, "mapArt", path).ToString();

        TextureCache.TryGetValue(finalPath, out Texture2DWrapper? texture);
        if(texture is not null) return texture;

        texture = new(Texture2D.FromFile(Batch.GraphicsDevice, finalPath));
        TextureCache.Add(finalPath, texture);
        return texture;
    }

    public void ClearTextureCache()
    {
        TextureCache.Clear();
    }

    public void ResizeLineShader()
    {
        if(_lineShader is null) return;

        _lineShader.Projection = Matrix.CreateOrthographicOffCenter
                (0, Batch.GraphicsDevice.Viewport.Width,
                Batch.GraphicsDevice.Viewport.Height, 0,
                0, 1);
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
            Action drawAction = DrawingQueue.PopMin();
            drawAction();
        }
    }
}