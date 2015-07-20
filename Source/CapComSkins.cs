﻿#region license
/*The MIT License (MIT)
CapComSkins - Store and initialize styles and textures

Copyright (c) 2015 DMagic

KSP Plugin Framework by TriggerAu, 2014: http://forum.kerbalspaceprogram.com/threads/66503-KSP-Plugin-Framework

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using CapCom.Framework;
using UnityEngine;

namespace CapCom
{
	[KSPAddon(KSPAddon.Startup.MainMenu, true)]
	class CapComSkins : CC_MBE
	{
		internal static GUISkin ccUnitySkin;

		//Window styles
		internal static GUIStyle newWindowStyle;
		internal static GUIStyle dropDown;

		//Button styles
		internal static GUIStyle titleButton;
		internal static GUIStyle titleButtonBehind;
		internal static GUIStyle textureButton;
		internal static GUIStyle tabButton;
		internal static GUIStyle tabButtonInactive;
		internal static GUIStyle menuButton;
		internal static GUIStyle keycodeButton;
		internal static GUIStyle warningButton;

		internal static GUIStyle acceptButton;
		internal static GUIStyle acceptButtonGreyed;
		internal static GUIStyle declineButton;
		internal static GUIStyle declineButtonGreyed;
		internal static GUIStyle cancelButton;
		internal static GUIStyle cancelButtonGreyed;

		internal static GUIStyle toggleOnButton;
		internal static GUIStyle toggleOffButton;

		//Icon Button styles
		internal static GUIStyle iconButton;

		//Text label styles
		internal static GUIStyle headerText;
		internal static GUIStyle reassignText;
		internal static GUIStyle reassignCurrentText;
		internal static GUIStyle warningText;
		internal static GUIStyle titleText;
		internal static GUIStyle briefingText;
		internal static GUIStyle synopsisText;
		internal static GUIStyle parameterText;
		internal static GUIStyle subParameterText;
		internal static GUIStyle noteText;
		internal static GUIStyle smallText;
		internal static GUIStyle timerText;
		internal static GUIStyle agencyContractText;

		//Reward styles
		internal static GUIStyle advance;
		internal static GUIStyle completion;
		internal static GUIStyle failure;
		internal static GUIStyle funds;
		internal static GUIStyle rep;
		internal static GUIStyle sci;

		internal static Texture2D toolbarIcon;
		internal static Texture2D dropDownTex;
		internal static Texture2D windowTex;
		internal static Texture2D buttonHover;
		internal static Texture2D fundsGreen;
		internal static Texture2D repRed;
		internal static Texture2D science;
		internal static Texture2D orderAsc;
		internal static Texture2D orderDesc;
		internal static Texture2D goldStar;
		internal static Texture2D goldStarTwo;
		internal static Texture2D goldStarThree;
		internal static Texture2D goldStarVertical;
		internal static Texture2D goldStarTwoVertical;
		internal static Texture2D goldStarThreeVertical;
		internal static Texture2D settingsIcon;
		internal static Texture2D resizeHandle;
		internal static Texture2D checkBox;
		internal static Texture2D failBox;
		internal static Texture2D emptyBox;
		internal static Texture2D notesPlusIcon;
		internal static Texture2D notesMinusIcon;
		internal static Texture2D sortStars;
		internal static Texture2D sortPlanet;
		internal static Texture2D toggleOn;
		internal static Texture2D toggleOff;
		internal static Texture2D toggleHoverOff;
		internal static Texture2D toggleHoverOn;
		internal static Texture2D currentFlag;

		internal static Texture2D acceptButtonNormal;
		internal static Texture2D acceptButtonHover;
		internal static Texture2D acceptButtonActive;
		internal static Texture2D acceptButtonInactive;

		internal static Texture2D declineButtonNormal;
		internal static Texture2D declineButtonHover;
		internal static Texture2D declineButtonActive;
		internal static Texture2D declineButtonInactive;

		internal static Texture2D cancelButtonNormal;
		internal static Texture2D cancelButtonHover;
		internal static Texture2D cancelButtonActive;
		internal static Texture2D cancelButtonInactive;

		internal static Texture2D titleButtonNormal;
		internal static Texture2D titleButtonActive;

		internal static Texture2D tabButtonNormalLeft;

		internal static Texture2D tabButtonHoverLeft;

		internal static Texture2D tabButtonActiveLeft;

		internal static Texture2D flagTex = new Texture2D(1, 1);

		internal static int fontSize = 0;

		protected override void OnGUIOnceOnly()
		{
			windowTex = GameDatabase.Instance.GetTexture("CapCom/Textures/WindowTex", false);
			dropDownTex = GameDatabase.Instance.GetTexture("CapCom/Textures/DropDownTex", false);
			toolbarIcon = GameDatabase.Instance.GetTexture("CapCom/Textures/CapComAppIcon", false);
			buttonHover = GameDatabase.Instance.GetTexture("CapCom/Textures/ButtonHover", false);
			fundsGreen = GameDatabase.Instance.GetTexture("CapCom/Textures/FundsGreenIcon", false);
			repRed = GameDatabase.Instance.GetTexture("CapCom/Textures/RepRedIcon", false);
			science = GameDatabase.Instance.GetTexture("CapCom/Textures/ScienceIcon", false);
			orderAsc = GameDatabase.Instance.GetTexture("CapCom/Textures/OrderAsc", false);
			orderDesc = GameDatabase.Instance.GetTexture("CapCom/Textures/OrderDesc", false);
			goldStar = GameDatabase.Instance.GetTexture("CapCom/Textures/GoldStar", false);
			goldStarTwo = GameDatabase.Instance.GetTexture("CapCom/Textures/GoldStarTwo", false);
			goldStarThree = GameDatabase.Instance.GetTexture("CapCom/Textures/GoldStarThree", false);
			goldStarVertical = GameDatabase.Instance.GetTexture("CapCom/Textures/GoldStarVertical", false);
			goldStarTwoVertical = GameDatabase.Instance.GetTexture("CapCom/Textures/GoldStarTwoVertical", false);
			goldStarThreeVertical = GameDatabase.Instance.GetTexture("CapCom/Textures/GoldStarThreeVertical", false);
			settingsIcon = GameDatabase.Instance.GetTexture("CapCom/Textures/ToolbarSettingsIcon", false);
			resizeHandle = GameDatabase.Instance.GetTexture("CapCom/Textures/ResizeIcon", false);
			checkBox = GameDatabase.Instance.GetTexture("CapCom/Textures/CheckBoxIcon", false);
			failBox = GameDatabase.Instance.GetTexture("CapCom/Textures/FailBoxIcon", false);
			emptyBox = GameDatabase.Instance.GetTexture("CapCom/Textures/EmptyBoxIcon", false);
			notesPlusIcon = GameDatabase.Instance.GetTexture("CapCom/Textures/OpenNotesIcon", false);
			notesMinusIcon = GameDatabase.Instance.GetTexture("CapCom/Textures/CloseNotesIcon", false);
			sortStars = GameDatabase.Instance.GetTexture("CapCom/Textures/SortDifficultyIcon", false);
			sortPlanet = GameDatabase.Instance.GetTexture("CapCom/Textures/SortPlanetsIcon", false);

			toggleButtons();
			initializeSkins();
		}

		private static void toggleButtons()
		{
			Texture2D toggleOnOriginal = CC_SkinsLibrary.DefKSPSkin.toggle.onNormal.background;
			Texture2D toggleOffOriginal = CC_SkinsLibrary.DefKSPSkin.toggle.normal.background;
			Texture2D toggleHoverOnOriginal = CC_SkinsLibrary.DefKSPSkin.toggle.onHover.background;
			Texture2D toggleHoverOffOriginal = CC_SkinsLibrary.DefKSPSkin.toggle.hover.background;

			toggleOn = new Texture2D(toggleOnOriginal.width, toggleOnOriginal.height);
			toggleOff = new Texture2D(toggleOffOriginal.width, toggleOffOriginal.height);
			toggleHoverOn = new Texture2D(toggleHoverOnOriginal.width, toggleHoverOnOriginal.height);
			toggleHoverOff = new Texture2D(toggleHoverOffOriginal.width, toggleHoverOffOriginal.height);

			var rt = RenderTexture.GetTemporary(toggleOnOriginal.width, toggleOnOriginal.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB, 1);

			Graphics.Blit(toggleOnOriginal, rt);

			RenderTexture.active = rt;

			toggleOn.ReadPixels(new Rect(0, 0, toggleOnOriginal.width, toggleOnOriginal.height), 0, 0);

			RenderTexture.active = null;
			RenderTexture.ReleaseTemporary(rt);

			toggleOn.Apply();

			rt = RenderTexture.GetTemporary(toggleOffOriginal.width, toggleOffOriginal.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB, 1);

			Graphics.Blit(toggleOffOriginal, rt);

			RenderTexture.active = rt;

			toggleOff.ReadPixels(new Rect(0, 0, toggleOffOriginal.width, toggleOffOriginal.height), 0, 0);

			RenderTexture.active = null;
			RenderTexture.ReleaseTemporary(rt);

			toggleOff.Apply();

			rt = RenderTexture.GetTemporary(toggleHoverOnOriginal.width, toggleHoverOnOriginal.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB, 1);

			Graphics.Blit(toggleHoverOnOriginal, rt);

			RenderTexture.active = rt;

			toggleHoverOn.ReadPixels(new Rect(0, 0, toggleHoverOnOriginal.width, toggleHoverOnOriginal.height), 0, 0);

			RenderTexture.active = null;
			RenderTexture.ReleaseTemporary(rt);

			toggleHoverOn.Apply();

			rt = RenderTexture.GetTemporary(toggleHoverOffOriginal.width, toggleHoverOffOriginal.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB, 1);

			Graphics.Blit(toggleHoverOffOriginal, rt);

			RenderTexture.active = rt;

			toggleHoverOff.ReadPixels(new Rect(0, 0, toggleHoverOffOriginal.width, toggleHoverOffOriginal.height), 0, 0);

			RenderTexture.active = null;
			RenderTexture.ReleaseTemporary(rt);

			toggleHoverOff.Apply();

			rt = null;

			toggleOnOriginal = null;
			toggleOffOriginal = null;
			toggleHoverOnOriginal = null;
			toggleHoverOffOriginal = null;

			var pix = toggleOn.GetPixels(14, 19, 92, 92);
			toggleOn = new Texture2D(92, 92);
			toggleOn.SetPixels(pix);
			toggleOn.Apply();

			TextureScale.Bilinear(toggleOn, 26, 26);
			pix = toggleOn.GetPixels();
			toggleOn = new Texture2D(28, 28);
			Color[] clearPix = new Color[28 * 28];
			for (int i = 0; i < clearPix.Length; i++)
			{
				clearPix[i] = Color.clear;
			}
			toggleOn.SetPixels(clearPix);
			toggleOn.SetPixels(0, 1, 26, 26, pix);
			toggleOn.Apply();

			pix = toggleOff.GetPixels(14, 19, 92, 92);
			toggleOff = new Texture2D(92, 92);
			toggleOff.SetPixels(pix);
			toggleOff.Apply();

			TextureScale.Bilinear(toggleOff, 26, 26);

			pix = toggleHoverOn.GetPixels(14, 19, 92, 92);
			toggleHoverOn = new Texture2D(92, 92);
			toggleHoverOn.SetPixels(pix);
			toggleHoverOn.Apply();

			TextureScale.Bilinear(toggleHoverOn, 26, 26);
			pix = toggleHoverOn.GetPixels();
			toggleHoverOn = new Texture2D(28, 28);
			clearPix = new Color[28 * 28];
			for (int i = 0; i < clearPix.Length; i++)
			{
				clearPix[i] = Color.clear;
			}
			toggleHoverOn.SetPixels(clearPix);
			toggleHoverOn.SetPixels(0, 1, 26, 26, pix);
			toggleHoverOn.Apply();

			pix = toggleHoverOff.GetPixels(14, 19, 92, 92);
			toggleHoverOff = new Texture2D(92, 92);
			toggleHoverOff.SetPixels(pix);
			toggleHoverOff.Apply();

			TextureScale.Bilinear(toggleHoverOff, 26, 26);
		}

		internal static void texturesFromAtlas(Texture2D atlas)
		{
			var pix = atlas.GetPixels(0, 372, 48, 49);

			acceptButtonNormal = new Texture2D(48, 49);
			acceptButtonNormal.SetPixels(pix);
			acceptButtonNormal.Apply();

			pix = atlas.GetPixels(49, 372, 48, 49);

			acceptButtonHover = new Texture2D(48, 49);
			acceptButtonHover.SetPixels(pix);
			acceptButtonHover.Apply();

			pix = atlas.GetPixels(98, 372, 48, 49);

			acceptButtonActive = new Texture2D(48, 49);
			acceptButtonActive.SetPixels(pix);
			acceptButtonActive.Apply();

			pix = atlas.GetPixels(0, 423, 48, 49);

			acceptButtonInactive = new Texture2D(48, 49);
			acceptButtonInactive.SetPixels(pix);
			acceptButtonInactive.Apply();

			pix = atlas.GetPixels(49, 423, 48, 49);

			declineButtonNormal = new Texture2D(48, 49);
			declineButtonNormal.SetPixels(pix);
			declineButtonNormal.Apply();

			pix = atlas.GetPixels(98, 423, 48, 49);

			declineButtonHover = new Texture2D(48, 49);
			declineButtonHover.SetPixels(pix);
			declineButtonHover.Apply();

			pix = atlas.GetPixels(0, 474, 48, 49);

			declineButtonActive = new Texture2D(48, 49);
			declineButtonActive.SetPixels(pix);
			declineButtonActive.Apply();

			pix = atlas.GetPixels(49, 474, 48, 49);

			declineButtonInactive = new Texture2D(48, 49);
			declineButtonInactive.SetPixels(pix);
			declineButtonInactive.Apply();

			pix = atlas.GetPixels(98, 474, 48, 49);

			cancelButtonNormal = new Texture2D(48, 49);
			cancelButtonNormal.SetPixels(pix);
			cancelButtonNormal.Apply();

			pix = atlas.GetPixels(0, 525, 48, 49);

			cancelButtonHover = new Texture2D(48, 49);
			cancelButtonHover.SetPixels(pix);
			cancelButtonHover.Apply();

			pix = atlas.GetPixels(49, 525, 48, 49);

			cancelButtonActive = new Texture2D(48, 49);
			cancelButtonActive.SetPixels(pix);
			cancelButtonActive.Apply();

			pix = atlas.GetPixels(98, 525, 48, 49);

			cancelButtonInactive = new Texture2D(48, 49);
			cancelButtonInactive.SetPixels(pix);
			cancelButtonInactive.Apply();

			pix = atlas.GetPixels(0, 319, 102, 51);

			titleButtonNormal = new Texture2D(102, 51);
			titleButtonNormal.SetPixels(pix);
			titleButtonNormal.Apply();

			pix = atlas.GetPixels(0, 268, 102, 51);

			titleButtonActive = new Texture2D(102, 51);
			titleButtonActive.SetPixels(pix);
			titleButtonActive.Apply();

			pix = atlas.GetPixels(0, 372, 48, 49);

			tabButtonNormalLeft = new Texture2D(48, 49);
			tabButtonNormalLeft.SetPixels(pix);
			tabButtonNormalLeft.Apply();

			pix = atlas.GetPixels(0, 372, 48, 49);

			tabButtonHoverLeft = new Texture2D(48, 49);
			tabButtonHoverLeft.SetPixels(pix);
			tabButtonHoverLeft.Apply();

			pix = atlas.GetPixels(0, 372, 48, 49);

			tabButtonActiveLeft = new Texture2D(48, 49);
			tabButtonActiveLeft.SetPixels(pix);
			tabButtonActiveLeft.Apply();

			pix = atlas.GetPixels(0, 155, 172, 113);

			flagTex = new Texture2D(172, 113);
			flagTex.SetPixels(pix);
			flagTex.Apply();

			atlasStyles();
		}

		private static void atlasStyles()
		{
			acceptButton = new GUIStyle(CC_SkinsLibrary.DefUnitySkin.button);
			acceptButton.padding = new RectOffset(0, 0, 0, 0);
			acceptButton.normal.background = acceptButtonNormal;
			acceptButton.hover.background = acceptButtonHover;
			acceptButton.active.background = acceptButtonActive;

			acceptButtonGreyed = new GUIStyle(acceptButton);
			acceptButtonGreyed.normal.background = acceptButtonGreyed.hover.background = acceptButtonGreyed.active.background = acceptButtonInactive;

			declineButton = new GUIStyle(acceptButton);
			declineButton.normal.background = declineButtonNormal;
			declineButton.hover.background = declineButtonHover;
			declineButton.active.background = declineButtonActive;

			declineButtonGreyed = new GUIStyle(acceptButtonGreyed);
			declineButtonGreyed.normal.background = declineButtonGreyed.hover.background = declineButtonGreyed.active.background = declineButtonInactive;

			cancelButton = new GUIStyle(acceptButton);
			cancelButton.normal.background = cancelButtonNormal;
			cancelButton.hover.background = cancelButtonHover;
			cancelButton.active.background = cancelButtonActive;

			cancelButtonGreyed = new GUIStyle(acceptButtonGreyed);
			cancelButtonGreyed.normal.background = cancelButtonGreyed.hover.background = cancelButtonGreyed.active.background = cancelButtonInactive;
		}

		internal static void initializeSkins()
		{
			ccUnitySkin = CC_SkinsLibrary.CopySkin(CC_SkinsLibrary.DefSkinType.Unity);
			CC_SkinsLibrary.AddSkin("CCUnitySkin", ccUnitySkin);

			newWindowStyle = new GUIStyle(CC_SkinsLibrary.DefUnitySkin.window);
			newWindowStyle.name = "WindowStyle";
			newWindowStyle.fontSize = 14;
			newWindowStyle.fontStyle = FontStyle.Bold;
			newWindowStyle.padding = new RectOffset(0, 1, 20, 12);
			newWindowStyle.normal.background = windowTex;
			newWindowStyle.focused.background = newWindowStyle.normal.background;
			newWindowStyle.onNormal.background = newWindowStyle.normal.background;

			dropDown = new GUIStyle(CC_SkinsLibrary.DefUnitySkin.box);
			dropDown.name = "DropDown";
			dropDown.normal.background = dropDownTex;

			//Button Styles
			titleButton = new GUIStyle(CC_SkinsLibrary.DefUnitySkin.button);
			titleButton.name = "TitleButton";
			titleButton.fontSize = 12 + fontSize;
			titleButton.wordWrap = true;
			titleButton.fontStyle = FontStyle.Bold;
			titleButton.normal.textColor = XKCDColors.ButterYellow;
			titleButton.padding = new RectOffset(2, 14, 2, 2);

			titleButtonBehind = new GUIStyle(titleButton);
			titleButtonBehind.hover.background = titleButtonBehind.normal.background;
			titleButtonBehind.hover.textColor = titleButtonBehind.normal.textColor;

			tabButton = new GUIStyle(CC_SkinsLibrary.DefUnitySkin.button);
			tabButton.fontSize = 14 + fontSize;
			tabButton.fontStyle = FontStyle.Bold;
			tabButton.normal.textColor = XKCDColors.White;

			tabButtonInactive = new GUIStyle(tabButton);
			tabButtonInactive.fontSize = 12 + fontSize;
			tabButtonInactive.normal.textColor = XKCDColors.LightGrey;

			textureButton = new GUIStyle(CC_SkinsLibrary.DefUnitySkin.button);
			textureButton.fontSize = 14;
			textureButton.fontStyle = FontStyle.Bold;
			textureButton.normal.background = CC_SkinsLibrary.DefUnitySkin.label.normal.background;
			textureButton.hover.background = buttonHover;
			textureButton.alignment = TextAnchor.MiddleCenter;
			textureButton.padding = new RectOffset(1, 1, 2, 2);

			menuButton = new GUIStyle(CC_SkinsLibrary.DefUnitySkin.label);
			menuButton.fontSize = 12 + fontSize;
			menuButton.fontStyle = FontStyle.Bold;
			menuButton.padding = new RectOffset(26, 2, 2, 2);
			menuButton.normal.textColor = XKCDColors.White;
			menuButton.hover.textColor = XKCDColors.AlmostBlack;
			Texture2D sortBackground = new Texture2D(1, 1);
			sortBackground.SetPixel(1, 1, XKCDColors.OffWhite);
			sortBackground.Apply();
			menuButton.hover.background = sortBackground;
			menuButton.alignment = TextAnchor.MiddleLeft;

			warningButton = new GUIStyle(CC_SkinsLibrary.DefUnitySkin.button);
			warningButton.fontSize = 13 + fontSize;
			warningButton.fontStyle = FontStyle.Bold;
			warningButton.alignment = TextAnchor.MiddleCenter;

			iconButton = new GUIStyle(CC_SkinsLibrary.DefUnitySkin.label);

			keycodeButton = new GUIStyle(CC_SkinsLibrary.DefUnitySkin.button);

			//Toggle Buttons
			toggleOnButton = new GUIStyle(CC_SkinsLibrary.DefUnitySkin.button);
			toggleOnButton.padding = new RectOffset(0, 0, 0, 0);
			toggleOnButton.border = new RectOffset(27, 0, 0, 0);
			toggleOnButton.normal.background = toggleOn;
			toggleOnButton.active.background = toggleHoverOn;
			toggleOnButton.hover.background = toggleHoverOn;

			toggleOffButton = new GUIStyle(toggleOnButton);
			toggleOffButton.normal.background = toggleOff;
			toggleOffButton.active.background = toggleHoverOff;
			toggleOffButton.hover.background = toggleHoverOff;

			//Label Styles
			headerText = new GUIStyle(CC_SkinsLibrary.DefUnitySkin.label);
			headerText.fontSize = 13 + fontSize;
			headerText.fontStyle = FontStyle.Bold;
			headerText.alignment = TextAnchor.MiddleLeft;
			headerText.normal.textColor = XKCDColors.FadedOrange;

			warningText = new GUIStyle(headerText);
			warningText.alignment = TextAnchor.MiddleCenter;
			warningText.normal.textColor = XKCDColors.VomitYellow;

			reassignText = new GUIStyle(warningText);
			reassignText.normal.textColor = Color.white;

			reassignCurrentText = new GUIStyle(reassignText);
			reassignCurrentText.fontStyle = FontStyle.Bold;

			titleText = new GUIStyle(headerText);
			titleText.normal.textColor = XKCDColors.White;

			briefingText = new GUIStyle(CC_SkinsLibrary.DefUnitySkin.label);
			briefingText.fontSize = 11 + fontSize;
			briefingText.alignment = TextAnchor.MiddleLeft;
			briefingText.normal.textColor = XKCDColors.KSPNeutralUIGrey;

			synopsisText = new GUIStyle(briefingText);
			synopsisText.fontSize = 12 + fontSize;
			synopsisText.fontStyle = FontStyle.Bold;

			parameterText = new GUIStyle(synopsisText);
			parameterText.normal.textColor = XKCDColors.Beige;

			subParameterText = new GUIStyle(parameterText);
			subParameterText.normal.textColor = XKCDColors.DarkBeige;

			timerText = new GUIStyle(synopsisText);
			timerText.normal.textColor = XKCDColors.OffWhite;

			noteText = new GUIStyle(synopsisText);
			noteText.fontStyle = FontStyle.Normal;
			noteText.normal.textColor = XKCDColors.TiffanyBlue;

			agencyContractText = new GUIStyle(synopsisText);
			agencyContractText.normal.textColor = XKCDColors.ButterYellow;

			smallText = new GUIStyle(CC_SkinsLibrary.DefUnitySkin.label);
			smallText.fontSize = 11 + fontSize;
			smallText.alignment = TextAnchor.MiddleLeft;

			//Reward and Penalty Styles
			advance = new GUIStyle(CC_SkinsLibrary.DefUnitySkin.label);
			advance.fontSize = 12 + fontSize;
			advance.fontStyle = FontStyle.Bold;
			advance.alignment = TextAnchor.MiddleLeft;
			advance.wordWrap = false;
			advance.normal.textColor = XKCDColors.DullYellow;

			completion = new GUIStyle(advance);
			completion.normal.textColor = XKCDColors.DustyGreen;

			failure = new GUIStyle(advance);
			failure.normal.textColor = XKCDColors.DustyRed;

			funds = new GUIStyle(advance);
			funds.fontStyle = FontStyle.Normal;
			funds.normal.textColor = XKCDColors.PaleOliveGreen;

			rep = new GUIStyle(funds);
			rep.normal.textColor = XKCDColors.BrownishYellow;

			sci = new GUIStyle(funds);
			sci.normal.textColor = XKCDColors.AquaBlue;

			//Add Default Styles
			CC_SkinsLibrary.List["CCUnitySkin"].window = new GUIStyle(newWindowStyle);
			CC_SkinsLibrary.List["CCUnitySkin"].box = new GUIStyle(dropDown);
			CC_SkinsLibrary.List["CCUnitySkin"].button = new GUIStyle(titleButton);

			CC_SkinsLibrary.AddStyle("CCUnitySkin", newWindowStyle);
			CC_SkinsLibrary.AddStyle("CCUnitySkin", dropDown);
			CC_SkinsLibrary.AddStyle("CCUnitySkin", titleButton);
		}
	}
}
