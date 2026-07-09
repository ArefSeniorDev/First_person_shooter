using UnityEngine;

public class WeaponViewFix : MonoBehaviour
{
    public Transform weaponRoot;
    public Transform aimPose;
    public Camera fpsCamera;
    public ParticleSystem muzzleFlash;
    public Renderer[] whiteArtifactRenderers;
    public bool autoHideWhiteNamedChildren = true;
    public bool autoFindWeaponParticles = true;
    public float hipFov = 60f;
    public float aimFov = 42f;
    public float aimSmooth = 10f;
    public Vector3 hipLocalPosition;
    public Vector3 aimLocalPosition = new Vector3(0f, -0.08f, 0.18f);

    private bool isAiming;
    private ParticleSystem[] weaponParticles;
    private Material muzzleFlashMaterial;

    void Start()
    {
        if (weaponRoot == null)
        {
            weaponRoot = transform;
        }

        if (fpsCamera == null)
        {
            fpsCamera = Camera.main;
        }

        hipLocalPosition = weaponRoot.localPosition;
        CacheWeaponParticles();
        StopStartupMuzzleFlash();
        HideWhiteArtifacts();
    }

    void Update()
    {
        // Hold right mouse button to move the gun into aim-down-sights mode.
        isAiming = Input.GetMouseButton(1);

        Vector3 targetPosition = aimPose != null ? aimPose.localPosition : aimLocalPosition;
        weaponRoot.localPosition = Vector3.Lerp(weaponRoot.localPosition, isAiming ? targetPosition : hipLocalPosition, Time.deltaTime * aimSmooth);

        if (fpsCamera != null)
        {
            fpsCamera.fieldOfView = Mathf.Lerp(fpsCamera.fieldOfView, isAiming ? aimFov : hipFov, Time.deltaTime * aimSmooth);
        }
    }

    public void PlayMuzzleFlash()
    {
        // Emit a short burst only when firing; this prevents white startup dots in front of the gun.
        if (weaponParticles != null && weaponParticles.Length > 0)
        {
            for (int i = 0; i < weaponParticles.Length; i++)
            {
                if (weaponParticles[i] == null)
                {
                    continue;
                }

                weaponParticles[i].Stop();
                weaponParticles[i].Clear();
                weaponParticles[i].Emit(9);
            }
        }
        else if (muzzleFlash != null)
        {
            muzzleFlash.Stop();
            muzzleFlash.Clear();
            muzzleFlash.Emit(9);
        }
    }

    public void HideWhiteArtifacts()
    {
        // Assign scope glints, placeholder planes, or unwanted white meshes here to hide them at runtime.
        if (whiteArtifactRenderers == null)
        {
            whiteArtifactRenderers = new Renderer[0];
        }

        for (int i = 0; i < whiteArtifactRenderers.Length; i++)
        {
            if (whiteArtifactRenderers[i] != null)
            {
                whiteArtifactRenderers[i].enabled = false;
            }
        }

        if (!autoHideWhiteNamedChildren || weaponRoot == null)
        {
            return;
        }

        Renderer[] renderers = weaponRoot.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            string lowerName = renderers[i].name.ToLower();
            if (lowerName.Contains("white") || lowerName.Contains("artifact") || lowerName.Contains("flash_plane"))
            {
                renderers[i].enabled = false;
            }
        }
    }

    private void StopStartupMuzzleFlash()
    {
        if (weaponParticles == null || weaponParticles.Length == 0)
        {
            return;
        }

        for (int i = 0; i < weaponParticles.Length; i++)
        {
            if (weaponParticles[i] == null)
            {
                continue;
            }

            ConfigureMuzzleParticle(weaponParticles[i]);
            weaponParticles[i].Stop();
            weaponParticles[i].Clear();
        }
    }

    private void CacheWeaponParticles()
    {
        if (!autoFindWeaponParticles || weaponRoot == null)
        {
            weaponParticles = muzzleFlash != null ? new ParticleSystem[] { muzzleFlash } : new ParticleSystem[0];
            return;
        }

        weaponParticles = weaponRoot.GetComponentsInChildren<ParticleSystem>(true);
        if ((weaponParticles == null || weaponParticles.Length == 0) && muzzleFlash != null)
        {
            weaponParticles = new ParticleSystem[] { muzzleFlash };
        }

        if (muzzleFlash == null && weaponParticles != null && weaponParticles.Length > 0)
        {
            muzzleFlash = weaponParticles[0];
        }
    }

    private void ConfigureMuzzleParticle(ParticleSystem particle)
    {
        ParticleSystem.MainModule main = particle.main;
        main.playOnAwake = false;
        main.loop = false;
        main.duration = 0.08f;
        main.startLifetime = 0.055f;
        main.startSpeed = 0.22f;
        main.startSize = 0.16f;
        main.startColor = new Color(1f, 0.72f, 0.22f, 0.95f);
        main.maxParticles = 18;

        ParticleSystem.EmissionModule emission = particle.emission;
        emission.rateOverTime = 0f;

        ParticleSystem.ShapeModule shape = particle.shape;
        shape.angle = 10f;
        shape.radius = 0.025f;

        ParticleSystemRenderer particleRenderer = particle.GetComponent<ParticleSystemRenderer>();
        if (particleRenderer != null)
        {
            if (muzzleFlashMaterial == null)
            {
                Shader shader = Shader.Find("Particles/Additive");
                if (shader != null)
                {
                    muzzleFlashMaterial = new Material(shader);
                    muzzleFlashMaterial.color = new Color(1f, 0.62f, 0.16f, 0.85f);
                }
            }

            if (muzzleFlashMaterial != null)
            {
                particleRenderer.material = muzzleFlashMaterial;
            }
        }
    }
}
