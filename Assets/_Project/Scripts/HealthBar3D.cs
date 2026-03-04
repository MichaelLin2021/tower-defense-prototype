using UnityEngine;

public class HealthBar3D : MonoBehaviour
{
    public Health health;
    public Transform barFG;

    private Vector3 fgStartScale;
    private Vector3 fgStartLocalPos;

    void Awake()
    {
        if (barFG != null)
        {
            fgStartScale = barFG.localScale;
            fgStartLocalPos = barFG.localPosition;
        }
    }

    void LateUpdate()
    {
        if (health == null || barFG == null) return;

        // Always face camera (billboard)
        Camera cam = Camera.main;
        if (cam != null)
        {
            // face camera; this method avoids flipping issues
            transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward,
                             cam.transform.rotation * Vector3.up);
        }

        // Scale based on HP
        float t = Mathf.Clamp01(health.Normalized);
        barFG.localScale = new Vector3(fgStartScale.x * t, fgStartScale.y, fgStartScale.z);

        // Anchor left side so it shrinks to the right
        float halfLoss = (fgStartScale.x - barFG.localScale.x) * 0.5f;
        barFG.localPosition = new Vector3(fgStartLocalPos.x - halfLoss, fgStartLocalPos.y, fgStartLocalPos.z);
    }
}