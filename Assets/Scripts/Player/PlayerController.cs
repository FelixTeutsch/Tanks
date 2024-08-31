using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> tankSprites;
        [SerializeField] private bool controlsLocked;
        [SerializeField] private PowerSlider powerSlider;

        private Player _currentPlayer;
        private GameManager _gameManager;

        public static PlayerController Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            // TODO: is this needed? Probably not
            _gameManager = GameManager.Instance;
        }

        public void OnMovement(InputValue value)
        {
            if (controlsLocked) return;
            _currentPlayer.MoveTank(value.Get<float>());
        }

        public void OnCannonAngle(InputValue value)
        {
            if (controlsLocked) return;
            _currentPlayer.RotateCannon(value.Get<float>());
        }

        public void OnFire()
        {
            if (controlsLocked) return;
            // Lock controls until bullet hits/explodes
            LockControls();
            _currentPlayer.Fire();
        }

        public GameObject GetTankSprite(EColour colour)
        {
            foreach (var tank in tankSprites)
                if (tank.name.ToLower().Contains(colour.ToString().ToLower()))
                    return tank;

            return null;
        }

        public void SwitchPlayer(Player player)
        {
            Debug.Log("PlayerController - SwitchPlayer START");
            _currentPlayer = player;
            Debug.Log("PlayerController - UnlockControls - CurrentPlayer: " + _currentPlayer);
            UnlockControls();
            Debug.Log("PlayerController - SwitchPlayer - powerSlider: " + powerSlider);
            powerSlider.SetPower(_currentPlayer.GetPower());
            _currentPlayer.SetActivePlayer();
            Debug.Log("PlayerController - SwitchPlayer END");
        }

        public float SetPower(float power)
        {
            Debug.Log("PlayerController - SetPower START");
            var temp = _currentPlayer.SetPower(power);
            Debug.Log("PlayerController - SetPower END");
            return temp;
        }


        // locks controls so tank can't take actions anymore
        public void LockControls()
        {
            controlsLocked = true;
        }

        // unlocks controls, so tank can take actions again
        public void UnlockControls()
        {
            Debug.Log("PlayerController - UnlockControls START");
            controlsLocked = false;
            _currentPlayer.UnlockControls();
            Debug.Log("PlayerController - UnlockControls END");
        }
    }
}