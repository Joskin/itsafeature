public static class CollisionEvents {

	public delegate void CollideEvent();
	
	public static event CollideEvent Hurt, HurtLeave;
	
	public static void TriggerHurt() {
		if(Hurt != null) {
			Hurt();
		}
	}
	
	public static void TriggerHurtLeave() {
		if(HurtLeave != null) {
			HurtLeave();
		}
	}
}
