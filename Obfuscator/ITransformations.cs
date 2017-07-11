using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;

namespace Obfuscator
{

    interface ITransformations
    {
        List<Transformations> Obfuscations { get; set; }
        List<Transformations> OwnObfuscationsToDo { get; set; }

        void SetObfuscationsList();
        void ScanToDoList(List<Transformations> ObfuscationsToDo);
        void ChooseTransformations(Transformations transformations, AssemblyDefinition assembly);
        void RunTransformations(AssemblyDefinition assembly);
    }
}
