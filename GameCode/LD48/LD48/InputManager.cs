using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LD48
{
    public class InputManager
    {

        public bool AttackPressed { get; protected set; }
        public bool MoveLeftPressed { get; protected set; }
        public bool MoveRightPressed { get; protected set; }
        public bool MoveUpPressed { get; protected set; }
        public bool MoveDownPressed { get; protected set; }
        public bool ZoomOutPressed { get; protected set; }
        public bool ZoomInPressed { get; protected set; }
        public bool SwitchWeaponPressed { get; protected set; }
        public bool EnterActionPressed { get; protected set; }

        public virtual void Update()
        {
            throw new NotImplementedException();
        }

        protected void ResetAllKeys()
        {
            AttackPressed = false;
            MoveLeftPressed = false;
            MoveRightPressed = false;
            MoveUpPressed = false;
            MoveDownPressed = false;
            MoveUpPressed = false;
            ZoomOutPressed = false;
            ZoomInPressed = false;
            SwitchWeaponPressed = false;
            EnterActionPressed = false;
        }

    }
}
