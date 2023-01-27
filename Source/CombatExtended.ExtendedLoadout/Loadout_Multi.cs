using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace CombatExtended.ExtendedLoadout;

public class Loadout_Multi : Loadout, IExposable, ILoadReferenceable
{
	private static int? _columnsCount;

	private Pawn? _pawn;

	private Loadout? _personalLoadout;

	public int uniqueID;

	private List<Loadout>? _loadouts;

	public static int ColumnsCount
	{
		get
		{
			return _columnsCount ?? throw new Exception("ColumnsCount not initialized!");
		}
		set
		{
			if (_columnsCount.HasValue)
			{
				throw new Exception("ColumnsCount can be setted one time!");
			}
			_columnsCount = value;
		}
	}

	public Loadout? PersonalLoadout => _personalLoadout;

	public new int SlotCount => Slots.Count;

	public new List<LoadoutSlot> Slots { get; private set; } = new List<LoadoutSlot>();


	public List<Loadout> Loadouts => _loadouts;

	public Loadout this[int index]
	{
		get
		{
			return _loadouts[index];
		}
		set
		{
			_loadouts[index] = value;
			NotifyLoadoutChanged();
		}
	}

	public Loadout_Multi()
	{
	}

	public Loadout_Multi(Pawn pawn)
	{
		GeneratePersonalLoadout(pawn);
		_loadouts = Enumerable.Repeat(LoadoutManager.DefaultLoadout, ColumnsCount).ToList();
		NotifyLoadoutChanged();
		uniqueID = LoadoutMulti_Manager.GetUniqueLoadoutID();
	}

	public void GeneratePersonalLoadout(Pawn pawn)
	{
		if (pawn.RaceProps.Humanlike)
		{
			_pawn = pawn;
			Loadout loadout = new Loadout(pawn.Name.ToStringShort);
			loadout.defaultLoadout = false;
			loadout.canBeDeleted = true;
			_personalLoadout = loadout;
		}
	}

	public void NotifyLoadoutChanged()
	{
		Slots = (from x in _loadouts.Prepend<Loadout>(PersonalLoadout)
			where x != null
			select x).SelectMany((Loadout x) => x.Slots).ToList();
	}

	public new string GetUniqueLoadID()
	{
		return "LoadoutMulti_" + uniqueID;
	}

	public new void ExposeData()
	{
		Scribe_Values.Look(ref uniqueID, "uniqueID", 0);
		Scribe_References.Look(ref _pawn, "pawn");
		Scribe_Deep.Look(ref _personalLoadout, "personalLoadout");
		Scribe_Collections.Look(ref _loadouts, "loadouts", LookMode.Reference);
		if (Scribe.mode != LoadSaveMode.PostLoadInit)
		{
			return;
		}
		int num = ColumnsCount - _loadouts.Count;
		if (num > 0)
		{
			Log.Warning($"[Loadout_Multi] Fix loadouts list. Count difference: {num}");
			for (int i = 0; i < num; i++)
			{
				_loadouts.Add(LoadoutManager.DefaultLoadout);
			}
		}
		else if (num < 0)
		{
			Log.Warning($"[Loadout_Multi] Fix loadouts list. Count difference: {num}");
			for (int j = 0; j < Math.Abs(num); j++)
			{
				_loadouts.RemoveAt(_loadouts.Count - 1);
			}
		}
		for (int k = 0; k < _loadouts.Count; k++)
		{
			if (_loadouts[k] == null)
			{
				Log.Warning($"[Loadout_Multi] Fix removed loadout id: {uniqueID}");
				_loadouts[k] = LoadoutManager.DefaultLoadout;
			}
		}
		_ = _pawn;
		_ = _personalLoadout;
		NotifyLoadoutChanged();
	}

	public Loadout? FindLoadoutWithThingDef(ThingDef t)
	{
		foreach (Loadout loadout in _loadouts)
		{
			foreach (LoadoutSlot slot in loadout.Slots)
			{
				if (slot.thingDef == t)
				{
					return loadout;
				}
			}
		}
		if (_personalLoadout != null)
		{
			foreach (LoadoutSlot slot2 in _personalLoadout!.Slots)
			{
				if (slot2.thingDef == t)
				{
					return _personalLoadout;
				}
			}
		}
		return null;
	}

	public Loadout? FindLoadoutWithSlot(LoadoutSlot targetSlot)
	{
		foreach (Loadout loadout in _loadouts)
		{
			foreach (LoadoutSlot slot in loadout.Slots)
			{
				if (slot == targetSlot)
				{
					return loadout;
				}
			}
		}
		if (_personalLoadout != null)
		{
			foreach (LoadoutSlot slot2 in _personalLoadout!.Slots)
			{
				if (slot2 == targetSlot)
				{
					return _personalLoadout;
				}
			}
		}
		return null;
	}
}
