using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public TileStateSO state{get;private set;}
    public TileCell cell{get;private set;}

    public bool locked {get;set;}
    public int number;
    [SerializeField]
    private Image background;
    [SerializeField]
    private TextMeshProUGUI text;

    public void SetState(TileStateSO state, int number){
        this.state = state;
        this.number = number;

        background.color = state.backgroundColor;
        text.color = state.textColor;
        text.text = number.ToString();
    }

    public void Spawn(TileCell cell)
    {
        if(this.cell != null ){
            this.cell.tile = null;
        }
        this.cell = cell;
        this.cell.tile = this;
        
        transform.position = cell.transform.position;
    }

    public void MoveTo(TileCell cell)
    {
        if(this.cell != null ){
            this.cell.tile = null;
        }
        this.cell = cell;
        this.cell.tile = this;
        
        StartCoroutine(Animate(cell.transform.position,false));
    }

    public void Merge(TileCell cell){
         if(this.cell != null ){
            this.cell.tile = null;
        }
        this.cell = null;
        cell.tile.locked = true;
        StartCoroutine(Animate(cell.transform.position,true));
    }

    private IEnumerator Animate(Vector3 to, bool merging){
        float elapsed = 0f;
        float duration = 0.1f;
        Vector3 form = transform.position;
        while (elapsed < duration){
            transform.position = Vector3.Lerp(form, to , elapsed/duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = to;

        if(merging){
            Destroy(gameObject);
        }
    }
}
