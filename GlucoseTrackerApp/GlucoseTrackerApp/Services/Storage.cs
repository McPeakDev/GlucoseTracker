using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO;

namespace GlucoseTrackerApp.Services
{
    /// <summary>
    /// Storage Service for Auto Login
    /// </summary>
    public static class Storage
    {
        /// <summary>
        /// Default File and Path
        /// </summary>
        private static readonly string _path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        private static readonly string _filename = Path.Combine(_path, "GT.json");

        /// <summary>
        /// Saves a User's Token to a file
        /// </summary>
        /// <param name="token">A User's Identity</param>
        public static async void SaveEmail(string email)
        {
            File.Delete(_filename);

            //Create StringBuilder and StringWriter
            StreamWriter sw = new StreamWriter(_filename);

            //Using the Writer do.....
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                //Set Formatting and Wtire necessary information
                writer.Formatting = Formatting.Indented;
                await writer.WriteStartObjectAsync();
                await writer.WritePropertyNameAsync("Email");
                await writer.WriteValueAsync(email);
                await writer.WriteEndObjectAsync();
                await writer.CloseAsync();
            }
        }

        public static string ReadEmail()
        {
            if (!File.Exists(_filename))
            {
                return null;
            }

            StreamReader streamReader = File.OpenText(_filename);

            string email = null;

            using (JsonTextReader reader = new JsonTextReader(streamReader))
            {
                bool tokenNext = false;

                while (reader.Read())
                {
                    if (reader.Value != null)
                    {
                        if (tokenNext == true)
                        {
                            email = reader.Value.ToString();
                            tokenNext = false;
                        }
                        else if (reader.Value.ToString() == "Email")
                        {
                            tokenNext = true;
                        }

                    }
                }
                reader.Close();
            }

            streamReader.Close();

            if (!(email is null))
            {
                return email;
            }
            return null;
        }

        public static void DeleteFile()
        {
            File.Delete(_filename);
        }
    }
}