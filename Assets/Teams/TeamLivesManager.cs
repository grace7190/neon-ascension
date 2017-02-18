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
        switch (playerGameObject.GetComponent<PlayerController>().Team)
        {
            case Teams.Blue:
                _blueLives--;
                if (_blueLives > 0)
                {
                    StartCoroutine(RespawnCoroutine(playerGameObject));
                }
                else
                {
                    Destroy(playerGameObject);
                }
                break;
            case Teams.Purple:
                _purpleLives--;
                if (_purpleLives > 0)
                {
                    StartCoroutine(RespawnCoroutine(playerGameObject));
                }
                else
                {
                    Destroy(playerGameObject);
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

    private IEnumerator RespawnCoroutine(GameObject playerGameObject)
    {
        playerGameObject.SetActive(false);
        yield return new WaitForSeconds(RespawnDelay);
        var team = playerGameObject.GetComponent<PlayerController>().Team;
        playerGameObject.transform.position = BlockColumnManager.Instance.GetRespawnPoint(team);
        playerGameObject.SetActive(true);
    }
}

