using System;
using System.Collections.Generic;
using System.Globalization;

namespace BoletoNet.Wi2
{
    public class JsonMapeador
    {
        /// <summary>
        /// Converte um objeto CedenteJson (da nova estrutura) para o objeto Cedente do BoletoNet.
        /// </summary>
        public Cedente ConverterCedente(CedenteJson cedenteJson)
        {
            var endereco = new Endereco
            {
                End = cedenteJson.Endereco.End,
                Numero = cedenteJson.Endereco.Numero.ToString(),
                Complemento = cedenteJson.Endereco.Complemento,
                Bairro = cedenteJson.Endereco.Bairro,
                Cidade = cedenteJson.Endereco.Cidade,
                CEP = cedenteJson.Endereco.CEP,
                UF = cedenteJson.Endereco.UF,
                Email = cedenteJson.Endereco.Email
            };

            return new Cedente(cedenteJson.CpfCnpj, cedenteJson.Nome, cedenteJson.Agencia, cedenteJson.Conta, cedenteJson.DigitoConta)
            {
                Codigo = cedenteJson.Codigo,
                Endereco = endereco
            };
        }

        /// <summary>
        /// Converte um objeto ItemRemessaJson (da nova estrutura) para um objeto Boleto do BoletoNet,
        /// tratando as datas, valores, descontos e instruções.
        /// </summary>
        public Boleto ConverterItemRemessa(ItemRemessaJson item)
        {
            DateTime dataVencimento, dataMulta, dataJurosMora;
            decimal valorBoleto, percMulta, percJurosMora;

            // Tratamento para datas
            dataVencimento = !string.IsNullOrWhiteSpace(item.DataVencimento)
                ? DateTime.ParseExact(item.DataVencimento, "ddMMyyyy", CultureInfo.InvariantCulture)
                : DateTime.MinValue;

            dataMulta = !string.IsNullOrWhiteSpace(item.DataMulta)
                ? DateTime.ParseExact(item.DataMulta, "ddMMyyyy", CultureInfo.InvariantCulture)
                : DateTime.MinValue;

            dataJurosMora = !string.IsNullOrWhiteSpace(item.DataJurosMora)
                ? DateTime.ParseExact(item.DataJurosMora, "ddMMyyyy", CultureInfo.InvariantCulture)
                : DateTime.MinValue;

            // Tratamento para valores numéricos
            valorBoleto = !string.IsNullOrWhiteSpace(item.ValorBoleto)
                ? decimal.Parse(item.ValorBoleto, CultureInfo.InvariantCulture)
                : 0m;

            percMulta = !string.IsNullOrWhiteSpace(item.PercMulta)
                ? decimal.Parse(item.PercMulta, CultureInfo.InvariantCulture)
                : 0m;

            percJurosMora = !string.IsNullOrWhiteSpace(item.PercJurosMora)
                ? decimal.Parse(item.PercJurosMora, CultureInfo.InvariantCulture)
                : 0m;

            // Mapeamento do endereço do sacado
            var sacadoEndereco = new Endereco
            {
                End = item.Sacado.Endereco.End,
                Numero = item.Sacado.Endereco.Numero.ToString(),
                Complemento = item.Sacado.Endereco.Complemento,
                Bairro = item.Sacado.Endereco.Bairro,
                Cidade = item.Sacado.Endereco.Cidade,
                CEP = item.Sacado.Endereco.CEP,
                UF = item.Sacado.Endereco.UF,
                Email = item.Sacado.Endereco.Email
            };

            // Criar o objeto Sacado
            var sacado = new Sacado(item.Sacado.CpfCnpj, item.Sacado.Nome)
            {
                Endereco = sacadoEndereco
            };

            // Cria o boleto com os campos obrigatórios
            var boleto = new Boleto
            {
                Carteira = item.Carteira,
                VariacaoCarteira = item.CarteiraVariacao,
                NossoNumero = item.NossoNumero,
                NumeroDocumento = item.NumeroDocumento,
                DataVencimento = dataVencimento,
                ValorBoleto = valorBoleto,
                PercMulta = percMulta,
                DataMulta = dataMulta,
                PercJurosMora = percJurosMora,
                DataJurosMora = dataJurosMora,
                Sacado = sacado
            };

            // Tratar descontos, se presentes (usando "outrosDescontos" e "dataOutrosDescontos")
            if (!string.IsNullOrEmpty(item.OutrosDescontos) && !string.IsNullOrEmpty(item.DataOutrosDescontos))
            {
                decimal valorDesconto = decimal.Parse(item.OutrosDescontos, CultureInfo.InvariantCulture);
                DateTime dataDesconto = DateTime.ParseExact(item.DataOutrosDescontos, "ddMMyyyy", CultureInfo.InvariantCulture);
                boleto.ValorDesconto = valorDesconto;
                boleto.DataDesconto = dataDesconto;
            }

            if (item.Instrucoes != null && item.Instrucoes.Count > 0)
            {
                foreach (var kvp in item.Instrucoes)
                {
                    boleto.Instrucoes.Add(new Instrucao_Itau { Descricao = kvp.Value });
                }
            }

            return boleto;
        }

        /// <summary>
        /// Converte um objeto BoletoBancarioJson (da nova estrutura) para um objeto BoletoBancario do BoletoNet.
        /// Este método mapeia o cedente, as instruções globais e os itens de remessa.
        /// </summary>
        public Boletos ConverterBoletos(BoletoBancarioJson boletoJson)
        {
            Cedente cedente = ConverterCedente(boletoJson.Cedente);
            var boletos = new Boletos();
            boletos.Banco = new Banco(int.Parse(boletoJson.CodigoBanco));
            boletos.Cedente = cedente;

            foreach (var item in boletoJson.ItensRemessa)
            {
                Boleto boleto = ConverterItemRemessa(item);
                boleto.Cedente = cedente;

                if (boletoJson.Instrucoes != null && boletoJson.Instrucoes.Count > 0)
                {
                    foreach (var kvp in boletoJson.Instrucoes)
                    {
                        boleto.Instrucoes.Add(new Instrucao_Itau { Descricao = kvp.Value });
                    }
                }

                switch (boletoJson.CodigoBanco.ToLower())
                {
                    case "336":
                        boleto.EspecieDocumento = new EspecieDocumento_C6("1"); // Duplicata Mercantil
                        boleto.TipoModalidade = "04"; // Modalidade obrigatória para C6
                        break;
                    default:
                        throw new Exception("Espécie de documento não configurada para o banco.");
                }

                boletos.Add(boleto);
            }

            return boletos;
        }

        /// <summary>
        /// Converte um objeto BoletoBancarioJson (da nova estrutura) para um objeto BoletoBancario do BoletoNet.
        /// Este método mapeia o cedente, as instruções globais e os itens de remessa.
        /// </summary>
        public List<BoletoBancario> ConverterBoletoBancario(BoletoBancarioJson boletoJson)
        {
            
            var boletos = ConverterBoletos(boletoJson);
            
            List <BoletoBancario> boletosBancarios = new List<BoletoBancario>();
            foreach ( var item in boletos)
            {
                BoletoBancario boletoBancario = new BoletoBancario
                {
                    CodigoBanco = short.Parse(boletoJson.CodigoBanco),
                    Boleto = item,
                    MostrarEnderecoCedente = (boletoJson.MostrarEnderecoCedente.Equals("1")),
                    MostrarComprovanteEntrega = (boletoJson.MostrarComprovanteEntrega.Equals("1"))
                };

                switch (boletoJson.CodigoBanco.ToLower())
                {
                    case "336":
                        boletoBancario.Boleto.LocalPagamento = "PAGÁVEL EM CANAIS ELETRÔNICOS, AGÊNCIAS OU CORRESPONDENTES";
                        break;
                }

                boletosBancarios.Add(boletoBancario);
            }

            return boletosBancarios;
        }
    }
}
