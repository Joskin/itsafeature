using UnityEngine;

public class cameraController : MonoBehaviour {
	
	public Transform ball;
	public float followSpeed = 10f, lookAtSpeed = 2f, followDistance = 5f, followHeight = 3f, lookAtHeightOffset = 1f;
	
	public static Transform cameraTransform;
	
	private Vector3 posBehindBall, lookAtTarget, prevLookAtTarget, newLookAtTarget;

	void Update () {
		cameraTransform = this.transform;
		
		posBehindBall = - transform.forward;
		posBehindBall.y = 0f;
		posBehindBall.Normalize();
		posBehindBall *= followDistance;
		posBehindBall.y = followHeight;
		posBehindBall += ball.position;
		
		//Debug.DrawLine(ball.position, posBehindBall, Color.blue);
		
		prevLookAtTarget = lookAtTarget;
		
		newLookAtTarget = ball.position;
		newLookAtTarget.y += lookAtHeightOffset;
		
		lookAtTarget = Vector3.Lerp(prevLookAtTarget, newLookAtTarget, lookAtSpeed * Time.deltaTime);
		
		transform.LookAt(lookAtTarget);
		transform.position = Vector3.Lerp(transform.position, posBehindBall, followSpeed * Time.deltaTime);
	}
}
