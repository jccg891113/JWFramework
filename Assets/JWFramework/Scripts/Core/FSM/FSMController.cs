using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JWFramework;

namespace JWFramework.FSM
{
	public class FSMachine<T>
	{
		protected Dictionary<T, FState<T>> statePools = new Dictionary<T, FState<T>> ();
		
		private FState<T> currentState = null;
		private bool needDefaultState = false;
		private T defaultState;
		private JWData defaultStateEnterData = null;

		public bool Running { get { return currentState != null; } }

		public T CurrrentStateType { 
			get {
				if (currentState != null) {
					return currentState.stateType;
				} else {
					return default(T);
				}
			}
		}

		public FSMachine ()
		{
		}

		public void AddState (FState<T> newState)
		{
			this.statePools [newState.stateType] = newState;
		}

		public void SetDefaultState (T defaultState, JWData defaultStateEnterData = null)
		{
			this.defaultState = defaultState;
			this.needDefaultState = true;
			this.defaultStateEnterData = defaultStateEnterData;
		}

		public FState<T> this [T stateType] {
			get {
				return statePools [stateType];
			}
		}

		public V GetState<V> (T stateType) where V : FState<T>
		{
			return (V)statePools [stateType];
		}

		public virtual void Tick (float deltaTime)
		{
			if (currentState == null) {
				if (needDefaultState) {
					currentState = statePools [defaultState];
					currentState.Enter (defaultState, defaultStateEnterData);
				}
			} else {
				JWData enterParamData = null;
				T beforeStateType = currentState.stateType;
				T nextStateType = currentState.GetNextStateType (out enterParamData);
				if (Comparer<T>.Default.Compare (nextStateType, beforeStateType) != 0) {
					Goto (nextStateType, enterParamData);
				}
				currentState.Tick (deltaTime);
			}
		}

		public void Goto (T nextStateType, JWData enterParamData)
		{
			T beforeStateType = default(T);
			if (currentState != null) {
				beforeStateType = currentState.stateType;
				currentState.Leave (nextStateType);
			}
			if (statePools.ContainsKey (nextStateType)) {
				currentState = statePools [nextStateType];
				currentState.Enter (beforeStateType, enterParamData);
			}
		}
		
	}
}