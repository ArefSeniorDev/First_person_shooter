using UnityEngine;

public class GunShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletForce = 20f;
    public AudioSource gunAudio;
    public WeaponViewFix weaponViewFix;

    void Start()
    {
        // Auto-find the view helper when both scripts are placed on the weapon.
        if (weaponViewFix == null)
        {
            weaponViewFix = GetComponent<WeaponViewFix>();
        }
    }

    void Update()
    {
        // Fire1 is left mouse by default in Unity input settings.
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Bullet prefab or fire point not assigned.");
            return;
        }

        // Play sound and muzzle flash immediately so missed shots still feel responsive.
        if (gunAudio != null)
        {
            gunAudio.Play();
        }

        if (weaponViewFix != null)
        {
            weaponViewFix.PlayMuzzleFlash();
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation) as GameObject;
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        // Prefer Rigidbody velocity, but the Bullet script still moves the projectile if no Rigidbody exists.
        if (rb != null)
        {
            rb.velocity = firePoint.forward * bulletForce;
        }
    }
}
