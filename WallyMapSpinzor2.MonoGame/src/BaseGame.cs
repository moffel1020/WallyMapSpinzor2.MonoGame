using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WallyMapSpinzor2.MonoGame;

public class BaseGame : Game
{
    private GraphicsDeviceManager _graphics;
    public string BrawlPath{get; set;}
    public MonoGameCanvas? Canvas{get; set;} = null;
    public IDrawable ToDraw{get; set;}

    public BaseGame(string brawlPath, IDrawable toDraw)
    {
        _graphics = new(this);
        ToDraw = toDraw;
        BrawlPath = brawlPath;
        IsMouseVisible = true;
    }

    protected override void Draw(GameTime gameTime)
    {
        Canvas ??= new(GraphicsDevice, BrawlPath);
        GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);

        Transform t = Transform.IDENTITY;
        if(ToDraw is LevelDesc ld)
        {
            double scale = Math.Min(GraphicsDevice.Viewport.Width/ld.CameraBounds.W, GraphicsDevice.Viewport.Height/ld.CameraBounds.H);
            t =
                Transform.CreateScale(scale, scale) *
                Transform.CreateTranslate(-ld.CameraBounds.X, -ld.CameraBounds.Y);
        }

        ToDraw.DrawOn(Canvas, new GlobalRenderData(), new RenderSettings(), t, 0f);
        Canvas.FinalizeDraw();
        base.Draw(gameTime);
    }
}