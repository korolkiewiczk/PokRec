using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Common
{
    public class SaveLoad
    {
        public const string ProjExtension = ".proj";
        private readonly string _path;

        public SaveLoad(string path)
        {
            _path = path;
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
        }

        public void SaveProject(Project prj)
        {
            string path = Path.Combine(_path, prj.Name + ProjExtension);
            File.WriteAllText(path, JsonConvert.SerializeObject(prj));
        }

        public Project LoadProject(string fileName)
        {
            string path = Path.Combine(_path, fileName);
            return JsonConvert.DeserializeObject<Project>(File.ReadAllText(path));
        }
    
    }
}
