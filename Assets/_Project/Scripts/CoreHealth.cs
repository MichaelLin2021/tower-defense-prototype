using UnityEngine;

public class CoreHealth : MonoBehaviour
{
    public int maxHp = 10;
    private int hp;

    void Awake()
    {
        hp = maxHp;
        GameVisuals.StyleCore(transform);
        Debug.Log($"CORE HP: {hp}");

        if (GameManager.Instance != null)
            GameManager.Instance.UpdateCoreHealth(hp, maxHp);
    }

    public void Damage(int amount)
    {
        if (hp <= 0) return;

        hp -= amount;
        Debug.Log($"CORE HIT! HP: {hp}");

        if (GameManager.Instance != null)
            GameManager.Instance.UpdateCoreHealth(hp, maxHp);

        if (hp <= 0)
        {
            Debug.Log("GAME OVER");

            if (GameManager.Instance != null)
                GameManager.Instance.GameOver();
            else
                Time.timeScale = 0f;
        }
    }

    public int GetHp() => hp;
}
