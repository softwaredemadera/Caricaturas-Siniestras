using System;
using System.Collections.Generic;

namespace AI.FSM
{
    public class StateMachine
    {
        private static readonly List<StateTransition> EmptyTransitions = new(0);

        private readonly Dictionary<BaseState, List<StateTransition>> transitions = new();
        private readonly List<StateTransition> anyTransitions = new();
        private List<StateTransition> currentTransitions = EmptyTransitions;

        public BaseState CurrentState { get; private set; }

        public void SetInitialState(BaseState initialState)
        {
            if (initialState == null)
            {
                throw new ArgumentNullException(nameof(initialState));
            }

            ChangeState(initialState);
        }

        public void AddTransition(BaseState from, BaseState to, Func<bool> condition)
        {
            if (from == null || to == null || condition == null)
            {
                throw new ArgumentNullException("Los parámetros de la transición no pueden ser nulos.");
            }

            if (!transitions.TryGetValue(from, out List<StateTransition> stateTransitions))
            {
                stateTransitions = new List<StateTransition>();
                transitions[from] = stateTransitions;
            }

            stateTransitions.Add(new StateTransition(to, condition));
        }

        public void AddAnyTransition(BaseState to, Func<bool> condition)
        {
            if (to == null || condition == null)
            {
                throw new ArgumentNullException("Los parámetros de la transición global no pueden ser nulos.");
            }

            anyTransitions.Add(new StateTransition(to, condition));
        }

        public void Tick(float deltaTime)
        {
            if (CurrentState == null)
            {
                return;
            }

            StateTransition? triggeredTransition = GetTriggeredTransition();
            if (triggeredTransition.HasValue)
            {
                ChangeState(triggeredTransition.Value.TargetState);
            }

            CurrentState.Tick(deltaTime);
        }

        public void FixedTick(float fixedDeltaTime)
        {
            CurrentState?.FixedTick(fixedDeltaTime);
        }

        public void ChangeState(BaseState nextState)
        {
            if (nextState == null || nextState == CurrentState)
            {
                return;
            }

            CurrentState?.Exit();
            CurrentState = nextState;

            if (!transitions.TryGetValue(CurrentState, out currentTransitions))
            {
                currentTransitions = EmptyTransitions;
            }

            CurrentState.Enter();
        }

        private StateTransition? GetTriggeredTransition()
        {
            for (int i = 0; i < anyTransitions.Count; i++)
            {
                StateTransition transition = anyTransitions[i];
                if (transition.TargetState != CurrentState && transition.Condition())
                {
                    return transition;
                }
            }

            for (int i = 0; i < currentTransitions.Count; i++)
            {
                StateTransition transition = currentTransitions[i];
                if (transition.Condition())
                {
                    return transition;
                }
            }

            return null;
        }
    }
}