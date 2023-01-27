using HarmonyLib;
using JetBrains.Annotations;
using Verse;

namespace CombatExtended.ExtendedLoadout;

[HarmonyPatch(typeof(JobGiver_UpdateLoadout), "FindPickup")]
[HotSwappable]
public class JobGiver_UpdateLoadout_FindPickup_RefillThreshold_Patch
{
	private static bool Prepare()
	{
		return ExtendedLoadoutMod.Instance.useHpAndQualityInLoadouts;
	}

	[HarmonyPrefix]
	[UsedImplicitly]
	public static bool FindPickup(Pawn pawn, LoadoutSlot curSlot, int findCount, ref JobGiver_UpdateLoadout.ItemPriority curPriority, ref Thing curThing, ref Pawn curCarrier)
	{
		Loadout loadout = pawn.GetLoadout();
		if (loadout == null)
		{
			return true;
		}
		if (loadout is Loadout_Multi loadout_Multi)
		{
			loadout = loadout_Multi.FindLoadoutWithSlot(curSlot);
			if (loadout == null)
			{
				return true;
			}
		}
		float refillThreshold = loadout.Extended().RefillThreshold;
		float num = (float)curSlot.count - (float)curSlot.count * refillThreshold;
		if ((float)findCount < num)
		{
			curPriority = JobGiver_UpdateLoadout.ItemPriority.None;
			curThing = null;
			curCarrier = null;
			return false;
		}
		return true;
	}
}
