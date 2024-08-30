using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

namespace Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField] public EColour playerColor = EColour.Blue;
        [SerializeField] private bool updateColor;
        public int score;
        public GameObject tank;
        public List<string> inventory;

        [SerializeField] private bool controlsLocked;

        private Tank _tank;

        private void Start()
        {
            Debug.Log("Player - Start START");
            AssignRandomColor();
            Debug.Log($"Player - Start - {transform.name} Color: {playerColor}");
            _tank = tank.GetComponent<Tank>();
            ScoreUiController.Instance.TankActive();
            Debug.Log("Player - Start END");
        }

        private void Update()
        {
            if (updateColor)
            {
                updateColor = false;
                if (ValidateTank())
                    _tank.SetColour(playerColor);
            }
        }

        private bool ValidateTank()
        {
            return (_tank ??= tank.GetComponent<Tank>()) != null;
        }

        public void AssignTank(GameObject tankInstance)
        {
            tank = tankInstance;
            // Get the correct script
            ValidateTank();
            // Set the tank's color
            _tank.SetColour(playerColor);
        }

        public void MoveTank(float movementInput)
        {
            if (controlsLocked) return;
            if (ValidateTank())
                _tank.Move(movementInput);
        }

        public void RotateCannon(float cannonInput)
        {
            if (controlsLocked) return;

            if (ValidateTank())
                _tank.RotateCannon(cannonInput);
        }

        public void Fire()
        {
            if (controlsLocked) return;
            LockControls();
            if (ValidateTank())
                _tank.Fire();
        }

        private void AssignRandomColor()
        {
            var colors = Enum.GetValues(typeof(EColour));
            playerColor = (EColour)colors.GetValue(Random.Range(0, colors.Length));

            if (ValidateTank()) _tank.SetColour(playerColor);
        }


        // locks controls so tank can't take actions anymore
        public void LockControls()
        {
            controlsLocked = true;
            if (ValidateTank())
                _tank.LockControls();
        }

        // unlocks controls, so tank can take actions again
        public void UnlockControls()
        {
            controlsLocked = false;
            if (ValidateTank())
                _tank.UnlockControls();
        }

        public float GetPower()
        {
            try
            {
                if (ValidateTank())
                    return _tank.GetPower();
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
                return _tank.UpdatePower(power);
            Debug.Log("Player - SetPower END");
            return -1;
        }
    }
}