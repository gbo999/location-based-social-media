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
using UnityEngine.UI;
using UnityEngine.Video;

[HelpURL("https://makaka.org/unity-assets")]
public class MenuItemContent : MonoBehaviour
{
	public Image imageSource;

    [Header("Texts")]

	public string header;
	public string description;
	public string link;

	[Header("Media (use only one)")]

	public Sprite spriteCustom;
	public VideoClip videoCustom;
}
