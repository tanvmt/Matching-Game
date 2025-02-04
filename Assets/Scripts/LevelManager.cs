using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManager : MonoBehaviour
{
    ScoreKeeper scoreKeeper;
    void Awake(){
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
    }
    public void LoadMenu(){
        SceneManager.LoadScene("Menu");
        scoreKeeper.ResetScore();
    }

    public void LoadGame(){
        SceneManager.LoadScene("Level 1");
    }

}
