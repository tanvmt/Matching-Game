using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private GameObject hightlightTile;
    [SerializeField] private Sprite[] sprites;
    private bool isClicked;
    private GridManager gridManager;
    private int x, y;
    
    private void Start() {
        gridManager = FindObjectOfType<GridManager>();
    }

    public void SetPosition(int x, int y){
        this.x = x;
        this.y = y;
    }

    public int GetX(){
        return x;
    }

    public int GetY(){
        return y;
    }

    public int GetSpriteLength(){
        return sprites.Length;
    }

    public void GetSprite(int index){
        GetComponent<SpriteRenderer>().sprite = sprites[index];
    }

    private void OnMouseDown() {
        gridManager.TileClicked(this);
        Debug.Log("Tile clicked: " + x + ", " + y);
    }

    public void ToggleHighlight(bool enable) {
        hightlightTile.SetActive(enable);
    }

}
