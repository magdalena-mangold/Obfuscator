using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Obfuscator.Managing;

namespace Obfuscator
{

    abstract class TransformationClass
    {
        public ReportManager reportManager;

        List<Transformations> obfuscations;
        public List<Transformations> Obfuscations
        {
            get { return obfuscations; }
            set { value = obfuscations; }
        }

        List<Transformations> ownObfuscationsToDo;
        public List<Transformations> OwnObfuscationsToDo
        {
            get { return ownObfuscationsToDo; }
            set { value = ownObfuscationsToDo; }
        }

        public TransformationClass( ReportManager reportManager, List<Transformations> ObfuscationsToDo )
        {
            obfuscations = new List<Transformations>();
            ownObfuscationsToDo = new List<Transformations>();
            //SetObfuscationsList();
            this.reportManager = reportManager;
            //ScanToDoList( ObfuscationsToDo );
        }

        public virtual void SetObfuscationsList() { }

        public void Init( List<Transformations> ObfuscationsToDo )
        {
            SetObfuscationsList();
            ScanToDoList( ObfuscationsToDo );
        }

        public void ScanToDoList(List<Transformations> ObfuscationsToDo)
        {
            foreach( var item in Obfuscations )
            {
                if( ObfuscationsToDo.Contains( item ) )
                {
                    OwnObfuscationsToDo.Add( item );
                }
            }
        }
        public virtual void ChooseTransformations(Transformations transformations, AssemblyDefinition assembly ) { }

        public void RunTransformations(AssemblyDefinition assembly)
        {
            try
            {
                foreach( var item in OwnObfuscationsToDo )
                {
                    ChooseTransformations( item, assembly );
                }
            }
            catch( NullReferenceException )
            {

                reportManager.AddLine( "No obfuscations to do. Proceed to next step." );
            }
        }
    }
}
