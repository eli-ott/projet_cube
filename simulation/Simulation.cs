using System.Text;
using System.Text.Json;


namespace Cube {
    public class Simulation {
        
        //=====================
        // D E F I N I T I O N
        //=====================
            
            /** <summary> Référence vers l'instance du client HTTP </summary> **/         private static readonly HttpClient client = new ();
            /** <summary> Référence vers le thread de génération aléatoire </summary> **/ private static readonly Random random     = new ();

            /** <summary> Quantité d'appareil à simuler. </summary> **/        private const int SIMULATED_DEVICES_COUNT      = 5;
            /** <summary> Quantité de type de mesure à simuler. </summary> **/ private const int SIMULATED_MEASURE_TYPE_COUNT = 4;


        //=============================
        // I M P L E M E N T A T I O N
        //=============================

            /// <summary>
            /// Lance une instance parallèle qui envoie des donnée générées aléatoirement vers l'API.
            /// </summary>
            public static async Task Run(bool onlyMeasures = false) {
                for (int device = 1; device <= SIMULATED_DEVICES_COUNT; device++) {
                    if (!onlyMeasures) {
                        await PostRandomMeasureType();
                        await PostRandomDevice(device);
                    } // if ..

                    await PostRandomMeasure(device);
                } // for ..
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
            private static async Task PostRandomDevice(int id) {
                Device device = new() {
                    idAppareil  = "192.168.0.32-" + id,
                    nomAppareil = "Device " + random.Next(),
                    idType      = random.Next(1, SIMULATED_MEASURE_TYPE_COUNT)
                }; // ..

                await PostData("http://localhost:5023/newdevice", device);
            } // Task ..


            /// <summary>
            /// Fait une requête POST pour une mesure aléatoire.
            /// </summary>
            private static async Task PostRandomMeasure(int id) {
                Measure measure = new() {
                    valeur     = random.NextSingle(),
                    instant    = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    idAppareil = "192.168.0.32-" + id,
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
