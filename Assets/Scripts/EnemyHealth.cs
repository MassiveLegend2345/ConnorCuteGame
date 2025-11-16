/*
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    public int maxHits = 3;
    public int currentHits = 0; 

    public int scorePerKill = 10;
    public float timePerKill = 2f;

    public float killSpeedBoost = 1.5f;
    public float killPunchBoost = 1.5f;
    public float boostDuration = 3f;

    public GameObject pointsPopupPrefab;
    public string pointsText = "+10";
    public UnityEngine.UI.Image screenFlash;
    public float flashDuration = 0.7f;

    public void TakeHit()
    {
        currentHits++;
        Debug.Log($"Enemy took hit! {currentHits}/{maxHits}");
    }

    public bool IsDead()
    {
        bool dead = currentHits >= maxHits;
        if (dead) Debug.Log("Enemy is dead!");
        return dead;
    }
}
*/