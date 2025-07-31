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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[HelpURL("https://makaka.org/unity-assets")]
public class PanelMenuControl : MonoBehaviour
{
	public PanelInfoControl panelInfoControl;
	public PanelMediaControl panelMediaControl;

	public VideoPlayerXControl videoPlayerControl;

	public void OnEnable ()
	{
        videoPlayerControl.Close();
	}

	private void Start()
	{
		if (panelMediaControl.dropControl)
		{
			panelMediaControl.dropControl.OnDropMenuItem += OnClick;
		}
	}

    public void OnClick(MenuItemContent menuItemContent)
	{
		panelInfoControl.SetDataAndAnimate(
			menuItemContent.header, 
			menuItemContent.description, 
			menuItemContent.link);

		if (menuItemContent.videoCustom)
		{
			videoPlayerControl.ShowAndPlay(menuItemContent.videoCustom);
		}
		else if (menuItemContent.imageSource && menuItemContent.imageSource.sprite)
		{
			videoPlayerControl.Close();

			panelMediaControl.SetSprite(menuItemContent.spriteCustom 
				? menuItemContent.spriteCustom 
				: menuItemContent.imageSource.sprite);
		}
	}
}
