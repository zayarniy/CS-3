using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningNewWords.ViewModel
{
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


        public class MyDictionary
        {

            public ObservableCollection<Word> List { get; set; } = new ObservableCollection<Word>();
            public List<string> themes = new List<string>();

            
            public MyDictionary(string filename)
            {
                FileStream stream = null;
                try
                {
                    stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    XmlSerializer xml = new XmlSerializer(typeof(ObservableCollection<Word>));
                    List = (ObservableCollection<Word>)xml.Deserialize(stream);
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc.Message);
                }
                finally
                {
                    stream?.Close();
                }
                foreach (Word word in List)
                    if (!themes.Contains(word.Encounter)) themes.Add(word.Encounter);

            }


            public List<string> Themes
            {
                get
                {
                    //themes.Clear();
                    return themes;
                }
            }


            public MyDictionary():this("dic.xml")
            {

            }

            public IEnumerable<Word> GetWords(string Theme)
            {
                return from word in List where word.Encounter==Theme select word;
            }

            public IEnumerable<Word> Overlap(string s)
            {
                //var res = from word in List where LevenshteinDistance(word.EnglishWord,s)<s.Length select word;
                var res = from word in List where Check(s,word.EnglishWord) select word;
                return res;
            }

            //Просто сравниваем два множества символов
            public bool Check(string searchingWord, string secondWord)
            {
                var hs1 = new HashSet<char>(searchingWord.ToCharArray());
               return hs1.Intersect(secondWord.ToCharArray()).Count() == hs1.Count;
               
            }
            //Редакционное расстояние
            static int LevenshteinDistance(string firstWord, string secondWord)
            {
                var n = firstWord.Length + 1;
                var m = secondWord.Length + 1;
                var matrixD = new int[n, m];

                const int deletionCost = 1;
                const int insertionCost = 1;

                for (var i = 0; i < n; i++)
                {
                    matrixD[i, 0] = i;
                }

                for (var j = 0; j < m; j++)
                {
                    matrixD[0, j] = j;
                }

                for (var i = 1; i < n; i++)
                {
                    for (var j = 1; j < m; j++)
                    {
                        var substitutionCost = firstWord[i - 1] == secondWord[j - 1] ? 0 : 1;

                        matrixD[i, j] = Math.Min(matrixD[i - 1, j] + deletionCost, Math.Min(          /* удаление*/
                                                matrixD[i, j - 1] + insertionCost,         /* вставка*/
                                                matrixD[i - 1, j - 1] + substitutionCost)); /* замена*/
                    }
                }

                return matrixD[n - 1, m - 1];
            }
            public void WriteToXML(string filename)
            {

                //List<Word> words = GetList();
                XmlSerializer xml = new XmlSerializer(typeof(List<Word>));
                FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.Write);
                xml.Serialize(stream, List.ToList());

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

}
