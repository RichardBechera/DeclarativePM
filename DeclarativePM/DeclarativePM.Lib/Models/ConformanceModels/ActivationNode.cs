using System.Collections.Generic;
using DeclarativePM.Lib.Models.LogModels;

namespace DeclarativePM.Lib.Models.ConformanceModels
{
    /// <summary>
    /// Represents a node in Activation binary tree and a subtrace of tree trace
    /// </summary>
    public class ActivationNode
    {
        public ActivationNode Left;
        public ActivationNode Right;
        public bool IsLeaf => Left is null && Right is null;
        public bool IsDead;
        public bool MaxFulfilling;
        public readonly List<Event> Subtrace;

        public ActivationNode(List<Event> subtrace)
        {
            Subtrace = subtrace;
        }

        public ActivationNode(ActivationNode left = null, ActivationNode right = null, bool isDead = false,
            bool fulfilling = false, List<Event> subtrace = null)
        {
            Left = left;
            Right = right;
            IsDead = isDead;
            MaxFulfilling = fulfilling;
            Subtrace = subtrace ?? new();
        }
    }
}