using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BoletoNet.Wi2
{
    // Representa a parte de remessa do JSON
    public class RemessaJson
    {
        [JsonPropertyName("codigoBanco")]
        public string CodigoBanco { get; set; }

        [JsonPropertyName("tipoArquivo")]
        public string TipoArquivo { get; set; }

        [JsonPropertyName("caminhoRemessa")]
        public string CaminhoRemessa { get; set; }
        
        [JsonPropertyName("numeroArquivoRemessa")]
        public string NumeroArquivoRemessa { get; set; }

        [JsonPropertyName("numeroConvenio")]
        public string NumeroConvenio { get; set; }
    }

    // Representa um endereço
    public class EnderecoJson
    {
        [JsonPropertyName("end")]
        public string End { get; set; }

        [JsonPropertyName("numero")]
        public string Numero { get; set; }

        [JsonPropertyName("complemento")]
        public string Complemento { get; set; }

        [JsonPropertyName("bairro")]
        public string Bairro { get; set; }

        [JsonPropertyName("cidade")]
        public string Cidade { get; set; }

        [JsonPropertyName("cep")]
        public string CEP { get; set; }

        [JsonPropertyName("uf")]
        public string UF { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }
    }

    // Representa os dados do cedente
    public class CedenteJson
    {
        [JsonPropertyName("cpfCnpj")]
        public string CpfCnpj { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("agencia")]
        public string Agencia { get; set; }

        [JsonPropertyName("conta")]
        public string Conta { get; set; }

        [JsonPropertyName("digitoConta")]
        public string DigitoConta { get; set; }

        [JsonPropertyName("codigo")]
        public string Codigo { get; set; }

        [JsonPropertyName("endereco")]
        public EnderecoJson Endereco { get; set; }
    }

    // Representa as informações do sacado
    public class SacadoJson
    {
        [JsonPropertyName("cpfCnpj")]
        public string CpfCnpj { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("agencia")]
        public string Agencia { get; set; }

        [JsonPropertyName("conta")]
        public string Conta { get; set; }

        [JsonPropertyName("digitoConta")]
        public string DigitoConta { get; set; }

        [JsonPropertyName("codigo")]
        public string Codigo { get; set; }

        [JsonPropertyName("endereco")]
        public EnderecoJson Endereco { get; set; }
    }

    // Representa cada item de remessa (boleto) presente na lista
    public class ItemRemessaJson
    {
        [JsonPropertyName("carteira")]
        public string Carteira { get; set; }

        [JsonPropertyName("carteiraVariacao")]
        public string CarteiraVariacao { get; set; }

        [JsonPropertyName("nossoNumero")]
        public string NossoNumero { get; set; }

        [JsonPropertyName("numeroDocumento")]
        public string NumeroDocumento { get; set; }

        [JsonPropertyName("dataVencimento")]
        public string DataVencimento { get; set; }

        [JsonPropertyName("valorBoleto")]
        public string ValorBoleto { get; set; }

        [JsonPropertyName("percMulta")]
        public string PercMulta { get; set; }

        [JsonPropertyName("dataMulta")]
        public string DataMulta { get; set; }

        [JsonPropertyName("percJurosMora")]
        public string PercJurosMora { get; set; }

        [JsonPropertyName("dataJurosMora")]
        public string DataJurosMora { get; set; }

        [JsonPropertyName("outrosDescontos")]
        public string OutrosDescontos { get; set; }

        [JsonPropertyName("dataOutrosDescontos")]
        public string DataOutrosDescontos { get; set; }

        [JsonPropertyName("instrucoes")]
        public Dictionary<string, string> Instrucoes { get; set; }

        [JsonPropertyName("sacado")]
        public SacadoJson Sacado { get; set; }
    }

    // Representa a parte de boleto bancário do JSON
    public class BoletoBancarioJson
    {
        [JsonPropertyName("codigoBanco")]
        public string CodigoBanco { get; set; }

        [JsonPropertyName("mostrarEnderecoCedente")]
        public string MostrarEnderecoCedente { get; set; }

        [JsonPropertyName("mostrarComprovanteEntrega")]
        public string MostrarComprovanteEntrega { get; set; }

        [JsonPropertyName("caminhoPDF")]
        public string CaminhoPDF { get; set; }

        [JsonPropertyName("imagemLogoCliente")]
        public string ImagemLogoCliente { get; set; }

        [JsonPropertyName("cedente")]
        public CedenteJson Cedente { get; set; }

        [JsonPropertyName("instrucoes")]
        public Dictionary<string, string> Instrucoes { get; set; }

        [JsonPropertyName("itensRemessa")]
        public List<ItemRemessaJson> ItensRemessa { get; set; }
    }

    // Classe raiz que mapeia o JSON completo
    public class RootJson
    {
        [JsonPropertyName("remessa")]
        public RemessaJson Remessa { get; set; }

        [JsonPropertyName("boletoBancario")]
        public BoletoBancarioJson BoletoBancario { get; set; }
    }


    // Classe para ler o arquivo JSON e retornar um objeto RootJson
    public class JsonLeitor
    {
        /// <summary>
        /// Lê o arquivo JSON e desserializa para o objeto RootJson.
        /// </summary>
        /// <returns>Objeto RootJson com os dados configurados.</returns>
        public RootJson LerConfiguracao(string caminhoArquivo = null, string jsonContent = null)
        {
            if (!string.IsNullOrWhiteSpace(caminhoArquivo))
            {
                if (!File.Exists(caminhoArquivo))
                    throw new FileNotFoundException("Arquivo JSON não encontrado.", caminhoArquivo);

                jsonContent = File.ReadAllText(caminhoArquivo);
            }
            else if (string.IsNullOrWhiteSpace(jsonContent))
            {
                throw new ArgumentException("É necessário fornecer o caminho do arquivo ou o conteúdo JSON.");
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            return JsonConvert.DeserializeObject<RootJson>(jsonContent);
            //return JsonSerializer.Deserialize<RootJson>(jsonContent, options);
        }
    }
}