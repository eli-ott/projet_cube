namespace Cube.Data {
    public static class Utils {

        /// <summary>
        /// Transforme la concatenation d'un IPV4 et d'un identifiant 32 bit en un identifiant 64 bit.
        /// </summary>
        /// <param name="self"> La concatenation d'un IPV4 et d'un identifiant 32 bit. </param>
        /// <returns> Un identifiant 64 bit. </returns>
        public static int ToDeviceBinaryID(this string self) {
            string[] arguments = self.Split('-', '.');
            return int.Parse(arguments[0])
            | int.Parse(arguments[1]) <<  8
            | int.Parse(arguments[2]) << 16
            | int.Parse(arguments[3]) << 24
            | int.Parse(arguments[4]) << 32;
        } // int ..
    } // class ..
} // namespace ..
