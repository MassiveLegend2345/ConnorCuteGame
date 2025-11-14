using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TMP_Text timerText;
    public TMP_Text scoreText;

    private void Update()
    {
        if (GameManager.Instance == null) return;

        timerText.text = "TIME: " + Mathf.CeilToInt(GameManager.Instance.timeRemaining);
        scoreText.text = "SCORE: " + GameManager.Instance.score;
    }
}
