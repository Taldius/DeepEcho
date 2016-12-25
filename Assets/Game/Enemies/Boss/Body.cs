using UnityEngine;
using System.Collections;

public class Body : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void OnTriggerEnter2D (Collider2D other) {
		if(other.gameObject.CompareTag("Attack")) {
			Destroy(other.gameObject);
		}
		
	}
}
