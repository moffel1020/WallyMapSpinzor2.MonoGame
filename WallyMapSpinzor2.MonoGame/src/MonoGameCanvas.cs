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

    public static int DefaultSubdiv(double R) => Math.Max(8, (int)Math.Ceiling(R / 1.5));

    public void DrawCircle(double x, double y, double radius, Color color, Transform trans, DrawPriorityEnum priority)
    {
        int subdiv = DefaultSubdiv(radius);

        VertexPositionColor[] vertices =
            Enumerable.Range(0, subdiv)
            .Select(i =>
            {
                (double sliceY, double sliceX) = Math.SinCos(Math.Tau * i / subdiv);
                return new VertexPositionColor(new Vector3((float)(x + radius * sliceX), (float)(y + radius * sliceY), 0), Utils.ToXnaColor(color));
            })
            .Prepend(new(new Vector3((float)x,(float)y,0), Utils.ToXnaColor(color)))
            .ToArray();

        short[] indices = new short[subdiv * 3];
        for(int i = 0; i < subdiv; ++i)
        {
            indices[3 * i] = 0;
            indices[3 * i + 1] = (short)(i + 1);
            indices[3 * i + 2] = (short)(i + 2);
        }
        indices[^1] = 1;

        DrawingQueue.Push(() =>
        {
            _lineShader ??= CreateLineShader(Batch.GraphicsDevice);
            _lineShader.World = Utils.TransformToMatrix(trans);
            foreach(EffectPass pass in _lineShader.CurrentTechnique.Passes)
            {
                pass.Apply();
                Batch.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices.ToArray(), 0, subdiv+1, indices, 0, subdiv);
            }
            _lineShader.World = Matrix.Identity;
        }, (int)priority);
    }

    public void DrawLine(double x1, double y1, double x2, double y2, Color color, Transform trans, DrawPriorityEnum priority)
    {
        //create vertex array
        VertexPositionColor[] vertices = new VertexPositionColor[]
        {
            new(new Vector3((float)x1, (float)y1, 0), Utils.ToXnaColor(color)),
            new(new Vector3((float)x2, (float)y2, 0), Utils.ToXnaColor(color))
        };

        DrawingQueue.Push(() =>
        {
            _lineShader ??= CreateLineShader(Batch.GraphicsDevice);
            _lineShader.World = Utils.TransformToMatrix(trans);
            foreach(EffectPass pass in _lineShader.CurrentTechnique.Passes)
            {
                pass.Apply();
                Batch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, vertices, 0, 1);
            }
            _lineShader.World = Matrix.Identity;
        }, (int)priority);
    }

    //we apply the camera transform to the line, and then offset the extra colored lines un-transformed
    //this keeps it looking good at any zoom
    public const double MULTI_COLOR_LINE_OFFSET = 1.0;
    public void DrawLineMultiColor(double x1, double y1, double x2, double y2, Color[] colors, Transform trans, DrawPriorityEnum priority)
    {
        (x1, y1) = trans * new Position(x1, y1);
        (x2, y2) = trans * new Position(x2, y2);
        if(x1 > x2)
        {
            (x1, x2) = (x2, x1);
            (y1, y2) = (y2, y1);
        }
        double center = (colors.Length - 1) / 2.0;
        (double offX, double offY) = (y1 - y2, x2 - x1);
        (offX, offY) = BrawlhallaMath.Normalize(offX, offY);
        for(int i = 0; i < colors.Length; ++i)
        {
            double mult = MULTI_COLOR_LINE_OFFSET * (i - center);
            DrawLine(x1 + offX * mult, y1 + offY * mult, x2 + offX * mult, y2 + offY * mult, colors[i], Transform.IDENTITY, priority);
        }
    }

    public void DrawRect(double x, double y, double w, double h, bool filled, Color color, Transform trans, DrawPriorityEnum priority)
    {
        Matrix m = Utils.TransformToMatrix(trans);
        //filled. use 1 pixel texture.
        if(filled)
        {
            DrawingQueue.Push(() =>
            {
                _pixelTexture ??= Create1PixelTexture(Batch.GraphicsDevice);
                //draw
                Batch.Begin(blendState: BlendState.NonPremultiplied, transformMatrix: m);
                Batch.Draw(_pixelTexture, new Vector2((float)x, (float)y), null, Utils.ToXnaColor(color), 0, Vector2.Zero, new Vector2((float)w, (float)h), SpriteEffects.None, 0);
                Batch.End();
            }, (int)priority);
        }
        //outline. use line draws.
        else
        {
            //create vertex array
            VertexPositionColor[] vertices = new VertexPositionColor[]
            {
                new(new Vector3((float)(x + 0), (float)(y + 0), 0), Utils.ToXnaColor(color)),
                new(new Vector3((float)(x + w), (float)(y + 0), 0), Utils.ToXnaColor(color)),
                new(new Vector3((float)(x + w), (float)(y + h), 0), Utils.ToXnaColor(color)),
                new(new Vector3((float)(x + 0), (float)(y + h), 0), Utils.ToXnaColor(color)),
                new(new Vector3((float)(x + 0), (float)(y + 0), 0), Utils.ToXnaColor(color))
            };
            DrawingQueue.Push(() =>
            {
                _lineShader ??= CreateLineShader(Batch.GraphicsDevice);
                _lineShader.World = m;
                foreach(EffectPass pass in _lineShader.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    Batch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, vertices, 0, vertices.Length - 1);
                }
                _lineShader.World = Matrix.Identity;
            }, (int)priority);
        }
    }

    public void DrawString(double x, double y, string text, double fontSize, Color color, Transform trans, DrawPriorityEnum priority)
    {
        
    }

    public void DrawTexture(double x, double y, Texture2DWrapper texture, Transform trans, DrawPriorityEnum priority)
    {
        if(texture.Texture is null) return;
        Matrix m = Utils.TransformToMatrix(trans);
        DrawingQueue.Push(() =>
        {
            Batch.Begin(blendState: BlendState.NonPremultiplied, rasterizerState: RasterizerState.CullNone, transformMatrix: m);
            Batch.Draw(texture.Texture, new Vector2((float)x, (float)y), Microsoft.Xna.Framework.Color.White);
            Batch.End();
        }, (int)priority);
    }

    public void DrawTextureRect(double x, double y, double w, double h, Texture2DWrapper texture, Transform trans, DrawPriorityEnum priority)
    {
        if(texture.Texture is null) return;
        Matrix m = Utils.TransformToMatrix(trans);
        DrawingQueue.Push(() =>
        {
            Batch.Begin(blendState: BlendState.NonPremultiplied, rasterizerState: RasterizerState.CullNone, transformMatrix: m);
            Batch.Draw(texture.Texture, new Rectangle((int)x, (int)y, (int)w, (int)h), Microsoft.Xna.Framework.Color.White);
            Batch.End();
        }, (int)priority);
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
