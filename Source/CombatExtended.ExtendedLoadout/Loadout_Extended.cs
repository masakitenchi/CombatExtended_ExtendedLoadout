using System.Collections.Generic;
using System.Diagnostics;
using RimWorld;
using UnityEngine;
using Verse;

namespace CombatExtended.ExtendedLoadout;

public class Loadout_Extended
{
	private static readonly Dictionary<Loadout, Loadout_Extended> Loadouts = new Dictionary<Loadout, Loadout_Extended>();

	public float RefillThreshold = 1f;

	public FloatRange HpRange = FloatRange.ZeroToOne;

	public QualityRange QualityRange = QualityRange.All;

	private static int _ticks;

	public static Loadout_Extended Get(Loadout loadout)
	{
		if (!Loadouts.TryGetValue(loadout, out var value))
		{
			value = new Loadout_Extended();
			Loadouts.Add(loadout, value);
		}
		return value;
	}

	public bool Allows(Thing t)
	{
		if (t.def.useHitPoints)
		{
			FloatRange hpRange = HpRange;
			float value = GenMath.RoundedHundredth((float)t.HitPoints / (float)t.MaxHitPoints);
			if (!hpRange.IncludesEpsilon(Mathf.Clamp01(value)))
			{
				return false;
			}
		}
		QualityRange qualityRange = QualityRange;
		if (qualityRange != QualityRange.All && t.def.FollowQualityThingFilter())
		{
			if (!t.TryGetQuality(out var qc))
			{
				qc = QualityCategory.Normal;
			}
			if (!qualityRange.Includes(qc))
			{
				return false;
			}
		}
		return true;
	}

	public void Copy(Loadout_Extended from)
	{
		RefillThreshold = from.RefillThreshold;
		HpRange = from.HpRange;
		QualityRange = from.QualityRange;
	}

	public void ExposeData()
	{
		Scribe_Values.Look(ref RefillThreshold, "RefillThreshold", 1f);
		Scribe_Values.Look(ref HpRange, "hpRange", FloatRange.ZeroToOne);
		Scribe_Values.Look(ref QualityRange, "qualityRange", QualityRange.All);
	}

	[ClearDataOnNewGame]
	public static void ClearData()
	{
		Loadouts.Clear();
	}

	[Conditional("DEBUG")]
	private static void DebugLog()
	{
		_ticks++;
		if (_ticks % 100 == 0)
		{
			Log.Warning($"[Loadout_Extended:Loadouts] Count: {Loadouts.Count}");
		}
	}
}
