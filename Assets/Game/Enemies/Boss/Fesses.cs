using UnityEngine;
using System.Collections;

public class Fesses : MonoBehaviour {

	public GameObject Boss;
	int hurt = Animator.StringToHash("Hurt");
	private Boss boss;

	// Use this for initialization
	void Start () {
		boss = Boss.GetComponent<Boss>();



	}


	void OnTriggerEnter2D (Collider2D other) {
		if(other.gameObject.CompareTag("Attack")) {
			boss.Hurt();

			GetComponent<Animator>().SetTrigger(hurt);
		}
	}
}
