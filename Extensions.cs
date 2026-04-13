using UnityEngine;

namespace ShamanBindBuff {
	public static class Extensions {
		public static Transform FindIncludeInactive(this Transform self, string path) {
			string[] parts = path.Split('/');
			Transform current = self;

			foreach(string part in parts) {
				bool found = false;

				foreach(Transform child in current) {
					if(child.name == part) {
						current = child;
						found = true;
						break;
					}
				}

				if(!found)
					return null;
			}

			return current;
		}

		public static GameObject Duplicate(this GameObject self, bool active) {
			GameObject go = GameObject.Instantiate(self);
			go.SetActive(active);
			go.transform.parent = self.transform.parent;
			go.transform.position = self.transform.position;
			go.transform.rotation = self.transform.rotation;
			go.transform.localScale = self.transform.localScale;
			return go;
		}
	}
}
