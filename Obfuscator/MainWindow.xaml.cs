using Microsoft.Win32;
using Obfuscator.Managing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Obfuscator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ReportManager reportManager;
        ObfuscatorManager obfuscatorManager;
        ObfuscatorCore obfuscatorCore;

        public MainWindow()
        {
            InitializeComponent();
            reportWindow.Document.Blocks.Clear();
            descriptionWindow.Document.Blocks.Clear();
            Init();
            dragDropPanel.DragEnter += new System.Windows.DragEventHandler( dragDropPanel_DragEnter );
            dragDropPanel.Drop += new System.Windows.DragEventHandler( dragDropPanel_Drop );

            
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
        }

        private void dragDropPanel_Drop(object sender, System.Windows.DragEventArgs e )
        {
            if(e.Data.GetDataPresent( System.Windows.DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData( System.Windows.DataFormats.FileDrop );
                File.WriteAllText( "FilePath.txt", string.Join( "", files) );
            }
            obfuscatorCore.Init();
            fileName.Text = obfuscatorCore.GetDefaultObfuscatedFileName();
        }

        private void dragDropPanel_DragEnter( object sender, System.Windows.DragEventArgs e )
        {
            if( e.Data.GetDataPresent( System.Windows.DataFormats.FileDrop ) )
            {
                e.Effects = System.Windows.DragDropEffects.Copy;
            }
            else
            {
                e.Effects = System.Windows.DragDropEffects.None;
            }
        }

        private void browseButton_Click( object sender, RoutedEventArgs e )
        {
            FolderBrowserDialog browser = new FolderBrowserDialog();
            if(browser.ShowDialog()==System.Windows.Forms.DialogResult.OK)
            {
                savePath.Text = browser.SelectedPath;
                
            }
        }

        private void startButton_Click( object sender, RoutedEventArgs e )
        {
            reportWindow.Document.Blocks.Clear();
            if(!File.Exists("FilePath.txt"))
            {
                reportManager.AddLine( "No aplication to obfuscate. Please drag and drop app to specified panel." );
            }
            else
            {
                if( profileCheckBox.IsChecked == true )
                {//For profiles
                    obfuscatorManager.CheckProfiles( profileList );
                    obfuscatorCore.StartObfuscation( savePath, fileName, obfuscatorManager );
                }
                else
                {//When there's a custom choice
                    obfuscatorManager.SearchSettings( layoutOptions, dataOptions, controlOptions );
                    if(obfuscatorManager.ObfuscationsToDo.Count == 0)
                    {
                        reportManager.AddLine( "Select obfuscating options." );
                    }
                    else
                    {
                        obfuscatorCore.StartObfuscation( savePath, fileName, obfuscatorManager );
                    }                  
                }
                //File.Delete( "FilePath.txt" );
            }
            
        }

        private void Init()
        {
            
            reportManager = new ReportManager( reportWindow );
            obfuscatorManager = new ObfuscatorManager();
            obfuscatorCore = new ObfuscatorCore( reportManager );
            savePath.Text = Environment.GetFolderPath( Environment.SpecialFolder.Desktop );

        }

        private void profileCheckBox_Unchecked( object sender, RoutedEventArgs e )
        {
            profileList.IsEnabled = false;
        }

        private void profileCheckBox_Checked( object sender, RoutedEventArgs e )
        {
            profileList.IsEnabled = true;
        }

        private void hideNamesRadio_Checked( object sender, RoutedEventArgs e )
        {
            descriptionWindow.Document.Blocks.Clear();
            descriptionWindow.AppendText( "All names will be invisible." );
        }

        private void randomStringsRadio_Checked( object sender, RoutedEventArgs e )
        {
            descriptionWindow.Document.Blocks.Clear();
            descriptionWindow.AppendText( "All names will be changed into a random string of symbols." );
        }

        private void randomWordsRadio_Checked( object sender, RoutedEventArgs e )
        {
            descriptionWindow.Document.Blocks.Clear();
            descriptionWindow.AppendText( "All names will be changed into a random words." );
        }
    }
}
