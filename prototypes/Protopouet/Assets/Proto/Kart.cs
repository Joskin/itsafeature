using UnityEngine;

public class Kart : MonoBehaviour {
	
	public float speed = 10f, power = 5f, rotationSpeed = 10f, rollAngle = 10f, boundDistance = 10f, trailLength = 4f;
	public GameObject colliderInstance;
	public Vector4[] colliders;
	// Vector 4 struct :
		// x : +x
		// y : -x
		// z : +z
		// w : -z
	public Color32 collidersColor, collidersProjColor;
	
	public static float currentSpeed = 0f;// This should be within -1 and 1
	
	// Tried to get the trail and body via code instead of public vars
	private TrailRenderer trail;
	private GameObject body;
	private float px, py, pz, bodyRadius = 0.5f, rotationFinalSpeed, currentRoll = 0f, targetRoll;
	private bool hurt, isColliding = false;
	private Vector2[] diagonal1, diagonal2;
	private Vector2[] squareCenter;
	private Material[] colliderMat, colliderProjMat;
	private Color32 collidersInitColor, collidersProjInitColor;
	
	void Start () {
		// A child object must contain a Trail Renderer component !
		trail = (TrailRenderer) GetComponentInChildren(typeof(TrailRenderer));
		// There must be a "body" in the scene
		body = GameObject.Find("body");
		
		diagonal1 = new Vector2[colliders.Length];
		diagonal2 = new Vector2[colliders.Length];
		squareCenter = new Vector2[colliders.Length];
		colliderMat = new Material[colliders.Length];
		colliderProjMat = new Material[colliders.Length];
		
		// Add objects to show our colliders and fill custom position vars
		for(int i = 0; i < colliders.Length; i++) {
			// Get center position
			squareCenter[i] = new Vector2((colliders[i].x + colliders[i].y) * 0.5f, (colliders[i].z + colliders[i].w) * 0.5f);
			// Split space in diagonals : 1 = \, 2 = /
			// Dot product get perpendicular, so moving points to get perpendicular vectors (it will invert diagonals for tests :)
			// (x, y) -> perp (-y, x)
			// Also since it gets perpendicular vectors, define vectors from center of colliders <- NO ! (maybe defining vector here means they are in localspace ?
			// Keep bodyRadius in mind (actually works like if the kart was a cube that don't turn)
			// Final check will result in : 1 = /, 2 = \
			
			/*float d1x = squareCenter[i].x - (colliders[i].z - squareCenter[i].y);
			float d1y = squareCenter[i].y + (colliders[i].y - squareCenter[i].x);
			diagonal1[i] = new Vector2(d1x, d1y);
			float d2x = squareCenter[i].x - (colliders[i].z - squareCenter[i].y);
			float d2y = squareCenter[i].y + (colliders[i].x - squareCenter[i].x);
			diagonal2[i] = new Vector2(d2x, d2y);*/
			
			float d1x = - (colliders[i].z + bodyRadius - squareCenter[i].y);
			float d1y = (colliders[i].y - bodyRadius - squareCenter[i].x);
			diagonal1[i] = new Vector2(d1x, d1y);
			float d2x = - (colliders[i].z + bodyRadius - squareCenter[i].y);
			float d2y = (colliders[i].x + bodyRadius - squareCenter[i].x);
			diagonal2[i] = new Vector2(d2x, d2y);
			// Get Scale
			float scaleX = colliders[i].x - colliders[i].y, scaleY = colliders[i].z - colliders[i].w;
			// Instanciate
			GameObject o = (GameObject)Instantiate(colliderInstance);
			Projector proj = (Projector) o.transform.GetComponentInChildren(typeof(Projector));
			colliderMat[i] = o.renderer.material;
			// Trick to instanciate materials on projectors since Unity don't do it by default (not needed for renderer.material)
			colliderProjMat[i] = new Material(proj.material);
			proj.material = colliderProjMat[i];
			o.transform.position = new Vector3(squareCenter[i].x, 0f, squareCenter[i].y);
			o.transform.localScale = new Vector3(scaleX, scaleY, 1f);
			o.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
			proj.orthographicSize = scaleY;
			proj.aspectRatio = scaleX / scaleY;
		}
		collidersInitColor = colliderMat[0].GetColor("_TintColor");
		collidersProjInitColor = colliderProjMat[0].GetColor("_Color");
	}
	
	void Update () {
		move();
	}
	
	private void move() {
		float v = Input.GetAxis("Vertical");
		float h = Input.GetAxis("Horizontal");
		
		targetRoll = -h * Mathf.Abs(v) * rollAngle;
		currentRoll = Mathf.Lerp(currentRoll, targetRoll, Time.deltaTime * speed);
		
		if(isColliding) {
			currentSpeed = Mathf.Lerp(currentSpeed, v * 0.3f, Time.deltaTime * power);
			rotationFinalSpeed = v * 0.7f;
		}
		else {
			currentSpeed = Mathf.Lerp(currentSpeed, v, Time.deltaTime * power);
			rotationFinalSpeed = currentSpeed;
		}
		
		float translate = speed * currentSpeed * Time.deltaTime;
		float rotation = speed * h * rotationSpeed * rotationFinalSpeed * Time.deltaTime;
		
		transform.Translate(0f, 0f, translate);
		
		keepInBounds();
		updateTrail(currentSpeed);
		
		transform.Rotate(0f, rotation, 0f);
		body.transform.parent.localRotation = Quaternion.Euler(0f, 0f, currentRoll);
		body.transform.Rotate(0f, -Mathf.PI * currentSpeed * bodyRadius * speed, 0f);
	}
	
	private void keepInBounds() {
		px = transform.localPosition.x;
		py = transform.localPosition.y;
		pz = transform.localPosition.z;
		
		hurt = false;
		
		if(px > boundDistance - bodyRadius) {
			px = boundDistance - bodyRadius;
			hurt = true;
		}
		else if(px < -boundDistance + bodyRadius) {
			px = -boundDistance + bodyRadius;
			hurt = true;
		}
		
		if(pz > boundDistance - bodyRadius) {
			pz = boundDistance - bodyRadius;
			hurt = true;
		}
		else if(pz < -boundDistance + bodyRadius) {
			pz = -boundDistance + bodyRadius;
			hurt = true;
		}
		
		for(int i = 0; i < colliders.Length; i++) {
			collisionTest(i);
			
			// 1 = \ blue
			// 2 = / green
			// 3 = [Center]->[Player] yellow
			//Debug.DrawLine(new Vector3(colliders[i].y, 1f, colliders[i].z), new Vector3(squareCenter[i].x, 1f, squareCenter[i].y), Color.blue);
			//Debug.DrawLine(new Vector3(colliders[i].x, 1f, colliders[i].z), new Vector3(squareCenter[i].x, 1f, squareCenter[i].y), Color.green);
			//Debug.DrawLine(new Vector3(diagonal1[i].x + squareCenter[i].x, 1f, diagonal1[i].y + squareCenter[i].y), new Vector3(squareCenter[i].x, 1f, squareCenter[i].y), Color.blue);
			//Debug.DrawLine(new Vector3(diagonal2[i].x + squareCenter[i].x, 1f, diagonal2[i].y + squareCenter[i].y), new Vector3(squareCenter[i].x, 1f, squareCenter[i].y), Color.green);
			//Debug.DrawLine(new Vector3(px, 1f, pz), new Vector3(squareCenter[i].x, 1f, squareCenter[i].y), Color.yellow);
		}
		
		transform.localPosition = new Vector3(px, py, pz);
		
		if(hurt && !isColliding) {
			CollisionEvents.TriggerHurt();
			isColliding = true;
		}
		else if(!hurt) {
			CollisionEvents.TriggerHurtLeave();
			isColliding = false;
		}
	}
	
	private void updateTrail(float v) {
		trail.time = Mathf.Lerp(trail.time, trailLength * Mathf.Abs(v), Time.deltaTime * trailLength);
	}
	
	private void collisionTest(int i) {
		// Get player position vector relative to center of collider
		Vector2 playerPos = new Vector2(px, pz) - squareCenter[i];
		// test 2 possibilites, 1 = /, 2 = \
		bool bottomLeft = Vector2.Dot(diagonal1[i], playerPos) > 0f;
		bool topLeft = Vector2.Dot(diagonal2[i], playerPos) > 0f;
		bool collideCurrent = false;
		
		// define space area, and do collision tests
		if(bottomLeft) {
			if(topLeft) {
				// Left
				//Debug.Log("Collider : "+i+", Left");
				if(px > colliders[i].y - bodyRadius) {
					px = colliders[i].y - bodyRadius;
					hurt = true;
					collideCurrent = true;
				}
			}
			else {
				// Bottom
				//Debug.Log("Collider : "+i+", Bottom");
				if(pz > colliders[i].w - bodyRadius) {
					pz = colliders[i].w - bodyRadius;
					hurt = true;
					collideCurrent = true;
				}
			}
		}
		else {
			if(topLeft) {
				// Top
				//Debug.Log("Collider : "+i+", Top");
				if(pz < colliders[i].z + bodyRadius) {
					pz = colliders[i].z + bodyRadius;
					hurt = true;
					collideCurrent = true;
				}
				
			}
			else {
				// Right
				//Debug.Log("Collider : "+i+", Right");
				if(px < colliders[i].x + bodyRadius) {
					px = colliders[i].x + bodyRadius;
					hurt = true;
					collideCurrent = true;
				}
			}
		}
		// Maybe change projector color too...
		if(collideCurrent) {
			colliderMat[i].SetColor("_TintColor", collidersColor);
			colliderProjMat[i].SetColor("_Color", collidersProjColor);
		}
		else {
			colliderMat[i].SetColor("_TintColor", collidersInitColor);
			colliderProjMat[i].SetColor("_Color", collidersProjInitColor);
		}
	}
}
