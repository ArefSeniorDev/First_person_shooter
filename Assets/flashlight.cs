using UnityEngine;

public class Flashlight : MonoBehaviour
{
	public GameObject spotLight; 
	private bool isOn = false;

	void Start()
	{
		if (spotLight == null)
		{
			Transform lightTransform = transform.Find("Spotlight"); 
			if (lightTransform != null)
			{
				spotLight = lightTransform.gameObject;
			}
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.F))
		{
			if (spotLight != null)
			{
				isOn = !isOn;
				spotLight.SetActive(isOn);
			}

		}
	}
}