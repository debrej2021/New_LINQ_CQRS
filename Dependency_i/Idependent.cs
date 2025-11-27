using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet_Quick_ref_all.Dependency_i
{
    public interface Idependent
    {
        Task<int> GetTotalSum();
    }
}
