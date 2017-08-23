using Mono.Cecil;
using Obfuscator.Managing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Obfuscator.Layout
{
    class HideNames
    {
        private ReportManager reportManager;

        public HideNames(ReportManager reportManager)
        {
            this.reportManager = reportManager;
        }

        public void RunHideNames(AssemblyDefinition assembly)
        {
            reportManager.AddLine( "Start - Hiding Names..." );

            char newModuleNamde = ' ';
            foreach(var module in assembly.Modules)
            {
                module.Name = newModuleNamde.ToString();
                char newTypeName = '\0';
                foreach(var type in module.Types)
                {
                    type.Name = newTypeName.ToString();
                    char newMethodName = '\0';
                    foreach(var method in type.Methods)
                    {
                        if( method.IsConstructor ) continue;
                        method.Name = newMethodName.ToString();
                        newMethodName++;
                    }
                    char newFieldName = '\0';
                    foreach(var field in type.Fields)
                    {
                        field.Name = newFieldName.ToString();
                        newFieldName++;
                    }
                    newTypeName++;
                }
                newModuleNamde++;
            }
            reportManager.AddLine( "Hiding Names done." );
        }

    }
}
