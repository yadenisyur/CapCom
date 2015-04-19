﻿using System;
using System.Collections.Generic;
using Contracts;
using CapCom.Framework;
using FinePrint;
using FinePrint.Contracts.Parameters;
using System.Reflection;

namespace CapCom
{
	public class CapComParameter
	{
		private string name;
		private CapComContract root;
		private ContractParameter param;
		private string notes;
		private int level;
		private Waypoint waypoint;
		private string fundsRew, fundsPen, repRew, repPen, sciRew;
		private float fundsRewStrat, fundsPenStrat, repRewStrat, repPenStrat, sciRewStrat;
		private List<CapComParameter> parameters = new List<CapComParameter>();

		public CapComParameter(CapComContract r, ContractParameter p, int l)
		{
			root = r;
			param = p;
			name = param.Title;
			notes = param.Notes;
			level = l;

			if (level < 4)
			{
				for (int i = 0; i < param.ParameterCount; i++)
				{
					ContractParameter cp = param.GetParameter(i);
					if (cp == null)
						continue;

					addToParams(cp, level + 1);
				}
			}

			paramRewards();
			paramPenalties();

			checkForWaypoints();
		}

		private void checkForWaypoints()
		{
			waypoint = null;

			CC_MBE.LogFormatted_DebugOnly("Checking Parameter For FinePrint Waypoints");

			if (param.GetType() != typeof(SurveyWaypointParameter))
				return;

			SurveyWaypointParameter s = (SurveyWaypointParameter)param;
			if (s.State != ParameterState.Incomplete)
				return;

			CC_MBE.LogFormatted_DebugOnly("Parameter Of Correct Waypoint Type");

			try
			{
				var field = (typeof(SurveyWaypointParameter)).GetFields(BindingFlags.NonPublic | BindingFlags.Instance)[0];
				waypoint = (Waypoint)field.GetValue(s);
				CC_MBE.LogFormatted_DebugOnly("Waypoint Assigned");
			}
			catch (Exception e)
			{
				waypoint = null;
				CC_MBE.LogFormatted("Error While Assigning FinePrint Waypoint Object... {0}", e);
			}
		}

		private void addToParams(ContractParameter p, int Level)
		{
			CapComParameter cc = new CapComParameter(root, p, Level);
			parameters.Add(cc);
			root.addToParams(cc);
		}

		private void paramRewards()
		{
			CurrencyModifierQuery currencyQuery = CurrencyModifierQuery.RunQuery(TransactionReasons.ContractReward, (float)param.FundsCompletion, param.ScienceCompletion, param.ReputationCompletion);
			fundsRew = "+ " + param.FundsCompletion.ToString("N0");
			fundsRewStrat = currencyQuery.GetEffectDelta(Currency.Funds);
			if (fundsRewStrat != 0f)
			{
				fundsRew = string.Format("+ {0:N0} ({1:N0})", param.FundsCompletion + fundsRewStrat, fundsRewStrat);
			}
			repRew = "+ " + param.ReputationCompletion.ToString("N0");
			repRewStrat = currencyQuery.GetEffectDelta(Currency.Reputation);
			if (repRewStrat != 0f)
			{
				repRew = string.Format("+ {0:N0} ({1:N0})", param.ReputationCompletion + repRewStrat, repRewStrat);
			}
			sciRew = "+ " + param.ScienceCompletion.ToString("N0");
			sciRewStrat = currencyQuery.GetEffectDelta(Currency.Science);
			if (sciRewStrat != 0f)
			{
				sciRew = string.Format("+ {0:N0} ({1:N0})", param.ScienceCompletion + sciRewStrat, sciRewStrat);
			}
		}

		private void paramPenalties()
		{
			CurrencyModifierQuery currencyQuery = CurrencyModifierQuery.RunQuery(TransactionReasons.ContractPenalty, (float)param.FundsFailure, 0f, param.ReputationFailure);
			fundsPen = "- " + param.FundsFailure.ToString("N0");
			fundsPenStrat = currencyQuery.GetEffectDelta(Currency.Funds);
			if (fundsPenStrat != 0f)
			{
				fundsPen = string.Format("- {0:N0} ({1:N0})", param.FundsFailure + fundsPenStrat, fundsPenStrat);
			}
			repPen = "- " + param.ReputationFailure.ToString("N0");
			repPenStrat = currencyQuery.GetEffectDelta(Currency.Reputation);
			if (repPenStrat != 0f)
			{
				repPen = string.Format("- {0:N0} ({1:N0})", param.ReputationFailure + repPenStrat, repPenStrat);
			}
		}

		public CapComParameter getParameter(int i)
		{
			if (parameters.Count >= i)
				return parameters[i];
			else
				CC_MBE.LogFormatted("CapCom Sub Parameter List Index Out Of Range; Something Went Wrong Here...");

			return null;
		}

		public string Name
		{
			get { return name; }
		}

		public string Notes
		{
			get { return notes; }
		}

		public int Level
		{
			get { return level; }
		}

		public Waypoint Way
		{
			get { return waypoint; }
		}

		public ContractParameter Param
		{
			get { return param; }
		}

		public int ParameterCount
		{
			get { return parameters.Count; }
		}

		public string FundsRew
		{
			get { return fundsRew; }
		}

		public string FundsPen
		{
			get { return fundsPen; }
		}

		public string RepRew
		{
			get { return repRew; }
		}

		public string RepPen
		{
			get { return repPen; }
		}

		public string SciRew
		{
			get { return sciRew; }
		}

		public float FundsRewStrat
		{
			get { return fundsRewStrat; }
		}

		public float FundsPenStrat
		{
			get { return fundsPenStrat; }
		}

		public float RepRewStrat
		{
			get { return repRewStrat; }
		}

		public float RepPenStrat
		{
			get { return repPenStrat; }
		}

		public float SciRewStrat
		{
			get { return sciRewStrat; }
		}
	}
}
