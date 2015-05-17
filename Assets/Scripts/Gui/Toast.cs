using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Toast : MonoBehaviour {

    private Animator animator;
    private Text text;

	void Start () {
        animator = GetComponent<Animator>();
        text = gameObject.transform.FindChild("Text").gameObject.GetComponent<Text>();
	}

    /// <summary>
    /// Make the toast pop up and show the message
    /// </summary>
    /// <param name="message"></param>
    public void Notify(string message) {
        text.text = message;
        animator.SetTrigger("Play");
        //ClosingButton(message);
        //StartStatistics("Toast");
    }
}
