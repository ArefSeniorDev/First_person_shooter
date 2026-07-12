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
        if (weaponViewFix == null)
        {
            weaponViewFix = GetComponent<WeaponViewFix>();
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            return;
        }

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

        if (rb != null)
        {
            rb.velocity = firePoint.forward * bulletForce;
        }
    }
}
