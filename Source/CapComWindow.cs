﻿#region license
/*The MIT License (MIT)
CapComWindow - Primary interface for the addon

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
using Contracts;
using Contracts.Agents;
using UnityEngine;

namespace CapCom
{
	public class CapComWindow : CC_MBW
	{
		private List<CapComContract> activeContracts = new List<CapComContract>();
		private List<CapComContract> offeredContracts = new List<CapComContract>();
		private List<CapComContract> completedContracts = new List<CapComContract>();
		private List<CapComContract> currentContracts = new List<CapComContract>();
		private List<CapComContract> agentOfferedContracts = new List<CapComContract>();
		private List<CapComContract> agentActiveContracts = new List<CapComContract>();
		private CapComContract currentContract;
		private CapComSettingsWindow settings;

		private Vector2 cScroll, infoScroll;
		private int contractIndex;
		private int currentList;
		private bool controlLock;
		private bool showAgency;
		private bool resizing;
		private bool mouseDown;
		private bool dropdown, warnDecline, warnCancel, sortMenu;
		private Rect ddRect;
		private float dH, dragStart;
		private int maxContracts;

		private const string lockID = "CapCom_LockID";

		protected override void Awake()
		{
			WindowRect = new Rect(70, 100, 840, 600);
			WindowCaption = "CapCom";
			WindowOptions = new GUILayoutOption[3] { GUILayout.Width(840), GUILayout.Height(300), GUILayout.MaxHeight(Screen.height) };
			WindowStyle = CapComSkins.newWindowStyle;
			Visible = false;
			DragEnabled = true;
			ClampToScreen = true;
			ClampToScreenOffset = new RectOffset(-650, -650, -300, -300);
			TooltipMouseOffset = new Vector2d(-10, -25);


			currentList = 0;

			CC_SkinsLibrary.SetCurrent("CCUnitySkin");

			InputLockManager.RemoveControlLock(lockID);
		}

		protected override void Start()
		{
			WindowRect.x = CapCom.Settings.windowPosX;
			WindowRect.y = CapCom.Settings.windowPosY;
			WindowRect.yMax = WindowRect.y + CapCom.Settings.windowHeight;
			TooltipsEnabled = CapCom.Settings.tooltipsEnabled;

			maxContracts = getMaxContracts();
		}

		protected override void OnDestroy()
		{
			CapCom.Settings.windowPosX = WindowRect.x;
			CapCom.Settings.windowPosY = WindowRect.y;
			CapCom.Settings.Save();
		}

		private int getMaxContracts()
		{
			return GameVariables.Instance.GetActiveContractsLimit(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.MissionControl));
		}

		protected override void Update()
		{
			Vector2 mousePos = Input.mousePosition;
			mousePos.y = Screen.height - mousePos.y;
			if (WindowRect.Contains(mousePos))
			{
				if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
					mouseDown = true;
				else
					mouseDown = false;

				if (Input.GetKeyDown(CapCom.Settings.scrollUp))
				{
					if (currentContracts.Count > 0)
					{
						if (currentContract == null)
						{
							contractIndex = currentContracts.Count - 1;
							currentContract = currentContracts[contractIndex];
						}
						else if (contractIndex > 0)
						{
							contractIndex -= 1;
							currentContract = currentContracts[contractIndex];
						}
						else
						{
							contractIndex = currentContracts.Count - 1;
							currentContract = currentContracts[contractIndex];
						}
						showAgency = false;
					}
				}
				if (Input.GetKeyDown(CapCom.Settings.scrollDown))
				{
					if (currentContracts.Count > 0)
					{
						if (currentContract == null)
						{
							contractIndex = 0;
							currentContract = currentContracts[contractIndex];
						}
						else if (contractIndex >= currentContracts.Count - 1)
						{
							contractIndex = 0;
							currentContract = currentContracts[contractIndex];
						}
						else
						{
							contractIndex += 1;
							currentContract = currentContracts[contractIndex];
						}
						showAgency = false;
					}
				}
				if (Input.GetKeyDown(CapCom.Settings.listRight))
				{
					if (currentList < 2)
						currentList += 1;
					else
						currentList = 0;

					sortContracts();

					if (currentContracts.Count > 0)
					{
						contractIndex = 0;
						currentContract = currentContracts[contractIndex];
						showAgency = false;
					}
					else
					{
						currentContract = null;
						contractIndex = 0;
						showAgency = false;
					}
				}
				if (Input.GetKeyDown(CapCom.Settings.listLeft))
				{
					if (currentList > 0)
						currentList -= 1;
					else
						currentList = 2;

					sortContracts();

					if (currentContracts.Count > 0)
					{
						contractIndex = 0;
						currentContract = currentContracts[contractIndex];
						showAgency = false;
					}
					else
					{
						currentContract = null;
						contractIndex = 0;
						showAgency = false;
					}
				}
				if (Input.GetKeyDown(CapCom.Settings.accept))
				{
					if (currentContract == null)
						return;

					if (currentContract.Root.ContractState != Contract.State.Offered)
						return;

					maxContracts = getMaxContracts();

					if (!CapCom.Settings.activeLimit || ContractSystem.Instance.GetActiveContractCount() < maxContracts)
						currentContract.Root.Accept();
				}
				if (Input.GetKeyDown(CapCom.Settings.cancel))
				{
					if (currentContract == null)
						return;

					if (currentContract.Root.ContractState == Contract.State.Offered)
					{
						if (!CapCom.Settings.forceDecline && !currentContract.CanBeDeclined)
							return;

						if (CapCom.Settings.showDeclineWarning)
						{
							dropdown = true;
							warnDecline = true;
						}
						else
							currentContract.Root.Decline();
					}
					else if (currentContract.Root.ContractState == Contract.State.Active)
					{
						if (!CapCom.Settings.forceCancel && !currentContract.CanBeCancelled)
							return;

						if (CapCom.Settings.showCancelWarning)
						{
							dropdown = true;
							warnCancel = true;
						}
						else
							currentContract.Root.Cancel();
					}
				}
			}
		}

		public void refreshContracts()
		{
			if (CapCom.Instance == null)
				LogFormatted("CapCom Instance Not Loaded; Something Went Wrong Here...");

			activeContracts = CapCom.Instance.getActiveContracts;
			offeredContracts = CapCom.Instance.getOfferedContracts;
			completedContracts = CapCom.Instance.getCompletedContracts;

			sortContracts();
		}

		private void sortContracts()
		{
			switch (currentList)
			{
				case 0:
					sortContracts(offeredContracts);
					break;
				case 1:
					sortContracts(activeContracts);
					break;
				case 2:
					sortContracts(completedContracts);
					break;
			}
		}

		private void sortContracts(List<CapComContract> ccList)
		{
			switch (CapCom.Settings.sortMode)
			{
				case 0:
					ccList.Sort((a, b) => RUIutils.SortAscDescPrimarySecondary(CapCom.Settings.ascending, a.Root.Prestige.CompareTo(b.Root.Prestige), a.Name.CompareTo(b.Name)));
					break;
				case 1:
					ccList.Sort((a, b) => RUIutils.SortAscDescPrimarySecondary(CapCom.Settings.ascending, a.TotalReward.CompareTo(b.TotalReward), a.Name.CompareTo(b.Name)));
					break;
				case 2:
					ccList.Sort((a, b) => RUIutils.SortAscDescPrimarySecondary(CapCom.Settings.ascending, a.TotalSciReward.CompareTo(b.TotalSciReward), a.Name.CompareTo(b.Name)));
					break;
				case 3:
					ccList.Sort((a, b) => RUIutils.SortAscDescPrimarySecondary(CapCom.Settings.ascending, a.TotalRepReward.CompareTo(b.TotalRepReward), a.Name.CompareTo(b.Name)));
					break;
				case 4:
					ccList.Sort((a, b) => RUIutils.SortAscDescPrimarySecondary(CapCom.Settings.ascending, a.RootAgent.Name.CompareTo(b.RootAgent.Name), a.Name.CompareTo(b.Name)));
					break;
				case 5:
					ccList.Sort((a, b) => RUIutils.SortAscDescPrimarySecondary(CapCom.Settings.ascending, a.Target.CompareTo(b.Target), a.Name.CompareTo(b.Name)));
					break;
				default:
					ccList.Sort((a, b) => RUIutils.SortAscDescPrimarySecondary(CapCom.Settings.ascending, a.Root.Prestige.CompareTo(b.Root.Prestige), a.Name.CompareTo(b.Name)));
					break;
			}

			currentContracts = ccList;
		}

		private void unlockControls()
		{
			switch (HighLogic.LoadedScene)
			{
				case GameScenes.TRACKSTATION:
				case GameScenes.SPACECENTER:
				case GameScenes.FLIGHT:
					InputLockManager.RemoveControlLock(lockID);
					controlLock = false;
					break;
				case GameScenes.EDITOR:
					EditorLogic.fetch.Unlock(lockID);
					controlLock = false;
					break;
				default:
					break;
			}
		}

		protected override void DrawWindowPre(int id)
		{
			//Prevent click through from activating part options
			if (HighLogic.LoadedSceneIsFlight)
			{
				Vector2 mousePos = Input.mousePosition;
				mousePos.y = Screen.height - mousePos.y;
				if (WindowRect.Contains(mousePos) && GUIUtility.hotControl == 0 && Input.GetMouseButton(0))
				{
					foreach (var window in GameObject.FindObjectsOfType(typeof(UIPartActionWindow)).OfType<UIPartActionWindow>().Where(p => p.Display == UIPartActionWindow.DisplayType.Selected))
					{
						window.enabled = false;
						window.displayDirty = true;
					}
				}
				else if (WindowRect.Contains(mousePos) && !controlLock)
				{
					InputLockManager.SetControlLock(ControlTypes.CAMERACONTROLS, lockID);
					controlLock = true;
				}
				else if (!WindowRect.Contains(mousePos) && controlLock)
					unlockControls();
			}

			//Lock space center click through
			if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
			{
				Vector2 mousePos = Input.mousePosition;
				mousePos.y = Screen.height - mousePos.y;
				if (WindowRect.Contains(mousePos) && !controlLock)
				{
					InputLockManager.SetControlLock(ControlTypes.CAMERACONTROLS | ControlTypes.KSC_FACILITIES | ControlTypes.KSC_UI, lockID);
					controlLock = true;
				}
				else if (!WindowRect.Contains(mousePos) && controlLock)
					unlockControls();
			}

			//Lock tracking scene click through
			if (HighLogic.LoadedScene == GameScenes.TRACKSTATION)
			{
				Vector2 mousePos = Input.mousePosition;
				mousePos.y = Screen.height - mousePos.y;
				if (WindowRect.Contains(mousePos) && !controlLock)
				{
					InputLockManager.SetControlLock(ControlTypes.CAMERACONTROLS | ControlTypes.TRACKINGSTATION_ALL, lockID);
					controlLock = true;
				}
				else if (!WindowRect.Contains(mousePos) && controlLock)
					unlockControls();
			}

			//Lock editor click through
			if (HighLogic.LoadedSceneIsEditor)
			{
				Vector2 mousePos = Input.mousePosition;
				mousePos.y = Screen.height - mousePos.y;
				if (WindowRect.Contains(mousePos) && !controlLock)
				{
					EditorLogic.fetch.Lock(true, true, true, lockID);
					controlLock = true;
				}
				else if (!WindowRect.Contains(mousePos) && controlLock)
					unlockControls();
			}

			if (!dropdown)
			{
				warnCancel = false;
				warnDecline = false;
				sortMenu = false;
			}
		}

		protected override void DrawWindow(int id)
		{
			drawVersion(id);
			closeButton(id);

			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
				GUILayout.BeginVertical();
					menuBar(id);
					GUILayout.Space(40);
					contractTabs(id);
					contractList(id);
				GUILayout.EndVertical();
				GUILayout.Space(10);
				GUILayout.BeginVertical();
					currentContractControls(id);
					GUILayout.Space(10);
					currentContractHeader(id);
					if (showAgency)
						currentAgentInfo(id);
					else
						currentContractInfo(id);
				GUILayout.EndVertical();
				GUILayout.Space(10);
			GUILayout.EndHorizontal();
			GUILayout.Space(10);

			dropDown(id);
			rescaleHandle(id);
		}

		protected override void DrawWindowPost(int id)
		{
			if (dropdown && Event.current.type == EventType.mouseDown && !ddRect.Contains(Event.current.mousePosition))
			{
				dropdown = false;
			}
		}

		private void drawVersion(int id)
		{
			Rect r = new Rect(6, 0, 50, 18);
			GUI.Label(r, CapCom.Version, CapComSkins.smallText);
		}

		private void closeButton(int id)
		{
			Rect r = new Rect(WindowRect.width - 60, 1, 24, 24);
			if (GUI.Button(r, new GUIContent(CapComSkins.settingsIcon, "Settings Menu"), CapComSkins.textureButton))
			{
				if (settings == null)
					settings = gameObject.AddComponent<CapComSettingsWindow>();
				settings.Visible = !settings.Visible;
			}

			r.x += 32;
			r.width = r.height = 22;

			if (GUI.Button(r, "✖", CapComSkins.textureButton))
			{
				unlockControls();
				Visible = false;
			}
		}

		private void menuBar(int id)
		{
			Rect r = new Rect(90, 0, 360, 20);

			maxContracts = getMaxContracts();
			string s = maxContracts < 1000000 ? string.Format("[{0} Max]", maxContracts) : "";
			GUI.Label(r, string.Format("{0} Active Contracts {1}", ContractSystem.Instance.GetActiveContractCount(), s), CapComSkins.headerText);

			r = new Rect(18, 20, 54, 44);
			Rect t = new Rect(-5, 20, 72, 44);

			if (GUI.Button(t, new GUIContent("", "Sort By Difficulty"), CapCom.Settings.sortMode == 0 ? CapComSkins.toggleOnButton : CapComSkins.toggleOffButton))
			{
				CapCom.Settings.sortMode = 0;
				CapCom.Settings.Save();
				sortContracts();
			}

			GUI.DrawTexture(r, CapComSkins.sortStars);

			r.x += 68;
			t.x += 72;

			if (GUI.Button(t, new GUIContent("", "Sort By Rewards"), CapCom.Settings.sortMode == 1 || CapCom.Settings.sortMode == 2 || CapCom.Settings.sortMode == 3 ? CapComSkins.toggleOnButton : CapComSkins.toggleOffButton))
			{
				dropdown = !dropdown;
				sortMenu = !sortMenu;
			}

			r.y += 18;
			r.height = 24;
			r.width = 24;
			if (CapCom.Settings.sortMode == 1)
			{
				GUI.DrawTexture(r, CapComSkins.repRed);
			}
			else
			{
				GUI.DrawTexture(r, CapComSkins.fundsGreen);
			}

			r.x += 28;

			GUI.DrawTexture(r, CapCom.Settings.sortMode == 2 ? CapComSkins.repRed : CapComSkins.science);

			r.x -= 20;
			r.y -= 16;
			r.width = 34;
			r.height = 34;
			if (CapCom.Settings.sortMode == 0 || CapCom.Settings.sortMode == 4 || CapCom.Settings.sortMode == 5)
			{
				r.height = 24;
				r.width = 24;
				r.y += 4;
				r.x += 4;
				GUI.DrawTexture(r, CapComSkins.repRed);
				r.y -= 4;
				r.x -= 4;
			}
			else if (CapCom.Settings.sortMode == 1)
			{
				GUI.DrawTexture(r, CapComSkins.fundsGreen);
			}
			else if (CapCom.Settings.sortMode == 2)
			{
				GUI.DrawTexture(r, CapComSkins.science);
			}
			else
			{
				GUI.DrawTexture(r, CapComSkins.repRed);
			}

			r.x += 68;
			r.width = 54;
			r.height = 40;
			t.x += 73;

			if (GUI.Button(t, new GUIContent("", "Sort By Agency"), CapCom.Settings.sortMode == 4 ? CapComSkins.toggleOnButton : CapComSkins.toggleOffButton))
			{
				CapCom.Settings.sortMode = 4;
				CapCom.Settings.Save();
				sortContracts();
			}

			GUI.DrawTexture(r, CapComSkins.flagTex);

			GUI.DrawTexture(r, CapComSkins.currentFlag);

			r.x += 72;
			r.width = 50;
			r.height = 38;
			t.x += 73;

			if (GUI.Button(t, new GUIContent("", "Sort By Target Body"), CapCom.Settings.sortMode == 5 ? CapComSkins.toggleOnButton : CapComSkins.toggleOffButton))
			{
				CapCom.Settings.sortMode = 5;
				CapCom.Settings.Save();
				sortContracts();
			}

			GUI.DrawTexture(r, CapComSkins.sortPlanet);

			r.x = 326;
			r.y = 26;
			r.width = 32;
			r.height = 26;

			if (GUI.Button(r, new GUIContent(CapCom.Settings.ascending ? CapComSkins.orderAsc : CapComSkins.orderDesc, "Ascending/Descending Order"), CapComSkins.textureButton))
			{
				CapCom.Settings.ascending = !CapCom.Settings.ascending;
				CapCom.Settings.Save();
				sortContracts();
			}
		}

		private void contractTabs(int id)
		{
			GUILayout.BeginHorizontal();
				if (dropdown)
				{
					GUILayout.Label("Offered", listStyle(0), GUILayout.Width(106), GUILayout.Height(22));
					GUILayout.Label("Active", listStyle(1), GUILayout.Width(106), GUILayout.Height(22));
					GUILayout.Label("Completed", listStyle(2), GUILayout.Width(106), GUILayout.Height(22));
				}
				else
				{
					if (GUILayout.Button("Offered", listStyle(0), GUILayout.Width(106), GUILayout.Height(22)))
					{
						if (currentList != 0)
						{
							currentList = 0;
							sortContracts();
						}
					}

					if (GUILayout.Button("Active", listStyle(1), GUILayout.Width(106), GUILayout.Height(22)))
					{
						if (currentList != 1)
						{
							currentList = 1;
							sortContracts();
						}
					}

					if (GUILayout.Button("Completed", listStyle(2), GUILayout.Width(106), GUILayout.Height(22)))
					{
						if (currentList != 2)
						{
							currentList = 2;
							sortContracts();
						}
					}
				}
			GUILayout.EndHorizontal();
		}

		private void contractList(int id)
		{
			cScroll = GUILayout.BeginScrollView(cScroll, false, true, GUILayout.Width(356));

			for (int i = 0; i < currentContracts.Count; i++)
			{
				GUILayout.BeginHorizontal();
					GUILayout.Space(75);
					if (dropdown)
					{
						GUILayout.Label(currentContracts[i].Name, CapComSkins.titleButtonBehind, GUILayout.Width(260), GUILayout.Height(46));
					}
					else
					{
						if (GUILayout.Button(currentContracts[i].Name, GUILayout.Width(260), GUILayout.Height(46)))
						{
							currentContract = currentContracts[i];
							contractIndex = i;
							showAgency = false;
						}
					}
					Rect r = GUILayoutUtility.GetLastRect();
				GUILayout.EndHorizontal();
				GUILayout.Space(-4);

				r.width = 69;
				r.x -= 69;
				r.height = 46;
				if (!dropdown)
				{
					if (GUI.Button(r, "", CapComSkins.iconButton))
					{
						currentContract = currentContracts[i];
						contractIndex = i;
						showAgency = false;
					}
				}
				GUI.DrawTexture(r, contractIndex == i ? CapComSkins.titleButtonActiveLeft : CapComSkins.titleButtonNormalLeft);
				r.x += 10;
				r.y += 3;
				r.width = 58;
				r.height = 40;
				GUI.DrawTexture(r, currentContracts[i].RootAgent.LogoScaled);
				r.width = 11;
				r.height = 40;
				r.x += 307;
				GUI.DrawTexture(r, contractPrestigeIcon(currentContracts[i].Root.Prestige));
			}
			GUILayout.EndScrollView();
		}

		private void currentContractControls(int id)
		{
			if (currentContract == null)
				return;

			if (currentContract.Root.ContractState == Contract.State.Offered)
			{
				GUI.Label(new Rect(550, 15, 100, 20), "Offered", CapComSkins.headerText);

				Rect r = new Rect(WindowRect.width - 60, 35, 44, 44);

				bool active = !CapCom.Settings.activeLimit || ContractSystem.Instance.GetActiveContractCount() < maxContracts;

				if (GUI.Button(r, new GUIContent("", "Accept"), active ? CapComSkins.acceptButton : CapComSkins.acceptButtonGreyed))
				{
					if (active)
						currentContract.Root.Accept();
				}

				r.y += 65;

				if (GUI.Button(r, new GUIContent("", "Decline"), CapCom.Settings.forceDecline || currentContract.CanBeDeclined ? CapComSkins.declineButton : CapComSkins.declineButtonGreyed))
				{
					if (CapCom.Settings.forceDecline || currentContract.CanBeDeclined)
					{
						if (CapCom.Settings.showDeclineWarning)
						{
							dropdown = true;
							warnDecline = true;
						}
						else
							currentContract.Root.Decline();
					}
				}
			}
			else if (currentContract.Root.ContractState == Contract.State.Active)
			{
				GUI.Label(new Rect(550, 15, 100, 20), "Active", CapComSkins.headerText);

				Rect r = new Rect(WindowRect.width - 60, 100, 44, 44);

				if (GUI.Button(r, new GUIContent("", "Cancel"), CapCom.Settings.forceCancel || currentContract.CanBeCancelled ? CapComSkins.cancelButton : CapComSkins.cancelButtonGreyed))
				{
					if (currentContract.CanBeCancelled)
					{
						if (CapCom.Settings.showCancelWarning)
						{
							dropdown = true;
							warnCancel = true;
						}
						else
							currentContract.Root.Cancel();
					}
				}
			}
			else
			{
				GUI.Label(new Rect(550, 15, 100, 20), "Completed", CapComSkins.headerText);
			}
		}

		private void currentContractHeader(int id)
		{
			if (currentContract == null)
				return;

			Rect r = new Rect(360, 33, 160, 106);

			if (GUI.Button(r, "", CapComSkins.iconButton))
			{
				showAgency = !showAgency;
				agentOfferedContracts.Clear();
				agentActiveContracts.Clear();
				foreach (CapComContract c in offeredContracts)
				{
					if (c.RootAgent == currentContract.RootAgent)
					{
						if (c == currentContract)
							continue;

						agentOfferedContracts.Add(c);
					}
				}
				foreach (CapComContract c in activeContracts)
				{
					if (c.RootAgent == currentContract.RootAgent)
					{
						if (c == currentContract)
							continue;

						agentActiveContracts.Add(c);
					}
				}
			}
			GUI.DrawTexture(r, CapComSkins.flagTex);
			r.x += 4;
			r.y += 5;
			r.width = 152;
			r.height = 95;
			GUI.DrawTexture(r, currentContract.RootAgent.Logo);

			GUILayout.BeginHorizontal();
				GUILayout.Space(170);
				GUILayout.BeginVertical();
					GUILayout.Label("Contract:", CapComSkins.headerText, GUILayout.Width(80));
					r = GUILayoutUtility.GetLastRect();
					r.x += 100;
					r.width = 60;
					r.height = 16;
					GUI.DrawTexture(r, contractPrestigeIcon(currentContract.Root.Prestige, false));
					GUILayout.Label(currentContract.Name, CapComSkins.titleText, GUILayout.Width(270));
					GUILayout.Label("Agent:", CapComSkins.headerText, GUILayout.Width(80));
					GUILayout.Label(currentContract.RootAgent.Name, CapComSkins.titleText, GUILayout.Width(270));
				GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}

		private void currentContractInfo(int id)
		{
			if (currentContract == null)
				return;

			GUILayout.Space(10);

			infoScroll = GUILayout.BeginScrollView(infoScroll, GUILayout.Width(500));

			GUILayout.Label("Briefing:", CapComSkins.headerText, GUILayout.Width(80));

			if (!CapCom.Settings.hideBriefing)
			{
				GUILayout.Label(currentContract.Briefing, CapComSkins.briefingText);
			}

			GUILayout.Label(currentContract.Root.Synopsys, CapComSkins.synopsisText);

			if (currentContract.Root.ContractState == Contract.State.Offered)
			{
				GUILayout.BeginHorizontal();
					GUILayout.Label("Offer Expires: ", CapComSkins.headerText, GUILayout.Width(85));
					GUILayout.Label(KSPUtil.PrintDateDeltaCompact((int)(currentContract.Root.DateExpire - Planetarium.fetch.time), true,	true), CapComSkins.timerText, GUILayout.Width(100));
					if (currentContract.Root.DateDeadline != 0)
					{
						GUILayout.Label("Mission Deadline: ", CapComSkins.headerText, GUILayout.Width(115));
						GUILayout.Label(KSPUtil.PrintDateDeltaCompact((int)(currentContract.Root.DateDeadline - Planetarium.fetch.time),	true, true), CapComSkins.timerText, GUILayout.Width(140));
					}
					else if (currentContract.Root.TimeDeadline != 0)
					{
						GUILayout.Label("Mission Duration: ", CapComSkins.headerText, GUILayout.Width(110));
						GUILayout.Label(KSPUtil.PrintDateDeltaCompact((int)currentContract.Root.TimeDeadline, true, true),	CapComSkins.timerText, GUILayout.Width(140));
					}
				GUILayout.EndHorizontal();
			}
			else if (currentContract.Root.ContractState == Contract.State.Active)
			{
				GUILayout.BeginHorizontal();
					if (currentContract.Root.DateDeadline != 0)
					{
						GUILayout.Label("Mission Deadline: ", CapComSkins.headerText, GUILayout.Width(115));
						GUILayout.Label(KSPUtil.PrintDateDeltaCompact((int)(currentContract.Root.DateDeadline - Planetarium.fetch.time),	true, true), CapComSkins.timerText, GUILayout.Width(150));
					}
				GUILayout.EndHorizontal();
			}

			if (!string.IsNullOrEmpty(currentContract.Notes))
			{
				GUILayout.Label("Mission Notes: ", CapComSkins.headerText, GUILayout.Width(100));

				if (CapCom.Settings.hideNotes)
				{
					Rect r = GUILayoutUtility.GetLastRect();
					r.x += 100;
					r.y += 2;
					r.width = 16;
					r.height = 16;
					if (GUI.Button(r, currentContract.ShowNotes ? CapComSkins.notesMinusIcon : CapComSkins.notesPlusIcon, CapComSkins.textureButton))
						currentContract.ShowNotes = !currentContract.ShowNotes;
				}

				if (!CapCom.Settings.hideNotes || currentContract.ShowNotes)
					GUILayout.Label(currentContract.Notes, CapComSkins.noteText);
			}

			if (currentContract.ParameterCount > 0)
			{
				GUILayout.Label("Objectives: ", CapComSkins.headerText, GUILayout.Width(100));

				for (int i = 0; i < currentContract.ParameterCount; i++)
				{
					CapComParameter cp = currentContract.getParameter(i);
					if (cp == null)
						continue;
					if (cp.Level > 4)
						continue;

					if (string.IsNullOrEmpty(cp.Name))
						continue;

					drawParameter(cp);
				}
			}

			GUILayout.Label("Rewards: ", CapComSkins.headerText, GUILayout.Width(80));

			sizedContent(currentContract.FundsAdv, "", "", TransactionReasons.ContractAdvance);

			sizedContent(currentContract.FundsRew, currentContract.SciRew, currentContract.RepRew, TransactionReasons.ContractReward);

			sizedContent(currentContract.FundsPen, "", currentContract.RepPen, TransactionReasons.ContractPenalty);

			GUILayout.EndScrollView();
		}

		private void drawParameter(CapComParameter cp)
		{
			bool notes = !string.IsNullOrEmpty(cp.Notes);

			GUILayout.BeginHorizontal();
				if (notes && CapCom.Settings.hideNotes)
					GUILayout.Space(30 + cp.Level * 8);
				else
					GUILayout.Space(16 + cp.Level * 8);

				GUILayout.BeginVertical();
					GUILayout.Label(cp.Name, cp.Level == 0 ? CapComSkins.parameterText : CapComSkins.subParameterText);
					Rect b = GUILayoutUtility.GetLastRect();

					if (notes && CapCom.Settings.hideNotes)
					{
						b.x -= 28;
						b.height = 16;
						b.width = 16;
						if (GUI.Button(b, cp.ShowNotes ? CapComSkins.notesMinusIcon :CapComSkins.notesPlusIcon, CapComSkins.textureButton))
							cp.ShowNotes = !cp.ShowNotes;

						b.y += 4;
						b.x += 16;
						b.height = 10;
						b.width = 10;
						GUI.DrawTexture(b, parameterStateIcon(cp.Param));
					}
					else
					{
						b.x -= 14;
						b.y += 4;
						b.height = 12;
						b.width = 12;
						GUI.DrawTexture(b, parameterStateIcon(cp.Param));
					}

					if (notes)
					{
						if (!CapCom.Settings.hideNotes || cp.ShowNotes)
							GUILayout.Label(cp.Notes, CapComSkins.noteText);
					}

					sizedContent(cp.FundsRew, cp.SciRew, cp.RepRew, TransactionReasons.ContractReward);

					sizedContent(cp.FundsPen, "", cp.RepPen, TransactionReasons.ContractPenalty);
				GUILayout.EndVertical();
			GUILayout.EndHorizontal();

			for (int j = 0; j < cp.ParameterCount; j++)
			{
				CapComParameter subP = cp.getParameter(j);
				if (subP == null)
					continue;
				if (subP.Level > 4)
					continue;

				if (string.IsNullOrEmpty(subP.Name))
					continue;

				drawParameter(subP);
			}
		}

		private void currentAgentInfo(int id)
		{
			if (currentContract == null)
				return;

			GUILayout.Space(10);

			infoScroll = GUILayout.BeginScrollView(infoScroll, GUILayout.Width(500));

			GUILayout.Label("Agency Description:", CapComSkins.headerText);
			GUILayout.Label(currentContract.RootAgent.Description, CapComSkins.briefingText);

			if (currentContract.RootAgent.Mentality.Count > 0)
			{
				GUILayout.Label("Agency Mentalities:", CapComSkins.headerText);
				foreach (AgentMentality m in currentContract.RootAgent.Mentality)
				{
					GUILayout.Label(m.GetType().Name + ":", CapComSkins.synopsisText);
					if (string.IsNullOrEmpty(m.Description))
						continue;
					GUILayout.BeginHorizontal();
						GUILayout.Space(20);
						GUILayout.Label(m.Description, CapComSkins.briefingText);
					GUILayout.EndHorizontal();
				}
			}

			if (agentOfferedContracts.Count > 0)
			{
				GUILayout.Label("Currently Offered Contracts:", CapComSkins.headerText);
				foreach (CapComContract c in agentOfferedContracts)
				{
					GUILayout.BeginHorizontal();
						GUILayout.Space(20);
						GUILayout.Label(c.Name, CapComSkins.agencyContractText);
					GUILayout.EndHorizontal();
				}
			}

			if (agentActiveContracts.Count > 0)
			{
				GUILayout.Label("Currently Active Contracts:", CapComSkins.headerText);
				foreach (CapComContract c in agentActiveContracts)
				{
					GUILayout.BeginHorizontal();
						GUILayout.Space(20);
						GUILayout.Label(c.Name, CapComSkins.agencyContractText);
					GUILayout.EndHorizontal();
				}
			}

			GUILayout.EndScrollView();
		}

		private void rescaleHandle(int id)
		{
			Rect r = new Rect(WindowRect.width - 24, WindowRect.height - 24, 22, 22);
			GUI.DrawTexture(r, CapComSkins.resizeHandle);

			if (Event.current.type == EventType.mouseDown && Event.current.button == 0)
			{
				if (r.Contains(Event.current.mousePosition))
				{
					resizing = true;
					dragStart = Input.mousePosition.y;
					dH = WindowRect.height;
					Event.current.Use();
				}
			}

			if (resizing)
			{
				if (Input.GetMouseButtonUp(0))
				{
					resizing = false;
					WindowRect.yMax = WindowRect.y + dH;
					CapCom.Settings.windowHeight = WindowRect.height;
					CapCom.Settings.Save();
				}
				else
				{
					float height = Input.mousePosition.y;
					if (Input.mousePosition.y < 0)
						height = 0;
					dH += dragStart - height;
					dragStart = height;
					WindowRect.yMax = WindowRect.y + dH;
					if (WindowRect.yMax > Screen.height)
					{
						WindowRect.yMax = Screen.height;
						dH = WindowRect.yMax - WindowRect.y;
					}
					if (dH < 300)
						dH = 300;
				}
			}
		}

		private void dropDown(int id)
		{
			if (dropdown)
			{
				if (sortMenu)
				{
					ddRect = new Rect(88, 68, 106, 100);
					GUI.Box(ddRect, "");
					Rect r = new Rect(ddRect.x + 5, ddRect.y + 5, 100, 20);
					GUI.Label(r, "Sort Options:", CapComSkins.titleText);
					for (int i = 1; i < 4; i++)
					{
						r.y += 22;
						if (GUI.Button(r, sortName(i), CapComSkins.menuButton))
						{
							CapCom.Settings.sortMode = i;
							CapCom.Settings.Save();
							sortContracts();
							sortMenu = false;
							dropdown = false;
						}
						Rect t = new Rect(r.x + 2, r.y + 2, 16, 16);
						GUI.DrawTexture(t, currencyIcon((Currency)(i - 1)));
					}
				}
				else if (warnCancel)
				{
					ddRect = new Rect(WindowRect.width - 230, 95, 160, 70);
					GUI.Box(ddRect, "");
					Rect r = new Rect(ddRect.x + 10, ddRect.y + 5, 140, 30);
					GUI.Label(r, "Cancel this contract?", CapComSkins.warningText);
					r = new Rect(ddRect.x + 40, ddRect.y + 30, 80, 30);
					if (GUI.Button(r, "Confirm", CapComSkins.warningButton))
					{
						if (currentContract.Root.CanBeCancelled())
							currentContract.Root.Cancel();
						warnCancel = false;
						dropdown = false;
					}
				}
				else if (warnDecline)
				{
					ddRect = new Rect(WindowRect.width - 230, 95, 160, 70);
					GUI.Box(ddRect, "");
					Rect r = new Rect(ddRect.x + 10, ddRect.y + 5, 140, 30);
					GUI.Label(r, "Decline this contract?", CapComSkins.warningText);
					r = new Rect(ddRect.x + 40, ddRect.y + 30, 80, 30);
					if (GUI.Button(r, "Confirm", CapComSkins.warningButton))
					{
						if (currentContract.Root.CanBeDeclined())
							currentContract.Root.Decline();
						warnDecline = false;
						dropdown = false;
					}
				}
				else
					dropdown = false;
			}
		}

		private void sizedContent(string funds, string sci, string rep, TransactionReasons type)
		{
			bool b1 = string.IsNullOrEmpty(funds);
			bool b2 = string.IsNullOrEmpty(sci);
			bool b3 = string.IsNullOrEmpty(rep);

			if (b1 && b2 && b3)
				return;

			Rect r = new Rect();

			rewardLabel(type, ref r);

			GUIStyle s;
			Vector2 sz = new Vector2();
			if (!b1)
			{
				r.width = 16;
				r.height = 16;
				GUI.DrawTexture(r, currencyIcon(Currency.Funds));
				s = currencyStyle(Currency.Funds);
				sz = s.CalcSize(new GUIContent(funds));

				r.x += 20;
				r.width = sz.x;
				r.height = sz.y;

				GUI.Label(r, funds, s);

				r.x += sz.x + 14;
			}

			if (!b2)
			{
				r.width = 16;
				r.height = 16;
				GUI.DrawTexture(r, currencyIcon(Currency.Science));
				s = currencyStyle(Currency.Science);
				sz = s.CalcSize(new GUIContent(sci));

				r.x += 20;
				r.width = sz.x;
				r.height = sz.y;

				GUI.Label(r, sci, s);

				r.x += sz.x + 14;
			}

			if (!b3)
			{
				r.width = 16;
				r.height = 16;
				GUI.DrawTexture(r, currencyIcon(Currency.Reputation));
				s = currencyStyle(Currency.Reputation);
				sz = s.CalcSize(new GUIContent(rep));

				r.x += 20;
				r.width = sz.x;
				r.height = sz.y;

				GUI.Label(r, rep, s);
			}
		}

		private void rewardLabel(TransactionReasons t, ref Rect r)
		{
			float right = 0;
			switch (t)
			{
				case TransactionReasons.ContractAdvance:
					{
						GUILayout.Label("Advance: ", CapComSkins.advance, GUILayout.Width(80));
						right = 62;
						break;
					}
				case TransactionReasons.ContractPenalty:
					{
						GUILayout.Label("Failure: ", CapComSkins.failure, GUILayout.Width(80));
						right = 50;
						break;
					}
				case TransactionReasons.ContractReward:
					{
						GUILayout.Label("Completion: ", CapComSkins.completion, GUILayout.Width(80));
						right = 76;
						break;
					}
				default:
					return;
			}
			r = GUILayoutUtility.GetLastRect();
			r.x += right;
		}

		private GUIStyle listStyle(int i)
		{
			return i == currentList ? CapComSkins.tabButton : CapComSkins.tabButtonInactive;
		}

		private GUIStyle currencyStyle(Currency t)
		{
			switch (t)
			{
				case Currency.Funds:
					return CapComSkins.funds;
				case Currency.Reputation:
					return CapComSkins.rep;
				default:
					return CapComSkins.sci;
			}
		}

		private Texture2D currencyIcon(Currency t)
		{
			switch (t)
			{
				case Currency.Funds:
					return CapComSkins.fundsGreen;
				case Currency.Reputation:
					return CapComSkins.repRed;
				default:
					return CapComSkins.science;
			}
		}

		private Texture2D contractPrestigeIcon(Contract.ContractPrestige p, bool vertical = true)
		{
			switch (p)
			{
				case Contract.ContractPrestige.Trivial:
					return vertical ? CapComSkins.goldStarVertical : CapComSkins.goldStar;
				case Contract.ContractPrestige.Significant:
					return vertical ? CapComSkins.goldStarTwoVertical : CapComSkins.goldStarTwo;
				default:
					return vertical ? CapComSkins.goldStarThreeVertical : CapComSkins.goldStarThree;
			}
		}

		private Texture2D parameterStateIcon(ContractParameter p)
		{
			switch (p.State)
			{
				case ParameterState.Complete:
					return CapComSkins.checkBox;
				case ParameterState.Incomplete:
					return CapComSkins.emptyBox;
				default:
					return CapComSkins.failBox;
			}
		}

		private string sortName(int i)
		{
			switch (i)
			{
				case 0:
					return "Difficulty";
				case 1:
					return "Funds";
				case 2:
					return "Science";
				case 3:
					return "Reputation";
				case 4:
					return "Agency";
				case 5:
					return "Target Body";
				default:
					return "";
			}
		}

	}
}
