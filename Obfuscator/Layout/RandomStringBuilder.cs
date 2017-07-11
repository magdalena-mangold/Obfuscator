using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Obfuscator.Layout
{
    //class used in NameObfuscation that will create and return random string
    class RandomStringBuilder
    {
        private string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private Random randomString;

        public RandomStringBuilder()
        {
            randomString = new Random();
        }

        public string GenerateRandomString()
        {
            return new string( Enumerable.Repeat( letters, 10 )
           .Select( s => s[randomString.Next( s.Length )] ).ToArray() );
        }

    }
}
