﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Input
{
    public enum MouseButton
    {
        LEFT,
        MIDDLE,
        RIGHT
    }

    public class MouseButtonState
    {
        public MouseButton button;
        public bool pressed;
        public bool dragged;
        public int durationFrames;
        public Vector2 initialMousePos;

        public const float DRAG_THRESHOLD = 3.0f;
        public const float FRAMERATE = 1 / 30.0f;

        public MouseButtonState(MouseButton button, MouseState initialState)
        {
            this.button = button;
            pressed = IsButtonPressed(initialState);
            durationFrames = 0;
            initialMousePos = new Vector2(initialState.X, initialState.Y);
        }

        public void Update(MouseState state)
        {
            bool newPressed = IsButtonPressed(state);
            if( pressed != newPressed )
            {
                pressed = newPressed;
                durationFrames = 0;
                dragged = false;
                initialMousePos = new Vector2(state.X, state.Y);
            }
            else
            {
                durationFrames++;

                if (pressed && !dragged && (initialMousePos - new Vector2(state.X, state.Y)).LengthSquared() > DRAG_THRESHOLD * DRAG_THRESHOLD)
                {
                    dragged = true;
                }
            }
        }

        bool IsButtonPressed(MouseState state)
        {
            switch(button)
            {
                case MouseButton.LEFT:
                    return state.LeftButton == ButtonState.Pressed;
                case MouseButton.MIDDLE:
                    return state.MiddleButton == ButtonState.Pressed;
                case MouseButton.RIGHT:
                    return state.RightButton == ButtonState.Pressed;
            }
            return false;
        }

        public float duration {
            get { return durationFrames * FRAMERATE; }
        }
    }

    public class InputState
    {
        PlayerIndex gamePadIndex;
        MouseState oldMouse;
        public MouseState mouse { get; internal set; }
        GamePadState oldGamePad;
        public GamePadState gamePad { get; internal set; }
        KeyboardState oldKeyboard;
        public KeyboardState keyboard { get; internal set; }
        public bool pauseMouse { get; private set; }

        public MouseButtonState mouseLeft;
        public MouseButtonState mouseMiddle;
        public MouseButtonState mouseRight;

        public void Update()
        {
            oldKeyboard = keyboard;
            keyboard = Keyboard.GetState();
            oldGamePad = gamePad;
            gamePad = GamePad.GetState(PlayerIndex.One);
            if (WasKeyJustPressed(Keys.Space))
            {
                pauseMouse = !pauseMouse;
            }
            else if (IsKeyDown(Keys.Space) && pauseMouse && (WasMouseLeftJustPressed() || WasMouseRightJustPressed()))
            {
                // force an update if the user clicks
                mouse = Mouse.GetState();
            }

            if (pauseMouse)
            {
                mouse = oldMouse;
            }
            else
            {
                oldMouse = mouse;
                mouse = Mouse.GetState();
            }

            if (mouseLeft != null)
            {
                mouseLeft.Update(mouse);
                mouseMiddle.Update(mouse);
                mouseRight.Update(mouse);
            }
            else
            {
                mouseLeft = new MouseButtonState(MouseButton.LEFT, mouse);
                mouseMiddle = new MouseButtonState(MouseButton.MIDDLE, mouse);
                mouseRight = new MouseButtonState(MouseButton.RIGHT, mouse);
            }
        }

        public Vector2 MousePos { get { return new Vector2(mouse.X, mouse.Y); } }

        public bool WasMouseLeftJustPressed()
        {
            return mouseLeft.pressed && mouseLeft.duration == 0;
        }

        public bool WasMouseLeftJustReleased()
        {
            return !mouseLeft.pressed && mouseLeft.duration == 0;
        }

        public bool WasMouseRightJustPressed()
        {
            return mouseRight.pressed && mouseRight.duration == 0;
        }

        public bool WasMouseRightJustReleased()
        {
            return !mouseRight.pressed && mouseRight.duration == 0;
        }

        public bool WasButtonJustPressed (Buttons button)
        {
            return gamePad.IsButtonDown(button) && !oldGamePad.IsButtonDown(button);
        }

        public bool WasButtonJustReleased (Buttons button)
        {
            return !gamePad.IsButtonDown(button) && oldGamePad.IsButtonDown(button);
        }

        public bool IsButtonDown (Buttons button)
        {
            return gamePad.IsButtonDown(button);
        }
        
        public bool IsButtonUp (Buttons button)
        {
            return gamePad.IsButtonUp(button);
        }

        public bool WasKeyJustPressed(Keys key)
        {
            return keyboard.IsKeyDown(key) && !oldKeyboard.IsKeyDown(key);
        }

        public bool WasKeyJustReleased(Keys key)
        {
            return !keyboard.IsKeyDown(key) && oldKeyboard.IsKeyDown(key);
        }

        public bool IsKeyDown(Keys key)
        {
            return keyboard.IsKeyDown(key);
        }

        public bool IsKeyUp(Keys key)
        {
            return keyboard.IsKeyUp(key);
        }

        public InputState (PlayerIndex playerIndex = PlayerIndex.One) { gamePadIndex = playerIndex; }

        public Vector2 GetPseudoJoystick (Vector2 thumbstick, Keys up, Keys down, Keys left, Keys right)
        {
            Vector2 vec = (thumbstick + GetPseudoJoystick(up, down, left, right));
            vec.X %= 2;
            vec.Y %= 2;
            return vec;
        }

        public Vector2 GetPseudoJoystick (Keys up, Keys down, Keys left, Keys right)
        {
            float upDown = 0.0f;
            if( keyboard.IsKeyDown(up) )
            {
                upDown = -1.0f;
            }
            else if( keyboard.IsKeyDown(down) )
            {
                upDown = 1.0f;
            }

            float leftRight = 0.0f;
            if (keyboard.IsKeyDown(left))
            {
                leftRight = -1.0f;
            }
            else if (keyboard.IsKeyDown(right))
            {
                leftRight = 1.0f;
            }

            return new Vector2(leftRight, upDown);
        }
    }
}
