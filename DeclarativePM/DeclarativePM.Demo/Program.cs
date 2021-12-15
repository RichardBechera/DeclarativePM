using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DeclarativePM.Lib.Declare_Templates.AbstractClasses;
using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;
using DeclarativePM.Lib.Discovery;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.IO.Import;
using DeclarativePM.Lib.Models.DeclareModels;
using DeclarativePM.Lib.Models.LogModels;
using DeclarativePM.Lib.Utils;

namespace DeclarativePM.Demo
{
    class Program
    {
        
        static void Main(string[] args)
        {
            char pathSeparator = Path.DirectorySeparatorChar;
            string sampleDataLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + pathSeparator + ".." 
                                  + pathSeparator + ".." + pathSeparator + ".." + pathSeparator + ".." 
                                  + pathSeparator + "SampleData";

            string pathToLog = $"{sampleDataLocation}{pathSeparator}logs{pathSeparator}importLogTest.csv";
            
            CsvLogImporter logImporter = new();
            Discovery discovery = new();
            ConformanceEvaluator conformanceEvaluator = new();

            ImportedEventLog importedEventLog = logImporter.LoadLog(pathToLog);

            string thirdColumnHeader = importedEventLog.Headers[2];
            
            importedEventLog.ChangeTimestamp(thirdColumnHeader);

            EventLog eventLog = importedEventLog.BuildEventLog("Test log");

            List<ParametrizedTemplate> templatesToDiscover = new List<ParametrizedTemplate>();

            ParametrizedTemplate exactly = new ParametrizedTemplate(TemplateInstanceType.Existence, 90, 90);
            ParametrizedTemplate respondedExistence = new ParametrizedTemplate(TemplateInstanceType.RespondedExistence);
            ParametrizedTemplate notChainSuccession = new ParametrizedTemplate(TemplateInstanceType.NotChainSuccession);
            
            templatesToDiscover.Add(exactly);
            templatesToDiscover.Add(respondedExistence);
            templatesToDiscover.Add(notChainSuccession);

            DeclareModel model = discovery.DiscoverModel(eventLog, templatesToDiscover);

            //responded existence should have at this moment some constraints discovered
            ITemplate reConstraint = respondedExistence.TemplateInstances.First();
        }
    }
}