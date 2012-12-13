using UnityEngine;

public class CameraManager : MonoBehaviour {

	public GameObject cameraFollow, cameraTop;
	public float rollAngle = 10f, yawAngle = 10f, pitchAngle = 5f, speed = 10f, shakeIntensity = 0.1f, shakeSpeed = 20f;
	
	private float currentRoll = 0f, targetRoll, currentYaw = 0f, targetYaw, initPitch, currentPitch = 0f, targetPitch;
	private float rx, ry, rz, kartSpeed;
	private Vector3 initPosFollow, initPosTop;
	private bool isColliding = false;
	
	void Start () {
		CollisionEvents.Hurt += Hurt;
		CollisionEvents.HurtLeave += HurtLeave;
		cameraTop.active = false;
		initPitch = cameraFollow.transform.rotation.eulerAngles.x;// Init as in scene
		initPosFollow = cameraFollow.transform.localPosition;
		initPosTop = cameraTop.transform.localPosition;
	}
	
	void Update () {
		kartSpeed = Kart.currentSpeed;
		
		// Better way would be to use common vars for Kart and Cameras, so an Input Manager would be handy...
		float v = Mathf.Abs(Input.GetAxis("Vertical"));
		targetRoll = Input.GetAxis("Horizontal") * -rollAngle * v;
		targetYaw = Input.GetAxis("Horizontal") * yawAngle * v;
		targetPitch = Input.GetAxis("Vertical") * -pitchAngle * v + initPitch;
		
		currentRoll = Mathf.Lerp(currentRoll, targetRoll, Time.deltaTime * speed);
		currentYaw = Mathf.Lerp(currentYaw, targetYaw, Time.deltaTime * speed);
		currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.deltaTime * speed);
		
		cameraFollow.transform.localRotation = Quaternion.Euler(currentPitch, currentYaw, currentRoll);
		
		if(isColliding) {
			cameraShake (kartSpeed);
		}
		
		if(Input.GetKeyDown(KeyCode.C)) {
			toggleCamera();
		}
	}
	
	private void toggleCamera() {
		cameraFollow.active = !cameraFollow.active;
		cameraTop.active = !cameraTop.active;
	}
	
	private void Hurt() {
		isColliding = true;
		rx = Random.Range(0.8f, 1.2f);
		ry = Random.Range(0.8f, 1.2f);
		rz = Random.Range(0.8f, 1.2f);
	}
	
	private void HurtLeave() {
		isColliding = false;
		cameraFollow.transform.localPosition = initPosFollow;
		cameraTop.transform.localPosition = initPosTop;
	}
	
	private void cameraShake(float power) {
		float px = power * shakeIntensity * Mathf.Sin(shakeSpeed * Time.timeSinceLevelLoad * rx);
		float py = power * shakeIntensity * Mathf.Sin(shakeSpeed * Time.timeSinceLevelLoad * ry);
		float pz = power * shakeIntensity * Mathf.Sin(shakeSpeed * Time.timeSinceLevelLoad * rz);
		cameraFollow.transform.localPosition = new Vector3(px + initPosFollow.x, py + initPosFollow.y, pz + initPosFollow.z);
		cameraTop.transform.localPosition = new Vector3((4f * px) + initPosTop.x, (4f * py) + initPosTop.y, (4f * pz) + initPosTop.z);
	}
}
