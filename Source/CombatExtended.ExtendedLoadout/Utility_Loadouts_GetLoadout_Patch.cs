using HarmonyLib;
using Verse;

namespace CombatExtended.ExtendedLoadout;

[HarmonyPatch(typeof(Utility_Loadouts))]
public static class Utility_Loadouts_GetLoadout_Patch
{
	private static bool Prepare()
	{
		return ExtendedLoadoutMod.Instance.useMultiLoadouts;
	}

	[HarmonyPatch("GetLoadout")]
	[HarmonyPrefix]
	public static bool GetLoadout(Pawn pawn, ref Loadout __result)
	{
		__result = LoadoutMulti_Manager.GetLoadout(pawn);
		return false;
	}
}
