using System.Collections.Generic;
using HarmonyLib;

namespace CombatExtended.ExtendedLoadout;

public static class LoadoutProxy_Patch
{
	private static bool Prepare()
	{
		return ExtendedLoadoutMod.Instance.useMultiLoadouts;
	}

	public static void Patch()
	{
		ExtendedLoadoutMod.Harmony.Patch(AccessTools.PropertyGetter(typeof(Loadout), "SlotCount"), new HarmonyMethod(typeof(LoadoutProxy_Patch), "SlotCount"));
		ExtendedLoadoutMod.Harmony.Patch(AccessTools.PropertyGetter(typeof(Loadout), "Slots"), new HarmonyMethod(typeof(LoadoutProxy_Patch), "Slots"));
	}

	public static void Unpatch()
	{
		ExtendedLoadoutMod.Harmony.Unpatch(AccessTools.PropertyGetter(typeof(Loadout), "SlotCount"), HarmonyPatchType.Prefix, ExtendedLoadoutMod.HarmonyId);
		ExtendedLoadoutMod.Harmony.Unpatch(AccessTools.PropertyGetter(typeof(Loadout), "Slots"), HarmonyPatchType.Prefix, ExtendedLoadoutMod.HarmonyId);
	}

	[HarmonyPrefix]
	[HarmonyPatch("SlotCount", MethodType.Getter)]
	public static bool SlotCount(Loadout __instance, ref int __result)
	{
		if (__instance is Loadout_Multi loadout_Multi)
		{
			__result = loadout_Multi.SlotCount;
			return false;
		}
		return true;
	}

	[HarmonyPrefix]
	[HarmonyPatch("Slots", MethodType.Getter)]
	public static bool Slots(Loadout __instance, ref List<LoadoutSlot> __result)
	{
		if (__instance is Loadout_Multi loadout_Multi)
		{
			__result = loadout_Multi.Slots;
			return false;
		}
		return true;
	}
}
