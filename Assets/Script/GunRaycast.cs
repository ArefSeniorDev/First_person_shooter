using UnityEngine;

public class GunRaycast : MonoBehaviour
{
	public float range = 100f;
	public float damage = 25f;
	public Camera fpsCam;
	public LayerMask enemyLayer;
	public ParticleSystem muzzleFlash;
	public WeaponViewFix weaponViewFix;

	public bool destroyEnemyOnHit = true; 

	void Start()
	{
		if (fpsCam == null)
		{
			fpsCam = Camera.main;
		}

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
		if (fpsCam == null)
		{
			return;
		}

		if (weaponViewFix != null)
		{
			weaponViewFix.PlayMuzzleFlash();
		}
		else if (muzzleFlash != null)
		{
			muzzleFlash.Stop();
			muzzleFlash.Clear();
			muzzleFlash.Emit(9);
		}

		Ray ray = fpsCam.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, range, enemyLayer))
		{
			EnemyHealth enemyHealth = hit.collider.GetComponentInParent<EnemyHealth>();

			if (enemyHealth != null)
			{
				enemyHealth.TakeDamage(damage);
			}
		}
	}
}