using UnityEngine;

public class BlobController : MonoBehaviour {
	
	private float elevation;
	
	void Start() {
		elevation = transform.localPosition.y;
	}
	
	void Update() {
		transform.position = transform.parent.position + Vector3.up * elevation;
		transform.rotation = Quaternion.LookRotation(-Vector3.up, transform.parent.forward);
	}
}
