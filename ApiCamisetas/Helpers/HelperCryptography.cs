using Azure.Security.KeyVault.Secrets;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace ApiCamisetas.Helpers
{
    public class HelperCryptography
    {
        public static string GenerateSalt()
        {
            Random random = new Random();
            string salt = "";
            for(int i = 1; i<=50; i++)
            {
                int aleatorio = random.Next(1, 255);
                char letra = Convert.ToChar(aleatorio);
                salt+=letra;
            }
            return salt;
        }

        //NECESITAMOS SABER SI EL PASSWORD QUE HEMOS ALMACENADO
        //EN BBDD ES IGUAL AL PASSWORD QUE NOS HABRAN DADO EN LA APP
        //ESTE ES UN METODO PARA COMPARAR DOS ARRAYS DE BYTES
        public static bool CompararArrays(byte[] a, byte[] b)
        {
            bool iguales = true;
            //COMPARAMOS EL TAMAÑO
            if (a.Length != b.Length)
            {
                iguales = false;
            }
            else
            {
                //RECORREMOS EL ARRAY a
                for (int i = 0; i < a.Length; i++)
                {
                    //COMPARAMOS BYTE A BYTE
                    if (a[i].Equals(b[i]) == false)
                    {
                        iguales = false;
                        break;
                    }
                }
            }
            return iguales;
        }

        //TENDREMOS UN METODO PARA CIFRAR EL PASSWORD
        //VAMOS A RECIBIR EL PASSWORD A CIFRAR (string) 
        //Y TAMBIEN VAMOS A RECIBIR EL SALT (string)
        //DEVOLVEREMOS UN ARRAY CON EL RESULTADO
        public static byte[] EncryptPassword(string password, string salt)
        {
            string contenido = password + salt;
            SHA512 managed = SHA512.Create();
            //CONVERTIMOS EL CONTENIDO A byte[]
            byte[] salida = Encoding.UTF8.GetBytes(contenido);
            //CREAMOS EL BUCLE DE CIFRADO CON ITERACIONES
            for (int i = 1; i <= 15; i++)
            {
                salida = managed.ComputeHash(salida);
            }
            managed.Clear();
            return salida;
        }

        /*
         * 
         */
        private static IConfiguration configuration;
        private static SecretClient secretClient;
        public static void Initialize(IConfiguration config,SecretClient client)
        {
            configuration = config;
            secretClient = client;
        }

        
        public static string EncryptString(string dato)
        {
            KeyVaultSecret secretSalt = secretClient.GetSecret("Salt");
            var saltconf = secretSalt.Value;
            KeyVaultSecret secretIterate = secretClient.GetSecret("Iterate");
            var bucleconf = secretIterate.Value;
            KeyVaultSecret secretKey = secretClient.GetSecret("Key");
            string passwd = secretKey.Value;

            byte[] saltpassword = EncriptarPasswordSalt
                (passwd, saltconf, int.Parse(bucleconf));
            string res = EncryptString(saltpassword, dato);
            return res;
        }

        public static string DecryptString(string dato)
        {
            KeyVaultSecret secretSalt = secretClient.GetSecret("Salt");
            var saltconf = secretSalt.Value;
            KeyVaultSecret secretIterate = secretClient.GetSecret("Iterate");
            var bucleconf = secretIterate.Value;
            KeyVaultSecret secretKey = secretClient.GetSecret("Key");
            string passwd = secretKey.Value;

            byte[] saltpassword = EncriptarPasswordSalt
                (passwd, saltconf, int.Parse(bucleconf));
            string res = DecryptString(saltpassword, dato);
            return res;
        }

        private static string EncryptString(byte[] key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        private static byte[] EncriptarPasswordSalt(string contenido
            , string salt, int numhash)
        {
            //REALIZAMOS LA COMBINACION DE ENCRIPTADO
            //CON SU SALT
            string textocompleto = contenido + salt;
            //DECLARAMOS EL OBJETO SHA256
            //SHA256Managed objsha = new SHA256Managed();
            SHA256 objsha = SHA256.Create();
            byte[] bytesalida = null;

            try
            {
                //CONVERTIMOS EL TEXTO A BYTES
                bytesalida =
                    Encoding.UTF8.GetBytes(textocompleto);
                //Convert.FromBase64String(textocompleto);
                //ENCRIPTAMOS EL TEXTO 1000 VECES
                for (int i = 0; i < numhash; i++)
                    bytesalida = objsha.ComputeHash(bytesalida);
            }
            finally
            {
                objsha.Clear();
            }
            //DEVOLVEMOS LOS BYTES DE SALIDA
            return bytesalida;
        }

        private static string DecryptString(byte[] key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

    }
}
