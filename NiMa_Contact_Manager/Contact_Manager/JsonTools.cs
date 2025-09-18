using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Contact_Manager
{
    internal class JsonTools // Hilfsklasse zum Speichern und Laden der Kontakte als JSON-Datei
    {
        private readonly string filePath; // //Variable zum Pfad der JSON-Datei

        public JsonTools()
        {
            string exeFolder = AppDomain.CurrentDomain.BaseDirectory; //Ordner in dem die exe läuft
            filePath = Path.Combine(exeFolder, "contacts.json"); // filepath wird mit dem exefolder und dem namen contacts.json erstellt
        }

        // Liste von Personen mit Typ-Infos speichern
        public void Save(List<Person> persons)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,  //   damit beim Laden wieder Employee/Customer/Trainee erkannt wird
                Formatting = Formatting.Indented // macht das JSON schön lesbar
            };
            
            string json = JsonConvert.SerializeObject(persons, settings); // Liste in JSON-Text umwandeln
            File.WriteAllText(filePath, json);  // JSON in Datei schreiben (überschreibt bestehende Datei)
        }

        // Liste von Personen mit Typ-Infos laden
        public List<Person> Load()
        {
            List<Person> persons = new List<Person>(); //Damit wird sichergestellt, dass man immer eine Liste zurückbekommt
            if (File.Exists(filePath))
            {
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All // liest die Typ-Infos aus dem JSON
                };

                string json = File.ReadAllText(filePath); // JSON aus Datei lesen
                persons = JsonConvert.DeserializeObject<List<Person>>(json, settings) ?? new List<Person>(); // JSON zurück in Liste umwandeln
            }
            else
            {
                persons = new List<Person>(); // Datei gibt es noch nicht und gibt eine leere Liste zurück
            }
            return persons;
        }
    }
}
