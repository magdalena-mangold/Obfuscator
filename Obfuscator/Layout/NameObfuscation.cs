using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Obfuscator.Managing;

namespace Obfuscator.Layout
{
    class NameObfuscation
    {
        public ReportManager reportManager;
        private NameBuilder nameBuilder;
        private RandomStringBuilder randomStringBuilder;

        public NameObfuscation( ReportManager reportManager )
        {
            this.reportManager = reportManager;
        }

        public void ChangeNamesToRandomString(AssemblyDefinition assemblyDefinition)
        {
            randomStringBuilder = new RandomStringBuilder();
            reportManager.AddLine( "Start - Changing Names (random strings)..." );
            //assemblyDefinition.Name = assemblyDefinition.Name.Name.Replace(assemblyDefinition.Name, nameBuilder.GenerateRandomString);
            var asName = assemblyDefinition.Name;
            //MessageBox.Show(asName.Name);
            assemblyDefinition.Name.Name.Replace( asName.Name, randomStringBuilder.GenerateRandomString() );

            foreach( var moduleDefinition in assemblyDefinition.Modules )
            {

                moduleDefinition.Name = moduleDefinition.Name.Replace( moduleDefinition.Name, randomStringBuilder.GenerateRandomString() );
                //moduleDefinition.ReadSymbols();
                foreach( var typeDefinition in moduleDefinition.Types )
                {
                    foreach(var methodDefinition in typeDefinition.Methods)
                    {
                        if(!methodDefinition.IsConstructor)
                        {
                            string oldName = methodDefinition.Name;
                            string newName = randomStringBuilder.GenerateRandomString();
                            methodDefinition.Name = newName;
                        }

                        foreach(var variableDefinition in methodDefinition.Body.Variables)
                        {
                            string oldName = variableDefinition.Name;
                            string newName = randomStringBuilder.GenerateRandomString();
                            variableDefinition.Name = newName;
                        }
                    }

                    foreach(var fieldDefinition in typeDefinition.Fields)
                    {
                        string oldName = fieldDefinition.Name;
                        string newName = randomStringBuilder.GenerateRandomString();
                        fieldDefinition.Name = newName;
                    }

                    foreach(var propertyDefinition in typeDefinition.Properties)
                    {
                        string oldName = propertyDefinition.Name;
                        string newName = randomStringBuilder.GenerateRandomString();
                        propertyDefinition.Name = newName;
                    }

                    string oldClassName = typeDefinition.Name;
                    string newClassName = randomStringBuilder.GenerateRandomString();
                    typeDefinition.Name = newClassName;
                }
            }
            reportManager.AddLine( "Changing Names (random strings) done." );
        }

        public void ChangeNamesToRandomWords(AssemblyDefinition assemblyDefinition)
        {
            nameBuilder = new NameBuilder("Words.txt");
            reportManager.AddLine( "Start - Changing Names..." );
            foreach( var moduleDefinition in assemblyDefinition.Modules )
            {
                //reportManager.AddLine( assemblyDefinition.MainModule.Name);
                //moduleDefinition.Name = moduleDefinition.Name.Replace( moduleDefinition.Name, nameBuilder.GenerateRandomString() );
                foreach( var typeDefinition in moduleDefinition.Types )
                {
                    foreach( var methodDefinition in typeDefinition.Methods )
                    {
                        if( !methodDefinition.IsConstructor )
                        {
                            string oldName = methodDefinition.Name;
                            string newName = nameBuilder.GetNewName();
                            methodDefinition.Name = newName;
                        }
                        foreach( var variableDefinition in methodDefinition.Body.Variables )
                        {
                            //moduleDefinition.ReadSymbols();
                            string oldVar = variableDefinition.Name;
                            string newVar = nameBuilder.GetNewName();
                            variableDefinition.Name = newVar;
                        }
                    }

                    foreach( var fieldDefinition in typeDefinition.Fields )
                    {
                        string oldName = fieldDefinition.Name;
                        string newName = nameBuilder.GetNewName();
                        fieldDefinition.Name = newName;
                    }

                    foreach( var propertyDefinition in typeDefinition.Properties )
                    {
                        string oldProp = propertyDefinition.Name;
                        string newProp = nameBuilder.GetNewName();
                        propertyDefinition.Name = newProp;
                    }
                    string oldTypeName = typeDefinition.Name;
                    string newTypeName = nameBuilder.GetNewName();
                    typeDefinition.Name = newTypeName;
                }
            }
            reportManager.AddLine( "Changing Names done." );
        }
    }
}
