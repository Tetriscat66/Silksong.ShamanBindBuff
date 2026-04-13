using BepInEx;
using BepInEx.Logging;
using GlobalSettings;
using HarmonyLib;
using HutongGames.PlayMaker;
using ShamanBindBuff.FSMEdits;
using TeamCherry.Localization;

namespace ShamanBindBuff;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("org.silksong-modding.fsmutil")]
[BepInDependency("org.silksong-modding.i18n", BepInDependency.DependencyFlags.SoftDependency)]
[Harmony]
public class Plugin : BaseUnityPlugin {
	internal static new ManualLogSource Logger;

	private void Awake() {
		Logger = base.Logger;
		Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
		harmony.PatchAll();
		Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
	}

	[HarmonyPatch(typeof(PlayMakerFSM), nameof(PlayMakerFSM.Start))]
	[HarmonyPostfix]
	private static void FSMEdits(PlayMakerFSM __instance) {
		NoDistanceLimit.RemoveDistanceLimitCondition(__instance);
	}

	[HarmonyPatch(typeof(FsmState), nameof(FsmState.OnEnter))]
	[HarmonyPostfix]
	private static void FSMStateHooksEnter(FsmState __instance) {
		DiveEffect.EnableAnimationAndDamager(__instance);
	}

	[HarmonyPatch(typeof(FsmState), nameof(FsmState.OnExit))]
	[HarmonyPrefix]
	private static void FSMStateHooksExit(FsmState __instance) {
		DiveEffect.DisableDamager(__instance);
	}

	[HarmonyPatch(typeof(HeroController), nameof(HeroController.Start))]
	[HarmonyPostfix]
	private static void OnHeroControllerStart(HeroController __instance) {
		ChangeShamanDiveSpeed.GetQuickbindTool(__instance);
		DiveEffect.SetupDive(__instance);
	}

	[HarmonyPatch(typeof(ToolItemManager), nameof(ToolItemManager.Awake))]
	[HarmonyPostfix]
	private static void OverrideDescription() {
		LocalisedString crestSpellDesc = new LocalisedString($"Mods.{MyPluginInfo.PLUGIN_GUID}", "CREST_SPELL_DESC");
		if(!crestSpellDesc.Exists) {
			Logger.LogWarning($"SpellCrest descriptions could not be replaced as LocalizedString(Mods.{MyPluginInfo.PLUGIN_GUID}, CREST_SPELL_DESC) does not exist. The original descriptions will be used. To replace the descriptions, use the I18N mod. If you are already using the mod then there is a bug here.");
			return;
		}
		Gameplay.SpellCrest.description = crestSpellDesc;
		Gameplay.SpellCrest.getPromptDesc = crestSpellDesc;
	}
}
