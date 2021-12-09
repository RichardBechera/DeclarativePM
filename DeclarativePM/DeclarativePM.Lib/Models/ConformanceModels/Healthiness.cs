using System.Collections.Generic;
using System.Linq;
using DeclarativePM.Lib.Models.LogModels;
using DeclarativePM.Lib.Utils;

namespace DeclarativePM.Lib.Models.ConformanceModels
{
    public struct Healthiness
    {
        //TODO descriptions
        public double ActivationSparsity { get; set; }
        public double FulfillmentRation { get; set; }
        public double ViolationRation { get; set; }
        public double ConflictRation { get; set; }

        public Healthiness(ActivationBinaryTree tree)
        {
            int violations = MainMethods.GetViolation(tree).Count;
            int fulfilments = MainMethods.GetFulfillment(tree).Count;
            int conflicts = MainMethods.GetConflict(tree).Count;
            int na = violations + fulfilments + conflicts;
            int n = tree.Leaves
                .SelectMany(x => x.Subtrace)
                .Distinct(new EventEqualityComparer())
                .Count();
            

            ActivationSparsity = 1 - na / n;
            FulfillmentRation = fulfilments / (double)na;
            ViolationRation = violations / (double)na;
            ConflictRation = conflicts / (double)na;
        }
        
        public Healthiness(ActivationBinaryTree tree, int violations, int fulfillments, int conflicts)
        {
            int na = violations + fulfillments + conflicts;
            int n = tree.Leaves
                .SelectMany(x => x.Subtrace)
                .Distinct(new EventEqualityComparer())
                .Count();
            

            ActivationSparsity = 1 - na / n;
            FulfillmentRation = fulfillments / (double)na;
            ViolationRation = violations / (double)na;
            ConflictRation = conflicts / (double)na;
        }

        public Healthiness(List<Healthiness> constraintHealthiness)
        {
            var withoutNaN = constraintHealthiness.Where(h =>
                !double.IsNaN(h.ActivationSparsity) && !double.IsNaN(h.ConflictRation) &&
                !double.IsNaN(h.ViolationRation) && !double.IsNaN(h.FulfillmentRation));
            var averages =
                withoutNaN.Aggregate(((double)0, (double)0, (double)0, (double)0), 
                    (i, healthiness) => 
                        (healthiness.ActivationSparsity + i.Item1,
                        healthiness.FulfillmentRation + i.Item2,
                        healthiness.ViolationRation + i.Item3,
                        healthiness.ConflictRation + i.Item4)
                    );
            var all = withoutNaN.Count();
            ActivationSparsity = averages.Item1 / all;
            FulfillmentRation = averages.Item2 / all;
            ViolationRation = averages.Item3 / all;
            ConflictRation = averages.Item4 / all;
        }

        public override string ToString()
        {
            return $"(AS: {ActivationSparsity}, FR: {FulfillmentRation}, VR: {ViolationRation}, CR: {ConflictRation})";
        }
    }
}