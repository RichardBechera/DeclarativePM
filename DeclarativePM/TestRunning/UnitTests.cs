using System;
using System.Collections.Generic;
using System.Linq;
using DeclarativePM.Lib.Declare_Templates;
using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;
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
        public void TestEventualEvaluation()
        {
            List<Event> eventsAHolds = new List<Event>()
            {
                new("b", "1"),
                new("b", "1"),
                new("a", "1")
            };
            
            List<Event> eventsANotHolds = new List<Event>()
            {
                new("b", "1"),
                new("b", "1"),
                new("b", "1"),
            };

            //eventual(b)
            LtlExpression exprA = new LtlExpression(Operators.Eventual,
                new LtlExpression("a"));

            Assert.True(MainMethods.EvaluateExpression(eventsAHolds, exprA));
            Assert.False(MainMethods.EvaluateExpression(eventsANotHolds, exprA));
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
            //This is true as there really was no other "a" until "c". => postprocessing later
            Assert.True(MainMethods.EvaluateExpression(eventsNotFinish, template.GetExpression()));
        }
        
        [Fact]
        public void TestAlternatePrecedenceEvaluate()
        {
            List<Event> eventsHolds = new List<Event>()
            {
                new("a", "1"),
                new("b", "1")
            };
            
            List<Event> eventsNotHolds = new List<Event>()
            {
                new("a", "1"),
                new("c", "1"),
                new("b", "1"),
                new("b", "1"),
            };
            
            List<Event> eventsANotOccurs = new List<Event>()
            {
                new("c", "1"),
                new("b", "1")
            };
            
            List<Event> eventsBNotOccurs = new List<Event>()
            {
                new("a", "1"),
                new("c", "1")
            };

            AlternatePrecedence template = new AlternatePrecedence("a", "b");

            //Algorithm from paper cant deal with b being last in the log
            Assert.False(MainMethods.EvaluateExpression(eventsHolds, template.GetExpression()));
            
            //with preprocessing 
            eventsHolds = UtilMethods.PreprocessTraceForEvaluation(template, eventsHolds);
            Assert.True(MainMethods.EvaluateExpression(eventsHolds, template.GetExpression()));
            
            Assert.False(MainMethods.EvaluateExpression(eventsNotHolds, template.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(eventsANotOccurs, template.GetExpression()));
            Assert.True(MainMethods.EvaluateExpression(eventsBNotOccurs, template.GetExpression()));
        }
        
        [Fact]
        public void TestAlternateSuccessionEvaluate()
        {
            List<Event> eventsHolds = new List<Event>()
            {
                new("a", "1"),
                new("c", "1"),
                new("c", "1"),
                new("b", "1"),
            };
            
            List<Event> eventsNotHolds = new List<Event>()
            {
                new("a", "1"),
                new("a", "1"),
                new("c", "1"),
                new("b", "1"),
            };
            
            List<Event> eventsNotFinish = new List<Event>()
            {
                new("a", "1"),
                new("c", "1"),
                new("c", "1"),
            };
            
            List<Event> eventsOnlyB = new List<Event>()
            {
                new("b", "1")
            };

            AlternateSuccession template = new AlternateSuccession("a", "b");
    
            ////Algorithm from paper cant deal with b being last in the log
            Assert.False(MainMethods.EvaluateExpression(eventsHolds, template.GetExpression()));
            
            Assert.False(MainMethods.EvaluateExpression(eventsNotHolds, template.GetExpression()));
            //This is true as there really was no other "a" until "b". => postprocessing later
            Assert.True(MainMethods.EvaluateExpression(eventsNotFinish, template.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(eventsOnlyB, template.GetExpression()));
            
            //with preprocessing
            eventsHolds = UtilMethods.PreprocessTraceForEvaluation(template, eventsHolds);
            Assert.True(MainMethods.EvaluateExpression(eventsHolds, template.GetExpression()));

            eventsNotHolds = UtilMethods.PreprocessTraceForEvaluation(template, eventsNotHolds);
            Assert.False(MainMethods.EvaluateExpression(eventsNotHolds, template.GetExpression()));

            //This is true as there really was no other "a" until "b". => postprocessing later
            eventsNotFinish = UtilMethods.PreprocessTraceForEvaluation(template, eventsNotFinish);
            Assert.True(MainMethods.EvaluateExpression(eventsNotFinish, template.GetExpression()));

            eventsOnlyB = UtilMethods.PreprocessTraceForEvaluation(template, eventsOnlyB);
            Assert.False(MainMethods.EvaluateExpression(eventsOnlyB, template.GetExpression()));
        }
        
        [Fact]
        public void TestChainResponseEvaluate()
        {
            List<Event> eventsHolds = new List<Event>()
            {
                new("a", "1"),
                new("b", "1")
            };
            
            List<Event> eventsNotHolds = new List<Event>()
            {
                new("a", "1"),
                new("c", "1"),
                new("b", "1")
            };
            
            List<Event> eventsNotFinish = new List<Event>()
            {
                new("a", "1")
            };
            
            List<Event> eventsRepeats = new List<Event>()
            {
                new("a", "1"),
                new("b", "1"),
                new("c", "1"),
                new("a", "1"),
                new("c", "1"),
            };

            ChainResponse template = new ChainResponse("a", "b");

            Assert.True(MainMethods.EvaluateExpression(eventsHolds, template.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(eventsNotHolds, template.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(eventsNotFinish, template.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(eventsRepeats, template.GetExpression()));
        }
        
        [Fact]
        public void TestChainPrecedenceEvaluate()
        {
            List<Event> eventsHolds = new List<Event>()
            {
                new("a", "1"),
                new("b", "1"),
            };
            
            List<Event> eventsNotHolds = new List<Event>()
            {
                new("b", "1"),
                new("a", "1")
            };
            
            List<Event> eventsFar = new List<Event>()
            {
                new("a", "1"),
                new("c", "1"),
                new("b", "1"),
            };

            ChainPrecedence template = new ChainPrecedence("a", "b");

            Assert.True(MainMethods.EvaluateExpression(eventsHolds, template.GetExpression()));
            //Algorithm from paper cant deal with b being first in the log => preprocessing
            Assert.True(MainMethods.EvaluateExpression(eventsNotHolds, template.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(eventsFar, template.GetExpression()));
            
            //with preprocessing
            eventsNotHolds = UtilMethods.PreprocessTraceForEvaluation(template, eventsNotHolds);
            Assert.False(MainMethods.EvaluateExpression(eventsNotHolds, template.GetExpression()));
        }
        
        [Fact]
        public void TestChainSuccessionEvaluate()
        {
            List<Event> eventsHolds = new List<Event>()
            {
                new("a", "1"),
                new("b", "1")
            };
            
            List<Event> eventsNotHolds = new List<Event>()
            {
                new("b", "1"),
                new("a", "1")
            };
            
            List<Event> eventsOnlyA = new List<Event>()
            {
                new("a", "1")
            };
            
            List<Event> eventsOnlyB = new List<Event>()
            {
                new("b", "1")
            };

            ChainSuccession template = new ChainSuccession("a", "b");

            Assert.True(MainMethods.EvaluateExpression(eventsHolds, template.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(eventsNotHolds, template.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(eventsOnlyA, template.GetExpression()));
            //Algorithm from paper cant deal with b being first in the log => preprocessing
            Assert.True(MainMethods.EvaluateExpression(eventsOnlyB, template.GetExpression()));
            
            //with preprocessing
            eventsOnlyB = UtilMethods.PreprocessTraceForEvaluation(template, eventsOnlyB);
            Assert.False(MainMethods.EvaluateExpression(eventsOnlyB, template.GetExpression()));
        }
        
        [Fact]
        public void TestResponseEvaluate()
        {
            List<Event> eventsHolds = new List<Event>()
            {
                new("a", "1"),
                new("c", "1"),
                new("c", "1"),
                new("b", "1"),
            };
            
            List<Event> eventsNotHolds = new List<Event>()
            {
                new("a", "1"),
                new("c", "1"),
                new("c", "1"),
            };

            Response template = new Response("a", "b");

            Assert.True(MainMethods.EvaluateExpression(eventsHolds, template.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(eventsNotHolds, template.GetExpression()));
        }
        
        [Fact]
        public void TestPrecedenceEvaluate()
        {
            List<Event> eventsHolds = new List<Event>()
            {
                new("a", "1"),
                new("c", "1"),
                new("c", "1"),
                new("b", "1"),
            };
            
            List<Event> eventsStarts = new List<Event>()
            {
                new("b", "1")
            };
            
            List<Event> eventsNotPreceded = new List<Event>()
            {
                new("c", "1"),
                new("c", "1"),
                new("b", "1"),
            };

            Precedence template = new Precedence("a", "b");

            Assert.True(MainMethods.EvaluateExpression(eventsHolds, template.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(eventsStarts, template.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(eventsNotPreceded, template.GetExpression()));
        }
        
        [Fact]
        public void TestSuccessionEvaluate()
        {
            List<Event> eventsHolds = new List<Event>()
            {
                new("a", "1"),
                new("c", "1"),
                new("c", "1"),
                new("b", "1"),
            };
            
            List<Event> eventsNotHolds = new List<Event>()
            {
                new("a", "1"),
                new("c", "1"),
                new("c", "1"),
            };

            Succession template = new Succession("a", "b");

            Assert.True(MainMethods.EvaluateExpression(eventsHolds, template.GetExpression()));
            //fail not vacuously
            Assert.False(MainMethods.EvaluateExpression(eventsNotHolds, template.GetExpression()));
        }
        
        [Fact]
        public void TestRespondedExistenceEvaluate()
        {
            List<Event> eventsAFirst = new List<Event>()
            {
                new("a", "1"),
                new("c", "1"),
                new("c", "1"),
                new("b", "1"),
            };
            
            List<Event> eventsBFirst = new List<Event>()
            {
                new("b", "1"),
                new("c", "1"),
                new("c", "1"),
                new("a", "1"),
            };
            
            List<Event> eventsBNotOccurs = new List<Event>()
            {
                new("a", "1"),
                new("c", "1"),
                new("c", "1"),
            };

            RespondedExistence template = new RespondedExistence("a", "b");

            Assert.True(MainMethods.EvaluateExpression(eventsAFirst, template.GetExpression()));
            Assert.True(MainMethods.EvaluateExpression(eventsBFirst, template.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(eventsBNotOccurs, template.GetExpression()));
        }
        
        [Fact]
        public void TestNotSuccessionEvaluate()
        {
            List<Event> eventsABeforeB = new List<Event>()
            {
                new("a", "1"),
                new("c", "1"),
                new("c", "1"),
                new("b", "1"),
            };
            
            List<Event> eventsNoB = new List<Event>()
            {
                new("a", "1"),
                new("c", "1"),
                new("c", "1")
            };
            
            List<Event> eventsBFirst = new List<Event>()
            {
                new("b", "1"),
                new("c", "1"),
                new("a", "1"),
            };

            NotSuccession template = new NotSuccession("a", "b");

            Assert.False(MainMethods.EvaluateExpression(eventsABeforeB, template.GetExpression()));
            Assert.True(MainMethods.EvaluateExpression(eventsNoB, template.GetExpression()));
            Assert.True(MainMethods.EvaluateExpression(eventsBFirst, template.GetExpression()));
        }
        
        [Fact]
        public void TestNotChainSuccessionEvaluate()
        {
            List<Event> eventsHolds = new List<Event>()
            {
                new("a", "1"),
                new("c", "1"),
                new("c", "1"),
                new("b", "1"),
            };
            
            List<Event> eventsNotHolds = new List<Event>()
            {
                new("a", "1"),
                new("b", "1")
            };
            
            List<Event> eventsNotFinish = new List<Event>()
            {
                new("a", "1"),
                new("c", "1")
            };

            NotChainSuccession template = new NotChainSuccession("a", "b");

            Assert.True(MainMethods.EvaluateExpression(eventsHolds, template.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(eventsNotHolds, template.GetExpression()));
            Assert.True(MainMethods.EvaluateExpression(eventsNotFinish, template.GetExpression()));
        }
        
        [Fact]
        public void TestNotCoexistenceEvaluate()
        {
            List<Event> eventsBoth = new List<Event>()
            {
                new("a", "1"),
                new("c", "1"),
                new("c", "1"),
                new("b", "1"),
            };
            
            List<Event> eventsOnlyA = new List<Event>()
            {
                new("a", "1"),
                new("c", "1"),
                new("c", "1"),
            };
            
            List<Event> eventsOnlyB = new List<Event>()
            {
                new("b", "1"),
                new("c", "1"),
                new("c", "1"),
            };

            NotCoexistence template = new NotCoexistence("a", "b");

            Assert.False(MainMethods.EvaluateExpression(eventsBoth, template.GetExpression()));
            Assert.True(MainMethods.EvaluateExpression(eventsOnlyA, template.GetExpression()));
            Assert.True(MainMethods.EvaluateExpression(eventsOnlyB, template.GetExpression()));
        }
        
        [Fact]
        public void TestInitEvaluate()
        {
            List<Event> eventsHolds = new List<Event>()
            {
                new("a", "1"),
                new("b", "1"),
            };
            
            List<Event> eventsNotHolds = new List<Event>()
            {
                new("b", "1"),
                new("a", "1"),
            };

            Init template = new Init("a");

            Assert.True(MainMethods.EvaluateExpression(eventsHolds, template.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(eventsNotHolds, template.GetExpression()));
        }
        
        [Fact]
        public void TestExistenceEvaluate()
        {
            List<Event> eventsThree = new List<Event>()
            {
                new("a", "1"),
                new("c", "1"),
                new("a", "1"),
                new("a", "1"),
            };
            
            List<Event> eventsZero = new List<Event>()
            {
                new("c", "1"),
                new("c", "1")
            };
            
            List<Event> eventsOnce = new List<Event>()
            {
                new("b", "1"),
                new("a", "1"),
                new("b", "1"),
            };

            Existence templateOne = new Existence(1, "a");
            Existence templateThree = new Existence(3, "a");
            Existence templateZero = new Existence(0, "a");

            Assert.True(MainMethods.EvaluateExpression(eventsThree, templateOne.GetExpression()));
            Assert.True(MainMethods.EvaluateExpression(eventsThree, templateThree.GetExpression()));
            Assert.True(MainMethods.EvaluateExpression(eventsThree, templateZero.GetExpression()));
            
            Assert.False(MainMethods.EvaluateExpression(eventsZero, templateOne.GetExpression()));
            Assert.True(MainMethods.EvaluateExpression(eventsZero, templateZero.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(eventsZero, templateThree.GetExpression()));
            
            Assert.True(MainMethods.EvaluateExpression(eventsOnce, templateOne.GetExpression()));
            Assert.True(MainMethods.EvaluateExpression(eventsOnce, templateZero.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(eventsOnce, templateThree.GetExpression()));
        }
        
        [Fact]
        public void TestExactlyEvaluate()
        {
            List<Event> eventsThree = new List<Event>()
            {
                new("a", "1"),
                new("c", "1"),
                new("a", "1"),
                new("a", "1"),
            };
            
            List<Event> eventsZero = new List<Event>()
            {
                new("c", "1"),
                new("c", "1")
            };
            
            List<Event> eventsOnce = new List<Event>()
            {
                new("b", "1"),
                new("a", "1"),
                new("b", "1"),
            };

            Exactly templateOne = new Exactly(1, "a");
            Exactly templateThree = new Exactly(3, "a");
            Exactly templateZero = new Exactly(0, "a");

            Assert.False(MainMethods.EvaluateExpression(eventsThree, templateOne.GetExpression()));
            Assert.True(MainMethods.EvaluateExpression(eventsThree, templateThree.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(eventsThree, templateZero.GetExpression()));
            
            Assert.False(MainMethods.EvaluateExpression(eventsZero, templateOne.GetExpression()));
            Assert.True(MainMethods.EvaluateExpression(eventsZero, templateZero.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(eventsZero, templateThree.GetExpression()));
            
            Assert.True(MainMethods.EvaluateExpression(eventsOnce, templateOne.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(eventsOnce, templateZero.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(eventsOnce, templateThree.GetExpression()));
        }
        
        [Fact]
        public void TestCoexistenceEvaluate()
        {
            List<Event> eventsBoth = new List<Event>()
            {
                new("a", "1"),
                new("c", "1"),
                new("c", "1"),
                new("b", "1"),
            };
            
            List<Event> eventsOnlyA = new List<Event>()
            {
                new("a", "1"),
                new("c", "1"),
                new("c", "1"),
            };
            
            List<Event> eventsOnlyB = new List<Event>()
            {
                new("b", "1"),
                new("c", "1"),
                new("c", "1"),
            };

            Coexistence template = new Coexistence("a", "b");

            Assert.True(MainMethods.EvaluateExpression(eventsBoth, template.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(eventsOnlyA, template.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(eventsOnlyB, template.GetExpression()));
        }
        
        [Fact]
        public void TestAbsenceEvaluate()
        {
            List<Event> eventsThree = new List<Event>()
            {
                new("a", "1"),
                new("c", "1"),
                new("a", "1"),
                new("a", "1"),
            };
            
            List<Event> eventsZero = new List<Event>()
            {
                new("c", "1"),
                new("c", "1")
            };
            
            List<Event> eventsOnce = new List<Event>()
            {
                new("b", "1"),
                new("a", "1"),
                new("b", "1"),
            };

            Absence templateOne = new Absence(1, "a");
            Absence templateTwo = new Absence(2, "a");
            
            Assert.Throws<ArgumentException>(() => new Absence(0, "a"));

            Assert.False(MainMethods.EvaluateExpression(eventsThree, templateOne.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(eventsThree, templateTwo.GetExpression()));
            
            Assert.True(MainMethods.EvaluateExpression(eventsZero, templateOne.GetExpression()));
            Assert.True(MainMethods.EvaluateExpression(eventsZero, templateTwo.GetExpression()));
            
            Assert.False(MainMethods.EvaluateExpression(eventsOnce, templateOne.GetExpression()));
            Assert.True(MainMethods.EvaluateExpression(eventsOnce, templateTwo.GetExpression()));
        }

    }
}