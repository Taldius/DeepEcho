using UnityEngine;
using System.Collections;

public class BossManager : MonoBehaviour {

	public GameObject Boss;
	public GameObject Door;

	public GameObject BossIntroSound;
	public GameObject BossExplanation;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D (Collider2D other) {
		if(other.gameObject.CompareTag("Player")) {
			GetComponent<BoxCollider2D>().enabled = false;

			GameObject sound = Instantiate(BossIntroSound,transform.position,Quaternion.identity) as GameObject;

			StartCoroutine(BossIntro(sound.GetComponent<AudioSource>().clip.length,sound));



		}
	}

	IEnumerator BossIntro (float timer, GameObject currSound) {
		Door.SetActive(true);

		yield return new WaitForSeconds(timer);

		Destroy(currSound);

		Boss.SetActive(true);

		GameObject sound = Instantiate(BossExplanation,transform.position,Quaternion.identity) as GameObject;
		Destroy(sound,sound.GetComponent<AudioSource>().clip.length);
	}
}
