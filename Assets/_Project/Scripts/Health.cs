using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHp = 20;
    public int goldReward = 10;

    private int hp;

    public float Normalized => maxHp <= 0 ? 0f : (float)hp / maxHp;

    void Awake()
    {
        hp = maxHp;
    }

    public void TakeDamage(int amount)
    {
        hp -= amount;

        if (hp <= 0)
        {
            Debug.Log("Enemy died, giving gold");

            if (GameManager.Instance != null)
                GameManager.Instance.AddGold(goldReward);

            Destroy(gameObject);
        }
    }
}