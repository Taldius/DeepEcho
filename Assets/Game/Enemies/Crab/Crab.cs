using UnityEngine;
using System.Collections;

public class Crab : MonoBehaviour {

	private bool isFading = false;

	public AudioClip idleSound;
	public AudioClip walkSound;

	public GameObject deathSound;

	public LayerMask layer;

	public float Speed = 2;
	public float Range = 10;

	private GameObject target;

	// Use this for initialization
	void Start () {

		isFading = false;

		layer = ~layer;
		GetComponent<AudioSource>().clip = idleSound;

		target = GameObject.Find("Hero");
	}
	
	// Update is called once per frame
	void Update () {

		if(isFading) {
			GetComponent<AudioSource>().volume = 0;
			GetComponent<Rigidbody2D>().velocity = Vector3.zero;
			GetComponent<CircleCollider2D>().enabled = false;

			float alpha = GetComponent<SpriteRenderer>().color.a;
			
			alpha = Mathf.MoveTowards(alpha,0,Time.deltaTime);
			
			GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r,GetComponent<SpriteRenderer>().color.g,GetComponent<SpriteRenderer>().color.b,alpha);
			
			if(alpha == 0)
				Destroy(gameObject);

			return;
		}

		Vector2 heading = target.transform.position - transform.position;
		heading.Normalize();

		RaycastHit2D hit = Physics2D.Raycast(transform.position,heading,Range,layer);


		if(hit.collider) { 

			if(hit.collider.gameObject.CompareTag("Player")) {
				if(GetComponent<AudioSource>().clip != walkSound) {
					GetComponent<AudioSource>().Stop ();
					GetComponent<AudioSource>().clip = walkSound;
					GetComponent<AudioSource>().Play ();
				}
				float rot_z = Mathf.Atan2(heading.y, heading.x) * Mathf.Rad2Deg;
				transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(0f, 0f, rot_z - 90),Time.deltaTime*10);
			
				GetComponent<Rigidbody2D>().velocity = transform.TransformDirection(Vector3.up * Speed * Time.deltaTime);
			}
			else {
				if(GetComponent<AudioSource>().clip != idleSound) {

					GetComponent<Rigidbody2D>().velocity = Vector3.zero;
					GetComponent<AudioSource>().Stop ();
					GetComponent<AudioSource>().clip = idleSound;
					GetComponent<AudioSource>().Play ();
				}

			}
		}
	}

	IEnumerator OnTriggerEnter2D (Collider2D other) {
		if(other.gameObject.CompareTag("Attack")) {
			GameObject sound = Instantiate(deathSound,transform.position,Quaternion.identity) as GameObject;
			Destroy(sound,sound.GetComponent<AudioSource>().clip.length);
			Destroy (other.gameObject);
			isFading = true;
		}
		yield return null;
	}
}
