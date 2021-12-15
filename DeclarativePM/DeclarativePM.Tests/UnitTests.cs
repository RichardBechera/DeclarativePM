using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DeclarativePM.Lib.Declare_Templates;
using DeclarativePM.Lib.Declare_Templates.AbstractClasses;
using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;
using DeclarativePM.Lib.Discovery;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.IO.Import;
using DeclarativePM.Lib.Models.ConformanceModels;
using DeclarativePM.Lib.Models.DeclareModels;
using DeclarativePM.Lib.Models.LogModels;
using DeclarativePM.Lib.Utils;
using Xunit;

namespace DeclarativePM.Tests
{
    public class UnitTests
    {
        private Discovery _disco = new();
        private CsvLogImporter _csvLogImporter = new();
        private ActivationTreeBuilder _builder = new();
        private ConformanceEvaluator _conformanceEvaluator = new();
        
        private readonly string _sampleDataLocation;
        private readonly char _pathSeparator;

        public UnitTests()
        {
            _pathSeparator = Path.DirectorySeparatorChar;
            _sampleDataLocation = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{_pathSeparator}.." +
                                  $"{_pathSeparator}..{_pathSeparator}..{_pathSeparator}..{_pathSeparator}" +
                                  $"SampleData{_pathSeparator}logs";
        }
        
        [Fact]
        public void Test1()
        {
            string path = $"{_sampleDataLocation}{_pathSeparator}bookExample1.csv";
            var third = _csvLogImporter.LoadLog(path);
            var log2 = third.BuildEventLog();
            var tree = _builder.BuildTree(log2.Logs, 
                new Response("C", "S"));
            
            var ful = _conformanceEvaluator.GetFulfillment(tree);
            Assert.True(ful.Exists(x => x.Activity == "C" && x.ActivityInTraceId == 1));
            
            var vio = _conformanceEvaluator.GetViolation(tree);
            Assert.True(vio.Exists(x => x.Activity == "C" && x.ActivityInTraceId == 3));
            
            var conf = _conformanceEvaluator.GetConflict(tree);
            Assert.Empty(conf);
        }
        
        [Fact]
        public void Test2()
        {
            //setup
            string path = $"{_sampleDataLocation}{_pathSeparator}bookExample2.csv";
            var third = _csvLogImporter.LoadLog(path);
            var log2 = third.BuildEventLog();
            
            //conformance activity tree
            var tree = _builder.BuildTree(log2.Logs, 
                new AlternateResponse("H", "M"));
            
            //check nodes
            var ful = _conformanceEvaluator.GetFulfillment(tree);
            Assert.True(ful.Exists(x => x.Activity == "H" && x.ActivityInTraceId == 1));
            
            var vio = _conformanceEvaluator.GetViolation(tree);
            Assert.Empty(vio);
            
            var conf = _conformanceEvaluator.GetConflict(tree);
            Assert.True(conf.Exists(x => x.Activity == "H" && x.ActivityInTraceId == 3));
            Assert.True(conf.Exists(x => x.Activity == "H" && x.ActivityInTraceId == 4));
        }
        
        [Fact]
        public void Test3()
        {
            string path = $"{_sampleDataLocation}{_pathSeparator}bookExample3.csv";
            var third = _csvLogImporter.LoadLog(path);
            var log2 = third.BuildEventLog();
            var tree = _builder.BuildTree(log2.Logs, 
                new NotCoexistence("L", "H"));
            
            var ful = _conformanceEvaluator.GetFulfillment(tree);
            Assert.Empty(ful);
            
            var vio = _conformanceEvaluator.GetViolation(tree);
            Assert.Empty(vio);
            
            var conf = _conformanceEvaluator.GetConflict(tree);
            Assert.True(conf.Exists(x => x.Activity == "H" && x.ActivityInTraceId == 1));
            Assert.True(conf.Exists(x => x.Activity == "L" && x.ActivityInTraceId == 3));
            Assert.True(conf.Exists(x => x.Activity == "L" && x.ActivityInTraceId == 4));
        }

        [Fact]
        public void Test4()
        {
            var path = $"{_sampleDataLocation}{_pathSeparator}bookExample3.csv";
            var third = _csvLogImporter.LoadLog(path);
            var log2 = third.BuildEventLog();
            var tree = _builder.BuildTree(log2.Logs, 
                new NotCoexistence("L", "H"));

            var result = _conformanceEvaluator.LocalLikelihood(tree);
            Assert.Equal(2, result.Count);
            Assert.Equal(result.First().Value, 2/(double)3);
            Assert.Equal(result.Last().Value, 1/(double)3);

            var cr = _conformanceEvaluator.GetConflictNodes(tree);
            Assert.Equal(2/(double)3, _conformanceEvaluator.LocalLikelihood(tree, cr.First()));
            Assert.Equal(1/(double)3, _conformanceEvaluator.LocalLikelihood(tree, cr.Last()));
            
        }
        
        [Fact]
        public void Test5()
        {
            Assert.True(true);
            var path4 = "/home/richard/Documents/bakalarka/sampleData/bookExample3.csv";
            
            var third = _csvLogImporter.LoadLog(path4);
            var log2 = third.BuildEventLog();
            NotCoexistence nc = new NotCoexistence("L", "H"); 
            var tree = _builder.BuildTree(log2.Logs, nc);

            List<ParametrizedTemplate> templates = new List<ParametrizedTemplate>()
            {
                new(TemplateInstanceType.AlternateResponse, new List<ITemplate>()
                {
                    new AlternateResponse("H", "M"),
                }),
                new(TemplateInstanceType.NotCoexistence, new List<ITemplate>(){nc})
            };
            var cr = _conformanceEvaluator.GetConflictNodes(tree);

            Assert.Equal(2, cr.Count);
            
            var gl1 = _conformanceEvaluator.GlobalLikelihood(tree, templates, cr.First());
            //Assert.Equal(0, gl1);
            
            var gl2 = _conformanceEvaluator.GlobalLikelihood(tree, templates, cr.Last());
            //Assert.Equal(1/(double)6, gl2);
        }
        
        [Fact]
        public void Test6()
        {
            string path = $"{_sampleDataLocation}{_pathSeparator}bookExample3.csv";
            
            var third = _csvLogImporter.LoadLog(path);
            var log = third.BuildEventLog();
            var model = _disco.DiscoverModel(log, new List<ParametrizedTemplate>()
            {
                new(TemplateInstanceType.Coexistence),
                new(TemplateInstanceType.ChainPrecedence),
                new(TemplateInstanceType.NotCoexistence),
                new(TemplateInstanceType.Precedence),
                new(TemplateInstanceType.AlternatePrecedence),
                new(TemplateInstanceType.ChainSuccession),
                new(TemplateInstanceType.ChainResponse),
                new(TemplateInstanceType.AlternateSuccession),
            });
            int[] result = {3, 1, 0, 3, 1, 1, 2, 1};
            for (int i = 0; i < 8; i++)
            {
                Assert.Equal(result[i], model.Constraints[i].TemplateInstances.Count);
            }

        }

        [Fact]
        public void responsetree()
        {
            List<Event> log = new()
            {
                new("A", "0"),
                new("B", "0"),
                new("A", "0"),
                new("A", "0"),
                new("B", "0"),
                
                new("A", "0"),
                new("A", "0"),
                new("B", "0"),

            };

            BiTemplate template = new ChainResponse("A", "B");

            ActivationBinaryTree tree = _builder.BuildTree(log, template);

            var allmaxFulfilling = tree.Leaves.Where(l => l.MaxFulfilling).ToList();

            Healthiness h = new Healthiness(tree);

        }

    }
}