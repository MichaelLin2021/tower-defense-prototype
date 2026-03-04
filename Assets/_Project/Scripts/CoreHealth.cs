using UnityEngine;

public class CoreHealth : MonoBehaviour
{
    public int maxHp = 10;
    private int hp;

    void Awake()
    {
        hp = maxHp;
        Debug.Log($"CORE HP: {hp}");
    }

    public void Damage(int amount)
    {
        hp -= amount;
        Debug.Log($"CORE HIT! HP: {hp}");

        if (hp <= 0)
        {
            Debug.Log("GAME OVER");
            Time.timeScale = 0f;
        }
    }

    public int GetHp() => hp;
}