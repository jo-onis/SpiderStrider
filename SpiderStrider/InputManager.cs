using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SpiderStrider
{
    public static class InputManager
    {
        private static GamePadState lastPadState = GamePad.GetState(PlayerIndex.One);
        private static KeyboardState lastKeyState = Keyboard.GetState();


        public static bool Pressed(Buttons button)
        {
            bool pressed = GamePad.GetState(PlayerIndex.One).IsButtonDown(button) && !lastPadState.IsButtonDown(button);
            return pressed;
        }

        public static bool IsDown(Buttons button)
        {
            return GamePad.GetState(PlayerIndex.One).IsButtonDown(button);
        }

        public static bool KeyPressed(Keys key)
        {
            bool pressed = Keyboard.GetState().IsKeyDown(key) && !lastKeyState.IsKeyDown(key);
            return pressed;
        }

        public static string GetPressedLetter()
        {
            var keys = Keyboard.GetState().GetPressedKeys();


            if (keys.Length > 0)
            {
                foreach(Keys key in keys)
                {
                    if (KeyPressed(key) && (IsKeyAChar(key) || IsKeyADigit(key)))
                    {
                        if (UpperCase()) return key.ToString().ToUpper();
                        else return key.ToString().ToLower();
                    }
                }

            }
            

            return "";
        }

        public static void UpdateLastState()
        {
            lastKeyState = Keyboard.GetState();
            lastPadState = GamePad.GetState(PlayerIndex.One);
        }

        internal static Vector2 GetMovementVector()
        {
            Vector2 movementVector = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left;

            if (movementVector == Vector2.Zero)
            {
                var state = Keyboard.GetState();
                if (state.IsKeyDown(Keys.Up) || state.IsKeyDown(Keys.W))
                    movementVector.Y += 1;
                if (state.IsKeyDown(Keys.Down) || state.IsKeyDown(Keys.S))
                    movementVector.Y -= 1;
                if (state.IsKeyDown(Keys.Left) || state.IsKeyDown(Keys.A))
                    movementVector.X -= 1;
                if (state.IsKeyDown(Keys.Right) || state.IsKeyDown(Keys.D))
                    movementVector.X += 1;
            }

            return movementVector;
        }

        public static bool UpperCase()
        {
            bool CAPSLOCK = Keyboard.GetState().CapsLock;
            bool SHIFT = Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift);

            return CAPSLOCK ? !SHIFT : SHIFT;
        }

        public static bool IsKeyAChar(Keys key)
        {
            return key >= Keys.A && key <= Keys.Z;
        }

        public static bool IsKeyADigit(Keys key)
        {
            return (key >= Keys.D0 && key <= Keys.D9) || (key >= Keys.NumPad0 && key <= Keys.NumPad9);
        }
    }
}