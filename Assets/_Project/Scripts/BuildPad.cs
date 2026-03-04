using UnityEngine;

public class BuildPad : MonoBehaviour
{
    public GameObject towerPrefab;
    public int towerCost = 50;

    private GameObject currentTower;

    // Call this from a raycast click system or keep OnMouseDown if it works
    public void TryBuild()
    {
        if (currentTower != null) return;

        if (GameManager.Instance == null) { Debug.Log("No GameManager in scene"); return; }
        if (!GameManager.Instance.TrySpend(towerCost)) { Debug.Log("Not enough gold"); return; }
        if (towerPrefab == null) { Debug.Log("Tower prefab not assigned"); return; }

        Vector3 spawnPos = transform.position + Vector3.up * 0.6f;
        currentTower = Instantiate(towerPrefab, spawnPos, Quaternion.identity);
    }

    private void OnMouseDown()
    {
        TryBuild();
    }
}