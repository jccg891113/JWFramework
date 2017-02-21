using UnityEngine;
using System.Collections;
using JWFramework;

namespace JWFramework.FSM
{
	public class FState<T>
	{
		protected readonly FSMachine<T> controller;
		public readonly T stateType;
		private float _runningTime;

		/// <summary>
		/// Get the running time.
		/// </summary>
		/// <value>Current state running time(s).</value>
		public float runningTime{ get { return _runningTime; } }

		public FState (T stateType, FSMachine<T> controller)
		{
			this.stateType = stateType;
			this.controller = controller;
			_runningTime = 0;
		}

		public void Enter (T beforeStateType, JWData enterParamData)
		{
			_runningTime = 0;
			this._Enter (beforeStateType, enterParamData);
		}

		protected virtual void _Enter (T beforeStateType, JWData enterParamData)
		{
		}

		public virtual T GetNextStateType (out JWData nextStateEnterParamData)
		{
			nextStateEnterParamData = null;
			return stateType;
		}

		public void Tick (float delta)
		{
			this._Tick (delta);
			_runningTime += delta;
		}

		protected virtual void _Tick (float delta)
		{
		}

		public void Leave (T nextStateType)
		{
			this._Leave (nextStateType);
		}

		protected virtual void _Leave (T nextStateType)
		{
		}

		protected void Goto (T nextStateType, JWData enterParamData)
		{
			controller.Goto (nextStateType, enterParamData);
		}
	}
}