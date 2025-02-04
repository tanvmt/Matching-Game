using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIDisplay : MonoBehaviour
{
    [Header("Level")]
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] int level = 1;

    [Header("Score")]
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] int score = 0;
    ScoreKeeper scoreKeeper;

    [Header("Time Bar")]
    [SerializeField] Slider timeBar;
    [SerializeField] float totalTime = 300f;
    [SerializeField] float timeLeft;

    [Header("Back Dialog")]
    [SerializeField] GameObject backDialog;

    [Header("UI Canvas Group")]
    [SerializeField] CanvasGroup uiCanvasGroup;

    [Header("Tile")]
    [SerializeField] Tile tile;

    enum GameState {Playing, Paused, GameOver};
    GameState gameState;


    void Start(){
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
        levelText.text = "Level " + level.ToString();
        scoreText.text = "Score: " + scoreKeeper.GetScore().ToString();
        timeLeft = totalTime;
        timeBar.maxValue = totalTime;
        timeBar.value = timeLeft;
        backDialog.SetActive(false);
        gameState = GameState.Playing;
    }

    void Update(){
        if(gameState == GameState.Playing){
            timeLeft -= Time.deltaTime;
            timeBar.value = timeLeft;
            if(timeLeft <= 0){
                gameState = GameState.GameOver;
                OpenBackDialog();
            }
        }
    }


    public void UpdateLevel(){
        level++;
        levelText.text = level.ToString();
    }

    public void AddToScore(int pointsToAdd){
        scoreKeeper.AddToScore(pointsToAdd);
        scoreText.text = "Score: " + scoreKeeper.GetScore().ToString();
    }

    public void ResetLevel(){
        level = 1;
        levelText.text = level.ToString();
    }

    public void OpenBackDialog(){
        backDialog.SetActive(true);
        uiCanvasGroup.interactable = false;
        uiCanvasGroup.blocksRaycasts = false;
        gameState = GameState.Paused;
        tile.SetCanClicked(false);
    }

    public void CloseBackDialog(){
        backDialog.SetActive(false);
        uiCanvasGroup.interactable = true;
        uiCanvasGroup.blocksRaycasts = true;
        gameState = GameState.Playing;
        tile.SetCanClicked(true);
    }

}
