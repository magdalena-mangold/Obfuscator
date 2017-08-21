using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Obfuscator.Managing
{
    class ObfuscatorManager
    {
        private bool isProfile;
        public bool IsProfile
        {
            get { return isProfile; }
            set { isProfile = value; }
        }
        private List<Transformations> obfuscationsToDo;
        public List<Transformations> ObfuscationsToDo
        {
            get { return obfuscationsToDo; }
            set { obfuscationsToDo = value; }
        }

        public ObfuscatorManager()
        {
            IsProfile = false;
            obfuscationsToDo = new List<Transformations>();
        }

        public void CheckProfiles(ListBox profileList)
        {
            var profiles = profileList.Items.OfType<RadioButton>();
            foreach(RadioButton radio in profiles)
            {
                if(radio.IsChecked == true)
                {
                    switch(radio.Name)
                    {
                        case "profileRadioEasy":
                            ObfuscationsToDo.Add( Transformations.ProfileEasy );
                            //ObfuscationsToDo.Add( Transformations.StringSplit );
                            //ObfuscationsToDo.Add( Transformations.StringSimpleEncryption );
                            break;
                        case "profileRadioMedium":

                            break;
                        case "profileRadioHard":

                            break;
                    }
                    IsProfile = true;
                }
            }
        }

        public void CheckLayoutOptions(ListBox layoutOptions)
        {
            var options = layoutOptions.Items.OfType<RadioButton>();
            foreach(RadioButton radio in options)
            {
                if(radio.IsChecked == true)
                {
                    switch(radio.Name)
                    {
                        case "hideNames":
                            ObfuscationsToDo.Add( Transformations.HideNames );
                            break;
                        case "randomStrings":
                            ObfuscationsToDo.Add( Transformations.RandomStrings );
                            break;
                        case "randomWords":
                            ObfuscationsToDo.Add( Transformations.RandomWords );
                            break;
                    }
                }
            }
        }

        public void CheckDataOptions(ListBox dataOptions)
        {

        }

        public void CheckControlOptions(ListBox controlOptions)
        {

        }

        
        public void SearchSettings( ListBox layoutOptions)
        {
            CheckLayoutOptions(layoutOptions);
            //CheckDataOptions(dataOptions);
            //CheckControlOptions(controlOptions);
        }
    }
}
