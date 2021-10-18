using System.Linq;
using DeclarativePM.Lib.Utils;

namespace DeclarativePM.Lib.Models
{
    public struct Healthiness
    {
        public double ActivationSparsity { get; set; }
        public double FulfillmentRation { get; set; }
        public double ViolationRation { get; set; }
        public double ConflictRation { get; set; }

        public Healthiness(ActivationBinaryTree tree)
        {
            int violations = MainMethods.GetViolation(tree).Count;
            int fulfillments = MainMethods.GetFulfillment(tree).Count;
            int conflicts = MainMethods.GetConflict(tree).Count;
            int na = violations + fulfillments + conflicts;
            int n = tree.Leafs
                .SelectMany(x => x.Subtrace)
                .Distinct(new EventEqualityComparer())
                .Count();
            

            ActivationSparsity = 1 - na / n;
            FulfillmentRation = (double)fulfillments / (double)na;
            ViolationRation = (double)violations / (double)na;
            ConflictRation = (double)conflicts / (double)na;
        }
        
        public Healthiness(ActivationBinaryTree tree, int violations, int fulfillments, int conflicts)
        {
            int na = violations + fulfillments + conflicts;
            int n = tree.Leafs
                .SelectMany(x => x.Subtrace)
                .Distinct(new EventEqualityComparer())
                .Count();
            

            ActivationSparsity = 1 - na / n;
            FulfillmentRation = (double)fulfillments / (double)na;
            ViolationRation = (double)violations / (double)na;
            ConflictRation = (double)conflicts / (double)na;
        }

        public override string ToString()
        {
            return $"(AS: {ActivationSparsity}, FR: {FulfillmentRation}, VR: {ViolationRation}, CR: {ConflictRation})";
        }
    }
}