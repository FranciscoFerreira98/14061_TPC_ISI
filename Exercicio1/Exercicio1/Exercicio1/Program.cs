/*
 * ESI 2020/21- ISI
 * Aula 09 20.out.2020     ETL - Expressões Regulares em C# 
 *                              (e continuação da leitura ficheiros JSON)
 * Oscar Ribeiro
 */


using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Aula09_20201020_Projeto
{
    public class Detalhe
    {
        public int globalIdLocal { get; set; }
        public string cidade { get; set; }

    }
    class Program
    {

        // <summary>
        ///     leitura do ficheiro com a informação acerca dos locais: locais.csv
        /// </summary>
        /// <param name="ficheiro"> caminho ficheiro .csv</param>
        static Dictionary<int, string> LerLocais(string ficheiro)
        {

            Dictionary<int, string> dicLocais = new Dictionary<int, string>();

            // Expressão Regular para instanciar objeto Regex
            String erString = @"^[0-9]{7},[123],([1-9]?\d,){2}[A-Z]{3},([^,\n]*)$";


            // -> Processar o conteúdo do ficheiro
            // -----------------------------------------------
            // Alternativa 01: ler tudo da Stream para uma string 
            //                 e depois passa a processar o texto da string
            //string csvString = null;
            //using (StreamReader reader =  new StreamReader(ficheiro))
            //{
            //    csvString = reader.ReadToEnd();
            //}
            //MatchCollection matches = Regex.Matches(csvString, erString, 
            //                                         RegexOptions.Multiline);
            //// RegexOptions.Multiline permite interpretar a String com tendo várias 
            //// linhas (caracter '\n'), permitindo usar os operadores de início e fim de linha  ^ $

            //foreach (Match m in matches)
            //{
            //    // m corresponderá ao conteúdo de cada linha do ficheiro (considerando que respeita a ER)  
            //    //  Console.WriteLine(m.Value);
            //}
            // FIM da alternativa 01


            // Alternativa 02: depois de ler o conteúdo do ficheiro para uma stream, 
            //                    vai acedendo "linha-a-linha"
            Regex g = new Regex(erString);
            using (StreamReader r = new StreamReader(ficheiro))
            {
                string line;

                while ((line = r.ReadLine()) != null)
                {
                    // Tenta correspondência (macthing) da ER com cada linha do ficheiro
                    Match m = g.Match(line);
                    if (m.Success)
                    {
                        //  estrutura de cada linha com correspondência:
                        //      globalIdLocal,idRegiao,idDistrito,idConcelho,idAreaAviso,local
                        //  separar pelas vírgulas...
                        string[] campos = m.Value.Split(',');
                        int codLocal = int.Parse(campos[0]);
                        string cidade = campos[5];
                        // Guardar na estrutura de dados dicionário
                        // dicLocais.Add( CHAVE ,  VALOR )
                        dicLocais.Add(codLocal, cidade);
                    }
                    else
                    {
                        Console.WriteLine($"Linha inválida: {line}");
                    }
                }
            }
            return dicLocais;
        }


        static PrevisaoIPMA LerFicheiroPrevisao(int globalIdLocal)
        {
            String jsonString = null;
            using (StreamReader reader =
                       new StreamReader(@"../../../data_forecast/" + globalIdLocal + ".json"))
            {
                jsonString = reader.ReadToEnd();
            }
            PrevisaoIPMA obj = JsonSerializer.Deserialize<PrevisaoIPMA>(jsonString);
            return obj;
        }


        static void Main(string[] args)
        {
            Dictionary<int, string> dicLocais = LerLocais(@"../../../locais.csv");

            //List<Detalhe> detalhe = new List<Detalhe>();
            // Apenas para mostrar o conteúdo da estrutura dicinário...
            foreach (KeyValuePair<int, string> kv in dicLocais)
            {
                Console.WriteLine($"globalIdLocal= {kv.Key} cidade= {kv.Value}");

                Detalhe detalhe = new Detalhe
                {
                    globalIdLocal = kv.Key,
                    cidade = kv.Value,
                };
                File.WriteAllText(@"../../" + detalhe.globalIdLocal + "-detalhe.json", JsonConvert.SerializeObject(detalhe));

                //XmlSerializer writer = new XmlSerializer(detalhe.GetType());

                var path = @"../../" + detalhe.globalIdLocal + "-detalhe.xml";
                //System.IO.FileStream file = System.IO.File.Create(path);

                //writer.Serialize(file, detalhe);

                using (var writer = new StreamWriter(path))
                {
                    XmlSerializer serializer = new XmlSerializer(detalhe.GetType());
                    serializer.Serialize(writer, detalhe);
                    writer.Flush();
                }

                // para cada linha do ficheiro CSV ... 
                PrevisaoIPMA previsaoIPMA = LerFicheiroPrevisao(kv.Key);

                previsaoIPMA.local = kv.Value;

            }




            Console.ReadKey();


        }
    }
}
