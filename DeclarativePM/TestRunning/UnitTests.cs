using System.Collections.Generic;
using System.Linq;
using DeclarativePM.Lib.Declare_Templates;
using DeclarativePM.Lib.Discovery;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Import;
using DeclarativePM.Lib.Models;
using DeclarativePM.Lib.Utils;
using Xunit;

namespace TestRunning
{
    public class UnitTests
    {
        private ImportCsvLogs _importer = new();
        private Discovery _disco = new();
        
        [Fact]
        public void Test1()
        {
            var path4 = "/home/richard/Documents/bakalarka/sampleData/bookExample1.csv";
            var third = _importer.LoadCsv(path4);
            var log2 = third.buildEventLog();
            var tree = ActivationTreeBuilder.BuildTree(log2.logs, 
                new Response("C", "S"));
            
            var ful = MainMethods.GetFulfillment(tree);
            Assert.True(ful.Exists(x => x.Activity == "C" && x.ActivityInTraceId == 1));
            
            var vio = MainMethods.GetViolation(tree);
            Assert.True(vio.Exists(x => x.Activity == "C" && x.ActivityInTraceId == 3));
            
            var conf = MainMethods.GetConflict(tree);
            Assert.Empty(conf);
        }
        
        [Fact]
        public void Test2()
        {
            var path4 = "/home/richard/Documents/bakalarka/sampleData/bookExample2.csv";
            var third = _importer.LoadCsv(path4);
            var log2 = third.buildEventLog();
            var tree = ActivationTreeBuilder.BuildTree(log2.logs, 
                new AlternateResponse("H", "M"));
            
            var ful = MainMethods.GetFulfillment(tree);
            Assert.True(ful.Exists(x => x.Activity == "H" && x.ActivityInTraceId == 1));
            
            var vio = MainMethods.GetViolation(tree);
            Assert.Empty(vio);
            
            var conf = MainMethods.GetConflict(tree);
            Assert.True(conf.Exists(x => x.Activity == "H" && x.ActivityInTraceId == 3));
            Assert.True(conf.Exists(x => x.Activity == "H" && x.ActivityInTraceId == 4));
        }
        
        [Fact]
        public void Test3()
        {
            var path4 = "/home/richard/Documents/bakalarka/sampleData/bookExample3.csv";
            var third = _importer.LoadCsv(path4);
            var log2 = third.buildEventLog();
            var tree = ActivationTreeBuilder.BuildTree(log2.logs, 
                new NotCoexistence("L", "H"));
            
            var ful = MainMethods.GetFulfillment(tree);
            Assert.Empty(ful);
            
            var vio = MainMethods.GetViolation(tree);
            Assert.Empty(vio);
            
            var conf = MainMethods.GetConflict(tree);
            Assert.True(conf.Exists(x => x.Activity == "H" && x.ActivityInTraceId == 1));
            Assert.True(conf.Exists(x => x.Activity == "L" && x.ActivityInTraceId == 3));
            Assert.True(conf.Exists(x => x.Activity == "L" && x.ActivityInTraceId == 4));
        }

        [Fact]
        public void Test4()
        {
            var path4 = "/home/richard/Documents/bakalarka/sampleData/bookExample3.csv";
            var third = _importer.LoadCsv(path4);
            var log2 = third.buildEventLog();
            var tree = ActivationTreeBuilder.BuildTree(log2.logs, 
                new NotCoexistence("L", "H"));

            var result = MainMethods.LocalLikelyhood(tree);
            Assert.Equal(2, result.Count);
            Assert.Equal(result.First().Value, 2/(double)3);
            Assert.Equal(result.Last().Value, 1/(double)3);
        }
        
        [Fact]
        public void Test5()
        {
            Assert.True(false);
            var path4 = "/home/richard/Documents/bakalarka/sampleData/bookExample3.csv";
            var third = _importer.LoadCsv(path4);
            var log2 = third.buildEventLog();
            var tree = ActivationTreeBuilder.BuildTree(log2.logs, 
                new NotCoexistence("L", "H"));

            List<ParametrisedTemplate> templates = new List<ParametrisedTemplate>()
            {
                new(TemplateInstanceType.NotCoexistence, new List<ITemplate>()
                {
                    new NotCoexistence("L", "H"),
                }),
                new(TemplateInstanceType.AlternateResponse, new List<ITemplate>()
                {
                    new AlternateResponse("H", "M"),
                }),
            };
            var cr = MainMethods.GetConflictNodes(tree);

            Assert.Equal(2, cr.Count);
            
            var gl1 = MainMethods.GlobalLikelyhood(tree, templates, cr.First());
            //Assert.Equal(0, gl1);
            
            var gl2 = MainMethods.GlobalLikelyhood(tree, templates, cr.Last());
            //Assert.Equal(1/(double)6, gl2);
        }
        
        
    }
}