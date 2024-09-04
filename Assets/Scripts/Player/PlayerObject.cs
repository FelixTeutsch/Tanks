using System;
using UnityEngine;
using Utility;

namespace Player
{
    public class PlayerObject
    {
        private Tank _tankScript;

        public PlayerObject(string name, EColour colour, GameObject tank)
        {
            this.name = name;
            this.colour = colour;
            this.tank = tank;
        }

        public PlayerObject(string name, EColour colour)
        {
            this.name = name;
            this.colour = colour;
        }

        public string name { get; set; }
        public EColour colour { get; set; }
        public GameObject tank { get; set; }


        public int score { get; set; }
        public bool controlsLocked { get; set; }

        private bool ValidateTank()
        {
            return (_tankScript ??= tank.GetComponent<Tank>()) != null;
        }

        public void SetColour(EColour colour)
        {
            this.colour = colour;
        }

        public void SetTank(GameObject tankInstance)
        {
            tank = tankInstance;
            _tankScript = tankInstance.GetComponent<Tank>();
            _tankScript.SetColourAndName(colour, name);
        }

        public void MoveTank(float movementInput)
        {
            if (controlsLocked) return;
            if (ValidateTank())
                _tankScript.Move(movementInput);
        }

        public void RotateCannon(float cannonInput)
        {
            if (controlsLocked) return;

            if (ValidateTank())
                _tankScript.RotateCannon(cannonInput);
        }

        public void Fire()
        {
            if (controlsLocked) return;
            LockControls();
            if (ValidateTank())
            {
                _tankScript.Fire();
                _tankScript.activePlayer = false;
            }
        }

        public void LockControls()
        {
            controlsLocked = true;
            if (ValidateTank())
                _tankScript.LockControls();
        }

        // unlocks controls, so tank can take actions again
        public void UnlockControls()
        {
            controlsLocked = false;
            if (ValidateTank())
                _tankScript.UnlockControls();
        }

        public float GetPower()
        {
            try
            {
                if (ValidateTank())
                    return _tankScript.GetPower();
                return 0;
            }
            catch (Exception e)
            {
                Debug.Log("Player - GetPower - Exception: " + e.Message);
                Debug.LogException(e);
                return -1;
            }
        }

        public float SetPower(float power)
        {
            Debug.Log("Player - SetPower START");
            if (controlsLocked) return -1;
            if (ValidateTank())
                return _tankScript.UpdatePower(power);
            Debug.Log("Player - SetPower END");
            return -1;
        }

        public void SetActivePlayer()
        {
            if (ValidateTank())
                _tankScript.activePlayer = true;
        }
    }
}