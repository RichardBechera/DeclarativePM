using System.Collections.Generic;
using DeclarativePM.Lib.Declare_Templates;

namespace DeclarativePM.Lib.Models.ConformanceModels
{
    public class ActivationBinaryTree
    {
        public ActivationNode Root { get; set; }
        public List<ActivationNode> Leafs { get; }
        public IBiTemplate Constraint { get; }

        public ActivationBinaryTree(IBiTemplate constraint)
        {
            Root = new();
            Leafs = new() {Root};
            Constraint = constraint;
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