namespace CombatExtended.ExtendedLoadout;

public static class Loadout_Extensions
{
	public static Loadout_Extended Extended(this Loadout loadout)
	{
		return Loadout_Extended.Get(loadout);
	}

	public static void CopyLoadoutExtended(this Loadout dest, Loadout from)
	{
		dest.Extended().Copy(from.Extended());
	}
}
