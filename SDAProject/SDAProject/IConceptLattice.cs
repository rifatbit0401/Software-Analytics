using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDAProject
{
    public interface IConceptLattice
    {
        List<Concept> ConstructConceptLattice(String fcaFilePath, String latticeFilePath);
    }
}
