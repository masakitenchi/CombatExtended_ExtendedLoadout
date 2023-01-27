using System;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace CombatExtended.ExtendedLoadout;

[HarmonyPatch(typeof(Dialog_ManageLoadouts))]
public class Dialog_ManageLoadouts_DrawSlotSelection_Patch
{
	[HarmonyPrefix]
	[HarmonyPatch("DrawSlotSelection")]
	public static bool DrawSlotSelection(Dialog_ManageLoadouts __instance, Rect canvas)
	{
		int num = ((__instance._sourceType == SourceSelection.Generic) ? __instance._sourceGeneric.Count : __instance._source.Count);
		GUI.DrawTexture(canvas, Dialog_ManageLoadouts._darkBackground);
		if ((__instance._sourceType != SourceSelection.Generic && __instance._source.NullOrEmpty()) || (__instance._sourceType == SourceSelection.Generic && __instance._sourceGeneric.NullOrEmpty()))
		{
			return false;
		}
		Rect rect = new Rect(canvas);
		rect.width -= 16f;
		rect.height = (float)num * 22f;
		Widgets.BeginScrollView(canvas, ref __instance._availableScrollPosition, rect.AtZero());
		int num2 = (int)Math.Floor((decimal)(__instance._availableScrollPosition.y / 22f));
		num2 = ((num2 >= 0) ? num2 : 0);
		int num3 = num2 + (int)Math.Ceiling((decimal)(canvas.height / 22f));
		num3 = ((num3 > num) ? num : num3);
		for (int i = num2; i < num3; i++)
		{
			Color color = GUI.color;
			Rect rect2 = new Rect(0f, (float)i * 22f, canvas.width, 22f);
			Rect rect3 = new Rect(rect2);
			if (__instance._sourceType == SourceSelection.Generic)
			{
				TooltipHandler.TipRegion(rect2, __instance._sourceGeneric[i].GetWeightAndBulkTip());
			}
			else
			{
				TooltipHandler.TipRegion(rect2, __instance._source[i].thingDef.GetWeightAndBulkTip());
			}
			rect3.xMin += 6f;
			if (i % 2 == 0)
			{
				GUI.DrawTexture(rect2, Dialog_ManageLoadouts._darkBackground);
			}
			Text.Anchor = TextAnchor.MiddleLeft;
			Text.WordWrap = false;
			if (__instance._sourceType == SourceSelection.Generic)
			{
				if (__instance.GetVisibleGeneric(__instance._sourceGeneric[i]))
				{
					GUI.color = Color.gray;
				}
				Widgets.Label(rect3, __instance._sourceGeneric[i].LabelCap);
			}
			else
			{
				Rect rect4 = rect2.LeftPart(0.9f);
				ThingDef thingDef = __instance._source[i].thingDef;
				Widgets.DefIcon(rect4.LeftPart(0.1f), (Def)thingDef, (ThingDef)null, 1f, (ThingStyleDef)null, false, (Color?)null);
				if (__instance._source[i].isGreyedOut)
				{
					GUI.color = Color.gray;
				}
				Widgets.Label(rect4.RightPart(0.85f), thingDef.LabelCap);
				if (Widgets.ButtonImageFitted(rect2.RightPart(0.15f).ContractedBy(2f), TexButton.Info))
				{
					ThingDef stuff = (thingDef.MadeFromStuff ? GenStuff.AllowedStuffsFor(thingDef).First() : null);
					Find.WindowStack.Add(new Dialog_InfoCard(thingDef, stuff));
				}
			}
			Text.WordWrap = true;
			Text.Anchor = TextAnchor.UpperLeft;
			Widgets.DrawHighlightIfMouseover(rect2);
			if (Widgets.ButtonInvisible(rect2))
			{
				if (__instance._sourceType == SourceSelection.Generic)
				{
					AddLoadoutSlotGeneric(__instance.CurrentLoadout, __instance._sourceGeneric[i]);
				}
				else
				{
					AddLoadoutSlotSpecific(__instance.CurrentLoadout, __instance._source[i].thingDef);
				}
			}
			GUI.color = color;
		}
		Widgets.EndScrollView();
		return false;
		static void AddLoadoutSlotGeneric(Loadout loadout, LoadoutGenericDef generic)
		{
			loadout.AddSlot(new LoadoutSlot(generic));
		}
		static void AddLoadoutSlotSpecific(Loadout loadout, ThingDef def, int count = 1)
		{
			loadout.AddSlot(new LoadoutSlot(def, count));
		}
	}
}
