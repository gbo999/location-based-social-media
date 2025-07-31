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

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

[HelpURL("https://makaka.org/unity-assets")]
public class DropControl : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
	private DragControl curentDragControl;

	public Image imageReceiver;

	public Image imageContainer;

	public Color containerColorHighlight = Color.black;
	private Color containerColorHighlightNormal;

	public event Action<MenuItemContent> OnDropMenuItem;
	public event Action OnDropAction;
	public event Action OnPointerEnterMenuItem;
	public event Action OnPointerExitAction;
	
	public void OnEnable ()
	{
		if (imageContainer)
		{
			containerColorHighlightNormal = imageContainer.color;
		}
	}
	
	public void OnDrop(PointerEventData data)
	{
		imageContainer.color = containerColorHighlightNormal;
		
		if (imageReceiver == null)
			return;

		curentDragControl = GetDragMe (data);

		if (curentDragControl && curentDragControl.menuItemContent)
		{
			if (OnDropMenuItem != null)
			{
				OnDropMenuItem.Invoke(curentDragControl.menuItemContent);
			}
		}

		if (OnDropAction != null)
		{
			OnDropAction.Invoke();
		}
	}

	public void OnPointerEnter(PointerEventData data)
	{
		if (imageContainer == null)
			return;
		
		curentDragControl = GetDragMe (data);

		if (curentDragControl 
		&& curentDragControl.menuItemContent.imageSource 
		&& curentDragControl.menuItemContent.imageSource.sprite)
		{
			imageContainer.color = containerColorHighlight;

			if (OnPointerEnterMenuItem != null)
			{
				OnPointerEnterMenuItem.Invoke();
			}	
		}
	}

	public void OnPointerExit(PointerEventData data)
	{
		if (imageContainer == null)
			return;
		
		imageContainer.color = containerColorHighlightNormal;

		if (OnPointerExitAction != null)
		{
			OnPointerExitAction.Invoke();
		}
	}
	
	private DragControl GetDragMe(PointerEventData data)
	{
		var originalObj = data.pointerDrag;

		if (originalObj == null)
			return null;

		return originalObj.GetComponent<DragControl>();
	}
}
