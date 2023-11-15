using Microsoft.Xna.Framework.Graphics;

namespace WallyMapSpinzor2.MonoGame;

public class MonoGameCanvas : ICanvas<Texture2DWrapper>
{
    public void DrawCircle(double X, double Y, double R, Color c, Transform t, DrawPriorityEnum p)
    {
        throw new NotImplementedException();
    }

    public void DrawDualColorLine(double X1, double Y1, double X2, double Y2, Color c1, Color c2, Transform t, DrawPriorityEnum p)
    {
        throw new NotImplementedException();
    }

    public void DrawLine(double X1, double Y1, double X2, double Y2, Color c, Transform t, DrawPriorityEnum p)
    {
        throw new NotImplementedException();
    }

    public void DrawRect(double X, double Y, double W, double H, bool filled, Color c, Transform t, DrawPriorityEnum p)
    {
        throw new NotImplementedException();
    }

    public void DrawString(double X, double Y, string text, double fontSize, Color c, Transform t, DrawPriorityEnum p)
    {
        throw new NotImplementedException();
    }

    public void DrawTexture(double X, double Y, Texture2DWrapper texture, Transform t, DrawPriorityEnum p)
    {
        throw new NotImplementedException();
    }

    public void DrawTextureRect(double X, double Y, double W, double H, Texture2DWrapper texture, Transform t, DrawPriorityEnum p)
    {
        throw new NotImplementedException();
    }

    public Texture2DWrapper LoadTextureFromPath(string path)
    {
        throw new NotImplementedException();
    }

    public Texture2DWrapper LoadTextureFromSWF(string filePath, string name)
    {
        throw new NotImplementedException();
    }
}