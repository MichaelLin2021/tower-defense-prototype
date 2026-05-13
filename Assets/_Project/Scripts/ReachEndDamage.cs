using UnityEngine;

public class ReachEndDamage : MonoBehaviour
{
    public CoreHealth core;
    public int damage = 1;

    private EnemyPath mover;

    void Awake()
    {
        mover = GetComponent<EnemyPath>();
    }

    void Update()
    {
        if (core == null || mover == null) return;

        // If enemy finished all waypoints, hit the core once and despawn
        if (HasFinishedPath())
        {
            core.Damage(damage);
            Destroy(gameObject);
        }
    }

    private bool HasFinishedPath()
    {
        if (mover.FinishedPath) return true;
        if (mover.waypoints == null || mover.waypoints.Length == 0) return false;

        Transform last = mover.waypoints[mover.waypoints.Length - 1];
        Vector2 currentFlat = new Vector2(transform.position.x, transform.position.z);
        Vector2 lastFlat = new Vector2(last.position.x, last.position.z);
        return Vector2.Distance(currentFlat, lastFlat) < 0.25f;
    }
}
