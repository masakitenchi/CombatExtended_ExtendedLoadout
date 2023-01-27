using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using HugsLib;
using HugsLib.Settings;
using RimWorld;
using Verse;

namespace CombatExtended.ExtendedLoadout;

public class ExtendedLoadoutMod : ModBase
{
	public static ExtendedLoadoutMod Instance = null;

	public const int MaxColumnCount = 10;

	public bool useMultiLoadouts;

	public bool useHpAndQualityInLoadouts;

	private readonly SettingHandle<string>[] loadoutNames = new SettingHandle<string>[10];

	public static readonly string HarmonyId = "PirateBY.CombatExtended.ExtendedLoadout";

	private static Harmony? _harmony;

	protected override bool HarmonyAutoPatch => false;

	public static Harmony Harmony => _harmony ?? (_harmony = new Harmony(HarmonyId));

	public override string ModIdentifier => "CombatExtended.ExtendedLoadout";

	public ExtendedLoadoutMod()
	{
		Instance = this;
	}

	public override void DefsLoaded()
	{
		ModSettingsPack modSettings = HugsLibController.Instance.Settings.GetModSettings("CombatExtended.ExtendedLoadout");
		SettingHandle<bool> handle = modSettings.GetHandle("UseHpAndQualityInLoadouts", "Settings.UseHpAndQualityInLoadouts.Label".Translate(), "Settings.UseHpAndQualityInLoadouts.Desc".Translate(), defaultValue: true);
		SettingHandle<bool> UseMultiLoadouts = modSettings.GetHandle("UseMultiLoadouts", "Settings.UseMultiLoadouts.Label".Translate(), "Settings.UseMultiLoadouts.Desc".Translate(), defaultValue: true);
		int result;
		SettingHandle<int> MultiLoadoutsCount = modSettings.GetHandle("MultiLoadoutsCount", "Settings.MultiLoadoutsCount.Label".Translate(), "Settings.MultiLoadoutsCount.Desc".Translate(), 3, (string value) => int.TryParse(value, out result) && result >= 2 && result <= 10);
		MultiLoadoutsCount.VisibilityPredicate = () => UseMultiLoadouts;
		for (int i = 0; i < 10; i++)
		{
			int colId = i;
			loadoutNames[i] = modSettings.GetHandle($"LoadoutName_{i}", $"Loadout{i + 1}".Translate(), "", $"Loadout{i + 1}".Translate().RawText);
			loadoutNames[i].VisibilityPredicate = () => (bool)UseMultiLoadouts && colId < (int)MultiLoadoutsCount;
			((SettingHandle)loadoutNames[i]).ValueChanged += ((Action<SettingHandle>) delegate
			{
				PawnColumnDef pawnColumnDef = Enumerable.FirstOrDefault(DefDatabase<PawnTableDef>.GetNamed("Assign").columns, (PawnColumnDef c) => c.defName.Equals($"Loadout_{colId}"));
				if (pawnColumnDef != null)
				{
					pawnColumnDef.label = loadoutNames[colId].Value;
					((Def)pawnColumnDef).cachedLabelCap = null;
				}
			});
		}
		useHpAndQualityInLoadouts = handle;
		if ((bool)UseMultiLoadouts && (int)MultiLoadoutsCount >= 2 && (int)MultiLoadoutsCount <= 10)
		{
			List<PawnColumnDef> columns = DefDatabase<PawnTableDef>.GetNamed("Assign").columns;
			int num = columns.FindIndex((PawnColumnDef x) => x.defName.Equals("Loadout"));
			if (num != -1)
			{
				columns.RemoveAt(num);
				columns.InsertRange(num, GeneratePawnColumnDefs(MultiLoadoutsCount));
				Loadout_Multi.ColumnsCount = MultiLoadoutsCount;
				useMultiLoadouts = true;
				Log.Message($"[CombatExtended.ExtendedLoadout] {MultiLoadoutsCount}x Loadout columns injected");
			}
			else
			{
				Log.Error("[CombatExtended.ExtendedLoadout] Can't find CE Loadout column");
			}
		}
		Harmony.PatchAll();
		if (!useMultiLoadouts)
		{
			LoadoutProxy_Patch.Unpatch();
		}
		MedicineDefs.Initialize();
		Log.Message("[CombatExtended.ExtendedLoadout] Initialized");
	}

	private IEnumerable<PawnColumnDef> GeneratePawnColumnDefs(int count)
	{
		yield return new PawnColumnDef
		{
			defName = "CE_UpdateLoadoutNow",
			workerClass = typeof(PawnColumnWorker_UpdateLoadoutNow),
			label = "CE_UpdateLoadoutNow".Translate(),
			sortable = false
		};
		for (int i = 0; i < count; i++)
		{
			yield return new PawnColumnDef
			{
				defName = $"Loadout_{i}",
				workerClass = typeof(PawnColumnWorker_Loadout_Multi),
				label = loadoutNames[i],
				sortable = true
			};
		}
	}
}
