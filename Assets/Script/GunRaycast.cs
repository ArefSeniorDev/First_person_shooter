using UnityEngine;
using UnityEngine.UI;

public class GunRaycast : MonoBehaviour
{
    public float range = 100f;
    public Camera fpsCam;
    public LayerMask enemyLayer;
    public ParticleSystem muzzleFlash;
    private int _point = 0;
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Ray ray = fpsCam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, range, enemyLayer))
        {
            muzzleFlash.Play();
            Debug.Log("Hit: " + hit.collider.name);
            Destroy(hit.collider.gameObject);
            FindObjectOfType<KillCounter>().AddKill();
        }
    }
}
