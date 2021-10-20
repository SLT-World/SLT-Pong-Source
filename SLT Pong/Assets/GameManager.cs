using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject BallPrefab;

    private void Awake() =>
        Instance = this;

    public void Restart() =>
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    public void Exit() =>
        Application.Quit();
}
