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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[HelpURL("https://makaka.org/unity-assets")]
public class PanelMediaControl : MonoBehaviour
{
	public DropControl dropControl;
	
	public Image imageSource;
	public Image imageContent;

	public Color colorHighlight = Color.black;
	private Color colorNormal;

    public void OnEnable ()
	{
        colorNormal = imageSource.color;
    }

	private void Start()
	{
		if (dropControl)
		{
			dropControl.OnPointerEnterMenuItem += SetHighlightColor;
			dropControl.OnPointerExitAction += SetNormalColor;
			dropControl.OnDropAction += SetNormalColor;
		}
	}

    public void SetSprite(Sprite sprite)
    {
		imageContent.overrideSprite = sprite;
    }

	public void SetHighlightColor()
	{
		imageSource.color = colorHighlight;
	}

	public void SetNormalColor()
	{
		imageSource.color = colorNormal;
	}

}
