using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace CombatExtended.ExtendedLoadout;

[HotSwappable]
public class PawnColumnWorker_Loadout_Multi : PawnColumnWorker_Loadout
{
    private static readonly Texture2D SortingIcon = ContentFinder<Texture2D>.Get("UI/Icons/Sorting");
    private static readonly Texture2D SortingDescendingIcon = ContentFinder<Texture2D>.Get("UI/Icons/SortingDescending");
    public static Texture2D PersonalLoadoutImage => ContentFinder<Texture2D>.Get("UI/personalLoadout");

	private int GetIndexFromDefName(string defName)
	{
		return int.Parse(defName.Split('_')[1]);
	}

	protected IEnumerable<Widgets.DropdownMenuElement<Loadout>> Btn_GenerateMenu(Pawn pawn)
	{
		Pawn pawn2 = pawn;
		foreach (Loadout loadout in LoadoutManager.Loadouts)
		{
			yield return new Widgets.DropdownMenuElement<Loadout>
			{
				option = new FloatMenuOption(loadout.LabelCap, delegate
				{
					pawn2.SetLoadout(loadout, GetIndexFromDefName(def.defName));
				}),
				payload = loadout
			};
		}
	}

	public override void DoHeader(Rect rect, PawnTable table)
	{
		if (GetIndexFromDefName(def.defName) == 0)
		{
			base.DoHeader(rect, table);
			return;
		}
		if (!def.label.NullOrEmpty())
		{
			Text.Font = DefaultHeaderFont;
			GUI.color = DefaultHeaderColor;
			Text.Anchor = TextAnchor.LowerCenter;
			Rect rect2 = rect;
			rect2.y += 3f;
			Widgets.Label(rect2, def.LabelCap.Resolve().Truncate(rect.width));
			Text.Anchor = TextAnchor.UpperLeft;
			GUI.color = Color.white;
			Text.Font = GameFont.Small;
		}
		else if (def.HeaderIcon != null)
		{
			Vector2 headerIconSize = def.HeaderIconSize;
			int num = (int)((rect.width - headerIconSize.x) / 2f);
			GUI.DrawTexture(new Rect(rect.x + (float)num, rect.yMax - headerIconSize.y, headerIconSize.x, headerIconSize.y).ContractedBy(2f), def.HeaderIcon);
		}
		if (table.SortingBy == def)
		{
			Texture2D texture2D = (table.SortingDescending ? SortingDescendingIcon : SortingIcon);
			GUI.DrawTexture(new Rect(rect.xMax - (float)texture2D.width - 1f, rect.yMax - (float)texture2D.height - 1f, texture2D.width, texture2D.height), texture2D);
		}
		if (!def.HeaderInteractable)
		{
			return;
		}
		Rect interactableHeaderRect = GetInteractableHeaderRect(rect, table);
		if (Mouse.IsOver(interactableHeaderRect))
		{
			Widgets.DrawHighlight(interactableHeaderRect);
			string headerTip = GetHeaderTip(table);
			if (!headerTip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(interactableHeaderRect, headerTip);
			}
		}
		if (Widgets.ButtonInvisible(interactableHeaderRect))
		{
			HeaderClicked(rect, table);
		}
	}

	public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
	{
		if (pawn.outfits == null)
		{
			return;
		}
		Log.Message("Pawn.outfits passed");
		int index = GetIndexFromDefName(def.defName);
		int num = Mathf.FloorToInt(rect.width - 4f - PawnColumnWorker_Loadout.IconSize);
		int num2 = Mathf.FloorToInt(PawnColumnWorker_Loadout.IconSize);
		float num3 = rect.x;
		float y = rect.y + (rect.height - PawnColumnWorker_Loadout.IconSize) / 2f;
		Rect rect2 = new Rect(num3, rect.y + 2f, num, rect.height - 4f);
		if (index == 0)
		{
			int num4 = 24;
			Rect rect3 = new Rect(rect2.x, rect2.y, num4, num4);
			rect2.x += 4f + (float)num4;
			rect2.width -= 4f + (float)num4;
			num3 += 4f + (float)num4;
			if (Widgets.ButtonImage(rect3, PersonalLoadoutImage))
			{
				Loadout_Multi loadout = new Loadout_Multi(pawn);
				Log.Message($"Pawn:{pawn},Loadout:{loadout.uniqueID}");
				Find.WindowStack.Add(new Dialog_ManageLoadouts_Extended(pawn, loadout.PersonalLoadout));
			}
			TooltipHandler.TipRegion(rect3, new TipSignal(PawnColumnWorker_Loadout.textGetter("CE_Extended.PersonalLoadoutTip"), pawn.GetHashCode() * 6178));
		}
		string buttonLabel = (pawn.GetLoadout() as Loadout_Multi)[index].label.Truncate(rect2.width);
		Widgets.Dropdown(rect2, pawn, (Pawn p) => (p.GetLoadout() as Loadout_Multi)[index], Btn_GenerateMenu, buttonLabel, null, null, null, null, paintable: true);
		num3 += rect2.width;
		num3 += 4f;
		Rect rect4 = new Rect(num3, y, num2, num2);
		if (Widgets.ButtonImage(rect4, PawnColumnWorker_Loadout.EditImage))
		{
			Find.WindowStack.Add(new Dialog_ManageLoadouts_Extended((pawn.GetLoadout() as Loadout_Multi)[index]));
		}
		TooltipHandler.TipRegion(rect4, new TipSignal(PawnColumnWorker_Loadout.textGetter("CE_Loadouts"), pawn.GetHashCode() * 613));
		num3 += (float)num2;
	}
}
