using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour {

	public LayerMask layer;
	public float Range = 5;
	public GameObject[] BossParts;

	public GameObject HurtSound;
	public GameObject BossEnd;

	public float Speed = 3;
	public float TurnSpeed = 2;
	private Vector3 direction = Vector3.zero;

	private Vector3 lastPos = Vector3.zero;
	private int i = 0;

	public int life = 4;

	public float delay = 0.5f;

	private bool dead = false;
	private bool immune = false;

	// Use this for initialization
	void Start () {


		dead = false;
		immune = false;
		for(int i = 1; i < BossParts.Length;i++) {
			BossParts[i].transform.eulerAngles = new Vector3(0,0,BossParts[0].transform.eulerAngles.z);
		}

		StartCoroutine(ChangeDirection());
	}
	
	// Update is called once per frame
	void Update () {

		if(dead) {

			foreach(GameObject parts in BossParts) {
				if(parts) {
					float alpha = parts.GetComponent<SpriteRenderer>().color.a;
					
					alpha = Mathf.MoveTowards(alpha,0,Time.deltaTime);
					
					parts.GetComponent<SpriteRenderer>().color = new Color(parts.GetComponent<SpriteRenderer>().color.r,parts.GetComponent<SpriteRenderer>().color.g,parts.GetComponent<SpriteRenderer>().color.b,alpha);
					
					if(alpha == 0)
						Destroy(parts);
				}
			}

			return;
		}

	
		BossParts[0].GetComponent<Rigidbody2D>().velocity = BossParts[0].transform.TransformDirection(Vector3.up*Time.deltaTime*Speed);
		BossParts[0].transform.rotation = Quaternion.Lerp(BossParts[0].transform.rotation,Quaternion.Euler(direction),Time.deltaTime*TurnSpeed);
	
		for(int i = 1; i < BossParts.Length;i++) {

			Vector2 heading = BossParts[i-1].transform.position - BossParts[i].transform.position;
			heading.Normalize();
			
			float rot_z = Mathf.Atan2(heading.y, heading.x) * Mathf.Rad2Deg;
			BossParts[i].transform.rotation = Quaternion.Lerp(BossParts[i].transform.rotation,Quaternion.Euler(0f, 0f, rot_z - 90),Time.deltaTime*TurnSpeed);


			if(Vector3.Distance(BossParts[i-1].transform.position,BossParts[i].transform.position) > 2) {

				BossParts[i].transform.position = Vector3.Lerp(BossParts[i].transform.position,BossParts[i-1].transform.position,Time.deltaTime*TurnSpeed*0.5f );

			}

		}

	}

	IEnumerator ChangeDirection () {
		while(!dead) {

			yield return new WaitForSeconds(delay);

				int random =  Random.Range(0,2);
				if(random < 1) {
				RaycastHit2D hit = Physics2D.Raycast(BossParts[0].transform.position,BossParts[0].transform.TransformDirection(Vector3.right),Range,layer);
				Debug.DrawRay(BossParts[0].transform.position,BossParts[0].transform.TransformDirection(Vector3.right));
						
				if(hit.collider) {Debug.Log(hit.collider.gameObject.name);		
						direction += new Vector3(0,0,90);
					}
					else {
						direction += new Vector3(0,0,-90);
					}
				}
				else {
				RaycastHit2D hit = Physics2D.Raycast(BossParts[0].transform.position,BossParts[0].transform.TransformDirection(-Vector3.right),Range,layer);
				Debug.DrawRay(BossParts[0].transform.position,BossParts[0].transform.TransformDirection(-Vector3.right));

				if(hit.collider) {Debug.Log(hit.collider.gameObject.name);				
					direction += new Vector3(0,0,-90);
				}
				else {
					direction += new Vector3(0,0,90);
				}
			}
		}

	}

	void OnTriggerEnter2D (Collider2D other) {

		if(other.gameObject.CompareTag("Attack")) {
			Destroy(other.gameObject);
			Hurt();
		}
	}

	public void Hurt () {

		if(life > 0) {
			if(!immune) {
				life --;
				Speed *=1.2f;
				TurnSpeed *=1.2f;
				delay *=0.9f;
				foreach(GameObject parts in BossParts) {
					if(parts.GetComponent<AudioSource>())
						parts.GetComponent<AudioSource>().pitch *= 1.2f;
				}

				Instantiate(HurtSound,BossParts[0].transform.position,Quaternion.identity);

				StartCoroutine(Immunity());
			}
		}
		else {

			StartCoroutine(EndSound());

			dead = true;

			foreach(GameObject parts in BossParts) {
				parts.tag = "Wall";
				if(parts.GetComponent<AudioSource>())
					parts.GetComponent<AudioSource>().volume = 0;

				parts.GetComponent<Rigidbody2D>().velocity = Vector3.zero;

				GetComponent<AudioSource>().Play();
			}
		}
	}

	IEnumerator Immunity () {
		immune = true;
		yield return new WaitForSeconds(0.5f);
		immune = false;
	}

	public void Bounce() {
		int random =  Random.Range(0,2);
		if(random < 1) {
			RaycastHit2D hit = Physics2D.Raycast(BossParts[0].transform.position,BossParts[0].transform.TransformDirection(Vector3.right),Range,layer);
			Debug.DrawRay(BossParts[0].transform.position,BossParts[0].transform.TransformDirection(Vector3.right));
			
			if(hit.collider) {Debug.Log(hit.collider.gameObject.name);		
				direction += new Vector3(0,0,90);
			}
			else {
				direction += new Vector3(0,0,-90);
			}
		}
		else {
			RaycastHit2D hit = Physics2D.Raycast(BossParts[0].transform.position,BossParts[0].transform.TransformDirection(-Vector3.right),Range,layer);
			Debug.DrawRay(BossParts[0].transform.position,BossParts[0].transform.TransformDirection(-Vector3.right));
			
			if(hit.collider) {Debug.Log(hit.collider.gameObject.name);				
				direction += new Vector3(0,0,-90);
			}
			else {
				direction += new Vector3(0,0,90);
			}
		}
	}

	IEnumerator EndSound () {
		GameObject sound = Instantiate(BossEnd,transform.position,Quaternion.identity) as GameObject;
		yield return new WaitForSeconds(sound.GetComponent<AudioSource>().clip.length);
		Destroy(sound);
		Application.LoadLevel(0);
	}
	
}
