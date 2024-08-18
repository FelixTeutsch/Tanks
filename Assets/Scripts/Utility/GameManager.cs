using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Worldgen;

namespace Utility
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [SerializeField] private WorldGeneration worldGeneration;
        [SerializeField] private WindEngine windEngine;
        public Slider powerSlider;

        private List<GameObject> _players;
        public GameObject currentPlayer { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this) Destroy(gameObject);
        }


        // Start is called before the first frame update
        private void Start()
        {
            Debug.Log(worldGeneration);
            _players = worldGeneration.InitialiseMap();
            Debug.Log(_players);
            currentPlayer = _players[0];
        }

        // Invoked when the value of the slider changes.
        public void UpdateTankPower()
        {
            currentPlayer.GetComponent<Tank.Tank>().UpdatePower(powerSlider.value);
        }

        public void SwitchPlayer()
        {
            var newPlayerIndex = (_players.IndexOf(currentPlayer) + 1) % _players.Count;
            currentPlayer = _players[newPlayerIndex];
            windEngine.UpdateWind();
        }
    }
}