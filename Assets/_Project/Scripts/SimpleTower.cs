using System.Collections.Generic;
using UnityEngine;

public class SimpleTower : MonoBehaviour
{
    public int damage = 5;
    public float fireInterval = 0.5f;

    private readonly List<Health> targets = new();
    private float timer;

    void Start()
    {
        GameVisuals.StyleTower(transform, damage, fireInterval);
    }

    void Update()
    {
        timer -= Time.deltaTime;

        // remove destroyed enemies
        targets.RemoveAll(t => t == null);

        if (targets.Count == 0 || timer > 0f) return;

        // attack nearest
        Health nearest = targets[0];
        float best = (nearest.transform.position - transform.position).sqrMagnitude;

        for (int i = 1; i < targets.Count; i++)
        {
            float d = (targets[i].transform.position - transform.position).sqrMagnitude;
            if (d < best) { best = d; nearest = targets[i]; }
        }

        nearest.TakeDamage(damage);
        timer = fireInterval;
    }

    void OnTriggerEnter(Collider other)
    {
        Health hp = other.GetComponent<Health>();
        if (hp != null && !targets.Contains(hp))
            targets.Add(hp);
    }

    void OnTriggerExit(Collider other)
    {
        Health hp = other.GetComponent<Health>();
        if (hp != null)
            targets.Remove(hp);
    }
}
