using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace CombatExtended.ExtendedLoadout;

public static class LoadoutMulti_Manager
{
	private static List<Pawn> keysWorkingList = null;

	private static List<Loadout_Multi> valuesWorkingList = null;

	private static Dictionary<Pawn, Loadout_Multi> assignedLoadoutsMulti = new Dictionary<Pawn, Loadout_Multi>();

	public static IEnumerable<Loadout_Multi> LoadoutsMulti => assignedLoadoutsMulti.Values;

	public static void ExposeData(LoadoutManager __instance)
	{
		Scribe_Collections.Look<Pawn, Loadout_Multi>(ref assignedLoadoutsMulti, "assignedLoadoutsMulti", LookMode.Reference, LookMode.Deep, ref keysWorkingList, ref valuesWorkingList);
		if (Scribe.mode != LoadSaveMode.PostLoadInit || assignedLoadoutsMulti != null)
		{
			return;
		}
		assignedLoadoutsMulti = new Dictionary<Pawn, Loadout_Multi>();
		Dictionary<Pawn, Loadout> assignedLoadouts = __instance._assignedLoadouts;
		if (assignedLoadouts == null || !assignedLoadouts.Any())
		{
			return;
		}
		foreach (KeyValuePair<Pawn, Loadout> assignedLoadout in __instance._assignedLoadouts)
		{
			assignedLoadout.Key.SetLoadout(assignedLoadout.Value, 0);
		}
	}

	public static void RemoveLoadout(Loadout loadout)
	{
		foreach (Loadout_Multi value in assignedLoadoutsMulti.Values)
		{
			List<Loadout> loadouts = value.Loadouts;
			for (int i = 0; i < loadouts.Count; i++)
			{
				if (loadouts[i] == loadout)
				{
					loadouts[i] = LoadoutManager.DefaultLoadout;
				}
			}
		}
	}

	[ClearDataOnNewGame]
	public static void ClearData()
	{
		assignedLoadoutsMulti.Clear();
	}

	public static int GetUniqueLoadoutID()
	{
		Dictionary<Pawn, Loadout_Multi>.ValueCollection values = assignedLoadoutsMulti.Values;
		if (!values.Any())
		{
			return 1;
		}
		return values.Max((Loadout_Multi l) => l.uniqueID) + 1;
	}

	public static Loadout GetLoadout(Pawn pawn)
	{
		if (!assignedLoadoutsMulti.TryGetValue(pawn, out var value))
		{
			value = new Loadout_Multi(pawn);
			assignedLoadoutsMulti.Add(pawn, value);
		}
		if (value.PersonalLoadout == null)
		{
			value.GeneratePersonalLoadout(pawn);
		}
		return value;
	}

	public static void SetLoadout(this Pawn pawn, Loadout loadout, int index)
	{
		if (pawn == null)
		{
			throw new ArgumentNullException("pawn");
		}
		if (assignedLoadoutsMulti.ContainsKey(pawn))
		{
			assignedLoadoutsMulti[pawn][index] = loadout;
			return;
		}
		assignedLoadoutsMulti.Add(pawn, new Loadout_Multi(pawn) { [index] = loadout });
	}
}
