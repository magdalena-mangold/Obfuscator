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
    class DataTransformations : ITransformations
    {
        public ReportManager reportManager;
        private StringObfuscationClassInjection stringObfClInj;
        private StringEncryption stringEncryption;

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

        public DataTransformations(ReportManager reportManager, List<Transformations> ObfuscationsToDo)
        {
            Obfuscations = new List<Transformations>();
            OwnObfuscationsToDo = new List<Transformations>();
            SetObfuscationsList();
            this.reportManager = reportManager;
            ScanToDoList( ObfuscationsToDo );
        }

        public void ChooseTransformations( Transformations transformations, AssemblyDefinition assembly )
        {
            switch(transformations)
            {
                case Transformations.ProfileEasy:
                    //stringObfClInj = new StringObfuscationClassInjection(reportManager);
                    //stringObfClInj.StringCuts( assembly );
                    stringEncryption = new StringEncryption( reportManager );
                    stringEncryption.EncryptStrings( assembly );
                    break;
            }
        }

        public void RunTransformations( AssemblyDefinition assembly )
        {
            foreach( var item in OwnObfuscationsToDo )
            {
                ChooseTransformations( item, assembly );
            }
        }

        public void ScanToDoList( List<Transformations> ObfuscationsToDo )
        {
            foreach( var item in Obfuscations )
            {
                if( ObfuscationsToDo.Contains( item ) )
                {
                    OwnObfuscationsToDo.Add( item );
                }
            }
        }

        public void SetObfuscationsList()
        {
            Obfuscations.Add( Transformations.ProfileEasy );
            Obfuscations.Add( Transformations.StringSplit );
            Obfuscations.Add( Transformations.StringSimpleEncryption );
        }
    }
}
