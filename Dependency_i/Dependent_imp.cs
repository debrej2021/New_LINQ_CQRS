using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet_Quick_ref_all.Dependency_i
{
    public class Dependent_imp: Idependent
    {
        public async Task<int> GetTotalSum()
        {
            await Task.Delay(100);
            return 42;
        }
    }
   
}
