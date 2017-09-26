using Mono.Cecil;
using Obfuscator.Data;
using Obfuscator.Layout;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Obfuscator.Managing
{
    class ObfuscatorCore
    {
        private string fileName;
        private AssemblyDefinition assembly;
        private ReportManager reportManager;
        private LayoutTransformations layoutTransformations;
        private DataTransformations dataTransformations;

        public ObfuscatorCore(ReportManager reportManager )
        {
            this.reportManager = reportManager;
        }

        public void Init()
        {
            fileName = File.ReadAllText( "FilePath.txt" );
            var readerParameters = new ReaderParameters { ReadSymbols = true };
            assembly = AssemblyDefinition.ReadAssembly( fileName, readerParameters );
            reportManager.AddLine( "Reading file..." );
        }

        //example - name.exe -> name-obfuscated.exe
        public string GetDefaultObfuscatedFileName()
        {
            string newFileName = "";

            string temp = assembly.MainModule.Name;
            newFileName = temp.Substring( 0, temp.Length - 4 );
            newFileName += "-obfuscated";
            return newFileName;
        }

        public void RunLayoutTransformations(List<Transformations> ObfuscationsToDo)
        {
            layoutTransformations = new LayoutTransformations( reportManager, ObfuscationsToDo );
            layoutTransformations.RunTransformations( assembly );
        }

        public void RunDataTransformations( List<Transformations> ObfuscationsToDo )
        {
            dataTransformations = new DataTransformations( reportManager, ObfuscationsToDo );
            dataTransformations.RunTransformations( assembly );
        }

        public void RunControlTransformation( List<Transformations> ObfuscationsToDo )
        {

        }

        public void RunProfiles(List<Transformations> ObfuscationsToDo)
        {
            RunDataTransformations( ObfuscationsToDo );
            RunLayoutTransformations( ObfuscationsToDo );           
        }

        public void LaunchTransformations(bool IsProfile, List<Transformations> ObfuscationsToDo)
        {
            if(IsProfile == true)
            {
                RunProfiles( ObfuscationsToDo );
            }
            else
            {
                RunDataTransformations( ObfuscationsToDo );
                RunLayoutTransformations( ObfuscationsToDo );
            }
        }

        public void StartObfuscation(TextBox savePath, TextBox fileName,
            ObfuscatorManager obfuscatorManager)
        {
            LaunchTransformations(obfuscatorManager.IsProfile, obfuscatorManager.ObfuscationsToDo);
            reportManager.AddLine( "Saving file..." );
            assembly.Write( Path.Combine( savePath.Text, fileName.Text + ".exe" ) );
            reportManager.AddLine( "Obfuscation finished!" );
            File.Delete( "FilePath.txt" );
        }
    }
}
