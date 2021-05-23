using System.Collections.Generic;
using System.Linq;

namespace DeclarativePM.Lib.Utils
{
    public static class UtilMethods
    {
        /// <summary>
        /// Generates all the possible combinations of elements in the bag.
        /// </summary>
        /// <param name="rest">How many elements do we combine into one combination.</param>
        /// <param name="bag">All the possible elements.</param>
        /// <param name="repeat">Whether generate combinations with repeat or not.</param>
        /// <typeparam name="T">Type of element for which we generate combinations.</typeparam>
        /// <returns>List containing all the combinations of elements of type T stored as List<T>.</returns>
        public static List<List<T>> Combinations<T>(int rest, T[] bag, bool repeat)
        {
            var res = new List<List<T>>();
            List<List<T>> recursive = null;
            
            if (rest != 1)
                recursive = Combinations(rest - 1, bag, repeat);

            foreach (var t in bag)
            {
                if (rest == 1)
                {
                    res.Add(new List<T> {t} );
                    continue;
                }

                if (!repeat)
                {
                    res.AddRange(recursive?.Where(c => !c.Contains(t)).Select(c => new List<T>(c) {t}) ?? new List<List<T>>());
                    continue;
                }

                res.AddRange(recursive?
                    .Select(c => new List<T>(c) {t}) ?? new List<List<T>>());
            }
            return res;
        }

        public static void CutIntoRange(ref decimal p, decimal min = 0, decimal max = 100)
        {
            p = p > max ? max : p;
            p = p < min ? min : p;
        }
    }
}