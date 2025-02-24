using System;
using System.Collections.Generic;
using System.IO;
using BoletoNet.Wi2;

namespace BoletoNet.Testes
{
    public class BancoC6Hom
    {
        private const int CODIGO_BANCO = (int)Enums.Bancos.C6Bank;
        private List<MassaHomologacao> massaHomologacao, massaHomologacaoTorres;
        private const int modalidadeIdentificadorLayoutEmissaoBanco = 3;
        private Banco banco;
        private readonly string caminhoRemessa = Path.Combine(@"C:\temp", $"REMESSA_{DateTime.Now:yyyyMMddHHmmss}.txt");

        public BancoC6Hom()
        {

            banco = new Banco(336);
            if (banco == null)
                throw new Exception("Erro ao inicializar a classe Banco.");

            massaHomologacao = new List<MassaHomologacao>()
            {
                new MassaHomologacao
                {
                    Cedente = new Cedente("11.085.618/0001-18", "HORTIPLAS DISTRIBUIÇÃO DE HORTIFRUTI E DESCARTÁVEIS LTDA", "1", "318268280", "0" )
                    {
                        Codigo = "000033099578",
                        Endereco = new Endereco
                        {
                            End = "AV ONIX",
                            Numero = "438",
                            Bairro = "VILA AYROSA",
                            Cidade = "OSASCO",
                            CEP = "06280-030",
                            UF = "SP"
                        }
                    },
                    Boletos = {
                        new Boleto {//Sem desconto, Sem multa
                            Carteira = "20",
                            DataVencimento = new DateTime(2025, 3, 06),
                            ValorBoleto = 2095.01m,
                            NossoNumero = "0000124898",
                            NumeroDocumento = "21350-001",
                            Sacado = new Sacado("62.149.000/0001-05", "ASSOCIAÇÃO DOS FUNCIONARIOS PUBLICOS DO ESTADO DE SÃO PAULO")
                            {
                                Endereco = new Endereco
                                {
                                    End = "R DOUTOR BETTENCOURT RODRIGUES",
                                    Numero = "155",
                                    Bairro = "CENTRO",
                                    Cidade = "SÃO PAULO",
                                    CEP = "01017-010",
                                    UF = "SP"
                                }
                            },
                            TipoModalidade = modalidadeIdentificadorLayoutEmissaoBanco.ToString(),
                            EspecieDocumento = new EspecieDocumento_C6("1") // Duplicata Mercantil
                        },
                        new Boleto { //Com Desconto
                            Carteira = "20",
                            DataVencimento = new DateTime(2025, 3, 05),
                            ValorBoleto = 1838.89m,
                            ValorDesconto = 91.94m,
                            DataDesconto = new DateTime(2025, 3, 05),
                            NossoNumero = "0000124888",
                            NumeroDocumento = "21340-001",
                            Sacado = new Sacado("61.021.598/0001-90", "PADARIA ELIZABETE")
                            {
                                Endereco = new Endereco
                                {
                                    End = "AV DOS REMEDIOS",
                                    Numero = "1005",
                                    Bairro = "VILA DOS REMEDIOS",
                                    Cidade = "SÃO PAULO",
                                    CEP = "06298-006",
                                    UF = "SP"
                                }
                            },
                            Instrucoes = {
                                new Instrucao_Bradesco((int)EnumInstrucoes_Bradesco.ComDesconto, 91.94, new DateTime(2025, 3, 05), AbstractInstrucao.EnumTipoValor.Reais)
                            },
                            TipoModalidade = modalidadeIdentificadorLayoutEmissaoBanco.ToString(),
                            EspecieDocumento = new EspecieDocumento_C6("1")
                        },
                        new Boleto { //Com multa e juros
                            Carteira = "20",
                            DataVencimento = new DateTime(2026, 3, 04),
                            ValorBoleto = 1061.10m,
                            DataMulta = new DateTime(2026, 3, 05),
                            PercMulta = 2.0m,
                            Instrucoes = {
                                new Instrucao_Bradesco((int)EnumInstrucoes_Bradesco.OutrasInstrucoes_ExibeMensagem_MultaVencimento, 2.0,AbstractInstrucao.EnumTipoValor.Percentual),
                                new Instrucao_Bradesco((int)EnumInstrucoes_Bradesco.OutrasInstrucoes_ExibeMensagem_MoraDiaria, 0.33,AbstractInstrucao.EnumTipoValor.Percentual),
                            },
                            JurosMora = (1061.10m*0.33m),
                            PercJurosMora = 0.33m,
                            DataJurosMora = new DateTime(2026, 3, 05),
                            NossoNumero = "0000124968",
                            NumeroDocumento = "21359-001",
                            Sacado = new Sacado("30.539.414/0001-54", "ALFIO GASTRONOMIA EIRELI")
                            {
                                Endereco = new Endereco
                                {
                                    End = "R CAPITAO PACHECO E CHAVES",
                                    Numero = "313",
                                    Bairro = "VILA PRUDENTE",
                                    Cidade = "SÃO PAULO",
                                    CEP = "03126-000",
                                    UF = "SP"
                                }
                            },
                            TipoModalidade = modalidadeIdentificadorLayoutEmissaoBanco.ToString(),
                            EspecieDocumento = new EspecieDocumento_C6("1") // Duplicata Mercantil
                        }
                    }
                }
            };

            massaHomologacaoTorres = new List<MassaHomologacao>()
            {
                new MassaHomologacao
                {
                    Cedente = new Cedente("11.239.652/0001-08", "M S M TORRES HORTIFRUTIGRANJEIROS", "1", "340086157", "0" )
                    {
                        Codigo = "000035537838",
                        Endereco = new Endereco
                        {
                            End = "R SABIA",
                            Numero = "211",
                            Bairro = "AYROSA",
                            Cidade = "OSASCO",
                            CEP = "06293-040",
                            UF = "SP"
                        }
                    },
                    Boletos = {
                        new Boleto {//Sem desconto, Sem multa
                            Carteira = "20",
                            DataVencimento = new DateTime(2025, 3, 15),
                            ValorBoleto = 11023.56m,
                            NossoNumero = "0001425689",
                            NumeroDocumento = "11351-001",
                            Sacado = new Sacado("44.285.599/0001-22", "SFAN CONTINENTAL LTDA")
                            {
                                Endereco = new Endereco
                                {
                                    End = "AVENIDA INTERLAGOS",
                                    Numero = "2255",
                                    Complemento = "ARCO 80 - LOJA 282",
                                    Bairro = "INTERLAGOS",
                                    Cidade = "SÃO PAULO",
                                    CEP = "44661-200",
                                    UF = "SP"
                                }
                            },
                            TipoModalidade = modalidadeIdentificadorLayoutEmissaoBanco.ToString(),
                            EspecieDocumento = new EspecieDocumento_C6("1") // Duplicata Mercantil
                        },
                        new Boleto { //Com Desconto
                            Carteira = "20",
                            DataVencimento = new DateTime(2025, 3, 17),
                            ValorBoleto = 3238.25m,
                            ValorDesconto = 161.91m,
                            DataDesconto = new DateTime(2025, 3, 17),
                            NossoNumero = "0001425690",
                            NumeroDocumento = "11352-001",
                            Sacado = new Sacado("31.409.068/0001-52", "GSGV ALIMENTOS EIRIELI")
                            {
                                Endereco = new Endereco
                                {
                                    End = "ROD RAPOSO TAVARES",
                                    Numero = "S/N",
                                    Complemento = "KM 23600 PISO L2 LJ SGV332",
                                    Bairro = "LAGEADINHO",
                                    Cidade = "SÃO PAULO",
                                    CEP = "06709-015",
                                    UF = "SP"
                                }
                            },
                            Instrucoes = {
                                new Instrucao_Bradesco((int)EnumInstrucoes_Bradesco.ComDesconto, 161.91, new DateTime(2025, 3, 17), AbstractInstrucao.EnumTipoValor.Reais)
                            },
                            TipoModalidade = modalidadeIdentificadorLayoutEmissaoBanco.ToString(),
                            EspecieDocumento = new EspecieDocumento_C6("1")
                        },
                        new Boleto { //Com multa e juros
                            Carteira = "20",
                            NossoNumero = "0001425691",
                            NumeroDocumento = "11353-001",
                            DataVencimento = new DateTime(2026, 3, 10),
                            ValorBoleto = 1000.00m,
                            DataMulta = new DateTime(2026, 3, 11),
                            PercMulta = 2.0m,
                            Instrucoes = {
                                new Instrucao_Bradesco((int)EnumInstrucoes_Bradesco.OutrasInstrucoes_ExibeMensagem_MultaVencimento, 2.0,AbstractInstrucao.EnumTipoValor.Percentual),
                                new Instrucao_Bradesco((int)EnumInstrucoes_Bradesco.OutrasInstrucoes_ExibeMensagem_MoraDiaria, 0.33,AbstractInstrucao.EnumTipoValor.Percentual),
                            },
                            JurosMora = (1000.00m*0.33m),
                            PercJurosMora = 0.33m,
                            DataJurosMora = new DateTime(2026, 3, 11),
                            Sacado = new Sacado("28.513.282/0001-40", "TATIANE LEMOS FERNANDES DE LIMA")
                            {
                                Endereco = new Endereco
                                {
                                    End = "R ANTONIO GUGANIS",
                                    Numero = "45",
                                    Bairro = "JARDIM SAO PAULO(ZONA NORTE)",
                                    Cidade = "SÃO PAULO",
                                    CEP = "02044-110",
                                    UF = "SP"
                                }
                            },
                            TipoModalidade = modalidadeIdentificadorLayoutEmissaoBanco.ToString(),
                            EspecieDocumento = new EspecieDocumento_C6("1") // Duplicata Mercantil
                        }
                    }
                }
            };
        }


        #region Classe para representar massa de testes de boletos a serem testados

        private class MassaHomologacao
        {
            public Cedente Cedente { get; set; }
            public Boletos Boletos { get; set; } = new Boletos();
        }


        #endregion Classe para representar massa de testes de boletos a serem testados

        private BoletoBancario GerarBoletoCarteira(Boleto item, Cedente cedente)
        {
            item.Cedente = cedente;
            var boletoBancario = new BoletoBancario
            {
                CodigoBanco = CODIGO_BANCO,
                Boleto = item
            };
            boletoBancario.Boleto.Valida();
            return boletoBancario;
        }

        #region Testes do arquivo remessa

        public void HomologacaoArquivoRemessa()
        {
            foreach (MassaHomologacao item in massaHomologacaoTorres)
            {
                foreach (Boleto boleto in item.Boletos)
                {
                    var boletoBancario = GerarBoletoCarteira(boleto, item.Cedente);
                    string caminhoPDF = Path.Combine(@"C:\temp", $"Boleto_{boletoBancario.Boleto.NossoNumero}.pdf");
                    File.WriteAllBytes(caminhoPDF, boletoBancario.MontaBytesPDF());
                }

                using (var fileStream = new FileStream(caminhoRemessa, FileMode.Create))
                {
                    var arquivoRemessa = new ArquivoRemessa(TipoArquivo.CNAB400);
                    arquivoRemessa.GerarArquivoRemessa(item.Cedente.Convenio.ToString(), banco, item.Cedente, item.Boletos, fileStream, 1);

                    arquivoRemessa.ValidarArquivoRemessa(
                        item.Cedente.Convenio.ToString(), banco, item.Cedente, item.Boletos, 1, out string mensagemValidacao);

                    if (!string.IsNullOrEmpty(mensagemValidacao))
                        throw new Exception($"Erro na remessa: {mensagemValidacao}");
                }

            }

        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Iniciando geração de boletos...");
            try
            {
               // BancoC6Hom bancoC6Hom = new BancoC6Hom();
//bancoC6Hom.HomologacaoArquivoRemessa();

                Console.WriteLine("Iniciando geração de boletos...");
                try
                {
                    Cobranca cobranca = new Cobranca();

                    //cobranca.GerarBoletosPDF(@"c:\temp\boletoBancario.json", "");
                    cobranca.GerarRemessa(@"c:\temp\boletoBancario.json", "", "");
                    //Console.WriteLine(cobranca.GerarBoletoHTML(@"c:\temp\boletoBancario.json", ""));
                    //cobranca.GerarBoletoHTML(@"c:\temp\boletoBancario.json", "");
                    //cobranca.GerarBoletoHTMLjson(@"c:\temp\boletoBancario.json", "");
                    Console.WriteLine("Boletos gerados e arquivos de remessa criados com sucesso!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro: {ex.Message}");
                }


                Console.WriteLine("Boletos gerados e arquivos de remessa criados com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }
        }

        #endregion
    }

}
