using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Pocket_Updater.Forms.Message_Box;
using System.Runtime.CompilerServices;

namespace RetroDriven
{
    public class Updater_Preferences
    {
        public string update_location { get; set; }
        public string update_drive_letter { get; set; }

        public static void Save_Updater_Json(string[] Entry, string Json)
        {
            try
            {
                Updater_Preferences config = new Updater_Preferences();

                config.update_location = Entry[0];
                config.update_drive_letter = Entry[1];

                JsonSerializer serializer = new JsonSerializer();
                using (StreamWriter sw = new StreamWriter(Json))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Formatting = Formatting.Indented;
                    serializer.Serialize(writer, config);
                }
            }
            catch(Exception Ex)
            {
                Message_Box form = new Message_Box();
                form.label1.Text = Ex.Message;
                form.Show();
            }
        }

        public static string Get_Updater_Json(string Entry, string Json)
        {
            try
            {
                using (StreamReader file = File.OpenText(Json)) 
                {
                    JsonSerializer serializer = new JsonSerializer();
                    Updater_Preferences config = (Updater_Preferences)serializer.Deserialize(file,typeof(Updater_Preferences));

                    if (Entry == "Update Location")
                    {
                        return config.update_location;
                    }
                    if (Entry == "Update Drive Letter")
                    {
                        return config.update_drive_letter;
                    }
                }
            }
            catch (Exception Ex)
            {
                Message_Box form = new Message_Box();
                form.label1.Text = Ex.Message;
                form.Show();
            }
            return null;
        }

    }
}
