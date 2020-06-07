using System;


namespace LearningNewWords
{
    public class Word
    {
        public string Encounter { get; set; }
        public string EnglishWord { get; set; }
        public string RussianWord { get; set; }
        public DateTime Time { get; set; }//Дата добавления
        public int Complicated { get; set; }//Сложность
        public bool IsKnow { get; set; }//Сложность

        public Word(string Encounter, string EnglishWord,string RussianWord, int Complicated)
        {
            this.Encounter = Encounter;
            this.EnglishWord = EnglishWord;
            this.RussianWord = RussianWord;
            this.Complicated = Complicated;
            this.Time = DateTime.Now;
            this.IsKnow = false;
        }

        public Word()
        {

        }
        

    }

    

    
}
