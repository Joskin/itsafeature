using UnityEngine;

public class WeaponManager : MonoBehaviour {

	public GameObject Kart, WeaponProjectile;
	
	private GameObject projectile;
	
	void Update() {
		if(projectile != null) {
			projectile.transform.Translate(0f, 0f, 0.35f);
		}
		if(Input.GetButtonDown("Jump")) {
			Destroy(projectile);
			projectile = (GameObject)Instantiate(WeaponProjectile);
			projectile.transform.localPosition = Kart.transform.localPosition;
			projectile.transform.localRotation = Kart.transform.localRotation;
		}
	}
}
