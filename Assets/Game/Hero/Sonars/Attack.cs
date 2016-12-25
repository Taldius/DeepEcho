using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour {

	public GameObject wallSound;

	public float Speed = 7;
	public float lifeTime = 1;

	// Use this for initialization
	IEnumerator Start () {
		yield return new WaitForSeconds(lifeTime);
		Destroy(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(Vector3.up * Speed * Time.deltaTime);
		//transform.localScale += new Vector3 (1,1,0) * Time.deltaTime;
	}

	/*IEnumerator OnTriggerEnter2D (Collider2D other) {
		if(other.gameObject.CompareTag("Wall")) {
			GameObject sound = Instantiate(wallSound,transform.position,Quaternion.identity) as GameObject;
			Destroy(sound,sound.GetComponent<AudioSource>().clip.length);
			Destroy(gameObject);
		}
		yield return null;
	}*/
}
