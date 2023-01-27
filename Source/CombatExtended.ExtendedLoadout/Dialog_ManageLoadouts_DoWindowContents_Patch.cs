using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using Verse;

namespace CombatExtended.ExtendedLoadout;

[HarmonyPatch(typeof(Dialog_ManageLoadouts), "DoWindowContents")]
[HotSwappable]
public class Dialog_ManageLoadouts_DoWindowContents_Patch
{
	private static bool Prepare()
	{
		return ExtendedLoadoutMod.Instance.useHpAndQualityInLoadouts;
	}

	[HarmonyTranspiler]
	[UsedImplicitly]
	public static IEnumerable<CodeInstruction> DoWindowContents_Transpiler(IEnumerable<CodeInstruction> instructions)
	{
		MethodInfo drawSlotList = AccessTools.Method(typeof(Dialog_ManageLoadouts), "DrawSlotList");
		bool heightFixed = false;
		bool drawHpQualityInjected = false;
		foreach (CodeInstruction instruction in instructions)
		{
			if (!heightFixed && instruction.opcode == OpCodes.Ldc_R4 && (float)instruction.operand == 48f)
			{
				instruction.operand = 160f;
				yield return instruction;
				heightFixed = true;
			}
			else if (heightFixed && !drawHpQualityInjected && instruction.opcode == OpCodes.Call && instruction.operand == drawSlotList)
			{
				yield return instruction;
				yield return new CodeInstruction(OpCodes.Ldarg_0);
				yield return new CodeInstruction(OpCodes.Ldloc_S, 10);
				yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Dialog_ManageLoadouts_DoWindowContents_Patch), "DrawHpQuality"));
				drawHpQualityInjected = true;
			}
			else
			{
				yield return instruction;
			}
		}
		if (!drawHpQualityInjected || !heightFixed)
		{
			Log.Error($"drawHpQualityInjected = {drawHpQualityInjected}; heightFixed = {heightFixed}");
		}
	}

	public static void DrawHpQuality(Dialog_ManageLoadouts dialog, Rect bulkBarRect)
	{
		Rect rect = new Rect(bulkBarRect.xMin, bulkBarRect.yMax + 36f, bulkBarRect.width, 24f);
		Rect rect2 = new Rect(rect.xMin, rect.yMax + 6f, rect.width, 24f);
		Rect rect3 = new Rect(rect2.xMin, rect2.yMax + 6f, rect2.width, 24f);
		Loadout_Extended loadout_Extended = dialog.CurrentLoadout.Extended();
		GUI.color = new Color(0.6f, 0.6f, 0.6f);
		loadout_Extended.RefillThreshold = Widgets.HorizontalSlider_NewTemp(rect, loadout_Extended.RefillThreshold, 0f, 1f, middleAlignment: false, "CE_Extended.RefillThreshold".Translate(Mathf.RoundToInt(loadout_Extended.RefillThreshold * 100f)));
		GUI.color = Color.white;
		Widgets.FloatRange(rect2, 976833333, ref loadout_Extended.HpRange, 0f, 1f, "HitPoints", ToStringStyle.PercentZero);
		Widgets.QualityRange(rect3, 976833334, ref loadout_Extended.QualityRange);
	}
}
