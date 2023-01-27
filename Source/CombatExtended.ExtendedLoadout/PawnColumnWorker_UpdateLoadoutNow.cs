using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace CombatExtended.ExtendedLoadout;

[HotSwappable]
public class PawnColumnWorker_UpdateLoadoutNow : PawnColumnWorker
{
	private const int TopAreaHeight = 65;

	private const int ManageOutfitsButtonHeight = 32;

	internal const float _MinWidth = 158f;

	internal const float _OptimalWidth = 188f;

	internal static float IconSize = 16f;

	public static Texture2D ClearImage => ContentFinder<Texture2D>.Get("UI/Icons/clear");

	private static void HoldTrackerClear(Pawn pawn)
	{
		Utility_HoldTracker.HoldTrackerClear(pawn);
	}

	private static void UpdateLoadoutNow(Pawn pawn)
	{
		Job job = pawn.thinker?.GetMainTreeThinkNode<JobGiver_UpdateLoadout>()?.TryGiveJob(pawn);
		if (job != null)
		{
			pawn.jobs.StartJob(job, JobCondition.InterruptForced, (ThinkNode)null, false, true, (ThinkTreeDef)null, (JobTag?)null, false, false);
			if (pawn.mindState != null)
			{
				pawn.mindState.nextApparelOptimizeTick = -99999;
			}
		}
	}

	public override void DoHeader(Rect rect, PawnTable table)
	{
		base.DoHeader(rect, table);
		Rect rect2 = new Rect(rect.x, rect.y + (rect.height - 65f), Mathf.Min(rect.width, 360f), 32f);
		if (Widgets.ButtonText(rect2, (string)"CE_UpdateLoadoutNow".Translate(), true, false, true))
		{
			IEnumerable<Pawn> enumerable = Find.CurrentMap?.mapPawns?.AllPawnsSpawned;
			foreach (Pawn item in enumerable ?? Enumerable.Empty<Pawn>())
			{
				UpdateLoadoutNow(item);
			}
		}
		UIHighlighter.HighlightOpportunity(rect2, "CE_UpdateLoadoutNow");
	}

	public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
	{
		Pawn pawn2 = pawn;
		if (pawn2.outfits == null)
		{
			return;
		}
		int num = Mathf.FloorToInt(rect.width - 4f - IconSize);
		int num2 = Mathf.FloorToInt(IconSize);
		float x = rect.x;
		float y = rect.y + (rect.height - IconSize) / 2f;
		"CE_UpdateLoadoutNow".Translate().GetWidthCached();
		bool num3 = Utility_HoldTracker.HoldTrackerAnythingHeld(pawn2);
		Rect rect2 = new Rect(x, rect.y + 2f, num, rect.height - 4f);
		if (pawn2.Spawned && Widgets.ButtonText(rect2, (string)"CE_UpdateLoadoutNow".Translate(), true, true, true))
		{
			UpdateLoadoutNow(pawn2);
		}
		x += rect2.width;
		x += 4f;
		Rect rect3 = new Rect(x, y, num2, num2);
		if (!num3)
		{
			return;
		}
		if (Widgets.ButtonImage(rect3, ClearImage))
		{
			HoldTrackerClear(pawn2);
		}
		TooltipHandler.TipRegion(rect3, new TipSignal(delegate
		{
			string text = "CE_ForcedHold".Translate() + ":\n";
			foreach (HoldRecord holdRecord in LoadoutManager.GetHoldRecords(pawn2))
			{
				if (holdRecord.pickedUp)
				{
					text = string.Concat(text + "\n   " + holdRecord.thingDef.LabelCap + " x", holdRecord.count.ToString());
				}
			}
			return text;
		}, pawn2.GetHashCode() * 613));
		x += (float)num2;
		x += 4f;
	}

	public override int GetMinWidth(PawnTable table)
	{
		return Mathf.Max(base.GetMinWidth(table), Mathf.CeilToInt(158f));
	}

	public override int GetOptimalWidth(PawnTable table)
	{
		return Mathf.Clamp(Mathf.CeilToInt(188f), GetMinWidth(table), GetMaxWidth(table));
	}

	public override int GetMinHeaderHeight(PawnTable table)
	{
		return Mathf.Max(base.GetMinHeaderHeight(table), 65);
	}

	public override int Compare(Pawn a, Pawn b)
	{
		return a.GetLoadoutId().CompareTo(b.GetLoadoutId());
	}
}
