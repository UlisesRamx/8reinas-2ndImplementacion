using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace _8reinas
{
    internal class Solucion
    {

        public String[,] Tablero { get; set; }
        public int Aptitud { get; set; }
        public double ProbabilidadSeleccion { get; set; }
        public int ColisionesTotales { get; set; }
        public string Nombre { get; set; }
        public string Identificador { get; set; }
        public string Encabezado { get; set; }

       

        public Solucion() {
            Tablero = new string[8, 8];
            Nombre = SeleccionNombreAleatorio();
        }

        public void IniciarSolucion() {
            Tablero = new string[8, 8];
            int numeroReinas = 0;
            int Xposicion, Yposicion;
            Random random = new Random();
            while (numeroReinas < 8)
            {
                do
                {
                    Xposicion = random.Next(0, 8);
                    Yposicion = random.Next(0, 8);
                } while (Tablero[Yposicion, Xposicion] != null);
                //Tablero[Xposicion, Yposicion] = "  Q"+numeroReinas+" ";
                Tablero[Yposicion, Xposicion] = numeroReinas.ToString();
                numeroReinas++;
            }
        }

        public void ImprimirSolucion() {
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (Tablero[y, x] != null)
                    {
                        Console.Write("[" + Tablero[y, x] + "]");
                    }
                    else
                    {
                        Console.Write("[.]");
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public int EvaluarSolucion() {
            string queen;
            ColisionesTotales = 0;
            int colisionesAH = 0, colisionesAV = 0, colisionesAD = 0;
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (Tablero[y, x] != null)
                    {
                        queen = Tablero[y, x];
                        colisionesAH = ContarAtaquesHorizontales(Tablero, queen, y);
                        colisionesAV = ContarAtaquesVerticales(Tablero, queen, x);
                        colisionesAD = ContarAtaquesDiagonales(Tablero, queen, y, x);
                        ColisionesTotales = ColisionesTotales + colisionesAH + colisionesAV + colisionesAD;
                    }
                }
            }
            Aptitud = ColisionesTotales;
            return ColisionesTotales;
        }

        public void MutarSolucion(double ProbabilidadMuta) {
            string Queen = "", randomQueen;
            int Xposicion,Yposicion;
            Random random = new Random();
            double ProbMuta = random.NextDouble();
            randomQueen = random.Next(1, 8).ToString();
            if (ProbabilidadMuta > ProbMuta) {
                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        if (Tablero[y, x] != null)
                        {
                            Queen = Tablero[y, x];
                            if (randomQueen == Queen)
                            {

                                Tablero[y, x] = null;
                                do
                                {
                                    Xposicion = random.Next(0, 8);
                                    Yposicion = random.Next(0, 8);
                                } while (Tablero[Yposicion, Xposicion] != null);
                                Tablero[Yposicion, Xposicion] = randomQueen;
                                break;
                            }
                        }
                    }
                }
            }

        }

        public Solucion CruzarSolucion(Solucion Pareja) {
            Solucion cria = new Solucion();
            int seleccionarMitad = new Random().Next(0, 2);
            String[,] primerProgenitor = this.Tablero;
            String[,] segundoProgenitor = Pareja.Tablero;
            cria.Identificador = this.Nombre.Substring(0,2)+Pareja.Nombre.Substring(0,2);
            List<String[,]> Progenitores = new List<string[,]>();
            Progenitores.Add(primerProgenitor);
            Progenitores.Add(segundoProgenitor);
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (y >= 4)
                    {
                        switch (seleccionarMitad)
                        {
                            case 0:
                                seleccionarMitad++;
                                break;
                            case 1:
                                seleccionarMitad--;
                                break;
                            default:
                                break;
                        }
                    }
                    if (Progenitores.ElementAt<String[,]>(seleccionarMitad) != null)
                    {
                        cria.Tablero[y, x] = Progenitores.ElementAt<String[,]>(seleccionarMitad)[y, x];
                    }
                }
            }
            var infoReinas = cria.ContarReinas();
            if (infoReinas.Item1 > 8)
            {
                CorregirCriaMuchasReinas(infoReinas.Item2, cria);
                infoReinas = cria.ContarReinas();
            }
            else if (infoReinas.Item1 < 8)
            {
                CorregirCriaMenosReinas(infoReinas.Item2, cria);
                infoReinas = cria.ContarReinas();
            }
            var repetidos = infoReinas.Item2.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (repetidos.Count > 0)
            {
                cria = CorregirValoresCria(infoReinas.Item2, cria);
            }
            return cria;
        }


        private Tuple<int, List<String>> ContarReinas()
        {
            List<String> Queens = new List<String>();
            int numeroReinas = 0;

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (Tablero[y, x] != null)
                    {
                        numeroReinas++;
                        Queens.Add(Tablero[y, x]);
                    }
                }
            }

            return Tuple.Create(numeroReinas, Queens);
        }
        private void CorregirCriaMuchasReinas(List<String> Queens, Solucion Cria) {
            List<String> ValoresPredeterminado = new List<String>() { "0", "1", "2", "3", "4", "5", "6", "7" };
            Random random = new Random();
            string Queen = "";
            int randomQueen;
            do {
                randomQueen = random.Next(0, Queens.Count() - 1);
                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        if (Cria.Tablero[y, x] != null)
                        {
                            Queen = Cria.Tablero[y, x];
                            if (Queen == Queens.ElementAt(randomQueen))
                            {
                                Cria.Tablero[y, x] = null;
                                Queens.Remove(Queen);
                                randomQueen = random.Next(1, Queens.Count() - 1);
                                if (Queens.Count() == 8)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    if (Queens.Count == 8)
                    {
                        break;
                    }
                }
            } while (Cria.ContarReinas().Item1 != 8);
        }
        private void CorregirCriaMenosReinas(List<String> Queens, Solucion Cria)
        {
            List<String> ValoresPredeterminado = new List<String>() { "0", "1", "2", "3", "4", "5", "6", "7" };
            int Xposicion, Yposicion;
            Random random = new Random();
            foreach (String valor in Queens) {
                if (ValoresPredeterminado.Contains(valor)) { 
                    ValoresPredeterminado.Remove(valor);
                }
            }
            while (Cria.ContarReinas().Item1 < 8)
            {
                do
                {
                    Xposicion = random.Next(0, 8);
                    Yposicion = random.Next(0, 8);
                } while (Tablero[Yposicion, Xposicion] != null);
                Cria.Tablero[Yposicion, Xposicion] = ValoresPredeterminado.ElementAt<string>(random.Next(0, ValoresPredeterminado.Count()));
            }
        }
        private Solucion CorregirValoresCria(List<String> Queens, Solucion Cria) {
            List<String> ValoresPredeterminado = new List<String>() { "0", "1", "2", "3", "4", "5", "6", "7" };
            var repetidos = Queens.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            var valoresUnicos = ValoresPredeterminado.Except(Queens).ToList();

            foreach (var valor in repetidos)
            {
                int index = Queens.IndexOf(valor);
                Queens[index] = valoresUnicos[0];
                valoresUnicos.RemoveAt(0);
            }

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (Cria.Tablero[y, x] != null)
                    {
                        Cria.Tablero[y, x] = Queens.FirstOrDefault<String>();
                        Queens.RemoveAt(Queens.IndexOf(Queens.FirstOrDefault<string>()));
                    }
                }
            }
            return Cria;
        }

        private int ContarAtaquesHorizontales(String[,] Tablero, string queen, int columnaQ)
        {
            int AH = 0;
            int x = 0;
            while (x < 8)
            {
                if (Tablero[columnaQ, x] != null && Tablero[columnaQ, x] != queen)
                {
                    AH++;
                }
                x++;
            }
            return AH;
        }
        private int ContarAtaquesVerticales(String[,] Tablero, string queen, int filaQ)
        {
            int AV = 0;
            int y = 0;
            while (y < 8)
            {
                if (Tablero[y, filaQ] != null && Tablero[y, filaQ] != queen)
                {
                    AV++;
                }
                y++;
            }
            return AV;

        }
        private int ContarAtaquesDiagonales(String[,] Tablero, string queen, int columnaQ, int filaQ)
        {
            int AD = 0;
            int x = 0, y = 0;
            // Comprobar las diagonales hacia arriba a la izquierda
            x = filaQ;
            y = columnaQ;
            while (x >= 0 && y >= 0)
            {
                if (Tablero[y, x] != null && Tablero[y, x] != queen)
                {
                    AD++;
                }
                x--;
                y--;
            }

            // Comprobar las diagonales hacia arriba a la derecha
            x = filaQ;
            y = columnaQ;
            while (x < 8 && y >= 0)
            {
                if (Tablero[y, x] != null && Tablero[y, x] != queen)
                {
                    AD++;
                }
                x++;
                y--;
            }

            // Comprobar las diagonales hacia abajo a la izquierda
            x = filaQ;
            y = columnaQ;
            while (x >= 0 && y < 8)
            {
                if (Tablero[y, x] != null && Tablero[y, x] != queen)
                {
                    AD++;
                }
                x--;
                y++;
            }

            // Comprobar las diagonales hacia abajo a la derecha
            x = filaQ;
            y = columnaQ;
            while (x < 8 && y < 8)
            {
                if (Tablero[y, x] != null && Tablero[y, x] != queen)
                {
                    AD++;
                }
                x++;
                y++;
            }

            return AD;
        }

        private String SeleccionNombreAleatorio() {
            List<string> nombres = new List<string>() { "Juan", "Ana", "Pedro", "Marta", "Luis", "Carla", "David", "Sofía", "María", "Andrés", "Rosa", "Pablo", "Elena", "Javier", "Lucía", "José", "Carmen", "Fernando", "Laura", "Miguel", "Paula", "Carlos", "Sara", "Rubén", "Isabel", "Alejandro", "Julia", "Diego", "Victoria", "Francisco", "Lorena", "Raúl", "Natalia", "Gabriel", "Nuria", "Jorge", "Silvia", "Alberto", "Alicia", "Antonio", "Olga", "Tomás", "Beatriz", "Emilio", "Teresa", "Germán", "Patricia", "Guillermo", "Inés", "Adrián", "Clara", "Simón", "Celia", "Roberto", "Cristina", "Rafael", "Mónica", "Ignacio", "Eva", "Víctor", "Aurora", "Oscar", "Gloria", "Federico", "Esther", "Álvaro", "Lidia", "Héctor", "Manuela", "Santiago", "Marina", "Enrique", "Paulina", "Mateo", "Adela", "Bruno", "Sandra", "Gonzalo", "Judit", "Óscar", "Mireia", "Mario", "Miriam", "Xavier", "Amparo", "Jesús", "Ainhoa", "Israel", "Eugenia", "Salvador", "Irma", "Lorenzo", "Alba", "Josué", "Noelia", "Maximiliano", "Ana Belén", "Gustavo", "Almudena" };
            Random random = new Random();
            int indiceAleatorio = random.Next(nombres.Count);
            return nombres[indiceAleatorio];
        }
        public void ImprimirEncabezado() {
            EvaluarSolucion();
            Console.WriteLine(Encabezado + Aptitud);
            ImprimirSolucion();

        }
    }
}
