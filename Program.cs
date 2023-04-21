using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace _8reinas
{
    class Program {

        static void Main(string[] args)
        {
            //Poblacion P = new Poblacion(30, .4, 10000, 1, 1);
            for (int i=0;i<30;i++) {
                Poblacion P = new Poblacion(30, .4, 10000, 1, 1);
                P.IniciarAE();
                Thread.Sleep(10000);
            }

        }
    }
}