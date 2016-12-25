using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	public GameObject Title;
	public GameObject Loading;


	// Use this for initialization
	void Start () {
		Title.SetActive(true);
		Loading.SetActive(false);
	}

	// Update is called once per frame
	void Update () {


		if(Title.activeSelf && (!Title.GetComponent<AudioSource>().isPlaying || Input.anyKeyDown)) {
			Title.SetActive(false);
			Loading.SetActive(true);

			Application.LoadLevel(1);

		}

	}
}
