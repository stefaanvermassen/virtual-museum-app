using UnityEngine;
using System.Collections;

public class FootstepsScript : MonoBehaviour {

	public AudioClip[] footsteps;
	private float nextFootTimer;
	public float timeBetweenFootsteps;
	private CharacterController controller;

	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController> ();
		nextFootTimer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		nextFootTimer -= Time.deltaTime * controller.velocity.magnitude;
		if(nextFootTimer <= 0 && controller.isGrounded && controller.velocity.magnitude > 0.3F) {
			GetComponent<AudioSource>().PlayOneShot(footsteps[Random.Range(0, footsteps.Length)]);
			nextFootTimer = timeBetweenFootsteps;
		}
	}
}
