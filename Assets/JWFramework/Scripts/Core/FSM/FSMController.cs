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
		
		private bool hadDefaultState = false;
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
			this.hadDefaultState = true;
			this.defaultStateEnterData = defaultStateEnterData;
		}

		public bool CurrentStateIs (T state)
		{
			if (currentState != null) {
				return Comparer<T>.Default.Compare (state, currentState.stateType) == 0;
			} else {
				return false;
			}
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
				if (hadDefaultState) {
					currentState = statePools [defaultState];
					currentState.Enter (defaultState, defaultStateEnterData);
				}
			} else {
				JWData enterParamData = null;
				T beforeStateType = currentState.stateType;
				T nextStateType = currentState.GetNextStateType (out enterParamData);
				if (Comparer<T>.Default.Compare (nextStateType, beforeStateType) != 0) {
					currentState.Leave (nextStateType);
					currentState = statePools [nextStateType];
					currentState.Enter (beforeStateType, enterParamData);
				}
				currentState.Tick (deltaTime);
			}
		}

		public void Goto (T nextStateType, JWData enterParamData, bool allowSameState = false)
		{
			T beforeStateType = default(T);
			if (currentState != null) {
				beforeStateType = currentState.stateType;
				if (!allowSameState && Comparer<T>.Default.Compare (nextStateType, beforeStateType) == 0) {
					return;
				}
				currentState.Leave (nextStateType);
			}
			if (statePools.ContainsKey (nextStateType)) {
				currentState = statePools [nextStateType];
				currentState.Enter (beforeStateType, enterParamData);
			}
		}
		
	}
}