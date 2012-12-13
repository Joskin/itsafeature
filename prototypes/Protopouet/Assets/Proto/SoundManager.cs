using UnityEngine;

public class SoundManager : MonoBehaviour {
	
	public AudioSource rollSound, hitSound;
	
	private float kartSpeed;
	
	void Start() {
		CollisionEvents.Hurt += Hurt;
	}
	
	void Update () {
		kartSpeed = Mathf.Abs(Kart.currentSpeed);
		rollSound.pitch = kartSpeed;
		rollSound.volume = kartSpeed;
	}
	
	void Hurt() {
		hitSound.volume = kartSpeed;
		hitSound.pitch = Random.Range(1f, 1.5f) * Mathf.Max(0.3f, kartSpeed);
		hitSound.Play();
	}
}
