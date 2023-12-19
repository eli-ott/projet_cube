namespace Cube {
    public static class ConsoleLogger {

        /// <summary>
        /// Permet d'écrire un log d'erreur dans la console.
        /// </summary>
        /// <param name="message"> Message qui doit apparaître dans la console. </param>
        public static void LogError(string message) {

            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"ERROR: {DateTime.Now} - {message}");
            Console.ForegroundColor = originalColor;

        } // void ..


        /// <summary>
        /// Permet d'écrire un log d'information dans la console.
        /// </summary>
        /// <param name="message"> Message qui doit apparaître dans la console. </param>
        public static void LogInfo(string message) {

            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"INFO: {DateTime.Now} - {message}");
            Console.ForegroundColor = originalColor;

        } // void ..


        /// <summary>
        /// Permet d'écrire un log d'avertissement dans la console.
        /// </summary>
        /// <param name="message"> Message qui doit apparaître dans la console. </param>
        public static void LogWarning(string message) {

            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"WARNING: {DateTime.Now} - {message}");
            Console.ForegroundColor = originalColor;

        } // void ..
    } // class ..
} // namespace ..
