using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Obfuscator.Data.StringObfuscation
{
    class StringObfuscationInstructionCreator
    {
        public StringObfuscationInstructionCreator()
        {

        }

        public Instruction[] CreateSwitches( ILProcessor worker, List<List<string>> StList )
        {
            Instruction[] switches = new Instruction[StList.Count];
            int i = 0;
            foreach( List<string> list in StList )
            {
                foreach( string item in list )
                {
                    if( item == list[0] )
                    {
                        string temp = item;
                        temp += " ";
                        switches[i] = worker.Create( OpCodes.Ldstr, temp );
                    }
                }
                i++;
            }
            return switches;
        }

        public List<List<Instruction>> CreateSwitchCaseInstructions( ILProcessor worker,
            Instruction[] switches, VariableDefinition helpInt, VariableDefinition returnString )
        {
            Instruction[] switchTab = switches;
            List<List<Instruction>> instrList = new List<List<Instruction>>();
            int j = 0;
            //start of the switch with ldarg in other function
            foreach( Instruction item in switches )
            {
                List<Instruction> temp = new List<Instruction>();
                temp.Add( worker.Create( OpCodes.Stloc, returnString ) );
                temp.Add( worker.Create( OpCodes.Ldc_I4, j ) );
                temp.Add( worker.Create( OpCodes.Stloc, helpInt ) );
                instrList.Add( temp );
                j++;
            }
            return instrList;
        }

        //string concatanation
        public List<string> JoinStrings( List<List<string>> StList )
        {
            List<string> joinedStrings = new List<string>();

            foreach( List<string> list in StList )
            {
                List<string> bufor = list;
                bufor.Remove( bufor[0] );
                string temp = string.Join( " ", bufor );
                joinedStrings.Add( temp );
            }
            return joinedStrings;
        }

        //Begining of instructions if
        public Instruction[] CreateIfStarts( int StListCount, ILProcessor worker )
        {
            Instruction[] ifStarts = new Instruction[StListCount];
            for( int i = 0; i < StListCount; i++ )
            {
                ifStarts[i] = worker.Create( OpCodes.Ldc_I4, i );
            }
            return ifStarts;
        }

        public List<List<Instruction>> CreateIfInstructions( ILProcessor worker,
    VariableDefinition helpInt, VariableDefinition returnString, Instruction ifEnd,
    List<string> joinedStrings, MethodReference concat, Instruction[] ifStarts )
        {
            List<List<Instruction>> instrList = new List<List<Instruction>>();

            int j = 1; int k = 0;
            foreach( Instruction item in ifStarts )
            {
                List<Instruction> temp = new List<Instruction>();
                temp.Add( worker.Create( OpCodes.Ldloc, helpInt ) );
                if( j == (ifStarts.Length) )
                {
                    temp.Add( worker.Create( OpCodes.Bne_Un, ifEnd ) );
                }
                else
                {
                    temp.Add( worker.Create( OpCodes.Bne_Un, ifStarts[j] ) );
                }

                temp.Add( worker.Create( OpCodes.Ldloc, returnString ) );
                temp.Add( worker.Create( OpCodes.Ldstr, joinedStrings[k] ) );
                temp.Add( worker.Create( OpCodes.Call, concat ) );
                temp.Add( worker.Create( OpCodes.Stloc, returnString ) );

                j++; k++;

                instrList.Add( temp );
            }

            return instrList;
        }

        public MethodDefinition CreateStringCutMethod( AssemblyDefinition assembly, List<List<string>> StList )
        {
            List<Instruction> instructionList = new List<Instruction>();

            MethodReference concat = assembly.MainModule.Import( typeof( String ).GetMethod( "Concat", new Type[] { typeof( string ), typeof( string ) } ) );
            MethodDefinition newMet = new MethodDefinition( "Test",
                        MethodAttributes.Public | MethodAttributes.Static,
                        assembly.MainModule.TypeSystem.String );
            //parameter choice
            TypeReference paramType = assembly.MainModule.Import( typeof( Int32 ) );
            //returned string
            TypeReference varStringType = assembly.MainModule.Import( typeof( string ) );
            //int i
            TypeReference varIntType = assembly.MainModule.Import( typeof( Int32 ) );

            VariableDefinition returnString = new VariableDefinition( "st", varStringType );
            newMet.Body.Variables.Add( returnString );
            VariableDefinition helpInt = new VariableDefinition( "i", varIntType );
            newMet.Body.Variables.Add( helpInt );

            var choice = new ParameterDefinition( "choice", ParameterAttributes.None, paramType );
            newMet.Parameters.Add( choice );

            newMet.Body.InitLocals = true;

            ILProcessor worker = newMet.Body.GetILProcessor();
            Instruction[] switches = CreateSwitches( worker, StList );
            Instruction[] ifStarts = CreateIfStarts( StList.Count, worker );
            List<List<Instruction>> switchCaseInstr = CreateSwitchCaseInstructions( worker, switches, helpInt, returnString );


            Instruction ifEnd = worker.Create( OpCodes.Ldloc, returnString );

            instructionList.Add( worker.Create( OpCodes.Ldstr, "" ) );
            instructionList.Add( worker.Create( OpCodes.Stloc, returnString ) );
            instructionList.Add( worker.Create( OpCodes.Ldc_I4, 0 ) );
            instructionList.Add( worker.Create( OpCodes.Stloc, helpInt ) );

            instructionList.Add( worker.Create( OpCodes.Ldarg, choice ) );
            instructionList.Add( worker.Create( OpCodes.Switch, switches ) );
            //SWITCH
            int i = 0;
            foreach( List<Instruction> list in switchCaseInstr )
            {
                instructionList.Add( switches[i] );
                foreach( Instruction item in list )
                {
                    instructionList.Add( item );
                }
                instructionList.Add( worker.Create( OpCodes.Br, ifStarts[0] ) );
                i++;
            }
            //END SWITCH

            List<string> joinedStrings = JoinStrings( StList );
            List<List<Instruction>> ifInstructions = CreateIfInstructions( worker, helpInt, returnString, ifEnd, joinedStrings, concat, ifStarts );

            int j = 0;
            //IF
            foreach( List<Instruction> list in ifInstructions )
            {
                instructionList.Add( ifStarts[j] );
                foreach( Instruction item in list )
                {
                    instructionList.Add( item );
                }
                j++;
            }

            //END IF
            instructionList.Add( ifEnd );
            instructionList.Add( worker.Create( OpCodes.Ret ) );

            foreach( Instruction instr in instructionList )
            {
                newMet.Body.Instructions.Add( instr );
            }

            return newMet;
        }
    }
}
