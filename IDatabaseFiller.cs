using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Online_BoekenApp
{
    interface IDatabaseFiller
    {
        bool fillUitgevers(string padNaarUitgeversFile);
        bool fillCategorieen(string padNaarCategorieenFile);
        bool fillBoeken(string padNaarBoekenFile);
        bool fillStatusen(string padNaarStatusenFile);
    }
}
