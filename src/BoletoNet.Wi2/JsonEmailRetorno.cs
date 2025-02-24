using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace BoletoNet.Wi2
{
    public class EnvioEmail
    {
        public string CaminhoArquivo { get; set; }
        public string NumeroDocumento { get; set; }
        public string LinhaDigitavel { get; set; }
        public string Email { get; set; }
    }

    public class RootEnvioEmail
    {
        public List<EnvioEmail> EnvioEmail { get; set; }
    }

    public class GerarJson
    {
        public static string ObterJson(List<EnvioEmail> listaEnvioEmail)
        {
            // Montamos o objeto raiz que contém a lista
            var rootObject = new RootEnvioEmail
            {
                EnvioEmail = listaEnvioEmail
            };


            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.None // ou Formatting.Indented, se preferir
            };

            // Se estiver usando o System.Text.Json:
            var jsonString = JsonConvert.SerializeObject(rootObject, settings);

            Console.WriteLine(jsonString);

            return jsonString;
        }
    }
}
