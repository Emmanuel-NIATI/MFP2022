
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;



// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace Monorail
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class TextToSpeechPage : Page
    {

        private ResourceContext speechContext;
        private ResourceMap speechResourceMap;

        private string nom = "";

        public TextToSpeechPage()
        {
            this.InitializeComponent();

            speechContext = ResourceContext.GetForCurrentView();
            speechContext.Languages = new string[] { SpeechSynthesizer.DefaultVoice.Language };

            speechResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("LocalizationTTSResources");

        }


        private void ComboBox1_SelectionChanged(object sender, RoutedEventArgs e)
        {

            nom = comboBox1.SelectedValue.ToString();
        }

        private async void Speak1_Click(object sender, RoutedEventArgs e)
        {

            MediaElement mediaElement = new MediaElement();
            var synth = new SpeechSynthesizer();
            SpeechSynthesisStream synthesisStream = await synth.SynthesizeTextToStreamAsync("Bonjour" + nom);
            mediaElement.SetSource(synthesisStream, synthesisStream.ContentType);
            mediaElement.Play();

        }

        private async void Speak2_Click(object sender, RoutedEventArgs e)
        {

            MediaElement mediaElement = new MediaElement();
            var synth = new SpeechSynthesizer();
            SpeechSynthesisStream synthesisStream = await synth.SynthesizeTextToStreamAsync("Je suis Gertrude, ta nouvelle assistante DIRISI. Puis-je t'aider ?");
            mediaElement.SetSource(synthesisStream, synthesisStream.ContentType);
            mediaElement.Play();

        }

        private async void Speak3_Click(object sender, RoutedEventArgs e)
        {

            MediaElement mediaElement = new MediaElement();
            var synth = new SpeechSynthesizer();
            SpeechSynthesisStream synthesisStream = await synth.SynthesizeTextToStreamAsync("Pour t'aider, je vais optimiser ton travail. As-tu saisi tes activités dans radium ? As-tu consulté toute ta messagerie ? As-tu consulté Nacre ? Es-tu heureux ou heureuse au Fort de Bicêtre ?");
            mediaElement.SetSource(synthesisStream, synthesisStream.ContentType);
            mediaElement.Play();

        }

        private async void Speak4_Click(object sender, RoutedEventArgs e)
        {

            MediaElement mediaElement = new MediaElement();
            var synth = new SpeechSynthesizer();
            SpeechSynthesisStream synthesisStream = await synth.SynthesizeTextToStreamAsync("Pour t'aider, il faudrait d'abord veiller à recruter. Pour faire venir des candidats, il faudrait revoir les repas servis à la cantine, l'état déplorable des toilettes et mettre en place des fontaines à eau et des distributeurs de café.");
            mediaElement.SetSource(synthesisStream, synthesisStream.ContentType);
            mediaElement.Play();

        }

        private async void Speak5_Click(object sender, RoutedEventArgs e)
        {

            MediaElement mediaElement = new MediaElement();
            var synth = new SpeechSynthesizer();
            SpeechSynthesisStream synthesisStream = await synth.SynthesizeTextToStreamAsync("La bourse, c'est le mal ! Des gens héritent des fortunes et achètent des actions pour gagner de l'argent sur le travail des pauvres. Et en plus, les riches disent que les pauvres sont jaloux !");
            mediaElement.SetSource(synthesisStream, synthesisStream.ContentType);
            mediaElement.Play();

        }

        private async void Speak6_Click(object sender, RoutedEventArgs e)
        {

            MediaElement mediaElement = new MediaElement();
            var synth = new SpeechSynthesizer();
            SpeechSynthesisStream synthesisStream = await synth.SynthesizeTextToStreamAsync("Microsoft a pensé les meilleurs produits pour améliorer la productivité de ses utilisateurs. Windows dix c'est dix fois mieux comme les dix doigts de la main !");
            mediaElement.SetSource(synthesisStream, synthesisStream.ContentType);
            mediaElement.Play();

        }

        private async void Speak7_Click(object sender, RoutedEventArgs e)
        {

            MediaElement mediaElement = new MediaElement();
            var synth = new SpeechSynthesizer();
            SpeechSynthesisStream synthesisStream = await synth.SynthesizeTextToStreamAsync("FO le syndicat qu'il vous faut ! Entre le premier et le huit décembre 2022, pensez au vote électronique, votez force ouvrière !");
            mediaElement.SetSource(synthesisStream, synthesisStream.ContentType);
            mediaElement.Play();

        }

    }
}
