using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private TextMeshProUGUI scoreText;

    public void StopGame(int score)
    {
        gameOverCanvas.SetActive(true);
        scoreText.text ="Your score: " +  score.ToString();  
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
