using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GlowEditor : ShaderGUI {

	bool showCustomInspector = true;

	Color glowColor;
	float outlineWidth;
	float glowOpacity;
	Color scaleFactor;

	public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
	{
		showCustomInspector = EditorGUILayout.Toggle("Use Custom Inspector", showCustomInspector);
		if (!showCustomInspector)
		{
			base.OnGUI(materialEditor, properties);
		}
		else
		{
			EditorGUI.BeginChangeCheck();//Start Checking if something Changed
			Material targetMat = materialEditor.target as Material;//Take material

			#region TitleImage
			//GetImage
			Texture titleTex = AssetDatabase.LoadAssetAtPath("Assets/OutlineGlowShaders/Editor/Title.png", typeof(Texture2D)) as Texture2D;//Get Title image
			GUIContent title = new GUIContent(titleTex);
			GUILayoutOption[] titleOptions = new GUILayoutOption[4];
			titleOptions[0] = GUILayout.MaxWidth(500);
			titleOptions[1] = GUILayout.MaxHeight(100);
			titleOptions[2] = GUILayout.MinWidth(150);
			titleOptions[3] = GUILayout.MinHeight(100);
			//Draw
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(title, titleOptions);//Image encapsulated to be centered
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			EditorGUILayout.Space();
			#endregion

			GUILayoutOption[] colorsLayout = new GUILayoutOption[4];
			colorsLayout[0] = GUILayout.MaxHeight(50);
			colorsLayout[1] = GUILayout.MinHeight(20);
			colorsLayout[2] = GUILayout.MinWidth(20);
			colorsLayout[3] = GUILayout.MaxWidth(500);

			GUIStyle titleStyle = new GUIStyle("label") { alignment = TextAnchor.MiddleCenter, fontSize = 13, fontStyle = FontStyle.Bold };
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Glow Color", titleStyle, GUILayout.ExpandWidth(true));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
			glowColor = EditorGUILayout.ColorField(new GUIContent(""), targetMat.GetColor("_GlowColor"), false, false, false, colorsLayout);
			GUILayout.EndHorizontal();

			outlineWidth = EditorGUILayout.Slider("Outline Width", targetMat.GetFloat("_Outline"), 0.02f, 0.25f);
			glowOpacity = EditorGUILayout.Slider("Glow Opacity", targetMat.GetFloat("_Opacity"), 0.5f, 2);

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Scale Factor", titleStyle, GUILayout.ExpandWidth(true));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			scaleFactor = EditorGUILayout.ColorField(new GUIContent(""), targetMat.GetColor("_OuterScale"),false,false,false,colorsLayout);
			EditorGUILayout.HelpBox("Use the greyscale for the scale", MessageType.Info);


			if (EditorGUI.EndChangeCheck())
			{
				targetMat.SetColor("_GlowColor", glowColor);
				targetMat.SetFloat("_Outline", outlineWidth);
				targetMat.SetFloat("_Opacity", glowOpacity);
				targetMat.SetColor("_OuterScale", scaleFactor);
			}
		}
	}
}
