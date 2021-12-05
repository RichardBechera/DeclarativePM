using DeclarativePM.Lib.Models.DeclareModels;
using DeclarativePM.Lib.Utils;
using Newtonsoft.Json;

namespace DeclarativePM.Lib.IO.Export
{
    public class Exporter
    {
        public string ExportModel(DeclareModel model)
        {
            return JsonConvert.SerializeObject(model, new ParametrizedTemplateConverter());
        }
    }
}