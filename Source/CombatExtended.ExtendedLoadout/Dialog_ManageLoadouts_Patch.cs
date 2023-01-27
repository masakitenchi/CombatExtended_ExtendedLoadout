using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace CombatExtended.ExtendedLoadout;

[HarmonyPatch]
public class Dialog_ManageLoadouts_Patch
{
	[HarmonyTargetMethod]
	public static IEnumerable<MethodBase> TargetMethods()
	{
		yield return AccessTools.Method(typeof(ITab_Inventory), "SyncedAddLoadout",new Type[]
		{
			typeof(Pawn)
		});
		yield return AccessTools.Method(typeof(PawnColumnWorker_Loadout), "DoHeader");
	}

	[HarmonyTranspiler]
	public static IEnumerable<CodeInstruction> CtorReplacer_Transpiler(MethodBase __originalMethod, IEnumerable<CodeInstruction> instructions)
	{
		ConstructorInfo dialog_ManageLoadoutsCtor = AccessTools.Constructor(typeof(Dialog_ManageLoadouts), new Type[1] { typeof(Loadout) });
		ConstructorInfo dialog_ManageLoadouts_ExtendedCtor = AccessTools.Constructor(typeof(Dialog_ManageLoadouts_Extended), new Type[1] { typeof(Loadout) });
		int i = 0;
		foreach (CodeInstruction instruction in instructions)
		{
			if (instruction.opcode == OpCodes.Newobj && instruction.operand as ConstructorInfo == dialog_ManageLoadoutsCtor)
			{
				yield return new CodeInstruction(OpCodes.Newobj, dialog_ManageLoadouts_ExtendedCtor);
				i++;
			}
			else
			{
				yield return instruction;
			}
		}
		if (i == 0)
		{
			Log.Error("Can't find Dialog_ManageLoadouts Constructor in method: " + __originalMethod.DeclaringType.FullName + ":" + __originalMethod.Name);
		}
	}
}
