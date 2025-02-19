using System;
using System.Runtime.InteropServices;
using System.IO;

namespace BoletoNet.Wi2
{

    [ComVisible(true)]
    [Guid("12345678-ABCD-1234-EFAB-1234567890AB")]
    [ProgId("BoletoNet.Wi2.GeradorBoleto")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class GeradorBoleto
    {

        public GeradorBoleto() { }

        public string GerarBoleto(
                    string banco,
                    string agencia,
                    string conta,
                    string digitoConta,
                    string codigoCedente,
                    string cnpjcpf,
                    string nome,
                    decimal valor,
                    DateTime vencimento,
                    string carteira,
                    string nossoNumero,
                    string tipoModalidade = "4") // Parâmetro opcional para C6
        {
            try
            {
                // 1. Configuração do Banco
                Banco bancoObj;
                string codigoBanco;
                TipoArquivo tipoArquivoRemessa = TipoArquivo.CNAB400;

                switch (banco.ToLower())
                {
                    case "c6":
                        codigoBanco = "336";
                        break;
                    default:
                        throw new Exception("Banco não suportado.");
                }

                bancoObj = new Banco(int.Parse(codigoBanco));

                // 2. Configuração do Cedente
                Cedente cedente = new Cedente(cnpjcpf, nome, agencia, conta, digitoConta)
                {
                    Codigo = codigoCedente,
                    Endereco = new Endereco
                    {
                        End = "R ARNOLD BENNETT, 270",
                        Bairro = "GRANJA NOSSA SENHORA APARECIDA",
                        Cidade = "SÃO PAULO",
                        CEP = "04860-130",
                        UF = "SP"
                    }
                };

                // 3. Configuração do Sacado
                Sacado sacado = new Sacado("60975737010386", "SOCIEDADE BENEFICENTE SÃO CAMILO")
                {
                    Endereco = new Endereco
                    {
                        End = "R ARNOLD BENNETT, 270",
                        Bairro = "GRANJA NOSSA SENHORA APARECIDA",
                        Cidade = "SÃO PAULO",
                        CEP = "04860-130",
                        UF = "SP"
                    }
                };

                // 4. Criação do Boleto
                Boleto boleto = new Boleto(vencimento, valor, carteira, nossoNumero, cedente)
                {
                    Sacado = sacado
                };

                // Configuração da espécie de documento com base no banco
                switch (banco.ToLower())
                {
                    case "c6":
                        boleto.EspecieDocumento = new EspecieDocumento_C6("1"); // Duplicata Mercantil
                        boleto.TipoModalidade = tipoModalidade; // Modalidade obrigatória para C6
                        break;
                    default:
                        throw new Exception("Espécie de documento não configurada para o banco.");
                }

                // Instruções do boleto
                boleto.Instrucoes.Add(new Instrucao_Itau
                {
                    Descricao = "Após o vencimento, cobrar multa de 2%."
                });

                // 5. Geração do PDF do Boleto
                var boletoBancario = new BoletoBancario
                {
                    CodigoBanco = (short)bancoObj.Codigo,
                    Boleto = boleto,
                    MostrarEnderecoCedente = true,
                    MostrarComprovanteEntrega = true,
                    ImagemLogoCliente = ""
                };

                boletoBancario.Boleto.Valida();
                //string caminhoPDF = Path.Combine(Environment.CurrentDirectory, $"Boleto_{nossoNumero}.pdf");
                string caminhoPDF = Path.Combine(@"C:\temp", $"Boleto_{boletoBancario.Boleto.NossoNumero}.pdf");
                File.WriteAllBytes(caminhoPDF, boletoBancario.MontaBytesPDF());

                /*
                // 6. Geração do Arquivo Remessa
                var boletos = new Boletos { boleto };
                string caminhoRemessa = Path.Combine(Environment.CurrentDirectory, $"REMESSA_{DateTime.Now:yyyyMMddHHmmss}.txt");

                using (var fileStream = new FileStream(caminhoRemessa, FileMode.Create))
                {
                    var arquivoRemessa = new ArquivoRemessa(tipoArquivoRemessa);
                    arquivoRemessa.GerarArquivoRemessa("09", bancoObj, cedente, boletos, fileStream, 1);
                }

                // 7. Validação do Arquivo Remessa
                string mensagemValidacao;
                new ArquivoRemessa(tipoArquivoRemessa).ValidarArquivoRemessa(
                    "09",
                    bancoObj,
                    cedente,
                    boletos,
                    1,
                    out mensagemValidacao);

                if (!string.IsNullOrEmpty(mensagemValidacao))
                    throw new Exception($"Erro na remessa: {mensagemValidacao}");
                */

                return boletoBancario.MontaHtml();
            }
            catch (Exception ex)
            {
                return $"ERRO: {ex.Message}";
            }
        }
    }
}
