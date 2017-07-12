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

        public StringGetter()
        {

        }

    }
}
