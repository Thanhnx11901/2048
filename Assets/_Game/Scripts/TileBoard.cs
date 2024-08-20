using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    [SerializeField]
    private Tile tilePrefab;
    [SerializeField]
    private TileStateSO[] tileStates;
    [SerializeField]
    private TileGrid grid;
    [SerializeField]
    private GameManager gameManager;
    private List<Tile> tiles = new List<Tile>();

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private Vector2 swipeDirection;
    private float swipeAngle;
    private bool waiting;
    public void CreateTile()
    {
        Tile tile = Instantiate(tilePrefab, grid.transform);
        tile.SetState(tileStates[0],2);
        tile.Spawn(grid.GetRandomeEmtyCell());
        tiles.Add(tile);
    }
    public void ClearBoard(){
        for(int i = 0; i < grid.Cells.Length; i++){
            grid.Cells[i].tile = null;
        }
        for (int i = 0; i < tiles.Count; i++){
            Destroy(tiles[i].gameObject);
        }
        tiles.Clear();
    }
    private void Update() {
        if (waiting) return;
        if (Input.GetMouseButtonDown(0))
        {
            startTouchPosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            endTouchPosition = Input.mousePosition;
            DetectSwipeAngleDirection();
        }
    }

    private void DetectSwipeAngleDirection()
    {
        swipeDirection = endTouchPosition - startTouchPosition;

        if (swipeDirection.magnitude > 50)
        {
            swipeAngle = Mathf.Atan2(swipeDirection.y, swipeDirection.x) * Mathf.Rad2Deg;

            if (swipeAngle >= -45 && swipeAngle <= 45)
            {
                MoveTiles(Vector2Int.right, grid.width - 2, -1, 0, 1);
            }
            else if (swipeAngle > 45 && swipeAngle < 135)
            {
                MoveTiles(Vector2Int.up, 0, 1, 1, 1);
            }
            else if (swipeAngle >= 135 || swipeAngle <= -135)
            {
                MoveTiles(Vector2Int.left, 1, 1, 0, 1);
            }
            else if (swipeAngle < -45 && swipeAngle > -135)
            {
                MoveTiles(Vector2Int.down, 0, 1, grid.height - 2, -1);
            }
        }
    }
    private void MoveTiles(Vector2Int direction, int startX, int incrementX, int startY, int incrementY){
        bool changed = false;
        // vòng lặp duyệt cách một dòng không cần thiết với hướng truyền vào 
        for(int x = startX; x >= 0 && x < grid.width; x += incrementX){
            for(int y = startY; y >= 0 && y < grid.height; y += incrementY){
                TileCell cell = grid.GetCell(x, y);
                if(cell.occupied){
                    changed |= MoveTiles(cell.tile,direction);
                }
            }
        }
        if(changed){
            StartCoroutine(WaitForChanges());
        }
    }

    private bool MoveTiles(Tile tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = grid.GetAdjacentCell(tile.cell, direction);
        // vòng lặp lấy cell trống theo hướng và merging nếu tương ứng 
        while(adjacent != null){
            if(adjacent.occupied){
                // TODO: merging
                if(CanMerge(tile, adjacent.tile)){
                    Merge(tile,adjacent.tile);
                    return true;
                }
                break;
            }
            newCell = adjacent;
            adjacent = grid.GetAdjacentCell(adjacent, direction);
        }
        if(newCell != null){
            tile.MoveTo(newCell);
            return true;
        }
        return false;
    }
    private bool CanMerge(Tile a, Tile b){
        return a.number == b.number && !b.locked;
    }
    private void Merge(Tile a, Tile b){
        tiles.Remove(a);
        a.Merge(b.cell);

        int index = Math.Clamp(IndexOf(b.state) + 1, 0, tileStates.Length -1);
        int number = b.number * 2;
        b.SetState(tileStates[index], number);

        gameManager.IncreaseScore(number);

    }
    private int IndexOf(TileStateSO state){
        for(int i = 0; i < tileStates.Length; i++){
            if(state == tileStates[i]){
                return i;
            }
        }
        return -1;
    }

    private IEnumerator WaitForChanges(){
        waiting = true;
        yield return new WaitForSeconds(0.1f);
        waiting = false;
        for(int i = 0; i < tiles.Count; i++){
            tiles[i].locked = false;
        }

        if(tiles.Count != grid.size){
            CreateTile();
        }
        if(CheckForGameOver()){
            gameManager.GameOver();
        }
    }
    private bool CheckForGameOver(){
        if(tiles.Count != grid.size){
            return false;
        }
        for(int i = 0; i < tiles.Count ; i++){
            TileCell up = grid.GetAdjacentCell(tiles[i].cell, Vector2Int.up);
            TileCell down = grid.GetAdjacentCell(tiles[i].cell, Vector2Int.down);
            TileCell left = grid.GetAdjacentCell(tiles[i].cell, Vector2Int.left);
            TileCell right = grid.GetAdjacentCell(tiles[i].cell, Vector2Int.right);

            // check từng ô bên cạnh có thể merge thì chưa game over
            if(up != null && CanMerge(tiles[i], up.tile)){
                return false;
            }
            if(down != null && CanMerge(tiles[i], down.tile)){
                return false;
            }
            if(left != null && CanMerge(tiles[i], left.tile)){
                return false;
            }
            if(right != null && CanMerge(tiles[i], right.tile)){
                return false;
            }
        }
        return true;

    }
}
