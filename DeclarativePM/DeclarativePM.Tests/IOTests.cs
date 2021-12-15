using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DeclarativePM.Lib.Declare_Templates;
using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.IO.Export;
using DeclarativePM.Lib.IO.Import;
using DeclarativePM.Lib.Models.DeclareModels;
using DeclarativePM.Lib.Models.LogModels;
using Xunit;

namespace DeclarativePM.Tests
{
    public class IOTests
    {
        private readonly string _sampleDataLocation;
        private readonly char _pathSeparator;
        
        private readonly JsonModelImporter _jsonModelImporter = new();
        private readonly JsonModelExporter _jsonModelExporter = new();
        private readonly CsvLogImporter _csvLogImporter = new();

        private readonly DeclareModel _model;
        private readonly string _modelJson;

        public IOTests()
        {
            _modelJson = "{\"Name\":\"Default name\",\"Constraints\":[{\"TemplateType\":8,\"Poe\":100.0,\"Poi\":100.0," +
                         "\"CheckVacuously\":true,\"TemplateInstances\":[{\"LogEventA\":\"A\",\"LogEventB\":\"B\"," +
                         "\"Optional\":true},{\"LogEventA\":\"B\",\"LogEventB\":\"C\",\"Optional\":true}]}," +
                         "{\"TemplateType\":13,\"Poe\":100.0,\"Poi\":100.0,\"CheckVacuously\":true,\"TemplateInstances\"" +
                         ":[{\"LogEventA\":\"A\",\"LogEventB\":\"B\",\"Optional\":false},{\"LogEventA\":\"B\"," +
                         "\"LogEventB\":\"C\",\"Optional\":false}]},{\"TemplateType\":17,\"Poe\":100.0,\"Poi\":100.0," +
                         "\"CheckVacuously\":true,\"TemplateInstances\":[{\"LogEventA\":\"A\",\"LogEventB\":\"B\"," +
                         "\"Optional\":true},{\"LogEventA\":\"B\",\"LogEventB\":\"C\",\"Optional\":false}]}]}";
            
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
            
            _model = new DeclareModel("Default name", new()
            {
                coexistenceP,
                notCoexistenceP,
                responseP
            });
            
            responseP.OptionalConstraints.Add(responseP.TemplateInstances[0]);
            coexistenceP.OptionalConstraints.Add(coexistenceP.TemplateInstances[0]);
            coexistenceP.OptionalConstraints.Add(coexistenceP.TemplateInstances[1]);

            _pathSeparator = Path.DirectorySeparatorChar;
            _sampleDataLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + _pathSeparator + ".." 
                                  + _pathSeparator + ".." + _pathSeparator + ".." + _pathSeparator + ".." 
                                  + _pathSeparator + "SampleData";
        }
        
        [Fact]
        public void ExportImportModelWithSaveTest()
        {
            _jsonModelExporter.ExportSaveModelAsync(_model, _sampleDataLocation, "testmodel");
            
            Assert.True(File.Exists($"{_sampleDataLocation}/testmodel.json"));
            
            var model = _jsonModelImporter.LoadModel($"{_sampleDataLocation}/testmodel.json");
            
            Assert.NotNull(model);
            
            CompareModelToOriginal(model);
            
            //clean up
            File.Delete($"{_sampleDataLocation}/testmodel.json");
        }
        
        [Fact]
        public void ExportImportModelTest()
        {
            string json = _jsonModelExporter.ExportModel(_model);
            var model = _jsonModelImporter.LoadModelFromString(json);
            
            Assert.NotNull(model);
            
            CompareModelToOriginal(model);
        }
        
        [Fact]
        public void ImportModelTest()
        {
            var model = _jsonModelImporter.LoadModelFromString(_modelJson);
            
            Assert.NotNull(model);
            
            CompareModelToOriginal(model);
        }
        
        [Fact]
        public void ExportModelTest()
        {
            string json = _jsonModelExporter.ExportModel(_model);
            
            Assert.Equal(_modelJson, json);
        }

        [Fact]
        public void ImportLogTest()
        {
            string logLocation = _sampleDataLocation
                                 + _pathSeparator + "logs" + _pathSeparator + "importLogTest.csv";
            Assert.True(File.Exists(logLocation));

            ImportedEventLog log = _csvLogImporter.LoadLog(logLocation);
            
            Assert.True(log.Headers.Length == 5);
            Assert.Contains("Action", log.Headers);
            Assert.Contains("Case", log.Headers);
            Assert.Contains("Timestamp", log.Headers);
            Assert.Contains("Resource1", log.Headers);
            Assert.Contains("Resource2", log.Headers);
            EventLog eventlog = log.BuildEventLog();
            Assert.True(eventlog.Logs.Count == 9);
        }


        private void CompareModelToOriginal(DeclareModel model)
        {
            Assert.Equal(_model.Name, model.Name);
            Assert.Equal(_model.Constraints.Count, model.Constraints.Count);
            for (int i = 0;  i < _model.Constraints.Count;  i++)
            {
                ParametrizedTemplate c1 = _model.Constraints[i];
                ParametrizedTemplate c2 = model.Constraints[i];
                
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