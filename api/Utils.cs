namespace Cube.Data {
    public static class Utils {

        /// <summary>
        ///       Transforme la concatenation d'un IPV4 et d'un identifiant 32 bit en un identifiant 64 bit.
        /// <br/> Exemple: "192.168.0.32-1" devient 4831881408
        /// </summary>
        /// <param name="self"> La concatenation d'un IPV4 et d'un identifiant 32 bit. </param>
        /// <returns> Un identifiant 64 bit si le format correspond, autrement retourne `null`. </returns>
        public static long? ToDeviceBinaryID(this string self) {
            string[] arguments = self.Split('-', '.');

            try {
                long id = (long)int.Parse(arguments[0])
                | ((long)int.Parse(arguments[1])) <<  8
                | ((long)int.Parse(arguments[2])) << 16
                | ((long)int.Parse(arguments[3])) << 24
                | ((long)int.Parse(arguments[4])) << 32;
                return id;
            } catch { return null; }
        } // int ..


        /// <summary>
        ///       Transforme un identifiant 64 bit sous sa forme concaténée d'un IPV4 et d'un identifiant 32 bit.
        /// <br/> Exemple: 4831881408 devient "192.168.0.32-1"
        /// </summary>
        /// <param name="self"> L'identifiant 64 bit </param>
        /// <returns> Forme IPV4-ID. </returns>
        public static string ToDeviceStringID(this long self) =>
            $"{self & 0xFF}.{(self >> 8) & 0xFF}.{(self >> 16) & 0xFF}.{(self >> 24) & 0xFF}-{self >> 32}";

    } // class ..
} // namespace ..
