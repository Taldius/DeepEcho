using UnityEngine;
using System.Collections;

public class Tete : MonoBehaviour {

	public GameObject Boss;
	
	private Boss boss;
	
	// Use this for initialization
	void Start () {
		boss = Boss.GetComponent<Boss>();
	}
	
	// Update is called once per frame
	void OnCollisionEnter2D (Collision2D other) {
		if(other.gameObject.CompareTag("Wall")) {
			boss.Bounce();
		}
		if(other.gameObject.CompareTag("Attack")) {
			Destroy(other.gameObject);
		}
	
	}
}
