using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace CombatExtended.ExtendedLoadout;

[HotSwappable]
public class Dialog_ManageLoadouts_Extended : Dialog_ManageLoadouts
{
	private Pawn? _pawn;

	private Loadout? _pawnLoadout;

	private Vector2 _cardSize;

	public bool IsPersonalLoadout => _pawn != null;

	public override Vector2 InitialSize
	{
		get
		{
			Vector2 initialSize = base.InitialSize;
			if (_pawn != null)
			{
				return new Vector2(initialSize.x + _cardSize.x + 50f, Mathf.Max(initialSize.y, _cardSize.y));
			}
			return initialSize;
		}
	}

	public Dialog_ManageLoadouts_Extended(Loadout loadout)
		: base(loadout)
	{
	}

	public Dialog_ManageLoadouts_Extended(Pawn pawn, Loadout loadout)
		: base(loadout)
	{
		_pawn = pawn;
		_pawnLoadout = loadout;
		_cardSize = CharacterCardUtility.PawnCardSize(pawn);
	}

	public override void DoWindowContents(Rect canvas)
	{
		if (_pawn != null)
		{
			Vector2 initialSize = base.InitialSize;
			CharacterCardUtility.DrawCharacterCard(canvas.RightPartPixels(_cardSize.x), _pawn);
			canvas = canvas.LeftPartPixels(initialSize.x);
			if (base.CurrentLoadout != _pawnLoadout)
			{
				_pawn = null;
			}
		}
		base.DoWindowContents(canvas);
	}

	public override void PostClose()
	{
		base.PostClose();
		foreach (Loadout_Multi item in LoadoutMulti_Manager.LoadoutsMulti)
		{
			item.NotifyLoadoutChanged();
		}
	}
}
