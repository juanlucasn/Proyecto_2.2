using Newtonsoft.Json;
using Proyecto_1.Clases;
using Proyecto_2.Modelos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;
using Proyecto_2.Models;

namespace Proyecto_2
{
    class Program
    {
        public static Conversion conversion = new Conversion();
        public static Currencies currencies = new Currencies();
        public static List<Currencies> listaCurrencies = new List<Currencies>();
        public static List<Ratio> listaRatios = new List<Ratio>();
        public static Cliente cliente = new Cliente();
        public static string json = string.Empty;
        public static string pathJSON = @"C:\Nubimetrics\currencies.json";
        public static string pathCSV = @"C:\Nubimetrics\currencies.csv";
        static async Task Main(string[] args)
        {
            try
            {
                string dir = @"C:\Nubimetrics";
                //Si el directorio no existe, lo creamos
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                //Obtenemos la lista de currencies

                listaCurrencies = await ObtenerCurrencies();
                Console.WriteLine("Recuperando monedas...");

                if (!listaCurrencies.Count.Equals(0))
                {
                    Console.WriteLine("Recuperando conversion con USD...");
                    foreach (var item in listaCurrencies)
                    {
                        //Buscamos la conversion y lo mergeamos con su currency
                        conversion = await ConvertirMoneda(item.id);
                        if (conversion != null) item.todolar = conversion;
                    }

                    json = JsonConvert.SerializeObject(listaCurrencies);

                    //Preparamos el archivo json e insertamos la lista de currencies
                    using (FileStream fs = File.Create(pathJSON))
                    {
                        
                        byte[] info = new UTF8Encoding(true).GetBytes(json);
                        fs.Write(info, 0, info.Length);
                        Console.WriteLine($"El Archivo JSON fue generado con exito. Se guardo en {pathJSON}");
                    }

                    //Preparamos la lista de ratios
                    foreach (var item in listaCurrencies)
                    {
                        if (item.todolar.ratio != null)
                        {
                            Ratio ratio = new Ratio();
                            ratio.ratio = item.todolar.ratio;
                            listaRatios.Add(ratio);
                        } 
                    }

                    //Preparamos el archivo csv e insertamos la lista de ratios
                    var stream = new MemoryStream();
                    var cc = new CsvConfiguration(new System.Globalization.CultureInfo("en-US"));
                    using (var writeFile = new StreamWriter(pathCSV, false, new UTF8Encoding(true)))
                    {
                        var csv = new CsvWriter(writeFile, cc);
                        csv.WriteRecords<Ratio>(listaRatios);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<Conversion> ConvertirMoneda(string id_currency)
        {
            string uri = $"https://api.mercadolibre.com/currency_conversions/search?from={id_currency}&to=USD";
            string apiResponse = await cliente.Client(uri);
            return JsonConvert.DeserializeObject<Conversion>(apiResponse);
        }

        public static async Task<List<Currencies>> ObtenerCurrencies()
        {
            string uri = $"https://api.mercadolibre.com/currencies/";
            string apiResponse = await cliente.Client(uri);
            return JsonConvert.DeserializeObject<List<Currencies>>(apiResponse);
        }
    }
}
