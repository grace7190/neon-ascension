using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TeamLivesManager : MonoBehaviour
{
    public static TeamLivesManager Instance;

    private const float MaxLives = 3;

    private const float RespawnDelay = 1;

    private float _blueLives = MaxLives;
    private float _purpleLives = MaxLives;
    private Text _blueText;
    private Text _purpleText;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _blueText = transform.FindChild("Blue").GetComponent<Text>();
        _purpleText = transform.FindChild("Purple").GetComponent<Text>();
        UpdateHud();
    }

    public void HandlePlayerDeath(GameObject playerGameObject)
    {
        DisablePlayer(playerGameObject);

        switch (playerGameObject.GetComponent<PlayerController>().Team)
        {
            case Team.Blue:
                _blueLives--;
                if (_blueLives > 0)
                {
                    StartCoroutine(RespawnCoroutine(playerGameObject));
                }
                else
                {
                    Destroy(playerGameObject);
                    EndOfGameManager.Instance.ShowVictoryScreen(Team.Purple);
                }
                break;
            case Team.Purple:
                _purpleLives--;
                if (_purpleLives > 0)
                {
                    StartCoroutine(RespawnCoroutine(playerGameObject));
                }
                else
                {
                    Destroy(playerGameObject);
                    EndOfGameManager.Instance.ShowVictoryScreen(Team.Blue);
                }
                break;
        }

        UpdateHud();
    }

    private void UpdateHud()
    {
        _blueText.text = "Lives: " + _blueLives;
        _purpleText.text = "Lives: " + _purpleLives;
    }

    private void DisablePlayer(GameObject playerGameObject)
    {
        playerGameObject.SetActive(false);
    }

    private IEnumerator RespawnCoroutine(GameObject playerGameObject)
    {
        yield return new WaitForSeconds(RespawnDelay);
        var team = playerGameObject.GetComponent<PlayerController>().Team;
        playerGameObject.transform.position = BlockColumnManager.Instance.GetRespawnPoint(team);
        playerGameObject.SetActive(true);
    }
}

