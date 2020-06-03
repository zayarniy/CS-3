using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Speech.Synthesis;
using System.IO;

namespace LearningNewWords
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SpeechSynthesizer synth = new SpeechSynthesizer();
        Prompt prompt;
        MyDictionary myDictionary = new MyDictionary();

        public MainWindow()
        {
            InitializeComponent();
            // Configure the audio output.   
            synth.SetOutputToDefaultAudioDevice();
            dgWords.ItemsSource = myDictionary.list;
        }

        private void  btnSpeakEnglish_Click(object sender, RoutedEventArgs e)
        {
            

            // Speak a string.  
            prompt=synth.SpeakAsync(tbEnglishWord.Text);

        }

        private void btnTranslateToRussian_Click(object sender, RoutedEventArgs e)
        {
            EnglishLerningUtils utils = new EnglishLerningUtils();
            utils.From = "en";
            utils.To = "ru";
            tbRussianWord.Text=utils.TranslateText(tbEnglishWord.Text);
            //System.Threading.Thread.Sleep(2000);//Waiting response from server
            if (cbSpeak.IsChecked.Value) synth.Speak(tbEnglishWord.Text);


        }

        private void btnTranslateToEnglish_Click(object sender, RoutedEventArgs e)
        {
            EnglishLerningUtils utils = new EnglishLerningUtils();
            utils.From = "ru";
            utils.To = "en";
            tbEnglishWord.Text = utils.TranslateText(tbRussianWord.Text);
            //System.Threading.Thread.Sleep(2000);//Waiting response from server
            if (cbSpeak.IsChecked.Value) synth.Speak(tbEnglishWord.Text);
        }

        private void btnEnglishFromClipboard_Click(object sender, RoutedEventArgs e)
        {
            tbEnglishWord.Text = Clipboard.GetText();
            btnTranslateToRussian.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

        }

        private void btnCancelSpeak_Click(object sender, RoutedEventArgs e)
        {
            
            if (prompt!=null && !prompt.IsCompleted) 
                synth.SpeakAsyncCancel(prompt);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            myDictionary.list.Add(new Word(tbEncounter.Text,tbEnglishWord.Text,tbRussianWord.Text,0));
            
        }

        private void btnWriteList_Click(object sender, RoutedEventArgs e)
        {
            myDictionary.WriteToXML("dic.xml");
            MessageBox.Show("Dictionary have saved successful");
        }

        private void btnLoadList_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists("dic.xml"))
                myDictionary = new MyDictionary("dic.xml");
            dgWords.ItemsSource = myDictionary.list;
        }
    }
}
