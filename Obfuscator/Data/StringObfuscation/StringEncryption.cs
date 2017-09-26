using Mono.Cecil;
using Mono.Cecil.Cil;
using Obfuscator.Managing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Obfuscator.Data.StringObfuscation
{
    class StringEncryption
    {
        private ReportManager reportManager;
        private bool isDone;
        public bool IsDone
        {
            get { return isDone; }
            set { isDone = value; }
        }

        private ParameterDefinition param1;
        public ParameterDefinition Param1
        {
            get { return param1; }
            set { param1 = value; }
        }
        private ParameterDefinition param2;
        public ParameterDefinition Param2
        {
            get { return param2; }
            set { param2 = value; }
        }
        private List<string> strs;
        public List<string> Strs
        {
            get { return strs; }
            set { strs = value; }
        }
        private List<string> inlineStrs;
        public List<string> InlineStrs
        {
            get { return inlineStrs; }
            set { inlineStrs = value; }
        }

        private Random rand;

        public StringEncryption( ReportManager reportManager )
        {
            IsDone = false;
            this.reportManager = reportManager;
            strs = new List<string>();
            inlineStrs = new List<string>();
            rand = new Random();
        }

        public void EncryptStrings( AssemblyDefinition assembly )
        {
            reportManager.AddLine( "Start - String encryption..." );
            FindStrs( assembly );
            InjectEncrypion( assembly );
            reportManager.AddLine( "Encryption finished." );
        }


        public void FindStrs( AssemblyDefinition assembly )
        {
            foreach( var type in assembly.MainModule.Types )
            {
                foreach( var met in type.Methods )
                {
                    foreach( var i in met.Body.Instructions )
                    {
                        if( i.Operand != null && i.OpCode == OpCodes.Ldstr && (string)i.Operand != "" )
                        {
                            Strs.Add( i.Operand.ToString() );
                        }
                    }
                }
            }
        }

        public void InjectEncrypion( AssemblyDefinition assembly )
        {
            int j = 0;
            int nameID = 0;
            do
            {
                foreach( string item in Strs )
                {
                    IsDone = false;
                    foreach( var type in assembly.MainModule.Types )
                    {
                        //MethodDefinition newMet = CreateDecryptMethod(assembly, type);

                        foreach( var met in type.Methods )
                        {
                            foreach( var i in met.Body.Instructions )
                            {
                                if( i.OpCode == OpCodes.Ldstr )
                                {
                                    if( i.Operand.ToString() == item )
                                    {
                                        //int rand = new Random( (int)DateTime.Now.Ticks ).Next( 255 );
                                        int randomNumber = rand.Next( 255 );
                                        MethodDefinition newMet = CreateDecryptMethod( assembly, type, nameID );
                                        reportManager.AddLine( "Decryption method injected." );
                                        var worker = met.Body.GetILProcessor();
                                        i.Operand =
                                        i.Operand.ToString().Select( c => ((char)(c + randomNumber)).ToString() ).Aggregate(
                                            ( c1, c2 ) => c1 + c2 );
                                        worker.InsertAfter( i, worker.Create( OpCodes.Ldc_I4, randomNumber ) );
                                        var inst = worker.Create( OpCodes.Call, newMet.Resolve() );
                                        worker.InsertAfter( i.Next, inst );

                                        IsDone = true;
                                        j++; nameID++;
                                        break;
                                    }
                                }
                            }
                            if( IsDone ) { break; }
                        }
                        if( IsDone ) { break; }
                    }
                    if( IsDone ) { break; }
                }
            } while( j < Strs.Count );
        }

        public MethodDefinition CreateDecryptMethod( AssemblyDefinition assembly, TypeDefinition type, int nameID )
        {
            MethodDefinition newMet = new MethodDefinition( DecryptMetNames( nameID ), MethodAttributes.Public | MethodAttributes.Static, assembly.MainModule.Import( typeof( string ) ) );
            type.Methods.Add( newMet );

            var stringType = assembly.MainModule.Import( typeof( string ) );
            var intType = assembly.MainModule.Import( typeof( int ) );
            var charType = assembly.MainModule.Import( typeof( char ) );
            var boolType = assembly.MainModule.Import( typeof( bool ) );
            var emptyStringType = assembly.MainModule.Import( typeof( string ) ).Resolve().Fields.Where( f => f.Name == "Empty" ).First();
            var getCharsType = assembly.MainModule.Import( typeof( string ) ).Resolve().Properties.Where( m => m.Name == "Chars" ).First().GetMethod;
            var concatType = assembly.MainModule.Import( typeof( string ) ).Resolve().Methods.Where( m => m.Name == "Concat" && m.Parameters.Count == 2 ).First();
            var lengthType = assembly.MainModule.Import( typeof( string ) ).Resolve().Properties.Where( p => p.Name == "Length" ).First().GetMethod;

            Param1 = new ParameterDefinition( "inString", ParameterAttributes.In, stringType );
            Param2 = new ParameterDefinition( "inInt", ParameterAttributes.In, intType );
            newMet.Parameters.Add( param1 ); newMet.Parameters.Add( param2 );

            VariableDefinition result = new VariableDefinition( "result", stringType );
            VariableDefinition tempChar = new VariableDefinition( "letter", charType );
            VariableDefinition tempString = new VariableDefinition( "temp", stringType );
            //VariableDefinition tempString2 = new VariableDefinition("temp2", stringType);
            VariableDefinition count = new VariableDefinition( "count", intType );
            VariableDefinition helpBool = new VariableDefinition( "helpBool", boolType );

            newMet.Body.Variables.Add( result );
            newMet.Body.Variables.Add( tempChar );
            newMet.Body.Variables.Add( tempString );
            //newMet.Body.Variables.Add(tempString2);
            newMet.Body.Variables.Add( count );
            newMet.Body.Variables.Add( helpBool );

            newMet.Body.InitLocals = true;

            var emptyString = assembly.MainModule.Import( emptyStringType );
            var getChars = assembly.MainModule.Import( getCharsType );
            var concat = assembly.MainModule.Import( concatType );
            var length = assembly.MainModule.Import( lengthType );

            Instruction jump = Instruction.Create( OpCodes.Stloc, count );
            Instruction loopStart = Instruction.Create( OpCodes.Ldloc, tempString );
            Instruction check = Instruction.Create( OpCodes.Ldloc, count );
            Instruction loadTemp = Instruction.Create( OpCodes.Ldloc, tempString );

            newMet.Body.Instructions.Add( Instruction.Create( OpCodes.Ldsfld, emptyString ) );
            newMet.Body.Instructions.Add( Instruction.Create( OpCodes.Stloc, result ) );
            newMet.Body.Instructions.Add( Instruction.Create( OpCodes.Ldarg, param1 ) );
            newMet.Body.Instructions.Add( Instruction.Create( OpCodes.Stloc, tempString ) );
            newMet.Body.Instructions.Add( Instruction.Create( OpCodes.Ldc_I4_0 ) );

            newMet.Body.Instructions.Add( jump );
            newMet.Body.Instructions.Add( Instruction.Create( OpCodes.Br_S, check ) );
            newMet.Body.Instructions.Add( loopStart );
            newMet.Body.Instructions.Add( Instruction.Create( OpCodes.Ldloc, count ) );

            newMet.Body.Instructions.Add( Instruction.Create( OpCodes.Callvirt, getChars ) );
            newMet.Body.Instructions.Add( Instruction.Create( OpCodes.Stloc, tempChar ) );

            newMet.Body.Instructions.Add( Instruction.Create( OpCodes.Ldloc, result ) );
            newMet.Body.Instructions.Add( Instruction.Create( OpCodes.Ldloc, tempChar ) );
            newMet.Body.Instructions.Add( Instruction.Create( OpCodes.Ldarg, param2 ) );
            newMet.Body.Instructions.Add( Instruction.Create( OpCodes.Sub ) );
            newMet.Body.Instructions.Add( Instruction.Create( OpCodes.Conv_U2 ) );
            newMet.Body.Instructions.Add( Instruction.Create( OpCodes.Box, charType ) );
            newMet.Body.Instructions.Add( Instruction.Create( OpCodes.Call, concat ) );
            newMet.Body.Instructions.Add( Instruction.Create( OpCodes.Stloc, result ) );

            newMet.Body.Instructions.Add( Instruction.Create( OpCodes.Ldloc, count ) );
            newMet.Body.Instructions.Add( Instruction.Create( OpCodes.Ldc_I4_1 ) );
            newMet.Body.Instructions.Add( Instruction.Create( OpCodes.Add ) );
            newMet.Body.Instructions.Add( Instruction.Create( OpCodes.Stloc, count ) );
            newMet.Body.Instructions.Add( check );
            newMet.Body.Instructions.Add( Instruction.Create( OpCodes.Ldloc, tempString ) );
            newMet.Body.Instructions.Add( Instruction.Create( OpCodes.Callvirt, length ) );
            newMet.Body.Instructions.Add( Instruction.Create( OpCodes.Clt ) );
            newMet.Body.Instructions.Add( Instruction.Create( OpCodes.Stloc, helpBool ) );
            newMet.Body.Instructions.Add( Instruction.Create( OpCodes.Ldloc, helpBool ) );
            newMet.Body.Instructions.Add( Instruction.Create( OpCodes.Brtrue_S, loopStart ) );

            newMet.Body.Instructions.Add( Instruction.Create( OpCodes.Ldloc, result ) );
            newMet.Body.Instructions.Add( Instruction.Create( OpCodes.Stloc, tempString ) );
            newMet.Body.Instructions.Add( Instruction.Create( OpCodes.Br_S, loadTemp ) );
            newMet.Body.Instructions.Add( loadTemp );
            newMet.Body.Instructions.Add( Instruction.Create( OpCodes.Ret ) );


            reportManager.AddLine( "Dencryption method created." );
            return newMet;
        }

        public string DecryptMetNames( int nameID )
        {
            string name = "mystery" + nameID.ToString();
            return name;
        }
    }
}
