using System;
using System.Collections.Generic;
using System.Linq;
using DeclarativePM.Lib.Declare_Templates;
using DeclarativePM.Lib.Discovery;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Import;
using DeclarativePM.Lib.Models;
using DeclarativePM.Lib.Models.DeclareModels;
using DeclarativePM.Lib.Utils;
using Xunit;

namespace TestRunning
{
    public class UnitTests
    {
        private Discovery _disco = new();
        
        [Fact]
        public void Test1()
        {
            var path4 = "/home/richard/Documents/bakalarka/sampleData/bookExample1.csv";
            var third = ImportCsvLogs.LoadCsv(path4);
            var log2 = third.buildEventLog();
            var tree = ActivationTreeBuilder.BuildTree(log2.Logs, 
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
            //TODO relative path
            var path4 = "/home/richard/Documents/bakalarka/sampleData/bookExample2.csv";
            var third = ImportCsvLogs.LoadCsv(path4);
            var log2 = third.buildEventLog();
            var tree = ActivationTreeBuilder.BuildTree(log2.Logs, 
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
            var third = ImportCsvLogs.LoadCsv(path4);
            var log2 = third.buildEventLog();
            var tree = ActivationTreeBuilder.BuildTree(log2.Logs, 
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
            var third = ImportCsvLogs.LoadCsv(path4);
            var log2 = third.buildEventLog();
            var tree = ActivationTreeBuilder.BuildTree(log2.Logs, 
                new NotCoexistence("L", "H"));

            var result = MainMethods.LocalLikelihood(tree);
            Assert.Equal(2, result.Count);
            Assert.Equal(result.First().Value, 2/(double)3);
            Assert.Equal(result.Last().Value, 1/(double)3);
        }
        
        [Fact]
        public void Test5()
        {
            Assert.True(false);
            var path4 = "/home/richard/Documents/bakalarka/sampleData/bookExample3.csv";
            var third = ImportCsvLogs.LoadCsv(path4);
            var log2 = third.buildEventLog();
            var tree = ActivationTreeBuilder.BuildTree(log2.Logs, 
                new NotCoexistence("L", "H"));

            List<ParametrizedTemplate> templates = new List<ParametrizedTemplate>()
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
            
            var gl1 = MainMethods.GlobalLikelihood(tree, templates, cr.First());
            //Assert.Equal(0, gl1);
            
            var gl2 = MainMethods.GlobalLikelihood(tree, templates, cr.Last());
            //Assert.Equal(1/(double)6, gl2);
        }
        
        [Fact]
        public void Test6()
        {
            var path4 = "/home/richard/Documents/bakalarka/sampleData/bookExample3.csv";
            var third = ImportCsvLogs.LoadCsv(path4);
            var log = third.buildEventLog();
            var model = _disco.DiscoverModel(log, new List<ParametrizedTemplate>()
            {
                new ParametrizedTemplate(TemplateInstanceType.Coexistence),
                new ParametrizedTemplate(TemplateInstanceType.ChainPrecedence),
                new ParametrizedTemplate(TemplateInstanceType.NotCoexistence),
                new ParametrizedTemplate(TemplateInstanceType.Precedence),
                new ParametrizedTemplate(TemplateInstanceType.AlternatePrecedence),
                new ParametrizedTemplate(TemplateInstanceType.ChainSuccession),
                new ParametrizedTemplate(TemplateInstanceType.ChainResponse),
                new ParametrizedTemplate(TemplateInstanceType.AlternateSuccession),
            });
            Assert.Equal(6, model.Constraints[0].TemplateInstances.Count);
            Assert.Equal(6, model.Constraints[0].TemplateInstances.Count);

        }
        
        [Fact]
        public void TestLeastEvaluation()
        {
            List<Event> eventsAHolds = new List<Event>()
            {
                new("a", "1"),
                new("a", "1"),
                new("a", "1"),
                new("a", "1"),
                new("b", "1"),
            };
            
            List<Event> eventsANotHolds = new List<Event>()
            {
                new("a", "1"),
                new("a", "1"),
                new("c", "1"),
                new("a", "1"),
                new("b", "1"),
            };

            //a U b
            LtlExpression expr = new LtlExpression(Operators.Least,
                new LtlExpression("a"),
                new LtlExpression("b"));
            
            //!c U b
            LtlExpression exprNotC = new LtlExpression(Operators.Least,
                new LtlExpression(Operators.Not, 
                    new LtlExpression("c")),
                new LtlExpression("b"));

            Assert.True(MainMethods.EvaluateExpression(eventsAHolds, expr));
            Assert.False(MainMethods.EvaluateExpression(eventsANotHolds, expr));
            Assert.True(MainMethods.EvaluateExpression(eventsAHolds, exprNotC));
            Assert.False(MainMethods.EvaluateExpression(eventsANotHolds, exprNotC));
        }
        
        
        [Fact]
        public void TestNextEvaluation()
        {
            List<Event> eventsNextB = new List<Event>()
            {
                new("a", "1"),
                new("b", "1")
            };

            //next(a)
            LtlExpression exprA = new LtlExpression(Operators.Next,
                new LtlExpression("a"));
            
            //next(!a)
            LtlExpression exprNotA = new LtlExpression(Operators.Next,
                new LtlExpression(Operators.Not, new LtlExpression("a")));
            
            //next(b)
            LtlExpression exprB = new LtlExpression(Operators.Next,
                new LtlExpression("b"));

            Assert.True(MainMethods.EvaluateExpression(eventsNextB, exprB));
            Assert.True(MainMethods.EvaluateExpression(eventsNextB, exprNotA));
            Assert.False(MainMethods.EvaluateExpression(eventsNextB, exprA));
        }
        
        [Fact]
        public void TestSubsequentEvaluation()
        {
            List<Event> eventsAHolds = new List<Event>()
            {
                new("a", "1"),
                new("a", "1"),
                new("a", "1"),
            };
            
            List<Event> eventsANotHolds = new List<Event>()
            {
                new("a", "1"),
                new("a", "1"),
                new("c", "1")
            };

            //subsequent(a)
            LtlExpression exprA = new LtlExpression(Operators.Subsequent,
                new LtlExpression("a"));
            
            //subsequent(!c)
            LtlExpression exprNotC = new LtlExpression(Operators.Next,
                new LtlExpression(Operators.Not, new LtlExpression("c")));

            Assert.True(MainMethods.EvaluateExpression(eventsAHolds, exprA));
            Assert.False(MainMethods.EvaluateExpression(eventsANotHolds, exprA));
            Assert.True(MainMethods.EvaluateExpression(eventsAHolds, exprNotC));
        }
        
        [Fact]
        public void TestAlternateResponseEvaluate()
        {
            List<Event> eventsHolds = new List<Event>()
            {
                new("a", "1"),
                new("b", "1"),
                new("b", "1"),
                new("c", "1"),
            };
            
            List<Event> eventsNotHolds = new List<Event>()
            {
                new("a", "1"),
                new("b", "1"),
                new("a", "1"),
                new("c", "1"),
            };
            
            List<Event> eventsNotFinish = new List<Event>()
            {
                new("a", "1"),
                new("b", "1"),
                new("b", "1"),
            };

            AlternateResponse template = new AlternateResponse("a", "c");

            Assert.True(MainMethods.EvaluateExpression(eventsHolds, template.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(eventsNotHolds, template.GetExpression()));
            //This is true as there really was no other "a" until "c".
            Assert.True(MainMethods.EvaluateExpression(eventsNotFinish, template.GetExpression()));
        }
        
        

    }
}