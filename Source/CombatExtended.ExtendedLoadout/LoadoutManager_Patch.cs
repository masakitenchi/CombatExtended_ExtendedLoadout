using HarmonyLib;

namespace CombatExtended.ExtendedLoadout;

[HarmonyPatch(typeof(LoadoutManager))]
public static class LoadoutManager_Patch
{
	private static bool Prepare()
	{
		return ExtendedLoadoutMod.Instance.useMultiLoadouts;
	}

	[HarmonyPatch("RemoveLoadout")]
	[HarmonyPostfix]
	public static void RemoveLoadout(Loadout loadout)
	{
		LoadoutMulti_Manager.RemoveLoadout(loadout);
	}

	[HarmonyPatch("ExposeData")]
	[HarmonyPostfix]
	public static void ExposeData(LoadoutManager __instance)
	{
		LoadoutMulti_Manager.ExposeData(__instance);
	}
}
