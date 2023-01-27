using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace CombatExtended.ExtendedLoadout;

[HarmonyPatch(typeof(Dialog_ManageLoadouts))]
[HotSwappable]
public static class HideButtons_PersonalLoadout_Patch
{
	private static bool Prepare()
	{
		return ExtendedLoadoutMod.Instance.useMultiLoadouts;
	}

	public static float Height30(Dialog_ManageLoadouts instance)
	{
		if (instance is Dialog_ManageLoadouts_Extended dialog_ManageLoadouts_Extended && dialog_ManageLoadouts_Extended.IsPersonalLoadout)
		{
			return 0f;
		}
		return 30f;
	}

	public static float Y42(Dialog_ManageLoadouts instance)
	{
		if (instance is Dialog_ManageLoadouts_Extended dialog_ManageLoadouts_Extended && dialog_ManageLoadouts_Extended.IsPersonalLoadout)
		{
			return 0f;
		}
		return 42f;
	}

	public static void DrawNameFieldNew(Dialog_ManageLoadouts instance, Rect canvas)
	{
		if (!(instance is Dialog_ManageLoadouts_Extended dialog_ManageLoadouts_Extended) || !dialog_ManageLoadouts_Extended.IsPersonalLoadout)
		{
			instance.DrawNameField(canvas);
		}
	}

	[HarmonyPatch("DoWindowContents")]
	[HarmonyTranspiler]
	public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
	{
		MethodInfo height30 = AccessTools.Method(typeof(HideButtons_PersonalLoadout_Patch), "Height30");
		MethodInfo y42 = AccessTools.Method(typeof(HideButtons_PersonalLoadout_Patch), "Y42");
		MethodInfo sortLoadouts = AccessTools.Method(typeof(LoadoutManager), "SortLoadouts");
		MethodInfo drawNameField = AccessTools.Method(typeof(Dialog_ManageLoadouts), "DrawNameField");
		MethodInfo drawNameFieldNew = AccessTools.Method(typeof(HideButtons_PersonalLoadout_Patch), "DrawNameFieldNew");
		bool end = false;
		bool buttonHeightPatched = false;
		bool yTopPatched = false;
		foreach (CodeInstruction instruction in instructions)
		{
			if (!end && instruction.opcode == OpCodes.Ldc_R4 && (float)instruction.operand == 30f)
			{
				yield return new CodeInstruction(OpCodes.Ldarg_0);
				yield return new CodeInstruction(OpCodes.Call, height30);
				buttonHeightPatched = true;
			}
			else if (!end && instruction.opcode == OpCodes.Ldc_R4 && (float)instruction.operand == 42f)
			{
				yield return new CodeInstruction(OpCodes.Ldarg_0);
				yield return new CodeInstruction(OpCodes.Call, y42);
				yTopPatched = true;
			}
			else if (!end && instruction.Calls(sortLoadouts))
			{
				end = true;
				yield return instruction;
			}
			else if (instruction.Calls(drawNameField))
			{
				yield return new CodeInstruction(OpCodes.Call, drawNameFieldNew);
			}
			else
			{
				yield return instruction;
			}
		}
		if (!buttonHeightPatched)
		{
			Log.Error("buttonHeightPatched false!");
		}
		if (!yTopPatched)
		{
			Log.Error("yTopPatched false!");
		}
	}
}
