using UnityEngine;
using System.Collections;

public class Onde : MonoBehaviour {

	public float Speed = 5;

	public float minScaleX = 0.5f;
	public float maxScaleX = 2;

	private float targetScaleX = 1;
	private float scaleX = 1;

	// Use this for initialization
	void Start () {
		targetScaleX = maxScaleX;
		Destroy(gameObject,1);
	}
	
	// Update is called once per frame
	void Update () {

		transform.Translate(Vector3.up*Time.deltaTime*Speed);

		scaleX = transform.localScale.x;
		scaleX = Mathf.MoveTowards(scaleX, targetScaleX, Time.deltaTime*10);
		transform.localScale = new Vector3(scaleX,transform.localScale.y,transform.localScale.z);

		if(scaleX >= maxScaleX) {
			targetScaleX = minScaleX;
		}
		if(scaleX <= minScaleX) {
			targetScaleX = maxScaleX;
		}
	}

	void OnTriggerEnter2D (Collider2D other) { 
		if(!other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("ExitSound") && !other.gameObject.CompareTag("BossTrigger"))
			StartCoroutine(DestroyObject ());

	}

	IEnumerator DestroyObject () {
		Destroy(gameObject);
		yield return null;
	}
}
