using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : Singleton<UIController>
{
  [SerializeField]
  Text scoreText;
  [SerializeField]
  GameObject gameOverPanel;


  private void Start()
  {
    gameOverPanel.SetActive(false);
  }
  private void OnEnable()
  {
    ActionSystem.OnScoreChanged += UpdateScore;
    ActionSystem.OnGameOver += SetGameOverScreen;
  }
  private void OnDisable()
  {
    ActionSystem.OnScoreChanged -= UpdateScore;
    ActionSystem.OnGameOver -= SetGameOverScreen;
  }

  void UpdateScore(int score)
  {
    scoreText.text = score.ToString();
  }

  void SetGameOverScreen()
  {
    gameOverPanel.SetActive(true);
  }

}
