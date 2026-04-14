using GlobalSettings;
using HutongGames.PlayMaker;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

namespace ShamanBindBuff.FSMEdits {
	internal static class DiveEffect {
		internal static GameObject DiveBurstFX, DiveBonkFX;
		internal static GameObject Dive, DiveShockwave, DiveContainer;
		internal static ToolItemSkill DiveTool;

		internal static void EnableAnimationAndDamager(FsmState state) {
			if(!(state.fsm.Name == "Bind" && state.Name == "Shaman Impact"))
				return;

			Dive.SetActive(true);
			DiveBonkFX.SetActive(true);
			DiveBurstFX.SetActive(true);

			HeroController.instance.parryInvulnTimer = 0.4f;
		}

		internal static void DisableDamager(FsmState state) {
			if(!(state.fsm.Name == "Bind" && state.Name == "Bind Ground"))
				return;

			if(Dive.activeSelf) {
				Dive.SetActive(false);
				DiveShockwave.SetActive(true);
			}
		}

		internal static void SetupDive(HeroController hc) {
			DiveTool = new ToolItemSkill();
			DiveTool.type = ToolItemType.Skill;
			DiveTool.zapDamageTicks = 1;
			DiveTool.name = "Shaman Dive";

			DiveContainer = new GameObject("Shaman Dive");
			DiveContainer.transform.parent = hc.transform.Find("Attacks");
			DiveContainer.transform.localPosition = new Vector3(0f, -0.885f, 0f);
			DiveContainer.layer = (1 << 4) + 1;
			DiveContainer.SetActive(true);

			DiveBurstFX = hc.transform.FindIncludeInactive("Special Attacks/Silk Charge DashBurst").gameObject.Duplicate(false);
			DiveBurstFX.name = "Dive FX";
			DiveBurstFX.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
			DiveBurstFX.transform.localPosition = new Vector3(-0.17f, 5f, 0f);
			DiveBurstFX.transform.GetChild(1).localPosition = new Vector3(0f, 0f, 0.003f);
			DiveBurstFX.transform.GetChild(0).localPosition = new Vector3(-3f, 0f, 0f);
			DiveBurstFX.transform.parent = DiveContainer.transform;

			DiveBonkFX = hc.transform.FindIncludeInactive("Special Attacks/Silk Charge WallBonk").gameObject.Duplicate(true);
			DiveBonkFX.name = "Shockwave FX";
			DiveBonkFX.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
			DiveBonkFX.transform.localPosition = new Vector3(0.125f, -0.7f, -0.008f);
			DiveBonkFX.transform.localScale = Vector3.one;
			DiveBonkFX.transform.parent = DiveContainer.transform;
			DiveBonkFX = DiveBonkFX.transform.GetChild(0).gameObject;
			DiveBonkFX.SetActive(false);

			Dive = new GameObject("Dive");
			Dive.transform.parent = DiveContainer.transform;
			Dive.transform.localPosition = Vector3.zero;
			Dive.layer = (1 << 4) + 1;
			Dive.SetActive(false);
			SetupCollider(Dive.AddComponent<PolygonCollider2D>(), 1f);
			SetupDamager(Dive.AddComponent<DamageEnemies>(), 0.75f);

			DiveShockwave = new GameObject("Shockwave");
			DiveShockwave.transform.parent = DiveContainer.transform;
			DiveShockwave.transform.localPosition = Vector3.zero;
			DiveShockwave.layer = (1 << 4) + 1;
			DiveShockwave.SetActive(false);
			SetupCollider(DiveShockwave.AddComponent<PolygonCollider2D>(), 1.5f);
			SetupDamager(DiveShockwave.AddComponent<DamageEnemies>(), 2.5f);
			DisableAfterTime disableShockwave = DiveShockwave.AddComponent<DisableAfterTime>();
			disableShockwave.waitTime = 0.2f;
			disableShockwave.isRealtime = false;
		}

		private static void SetupDamager(DamageEnemies dmg, float mult) {
			dmg.attackType = AttackTypes.Spell;
			dmg.useNailDamage = true;
			dmg.nailDamageMultiplier = mult;
			dmg.damageMultiplier = Gameplay.SpellCrestRuneDamageMult;
			dmg.representingTool = DiveTool;
			dmg.slashEffectOverrides = [];
			dmg.damageFSMEvent = string.Empty;
			dmg.deathEvent = string.Empty;
			dmg.targetRecordedFSMEvent = string.Empty;
			dmg.contactFSMEvent = string.Empty;
			dmg.dealtDamageFSMEvent = string.Empty;
			dmg.silkGeneration = HitSilkGeneration.None;
			dmg.specialType = SpecialTypes.Heavy | SpecialTypes.CocoonBreak;
			dmg.corpseDirection = new OverrideFloat();
			dmg.corpseMagnitudeMult = new OverrideFloat();
			dmg.currencyMagnitudeMult = new OverrideFloat();
			dmg.sourceIsHero = false;
			dmg.damageDealt = 15;
			dmg.damageMultPerHit = [];
			dmg.ignoreInvuln = false;
			dmg.magnitudeMult = 1.25f;
			dmg.DealtDamage = new UnityEvent();
			dmg.Tinked = new UnityEvent();
			dmg.allowedTinkFlags = ITinkResponder.TinkFlags.Projectile;
			dmg.doesNotTink = true;
			dmg.directionSourceOverride = DamageEnemies.DirectionSourceOverrides.CircleDirection;
			dmg.useHeroDamageAffectors = true;
			dmg.hitOnceUntilEnd = true;
		}

		private static void SetupCollider(PolygonCollider2D col, float mult) {
			col.isTrigger = true;
			col.points = [
				new Vector2(0f, 3.85f) * mult,
				new Vector2(-1.75f, 3.25f) * mult,
				new Vector2(-2.5f, 2.25f) * mult,
				new Vector2(-3f, -1f) * mult,
				new Vector2(-2.5f, -1.5f) * mult,
				new Vector2(2.5f, -1.5f) * mult,
				new Vector2(3f, -1f) * mult,
				new Vector2(2.5f, 2.25f) * mult,
				new Vector2(1.75f, 3.25f) * mult
			];
		}
	}
}
