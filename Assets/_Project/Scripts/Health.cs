using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHp = 20;
    public int goldReward = 10;

    private int hp;
    private bool diedForReward;
    private bool removedFromCounter;

    public float Normalized => maxHp <= 0 ? 0f : (float)hp / maxHp;

    void Awake()
    {
        hp = maxHp;
        GameVisuals.StyleEnemy(transform);
    }

    public void SetWaveVisuals(int wave)
    {
        GameVisuals.StyleEnemy(transform, wave);
    }

    public void AddMaxHp(int amount)
    {
        maxHp += amount;
        hp += amount;
    }

    public void TakeDamage(int amount)
    {
        if (diedForReward) return;

        hp -= amount;

        if (hp <= 0)
        {
            diedForReward = true;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddGold(goldReward);
                GameManager.Instance.AddKill();
            }

            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (removedFromCounter) return;
        removedFromCounter = true;

        if (GameManager.Instance != null)
            GameManager.Instance.RegisterEnemyRemoved();
    }
}
