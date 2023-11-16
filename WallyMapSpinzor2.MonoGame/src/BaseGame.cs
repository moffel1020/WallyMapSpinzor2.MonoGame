using System.Drawing;
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
    }

    protected override void Draw(GameTime gameTime)
    {
        Canvas ??= new(GraphicsDevice, BrawlPath);
        GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
        ToDraw.DrawOn(Canvas, new GlobalRenderData(), new RenderSettings(), Transform.IDENTITY, 0f/16);
        Canvas.FinalizeDraw();
        base.Draw(gameTime);
    }
}