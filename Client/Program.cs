using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static async Task DoWork(string path)
        {
            var lines = File.ReadLines(path);
            using (var client = new HttpClient())
            {
                var strBuilder = new StringBuilder();
                client.BaseAddress = new Uri("http://localhost:61214/api/");
                foreach (var line in lines)
                {
                    var result = await client.GetAsync("service/" + line);
                    if (!result.IsSuccessStatusCode) continue;
                    var value = await result.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(value)) strBuilder.AppendLine(value);
                }

                File.WriteAllText("out.txt", strBuilder.ToString());
                if (File.Exists("out.txt"))
                {
                    Console.WriteLine("out.txt created successfully");
                }
            }
        }
        static void Main(string[] args)
        {
            string path;
#if DEBUG
            path = "in.txt";
#else
            if (args.Length == 0)
            {
                global::System.Console.WriteLine("Please specify file Name");
                return;
            }
            path = args[0].Trim();
#endif

            if (File.Exists(path))
            {
                DoWork(path).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            else
            {
                Console.WriteLine("File not found, closing program");
                Environment.Exit(0);
            }

            Console.ReadKey();
        }
    }
}
