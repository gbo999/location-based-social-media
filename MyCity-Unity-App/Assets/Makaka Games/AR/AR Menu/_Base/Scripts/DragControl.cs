/*
===================================================================
Unity Assets by MAKAKA GAMES: https://makaka.org/o/all-unity-assets
===================================================================

Online Docs (Latest): https://makaka.org/unity-assets
Offline Docs: You have a PDF file in the package folder.

=======
SUPPORT
=======

First of all, read the docs. If it didn’t help, get the support.

Web: https://makaka.org/support
Email: info@makaka.org

If you find a bug or you can’t use the asset as you need, 
please first send email to info@makaka.org
before leaving a review to the asset store.

I am here to help you and to improve my products for the best.
*/

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(Image))]
[HelpURL("https://makaka.org/unity-assets")]
public class DragControl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public MenuItemContent menuItemContent;

	[Header("Dragging")]
	
	public GameObject draggableIcon;
	
	public Color draggableColor = new Color32(255, 102, 0, 255);

	public Vector3 draggableScalingFactors = new Vector3(1.4f, 1.4f, 1f);
	private Vector3 draggableScaleNormal;

	private Dictionary<int,GameObject> m_DraggingIcons = new Dictionary<int, GameObject>();
	private Dictionary<int, RectTransform> m_DraggingPlanes = new Dictionary<int, RectTransform>();

	public void OnBeginDrag(PointerEventData eventData)
	{	
		draggableIcon.SetActive(true);
		draggableScaleNormal = draggableIcon.transform.localScale;

		m_DraggingIcons[eventData.pointerId] = draggableIcon;

		Image draggableImage = m_DraggingIcons[eventData.pointerId].GetComponent<Image>();

		draggableImage.overrideSprite = menuItemContent.imageSource.sprite;
		draggableImage.color = draggableColor;

		draggableImage.rectTransform.localScale = Vector3.Scale(
			draggableImage.rectTransform.localScale, 
			draggableScalingFactors);

		m_DraggingPlanes[eventData.pointerId] = transform as RectTransform;
		
		SetDraggedPosition(eventData);
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (m_DraggingIcons[eventData.pointerId])
			SetDraggedPosition(eventData);
	}

	private void SetDraggedPosition(PointerEventData eventData)
	{
		if (eventData.pointerEnter && eventData.pointerEnter.transform as RectTransform)
			m_DraggingPlanes[eventData.pointerId] = eventData.pointerEnter.transform as RectTransform;
		
		var rt = m_DraggingIcons[eventData.pointerId].GetComponent<RectTransform>();
		Vector3 globalMousePos;

		if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlanes[eventData.pointerId], eventData.position, eventData.pressEventCamera, out globalMousePos))
		{
			rt.position = globalMousePos;
			rt.rotation = m_DraggingPlanes[eventData.pointerId].rotation;
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (m_DraggingIcons[eventData.pointerId])
		{
			m_DraggingIcons[eventData.pointerId].transform.localScale = draggableScaleNormal;
			m_DraggingIcons[eventData.pointerId].SetActive(false);
		}

		m_DraggingIcons[eventData.pointerId] = null;
	}

}
