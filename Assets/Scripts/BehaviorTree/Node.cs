namespace BehaviorTree
{
    public abstract class Node
    {
        public enum NodeState { Success, Failure, Running }

        protected NodeState state;

        public NodeState State { get { return state; } }

        public abstract NodeState Execute();
    }
}