using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private GameObject hightlightTile;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private static bool canClicked = true;
    private GridManager gridManager;
    private int x, y;
    private float posX, posY;
    
    private void Start() {
        gridManager = FindObjectOfType<GridManager>();
    }

    public void SetPositionOnGrid(int x, int y){
        this.x = x;
        this.y = y;
    }

    public int GetX(){
        return x;
    }

    public int GetY(){
        return y;
    }

    public float getPosX(){
        return posX;
    }

    public float getPosY(){
        return posY;
    } 

    public void SetPositionOnScene(float x, float y){
        this.posX = x;
        this.posY = y;
    }

    public int GetSpriteLength(){
        return sprites.Length;
    }

    public void GetSprite(int index){
        GetComponent<SpriteRenderer>().sprite = sprites[index];
    }

    private void OnMouseDown() {
        if(canClicked){
            gridManager.TileClicked(this);
        }
        // Debug.Log("Tile clicked: " + x + ", " + y);
    }

    public void ToggleHighlight(bool enable) {
        hightlightTile.SetActive(enable);
    }

    public void SetCanClicked(bool flag){
        canClicked = flag;
    }

}
