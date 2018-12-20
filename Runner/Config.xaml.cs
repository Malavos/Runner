using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Runner
{
    /// <summary>
    /// Interaction logic for Config.xaml
    /// </summary>
    public partial class Config : MetroWindow
    {
        public Config()
        {
            InitializeComponent();
            this.WindowState = WindowState.Normal;
            this.Activate();
            //MessageBox.Show(Properties.Settings.Default.AcceptanceTests[0]);
            Loader();
        }

        private void Loader()
        {
            LoadTests();
            LoadImageFolders();
            LoadLayout();
        }

        private void LoadTests()
        {
            if (Properties.Settings.Default.AcceptanceTests != null)
            {
                var tests = Properties.Settings.Default.AcceptanceTests;
                listBoxTests.Items.Clear();

                foreach (var item in tests)
                {
                    ListBoxItem listBoxItem = new ListBoxItem();
                    listBoxItem.Content = item;
                    listBoxTests.Items.Add(listBoxItem);
                }
            }
        }

        private void LoadImageFolders()
        { }


        private void LoadLayout()
        { }

        private async void AbrirDiretorioTeste(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();

            dialog.InitialDirectory = "C:".TrimEnd(new char[] { '\\' });
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var files = Directory.GetFiles(dialog.FileName, "*Cest*.php", SearchOption.TopDirectoryOnly);
                List<string> fileList = files.OfType<string>().ToList();
                var progressDialog = await this.ShowProgressAsync("Aguarde...", "Estou vasculhando os arquivos.");
                progressDialog.SetIndeterminate();

                this.Dispatcher.Invoke(() => ProccessAcceptanceTestFiles(fileList, progressDialog, dialog.FileName));
            }
        }

        private async void ProccessAcceptanceTestFiles(List<string> files, ProgressDialogController dialog, string directory)
        {
            listBoxTests.Items.Clear();

            foreach (var file in files)
            {

                TextReader textReader = File.OpenText(file);
                if (!string.IsNullOrEmpty(textReader.ReadToEnd()))
                {
                    ListBoxItem listBoxItem = new ListBoxItem();
                    listBoxItem.Content = file;
                    listBoxTests.Items.Add(listBoxItem);
                }
                textReader.Close();
            }
            WriteToSettings(listBoxTests.Items, directory);
            Dispatcher.Invoke(() => flyOutTests.IsOpen = true);
            textBoxTestsDirectory.Text = directory;
            await dialog.CloseAsync();
        }

        private void WriteToSettings(object listBoxTests, string directory)
        {

            System.Collections.Specialized.StringCollection acceptanceTests = new System.Collections.Specialized.StringCollection();

            foreach (ListBoxItem item in listBoxTests as ItemCollection)
            {
                acceptanceTests.Add(item.Content.ToString());
            }
            Properties.Settings.Default.AcceptanceTests = acceptanceTests;
            Properties.Settings.Default.AcceptanceTestsFolder = directory;
            Properties.Settings.Default.Save();
        }

        private void InstallCoverlet(object sender, RoutedEventArgs e)
        {
            //ProcessStartInfo processInfo = new ProcessStartInfo();
            //processInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //processInfo.FileName = "cmd.exe";
            Process.Start("cmd", "/k dotnet tool install --global coverlet.console");
        }

        private void InstallReportGenerator(object sender, RoutedEventArgs e)
        {
            Process.Start("cmd", "/k dotnet tool install --global dotnet-reportgenerator-globaltool");
        }

        private void LookupTests(object sender, RoutedEventArgs e)
        {
            flyOutTests.IsOpen = true;
        }

        private async void SelectXUnitFolder(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();

            dialog.InitialDirectory = "C:".TrimEnd(new char[] { '\\' });
            dialog.IsFolderPicker = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var files = Directory.GetFiles(dialog.FileName, "*Test*.cs", SearchOption.TopDirectoryOnly);
                List<string> fileList = files.OfType<string>().ToList();
                var progressDialog = await this.ShowProgressAsync("Aguarde...", "Estou vasculhando os arquivos.");
            }
        }


        private void RevalidarDiretorio(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxTestsDirectory.Text))
            {
                flyOutTests.IsOpen = true;
                progressRingFlyout.IsActive = true;
                listBoxTests.Items.Clear();
                var files = Directory.GetFiles(textBoxTestsDirectory.Text, "*Cest*.php", SearchOption.TopDirectoryOnly);
                List<string> fileList = files.OfType<string>().ToList();
                foreach (var file in files)
                {

                    TextReader textReader = File.OpenText(file);
                    if (!string.IsNullOrEmpty(textReader.ReadToEnd()))
                    {
                        ListBoxItem listBoxItem = new ListBoxItem();
                        listBoxItem.Content = file;
                        listBoxTests.Items.Add(listBoxItem);
                    }
                    textReader.Close();
                }

                progressRingFlyout.IsActive = false;
            }
            else
            {
                this.ShowMessageAsync("Erro", "Não foi informado um diretório!");
            }
        }

        /// <summary>
        /// Helper. Busca pelos tipos de objetos definidos no depObj.
        /// Usado para buscar todos os objetos de um tipo.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="depObj"></param>
        /// <returns></returns>
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

    }
}