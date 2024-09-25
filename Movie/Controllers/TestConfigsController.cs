using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Movie.Configs;

namespace Movie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestConfigsController : ControllerBase
    {
        private readonly TestConfigs testConfigs;
        private readonly IOptions<TestConfigs> options;
        private readonly IOptionsSnapshot<TestConfigs> optionsSnapshot;
        private readonly IOptionsMonitor<TestConfigs> optionsMonitor;

        public TestConfigsController(TestConfigs testConfigs, IOptions<TestConfigs> options
            ,IOptionsSnapshot<TestConfigs> optionsSnapshot,IOptionsMonitor<TestConfigs> optionsMonitor)
        {
            this.testConfigs = testConfigs;
            this.options = options;
            this.optionsSnapshot = optionsSnapshot;
            this.optionsMonitor = optionsMonitor;
            //  reload the change when u run next time
            var value =options.Value;

            //  reload the change when  u run next request
            var value2 = optionsSnapshot.Value;

            //  reload the change when  u run change in same request
            var value3 = optionsMonitor.CurrentValue;
        }
        [HttpGet]
        public IActionResult Get() {

            //Thread.Sleep(1000);
        return Ok(testConfigs);
        }

        [HttpGet("way31")]
        public IActionResult Get31()
        {

            Thread.Sleep(10000);
            return Ok(options.Value);
        }
        [HttpGet("way32")]
        public IActionResult Get32()
        {

            Thread.Sleep(10000);
            return Ok(optionsSnapshot.Value);
        }

        [HttpGet("way33")]
        public IActionResult Get33()
        {

            Thread.Sleep(10000);
            return Ok(optionsMonitor.CurrentValue);
        }


    }
}
