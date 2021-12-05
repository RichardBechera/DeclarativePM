using System;
using System.IO;
using System.Threading.Tasks;
using DeclarativePM.Lib.Models.DeclareModels;
using Newtonsoft.Json;

namespace DeclarativePM.Lib.IO.Export
{
    public class Exporter
    {
        public string ExportModel(DeclareModel model)
        {
            return JsonConvert.SerializeObject(model, new ParametrizedTemplateConverter());
        }

        public async Task ExportSaveModelAsync(DeclareModel model, string path, string fileName)
        {
            if (File.Exists(Path.Combine(path + ".json", fileName)) || !Directory.Exists(path))
                throw new Exception("Path does not exist or file with given name already exists");


            string json = JsonConvert.SerializeObject(model, new ParametrizedTemplateConverter());
            await File.WriteAllTextAsync(Path.Combine(path, fileName + ".json"), json);
        }
    }
}