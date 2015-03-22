using UnityEngine;
using System.Collections;

/// <summary>
/// Plays footstep sounds when moving.
/// </summary>
public class FootstepsScript : MonoBehaviour {

	public AudioClip[] footsteps;
	private float nextFootTimer;
	public float timeBetweenFootsteps;
	private CharacterController controller;

	/// <summary>
	/// Start footstep timer.
	/// </summary>
	void Start () {
		controller = GetComponent<CharacterController> ();
		nextFootTimer = 0;
	}
	
	/// <summary>
	/// Decreases footstep timer by movement speed. Plays footstep sound and resets if 0 is reached.
	/// </summary>
	void Update () {
		nextFootTimer -= Time.deltaTime * controller.velocity.magnitude * 2;
		if(nextFootTimer <= 0 && controller.isGrounded && controller.velocity.magnitude > 0.3F) {
			GetComponent<AudioSource>().PlayOneShot(footsteps[Random.Range(0, footsteps.Length)]);
			nextFootTimer = timeBetweenFootsteps;
		}
	}
}
