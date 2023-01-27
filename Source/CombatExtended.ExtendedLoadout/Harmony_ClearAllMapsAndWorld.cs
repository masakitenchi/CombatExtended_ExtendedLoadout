using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Verse;
using Verse.Profile;

namespace CombatExtended.ExtendedLoadout;

[HarmonyPatch(typeof(MemoryUtility), "ClearAllMapsAndWorld")]
public static class Harmony_ClearAllMapsAndWorld
{
	private static List<MethodInfo>? _clearDataMethods;

	public static void Prefix()
	{
		if (_clearDataMethods == null)
		{
			_clearDataMethods = GetClearingMethods();
		}
		_clearDataMethods?.ForEach(delegate(MethodInfo x)
		{
			x.Invoke(null, null);
		});
	}

	private static List<MethodInfo>? GetClearingMethods()
	{
		ClearDataOnNewGame customAttribute;
		return (from x in Assembly.GetExecutingAssembly().GetTypes().SelectMany((Type x) => x.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
			where x.TryGetAttribute<ClearDataOnNewGame>(out customAttribute)
			select x).ToList();
	}
}
