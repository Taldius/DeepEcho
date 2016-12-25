using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour {

	public GameObject Hide;

	private int Key = 0;

	public LayerMask layer;

	public GameObject attackWave;
	private GameObject attack = null;

	private GameObject currMap;
	public int mapNb = 0;

	private bool canMove = true;
	private bool canSonar = true;
	private bool canAttack = true;

	public bool firstPlay = true;

	public GameObject radarTutoSound;
	private GameObject _radarTutoSound;

	public GameObject moveTutoSound;
	private GameObject _moveTutoSound;

	public GameObject[] Maps;

	public GameObject Win;

	private GameObject endSound;

	public GameObject firstWallSound;
	private bool firstWall = true;
	public GameObject firstDangerSound;
	private bool firstDanger = true;
	public GameObject firstEndSound;
	private bool firstEnd = true;
	public GameObject firstDeathSound;
	private bool firstDeath = true;

	public GameObject firstBossSound;
	private bool firstBoss = true;

	public GameObject canAttackSound;

	public GameObject gotKeySound;

	public GameObject openDoorSound;
	public GameObject closeDoorSound;
	public GameObject doorSpeech;
	private bool firstDoor = true;

	public GameObject keySpeech;
	private bool firstKey = true;

	public GameObject foundExitSound;

	public GameObject FinishSpeech;

	public GameObject EndSpeech;

	public GameObject foundWaySound;
	private bool foundWay = true;
	
	public Vector3 initPos;

	public GameObject Wave;
	public GameObject GoodWave;
	public GameObject BadWave;

	private GameObject wave;

	public GameObject HitWall;
	public GameObject HitEnemy;

	public GameObject Sonar;
	public GameObject GoodSonar;
	public GameObject BadSonar;

	public float Speed  = 3;

	private Vector2 moveDirection;
	private Vector2 radarDirection;

	private Animator anim;

	// Use this for initialization
	void Start () {
		layer = ~layer;
		currMap = Instantiate(Maps[mapNb],Vector3.zero,Quaternion.identity) as GameObject;

		Key = 0;

		Win.SetActive(false);
		Sonar.SetActive(false);
		GoodSonar.SetActive(false);
		BadSonar.SetActive(false);

		GetComponent<AudioSource>().volume = 0;

		if(firstPlay) {
			_radarTutoSound = Instantiate(radarTutoSound,transform.position,Quaternion.identity) as GameObject;
			firstPlay = false;
			Destroy(_radarTutoSound,_radarTutoSound.GetComponent<AudioSource>().clip.length);

			canMove = false;
			canSonar = false;
			canAttack = false;
			StartCoroutine(waitTuto(7,1));
		}

		wave = Wave;
		anim = GetComponent<Animator>();
	}

	IEnumerator waitTuto (float timer, int canNb) {
		yield return new WaitForSeconds(timer);
		Debug.Log (canSonar);
		if(canNb == 1)
			canSonar = true;
		if(canNb == 2)
			canMove = true;
	}
	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.LeftControl)) {
			if(!Hide.activeSelf)
				Hide.SetActive(true);
			else
				Hide.SetActive(false);

		}

		if(GetComponent<Rigidbody2D>().velocity.magnitude > 0.5f)
			GetComponent<AudioSource>().volume = 0.3f;
		else
			GetComponent<AudioSource>().volume = 0;

		if(Win.activeSelf) {

			GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			GetComponent<AudioSource>().volume = 0;
			Sonar.SetActive(false);
			GoodSonar.SetActive(false);
			BadSonar.SetActive(false);

			if(mapNb+1 < Maps.Length) {
				Key = 0;
					Win.SetActive(false);
					transform.position = Vector3.zero;
					Destroy(currMap);
					currMap = Instantiate(Maps[mapNb+1],Vector3.zero,Quaternion.identity) as GameObject;
					mapNb++;

			}
			else {
				if(!endSound)
					Application.LoadLevel(0);
			
			}
			Debug.Log(mapNb+1 + "  " + Maps.Length);
			return;
		}

		if(_radarTutoSound) {
			if(Input.GetKey(KeyCode.JoystickButton0)) {
				Destroy(_radarTutoSound);
				canSonar = true;
				canMove = true;
			}
		}
		if(_moveTutoSound) {
			if(Input.GetKey(KeyCode.JoystickButton0)) {
				Destroy(_moveTutoSound);
				canMove = true;
			}
		}


		if(canSonar)
			useSonar();
	
		if(canMove)
			Move();

		if(canAttack)
			StartCoroutine(Attack());

	}

	void Move () {
		if(new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical")) != Vector2.zero)
			moveDirection = new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
		else
			moveDirection = new Vector2(Input.GetAxis("HorizontalKEY"),Input.GetAxis("VerticalKEY"));
		moveDirection.Normalize();
		moveDirection *= Speed;

		anim.SetFloat("Speed",moveDirection.magnitude);


		GetComponent<Rigidbody2D>().velocity = moveDirection;

	}

	void useSonar () {
		if(new Vector2(Input.GetAxis("LookX"),Input.GetAxis("LookY")) != Vector2.zero)
			radarDirection = new Vector2(Input.GetAxis("LookX"),Input.GetAxis("LookY"));
		else
			radarDirection = new Vector2(Input.GetAxis("LookXKEY"),Input.GetAxis("LookYKEY"));

		if(radarDirection == Vector2.zero) {
			radarDirection = moveDirection;
		}


		RaycastHit2D hit = Physics2D.Raycast(transform.position,radarDirection,20,layer.value);

		if(hit.collider) {//Debug.Log (hit.collider.gameObject.name);

			float radarPitch = 1.5f - Mathf.Clamp(Vector2.Distance(hit.point,transform.position)/15,0,0.6f);
			float radarVolume = 1 - Mathf.Clamp(Vector2.Distance(hit.point,transform.position)/20,0.5f,0.9f);

			reloadTime = Mathf.Clamp(Vector2.Distance(hit.point,transform.position)/20,0.1f,0.6f);

			if(hit.collider.gameObject.CompareTag("Trigger") && foundWay) {
				StartCoroutine(waitTuto(2,2));
				Destroy(hit.collider.gameObject);
				Destroy(_radarTutoSound);
				_moveTutoSound = Instantiate(moveTutoSound,transform.position,Quaternion.identity) as GameObject;
				Destroy(_moveTutoSound,_moveTutoSound.GetComponent<AudioSource>().clip.length);
			}

			if(hit.collider.gameObject.CompareTag("Wall")) {
				Sonar.SetActive(true);
				GoodSonar.SetActive(false);
				BadSonar.SetActive(false);

				wave = Wave;

				Sonar.GetComponent<AudioSource>().pitch = Mathf.Lerp(Sonar.GetComponent<AudioSource>().pitch,radarPitch,Time.deltaTime*10);
				Sonar.GetComponent<AudioSource>().volume = Mathf.Lerp(Sonar.GetComponent<AudioSource>().volume,radarVolume,Time.deltaTime*10);
	
			}
			if(hit.collider.gameObject.CompareTag("Exit")) {

				if(firstEnd) {
					GameObject sound = Instantiate(firstEndSound,transform.position,Quaternion.identity) as GameObject;
					firstEnd = false;
					Destroy(sound,sound.GetComponent<AudioSource>().clip.length);
				}

				GoodSonar.SetActive(true);
				Sonar.SetActive(false);
				BadSonar.SetActive(false);



				wave = GoodWave;


				GoodSonar.GetComponent<AudioSource>().pitch = Mathf.Lerp(GoodSonar.GetComponent<AudioSource>().pitch,radarPitch,Time.deltaTime*10);
				GoodSonar.GetComponent<AudioSource>().volume = Mathf.Lerp(GoodSonar.GetComponent<AudioSource>().volume,radarVolume,Time.deltaTime*10);
			}

			if(hit.collider.gameObject.CompareTag("Key")) {
				
				if(firstKey) {
					GameObject sound = Instantiate(keySpeech,transform.position,Quaternion.identity) as GameObject;
					Destroy(sound,sound.GetComponent<AudioSource>().clip.length);
					firstKey = false;
				}
				
				GoodSonar.SetActive(true);
				Sonar.SetActive(false);
				BadSonar.SetActive(false);
				
				
				
				wave = GoodWave;
				
				
				GoodSonar.GetComponent<AudioSource>().pitch = Mathf.Lerp(GoodSonar.GetComponent<AudioSource>().pitch,radarPitch,Time.deltaTime*10);
				GoodSonar.GetComponent<AudioSource>().volume = Mathf.Lerp(GoodSonar.GetComponent<AudioSource>().volume,radarVolume,Time.deltaTime*10);
			}

			if(hit.collider.gameObject.CompareTag("WeakBoss")) {
				
				if(firstBoss) {
					GameObject sound = Instantiate(firstBossSound,transform.position,Quaternion.identity) as GameObject;
					Destroy(sound,sound.GetComponent<AudioSource>().clip.length);
					firstBoss = false;
				}
				
				GoodSonar.SetActive(true);
				Sonar.SetActive(false);
				BadSonar.SetActive(false);
				
				
				
				wave = GoodWave;
				
				
				GoodSonar.GetComponent<AudioSource>().pitch = Mathf.Lerp(GoodSonar.GetComponent<AudioSource>().pitch,radarPitch,Time.deltaTime*10);
				GoodSonar.GetComponent<AudioSource>().volume = Mathf.Lerp(GoodSonar.GetComponent<AudioSource>().volume,radarVolume,Time.deltaTime*10);
			}

			if(hit.collider.gameObject.CompareTag("Door")) {
				
				if(firstDoor) {
					GameObject sound = Instantiate(doorSpeech,transform.position,Quaternion.identity) as GameObject;
					Destroy(sound,sound.GetComponent<AudioSource>().clip.length);
					firstDoor = false;
				}
				
				GoodSonar.SetActive(true);
				Sonar.SetActive(false);
				BadSonar.SetActive(false);
				
				
				
				wave = GoodWave;
				
				
				GoodSonar.GetComponent<AudioSource>().pitch = Mathf.Lerp(GoodSonar.GetComponent<AudioSource>().pitch,radarPitch,Time.deltaTime*10);
				GoodSonar.GetComponent<AudioSource>().volume = Mathf.Lerp(GoodSonar.GetComponent<AudioSource>().volume,radarVolume,Time.deltaTime*10);
			}

			if(hit.collider.gameObject.CompareTag("Enemy")) {

				if(firstDanger) {
					GameObject sound = Instantiate(firstDangerSound,transform.position,Quaternion.identity) as GameObject;
					firstDanger = false;
					Destroy(sound,sound.GetComponent<AudioSource>().clip.length);
				}

				BadSonar.SetActive(true);
				Sonar.SetActive(false);
				GoodSonar.SetActive(false);


				wave = BadWave;


				BadSonar.GetComponent<AudioSource>().pitch = Mathf.Lerp(BadSonar.GetComponent<AudioSource>().pitch,radarPitch,Time.deltaTime*10);
				BadSonar.GetComponent<AudioSource>().volume = Mathf.Lerp(BadSonar.GetComponent<AudioSource>().volume,radarVolume,Time.deltaTime*10);
			
			}
		}
		else  {


			Sonar.SetActive(true);
			GoodSonar.SetActive(false);
			BadSonar.SetActive(false);

			
			reloadTime = 0.4f;

			wave = Wave;

			Sonar.GetComponent<AudioSource>().pitch = Mathf.Lerp(Sonar.GetComponent<AudioSource>().pitch,0.4f,Time.deltaTime*10);
			Sonar.GetComponent<AudioSource>().volume = Mathf.Lerp(Sonar.GetComponent<AudioSource>().volume,0.1f,Time.deltaTime*10);


		}


		if(radarDirection == Vector2.zero) { 
			GetComponent<AudioSource>().volume = 0;

				

			
			if(moveDirection != Vector2.zero) {
				GetComponent<AudioSource>().volume = 0;
				float rot_z = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
				transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(0f, 0f, rot_z - 90),Time.deltaTime*10);
				StartCoroutine(CreateOnde ());
			}
			else {
				Sonar.SetActive(false);
				GoodSonar.SetActive(false);
				BadSonar.SetActive(false);
			}


			
		}
		else {
			float rot_z = Mathf.Atan2(radarDirection.y, radarDirection.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
			StartCoroutine(CreateOnde ());
		}



	}

	public float reloadTime = 0.5f;
	private float reload;

	
	IEnumerator CreateOnde() {
		
		if(reload >= reloadTime) {
			if(wave)
				Instantiate(wave, transform.TransformPoint(Vector3.forward*0.75f), transform.rotation);
			//onde.GetComponent<Onde>().Speed = 
			reload = 0;

			
		}
		reload +=Time.deltaTime;
		yield return null;
	}

	IEnumerator Attack() {
		if(!attack) {
			if(Input.GetButtonDown("Attack") || Input.GetButtonDown("AttackALT") || Input.GetButtonDown("AttackKEY") || Mathf.Abs(Input.GetAxis("AttackAXIS")) == 1) {
				attack = Instantiate(attackWave, transform.TransformPoint(Vector3.forward*0.75f), transform.rotation) as GameObject;
			}
		}

		yield return null;
	}


	IEnumerator playSound (GameObject Sound) {
		GameObject sound = Instantiate(Sound,transform.position,Quaternion.identity) as GameObject;
		Destroy(sound,sound.GetComponent<AudioSource>().clip.length);
		yield return null;
	}

	void OnTriggerEnter2D (Collider2D other) {
		if(other.gameObject.CompareTag("Exit")) {
			
			Win.SetActive(true);
			Win.transform.position = other.gameObject.transform.position;
			Destroy(other.gameObject);
			
			if(mapNb+1 < Maps.Length) {
				GameObject sound = Instantiate(FinishSpeech,transform.position,Quaternion.identity) as GameObject;
				Destroy(sound,sound.GetComponent<AudioSource>().clip.length);
			}
			else {
				endSound = Instantiate(EndSpeech,transform.position,Quaternion.identity) as GameObject;
				Destroy(endSound,endSound.GetComponent<AudioSource>().clip.length);
			}
		}

		if(other.gameObject.CompareTag("ExitSound")) {
			other.gameObject.GetComponent<CircleCollider2D>().enabled = false;
			GameObject sound = Instantiate(foundExitSound,transform.position,Quaternion.identity) as GameObject;
			Destroy(sound,sound.GetComponent<AudioSource>().clip.length);
		}

		if(other.gameObject.CompareTag("AttackTrigger")) {
			Destroy(other.gameObject);
			GameObject sound = Instantiate(canAttackSound,transform.position,Quaternion.identity) as GameObject;
			Destroy(sound,sound.GetComponent<AudioSource>().clip.length);

			StartCoroutine(WaitForAttack());
		}

		if(other.gameObject.CompareTag("Key")) {

			Destroy(other.gameObject);
			Key++;

			GameObject sound = Instantiate(gotKeySound,transform.position,Quaternion.identity) as GameObject;
			Destroy(sound,sound.GetComponent<AudioSource>().clip.length);


		}

		if(other.gameObject.CompareTag("Enemy")) { Debug.Log(other.gameObject.name);
			StartCoroutine(playSound(HitEnemy));
			Destroy (currMap);
			currMap = Instantiate(Maps[mapNb],Vector3.zero,Quaternion.identity) as GameObject;

			gameObject.transform.position = initPos;
			if(firstDeath) {
				GameObject sound = Instantiate(firstDeathSound,transform.position,Quaternion.identity) as GameObject;
				firstDeath = false;
				Destroy(sound,sound.GetComponent<AudioSource>().clip.length);
			}
		}
	}

	IEnumerator WaitForAttack () {
		yield return new WaitForSeconds(4);
		canAttack = true;
	}

	void OnCollisionEnter2D(Collision2D other) {


		if(other.gameObject.CompareTag("Door")) {
			if(Key > 0) {
				Destroy(other.gameObject);
				Key--;
				
				GameObject sound = Instantiate(openDoorSound,transform.position,Quaternion.identity) as GameObject;
				Destroy(sound,sound.GetComponent<AudioSource>().clip.length);
				
			}
			else {
				GameObject sound = Instantiate(closeDoorSound,transform.position,Quaternion.identity) as GameObject;
				Destroy(sound,sound.GetComponent<AudioSource>().clip.length);
			}
		}

		if(other.gameObject.CompareTag("Wall")) {
			StartCoroutine(playSound(HitWall));
			if(firstWall) {
				GameObject sound = Instantiate(firstWallSound,transform.position,Quaternion.identity) as GameObject;
				firstWall = false;
				Destroy(sound,sound.GetComponent<AudioSource>().clip.length);
			}
		}
		if(other.gameObject.CompareTag("Enemy")) { Debug.Log(other.gameObject.name);
		   StartCoroutine(playSound(HitEnemy));

			Destroy (currMap);
			currMap = Instantiate(Maps[mapNb],Vector3.zero,Quaternion.identity) as GameObject;

			gameObject.transform.position = initPos;
			if(firstDeath) {
				GameObject sound = Instantiate(firstDeathSound,transform.position,Quaternion.identity) as GameObject;
				firstDeath = false;
				Destroy(sound,sound.GetComponent<AudioSource>().clip.length);
			}
		}
	}
}
