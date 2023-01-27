using System.Diagnostics;
using Verse;

namespace CombatExtended.ExtendedLoadout;

public class DbgLog
{
	[Conditional("DEBUG")]
	public static void Msg(string message)
	{
		Log.Message("[ExtendedLoadout] " + message);
	}

	[Conditional("DEBUG")]
	public static void Err(string message)
	{
		Log.Error("[ExtendedLoadout] " + message);
	}

	[Conditional("DEBUG")]
	public static void Wrn(string message)
	{
		Log.Warning("[ExtendedLoadout] " + message);
	}
}
