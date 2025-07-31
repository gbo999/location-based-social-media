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
using System.Collections;

[HelpURL("https://makaka.org/unity-assets")]
public class ArrowBlinkingControl : MonoBehaviour 
{
	public float scrollSpeed = 0.5f;
	public Renderer rend;

	void Update() 
	{
		rend.material.SetTextureOffset("_MainTex", new Vector2(0f, -Time.time * scrollSpeed));
	}

}
