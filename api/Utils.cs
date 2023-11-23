namespace Cube {
    public static class Utils {

        /// <summary>
        /// Convertis une clé de ville en structure adaptée au format JSON
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Dictionary<string, string>? ParseCityKey(this string self) {
            
            if (self.Length >= 5) {
                string zipCode  = self[0..5];
                string cityName = self[5..^0];
                
                return new Dictionary<string, string>() {
                    { "zipCode", zipCode },
                    { "cityName", cityName }
                }; // ..
            } else return null;
        } // Dictionary ..
    } // class ..
} // namespace ..
