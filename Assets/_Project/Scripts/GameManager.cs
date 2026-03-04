using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Economy")]
    public int startGold = 100;
    public int Gold { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI goldText;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        Gold = startGold;
        UpdateUI();
    }

    public bool TrySpend(int amount)
    {
        if (Gold < amount) return false;
        Gold -= amount;
        UpdateUI();
        return true;
    }

    public void AddGold(int amount)
    {
        Gold += amount;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (goldText != null)
            goldText.text = $"Gold: {Gold}";
    }
}