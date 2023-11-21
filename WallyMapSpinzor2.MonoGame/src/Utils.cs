namespace WallyMapSpinzor2.MonoGame;

public static class Utils
{   
    public static Microsoft.Xna.Framework.Matrix TransformToMatrix(WallyMapSpinzor2.Transform t) => new(
        (float)t.ScaleX, (float)t.SkewY, 0, 0,
        (float)t.SkewX, (float)t.ScaleY, 0, 0,
        0, 0, 1, 0,
        (float)t.TranslateX, (float)t.TranslateY, 0, 1
    );

    public static Microsoft.Xna.Framework.Color ToXnaColor(WallyMapSpinzor2.Color c) => new(c.R/255f,c.G/255f,c.B/255f,c.A/255f);

}