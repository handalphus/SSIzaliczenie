using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace SSIzaliczenie
{
    class Program
    {
        
        static void Main(string[] args)
        {
            string addrDuzy = @"E:\Studia_2019-20\Semestr_letni\Informatyka\Systemy-sztucznej-inteligencji\SSI_t\7.png";
            string addrMaly = @"E:\Studia_2019-20\Semestr_letni\Informatyka\Systemy-sztucznej-inteligencji\SSI_t\7doZnalezienia.png";
            string addrZapis = @"E:\Studia_2019-20\Semestr_letni\Informatyka\Systemy-sztucznej-inteligencji\SSI_t\7znalezione.png";

            SoftSetClassifier a = new SoftSetClassifier(addrDuzy, addrMaly);
            a.FindSimilarPatterns(addrZapis);
            //string path = @"E:\Studia_2019-20\Semestr_letni\Informatyka\Systemy-sztucznej-inteligencji\SSI_t\";
            //SoftSetClassifier[] klasyfikatory = new SoftSetClassifier[15];
            //for (int i = 1; i <= 15; i++)
            //{
            //    addrDuzy = path + $@"{i}.png";
            //    addrMaly = path + $@"{i}doZnalezienia.png";
            //    addrZapis = path + $@"{i}znalezione.png";
            //    klasyfikatory[i - 1] = new SoftSetClassifier(addrDuzy, addrMaly);
            //    klasyfikatory[i - 1].FindSimilarPatterns(addrZapis);
            //}

        }

    }
}
