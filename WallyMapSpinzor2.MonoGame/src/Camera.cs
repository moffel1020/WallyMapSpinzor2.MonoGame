namespace WallyMapSpinzor2.MonoGame;

public class Camera
{
    public double X{get; set;}
    public double Y{get; set;}
    public double Zoom{get; set;} = 1;

    public Camera(double x = 0, double y = 0, double zoom = 1)
    {
        X = x;
        Y = y;
        Zoom = zoom;
    }

    public Transform ToTransform() => Transform.CreateScale(Zoom, Zoom) * Transform.CreateTranslate(X, Y);
}
