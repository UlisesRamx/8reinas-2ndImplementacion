using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace _8reinas
{
    internal class Poblacion
    {
        public int Tamano { get; set; } //50
        public double ProporcionPadres { get; set; } //40% = .4
        public int Generaciones { get; set; } //10000
        public double ProbabilidadMuta { get; set; } //100 = 1
        public double ProbabilidadCruza { get; set; } //100 =1
        public List<Solucion> Muestra { get; set; }
        public List<Solucion> Progenitores { get; set; }
        public List<Solucion> Crias { get; set; }

        public Poblacion(int Tamano, double ProporcionPadres,int Generaciones, double ProbabilidadCruza, double ProbabilidadMuta) {
            this.Tamano = Tamano;
            this.ProporcionPadres = ProporcionPadres;
            this.Generaciones = Generaciones;
            this.ProbabilidadCruza = ProbabilidadCruza;
            this.ProbabilidadMuta = ProbabilidadMuta;
            Muestra = new List<Solucion>();
            Progenitores = new List<Solucion>();
            Crias = new List<Solucion>();

            //Generar una Muestra aleatoria
            for (int i = 0; i < Tamano; i++)
            {
                Solucion s = new Solucion();
                s.IniciarSolucion();
                s.EvaluarSolucion();
                s.Identificador = 0 + "SNSN";
                Muestra.Add(s);
            }
            //Ordenar Muestra
            Muestra = Muestra.OrderByDescending(individuo => individuo.Aptitud).ToList<Solucion>();
        }

        public void IniciarAE() {
            Solucion mejorEspecimen = new Solucion();
            double probabildiadAleatoria;
            int generacionActual = 1;
            List<Solucion> MejorSeleccion = new List<Solucion>();
            List<Solucion> PeorSeleccion = new List<Solucion>();
            while (generacionActual <= Generaciones) {

                //Asignar Probabilidad de Seleccion a MUESTRA
                foreach (Solucion individuo in Muestra)
                {
                    individuo.ProbabilidadSeleccion = 1d / ((100 + 1) - Muestra.IndexOf(individuo));
                }

                //Seleccionar PROGENITORES
                MejorSeleccion = Muestra.GetRange(((Muestra.Count()) / 2), ((Muestra.Count()) / 2));
                PeorSeleccion = Muestra.GetRange(0, ((Muestra.Count()) / 2));

                Progenitores.Clear();
                do {
                    foreach (Solucion individuo in MejorSeleccion)
                    {
                        if ((((Tamano * ProporcionPadres) / 4)*3) > Progenitores.Count())
                        {
                            probabildiadAleatoria = new Random().NextDouble();
                            if (probabildiadAleatoria < individuo.ProbabilidadSeleccion)
                            {
                                Progenitores.Add(individuo);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                } while ((((Tamano * ProporcionPadres) / 4) * 3) > Progenitores.Count());

                do
                {
                    foreach (Solucion individuo in PeorSeleccion)
                    {
                        if ((Tamano * ProporcionPadres) > Progenitores.Count())
                        {
                            probabildiadAleatoria = new Random().NextDouble();
                            if (probabildiadAleatoria < individuo.ProbabilidadSeleccion)
                            {
                                Progenitores.Add(individuo);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                } while ((Tamano * ProporcionPadres) > Progenitores.Count());


                //Cruzar PROGENITORES en base a una probabilidad
                Muestra.Reverse();
                Crias.Clear();
                foreach (Solucion progenitor in Progenitores)
                {
                    int indice = Progenitores.IndexOf(progenitor);
                    if (indice % 2 != 0 && indice < Progenitores.Count) {
                        probabildiadAleatoria = new Random().NextDouble();
                        if (probabildiadAleatoria < ProbabilidadCruza)
                        {
                            //Engendrar Nueva CRIA
                            Solucion cria = new Solucion();
                            cria = Progenitores[indice].CruzarSolucion(Progenitores[indice - 1]);
                            cria.Identificador = generacionActual.ToString() + cria.Identificador;
                            //Mutar CRIA
                            cria.MutarSolucion(ProbabilidadMuta);
                            //Evalauar CRIA
                            cria.EvaluarSolucion();
                            Crias.Add(cria);
                        }
                    }
                }

                //Añadir CRIAS a la MUESTRA
                foreach (Solucion cria in Crias)
                {
                    Muestra.Add(cria);
                }

                //Ordenar por Aptitud la MUESTRA que tiene las CRIAS generadas
                Muestra = Muestra.OrderByDescending(individuo => individuo.Aptitud).ToList<Solucion>();
                //Seleccionar a los individuos con las mejores Aptitudes de la MUESTRA
                if (Muestra.Count > Tamano)
                {
                    Muestra = Muestra.GetRange(Muestra.Count - Tamano, Tamano);
                }

                //Sustituir 20% de los elementos
                int numEliminar = (int)Math.Round(Muestra.Count * 0.2);
                for (int i = 0; i < numEliminar; i++)
                {
                    Muestra.RemoveAt(0);
                    Solucion s = new Solucion();
                    s.IniciarSolucion();
                    s.EvaluarSolucion();
                    Muestra.Add(s);
                }

                //Ordenar la muestra Con los nuevos 20% 
                Muestra = Muestra.OrderByDescending(individuo => individuo.Aptitud).ToList<Solucion>();

                //Imprimir el Mejor Candidato de la Generacion
                mejorEspecimen = Muestra.Last<Solucion>();
                mejorEspecimen.Encabezado = "|*| Generacion:" + generacionActual + "   ["+mejorEspecimen.Identificador+"] Mejor Cria: " + mejorEspecimen.Nombre + " |*|\n" + "Numero Ataques= ";
                mejorEspecimen.ImprimirEncabezado();

                //Si el Mejor Candidato tiene una Aptitud de 0, terminar Programa.
                if (mejorEspecimen.Aptitud == 0){
                    break;

                }else{
                    generacionActual++;
                    if (generacionActual<=Generaciones) { //Thread.Sleep(10);
                                                          Console.Clear(); }
                }                
            }
        }
    }
}
