using UnityEngine;
using System.Collections;

public class MoveTrap : MonoBehaviour {

	public AudioClip Move;
	public AudioClip Hit;

	public float Speed = 5; 

	private float direction = 1;

	private GameObject target;

	// Use this for initialization
	void Start () {

		GetComponent<AudioSource>().clip = Move;
		target = GameObject.Find("Hero");
	}
	
	// Update is called once per frame
	void Update () {

			transform.Translate(Vector3.right * Speed * Time.deltaTime * direction);
	}

	void OnTriggerEnter2D (Collider2D other) {
		if(other.gameObject.CompareTag("Wall") ) {
			direction *= -1;

			if(Vector3.Distance(target.transform.position,transform.position) <= 20)
			StartCoroutine(HitWall());



		}
	}

	IEnumerator HitWall () {

		GetComponent<AudioSource>().Stop ();
		GetComponent<AudioSource>().clip = Hit;
		GetComponent<AudioSource>().volume = 1;
		GetComponent<AudioSource>().Play ();

		yield return new WaitForSeconds(0.3f);

		GetComponent<AudioSource>().Stop ();
		GetComponent<AudioSource>().clip = Move;
		GetComponent<AudioSource>().volume = 0.5f;
		GetComponent<AudioSource>().Play ();

	}
}
