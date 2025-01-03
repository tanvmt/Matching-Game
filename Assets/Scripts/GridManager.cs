using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] Tile tile;
    [SerializeField] int gridWidth;
    [SerializeField] int gridHeight;
    [SerializeField] float tileSpace;
    Dictionary<Vector2, Tile> grid;
    List<Tile> clickedTiles;
    int[,] gridValue;
    void Start()
    {
        GenerateGrid();
        clickedTiles = new List<Tile>();
    }

    private void GenerateGrid()
    {
        grid = new Dictionary<Vector2, Tile>();
        gridValue = new int[gridHeight, gridWidth];
        int totalTiles = gridHeight * gridWidth;
        int tileTypes = tile.GetSpriteLength();
        List<int> tileCounts = GenerateRandomTileCounts(totalTiles, tileTypes);

        List<int> tilePool = new List<int>();
        for(int i=0;i<tileTypes;i++)
        {
            for(int j=0;j<tileCounts[i];j++)
            {
                tilePool.Add(i);
            }
        }

        Shuffle(tilePool);

        for (int i = 0; i < gridHeight; i++) {
            for (int j = 0; j < gridWidth; j++){
                int tileIndex = tilePool[0];
                tilePool.RemoveAt(0);

                grid[new Vector2(i, j)] = Instantiate(tile, new Vector3(j * tileSpace - gridWidth * tileSpace / 2 + tileSpace / 2, -(i * tileSpace - gridHeight * tileSpace / 2) - 0.8f, 0), Quaternion.identity);
                grid[new Vector2(i,j)].transform.parent = this.transform;
                grid[new Vector2(i,j)].SetPosition(i, j);
                gridValue[i, j] = tileIndex;
                grid[new Vector2(i,j)].GetSprite(tileIndex);
            }

        }
    }

    private List<int> GenerateRandomTileCounts(int totalTiles, int tileTypes){
        List<int> tileCounts = new List<int>();
        int remainingTilePairs = totalTiles / 2;

        for(int i=0;i<tileTypes;i++){
            if(i==tileTypes - 1){
                tileCounts.Add(remainingTilePairs * 2);
            }
            else{
                int randomCount = Random.Range(1, (remainingTilePairs - (tileTypes - i - 1))/2);
                tileCounts.Add(randomCount * 2);
                remainingTilePairs -= randomCount;
            }    
        }
        return tileCounts;
    }

    private void Shuffle(List<int> list){
        for(int i=0;i<list.Count;i++){
            int randomIndex = Random.Range(i, list.Count);
            int temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void TileClicked(Tile clickedTile){
        Debug.Log("Tile value: " + gridValue[clickedTile.GetX(), clickedTile.GetY()]);

        if (clickedTiles.Count == 0)
        {
            AddTileToClicked(clickedTile);
        }
        else if (clickedTiles.Count == 1)
        {
            if(clickedTiles.Contains(clickedTile)){
                RemoveTileFromClicked(clickedTile);
            }
            else{
                AddTileToClicked(clickedTile);
                Invoke("IsSelectedTilesMatch", 1f);
            }
        }
    }

    public void AddTileToClicked(Tile tile){
        clickedTiles.Add(tile);
        tile.ToggleHighlight(true);
    }

    public void RemoveTileFromClicked(Tile tile){
        clickedTiles.Remove(tile);
        tile.ToggleHighlight(false);
    }

    public void IsSelectedTilesMatch(){
        if(gridValue[clickedTiles[0].GetX(), clickedTiles[0].GetY()] != gridValue[clickedTiles[1].GetX(), clickedTiles[1].GetY()]){
            Debug.Log("Not match");
            ResetClickedTiles();
            return;
        }

        int shortestPath = BFSShortestPath(clickedTiles[0], clickedTiles[1]);
        if(0<= shortestPath && shortestPath <= 3){
            DestroyObject(clickedTiles[0], clickedTiles[1]);
        }

        if(AreOnEdge(clickedTiles[0], clickedTiles[1])){
            DestroyObject(clickedTiles[0], clickedTiles[1]);
        }

        ResetClickedTiles();
    }

    private void DestroyObject(Tile tile1, Tile tile2){
        gridValue[tile1.GetX(), tile1.GetY()] = -1;
        Destroy(tile1.gameObject);
        gridValue[tile2.GetX(), tile2.GetY()] = -1;
        Destroy(tile2.gameObject);
    }

    private void ResetClickedTiles(){
        foreach(Tile tile in clickedTiles){
            tile.ToggleHighlight(false);
        }
        clickedTiles.Clear();
    }

    private bool AreOnEdge(Tile tile1, Tile tile2){
        return (tile1.GetX() == 0 && tile2.GetX() == 0) || //left
                (tile1.GetX() == gridHeight - 1 && tile2.GetX() == gridHeight - 1) || //right 
                (tile1.GetY() == 0 && tile2.GetY() == 0) || //top
                (tile1.GetY() == gridWidth - 1 && tile2.GetY() == gridWidth - 1); //bottom
    }

    // private List<Vector2> GetPossibleDirections(Tile end, Tile current)
    // {
    //     List<Vector2> possibleDirections = new List<Vector2>();
    //     Vector2[] directions = { new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0) };

    //     foreach (Vector2 direction in directions)
    //     {
    //         Vector2 neighbourPos = new Vector2(current.GetX(), current.GetY()) + direction;
            
    //         if (0<=neighbourPos.x && neighbourPos.x<gridWidth && 0<=neighbourPos.y && neighbourPos.y<gridHeight)
    //         {
    //             Debug.Log("Checking: " + neighbourPos.x + ", " + neighbourPos.y);
    //             if(gridValue[(int)neighbourPos.x, (int)neighbourPos.y] == -1){
    //                 possibleDirections.Add(neighbourPos);
    //                 Debug.Log("Current: " + current.GetX() + ", " + current.GetY());
    //                 Debug.Log("dỉrection: " + direction);
    //                 Debug.Log("Possible direction: " + neighbourPos.x + ", " + neighbourPos.y);
    //                 Debug.Log("-----------------");
                    
    //             }
    //             if(neighbourPos == new Vector2(end.GetX(), end.GetY()) && gridValue[(int)neighbourPos.x, (int)neighbourPos.y] == gridValue[current.GetX(), current.GetY()]){
    //                 possibleDirections.Add(neighbourPos);
    //                 Debug.Log("Current: " + current.GetX() + ", " + current.GetY());
    //                 Debug.Log("dỉrection: " + direction);
    //                 Debug.Log("Possible direction: " + neighbourPos.x + ", " + neighbourPos.y);
    //                 Debug.Log("-----------------");
                    
                    
    //             }
                
    //         }
    //     }
    //     return possibleDirections;
    // }

    // public int BFSShortestPath(Tile start, Tile end){
    //     Queue<Tile> queue = new Queue<Tile>();
    //     Dictionary<Vector2, int> distance = new Dictionary<Vector2, int>();
    //     queue.Enqueue(start);
    //     distance[new Vector2(start.GetX(), start.GetY())] = 0;
    //     Debug.Log("Start: " + start.GetX() + ", " + start.GetY());
    //     while(queue.Count > 0){
    //         Debug.Log("in while");
    //         Tile current = queue.Dequeue();
    //         if(current == end){
    //             Debug.Log("Distance: " + distance[new Vector2(current.GetX(), current.GetY())]);
    //             return distance[new Vector2(current.GetX(), current.GetY())];
    //         }
    //         List<Vector2> directions = GetPossibleDirections(end, current);
    //         foreach(Vector2 direction in directions){
    //             if(!distance.ContainsKey(direction)){
    //                 distance[direction] = distance[new Vector2(current.GetX(), current.GetY())] + 1;
    //                 queue.Enqueue(grid[direction]);
    //             }
    //         }
    //     }
    //     return -1;
    // }

    private int BFSShortestPath(Tile start, Tile end){
        Queue<Tile> queue = new Queue<Tile>();
        Dictionary<Vector2, int> distance = new Dictionary<Vector2, int>();
        queue.Enqueue(start);
        distance[new Vector2(start.GetX(), start.GetY())] = 0;
        while(queue.Count > 0){
            Tile current = queue.Dequeue();
            if(current == end){
                return distance[new Vector2(current.GetX(), current.GetY())];
            }
            Vector2[] directions = { new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0) };
            foreach(Vector2 direction in directions){
                Vector2 neighbourPos = new Vector2(current.GetX(), current.GetY()) + direction;
                if (0<=neighbourPos.x && neighbourPos.x<gridWidth && 0<=neighbourPos.y && neighbourPos.y<gridHeight)
                {
                    if(!distance.ContainsKey(neighbourPos) && (gridValue[(int)neighbourPos.x, (int)neighbourPos.y] == -1 || neighbourPos == new Vector2(end.GetX(), end.GetY()) && gridValue[(int)neighbourPos.x, (int)neighbourPos.y] == gridValue[current.GetX(), current.GetY()])){
                        distance[neighbourPos] = distance[new Vector2(current.GetX(), current.GetY())] + 1;
                        queue.Enqueue(grid[neighbourPos]);
                    }
                }
            }
        }
        return -1;
    }
}
