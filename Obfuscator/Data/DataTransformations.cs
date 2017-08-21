using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Obfuscator.Data.StringObfuscation;
using Obfuscator.Managing;

namespace Obfuscator.Data
{
    class DataTransformations : TransformationClass
    {
        private StringObfuscationClassInjection stringObfClInj;
        private StringEncryption stringEncryption;

        public DataTransformations( ReportManager reportManager, List<Transformations> ObfuscationsToDo )
            : base(reportManager, ObfuscationsToDo)
        {
            Init( ObfuscationsToDo );
        }

        public override void ChooseTransformations( Transformations transformations, AssemblyDefinition assembly )
        {
            switch(transformations)
            {
                case Transformations.ProfileEasy:
                    stringObfClInj = new StringObfuscationClassInjection( reportManager );
                    stringObfClInj.StringCuts( assembly );
                    //stringEncryption = new StringEncryption( reportManager );
                    //stringEncryption.EncryptStrings( assembly );
                    break;
            }
        }

        public override void SetObfuscationsList()
        {
            Obfuscations.Add( Transformations.ProfileEasy );
            Obfuscations.Add( Transformations.StringSplit );
            Obfuscations.Add( Transformations.StringSimpleEncryption );
        }
    }
}
