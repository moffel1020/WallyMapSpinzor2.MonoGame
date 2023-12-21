using Microsoft.Xna.Framework.Input;

namespace WallyMapSpinzor2.MonoGame;

public static class Input
{
    private static KeyboardState? _previousKeyState;
    private static KeyboardState? _currentKeyState;
    private static MouseState? _previousMouseState;
    private static MouseState? _currentMouseState;

    public static void Update()
    {
        _previousKeyState = _currentKeyState;
        _currentKeyState = Keyboard.GetState();
        
        _previousMouseState = _currentMouseState;
        _currentMouseState = Mouse.GetState();
    }

    public static bool IsKeyDown(Keys key) => _currentKeyState?.IsKeyDown(key) ?? false;
    public static bool IsKeyUp(Keys key) => _currentKeyState?.IsKeyUp(key) ?? false;
    public static bool IsKeyPressed(Keys key) => (_currentKeyState?.IsKeyDown(key) ?? false) && !(_previousKeyState?.IsKeyDown(key) ?? false);
    public static bool IsKeyReleased(Keys key) => !(_currentKeyState?.IsKeyDown(key) ?? false) && (_previousKeyState?.IsKeyDown(key) ?? false);

    public static bool IsMouseDown(MouseButtonEnum button) => button switch
    {
        MouseButtonEnum.Left => _currentMouseState?.LeftButton == ButtonState.Pressed,
        MouseButtonEnum.Middle => _currentMouseState?.MiddleButton == ButtonState.Pressed,
        MouseButtonEnum.Right => _currentMouseState?.RightButton == ButtonState.Pressed,
        _ => false,
    };

    public static bool IsMouseUp(MouseButtonEnum button) => button switch
    {
        MouseButtonEnum.Left => _currentMouseState?.LeftButton == ButtonState.Released,
        MouseButtonEnum.Middle => _currentMouseState?.MiddleButton == ButtonState.Released,
        MouseButtonEnum.Right => _currentMouseState?.RightButton == ButtonState.Released,
        _ => false,
    };

    public static bool IsMousePressed(MouseButtonEnum button) => button switch
    {
        MouseButtonEnum.Left => _currentMouseState?.LeftButton == ButtonState.Pressed && _previousMouseState?.LeftButton != ButtonState.Pressed,
        MouseButtonEnum.Middle => _currentMouseState?.MiddleButton == ButtonState.Pressed && _previousMouseState?.MiddleButton != ButtonState.Pressed,
        MouseButtonEnum.Right => _currentMouseState?.RightButton == ButtonState.Pressed && _previousMouseState?.RightButton != ButtonState.Pressed,
        _ => false,
    };

    public static bool IsMouseReleased(MouseButtonEnum button) => button switch
    {
        MouseButtonEnum.Left => _currentMouseState?.LeftButton == ButtonState.Released && _previousMouseState?.LeftButton != ButtonState.Released,
        MouseButtonEnum.Middle => _currentMouseState?.MiddleButton == ButtonState.Released && _previousMouseState?.MiddleButton != ButtonState.Released,
        MouseButtonEnum.Right => _currentMouseState?.RightButton == ButtonState.Released && _previousMouseState?.RightButton != ButtonState.Released,
        _ => false,
    };

    public static Microsoft.Xna.Framework.Point GetMouseDelta() => (_currentMouseState?.Position - _previousMouseState?.Position) ?? new(0, 0);
    public static int GetScrollWheelDelta() => (_currentMouseState?.ScrollWheelValue - _previousMouseState?.ScrollWheelValue) ?? 0;
}
