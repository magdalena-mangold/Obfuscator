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
    class StringObfuscationClassInjection
    {
        private ReportManager reportManager;
        private StringGetter stringGetter;
        private StringObfuscationInstructionCreator stringObfuscationInstructionCreator;

        public StringObfuscationClassInjection(ReportManager reportManager)
        {
            this.reportManager = reportManager;
            stringGetter = new StringGetter( reportManager );
            stringObfuscationInstructionCreator = new StringObfuscationInstructionCreator();
        }

        public void MethodInjection( AssemblyDefinition assembly )
        {
            MethodDefinition met;
            string methodName = "";
            MethodReference testmethod_ref = null;
            ILProcessor worker = null;
            foreach( var type in assembly.MainModule.Types )
            {
                if( type.Name == "StringCuts" )
                {
                    met = stringObfuscationInstructionCreator.CreateStringCutMethod( assembly, stringGetter.StList );
                    //reportManager.AddLine( "Stworzono metodę" );
                    type.Methods.Add( met );
                    methodName = met.Name;
                }

                foreach( var m in type.Methods )
                {
                    if( m.Name == methodName )
                    {
                        worker = m.Body.GetILProcessor();
                        testmethod_ref = assembly.MainModule.Import( m ); break;
                    }
                }


            }
            MethodReference writeline = assembly.MainModule.Import( typeof( Console ).GetMethod( "WriteLine", new Type[] { typeof( string ) } ) );
            assembly.MainModule.Import( writeline );

            ReplaceInstructions( assembly, testmethod_ref, writeline, worker );
            reportManager.AddLine( "Method injected." );
        }

        public void ReplaceInstructions( AssemblyDefinition assembly, MethodReference metRef,
    MethodReference writeline, ILProcessor worker )
        {
            //List<string> strs = new List<string>();
            int i = 0;
            foreach( var type in assembly.MainModule.Types )
            {
                foreach( var met in type.Methods )
                {
                    foreach( List<Instruction> list in stringGetter.InstructionsToRemove )
                    {
                        foreach( Instruction item in list )
                        {
                            if( met.Body.Instructions.Contains( item ) )
                            {
                                if( met.Body.Instructions.Contains( list[1] ) )
                                {
                                    int index = met.Body.Instructions.IndexOf( item );
                                    met.Body.Instructions.Insert( index, worker.Create( OpCodes.Ldc_I4, i ) );
                                    met.Body.Instructions.Insert( index + 1, worker.Create( OpCodes.Call, metRef ) );
                                    met.Body.Instructions.Insert( index + 2, worker.Create( OpCodes.Call, writeline ) );
                                    met.Body.Instructions.Remove( item );
                                    met.Body.Instructions.Remove( list[1] );
                                    i++;
                                    break;
                                }
                            }
                        }

                    }

                }
            }

        }

        public void StringCuts( AssemblyDefinition assembly )
        {
            reportManager.AddLine( "Start - Obfuscating strings..." );
            stringGetter.PopulateStringList( assembly );
            if( stringGetter.StList != null )
            {
                TypeInjection( assembly );
                MethodInjection( assembly );
                reportManager.AddLine( "String obfuscation done." );
            }
            else
            {
                reportManager.AddLine( "No strings found." );
            }
        }

        public void TypeInjection( AssemblyDefinition assembly )
        {
            string name = "";
            foreach( var module in assembly.Modules )
            {
                foreach( var type in module.Types )
                {
                    name = type.Namespace;
                }
            }

            TypeDefinition newType = new TypeDefinition( name, "StringCuts", TypeAttributes.Class | TypeAttributes.Public );
            TypeReference tr = assembly.MainModule.Import( typeof( System.Object ) );
            newType.BaseType = tr;
            assembly.MainModule.Types.Add( newType );
            reportManager.AddLine( "StringCut class injected." );
        }
    }
}
