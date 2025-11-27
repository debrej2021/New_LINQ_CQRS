using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DotNet_Quick_ref_all.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MainController:ControllerBase
    {
        public readonly Dependency_i.Idependent _dependent;

        public MainController(Dependency_i.Idependent dependent)
        {
            _dependent = dependent;
        }
        [HttpGet("GetSum")]
        public async Task<IActionResult> GetSum()
        {
            var sum = await _dependent.GetTotalSum();
            return Ok(sum);
        }
    }
}
