using BoletoNet.Enums;
using BoletoNet.Util;
using System;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Web.Security;
using System.Web.UI;

[assembly: WebResource("BoletoNet.Imagens.336.jpg", "image/jpg")]

namespace BoletoNet
{
    internal class Banco_C6 : AbstractBanco, IBanco
    {
        #region Construtores

        internal Banco_C6()
        {
            try
            {
                this.Codigo = 336;
                this.Digito = "0";
                this.Nome = "C6 Bank";
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Erro ao instanciar objeto.", ex);
            }
        }

        #endregion Construtores

        #region Métodos de formatação genéricos
        public void FormataCodigoCliente(Cedente cedente)
        {
            if (cedente.Codigo.Length == 7)
                cedente.DigitoCedente = Convert.ToInt32(cedente.Codigo.Substring(6));

            cedente.Codigo = cedente.Codigo.Substring(0, 6).PadLeft(6, '0');
        }

        public void FormataCodigoCliente(Boleto boleto)
        {
            if (boleto.Cedente.Codigo.Length == 7)
                boleto.Cedente.DigitoCedente = Convert.ToInt32(boleto.Cedente.Codigo.Substring(6));

            boleto.Cedente.Codigo = boleto.Cedente.Codigo.Substring(0, 6).PadLeft(6, '0');
        }

        public String FormataNumeroTitulo(Boleto boleto)
        {
            var novoTitulo = new StringBuilder();
            novoTitulo.Append(boleto.NossoNumero.Replace("-", "").PadLeft(8, '0'));
            return novoTitulo.ToString();
        }

        private string Mod11BancoC6(string sequencial, int baseDivisao)
        {
            #region Trecho do manual https://cms-assets-p.c6bank.com.br/uploads/manual-do-calculo-do-digito-verificador.pdf do C6
            /* 
            Para o cálculo do dígito, será necessário acrescentar o número da carteira à esquerda antes do Nosso Número, 
            e aplicar o módulo 11, com base 7.
            Multiplicar cada algarismo que compõe o número pelo seu respectivo multiplicador (PESO).
            Os multiplicadores(PESOS) variam de 2 a 7.
            O primeiro dígito da direita para a esquerda deverá ser multiplicado por 2, o segundo por 3 e assim sucessivamente.
             
              Fixo Carteira   Nosso Numero
              ____   ______   ______________________________________
               0     4    3   4   4   4   4   4   4   4   4   4   4 
               x     x    x   x   x   x   x   x   x   x   x   x   x 
               2     7    6   5   4   3   2   7   6   5   4   3   2 
               =     =    =   =   =   =   =   =   =   =   =   =   = 
               0    28 + 18 +20 +16 +12 + 8 +28 +24 +20 +16 +12 + 8  = 210

            O total da soma deverá ser dividido por 11: 210/ 11 = 19 tendo como resto = 1
            A diferença entre o divisor e o resto, será o dígito de autoconferência: 11 - 1 = 10 (dígito de auto-conferência DV)
            
            Se DV for "11", considerar o dígito como “0”. 
            Se DV for "10", considerar o dígito como “P”.
            */
            #endregion

            /* Variáveis
             * -------------
             * s - Soma
             * p - Peso
             * b - Base
             * r - Resto
             */

            int s = 0, p = 2;

            for (int i = sequencial.Length; i > 0; i--)
            {
                s = s + (Convert.ToInt32(sequencial.Mid(i, 1)) * p);
                if (p == baseDivisao)
                    p = 2;
                else
                    p = p + 1;
            }

            int r = (s % 11);

            if (r == 0)
                return "0";
            else if (r == 1)
                return "P";
            else
                return (11 - r).ToString();
        }
        #endregion 

        #region Métodos de Instância

        /// <summary>
        /// Validações particulares do Banco C6
        /// </summary>
        public override void ValidaBoleto(Boleto boleto)
        {
            var carteirasImplementadas = new int[] { 10, 20, 21, 22, 23, 24 };
            var possiveisModalidadesIdentificadorLayout = new int[] { 3, 4 };

            int carteiraInt;
            int tipoModalidadeInt;

            if (string.IsNullOrEmpty(boleto.Carteira) || !int.TryParse(boleto.Carteira, out carteiraInt))
                throw new ArgumentException("Carteira não informada ou inválida.");

            if (!carteirasImplementadas.Contains(carteiraInt))
                throw new ArgumentException(string.Format("Carteira {0} não implementada (Carteiras disponíveis: {1}).", carteiraInt, string.Join(",", carteirasImplementadas)));

            if (string.IsNullOrEmpty(boleto.TipoModalidade) || !int.TryParse(boleto.TipoModalidade, out tipoModalidadeInt))
                throw new ArgumentException("'TipoModalidade' não informada ou inválida para o boleto (Corresponde ao IdentificadorLayout para o C6 Bank).");

            if (!possiveisModalidadesIdentificadorLayout.Contains(tipoModalidadeInt))
                throw new ArgumentException(string.Format("Modalidade informada {0} é inválida (Modalidades disponíveis: {1}).", tipoModalidadeInt, string.Join(",", possiveisModalidadesIdentificadorLayout)));

            if (boleto.NossoNumero.Length != 10)
                throw new ArgumentException("Nosso número deve possuir 10 posições");

            if (boleto.Cedente.Codigo.Length != 12)
                throw new NotImplementedException("Código do cedente precisa conter 12 dígitos");

            // Atribui o nome do banco ao local de pagamento
            boleto.LocalPagamento = "PAGÁVEL EM CANAIS ELETRONICOS, AGÊNCIAS OU CORRESPONDENTES";

            //Verifica se data do processamento é valida
            if (boleto.DataProcessamento == DateTime.MinValue)
                boleto.DataProcessamento = DateTime.Now;

            //Verifica se data do documento é valida
            if (boleto.DataDocumento == DateTime.MinValue)
                boleto.DataDocumento = DateTime.Now;

            boleto.QuantidadeMoeda = 0;

            FormataCodigoBarra(boleto);
            FormataLinhaDigitavel(boleto);
            FormataNossoNumero(boleto);
        }

        #endregion Métodos de Instância

        #region Métodos de formatação do boleto

        public override void FormataNossoNumero(Boleto boleto)
        {
            // Sem necessidade de formatar, considera o valor recebido.
        }

        public override void FormataNumeroDocumento(Boleto boleto)
        {
            // Sem necessidade de formatar, considera o valor recebido.
        }

        public override void FormataCodigoBarra(Boleto boleto)
        {
            string valorBoleto = boleto.ValorBoleto.ToString("f").Replace(",", "").Replace(".", "");
            valorBoleto = Utils.FormatCode(valorBoleto, 10);

            boleto.CodigoBarra.Codigo = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}",
                Utils.FormatCode(Codigo.ToString(), 3),
                boleto.Moeda,
                FatorVencimento(boleto),
                valorBoleto,
                boleto.Cedente.Codigo.PadLeft(12, '0'),
                boleto.NossoNumero.PadLeft(10, '0'),
                boleto.Carteira.PadLeft(2, '0'),
                int.Parse(boleto.TipoModalidade));

            int _dacBoleto = Mod11Peso2a9(boleto.CodigoBarra.Codigo);

            boleto.CodigoBarra.Codigo = boleto.CodigoBarra.Codigo.Substring(0, 4) + _dacBoleto + boleto.CodigoBarra.Codigo.Substring(4, 39);
        }

        public string CalcularDigitoNossoNumero(Boleto boleto)
        {
            return Mod11BancoC6("0" + boleto.Carteira + Utils.FitStringLength(boleto.NossoNumero, 10, 10, '0', 1, true, true, true), 7);
        }

        public override void FormataLinhaDigitavel(Boleto boleto)
        {
            string campo1;
            string campo2;
            string campo3;
            string campo4;
            string campo5;
            int digitoMod;

            /*
            Campos 1
            Código do Banco na Câmara de Compensação “336”
            Código da moeda "9" (*)
            Código do Cedente – 5 Posições
            Dígito Verificador Módulo 10 (Campo 1)
             */
            campo1 = String.Concat(Utils.FormatCode(Codigo.ToString(), 3), boleto.Moeda, boleto.Cedente.Codigo.Substring(0, 5));
            digitoMod = Mod10(campo1);
            campo1 += digitoMod.ToString();
            campo1 = campo1.Substring(0, 5) + "." + campo1.Substring(5, 5);

            /*
            Campo 2
            Código do Cedente – 7 Posições
            Nosso número – 3 posições
            Dígito Verificador Módulo 10 (Campo 2)
             */
            campo2 = String.Concat(boleto.Cedente.Codigo.Substring(5, 7), boleto.NossoNumero.Substring(0, 3));
            digitoMod = Mod10(campo2);
            campo2 += digitoMod.ToString();
            campo2 = campo2.Substring(0, 5) + "." + campo2.Substring(5, 6);

            /*
            Campo 3
            Nosso número – 7 posições
            Código da Carteira
            Identificador de Layout
            Dígito Verificador Módulo 10 (Campo 3)
             */
            campo3 = String.Concat(boleto.NossoNumero.Substring(3, 7), boleto.Carteira.PadLeft(2, '0'), boleto.TipoModalidade);
            digitoMod = Mod10(campo3);
            campo3 += digitoMod;
            campo3 = campo3.Substring(0, 5) + "." + campo3.Substring(5, 6);

            /*
            Campo 4
            Dígito Verificador Geral
             */
            campo4 = boleto.CodigoBarra.Codigo.Substring(4, 1);

            /*
            Campo 5 (UUUUVVVVVVVVVV)
            U = Fator de Vencimento ( Anexo 10)
            V = Valor do Título (*)
             */
            string valorBoleto = boleto.ValorBoleto.ToString("f").Replace(",", "").Replace(".", "");
            valorBoleto = Utils.FormatCode(valorBoleto, 10);
            campo5 = String.Concat(FatorVencimento(boleto).ToString(), valorBoleto);

            boleto.CodigoBarra.LinhaDigitavel = campo1 + " " + campo2 + " " + campo3 + " " + campo4 + " " + campo5;
        }

        #endregion Métodos de formatação do boleto

        #region Métodos de geração do arquivo remessa - Genéricos

        public override string GerarHeaderLoteRemessa(string numeroConvenio, Cedente cedente, int numeroArquivoRemessa, TipoArquivo tipoArquivo)
        {
            throw new NotImplementedException();
        }

        public override bool ValidarRemessa(TipoArquivo tipoArquivo, string numeroConvenio, IBanco banco, Cedente cedente, Boletos boletos, int numeroArquivoRemessa, out string mensagem)
        {
            bool vRetorno = true;
            string vMsg = string.Empty;
            ////IMPLEMENTACAO PENDENTE
            mensagem = vMsg;
            return vRetorno;
        }

        public override string GerarDetalheRemessa(Boleto boleto, int numeroRegistro, TipoArquivo tipoArquivo)
        {
            try
            {
                string _detalhe = " ";

                //Se o nosso número ainda não foi formatado então formata
                if (!string.IsNullOrWhiteSpace(boleto.NossoNumero) && boleto.NossoNumero.Length <= 7)
                {
                    FormataNossoNumero(boleto);
                }

                base.GerarDetalheRemessa(boleto, numeroRegistro, tipoArquivo);

                switch (tipoArquivo)
                {

                    case TipoArquivo.CNAB400:
                        _detalhe = GerarDetalheRemessaCNAB400(boleto, numeroRegistro, tipoArquivo);
                        break;
                    case TipoArquivo.Outro:
                        throw new Exception("Tipo de arquivo inexistente.");
                }

                return _detalhe;

            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a geração do DETALHE do arquivo de REMESSA.", ex);
            }
        }

        public override string GerarDetalheSegmentoPRemessa(Boleto boleto, int numeroRegistro, string numeroConvenio)
        {
            throw new NotImplementedException();
        }

        public override string GerarDetalheSegmentoQRemessa(Boleto boleto, int numeroRegistro, TipoArquivo tipoArquivo)
        {
            throw new NotImplementedException();
        }

        public override string GerarDetalheSegmentoRRemessa(Boleto boleto, int numeroRegistro, TipoArquivo tipoArquivo)
        {
            throw new NotImplementedException();
        }

        public override string GerarTrailerRemessa(int numeroRegistro,TipoArquivo tipoArquivo,Cedente cedente,decimal vltitulostotal)
        {
            try
            {
                var trailer = string.Empty;

                base.GerarTrailerRemessa(numeroRegistro, tipoArquivo, cedente, vltitulostotal);

                switch (tipoArquivo)
                {
                    case TipoArquivo.CNAB400:
                        trailer = this.GerarTrailerArquivoRemessa(numeroRegistro);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("tipoArquivo", tipoArquivo, null);
                }

                return trailer;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override string GerarTrailerLoteRemessa(int numeroRegistro)
        {
            throw new NotImplementedException();
        }

        public override string GerarTrailerArquivoRemessa(int numeroRegistro)
        {
            StringBuilder _trailer = new StringBuilder();
            try
            {
                // Campo 1: Tipo de Registro (1 posição) - "9" identifica o trailer do arquivo.
                _trailer.Append("9");

                // Campo 2: Uso do Banco (393 posições) - Preenchido com espaços em branco.
                _trailer.Append(new string(' ', 393));

                // Campo 3: Sequencial (6 posições) - Número sequencial do registro no arquivo.
                _trailer.Append(numeroRegistro.ToString().PadLeft(6, '0'));

                return Utils.SubstituiCaracteresEspeciais(_trailer.ToString());
            }
            catch (Exception e)
            {
                throw new Exception("Erro durante a geração do registro TRAILER do ARQUIVO de REMESSA.", e);
            }
        }

        public override string GerarHeaderRemessa(string numeroConvenio, Cedente cedente, TipoArquivo tipoArquivo, int numeroArquivoRemessa)
        {
            try
            {
                string _header = " ";

                base.GerarHeaderRemessa(numeroConvenio, cedente, tipoArquivo, numeroArquivoRemessa);

                switch (tipoArquivo)
                {

                    case TipoArquivo.CNAB400:
                        _header = GerarHeaderRemessaCNAB400(cedente, numeroArquivoRemessa);
                        break;
                    case TipoArquivo.Outro:
                        throw new Exception("Tipo de arquivo inexistente.");
                }

                return _header;

            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a geração do HEADER do arquivo de REMESSA.", ex);
            }
        }

        public override string GerarHeaderRemessa(string numeroConvenio, Cedente cedente, TipoArquivo tipoArquivo, int numeroArquivoRemessa, Boleto boletos)
        {
            throw new NotImplementedException();
        }

        #endregion Métodos de geração do arquivo remessa - Genéricos

        #region Métodos de geração do arquivo remessa - específicos

        private string GerarHeaderRemessaCNAB400(Cedente cedente, int numeroArquivoRemessa)
        {
            //Variaveis
            StringBuilder _header = new StringBuilder();
            try
            {
                // Campo 1: Tipo de Registro (1 posição) - "0" identifica o header do arquivo.
                _header.Append("0");

                // Campo 2: Código Remessa (1 posição) - "1" para remessa.
                _header.Append("1");

                // Campo 3: Literal Remessa (7 posições) - "REMESSA".
                _header.Append("REMESSA");

                // Campo 4: Código do Serviço (2 posições) - "01" para cobrança.
                _header.Append("01");

                // Campo 5: Literal Serviço (8 posições) - "COBRANCA".
                _header.Append("COBRANCA");

                // Campo 6: Uso do Banco (7 posições) - preenchido com espaços.
                _header.Append(new string(' ', 7));

                // Campo 7: Código do Cedente (12 posições) - preenchido com zeros à esquerda.
                _header.Append(cedente.Codigo.PadLeft(12, '0'));

                // Campo 8: Uso do Banco (8 posições) - preenchido com espaços.
                _header.Append(new string(' ', 8));

                // Campo 9: Nome da Empresa (30 posições) - preenchido com espaços à direita.
                _header.Append(Utils.FitStringLength(cedente.Nome, 30, 30, ' ', 0, true, true, false));

                // Campo 10: Código do Banco (3 posições) - "336" para C6 Bank.
                _header.Append("336");

                // Campo 11: Uso do Banco (15 posições) - preenchido com espaços.
                _header.Append(new string(' ', 15));

                // Campo 12: Data de Gravação (6 posições) - formato DDMMAA.
                _header.Append(DateTime.Now.ToString("ddMMyy"));

                // Campo 13: Uso do Banco (8 posições) - preenchido com espaços.
                _header.Append(new string(' ', 8));

                // Campo 14: Conta Cobrança Direta (12 posições) - preenchido com zeros.
                _header.Append(Utils.FitStringLength(cedente.ContaBancaria.Conta, 12, 12, '0', 0, true, true, true));

                // Campo 15: Uso do Banco (266 posições) - preenchido com espaços.
                _header.Append(new string(' ', 266));

                // Campo 16: Sequencial de Remessa (8 posições) - preenchido com zeros à esquerda.
                _header.Append(numeroArquivoRemessa.ToString().PadLeft(8, '0'));

                // Campo 17: Sequencial (6 posições) - preenchido com "1".
                _header.Append("000001");

                return Utils.SubstituiCaracteresEspeciais(_header.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar HEADER do arquivo de remessa do CNAB400.", ex);
            }
        }

        private string GerarDetalheRemessaCNAB400(Boleto boleto, int numeroRegistro, TipoArquivo tipoArquivo)
        {
            var _detalhe = new StringBuilder();
            try
            {
                // === CAMPOS 1 a 25 – Cabeçalho do Registro Detalhe ===

                // Campo 18: Tipo de Registro (1 posição) – "1"
                _detalhe.Append("1"); // Posição 001

                // Campo 19: Tipo Inscrição Empresa (2 posições) – ex.: "02"
                _detalhe.Append("02"); // Posições 002 a 003

                // Campo 20: CNPJ da Empresa (14 posições)
                _detalhe.Append(Utils.FitStringLength(
                    boleto.Cedente.CPFCNPJ.Replace(".", "").Replace("-", "").Replace("/", ""),
                    14, 14, '0', 0, true, true, true)); // Posições 004 a 017

                // Campo 21: Código da Empresa (12 posições)
                // Utiliza boleto.Cedente.CodigoEmpresa se informado; caso contrário, usa "EMPRESA001"
                _detalhe.Append(Utils.FitStringLength(boleto.Cedente.ContaBancaria.Conta, 12, 12, '0', 0, true, true, true)); // Posições 018 a 029

                // Campo 22: Uso do Banco (8 posições) – brancos
                _detalhe.Append(new string(' ', 8)); // Posições 030 a 037

                // Campo 23: Uso da Empresa (25 posições) – brancos
                _detalhe.Append(new string(' ', 25)); // Posições 038 a 062

                // Campo 24: Nosso Número (11 posições)
                _detalhe.Append(Utils.FitStringLength(FormataNumeroTitulo(boleto), 11, 11, '0', 0, true, true, true)); // Posições 063 a 073

                // Campo 25: Dígito Verificador do Nosso Número (1 posição)
                string digitoNossoNumero = string.IsNullOrEmpty(boleto.DigitoNossoNumero) ? CalcularDigitoNossoNumero(boleto) : boleto.DigitoNossoNumero;
                _detalhe.Append(Utils.FitStringLength(digitoNossoNumero, 1, 1, '0', 0, true, true, false)); // Posição 074

                // === CAMPOS 26 a 61 – Dados do Título e Sacado, Instruções e Totais ===
                // Campo 26: Uso do Banco (8 posições) – brancos
                _detalhe.Append(new string(' ', 8)); // Posições 075 a 082

                // Campo 27: Código do Banco (3 posições) – se carteira = "20", preenche com "336"; caso contrário, brancos
                if (boleto.Carteira == "20")
                    _detalhe.Append("336");
                else
                    _detalhe.Append(new string(' ', 3)); // Posições 083 a 085

                // Campo 28: Uso do Banco (21 posições) – brancos
                _detalhe.Append(new string(' ', 21)); // Posições 086 a 106

                // Campo 29: Código da Carteira (2 posições) – valor numérico de boleto.Carteira
                _detalhe.Append(Utils.FitStringLength(boleto.Carteira, 2, 2, '0', 0, true, true, true)); // Posições 088 a 089

                // Campo 30: Código Ocorrência Remessa (2 posições) ***********************
                _detalhe.Append(Utils.FitStringLength("01", 2, 2, '0', 0, true, true, true)); // Posições 090 a 091

                // Campo 31: Seu Número (10 posições) – normalmente informado pelo cliente **************************
                _detalhe.Append(Utils.FitStringLength(boleto.NumeroDocumento, 10, 10, ' ', 0, true, true, false)); // Posições 092 a 101

                // Campo 32: Data Vencimento (6 posições, formato DDMMAA)
                _detalhe.Append(boleto.DataVencimento.ToString("ddMMyy")); // Posições 102 a 107

                // Campo 33: Valor do Título (13 posições, valor em centavos)
                _detalhe.Append(Utils.FitStringLength(boleto.ValorBoleto.ApenasNumeros(), 13, 13, '0', 0, true, true, true)); // Posições 108 a 120

                // Campo 34: Uso do Banco (8 posições) – brancos
                _detalhe.Append(new string(' ', 8)); // Posições 121 a 128

                // Campo 35: Espécie do Título (2 posições)
                _detalhe.Append(Utils.FitStringLength(boleto.EspecieDocumento.Codigo.ToString(), 2, 2, '0', 0, true, true, true)); // Posições 129 a 130

                // Campo 36: Aceite (1 posição) – usa o primeiro caractere de boleto.Aceite (maiúsculo)
                _detalhe.Append(boleto.Aceite.Substring(0, 1).ToUpper()); // Posição 131

                // Campo 37: Data Emissão do Título (6 posições, formato DDMMAA)
                _detalhe.Append(boleto.DataDocumento.ToString("ddMMyy")); // Posições 132 a 137

                // Campo 38: Instrução 1 (2 posições) – se não informado, utiliza "00"
                string instrucao1 = boleto.Instrucoes.Count() > 0
                    ? boleto.Instrucoes.First().Codigo.ToString().PadLeft(2, '0')
                    : "00";
                _detalhe.Append(instrucao1); // Posições 138 a 139

                // Campo 39: Instrução 2 (2 posições) – se não informado, utiliza "00"
                string instrucao2 = boleto.Instrucoes.Count() > 1
                    ? boleto.Instrucoes.ElementAt(1).Codigo.ToString().PadLeft(2, '0')
                    : "00";
                _detalhe.Append(instrucao2); // Posições 140 a 141

                // Campo 40: Juros ao Dia (13 posições, valor em centavos)
                _detalhe.Append(Utils.FitStringLength(boleto.JurosMora.ApenasNumeros(), 13, 13, '0', 0, true, true, true)); // Posições 142 a 154

                // Campo 41: Data Desconto 1 (6 posições, formato DDMMAA)
                _detalhe.Append(boleto.DataDesconto != DateTime.MinValue ? boleto.DataDesconto.ToString("ddMMyy") : "000000"); // Posições 155 a 160

                // Campo 42: Valor Desconto 1 (13 posições, valor em centavos)
                _detalhe.Append(Utils.FitStringLength(boleto.ValorDesconto.ApenasNumeros(), 13, 13, '0', 0, true, true, true)); // Posições 161 a 173

                // Campo 43: Data Multa (6 posições, formato DDMMAA)
                _detalhe.Append(boleto.DataMulta != DateTime.MinValue ? boleto.DataMulta.ToString("ddMMyy") : "000000"); // Posições 174 a 179

                // Campo 44: Uso do Banco (7 posições) – brancos
                _detalhe.Append(new string(' ', 7)); // Posições 180 a 186

                // Campo 45: Valor Abatimento (13 posições, valor em centavos)
                _detalhe.Append(Utils.FitStringLength(boleto.Abatimento.ApenasNumeros(), 13, 13, '0', 0, true, true, true)); // Posições 187 a 199

                // Campo 46: Tipo Sacado (2 posições) – obtido via identificação do tipo (ex.: "01" ou "02")
                _detalhe.Append(Utils.IdentificaTipoInscricaoSacado(boleto.Sacado.CPFCNPJ)); // Posições 200 a 201

                // Campo 47: CNPJ/CPF Sacado (14 posições)
                _detalhe.Append(Utils.FitStringLength(boleto.Sacado.CPFCNPJ.Replace(".", "").Replace("-", "").Replace("/", ""), 14, 14, '0', 0, true, true, true)); // Posições 202 a 215

                // Campo 48: Nome do Sacado (40 posições)
                _detalhe.Append(Utils.FitStringLength(boleto.Sacado.Nome, 40, 40, ' ', 0, true, true, false)); // Posições 216 a 255

                // Campo 49: Endereço Sacado (40 posições)
                _detalhe.Append(Utils.FitStringLength(boleto.Sacado.Endereco.EndComNumeroEComplemento, 40, 40, ' ', 0, true, true, false)); // Posições 256 a 295

                // Campo 50: Bairro Sacado (12 posições)
                _detalhe.Append(Utils.FitStringLength(boleto.Sacado.Endereco.Bairro, 12, 12, ' ', 0, true, true, false)); // Posições 296 a 307

                // Campo 51: CEP Sacado (8 posições)
                _detalhe.Append(Utils.FitStringLength(boleto.Sacado.Endereco.CEP, 8, 8, '0', 0, true, true, true)); // Posições 308 a 315

                // Campo 52: Cidade Sacado (15 posições)
                _detalhe.Append(Utils.FitStringLength(boleto.Sacado.Endereco.Cidade, 15, 15, ' ', 0, true, true, false)); // Posições 316 a 330

                // Campo 53: UF Sacado (2 posições)
                _detalhe.Append(Utils.FitStringLength(boleto.Sacado.Endereco.UF, 2, 2, ' ', 0, true, true, false)); // Posições 331 a 332

                // Campo 54: Sacador / Mensagem / Código CMC7 (30 posições)
                _detalhe.Append(Utils.FitStringLength("", 30, 30, ' ', 0, true, true, false)); // Posições 333 a 362

                // Campo 55: Tipo de Multa (1 posição)
                // Campo 56: Percentual de Multa (2 posições)
                if (boleto.PercMulta > 0)
                {
                    _detalhe.Append("2");
                    _detalhe.Append(Math.Truncate(boleto.PercMulta).ToString("00")); //Percentual Multa 9(2)V99 - (04) - Somente numeros inteiros
                }
                else
                {
                    _detalhe.Append("0");
                    _detalhe.Append("00");
                }

                // Campo 57: Uso do Banco (1 posição) – branco
                _detalhe.Append(" "); // Posição 366

                // Campo 58: Data Juros Mora (6 posições, formato DDMMAA)
                _detalhe.Append(boleto.DataJurosMora != DateTime.MinValue ? boleto.DataJurosMora.ToString("ddMMyy") : "000000"); // Posições 367 a 372

                // Campo 59: Uso do Banco (2 posições) – default "00"
                _detalhe.Append("00"); // Posições 373 a 374

                // Campo 60: Uso do Banco (1 posição) – branco
                _detalhe.Append(" "); // Posição 375

                // Campo 61: Sequencial (6 posições)
                _detalhe.Append(Utils.FitStringLength(numeroRegistro.ToString(), 6, 6, '0', 0, true, true, true)); // Posições 376 a 381

                // Caso o registro fique com menos de 400 caracteres, completa com espaços
                if (_detalhe.Length < 400)
                    _detalhe.Append(new string(' ', 400 - _detalhe.Length));

                return Utils.SubstituiCaracteresEspeciais(_detalhe.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do arquivo de remessa CNAB400 para C6.", ex);
            }
        }

        private string GerarDetalheRemessaCNAB400Old(Boleto boleto, int numeroRegistro, TipoArquivo tipoArquivo)
        {
            //Variaveis
            var _detalhe = new StringBuilder();

            //Tratamento de erros
            try
            {
                //Montagem do Detalhe
                _detalhe.Append("1"); //Posição 001
                _detalhe.Append(Utils.IdentificaTipoInscricaoSacado(boleto.Cedente.CPFCNPJ)); //Posição 002 a 003
                _detalhe.Append(Utils.FitStringLength(boleto.Cedente.CPFCNPJ.Replace(".", "").Replace("-", "").Replace("/", ""), 14, 14, '0', 0, true, true, true)); //Posição 004 a 017
                _detalhe.Append(new string('0', 6)); //Posição 018 a 029
                _detalhe.Append(Utils.FitStringLength(boleto.Cedente.ContaBancaria.Agencia, 4, 4, '0', 0, true, true, true)); //Posição 018 a 021
                _detalhe.Append(Utils.FitStringLength(boleto.Cedente.ContaBancaria.DigitoAgencia, 1, 1, '0', 0, true, true, true)); //Posição 022
                _detalhe.Append(Utils.FitStringLength(boleto.Cedente.ContaBancaria.Conta, 8, 8, '0', 0, true, true, true)); //Posição 023 a 030
                _detalhe.Append(Utils.FitStringLength(boleto.Cedente.ContaBancaria.DigitoConta, 1, 1, '0', 0, true, true, true)); //Posição 031
                _detalhe.Append(new string('0', 6)); //Posição 032 a 037
                _detalhe.Append(Utils.FitStringLength(boleto.NumeroDocumento, 25, 25, ' ', 0, true, true, false)); //Posição 038 a 62
                _detalhe.Append(Utils.FitStringLength(FormataNumeroTitulo(boleto), 12, 12, '0', 0, true, true, true)); //Posição 063 a 074
                _detalhe.Append(Utils.FitStringLength(boleto.NumeroParcela.ToString(), 2, 2, '0', 0, true, true, true)); //Posição 075 a 076
                _detalhe.Append("00"); //Posição 077 a 078
                _detalhe.Append("   "); //Posição 079 a 081
                _detalhe.Append(" "); //Posição 082
                _detalhe.Append("   "); //Posição 083 a 085
                _detalhe.Append("000"); //Posição 086 a 088
                _detalhe.Append("0"); //Posição 089
                _detalhe.Append("00000"); //Posição 090 a 094
                _detalhe.Append("0"); //Posição 095
                _detalhe.Append(Utils.FitStringLength(boleto.Cedente.NumeroBordero.ToString(), 6, 6, '0', 0, true, true, true)); //Posição 096 a 101
                _detalhe.Append(new string(' ', 4)); //Posição 102 a 105

                // Tipo de emissão"Tipo de Emissão: 1 - Cooperativa 2 - Cliente"
                var tipoDeEmissao = "1";
                if (boleto.TipoEmissao == TipoEmissao.EmissaoPeloCedente)
                    tipoDeEmissao = "2";

                _detalhe.Append(Utils.FitStringLength(tipoDeEmissao, 1, 1, '0', 0, true, true, true)); // Posição 106 a 106

                _detalhe.Append(Utils.FitStringLength(boleto.TipoModalidade, 2, 2, '0', 0, true, true, true));  //Posição 107 a 108
                _detalhe.Append(Utils.FitStringLength(boleto.Remessa.CodigoOcorrencia, 2, 2, '0', 0, true, true, true)); //Posição 109 a 110 - (1)REGISTRO DE TITULOS (2)Solicitação de Baixa
                _detalhe.Append(Utils.FitStringLength(boleto.NumeroDocumento, 10, 10, '0', 0, true, true, true)); //Posição 111 a 120
                _detalhe.Append(boleto.DataVencimento.ToString("ddMMyy")); //Posição 121 a 126
                _detalhe.Append(Utils.FitStringLength(boleto.ValorBoleto.ApenasNumeros(), 13, 13, '0', 0, true, true, true)); //Posição 127 a 139 
                _detalhe.Append(boleto.Banco.Codigo); //Posição 140 a 142
                _detalhe.Append(Utils.FitStringLength(boleto.Cedente.ContaBancaria.Agencia, 4, 4, '0', 0, true, true, true)); //Posição 143 a 146
                _detalhe.Append(Utils.FitStringLength(boleto.Cedente.ContaBancaria.DigitoAgencia, 1, 1, '0', 0, true, true, true)); //Posição 147
                _detalhe.Append(Utils.FitStringLength(boleto.EspecieDocumento.Codigo.ToString(), 2, 2, '0', 0, true, true, true)); //Posição 148 a 149

                _detalhe.Append(boleto.Aceite == "N" ? "0" : "1"); //Posição 150
                _detalhe.Append(boleto.DataProcessamento.ToString("ddMMyy")); //Posição 151 a 156
                _detalhe.Append("07"); //Posição 157 a 158 - NÂO PROTESTAR
                _detalhe.Append("22"); //Posição 159 a 160 - PERMITIR DESCONTO SOMENTE ATE DATA ESTIPULADA
                _detalhe.Append(Utils.FitStringLength(Convert.ToInt32(boleto.PercJurosMora * 10000).ToString(), 6, 6, '0', 1, true, true, true)); //Posição 161 a 166
                _detalhe.Append(Utils.FitStringLength(Convert.ToInt32(boleto.PercMulta * 10000).ToString(), 6, 6, '0', 1, true, true, true)); //Posição 167 a 172
                _detalhe.Append(" "); //Posição 173
                _detalhe.Append(Utils.FitStringLength((boleto.DataDesconto == DateTime.MinValue ? "0" : boleto.DataDesconto.ToString("ddMMyy")), 6, 6, '0', 0, true, true, true)); //Posição 174 a 179
                _detalhe.Append(Utils.FitStringLength(boleto.ValorDesconto.ApenasNumeros(), 13, 13, '0', 0, true, true, true)); //Posição 180 a 192
                _detalhe.Append("9" + Utils.FitStringLength(boleto.IOF.ApenasNumeros(), 12, 12, '0', 0, true, true, true)); //Posição 193 a 205
                _detalhe.Append(Utils.FitStringLength(boleto.Abatimento.ApenasNumeros(), 13, 13, '0', 0, true, true, true)); //Posição 206 a 218
                _detalhe.Append(Utils.IdentificaTipoInscricaoSacado(boleto.Sacado.CPFCNPJ)); //Posição 219 a 220
                _detalhe.Append(Utils.FitStringLength(boleto.Sacado.CPFCNPJ.Replace(".", "").Replace("-", "").Replace("/", ""), 14, 14, '0', 0, true, true, true)); //Posição 221 a 234
                _detalhe.Append(Utils.FitStringLength(boleto.Sacado.Nome, 40, 40, ' ', 0, true, true, false)); //Posição 235 a 274
                _detalhe.Append(Utils.FitStringLength(boleto.Sacado.Endereco.EndComNumeroEComplemento, 37, 37, ' ', 0, true, true, false)); //Posição 275 a 311
                _detalhe.Append(Utils.FitStringLength(boleto.Sacado.Endereco.Bairro, 15, 15, ' ', 0, true, true, false)); //Posição 312 a 326
                _detalhe.Append(Utils.FitStringLength(boleto.Sacado.Endereco.CEP, 8, 8, '0', 0, true, true, true)); //Posição 327 a 334
                _detalhe.Append(Utils.FitStringLength(boleto.Sacado.Endereco.Cidade, 15, 15, ' ', 0, true, true, false)); //Posição 335 a 349
                _detalhe.Append(boleto.Sacado.Endereco.UF); //Posição 350 a 351
                _detalhe.Append(new string(' ', 40)); //Posição 352 a 391 - OBSERVACOES
                _detalhe.Append("00"); //Posição 392 a 393 - DIAS PARA PROTESTO
                _detalhe.Append(" "); //Posição 394
                _detalhe.Append(Utils.FitStringLength(numeroRegistro.ToString(), 6, 6, '0', 0, true, true, true)); //Posição 394 a 400

                //Retorno
                return Utils.SubstituiCaracteresEspeciais(_detalhe.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do arquivo de remessa do CNAB400.", ex);
            }
        }
        #endregion
    }
}