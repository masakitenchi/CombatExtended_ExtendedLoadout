using System;
using HarmonyLib;

namespace CombatExtended.ExtendedLoadout;

[HarmonyPatch(typeof(Loadout))]
public static class Loadout_Patch
{
	private static bool Prepare()
	{
		return ExtendedLoadoutMod.Instance.useHpAndQualityInLoadouts;
	}

	[HarmonyPostfix]
	[HarmonyPatch("Copy", new Type[] { typeof(Loadout) })]
	private static void Copy(Loadout source, Loadout __result)
	{
		__result.CopyLoadoutExtended(source);
	}

	[HarmonyPostfix]
	[HarmonyPatch("ExposeData")]
	private static void ExposeData(Loadout __instance)
	{
		__instance.Extended().ExposeData();
	}
}
