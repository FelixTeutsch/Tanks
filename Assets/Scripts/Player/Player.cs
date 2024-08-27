using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private EColour playerColor = EColour.Blue;
        [SerializeField] private bool updateColor;
        public int score;
        public GameObject tank;
        public List<string> inventory;

        private void Update()
        {
            if (updateColor)
            {
                updateColor = false;
                tank.GetComponent<Tank>().SetColour(playerColor);
            }
        }

        public void AssignTank(GameObject tankInstance)
        {
            tank = tankInstance;
            // Set the tank's color
            tank.GetComponent<Tank>().SetColour(playerColor);
        }

        public void MoveTank(float movementInput)
        {
            tank.GetComponent<Tank>().Move(movementInput);
        }

        public void RotateCannon(float cannonInput)
        {
            tank.GetComponent<Tank>().RotateCannon(cannonInput);
        }

        public void Fire()
        {
            tank.GetComponent<Tank>().Fire();
        }
    }
}