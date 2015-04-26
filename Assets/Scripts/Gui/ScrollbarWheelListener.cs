using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollbarWheelListener : MonoBehaviour, IScrollHandler {
	Scrollbar scrollBar;
	public float scrollSpeed = 0.2f;

	void Start () {
		scrollBar = GetComponent<ScrollRect> ().verticalScrollbar;
	}
	
	public void OnScroll(PointerEventData eventData){
		scrollBar.value += eventData.scrollDelta.y * scrollSpeed;
	}
}