using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Obfuscator.Layout
{
    //words source: https://github.com/dwyl/english-words
    class NameBuilder
    {
        private Random randomWord;
        List<string> names;
        List<string> chosenWords;

        public NameBuilder()
        {
            randomWord = new Random();
            names = new List<string>();
            names = ReadWords();
            chosenWords = new List<string>();
        }

        public List<string> ReadWords()
        {
            List<string> names = new List<string>();

            string resource_data = Properties.Resources.Words;
            names = resource_data.Split( new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries ).ToList();

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
