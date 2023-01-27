using Verse;

namespace CombatExtended.ExtendedLoadout;

[StaticConstructorOnStartup]
public static class EarlyInit
{
	static EarlyInit()
	{
		LoadoutProxy_Patch.Patch();
	}
}
