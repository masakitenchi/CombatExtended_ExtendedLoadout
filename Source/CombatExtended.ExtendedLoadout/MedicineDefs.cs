using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace CombatExtended.ExtendedLoadout;

public class MedicineDefs
{
	public static void Initialize()
	{
		List<ThingDef> list = DefDatabase<ThingDef>.AllDefs.Where((ThingDef x) => x.thingCategories != null && x.thingCategories.Contains(ThingCategoryDefOf.Medicine) && x.IsMedicine).ToList();
		list.Sort(delegate(ThingDef a, ThingDef b)
		{
			if (a.GetStatValueAbstract(StatDefOf.MedicalPotency) < b.GetStatValueAbstract(StatDefOf.MedicalPotency))
			{
				return -1;
			}
			return (a.GetStatValueAbstract(StatDefOf.MedicalPotency) > b.GetStatValueAbstract(StatDefOf.MedicalPotency)) ? 1 : 0;
		});
		for (int i = 0; i < list.Count; i++)
		{
			ThingDef thingDef = list[i];
			LoadoutGenericDef obj = new LoadoutGenericDef
			{
				defName = "CEEL_GenericMedicine_" + thingDef.defName,
				defaultCount = 5,
				defaultCountType = LoadoutCountType.pickupDrop,
				description = "Generic Loadout for Medicine.  Intended for pawns which will handle triage activities.",
				label = "CE_Extended.Medicines".Translate(thingDef.LabelCap),
				thingRequestGroup = ThingRequestGroup.PotentialBillGiver
			};
			List<ThingDef> allowedMeds = list.Take(i + 1).ToList();
			obj._lambda = (ThingDef td) => td != null && td.IsMedicine && allowedMeds.Contains(td);
			obj.isBasic = false;
			DefDatabase<LoadoutGenericDef>.Add(obj);
		}
	}
}
