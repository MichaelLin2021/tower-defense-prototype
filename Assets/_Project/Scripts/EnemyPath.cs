using UnityEngine;

public class EnemyPath : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 2f;

    int currentWaypoint = 0;
    float baseY = 0.5f;

    public bool FinishedPath => waypoints != null && currentWaypoint >= waypoints.Length;

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        if (FinishedPath) return;

        Transform target = waypoints[currentWaypoint];

        // Ignore Y when moving
        Vector3 currentFlat = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 targetFlat = new Vector3(target.position.x, 0, target.position.z);

        Vector3 nextFlat = Vector3.MoveTowards(
            currentFlat,
            targetFlat,
            speed * Time.deltaTime);

        // Add visible bounce only on Y
        float bounceY = baseY + Mathf.Sin(Time.time * 8f) * 0.1f;

        transform.position = new Vector3(nextFlat.x, bounceY, nextFlat.z);

        // Spin animation
        transform.Rotate(0, 120 * Time.deltaTime, 0);

        // Check distance ignoring Y
        if (Vector3.Distance(nextFlat, targetFlat) < 0.1f)
        {
            currentWaypoint++;
        }
    }
}
