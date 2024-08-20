using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    [SerializeField]
    private TileCell[] cells;
    [SerializeField]
    private TileRow[] rows;

    public int size => Cells.Length;
    public int height => Rows.Length;
    public int width => size/height;

    public TileCell[] Cells { get => cells;}
    public TileRow[] Rows { get => rows;}

    private void Start() {
        // xét tạo độ cho từng ô 
        for (int y = 0; y < Rows.Length; y++) {
            for(int x = 0; x < Rows[y].Cells.Length; x++) {
                Rows[y].Cells[x].coordinates = new Vector2Int(x, y);
            }
        }
    }
    public TileCell GetCell(int x, int y) {
        if( x>= 0 && x < width && y >=0 && y < height) return Rows[y].Cells[x];
        else return null;
    }
    public TileCell GetCell(Vector2Int coordinates) {
        return GetCell(coordinates.x, coordinates.y);
    }
    // lấy ra ô bên cạnh của cell  
    public TileCell GetAdjacentCell(TileCell cell ,Vector2Int direction){
        Vector2Int coordinates = cell.coordinates;
        coordinates.x += direction.x;
        coordinates.y -= direction.y;

        return GetCell(coordinates);
    }
    //lấy ra cell ngẫu nhiên mà rỗng, ko có trả về null
    public TileCell GetRandomeEmtyCell(){
        int index = Random.Range(0, cells.Length);
        int startingIndex = index;
        while (cells[index].occupied){
            index++;
            if(index >= cells.Length){
                index = 0;
            }
            if(index == startingIndex){
                return null;
            }
        }
        return cells[index];
    }
}
