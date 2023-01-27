using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace CombatExtended.ExtendedLoadout;

[HarmonyPatch(typeof(Utility_HoldTracker))]
public class Utility_HoldTracker_Patch
{
	private static bool Prepare()
	{
		return ExtendedLoadoutMod.Instance.useHpAndQualityInLoadouts;
	}

	[HarmonyPatch("GetExcessEquipment")]
	[HarmonyPostfix]
	[UsedImplicitly]
	public static void Utility_HoldTracker_GetExcessEquipment(Pawn pawn, ref ThingWithComps dropEquipment, ref bool __result)
	{
		if (__result)
		{
			return;
		}
		Loadout loadout = pawn.GetLoadout();
		ThingWithComps thingWithComps = pawn.equipment?.Primary;
		if (loadout == null || loadout.Slots.NullOrEmpty() || thingWithComps == null || !thingWithComps.def.IsWeapon)
		{
			return;
		}
		if (loadout is Loadout_Multi loadout_Multi)
		{
			loadout = loadout_Multi.FindLoadoutWithThingDef(thingWithComps.def);
			if (loadout == null)
			{
				return;
			}
		}
		if (!loadout.Extended().Allows(thingWithComps))
		{
			dropEquipment = thingWithComps;
			__result = true;
		}
	}

	[HarmonyPatch("GetExcessThing")]
	[HarmonyPostfix]
	[UsedImplicitly]
	public static void Utility_HoldTracker_GetExcessThing(Pawn pawn, ref Thing dropThing, ref int dropCount, ref bool __result)
	{
		if (__result)
		{
			return;
		}
		Loadout loadout = pawn.GetLoadout();
		if (pawn.inventory?.innerContainer == null || loadout == null || loadout.Slots.NullOrEmpty())
		{
			return;
		}
		Loadout_Extended loadout_Extended;
		if (loadout is Loadout_Multi loadout_Multi)
		{
			{
				foreach (Thing item in pawn.inventory.innerContainer)
				{
					Thing innerIfMinified = item.GetInnerIfMinified();
					if (!innerIfMinified.def.IsWeapon)
					{
						continue;
					}
					loadout = loadout_Multi.FindLoadoutWithThingDef(innerIfMinified.def);
					if (loadout != null)
					{
						loadout_Extended = loadout.Extended();
						if (!loadout_Extended.Allows(innerIfMinified))
						{
							dropThing = innerIfMinified;
							dropCount = 1;
							__result = true;
							break;
						}
					}
				}
				return;
			}
		}
		loadout_Extended = loadout.Extended();
		foreach (Thing item2 in pawn.inventory.innerContainer)
		{
			Thing innerIfMinified2 = item2.GetInnerIfMinified();
			if (innerIfMinified2.def.IsWeapon && !loadout_Extended.Allows(innerIfMinified2))
			{
				dropThing = innerIfMinified2;
				dropCount = 1;
				__result = true;
				break;
			}
		}
	}
}
