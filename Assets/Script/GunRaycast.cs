using UnityEngine;

public class GunRaycast : MonoBehaviour
{
    public float range = 100f;
    public Camera fpsCam;
    public LayerMask enemyLayer;
    public ParticleSystem muzzleFlash;
    public WeaponViewFix weaponViewFix;
    public bool destroyEnemyOnHit = true;

    void Start()
    {
        // Camera.main and WeaponViewFix are resolved here so the Inspector can stay simple.
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
            Debug.LogWarning("GunRaycast needs an FPS camera.");
            return;
        }

        // Muzzle flash belongs to the shot itself, not only to successful hits.
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
            Debug.Log("Hit: " + hit.collider.name);

            // FBX enemies often have colliders on child bones, so remove the object that owns EnemyAI.
            EnemyAI enemy = hit.collider.GetComponentInParent<EnemyAI>();
            GameObject objectToDestroy = enemy != null ? enemy.gameObject : hit.collider.gameObject;

            KillCounter killCounter = FindObjectOfType<KillCounter>();
            if (killCounter != null)
            {
                killCounter.AddKill();
            }

            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.EnemyKilled();
            }

            if (destroyEnemyOnHit)
            {
                Destroy(objectToDestroy);
            }
        }
    }
}
