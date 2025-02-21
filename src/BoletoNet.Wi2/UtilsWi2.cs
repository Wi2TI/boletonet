using System;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;

namespace BoletoNet.Wi2
{
    public static class UtilsWi2
    {
        /// <summary>
        /// Converte um inteiro para booleano.
        /// Retorna true se o valor for diferente de zero, caso contrário false.
        /// </summary>
        /// <param name="value">Valor inteiro a ser convertido.</param>
        /// <returns>Booleano correspondente.</returns>
        public static bool ConvertIntToBool(int value)
        {
            return value != 0;
        }

        /// <summary>
        /// Converte uma imagem para uma string Base64.
        /// Permite redimensionar a imagem e ajustar o DPI caso os parâmetros targetWidth e targetHeight sejam informados.
        /// Se não forem informados, a imagem original será convertida.
        /// </summary>
        /// <param name="imagePath">Caminho completo para o arquivo de imagem.</param>
        /// <param name="targetWidth">Largura desejada para o redimensionamento (opcional).</param>
        /// <param name="targetHeight">Altura desejada para o redimensionamento (opcional).</param>
        /// <param name="dpi">Valor de DPI desejado (padrão 96, ajustar para impressões de alta qualidade, ex: 300).</param>
        /// <returns>String Base64 da imagem processada.</returns>
        public static string ImageToBase64(string imagePath, int? targetWidth = null, int? targetHeight = null, float dpi = 96)
        {
            if (!File.Exists(imagePath))
                throw new FileNotFoundException("Arquivo de imagem não encontrado.", imagePath);

            // Se dimensões foram informadas, realiza o redimensionamento e ajuste de DPI
            if (targetWidth.HasValue && targetHeight.HasValue)
            {
                using (var originalImage = System.Drawing.Image.FromFile(imagePath))
                {
                    using (var resizedImage = new Bitmap(targetWidth.Value, targetHeight.Value))
                    {
                        resizedImage.SetResolution(dpi, dpi);

                        using (var graphics = Graphics.FromImage(resizedImage))
                        {
                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            graphics.SmoothingMode = SmoothingMode.HighQuality;
                            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            graphics.CompositingQuality = CompositingQuality.HighQuality;
                            graphics.DrawImage(originalImage, 0, 0, targetWidth.Value, targetHeight.Value);
                        }

                        using (var ms = new MemoryStream())
                        {
                            resizedImage.Save(ms, ImageFormat.Png);
                            return Convert.ToBase64String(ms.ToArray());
                        }
                    }
                }
            }
            else // Caso não seja necessário redimensionar, converte a imagem original
            {
                byte[] imageBytes = File.ReadAllBytes(imagePath);
                return Convert.ToBase64String(imageBytes);
            }
        }
    }
}
