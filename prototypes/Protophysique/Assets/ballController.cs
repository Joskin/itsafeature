using UnityEngine;

public class ballController : MonoBehaviour {
	
	public float speed = 100f, gravity = 10f, jumpPower = 10f;
	
	private float v, h;
	private Vector3 cameraDirection, cameraOrthoDirection, ballForces;
	private Transform cameraTransform;
	private bool isGrounded = false, isJumping = false;
	
	void Update () {
		v = Input.GetAxis("Vertical");
		h = Input.GetAxis("Horizontal");
		cameraTransform = cameraController.cameraTransform;
		
		cameraDirection = cameraTransform.forward;
		cameraDirection.y = 0f;
		cameraDirection.Normalize();
		cameraOrthoDirection = cameraTransform.right;
		cameraOrthoDirection.y = 0f;
		cameraOrthoDirection.Normalize();
		ballForces = v * cameraDirection + h * cameraOrthoDirection;
		ballForces.Normalize();
		
		/*Debug.DrawRay(transform.position, cameraDirection * 1.4f, Color.magenta);
		Debug.DrawRay(transform.position, cameraOrthoDirection * 1.4f, Color.yellow);
		Debug.DrawRay(transform.position, ballForces, Color.black);*/
		
		ballForces *= speed;
		
		rigidbody.AddForce(0f, -gravity, 0f, ForceMode.Acceleration);// Custom gravity
		rigidbody.AddForce(ballForces, ForceMode.Force);
		
		if(Input.GetButtonDown("Jump")) {
			isJumping = true;
		}
		else if(Input.GetButtonUp("Jump")) {
			isJumping = false;
		}
		
		if(isJumping && isGrounded) {
			rigidbody.AddForce(0f, jumpPower, 0f, ForceMode.Impulse);
			isJumping = false;
		}
	}
	
	void OnCollisionEnter(Collision collision) {
		if(collision.gameObject.layer == 8) {
			isGrounded = true;
		}
	}
	
	void OnCollisionExit(Collision collision) {
		if(collision.gameObject.layer == 8) {
			isGrounded = false;
		}
	}
}
