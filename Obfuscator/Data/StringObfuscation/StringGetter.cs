using Mono.Cecil;
using Mono.Cecil.Cil;
using Obfuscator.Managing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Obfuscator.Data.StringObfuscation
{
    //Gets all strings in the program
    class StringGetter
    {
        private ReportManager reportManager;

        private List<string> strs;
        public List<string> Strs
        {
            get { return strs; }
            set { strs = value; }
        }
        private List<List<string>> stList;
        public List<List<string>> StList
        {
            get { return stList; }
            set { stList = value; }
        }
        private List<List<Instruction>> instructionsToRemove;
        public List<List<Instruction>> InstructionsToRemove
        {
            get { return instructionsToRemove; }
            set { instructionsToRemove = value; }
        }

        public StringGetter(ReportManager reportManager)
        {
            this.reportManager = reportManager;
            strs = new List<string>();
            stList = new List<List<string>>();
            instructionsToRemove = new List<List<Instruction>>();
        }

        public List<string> FindString( AssemblyDefinition assembly )
        {
            //List<string> strs = new List<string>();
            foreach( var type in assembly.MainModule.Types )
            {
                foreach( var met in type.Methods )
                {
                    foreach( Instruction ist in met.Body.Instructions )
                    {
                        if( ist.OpCode == OpCodes.Ldstr )
                        {
                            //get offset
                            //if( ist.Next.OpCode == OpCodes.Call )
                            //{
                                //MethodReference mc = ist.Next.Operand as MethodReference;
                                //if( mc.Name == "WriteLine" )
                                //{
                                    Strs.Add( ist.Operand.ToString() );
                                    reportManager.AddLine( "Found String." );
                                    List<Instruction> temp = new List<Instruction>();
                                    temp.Add( ist );
                                    temp.Add( ist.Next );
                                    InstructionsToRemove.Add( temp );
                                //}

                            //}
                        }
                    }
                }
            }
            return Strs;
        }

        public List<List<string>> GetLists( List<string> Strs )
        {
            //List<List<string>> stList = new List<List<string>>();
            char[] signs = { ' ', '\n', '\t' };

            foreach( string item in Strs )
            {
                List<string> temp = new List<string>();
                string[] words = item.Split( signs );
                foreach( string w in words )
                {
                    temp.Add( w );
                }
                StList.Add( temp );
            }
            return StList;
        }

        public void PopulateStringList( AssemblyDefinition assembly )
        {
            Strs = FindString( assembly );
            StList = GetLists( Strs );
        }
    }
}
