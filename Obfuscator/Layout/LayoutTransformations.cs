using Mono.Cecil;
using Obfuscator.Managing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Obfuscator.Layout
{
    class LayoutTransformations : ITransformations
    {
        public ReportManager reportManager;
        private HideNames hideNames;
        private NameObfuscation nameObfuscation;

        private List<Transformations> obfuscations;
        public List<Transformations> Obfuscations
        {
            get { return obfuscations; }
            set { obfuscations = value; }
        }

        private List<Transformations> ownObfuscationsToDo;
        public List<Transformations> OwnObfuscationsToDo
        {
            get { return ownObfuscationsToDo; }
            set { ownObfuscationsToDo = value; }
        }

        public LayoutTransformations(ReportManager reportManager, List<Transformations> ObfuscationsToDo)
        {
            Obfuscations = new List<Transformations>();
            OwnObfuscationsToDo = new List<Transformations>();
            SetObfuscationsList();
            this.reportManager = reportManager;
            ScanToDoList( ObfuscationsToDo );
        }

        public void RunTransformations(AssemblyDefinition assembly)
        {
            foreach(var item in OwnObfuscationsToDo)
            {
                ChooseTransformations( item, assembly );
            }
        }

        public void ScanToDoList( List<Transformations> ObfuscationsToDo )
        {
            foreach(var item in Obfuscations)
            {
                if(ObfuscationsToDo.Contains(item))
                {
                    OwnObfuscationsToDo.Add( item );
                }
            }
        }

        //Setting all possible obfuscations in the main list
        public void SetObfuscationsList()
        {
            Obfuscations.Add( Transformations.ProfileEasy );
            Obfuscations.Add( Transformations.HideNames );
            Obfuscations.Add( Transformations.RandomStrings );
            Obfuscations.Add( Transformations.RandomWords );
        }

        public void ChooseTransformations( Transformations transformations,
            AssemblyDefinition assembly )
        {
            switch(transformations)
            {
                case Transformations.ProfileEasy:
                    hideNames = new HideNames( reportManager );
                    hideNames.RunHideNames(assembly);
                    break;
                case Transformations.HideNames:
                    hideNames = new HideNames( reportManager );
                    hideNames.RunHideNames( assembly );
                    break;
                case Transformations.RandomStrings:
                    nameObfuscation = new NameObfuscation(reportManager);
                    nameObfuscation.ChangeNamesToRandomString( assembly );
                    break;
                case Transformations.RandomWords:
                    nameObfuscation = new NameObfuscation(reportManager);
                    nameObfuscation.ChangeNamesToRandomWords( assembly );
                    break;
            }
        }
    }
}
