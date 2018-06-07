using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MemoryUsageTest.Controllers
{
    public class ValuesController : Controller
    {
        private static readonly Random Random = new Random();

        [HttpGet("gc-info")]
        public object GcInfo()
        {
            return new
            {
                isServer = GCSettings.IsServerGC,
                latencyMode = GCSettings.LatencyMode,
            };
        }

        [HttpGet("workingset")]
        public long WorkingSet()
        {
            return Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024;
        }

        [HttpGet("allocate/{mb}")]
        public long Get(int mb)
        {
            Allocate(mb).ToString();
            return WorkingSet();
        }

        [HttpGet("collect")]
        public long Collect()
        {
            GC.Collect();
            GC.Collect();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
            return WorkingSet();
        }

        private object Allocate(int megabytes)
        {
            var array = new Object[megabytes];
            for (int i = 0; i < megabytes; i++)
            {
                array[i] = Allocate1Mb();
            }

            return array;
        }

        private object Allocate1Mb()
        {
            var array = new byte[1048576];
            Random.NextBytes(array);
            return array;
        }
    }
}
