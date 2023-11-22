using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace WallyMapSpinzor2.MonoGame;

public class BaseGame : Game
{
    private GraphicsDeviceManager _graphics;
    public string BrawlPath{get; set;}
    public MonoGameCanvas? Canvas{get; set;} = null;
    public IDrawable ToDraw{get; set;}
    public Camera? Cam{get; set;}
    private double _windowScale = 1;

    public BaseGame(string brawlPath, IDrawable toDraw)
    {
        _graphics = new(this);
        ToDraw = toDraw;
        BrawlPath = brawlPath;
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
        Window.Title = "WallyMapSpinzor2.MonoGame";
        Window.ClientSizeChanged += OnWindowResize;
    }

    private void OnWindowResize(object? sender, EventArgs e)
    {
        Canvas?.ResizeLineShader();
    }

    protected override void Update(GameTime gameTime)
    {
        Input.Update();

        if(Cam is not null) 
        {
            Cam.Zoom += 0.00005 * Input.GetScrollWheelDelta() * gameTime.ElapsedGameTime.Milliseconds;
            Cam.Zoom = Math.Clamp(Cam.Zoom, 0.1, 7);

            if (Input.IsMouseDown(MouseButton.Right))
            {
                Input.GetMouseDelta().Deconstruct(out int x, out int y);
                Position delta = Position.ZERO with {X = x, Y = y};

                delta = Transform.CreateScale(1/(Cam.Zoom * _windowScale), 1/(Cam.Zoom * _windowScale)) * delta;
                Cam.X += delta.X;
                Cam.Y += delta.Y;
            }
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        Canvas ??= new(GraphicsDevice, BrawlPath);
        GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);

        Transform t = Transform.IDENTITY;
        if(ToDraw is LevelDesc ld)
        {
            _windowScale = Math.Min(GraphicsDevice.Viewport.Width/ld.CameraBounds.W, GraphicsDevice.Viewport.Height/ld.CameraBounds.H);

            // initialize inside camerabounds
            Cam ??= new(-ld.CameraBounds.X - GraphicsDevice.Viewport.Width/(2 * _windowScale),
                -ld.CameraBounds.Y - GraphicsDevice.Viewport.Height/(2 * _windowScale));

            t = Transform.CreateTranslate(GraphicsDevice.Viewport.Width/2, GraphicsDevice.Viewport.Height/2) * // set center to x=0, y=0 for scaling
                Transform.CreateScale(_windowScale, _windowScale) *
                Cam.ToTransform();
        }

        ToDraw.DrawOn(Canvas, new GlobalRenderData(), new RenderSettings(), t, 60.0 * gameTime.TotalGameTime.Ticks / TimeSpan.TicksPerSecond);
        Canvas.FinalizeDraw();
        base.Draw(gameTime);
    }
}