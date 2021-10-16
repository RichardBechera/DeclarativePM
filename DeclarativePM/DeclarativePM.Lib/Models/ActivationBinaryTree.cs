using System.Collections.Generic;

namespace DeclarativePM.Lib.Models
{
    public class ActivationBinaryTree
    {
        public ActivationNode Root;
        public List<ActivationNode> Leafs;

        public ActivationBinaryTree()
        {
            Root = new();
            Leafs = new() {Root};
        }

        public void AddNodeLeft(ActivationNode current, ActivationNode node)
        {
            current.Left = node;
            UpdateLeafsList(current, node);
        }
        
        public void AddNodeRight(ActivationNode current, ActivationNode node)
        {
            current.Right = node;
            UpdateLeafsList(current, node);
        }

        private void UpdateLeafsList(ActivationNode current, ActivationNode node)
        {
            if (!Leafs.Contains(node))
                Leafs.Add(node);
            Leafs.Remove(current);
        }
    }
}