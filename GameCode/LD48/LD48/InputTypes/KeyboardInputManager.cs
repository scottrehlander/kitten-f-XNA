using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace LD48.InputTypes
{
    public class KeyboardInputManager : InputManager
    {

        public override void Update()
        {
            // Grab the state of the keyboard
            KeyboardState kb = Keyboard.GetState();
            Keys[] keysDown = kb.GetPressedKeys();

            ResetAllKeys();

            // Check the state of the keyboard and manipulate the hero position and the 
            if (keysDown.Contains(Keys.A) || keysDown.Contains(Keys.Left))
            {
                MoveLeftPressed = true;
            }
            if (keysDown.Contains(Keys.D) || keysDown.Contains(Keys.Right))
            {
                MoveRightPressed = true;
            }
            if (keysDown.Contains(Keys.W) || keysDown.Contains(Keys.Up))
            {
                MoveUpPressed = true;
            }
            if (keysDown.Contains(Keys.S) || keysDown.Contains(Keys.Down))
            {
                MoveDownPressed = true;
            }

            // Attacking
            if (keysDown.Contains(Keys.Space))
            {
                AttackPressed = true;
            }

            // Zooming ( should we disable )?
            if (keysDown.Contains(Keys.O))
            {
                ZoomOutPressed = true;
            }
            if (keysDown.Contains(Keys.P))
            {
                ZoomInPressed = true;
            }

            if (keysDown.Contains(Keys.F))
            {
                SwitchWeaponPressed = true;
            }
        }

    }
}
