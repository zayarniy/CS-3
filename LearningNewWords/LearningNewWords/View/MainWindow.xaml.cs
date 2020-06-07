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
using System.Data;
using System.Windows.Threading;

namespace LearningNewWords
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SpeechSynthesizer synth = new SpeechSynthesizer();
        Prompt prompt;
        ViewModel.LearningNewWords.MyDictionary myDictionary = new ViewModel.LearningNewWords.MyDictionary();

        public MainWindow()
        {
            InitializeComponent();
            // Configure the audio output.   
            synth.SetOutputToDefaultAudioDevice();
            this.DataContext = myDictionary;
            //dgWords.ItemsSource = myDictionary.list;
            for(int i=55;i<=80;i++)
                cbEncounter.Items.Add("Encounter "+i);
            cbEncounter.Items.Add("Разное ");
        }

        private void  btnSpeakEnglish_Click(object sender, RoutedEventArgs e)
        {
            

            // Speak a string.  
            prompt=synth.SpeakAsync(tbEnglishWord.Text);

        }

        private void btnTranslateToRussian_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.LearningNewWords.EnglishLerningUtils utils = new ViewModel.LearningNewWords.EnglishLerningUtils();
            utils.From = "en";
            utils.To = "ru";
            tbRussianWord.Text=utils.TranslateText(tbEnglishWord.Text);
            //System.Threading.Thread.Sleep(2000);//Waiting response from server
            if (cbSpeak.IsChecked.Value) synth.Speak(tbEnglishWord.Text);
        }

        private void btnTranslateToEnglish_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.LearningNewWords.EnglishLerningUtils utils = new ViewModel.LearningNewWords.EnglishLerningUtils();
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
            myDictionary.List.Add(new Word(cbEncounter.Text,tbEnglishWord.Text,tbRussianWord.Text,0));
            
        }

        private void btnWriteList_Click(object sender, RoutedEventArgs e)
        {
            myDictionary.WriteToXML("dic.xml");
            MessageBox.Show("Dictionary have saved successful");
        }

        private void btnLoadList_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists("dic.xml"))
                myDictionary = new ViewModel.LearningNewWords.MyDictionary("dic.xml");
            dgWords.ItemsSource = myDictionary.List;
        }


        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgWords.SelectedItem!=null)
            {
                myDictionary.List.Remove(dgWords.SelectedItem as Word);
            }
        }

        private void dgWords_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            //DataRowView newRow = (DataRowView)dgWords.SelectedItem;
            if (dgWords.SelectedItem as Word==null) return;
            Word word = (Word)dgWords.SelectedItem;
            tbEnglishWord.Text = word.EnglishWord;
            tbRussianWord.Text = word.RussianWord;         
            if (cbSpeak.IsChecked.Value)
                prompt = synth.SpeakAsync(tbEnglishWord.Text);
        }

        private void wndMain_Loaded(object sender, RoutedEventArgs e)
        {
            //myDictionary = new MyDictionary("dic.xml");
            btnLoadList.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private void TabItem_GotFocus(object sender, RoutedEventArgs e)
        {
            BindingExpression expression = cbThemes.GetBindingExpression(ComboBox.ItemsSourceProperty);
            expression.UpdateTarget();
        }

        List<Word> words;
        Random random = new Random();
        DispatcherTimer timer=new DispatcherTimer();

        private void btStart_Click(object sender, RoutedEventArgs e)
        {
            if (timer.IsEnabled)
            {
                timer.Stop();
                return;
            }
            if (cbEncounter.Text == "") return;
            words = myDictionary.GetWords(cbThemes.Text).ToList();
            if (words.Count == 0) return;
            //timer = new DispatcherTimer();
            timer = new DispatcherTimer();
            timer.Interval=new TimeSpan(0,0,(int)slInterval.Value);
            timer.Tick += Timer_Tick;
            Timer_Tick(sender, EventArgs.Empty);
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Word word = words[random.Next(0, words.Count)];
            tbWord.Text = word.EnglishWord;
            tbTranslate.Text = word.RussianWord;
            prompt = synth.SpeakAsync(tbWord.Text);
        }

        private void tbFindWord_TextChanged(object sender, TextChangedEventArgs e)
        {
            dgWords.ItemsSource = null;
            dgWords.ItemsSource = myDictionary.Overlap(tbFindWord.Text);
        }

        private void tbFindWord_LostFocus(object sender, RoutedEventArgs e)
        {
            dgWords.ItemsSource = null;
            dgWords.ItemsSource = myDictionary.List;
        }
    }
}
