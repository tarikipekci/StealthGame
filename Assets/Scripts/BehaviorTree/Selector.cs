using System.Collections.Generic;

namespace BehaviorTree
{
    public class Selector : Node
    {
        private List<Node> children = new List<Node>();

        public Selector(List<Node> nodes)
        {
            children = nodes;
        }

        public override NodeState Execute()
        {
            foreach (Node child in children)
            {
                switch (child.Execute())
                {
                    case NodeState.Success:
                        state = NodeState.Success;
                        return state;
                    case NodeState.Running:
                        state = NodeState.Running;
                        return state;
                }
            }

            state = NodeState.Failure;
            return state;
        }
    }
}