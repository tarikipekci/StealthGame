namespace BehaviorTree
{
    public class ActionNode : Node
    {
        private System.Func<NodeState> action;

        public ActionNode(System.Func<NodeState> action)
        {
            this.action = action;
        }

        public override NodeState Execute()
        {
            return action();
        }
    }
}