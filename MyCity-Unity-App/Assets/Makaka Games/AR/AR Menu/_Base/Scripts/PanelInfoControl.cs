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

using UnityEngine;
using UnityEngine.UI;

using TMPro;

#pragma warning disable 649

[HelpURL("https://makaka.org/unity-assets")]
public class PanelInfoControl : MonoBehaviour
{
	[SerializeField]
	private Animator panelInfoAnimator;
	private string animationRotationName = "Flash";

	[SerializeField]
	private TextMeshProUGUI textInfoHeader;
	
	[SerializeField]
	private TextMeshProUGUI textInfoDescription;

	[SerializeField]
	private GameObject buttonLink;

	private string currentLink;

	public void OnEnable ()
	{
        SetButtonLinkActive(false);
	}

	public void SetCurrentLink(string link)
	{
		currentLink = link;
	}
	
	public void SetButtonLinkActive(bool isActive)
	{
		buttonLink.SetActive(isActive);
	}

	public void OpenUrl(string link)
	{
		Application.OpenURL(link);
	}

	public void OpenUrl()
	{
		Application.OpenURL(currentLink);
	}

	public void Animate()
	{
		panelInfoAnimator.SetTrigger(animationRotationName);
	}

	public void SetHeaderText(string text)
	{
		textInfoHeader.text = text;
	}

	public void SetDescriptionText(string text)
	{
		textInfoDescription.text = text;
	}

    public void SetData(string header, string description, string link)
    {
		SetHeaderText (header);
		SetDescriptionText (description);
	
		SetButtonLinkActive(link.Length > 0);
		SetCurrentLink(link);
    }

	public void SetDataAndAnimate(string header, string description, string link)
	{
		SetData(header, description, link);
		Animate();
	}
}
