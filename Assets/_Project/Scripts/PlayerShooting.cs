using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireCooldown = 0.3f;

    private float timer;

    void Update()
    {
        timer -= Time.deltaTime;

        if (Keyboard.current == null) return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame && timer <= 0f)
        {
            Shoot();
            timer = fireCooldown;
        }
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, transform.rotation);
    }
}