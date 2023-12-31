using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Cube.Data {
    public static class Utils {

        /// <summary>
        ///       Transforme la concatenation d'un IPV4 et d'un identifiant 32 bit en un identifiant 64 bit.
        /// <br/> Exemple: "192.168.0.32-1" devient 4831881408
        /// </summary>
        /// <param name="self"> La concatenation d'un IPV4 et d'un identifiant 32 bit. </param>
        /// <returns> Un identifiant 64 bit si le format correspond, autrement retourne `null`. </returns>
        public static long? ToDeviceBinaryID(this string self) {
            try {
                string[] arguments = self.Split('-');
                string[] bytes     = arguments[0].Split('.');
                long id = (long)int.Parse(bytes[0])
                | ((long)int.Parse(bytes[1]))     <<  8
                | ((long)int.Parse(bytes[2]))     << 16
                | ((long)int.Parse(bytes[3]))     << 24
                | ((long)int.Parse(arguments[1])) << 32;
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


        /// <summary>
        /// Permets d'obtenir une adresse IPV4 en fonction d'un type d'interface réseau.
        /// </summary>
        /// <returns> L'adresse IPV4 sous forme de chaîne de caractère pouvant être nulle en cas d'échec. </returns>
        public static string? GetLocalIPv4() {
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
                if (item.NetworkInterfaceType == NetworkInterfaceType.Ethernet && item.OperationalStatus == OperationalStatus.Up)
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                            return ip.Address.ToString();

            return null;
        } // string ..
    } // class ..
} // namespace ..
