using Mono.Cecil;
using Obfuscator.Managing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Obfuscator.Layout
{
    class LayoutTransformations : TransformationClass
    {
        private HideNames hideNames;
        private NameObfuscation nameObfuscation;

        public LayoutTransformations( ReportManager reportManager, List<Transformations> ObfuscationsToDo ) 
            : base( reportManager, ObfuscationsToDo )
        {
            Init( ObfuscationsToDo );
        }

        //Setting all possible obfuscations in the main list
        public override void SetObfuscationsList()
        {
            Obfuscations.Add( Transformations.ProfileEasy );
            Obfuscations.Add( Transformations.HideNames );
            Obfuscations.Add( Transformations.RandomStrings );
            Obfuscations.Add( Transformations.RandomWords );
        }

        public override void ChooseTransformations( Transformations transformations,
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
