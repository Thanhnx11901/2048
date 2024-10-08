using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Button tryAgainBtn;
    [SerializeField]
    private Button NewGameBtn;
    [SerializeField]
    private TileBoard board;
    [SerializeField]
    private CanvasGroup gameOver;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI hiscoreText;

    private int score;

    private void Awake() {
        tryAgainBtn.onClick.AddListener(NewGame);
        NewGameBtn.onClick.AddListener(NewGame);
    }
    private void Start() {
        NewGame();
    }

    public void NewGame()
    {
        SetScore(0);
        hiscoreText.text = LoadHiscore().ToString();

        gameOver.alpha = 0f;
        gameOver.interactable = false;

        board.ClearBoard();
        board.CreateTile();
        board.CreateTile();
        board.enabled = true;
    }
    public void GameOver(){
        gameOver.interactable = true;
        board.enabled = false;
        StartCoroutine(Fade(gameOver, 1f, 1f));
    }
    private IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay){
        yield return new WaitForSeconds(delay);
        float elapsed = 0f;
        float duration = 0.5f;
        float from = canvasGroup.alpha;
        while (elapsed < duration){
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed/duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = to;
    }
    public void IncreaseScore(int points){
        SetScore(score+ points);
    }
    private void SetScore(int score){
        this.score = score;
        this.scoreText.text = score.ToString();
        SaveHiscore();
    }
    private void SaveHiscore(){
        int hiscore = LoadHiscore();
        if(score > hiscore){
            PlayerPrefs.SetInt("hiscore",score);
        }
    }
    private int LoadHiscore(){
        return PlayerPrefs.GetInt("hiscore");
    }
}
