using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRow : MonoBehaviour
{
    [SerializeField]
    private TileCell[] cells;

    public TileCell[] Cells { get => cells; set => cells = value; }
}
