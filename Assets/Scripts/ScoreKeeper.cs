using Assets.Scripts;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    public static int score { get; private set; }

    private float _lastEnemyKillTime;
    private int _streakCount;
    private float _streakExpiry = 1;

    private void Start()
    {
        Enemy.OnDeathStatic += OnEnemyKilled;
        FindObjectOfType<Player>().OnDeath += OnPlayerDeath;
    }

    void OnEnemyKilled()
    {

        if (Time.time < _lastEnemyKillTime+_streakExpiry)
        {
            _streakCount++;
        }
        else
        {
            _streakCount = 0;
        }

        _lastEnemyKillTime = Time.time;

        score += 5 + (int)Mathf.Pow(2,_streakCount);

    }

    void OnPlayerDeath()
    {
        Enemy.OnDeathStatic -= OnEnemyKilled;
    }

}
