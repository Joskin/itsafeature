using UnityEngine;

public class LightsManager : MonoBehaviour {
	
	public Light diffuse, spot;
	public Color collideColor;
	
	private Color initDiffuseColor, initSpotColor;
	private bool isColliding = false;
	
	void Start () {
		CollisionEvents.Hurt += Hurt;
		CollisionEvents.HurtLeave += HurtLeave;
		initDiffuseColor = diffuse.color;
		initSpotColor = spot.color;
	}

	void Update () {
		if(isColliding) {
			diffuse.color = collideColor;
			spot.color = collideColor;
		}
		else {
			diffuse.color = Color.Lerp(diffuse.color, initDiffuseColor, 5 * Time.deltaTime);
			spot.color = Color.Lerp(spot.color, initSpotColor, 5 * Time.deltaTime);
		}
	}
	
	private void Hurt() {
		isColliding = true;
	}
	
	private void HurtLeave() {
		isColliding = false;
	}
}
