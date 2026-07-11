using System;
using UnityEngine;

namespace UnityStandardAssets.Water
{
    [ExecuteInEditMode]
    public class WaterBasic : MonoBehaviour
    {
        private const string WaveSpeedProperty = "WaveSpeed";
        private const string LegacyWaveSpeedProperty = "_WaveSpeed";
        private const string WaveScaleProperty = "_WaveScale";

        void Update()
        {
            Renderer r = GetComponent<Renderer>();
            if (!r)
            {
                return;
            }
            Material mat = r.sharedMaterial;
            if (!mat)
            {
                return;
            }

            string waveSpeedProperty;
            if (!TryGetWaveSpeedProperty(mat, out waveSpeedProperty) || !mat.HasProperty(WaveScaleProperty))
            {
                return;
            }

            Vector4 waveSpeed = mat.GetVector(waveSpeedProperty);
            float waveScale = mat.GetFloat(WaveScaleProperty);
            float t = Time.time / 20.0f;

            Vector4 offset4 = waveSpeed * (t * waveScale);
            Vector4 offsetClamped = new Vector4(Mathf.Repeat(offset4.x, 1.0f), Mathf.Repeat(offset4.y, 1.0f),
                Mathf.Repeat(offset4.z, 1.0f), Mathf.Repeat(offset4.w, 1.0f));
            mat.SetVector("_WaveOffset", offsetClamped);
        }

        private static bool TryGetWaveSpeedProperty(Material mat, out string propertyName)
        {
            propertyName = WaveSpeedProperty;
            if (mat.HasProperty(WaveSpeedProperty))
            {
                return true;
            }

            propertyName = LegacyWaveSpeedProperty;
            return mat.HasProperty(LegacyWaveSpeedProperty);
        }
    }
}
