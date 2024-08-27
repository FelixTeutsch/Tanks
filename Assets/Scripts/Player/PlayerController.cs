// PlayerController.cs

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> tankSprites;
        private GameManager _gameManager;
        public static PlayerController Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            _gameManager = GameManager.Instance;
        }

        public void OnMovement(InputValue value)
        {
            _gameManager.CurrentPlayer.MoveTank(value.Get<float>());
        }

        public void OnCannonAngle(InputValue value)
        {
            _gameManager.CurrentPlayer.RotateCannon(value.Get<float>());
        }

        public void OnFire()
        {
            _gameManager.CurrentPlayer.Fire();
            // Lock controls until bullet hits/explodes
        }

        public GameObject GetTankSprite(EColour colour)
        {
            foreach (var tank in tankSprites)
                if (tank.name.ToLower().Contains(colour.ToString().ToLower()))
                    return tank;

            return null;
        }
    }
}