using System.Text;
using System.Text.Json;


namespace Cube {
    public class Simulation {

        /** <summary> Référence vers l'instance du client HTTP </summary> **/         private static readonly HttpClient client = new ();
        /** <summary> Référence vers le thread de génération aléatoire </summary> **/ private static readonly Random random     = new ();


        /// <summary>
        /// Lance une instance parallèle qui envoie des donnée générées aléatoirement vers l'API.
        /// </summary>
        public static async Task Run(bool onlyMeasures = false) {
            if (!onlyMeasures) {
                await PostRandomMeasureType();
                await PostRandomDevice();
            } // if ..

            await PostRandomMeasure();
        } // Task ..


        /// <summary>
        /// Fait une requête POST pour un type de mesure aléatoire.
        /// </summary>
        private static async Task PostRandomMeasureType() {
            MeasureType measureType = new() {
                nomType     = "Type " + random.Next(1, 100),
                uniteMesure = "Unit " + random.Next(1, 10),
                limiteMin   = random.NextSingle(),
                limiteMax   = random.NextSingle()
            }; // ..

            await PostData("http://localhost:5023/newmeasuretype", measureType);
        } // Task ..


        /// <summary>
        /// Fait une requête POST pour un appareil aléatoire.
        /// </summary>
        private static async Task PostRandomDevice() {
            Device device = new() {
                idAppareil  = random.Next(0, 10),
                nomAppareil = "Device " + random.Next(),
                idType      = random.Next(1, 10)
            }; // ..

            await PostData("http://localhost:5023/newdevice", device);
        } // Task ..


        /// <summary>
        /// Fait une requête POST pour une mesure aléatoire.
        /// </summary>
        private static async Task PostRandomMeasure() {
            Measure measure = new() {
                valeur     = random.NextSingle(),
                instant    = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                idAppareil = random.Next(1, 10)
            }; // ..

            await PostData("http://localhost:5023/newmeasure", measure);
        } // Task ..


        /// <summary>
        /// Fait une requête POST vers un URL donné.
        /// </summary>
        /// <typeparam name="T"> Le type  </typeparam>
        /// <param name="url">   L'URL cible </param>
        /// <param name="data">  La donnée a transférer </param>
        private static async Task PostData<T>(string url, T data) {

            string jsonData       = JsonSerializer.Serialize(data);
            StringContent content = new (jsonData, Encoding.UTF8, "application/json");
            
            try { HttpResponseMessage response = await client.PostAsync(url, content); }
            catch (Exception ex) { ConsoleLogger.LogError("Impossible de transférer les données vers " + url + " !\n" + ex.Message); }

        } // Task ..
    } // class ..
} // namespace ..
