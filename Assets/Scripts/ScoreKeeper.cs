using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreKeeper : MonoBehaviour
{
    [SerializeField] private int score = 0;

    void Awake(){
        int scoreKeeperCount = FindObjectsOfType<ScoreKeeper>().Length;
        if(scoreKeeperCount > 1){
            gameObject.SetActive(false);
            Destroy(gameObject);
        } else {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void AddToScore(int pointsToAdd){
        score += pointsToAdd;
    }

    public int GetScore(){
        return score;
    }

    public void ResetScore(){
        score = 0;
    }


}
