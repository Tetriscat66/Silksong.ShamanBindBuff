using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Silksong.FsmUtil;

namespace ShamanBindBuff.FSMEdits {
	internal static class NoDistanceLimit {
		internal static void RemoveDistanceLimitCondition(PlayMakerFSM fsm) {
			if(fsm.FsmName != "Bind")
				return;

			FsmState state = fsm.GetState("Shaman Fall");
			FloatTestToBool floatTestToBool = state.GetStateAction(8) as FloatTestToBool;
			floatTestToBool.greaterThanBool = floatTestToBool.lessThanBool;
		}
	}
}
