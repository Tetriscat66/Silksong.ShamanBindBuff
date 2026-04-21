using UnityEngine;

namespace ShamanBindBuff.Components {
	internal class AudioFadeOut : MonoBehaviour {
		private AudioSource source;
		private float initialVolume;
		private bool isFadingOut = false;

		public bool UseUnscaledTime = false;
		public float FadeSpeedMultiplier = 1f;
		
		private void Start() {
			source = GetComponent<AudioSource>();
			initialVolume = source.volume;
			isFadingOut = false;
		}

		private void OnEnable() {
			source.volume = initialVolume;
			isFadingOut = false;
		}

		private void Update() {
			if(isFadingOut) {
				float dt = UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
				source.volume -= dt * FadeSpeedMultiplier;
				if(source.volume <= 0) {
					source.volume = 0;
					gameObject.SetActive(false);
				}
			}
		}

		public void StartFadeout() {
			source.volume = initialVolume;
			isFadingOut = true;
		}
	}
}
