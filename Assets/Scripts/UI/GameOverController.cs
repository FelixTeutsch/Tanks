using System;
using System.Collections.Generic;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;
using Random = UnityEngine.Random;

public class GameOverController : MonoBehaviour
{
    [SerializeField] private GameObject playerResultPrefab;
    [SerializeField] private GameObject playerResultArea;

    private List<PlayerObject> _players;

    // Start is called before the first frame update
    private void Start()
    {
        _players = new List<PlayerObject>();
        if (GameManager.instance != null)
        {
            _players = GameManager.instance.getPlayers;
        }
        else
        {
            var colours = (EColour[])Enum.GetValues(typeof(EColour));
            for (var i = 0; i < 4; i++)
            {
                var randomColour = colours[Random.Range(0, colours.Length)];
                var player = new PlayerObject("Player " + i, randomColour, Random.Range(0, 100));
                _players.Add(player);
            }
        }

        Debug.Log("_players: " + _players.Count);

        PopulatePlayerResults();
    }

    public void OnMainMenu()
    {
        SceneManager.LoadScene(EScene.MainMenu);
    }

    public void OnRestart()
    {
        // Reset Scores
        foreach (var player in _players) player.ResetPlayer();
        // Start new round
        SceneManager.LoadScene(EScene.Game);
    }

    private void PopulatePlayerResults()
    {
        Debug.Log("Populating Player Results");
        var padding = 16;
        var gap = 8;
        _players.Sort((a, b) => b.score.CompareTo(a.score));
        for (var i = 0; i < _players.Count; i++)
        {
            Debug.Log("Player " + i + ": " + _players[i].name + " - " + _players[i].score);
            var playerResult = Instantiate(playerResultPrefab, playerResultArea.transform);

            var rectTransform = playerResult.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0,
                -i * (playerResult.GetComponent<RectTransform>().rect.height + gap) - padding);

            playerResult.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = _players[i].name;
            playerResult.transform.Find("Score").GetComponent<TextMeshProUGUI>().text = _players[i].score.ToString();
            playerResult.transform.Find("Rank").GetComponent<TextMeshProUGUI>().text = (i + 1).ToString();
            playerResult.transform.Find("Color").GetComponent<Image>().color = _players[i].colour.GetColour();

            playerResult.name = _players[i].name;
        }

        Debug.Log("Player Results Populated");
    }
}