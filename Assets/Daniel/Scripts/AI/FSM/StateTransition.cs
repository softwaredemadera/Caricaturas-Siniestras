using System;

namespace AI.FSM
{
    public readonly struct StateTransition
    {
        public BaseState TargetState { get; }
        public Func<bool> Condition { get; }

        public StateTransition(BaseState targetState, Func<bool> condition)
        {
            TargetState = targetState;
            Condition = condition;
        }
    }
}