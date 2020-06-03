using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//You must add a reference to System.Web.Extensions. Then add the following using directives:
using System.Net.Http;
using System.Collections;
using System.Web.Script.Serialization;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;

namespace LearningNewWords
{
    public class Word
    {
        public string Encounter { get; set; }
        public string EnglishWord { get; set; }
        public string RussianWord { get; set; }
        public DateTime Time { get; set; }//Дата добавления
        public int Complicated { get; set; }//Сложность

        public Word(string Encounter, string EnglishWord,string RussianWord, int Complicated)
        {
            this.Encounter = Encounter;
            this.EnglishWord = EnglishWord;
            this.RussianWord = RussianWord;
            this.Complicated = Complicated;
            this.Time = DateTime.Now;
        }

        public Word()
        {

        }
        

    }

    

    public class MyDictionary
    {
        public ObservableCollection<Word> list { get; set; } = new ObservableCollection<Word>();

        public MyDictionary(string filename)
        {
            FileStream stream = null;
            List<Word> temp = new List<Word>();
            try
            {
                XmlSerializer xml = new XmlSerializer(typeof(List<Word>));
                stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                temp = (List<Word>)xml.Deserialize(stream);
                stream.Close();
            }
            catch(Exception exc)
            {
                Debug.WriteLine(exc.Message);
            }
            finally
            {
                stream?.Close();
            }
            foreach (Word word in temp)
                list.Add(word);
            
        }


        
        public MyDictionary()
        {

        }

        public void WriteToXML(string filename)
        {

            //List<Word> words = GetList();
            XmlSerializer xml = new XmlSerializer(typeof(List<Word>));
            FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.Write);
            xml.Serialize(stream, list.ToList());

            stream.Close();
        }
    }

    class EnglishLerningUtils
    {
        public string From { get; set; } = "en";
        public string To { get; set; } = "ru";

        public void ExcangeDirTrans()
        {
            string t = From;
            From = To;
            To = t;
        }
                
        public string TranslateText(string input)
        {
            // Set the language from/to in the url (or pass it into this function)
            string url = String.Format
            ("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}",
             From, To, Uri.EscapeUriString(input));
            HttpClient httpClient = new HttpClient();
            string result = httpClient.GetStringAsync(url).Result;
            // Get all json data
            var jsonData = new JavaScriptSerializer().Deserialize<List<dynamic>>(result);
            // Extract just the first array element (This is the only data we are interested in)
            var translationItems = jsonData[0];

            // Translation Data
            string translation = "";

            // Loop through the collection extracting the translated objects
            foreach (object item in translationItems)
            {
                // Convert the item array to IEnumerable
                IEnumerable translationLineObject = item as IEnumerable;

                // Convert the IEnumerable translationLineObject to a IEnumerator
                IEnumerator translationLineString = translationLineObject.GetEnumerator();

                // Get first object in IEnumerator
                translationLineString.MoveNext();

                // Save its value (translated text)
                translation += string.Format(" {0}", Convert.ToString(translationLineString.Current));
            }

            // Remove first blank character
            if (translation.Length > 1) { translation = translation.Substring(1); };

            // Return translation
            return translation;
        }
    }
}
