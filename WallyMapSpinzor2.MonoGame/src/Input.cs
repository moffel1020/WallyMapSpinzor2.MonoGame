using Microsoft.Xna.Framework.Input;

namespace WallyMapSpinzor2.MonoGame;

public enum MouseButton
{
    Left,
    Middle,
    Right
}

public static class Input
{
    private static KeyboardState _previousKeyState, _currentKeyState;
    private static MouseState _previousMouseState, _currentMouseState;

    public static void Update()
    {
        _previousKeyState = _currentKeyState;
        _currentKeyState = Keyboard.GetState();
        
        _previousMouseState = _currentMouseState;
        _currentMouseState = Mouse.GetState();
    }

    public static bool IsKeyDown(Keys key) => _currentKeyState.IsKeyDown(key);
    public static bool IsKeyUp(Keys key) => _currentKeyState.IsKeyUp(key);
    public static bool IsKeyPressed(Keys key) => _currentKeyState.IsKeyDown(key) && !_previousKeyState.IsKeyDown(key);
    public static bool IsKeyReleased(Keys key) => !_currentKeyState.IsKeyDown(key) && _previousKeyState.IsKeyDown(key);

    public static bool IsMouseDown(MouseButton button) => button switch
    {
        MouseButton.Left => _currentMouseState.LeftButton == ButtonState.Pressed,
        MouseButton.Middle => _currentMouseState.MiddleButton == ButtonState.Pressed,
        MouseButton.Right => _currentMouseState.RightButton == ButtonState.Pressed,
        _ => false,
    };

    public static bool IsMouseUp(MouseButton button) => button switch
    {
        MouseButton.Left => _currentMouseState.LeftButton == ButtonState.Released,
        MouseButton.Middle => _currentMouseState.MiddleButton == ButtonState.Released,
        MouseButton.Right => _currentMouseState.RightButton == ButtonState.Released,
        _ => false,
    };

    public static bool IsMousePressed(MouseButton button) => button switch
    {
        MouseButton.Left => _currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton != ButtonState.Pressed,
        MouseButton.Middle => _currentMouseState.MiddleButton == ButtonState.Pressed && _previousMouseState.MiddleButton != ButtonState.Pressed,
        MouseButton.Right => _currentMouseState.RightButton == ButtonState.Pressed && _previousMouseState.RightButton != ButtonState.Pressed,
        _ => false,
    };

    public static bool IsMouseReleased(MouseButton button) => button switch
    {
        MouseButton.Left => _currentMouseState.LeftButton == ButtonState.Released && _previousMouseState.LeftButton != ButtonState.Released,
        MouseButton.Middle => _currentMouseState.MiddleButton == ButtonState.Released && _previousMouseState.MiddleButton != ButtonState.Released,
        MouseButton.Right => _currentMouseState.RightButton == ButtonState.Released && _previousMouseState.RightButton != ButtonState.Released,
        _ => false,
    };

    public static Microsoft.Xna.Framework.Point GetMouseDelta() => _currentMouseState.Position - _previousMouseState.Position;
    public static int GetScrollWheelDelta() => _currentMouseState.ScrollWheelValue - _previousMouseState.ScrollWheelValue;
}