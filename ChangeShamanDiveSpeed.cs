using HarmonyLib;

namespace ShamanBindBuff {
	[Harmony]
	internal static class ChangeShamanDiveSpeed {
		private static ToolItem quickbindTool;

		internal static void GetQuickbindTool(HeroController hc) {
			quickbindTool = ToolItemManager.GetToolByName("Quickbind");
		}

		[HarmonyPatch(typeof(HeroController), nameof(HeroController.GetMaxFallVelocity))]
		[HarmonyPostfix]
		private static void ChangeDiveSpeed(HeroController __instance, ref float __result) {
			if(__instance.spellControl.ActiveStateName == "Shaman Fall" && quickbindTool.IsEquipped) {
				__result *= 2f;
			}
		}
	}
}
