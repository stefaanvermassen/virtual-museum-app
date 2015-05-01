using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollbarWheelListener : MonoBehaviour, IScrollHandler {
	public Scrollbar scrollBar;
	ScrollRect scrollRect;
	public float scrollSpeed = 0.2f;

	public void Start() {
		scrollRect = GetComponent<ScrollRect> ();
	}
	
	public void OnScroll(PointerEventData eventData) {
		if (scrollBar != null) {
			if(scrollRect != null) {
				scrollBar.value += (float) ((double)eventData.scrollDelta.y * (double)scrollSpeed / ((double)scrollRect.content.sizeDelta.y * 0.001));
			} else {
				scrollBar.value += (float) ((double)eventData.scrollDelta.y * (double)scrollSpeed);
			}
		}
	}
}