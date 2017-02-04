using UnityEngine;
using System.Collections;
using JWFramework;

namespace JWFramework.FSM
{
	public class FState<T>
	{
		protected readonly FSMachine<T> controller;
		public readonly T stateType;

		public FState (T stateType, FSMachine<T> controller)
		{
			this.stateType = stateType;
			this.controller = controller;
		}

		public virtual void Enter (T beforeStateType, JWData enterParamData)
		{
		}

		public virtual T GetNextStateType (out JWData nextStateEnterParamData)
		{
			nextStateEnterParamData = null;
			return stateType;
		}

		public virtual void Tick (float delta)
		{
		}

		public virtual void Leave (T nextStateType)
		{
		}

		protected void Goto (T nextStateType, JWData enterParamData)
		{
			controller.Goto (nextStateType, enterParamData);
		}
	}
}