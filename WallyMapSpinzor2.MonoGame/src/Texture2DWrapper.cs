using Microsoft.Xna.Framework.Graphics;

namespace WallyMapSpinzor2.MonoGame;

public class Texture2DWrapper : ITexture
{
    public Texture2D? Texture{get; set;}

    public Texture2DWrapper(Texture2D? texture)
    {
        Texture = texture;
    }

    ~Texture2DWrapper()
    {
        Texture?.Dispose();
    }

    public int W => Texture?.Width ?? 0;

    public int H => Texture?.Height ?? 0;
}