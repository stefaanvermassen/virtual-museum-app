using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollbarWheelListener : MonoBehaviour, IScrollHandler {
	public Scrollbar scrollBar;
	public float scrollSpeed = 0.2f;
	
	public void OnScroll(PointerEventData eventData) {
		if(scrollBar != null) scrollBar.value += eventData.scrollDelta.y * scrollSpeed;
	}
}