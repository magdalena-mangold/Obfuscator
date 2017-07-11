using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Obfuscator.Layout
{
    //words source: https://github.com/dwyl/english-words
    class NameBuilder
    {
        private Random randomWord;
        List<string> names;
        List<string> chosenWords;

        public NameBuilder(string fileName)
        {
            randomWord = new Random();
            names = new List<string>();
            names = ReadWords( fileName );
            chosenWords = new List<string>();
        }

        public List<string> ReadWords( string fileName )
        {
            List<string> names = new List<string>();
            using( StreamReader str = new StreamReader( fileName ) )
            {
                string line;
                while( (line = str.ReadLine()) != null )
                {
                    names.Add( line );
                }
            }
            return names;
        }
        //needs checking
        public bool CheckIfWordWasUsed(string newName)
        {
            if(chosenWords.Count > 1)
            {
                foreach( string name in chosenWords )
                {
                    if( newName == name )
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public string GetNewName()
        {
            string newName = "";
            newName = names[randomWord.Next( names.Count - 1 )];            
            if(CheckIfWordWasUsed(newName))
            {
                GetNewName();
            }
            chosenWords.Add( newName );
            return newName;
        }
    }
}
