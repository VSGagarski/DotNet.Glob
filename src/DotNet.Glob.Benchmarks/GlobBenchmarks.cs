﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using DotNet.Globbing;
using DotNet.Globbing.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Glob;
using BenchmarkDotNet.Attributes.Columns;

namespace DotNet.Glob.PerfTests
{

    [ClrJob, CoreJob, MemoryDiagnoser, MinColumn, MaxColumn]
    public class GlobBenchmarks
    {

        private global::Glob.Glob _glob;

        private List<string> _testData;

        [Setup]
        public void SetupData()
        {
            _testData = new List<string>(NumberOfMatches);
            var tokens = new GlobTokeniser().Tokenise(GlobPattern);
            var generator = new GlobMatchStringGenerator(tokens);

            for (int i = 0; i < 10000; i++)
            {
                _testData.Add(generator.GenerateRandomMatch());
            }

            _glob = new global::Glob.Glob(GlobPattern);
        }

        [Params(1, 10, 100, 500, 1000, 10000)]
        public int NumberOfMatches { get; set; }

        [Params("p?th/a[e-g].txt",
                "p?th/a[bcd]b[e-g].txt",
                "p?th/a[bcd]b[e-g]a[1-4][!wxyz][!a-c][!1-3].txt")]
        public string GlobPattern { get; set; }

        //[Benchmark]
        //public global::Glob.Glob Parse()
        //{
        //    return new global::Glob.Glob(GlobPattern, GlobOptions.Compiled);
        //}

        [Benchmark]
        public List<bool> IsMatch()
        {
            // we collect all results in a list and return it to prevent dead code elimination (optimisation)
            var results = new List<bool>(NumberOfMatches);
            for (int i = 0; i < NumberOfMatches; i++)
            {
                var testString = _testData[i];
                var result = _glob.IsMatch(testString);
                results.Add(result);
            }
            return results;
        }

    }
}