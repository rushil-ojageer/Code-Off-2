using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeOff2
{
    class Program
    {
        public static void WriteFile(List<string> lines)
        {
            using (var file = new StreamWriter(@"..\..\..\..\output.txt"))
            {
                foreach (var line in lines)
                {
                    file.WriteLine(line);
                }
            }
        }
        public static List<string> ReadFile(string filename)
        {
            var lines = new List<string>();
            string line;

            var file = new StreamReader(filename);
            while ((line = file.ReadLine()) != null)
            {
                lines.Add(line);
            }

            file.Close();
            return lines;
        }
        public static ParsedInput Parse(string filename)
        {
            var lines = ReadFile("code_off-2.in");

            var liquidTypes = new List<LiquidType>();
            for (var i = 1; i < Int32.Parse(lines[0]) + 1; i++)
            {
                var liquidType = new LiquidType();
                liquidType.Number = i - 1;
                liquidType.Litres = Int32.Parse(lines[i]);

                liquidTypes.Add(liquidType);
            }

            var jars = new List<Jar>();
            var jarsStartPoint = Int32.Parse(lines[0]) + 2;

            for (var i = jarsStartPoint; i < lines.Count; i++)
            {
                var parts = lines[i].Split(',');

                var jar = new Jar
                {
                    Number = i - jarsStartPoint, 
                    MaxLitres = Int32.Parse(parts[0])
                };

                for (int j = 1; j < parts.Length; j++)
                {
                    jar.LiquidTypes.Add(Int32.Parse(parts[j]));
                }

                jars.Add(jar);
            }

            var parsedInput = new ParsedInput();
            parsedInput.Jars = jars;
            parsedInput.LiquidTypes = liquidTypes;

            return parsedInput;
        }

        /*
         * This method is badly names - its does not optimization whatsoever 
         */
        public static void Optimize(ParsedInput input)
        {
            foreach (var liquid in input.LiquidTypes)
            {
                var availableJars = input.Jars.Where(x => x.CurrentLitres < x.MaxLitres 
                    && x.LiquidTypes.Contains(liquid.Number)
                    && (x.CurrentLiquidType == null || x.CurrentLiquidType == liquid.Number));

                foreach (var availableJar in availableJars)
                {
                    var litresToAssign = liquid.Litres < availableJar.MaxLitres
                        ? liquid.Litres
                        : availableJar.MaxLitres;

                    if (availableJar.CurrentLiquidType == null)
                    {
                        availableJar.CurrentLiquidType = liquid.Number;
                    }

                    availableJar.CurrentLitres += litresToAssign;
                    liquid.Litres -= litresToAssign;
                }
            }
        }

        public static void GenerateOutput(ParsedInput parsedInput)
        {
            var lines = new List<string>();
            var remainingLiquid = parsedInput.LiquidTypes.Sum(x => x.Litres) -
                                  parsedInput.Jars.Sum(x => x.CurrentLitres);

            lines.Add(remainingLiquid.ToString());

            for (var i = 0; i < parsedInput.Jars.Count(); i++)
            {
                var jar = parsedInput.Jars[i];

                if (jar.CurrentLiquidType != null)
                {
                    lines.Add(jar.CurrentLiquidType + "," + jar.CurrentLitres);
                }
                else
                {

                    lines.Add(jar.LiquidTypes.First()+",0");
                }
            }

            WriteFile(lines);
        }

        public static void Main(string[] args)
        {
            var parsedInput = Parse("code_off-2.in");
            Optimize(parsedInput);
            GenerateOutput(parsedInput);
            Console.ReadLine();
        }

        public class LiquidType
        {
            public int Number { get; set; }
            public int Litres { get; set; }
        }
        public class Jar
        {
            public int Number { get; set; }
            public int MaxLitres { get; set; }
            public int CurrentLitres { get; set; }
            public int? CurrentLiquidType { get; set; }
            public List<int> LiquidTypes { get; set; }
 
            public Jar()
            {
                LiquidTypes = new List<int>();
            }
        }
        public class ParsedInput
        {
            public List<LiquidType> LiquidTypes { get; set; } 
            public List<Jar> Jars { get; set; } 
        }
    }
}
