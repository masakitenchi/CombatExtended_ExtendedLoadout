using Verse;

namespace CombatExtended.ExtendedLoadout;

public class ModActive
{
	private static bool? _betterPawnControl;

	public static bool BetterPawnControl
	{
		get
		{
			bool valueOrDefault = _betterPawnControl.GetValueOrDefault();
			if (!_betterPawnControl.HasValue)
			{
				valueOrDefault = ModLister.GetActiveModWithIdentifier("VouLT.BetterPawnControl") != null;
				_betterPawnControl = valueOrDefault;
				return valueOrDefault;
			}
			return valueOrDefault;
		}
	}
}
