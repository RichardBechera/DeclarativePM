using System.Collections.Generic;
using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;

namespace DeclarativePM.Lib.Models.ConformanceModels
{
    public class ActivationBinaryTree
    {
        public ActivationNode Root { get; set; }
        public List<ActivationNode> Leaves { get; }
        public BiTemplate Constraint { get; }

        public ActivationBinaryTree(BiTemplate constraint)
        {
            Root = new();
            Leaves = new() {Root};
            Constraint = constraint;
        }

        public void AddNodeLeft(ActivationNode current, ActivationNode node)
        {
            current.Left = node;
            UpdateLeavesList(current, node);
        }
        
        public void AddNodeRight(ActivationNode current, ActivationNode node)
        {
            current.Right = node;
            UpdateLeavesList(current, node);
        }

        private void UpdateLeavesList(ActivationNode current, ActivationNode node)
        {
            if (!Leaves.Contains(node))
                Leaves.Add(node);
            Leaves.Remove(current);
        }
    }
}