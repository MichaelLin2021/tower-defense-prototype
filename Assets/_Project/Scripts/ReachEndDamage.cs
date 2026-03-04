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
        // EnemyPath stops moving when currentWaypoint >= waypoints.Length.
        // We don't have direct access to currentWaypoint (private),
        // so we detect "finished" by checking distance to last waypoint and movement stop.
        // Simple, robust workaround: if we're extremely close to the last waypoint.
        if (mover.waypoints == null || mover.waypoints.Length == 0) return false;

        Transform last = mover.waypoints[mover.waypoints.Length - 1];
        return Vector3.Distance(transform.position, last.position) < 0.15f;
    }
}