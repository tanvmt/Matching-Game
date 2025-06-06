using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] Tile tile;
    [SerializeField] int gridWidth;
    [SerializeField] int gridHeight;
    [SerializeField] float tileSpace;
    [SerializeField] GameObject lineObject;
    [SerializeField] float clearLineDelay = 0.1f;
    [SerializeField] float lineOffset = 0.5f;
    Dictionary<Vector2, Tile> grid;
    Dictionary<int, List<Tile>> tilesOfType;
    List<int> tilePool;
    List<Tile> clickedTiles;
    int[,] gridValue;
    UIDisplay UI;

    void Start()
    {
        LoadGame();
        
    }

    public void LoadGame(){
        UI = FindObjectOfType<UIDisplay>();
        InitializeGrid();
        GenerateGrid();
        clickedTiles = new List<Tile>();
    }

    private void InitializeGrid(){
        grid = new Dictionary<Vector2, Tile>();
        gridValue = new int[gridHeight + 2, gridWidth + 2];
        tilesOfType = new Dictionary<int, List<Tile>>();
        tilePool = new List<int>();
        int totalTiles = gridHeight * gridWidth;
        int tileTypes = tile.GetSpriteLength();
        List<int> tileCounts = GenerateRandomTileCounts(totalTiles, tileTypes);

        for(int i=0;i<tileTypes;i++)
        {
            for(int j=0;j<tileCounts[i];j++)
            {
                tilePool.Add(i);
            }
        }
    }
    public void GenerateGrid()
    {
        bool validGrid = false;
        while (!validGrid)
        {
            ClearGrid();
            if (tilePool.Count == 0)
            {
                // ClearGrid();
                tilePool = GetRemainingTiles();
            }
            Shuffle(tilePool);
            SpawnTiles(tilePool);
            validGrid = AreThereAnyMatches();
            // if (!validGrid)
            // {
            //     ClearGrid();
            // }
        }
        // ShowTiles();
        
    }

    private void ShowTiles(){
        for(int i=1;i<gridHeight + 1;i++){
            for(int j=0;j<gridWidth + 1;j++){
                grid[new Vector2(i, j)].gameObject.SetActive(true);
            }
        }
    }

    private List<int> GetRemainingTiles(){
        List<int> remainingTiles = new List<int>();
        for(int i=0;i<gridHeight + 2;i++){
            for(int j=0;j<gridWidth + 2;j++){
                if(gridValue[i, j] != -1){
                    remainingTiles.Add(gridValue[i, j]);
                }
            }
        }
        return remainingTiles;
    }

    private bool AreThereAnyMatches()
    {
        foreach(KeyValuePair<int, List<Tile>> entry in tilesOfType){
            for(int i=0;i<entry.Value.Count;i++){
                for(int j=i+1;j<entry.Value.Count;j++){
                    if(FindOptimalPath(entry.Value[i], entry.Value[j]) != null){
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void ClearGrid(){
        foreach(KeyValuePair<Vector2, Tile> entry in grid){
            if(entry.Value != null){
                Destroy(entry.Value.gameObject);
            }
        }
        grid.Clear();
        tilesOfType.Clear();
    }

    public void SpawnTiles(List<int> tilePool){
        // Lấy thông tin về chiều rộng và chiều cao của camera
        float screenWidth = Camera.main.orthographicSize * 2.0f * Screen.width / Screen.height;
        float screenHeight = Camera.main.orthographicSize * 2.0f;

        // Tính toán kích thước ô dựa trên không gian hiển thị
        float tileWidth = (screenWidth - 2) / (gridWidth + 1);
        float tileHeight = (screenHeight - 3) / (gridHeight + 1);

        // Sử dụng giá trị nhỏ hơn giữa tileWidth và tileHeight để đảm bảo ô vuông
        float tileSize = Mathf.Min(tileWidth, tileHeight);

        // Khoảng cách giữa các tile (có thể điều chỉnh giá trị này để thêm khoảng cách)
        float tileSpacing = tileSize * 0.1f; // 10% của kích thước tile
        for (int i = 0; i < gridHeight + 2; i++) {
            for (int j = 0; j < gridWidth + 2; j++){
                // Tính toán vị trí của tile
                float xPos = j * tileSize - (gridWidth * tileSize / 2);
                float yPos = -(i * tileSize - (gridHeight * tileSize / 2));

                // Instantiate tile và thiết lập vị trí
                grid[new Vector2(i, j)] = Instantiate(tile, new Vector3(xPos, yPos, 0), Quaternion.identity);
                grid[new Vector2(i,j)].transform.parent = this.transform;
                grid[new Vector2(i,j)].SetPositionOnGrid(i, j);
                grid[new Vector2(i,j)].SetPositionOnScene(grid[new Vector2(i,j)].transform.position.x, grid[new Vector2(i,j)].transform.position.y);

                if(i == 0 || i == gridHeight + 1 || j == 0 || j == gridWidth + 1){
                    gridValue[i, j] = -1;
                     grid[new Vector2(i,j)].gameObject.SetActive(false);
                    continue;
                }

                if(gridValue[i, j] == -1){
                    grid[new Vector2(i,j)].gameObject.SetActive(false);
                    continue;
                }

                int tileIndex = tilePool[0];
                tilePool.RemoveAt(0);
                gridValue[i, j] = tileIndex;
                grid[new Vector2(i,j)].GetSprite(tileIndex);

                if(!tilesOfType.ContainsKey(tileIndex)){
                    tilesOfType[tileIndex] = new List<Tile>();
                }

                tilesOfType[tileIndex].Add(grid[new Vector2(i,j)]);
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
                CheckSelectedTilesMatch();
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

    public void CheckSelectedTilesMatch(){
        if(!IsTilesMatched(clickedTiles[0], clickedTiles[1])){
            Debug.Log("Not match");
            ResetClickedTiles();
            return;
        }

        List<Tile> path = FindOptimalPath(clickedTiles[0], clickedTiles[1]);
        if(path != null){
            DrawLine(path);
            Invoke("ClearLines", clearLineDelay);
            DestroyObjects(clickedTiles[0], clickedTiles[1]);
            UI.AddToScore(10);
            if(!AreThereAnyMatches()){
                GenerateGrid();
            }
        }

        ResetClickedTiles();
    }

    public bool IsTilesMatched(Tile tile1, Tile tile2){
        return gridValue[tile1.GetX(), tile1.GetY()] == gridValue[tile2.GetX(), tile2.GetY()];
    }

    private void DestroyObjects(Tile tile1, Tile tile2){
        tilesOfType[gridValue[tile1.GetX(), tile1.GetY()]].Remove(tile1);
        gridValue[tile1.GetX(), tile1.GetY()] = -1;
        Destroy(tile1.gameObject);
        tilesOfType[gridValue[tile2.GetX(), tile2.GetY()]].Remove(tile2);
        gridValue[tile2.GetX(), tile2.GetY()] = -1;
        Destroy(tile2.gameObject);
    }

    private void ResetClickedTiles(){
        foreach(Tile tile in clickedTiles){
            tile.ToggleHighlight(false);
        }
        clickedTiles.Clear();
    }

    private List<Tile> FindOptimalPath(Tile start, Tile end)
    {
        // Tìm tất cả các đường đi
        Dictionary<int, List<List<Tile>>> allPaths = BFSFindAllPaths(start, end);

        // Chọn đường đi có ít lần đổi hướng
        List<Tile> optimalPath = GetOptimalPath(allPaths);
        

        return optimalPath;
    }

    private Dictionary<int, List<List<Tile>>> BFSFindAllPaths(Tile start, Tile end)
    {
        Queue<List<Tile>> queue = new Queue<List<Tile>>();
        queue.Enqueue(new List<Tile> { start });
        // List<List<Tile>> allPaths = new List<List<Tile>>();
        Dictionary<int, List<List<Tile>>> allPaths = new Dictionary<int, List<List<Tile>>>();
        while(queue.Count > 0){
            List<Tile> path = queue.Dequeue();
            Tile current = path[path.Count - 1];

            if (CountDirectionChanges(path) > 2) {
                continue;
            }

            if(current == end){
                // allPaths.Add(path);
                int turns = CountDirectionChanges(path);
                if(!allPaths.ContainsKey(turns)){
                    allPaths[turns] = new List<List<Tile>>();
                }
                allPaths[turns].Add(path);
                continue;
            }

            List<Tile> neighbours = GetNeighbours(current, end);
            foreach(Tile neighbour in neighbours){
                if(!path.Contains(neighbour)){
                    List<Tile> newPath = new List<Tile>(path);
                    newPath.Add(neighbour);
                    queue.Enqueue(newPath);
                }
            }
        }

        return allPaths; // Trả về tất cả các đường đi
    }



    private List<Tile> GetOptimalPath(Dictionary<int, List<List<Tile>>> allPaths)
    {
        List<Tile> optimalPath = null;
        int minTurns = int.MaxValue;

        // foreach (var path in allPaths)
        // {
        //     int turns = CountDirectionChanges(path);
        //     if (turns < minTurns)
        //     {
        //         minTurns = turns;
        //         optimalPath = path;
        //     }
        // }

        if(allPaths.ContainsKey(0)){
            optimalPath = allPaths[0][0];
            minTurns = 0;
        }
        else if(allPaths.ContainsKey(1)){
            optimalPath = allPaths[1][0];
            minTurns = 1;
        }
        else if(allPaths.ContainsKey(2)){
            optimalPath = allPaths[2][0];
            minTurns = 2;
        }

        // Hiển thị đường đi tối ưu
        if (optimalPath != null)
        {
            string optimalPathLog = "Optimal Path: ";
            foreach (Tile tile in optimalPath)
            {
                optimalPathLog += $"({tile.GetX()}, {tile.GetY()}) -> ";
            }
            Debug.Log(optimalPathLog); // Hiển thị đường đi tối ưu trên console
            Debug.Log("Number of turns: " + minTurns); // Hiển thị số lần đổi hướng trên console
        }

        return optimalPath;
    }


    private int CountDirectionChanges(List<Tile> path)
    {
        int turns = 0;
        Vector2? previousDirection = null;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 currentDirection = new Vector2(path[i].GetX() - path[i - 1].GetX(), path[i].GetY() - path[i - 1].GetY());
            if (previousDirection != null && previousDirection != currentDirection)
            {
                turns++;
            }
            previousDirection = currentDirection;
        }

        return turns;
    }

    private List<Tile> GetNeighbours(Tile current, Tile end){
        List<Tile> neighbours = new List<Tile>();
        Vector2[] directions = { new Vector2(-1, 0), new Vector2(1, 0), new Vector2(0, -1), new Vector2(0, 1) };

        foreach (Vector2 direction in directions){
            Vector2 neighbourPos = new Vector2(current.GetX(), current.GetY()) + direction;
            // Debug.Log("--------------------");
            // Debug.Log("current: " + current.GetX() + ", " + current.GetY());
            // Debug.Log("direction: " + direction);
            // Debug.Log("neighbourPos: " + neighbourPos);
            // Debug.Log("--------------------");
            

            if(neighbourPos.x < 0 || neighbourPos.x > gridHeight + 1 || neighbourPos.y < 0 || neighbourPos.y > gridWidth + 1){
                continue;
            }

            if(grid[neighbourPos] == end){
                neighbours.Add(end);
                return neighbours;
            }
            if (grid.ContainsKey(neighbourPos) && gridValue[(int)neighbourPos.x, (int)neighbourPos.y] == -1){
                neighbours.Add(grid[neighbourPos]);
            }
        }

        return neighbours;
    }

    private void DrawLine(List<Tile> path){
        Quaternion quaternion = Quaternion.identity;
        for(int i = 0;i < path.Count - 1;i++){
            Vector3 currentPos = new Vector3(path[i].getPosX(), path[i].getPosY(), 0);
            switch (new Vector2(path[i + 1].GetX(), path[i + 1].GetY()) - new Vector2(path[i].GetX(), path[i].GetY()))
            {
                case Vector2 v when v == new Vector2(-1, 0): // Up              
                    currentPos.y += lineOffset;
                    quaternion = Quaternion.Euler(0, 0, 0);
                    break;
                case Vector2 v when v == new Vector2(1, 0): // Down
                    currentPos.y -= lineOffset;
                    quaternion = Quaternion.Euler(0, 0, 0);
                    break;
                case Vector2 v when v == new Vector2(0, -1): // Left
                    currentPos.x -= lineOffset;
                    quaternion = Quaternion.Euler(0, 0, 90);
                    break;
                case Vector2 v when v == new Vector2(0, 1): // Right
                    currentPos.x += lineOffset;
                    quaternion = Quaternion.Euler(0, 0, 90);
                    break;
            }
            GameObject line = Instantiate(lineObject, currentPos, quaternion);
        }
    }

    private void ClearLines(){
        GameObject[] lines = GameObject.FindGameObjectsWithTag("Line");
        foreach(GameObject line in lines){
            Destroy(line);
        }
    }
    
}
