using DeclarativePM.Lib.Import;
using DeclarativePM.Lib.Models.DeclareModels;
using DeclarativePM.Lib.Utils;
using Newtonsoft.Json;

namespace DeclarativePM.Lib.Export
{
    public class JsonModelExporter
    {
        public string ExportModel(DeclareModel model)
        {
            string json = JsonConvert.SerializeObject(model, new ParametrizedTemplateConverter());
            
            return json;
        }
    }
}