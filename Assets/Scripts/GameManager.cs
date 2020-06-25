using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
  static int score = 0;
  public static int Score
  { get { return score; }
    private set
    {
      if(value/1000>score/1000)
      {
        IsReadyToSetBomb = true;
      }
      score = value;
    }
  }
  public static bool IsGameOn;
  public static bool IsReadyToSetBomb;
  void Start()
  {
    IsGameOn = true;
    UpdateScore(0);
  }

  private void OnEnable()
  {
    ActionSystem.OnBombExplode += GameOver;
  }

  private void OnDisable()
  {
    ActionSystem.OnBombExplode -= GameOver;
  }

  public static void IncreaseScore(int value)
  {
    UpdateScore(Score + value);
  }

  public static void UpdateScore(int newScore)
  {
    Score = newScore;
    ActionSystem.OnScoreChanged?.Invoke(Score);
  }

  void GameOver()
  {
    ActionSystem.OnGameOver?.Invoke();
     IsGameOn = false;
  }

  public void RefreshLevel()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
  }
}
