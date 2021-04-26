using System;
using System.Collections.Generic;
using System.Linq;

namespace DeclarativePM.Lib.Discovery
{
    public class Discovery
    {
        public void DiscoverModel(List<string> templates, List<char> log)
        {
            /*foreach (var template in templates)
            {
                
            }*/
            
            
            
        }
        
        List<List<char>> Combinations(int rest, char[] bag)
        {
            var res = new List<List<char>>();
            List<List<char>> recursive = null;
            
            if (rest != 1)
                recursive = Combinations(rest - 1, bag);

            foreach (var t in bag)
            {
                if (rest == 1)
                {
                    res.Add(new List<char> {t} );
                    continue;
                }
                res.AddRange(recursive?
                    .Select(c => new List<char>(c) {t})!);
            }
            return res;
        }
    }
}