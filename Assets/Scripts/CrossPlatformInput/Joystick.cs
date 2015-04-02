using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityStandardAssets.CrossPlatformInput
{
	public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
	{
		public enum AxisOption
		{
			// Options for which axes to use
			Both, // Use both
			OnlyHorizontal, // Only horizontal
			OnlyVertical // Only vertical
		}

		public int MovementRange = 100;
		public AxisOption axesToUse = AxisOption.Both; // The options for the axes that the still will use
		public string horizontalAxisName = "Horizontal"; // The name given to the horizontal axis for the cross platform input
		public string verticalAxisName = "Vertical"; // The name given to the vertical axis for the cross platform input

		Vector2 holdingPos;
		Vector3 m_StartPos;
		Vector3 m_ParentOffset;
		bool m_UseX; // Toggle for using the x axis
		bool m_UseY; // Toggle for using the Y axis
		CrossPlatformInputManager.VirtualAxis m_HorizontalVirtualAxis; // Reference to the joystick in the cross platform input
		CrossPlatformInputManager.VirtualAxis m_VerticalVirtualAxis; // Reference to the joystick in the cross platform input

		public void Start() {
			//float scale = (float)(Screen.height * Screen.dpi) / (720 * 120);
			float scale = (float)(Screen.height) / (720);
			MovementRange = (int)(MovementRange*scale);
			m_StartPos = transform.localPosition;
			holdingPos.x = 0;
			holdingPos.y = 0;
			m_ParentOffset = GetComponentInParent<Canvas> ().transform.position;
			CreateVirtualAxes();
		}

		void UpdateVirtualAxes(Vector3 value)
		{
			var delta = m_StartPos - value;
			delta.y = -delta.y;
			delta /= MovementRange;
			if (m_UseX)
			{
				m_HorizontalVirtualAxis.Update(-delta.x);
			}

			if (m_UseY)
			{
				m_VerticalVirtualAxis.Update(delta.y);
			}
		}

		void CreateVirtualAxes()
		{
			// set axes to use
			m_UseX = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyHorizontal);
			m_UseY = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyVertical);

			// create new axes based on axes to use
			if (m_UseX)
			{
				if(CrossPlatformInputManager.VirtualAxisReference(horizontalAxisName) != null) {
					m_HorizontalVirtualAxis = CrossPlatformInputManager.VirtualAxisReference(horizontalAxisName);
				} else {
					m_HorizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(horizontalAxisName);
					CrossPlatformInputManager.RegisterVirtualAxis(m_HorizontalVirtualAxis);
				}
			}
			if (m_UseY)
			{
				if(CrossPlatformInputManager.VirtualAxisReference(verticalAxisName) != null) {
					m_VerticalVirtualAxis = CrossPlatformInputManager.VirtualAxisReference(verticalAxisName);
				} else {
					m_VerticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(verticalAxisName);
					CrossPlatformInputManager.RegisterVirtualAxis(m_VerticalVirtualAxis);
				}
			}
		}


		public void OnDrag(PointerEventData data) {
			Vector3 newPos = Vector3.zero;

			if (m_UseX) {
				int delta = (int)(data.position.x - m_StartPos.x - m_ParentOffset.x - holdingPos.x);
				//delta = Mathf.Clamp(delta, - MovementRange, MovementRange);
				newPos.x = delta;
			}

			if (m_UseY) {
				int delta = (int)(data.position.y - m_StartPos.y - m_ParentOffset.y - holdingPos.y);
				//delta = Mathf.Clamp(delta, -MovementRange, MovementRange);
				newPos.y = delta;
			}
			transform.localPosition = m_StartPos + Vector3.ClampMagnitude(newPos, MovementRange);
			UpdateVirtualAxes(transform.localPosition);
		}


		public void OnPointerUp(PointerEventData data)
		{
			transform.localPosition = m_StartPos;
			holdingPos.x = 0;
			holdingPos.y = 0;
			UpdateVirtualAxes(m_StartPos);
		}


		public void OnPointerDown(PointerEventData data) {
			holdingPos.x = data.position.x - m_StartPos.x - m_ParentOffset.x;
			holdingPos.y = data.position.y - m_StartPos.y - m_ParentOffset.y;
		}

		void OnDisable()
		{
			// remove the joysticks from the cross platform input
			/*if (m_UseX)
			{
				m_HorizontalVirtualAxis.Remove();
			}
			if (m_UseY)
			{
				m_VerticalVirtualAxis.Remove();
			}*/
		}
	}
}