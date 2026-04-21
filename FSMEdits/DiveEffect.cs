using GlobalSettings;
using HutongGames.PlayMaker;
using ShamanBindBuff.Components;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

namespace ShamanBindBuff.FSMEdits {
	internal static class DiveEffect {
		internal static GameObject DiveBurstFX, DiveBonkFX, BloomFX;
		internal static GameObject Dive, DiveShockwave, DiveContainer;
		internal static ToolItemSkill DiveTool;
		internal static SpriteRenderer DiveBurstSprite;
		internal static tk2dSprite DiveBonkSprite;
		internal static ParticleSystem.EmissionModule ZapEmission;
		internal static AudioFadeOut ZapFadeOut;

		internal static void EnableAnimationAndDamager(FsmState state) {
			if(!(state.fsm.Name == "Bind" && state.Name == "Shaman Impact"))
				return;

			Dive.SetActive(true);

			Color color = Gameplay.ZapImbuementTool.IsEquipped ? Gameplay.ZapDamageTintColour : Color.white;
			DiveBurstSprite.color = color;
			DiveBonkSprite.color = color;

			DiveBonkFX.SetActive(true);
			DiveBurstFX.SetActive(true);
			if(Gameplay.ZapImbuementTool.IsEquipped) {
				ZapEmission.enabled = true;
				ZapFadeOut.gameObject.SetActive(true);
			}

			HeroController.instance.parryInvulnTimer = 0.4f;
		}

		internal static void DisableDamager(FsmState state) {
			if(!(state.fsm.Name == "Bind" && state.Name == "Bind Ground"))
				return;

			if(Dive.activeSelf) {
				Dive.SetActive(false);
				DiveShockwave.SetActive(true);
				ZapEmission.enabled = false;
				ZapFadeOut.StartFadeout();
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
			DiveBurstFX.name = "Dive Burst FX";
			DiveBurstFX.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
			DiveBurstFX.transform.localPosition = new Vector3(-0.17f, 5f, 0f);
			DiveBurstFX.transform.GetChild(1).localPosition = new Vector3(0f, 0f, 0.003f);
			DiveBurstFX.transform.GetChild(0).localPosition = new Vector3(-3f, 0f, 0f);
			DiveBurstFX.transform.parent = DiveContainer.transform;

			DiveBonkFX = hc.transform.FindIncludeInactive("Special Attacks/Silk Charge WallBonk").gameObject.Duplicate(true);
			DiveBonkFX.name = "Dive Bonk FX";
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

			BloomFX = hc.transform.FindIncludeInactive("Special Attacks/Silk Charge Damager/Shaman Rune/Shaman Rune Camera Bloom").gameObject.Duplicate(false);
			BloomFX.name = "Bloom Effect";
			BloomFX.transform.parent = DiveContainer.transform;
			BloomFX.transform.localPosition = new Vector3(1, 0, -0.0017f);
			DisableAfterTime disableBloomFX = BloomFX.AddComponent<DisableAfterTime>();
			disableBloomFX.waitTime = 1.25f;
			disableBloomFX.isRealtime = false;

			SetupRuneEffect(Dive.AddComponent<HeroShamanRuneEffect>(), 
				Dive.GetComponent<DamageEnemies>(),
				BloomFX,
				[ BloomFX.GetComponent<SpriteRenderer>() ]
			);
			SetupRuneEffect(
				DiveShockwave.AddComponent<HeroShamanRuneEffect>(), 
				DiveShockwave.GetComponent<DamageEnemies>() 
			);

			DiveBurstSprite = DiveBurstFX.transform.FindIncludeInactive("sprint_fall_dash_effect0002").GetComponent<SpriteRenderer>();
			DiveBonkSprite = DiveBonkFX.GetComponent<tk2dSprite>();

			GameObject zapParticles = hc.transform.FindIncludeInactive("Effects/Silk Charge Particles Zap").gameObject.Duplicate(true);
			zapParticles.name = "Zap Particles";
			zapParticles.transform.parent = DiveContainer.transform;
			zapParticles.transform.localPosition = new Vector3(0, -0.57f, 0.056f);
			ParticleSystem zapSystem = zapParticles.GetComponent<ParticleSystem>();
			ZapEmission = zapSystem.emission;
			ZapEmission.rateOverDistance = 0;
			ZapEmission.rateOverTime = 100;
			ParticleSystem.MainModule zapMain = zapSystem.main;
			zapMain.playOnAwake = true;
			zapMain.gravityModifier = 0;
			ParticleSystem.MinMaxCurve zapStartSize = zapMain.startSize;
			zapStartSize.constant = 3.2f;
			ParticleSystem.MinMaxCurve zapStartSpeed = zapMain.startSpeed;
			zapStartSpeed.constant = 3;
			ParticleSystem.ShapeModule zapShape = zapSystem.shape;
			zapShape.angle = 0;
			zapShape.arc = 180;
			zapShape.radius = 2.6f;
			zapShape.shapeType = ParticleSystemShapeType.Hemisphere;

			GameObject zapSFX = hc.transform.FindIncludeInactive("Special Attacks/Sphere Ball/Ball/thread_sphere_effect_zap/Zap Loop").gameObject.Duplicate(false);
			zapSFX.name = "Zap Loop";
			zapSFX.transform.parent = DiveContainer.transform;
			zapSFX.transform.localPosition = Vector3.zero;
			zapSFX.GetComponent<AudioSource>().pitch = 1.2f;
			ZapFadeOut = zapSFX.AddComponent<AudioFadeOut>();
			ZapFadeOut.FadeSpeedMultiplier = 3f;
		}

		private static void SetupRuneEffect(HeroShamanRuneEffect runeEffect, DamageEnemies damager, GameObject rune = null, SpriteRenderer[] runeSprites = null) {
			runeEffect.damager = damager;
			runeEffect.rune = rune;
			runeEffect.zapTintSprites = [];
			runeEffect.zapTintParticles = [];

			if(runeSprites != null) {
				runeEffect.zapTintSprites.AddRange(runeSprites);
			}
		}

		private static void SetupDamager(DamageEnemies dmg, float mult) {
			dmg.attackType = AttackTypes.Heavy;
			dmg.useNailDamage = true;
			dmg.nailDamageMultiplier = mult;
			dmg.damageMultiplier = 1; // Gameplay.SpellCrestRuneDamageMult
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
