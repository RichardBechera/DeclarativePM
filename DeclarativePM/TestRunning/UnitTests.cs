using System;
using System.Collections.Generic;
using System.Linq;
using DeclarativePM.Lib.Declare_Templates;
using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;
using DeclarativePM.Lib.Discovery;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.IO.Export;
using DeclarativePM.Lib.IO.Import;
using DeclarativePM.Lib.Models;
using DeclarativePM.Lib.Models.DeclareModels;
using DeclarativePM.Lib.Utils;
using Xunit;

namespace TestRunning
{
    public class UnitTests
    {
        private Discovery _disco = new();
        Importer importer = new Importer();
        
        private readonly List<Event> _eventsNotActivated = new()
        {
            new("c", "1"),
            new("c", "1"),
            new("c", "1"),
        };
        
        private readonly List<Event> _eventsANotOccurs = new()
        {
            new("c", "1"),
            new("b", "1")
        };
            
        private readonly List<Event> _eventsBNotOccurs = new()
        {
            new("a", "1"),
            new("c", "1"),
            new("c", "1")
        };

        /// <summary>
        /// Checks whether vacuity detection
        /// </summary>
        /// <param name="template"></param>
        /// <param name="activation">0 - both A and B are activators, 1 - A activates, 2 - B activates</param>
        private void CheckVacuity(BiTemplate template, short activation)
        {
            if (0 > activation || activation > 2)
                throw new ArgumentException("Only values from 0 to 2 are allowed as activation");
            
            if (activation == 0)
            {
                Assert.True(MainMethods.EvaluateExpression(_eventsNotActivated, template.GetExpression()));
                Assert.False(MainMethods.EvaluateExpression(_eventsNotActivated, template.GetWitnessExpression()));
            }
            else if (activation == 1)
            {
                Assert.True(MainMethods.EvaluateExpression(_eventsANotOccurs, template.GetExpression()));
                Assert.False(MainMethods.EvaluateExpression(_eventsANotOccurs, template.GetWitnessExpression()));
            }
            else
            {
                Assert.True(MainMethods.EvaluateExpression(_eventsBNotOccurs, template.GetExpression()));
                Assert.False(MainMethods.EvaluateExpression(_eventsBNotOccurs, template.GetWitnessExpression()));
            }
        }

        [Fact]
        public void Test1()
        {
            var path4 = "/home/richard/Documents/bakalarka/sampleData/bookExample1.csv";
            var third = importer.LoadCsv(path4);
            var log2 = third.BuildEventLog();
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
            var third = importer.LoadCsv(path4);
            var log2 = third.BuildEventLog();
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
            var third = importer.LoadCsv(path4);
            var log2 = third.BuildEventLog();
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
            var third = importer.LoadCsv(path4);
            var log2 = third.BuildEventLog();
            var tree = ActivationTreeBuilder.BuildTree(log2.Logs, 
                new NotCoexistence("L", "H"));

            var result = MainMethods.LocalLikelihood(tree);
            Assert.Equal(2, result.Count);
            Assert.Equal(result.First().Value, 2/(double)3);
            Assert.Equal(result.Last().Value, 1/(double)3);

            var cr = MainMethods.GetConflictNodes(tree);
            Assert.Equal(2/(double)3, MainMethods.LocalLikelihood(tree, cr.First()));
            Assert.Equal(1/(double)3, MainMethods.LocalLikelihood(tree, cr.Last()));
            
        }
        
        [Fact]
        public void Test5()
        {
            Assert.True(true);
            var path4 = "/home/richard/Documents/bakalarka/sampleData/bookExample3.csv";
            
            var third = importer.LoadCsv(path4);
            var log2 = third.BuildEventLog();
            NotCoexistence nc = new NotCoexistence("L", "H"); 
            var tree = ActivationTreeBuilder.BuildTree(log2.Logs, nc);

            List<ParametrizedTemplate> templates = new List<ParametrizedTemplate>()
            {
                new(TemplateInstanceType.AlternateResponse, new List<ITemplate>()
                {
                    new AlternateResponse("H", "M"),
                }),
                new(TemplateInstanceType.NotCoexistence, new List<ITemplate>(){nc})
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
            
            var third = importer.LoadCsv(path4);
            var log = third.BuildEventLog();
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
                new("c", "1"),
                new("c", "1"),
                new("b", "1"),
            };
            
            List<Event> eventsNotHolds = new List<Event>()
            {
                new("a", "1"),
                new("c", "1"),
                new("a", "1"),
                new("b", "1"),
            };

            AlternateResponse template = new AlternateResponse("a", "b");

            Assert.True(MainMethods.EvaluateExpression(eventsHolds, template.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(eventsNotHolds, template.GetExpression()));
            //This is true as there really was no other "a" until "c". => postprocessing later
            Assert.True(MainMethods.EvaluateExpression(_eventsBNotOccurs, template.GetExpression()));
            
            CheckVacuity(template, 1);
            
            Assert.True(MainMethods.EvaluateExpression(eventsHolds, template.GetFinishingExpression()));
            Assert.False(MainMethods.EvaluateExpression(eventsNotHolds, template.GetFinishingExpression()));
            Assert.False(MainMethods.EvaluateExpression(_eventsBNotOccurs, template.GetFinishingExpression()));
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

            AlternatePrecedence template = new AlternatePrecedence("a", "b");

            //Algorithm from paper cant deal with b being last in the log
            Assert.False(MainMethods.EvaluateExpression(eventsHolds, template.GetExpression()));
            
            //with preprocessing 
            eventsHolds = UtilMethods.PreprocessTraceForEvaluation(template, eventsHolds);
            Assert.True(MainMethods.EvaluateExpression(eventsHolds, template.GetExpression()));
            
            Assert.False(MainMethods.EvaluateExpression(eventsNotHolds, template.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(_eventsANotOccurs, template.GetExpression()));
            
            CheckVacuity(template, 2);
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

            AlternateSuccession template = new AlternateSuccession("a", "b");
    
            ////Algorithm from paper cant deal with b being last in the log
            Assert.False(MainMethods.EvaluateExpression(eventsHolds, template.GetExpression()));
            
            Assert.False(MainMethods.EvaluateExpression(eventsNotHolds, template.GetExpression()));
            //This is true as there really was no other "a" until "b". => vacuity check
            Assert.True(MainMethods.EvaluateExpression(_eventsBNotOccurs, template.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(_eventsANotOccurs, template.GetExpression()));
            
            //finishing
            Assert.False(MainMethods.EvaluateExpression(_eventsBNotOccurs, template.GetFinishingExpression()));
            
            //with preprocessing
            eventsHolds = UtilMethods.PreprocessTraceForEvaluation(template, eventsHolds);
            Assert.True(MainMethods.EvaluateExpression(eventsHolds, template.GetExpression()));

            eventsNotHolds = UtilMethods.PreprocessTraceForEvaluation(template, eventsNotHolds);
            Assert.False(MainMethods.EvaluateExpression(eventsNotHolds, template.GetExpression()));

            //This is true as there really was no other "a" until "b". => postprocessing later
            List<Event> eventsNotFinish = UtilMethods.PreprocessTraceForEvaluation(template, _eventsBNotOccurs);
            Assert.True(MainMethods.EvaluateExpression(eventsNotFinish, template.GetExpression()));

            List<Event> eventsOnlyB = UtilMethods.PreprocessTraceForEvaluation(template, _eventsANotOccurs);
            Assert.False(MainMethods.EvaluateExpression(eventsOnlyB, template.GetExpression()));
            
            //finishing
            Assert.False(MainMethods.EvaluateExpression(eventsNotFinish, template.GetFinishingExpression()));
            
            CheckVacuity(template, 0);
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
            
            CheckVacuity(template, 1);
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
            
            CheckVacuity(template, 2);
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
            
            CheckVacuity(template, 0);
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

            Response template = new Response("a", "b");

            Assert.True(MainMethods.EvaluateExpression(eventsHolds, template.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(_eventsBNotOccurs, template.GetExpression()));
            
            CheckVacuity(template, 1);
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

            Precedence template = new Precedence("a", "b");

            Assert.True(MainMethods.EvaluateExpression(eventsHolds, template.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(eventsStarts, template.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(_eventsANotOccurs, template.GetExpression()));
            
            CheckVacuity(template, 2);
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

            Succession template = new Succession("a", "b");

            Assert.True(MainMethods.EvaluateExpression(eventsHolds, template.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(_eventsBNotOccurs, template.GetExpression()));
            
            CheckVacuity(template, 0);
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

            RespondedExistence template = new RespondedExistence("a", "b");

            Assert.True(MainMethods.EvaluateExpression(eventsAFirst, template.GetExpression()));
            Assert.True(MainMethods.EvaluateExpression(eventsBFirst, template.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(_eventsBNotOccurs, template.GetExpression()));
            
            CheckVacuity(template, 1);
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
            
            List<Event> eventsBFirst = new List<Event>()
            {
                new("b", "1"),
                new("c", "1"),
                new("a", "1"),
            };

            NotSuccession template = new NotSuccession("a", "b");

            Assert.False(MainMethods.EvaluateExpression(eventsABeforeB, template.GetExpression()));
            Assert.True(MainMethods.EvaluateExpression(_eventsBNotOccurs, template.GetExpression()));
            Assert.True(MainMethods.EvaluateExpression(eventsBFirst, template.GetExpression()));
            
            CheckVacuity(template, 0);
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

            NotChainSuccession template = new NotChainSuccession("a", "b");

            Assert.True(MainMethods.EvaluateExpression(eventsHolds, template.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(eventsNotHolds, template.GetExpression()));
            Assert.True(MainMethods.EvaluateExpression(_eventsBNotOccurs, template.GetExpression()));
            
            CheckVacuity(template, 0);
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

            NotCoexistence template = new NotCoexistence("a", "b");

            Assert.False(MainMethods.EvaluateExpression(eventsBoth, template.GetExpression()));
            Assert.True(MainMethods.EvaluateExpression(_eventsBNotOccurs, template.GetExpression()));
            Assert.True(MainMethods.EvaluateExpression(_eventsANotOccurs, template.GetExpression()));
            
            CheckVacuity(template, 0);
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

            Coexistence template = new Coexistence("a", "b");

            Assert.True(MainMethods.EvaluateExpression(eventsBoth, template.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(_eventsBNotOccurs, template.GetExpression()));
            Assert.False(MainMethods.EvaluateExpression(_eventsANotOccurs, template.GetExpression()));
            
            CheckVacuity(template, 0);
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

        [Fact]
        public void ExporTest()
        {
            var coexistenceP = new ParametrizedTemplate(TemplateInstanceType.Coexistence, new List<ITemplate>()
            {
                new Coexistence("A", "B"),
                new Coexistence("B", "C")
            });
            var notCoexistenceP = new ParametrizedTemplate(TemplateInstanceType.NotCoexistence, new List<ITemplate>()
            {
                new NotCoexistence("A", "B"),
                new NotCoexistence("B", "C")
            });
            var responseP = new ParametrizedTemplate(TemplateInstanceType.Response, new List<ITemplate>()
            {
                new Response("A", "B"),
                new Response("B", "C")
            });
            DeclareModel model = new DeclareModel("Default name", new()
            {
                coexistenceP,
                notCoexistenceP,
                responseP
            });
            responseP.OptionalConstraints.Add(responseP.TemplateInstances[0]);
            coexistenceP.OptionalConstraints.Add(coexistenceP.TemplateInstances[0]);
            coexistenceP.OptionalConstraints.Add(coexistenceP.TemplateInstances[1]);

            Exporter exporter = new Exporter();
            string json = exporter.ExportModel(model);
            var m = importer.LoadModelFromJsonString(json);

            //exporter.ExportSaveModelAsync(model, "/home/richard/Documents/bakalarka/garbage", "testmodel");
            //var m = importer.LoadModelFromJsonPath("/home/richard/Documents/bakalarka/garbage/testmodel.json");
            
            Assert.Equal(model.Name, m.Name);
            Assert.Equal(model.Constraints.Count, m.Constraints.Count);
            for (int i = 0;  i < model.Constraints.Count;  i++)
            {
                ParametrizedTemplate c1 = model.Constraints[i];
                ParametrizedTemplate c2 = m.Constraints[i];
                
                Assert.Equal(c1.Poe, c2.Poe);
                Assert.Equal(c1.Poi, c2.Poi);
                Assert.Equal(c1.CheckVacuously, c2.CheckVacuously);
                Assert.Equal(c1.TemplateDescription.TemplateType, c2.TemplateDescription.TemplateType);
                
                Assert.Equal(c1.TemplateInstances.Count, c1.TemplateInstances.Count);
                for (int j = 0; j < c1.TemplateInstances.Count; j++)
                {
                    ITemplate t1 = c1.TemplateInstances[j];
                    ITemplate t2 = c2.TemplateInstances[j];
                    
                    Assert.Equal(t1.GetExpression().ToString(), t2.GetExpression().ToString());
                }
                
                Assert.Equal(c1.OptionalConstraints.Count, c1.OptionalConstraints.Count);
                for (int j = 0; j < c1.OptionalConstraints.Count; j++)
                {
                    ITemplate t1 = c1.OptionalConstraints[j];
                    ITemplate t2 = c2.OptionalConstraints[j];
                    
                    Assert.Equal(t1.GetExpression().ToString(), t2.GetExpression().ToString());
                }
            }
        }

    }
}