using System.Collections.Generic;

namespace BehaviorTree
{
    public class Sequence : Node
    {
        private List<Node> children = new List<Node>();

        public Sequence(List<Node> nodes)
        {
            children = nodes;
        }

        public override NodeState Execute()
        {
            foreach (Node child in children)
            {
                switch (child.Execute())
                {
                    case NodeState.Failure:
                        state = NodeState.Failure;
                        return state;
                    case NodeState.Running:
                        state = NodeState.Running;
                        return state;
                }
            }

            state = NodeState.Success;
            return state;
        }
    }
}