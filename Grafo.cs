using System;
using System.Collections.Generic;
using System.Text;

namespace DFinalPed
{
    public class Grafo
    {
        private readonly Dictionary<string, Edificio> edificios;
        private readonly Dictionary<string, List<Camino>> adyacencia;

        public List<string> UltimasVisitas { get; set; }
        public List<string> UltimoCamino { get; set; }

        private static readonly Dictionary<string, string> NombresCampus = new Dictionary<string, string>()
        {
            { "A", "Biblioteca Central" },
            { "B", "Cafetería" },
            { "C", "Laboratorio de Cómputo" },
            { "D", "Rectoría" },
            { "E", "Gimnasio" },
            { "F", "Aulas Generales" },
            { "G", "Estacionamiento" }
        };

        public Grafo()
        {
            edificios = new Dictionary<string, Edificio>();
            adyacencia = new Dictionary<string, List<Camino>>();
            UltimasVisitas = new List<string>();
            UltimoCamino = new List<string>();
        }

        public Dictionary<string, Edificio> Edificios
        {
            get { return edificios; }
        }

        public Dictionary<string, List<Camino>> Adyacencia
        {
            get { return adyacencia; }
        }

        public static Grafo CrearCampusPorDefecto()
        {
            Grafo grafo = new Grafo();

            grafo.AgregarEdificio("A", "Biblioteca Central");
            grafo.AgregarEdificio("B", "Cafetería");
            grafo.AgregarEdificio("C", "Laboratorio de Cómputo");
            grafo.AgregarEdificio("D", "Rectoría");
            grafo.AgregarEdificio("E", "Gimnasio");
            grafo.AgregarEdificio("F", "Aulas Generales");
            grafo.AgregarEdificio("G", "Estacionamiento");

            grafo.AgregarCamino("A", "B", 120);
            grafo.AgregarCamino("A", "C", 200);
            grafo.AgregarCamino("B", "D", 150);
            grafo.AgregarCamino("B", "E", 300);
            grafo.AgregarCamino("C", "F", 100);
            grafo.AgregarCamino("D", "F", 80);
            grafo.AgregarCamino("E", "G", 250);
            grafo.AgregarCamino("F", "G", 180);

            return grafo;
        }

        public void AgregarEdificio(string nombre)
        {
            string clave = ResolverClave(nombre);
            string nombreCompleto = nombre;

            if (NombresCampus.ContainsKey(clave))
            {
                nombreCompleto = NombresCampus[clave];
            }

            AgregarEdificio(clave, nombreCompleto);
        }

        public void AgregarEdificio(string clave, string nombre)
        {
            if (string.IsNullOrWhiteSpace(clave))
            {
                throw new ArgumentException("La clave del edificio no puede estar vacía.");
            }

            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException("El nombre del edificio no puede estar vacío.");
            }

            string claveNormalizada = clave.Trim().ToUpperInvariant();
            string nombreNormalizado = nombre.Trim();

            if (edificios.ContainsKey(claveNormalizada))
            {
                return;
            }

            edificios.Add(claveNormalizada, new Edificio(claveNormalizada, nombreNormalizado));
            adyacencia.Add(claveNormalizada, new List<Camino>());
        }

        public void AgregarCamino(string origen, string destino, int distancia)
        {
            if (distancia <= 0)
            {
                throw new ArgumentException("La distancia debe ser mayor que cero.");
            }

            string claveOrigen = ResolverClave(origen);
            string claveDestino = ResolverClave(destino);

            if (!edificios.ContainsKey(claveOrigen))
            {
                AgregarEdificio(claveOrigen);
            }

            if (!edificios.ContainsKey(claveDestino))
            {
                AgregarEdificio(claveDestino);
            }

            AgregarAdyacencia(claveOrigen, claveDestino, distancia);
            AgregarAdyacencia(claveDestino, claveOrigen, distancia);
        }

        private void AgregarAdyacencia(string origen, string destino, int distancia)
        {
            Camino existente = null!;

            foreach (Camino camino in adyacencia[origen])
            {
                if (camino.Destino.ToUpper() == destino.ToUpper())
                {
                    existente = camino;
                    break;
                }
            }

            if (existente == null)
            {
                adyacencia[origen].Add(new Camino(origen, destino, distancia));
                return;
            }

            existente.Distancia = distancia;
        }

        public string MostrarGrafo()
        {
            StringBuilder salida = new StringBuilder();
            salida.AppendLine("=== LISTA DE ADYACENCIA ===");
            salida.AppendLine(new string('-', 50));

            foreach (string clave in ObtenerClavesOrdenadas())
            {
                salida.Append(FormatearEdificioCorto(clave));
                salida.Append(":");

                foreach (Camino camino in adyacencia[clave])
                {
                    salida.Append(" -> ");
                    salida.Append(FormatearEdificioCorto(camino.Destino));
                    salida.Append(" [");
                    salida.Append(camino.Distancia);
                    salida.Append("m]");
                }

                salida.AppendLine();
            }

            salida.AppendLine(new string('-', 50));

            string texto = salida.ToString();
            Console.WriteLine(texto);
            return texto;
        }

        public string RecorridoBFS(string inicio)
        {
            string claveInicio = ResolverClaveExistente(inicio);
            UltimasVisitas = new List<string>();
            UltimoCamino = new List<string>();

            Queue<string> cola = new Queue<string>();
            HashSet<string> visitados = new HashSet<string>();
            Dictionary<string, int> niveles = new Dictionary<string, int>();

            visitados.Add(claveInicio);
            niveles[claveInicio] = 0;
            cola.Enqueue(claveInicio);

            while (cola.Count > 0)
            {
                string actual = cola.Dequeue();
                UltimasVisitas.Add(actual);

                foreach (Camino camino in adyacencia[actual])
                {
                    if (visitados.Contains(camino.Destino))
                    {
                        continue;
                    }

                    visitados.Add(camino.Destino);
                    niveles[camino.Destino] = niveles[actual] + 1;
                    cola.Enqueue(camino.Destino);
                }
            }

            StringBuilder salida = new StringBuilder();
            salida.AppendLine("▶ Iniciando BFS desde: " + FormatearEdificioCorto(claveInicio));
            salida.AppendLine(new string('-', 50));

            for (int nivel = 0; nivel <= ObtenerNivelMayor(niveles); nivel++)
            {
                salida.Append("Nivel ");
                salida.Append(nivel);
                salida.Append(": ");

                List<string> edificiosNivel = ObtenerEdificiosPorNivel(niveles, nivel);

                for (int i = 0; i < edificiosNivel.Count; i++)
                {
                    if (i > 0)
                    {
                        salida.Append(" | ");
                    }

                    salida.Append(FormatearEdificioCorto(edificiosNivel[i]));
                }

                salida.AppendLine();
            }

            salida.AppendLine(new string('-', 50));
            salida.AppendLine("Total de edificios visitados: " + UltimasVisitas.Count);

            string texto = salida.ToString();
            Console.WriteLine(texto);
            return texto;
        }

        public string RecorridoDFS(string inicio, string destino)
        {
            string claveInicio = ResolverClaveExistente(inicio);
            string claveDestino = ResolverClaveExistente(destino);

            UltimasVisitas = new List<string>();
            UltimoCamino = new List<string>();

            Stack<string> pila = new Stack<string>();
            HashSet<string> descubiertos = new HashSet<string>();
            HashSet<string> visitados = new HashSet<string>();
            Dictionary<string, string> padre = new Dictionary<string, string>();

            StringBuilder salida = new StringBuilder();
            salida.AppendLine("▶ DFS: " + FormatearEdificioCorto(claveInicio) + " --> " + FormatearEdificioCorto(claveDestino));
            salida.AppendLine(new string('-', 50));

            pila.Push(claveInicio);
            descubiertos.Add(claveInicio);
            padre[claveInicio] = string.Empty;

            bool encontrado = false;

            while (pila.Count > 0)
            {
                string actual = pila.Pop();

                if (visitados.Contains(actual))
                {
                    continue;
                }

                visitados.Add(actual);
                UltimasVisitas.Add(actual);
                salida.AppendLine("Visitando: " + FormatearEdificioCorto(actual));

                if (actual.ToUpper() == claveDestino.ToUpper())
                {
                    encontrado = true;
                    break;
                }

                List<Camino> vecinosParaPila = ObtenerVecinosParaPila(actual, descubiertos);

                foreach (Camino camino in vecinosParaPila)
                {
                    if (descubiertos.Contains(camino.Destino))
                    {
                        continue;
                    }

                    descubiertos.Add(camino.Destino);
                    padre[camino.Destino] = actual;
                    pila.Push(camino.Destino);
                }
            }

            if (encontrado)
            {
                UltimoCamino = ReconstruirCamino(claveInicio, claveDestino, padre);
                salida.AppendLine();
                salida.AppendLine("✓ Camino encontrado:");
                salida.AppendLine("    " + string.Join(" -> ", UltimoCamino));
                salida.AppendLine("Distancia total del camino: " + CalcularDistanciaCamino(UltimoCamino) + " metros");
            }
            else
            {
                salida.AppendLine("✗ Camino no encontrado entre " + FormatearEdificio(claveInicio) + " y " + FormatearEdificio(claveDestino) + ".");
            }

            string texto = salida.ToString();
            Console.WriteLine(texto);
            return texto;
        }

        private List<Camino> ObtenerVecinosParaPila(string actual, HashSet<string> descubiertos)
        {
            List<Camino> ordenPopDeseado = new List<Camino>();

            foreach (Camino camino in adyacencia[actual])
            {
                if (!descubiertos.Contains(camino.Destino))
                {
                    ordenPopDeseado.Add(camino);
                }
            }

            OrdenarCaminosPorClave(ordenPopDeseado);

            return ordenPopDeseado;
        }

        private List<string> ReconstruirCamino(string inicio, string destino, Dictionary<string, string> padre)
        {
            List<string> camino = new List<string>();
            string actual = destino;

            while (!string.IsNullOrEmpty(actual))
            {
                camino.Add(actual);

                if (actual.ToUpper() == inicio.ToUpper())
                {
                    break;
                }

                if (padre.ContainsKey(actual))
                {
                    actual = padre[actual];
                }
                else
                {
                    actual = string.Empty;
                }
            }

            camino.Reverse();
            return camino;
        }

        public int CalcularDistanciaCamino(IList<string> camino)
        {
            if (camino == null || camino.Count < 2)
            {
                return 0;
            }

            int total = 0;

            for (int i = 0; i < camino.Count - 1; i++)
            {
                string origen = ResolverClaveExistente(camino[i]);
                string destino = ResolverClaveExistente(camino[i + 1]);
                Camino tramo = null!;

                foreach (Camino posibleTramo in adyacencia[origen])
                {
                    if (posibleTramo.Destino.ToUpper() == destino.ToUpper())
                    {
                        tramo = posibleTramo;
                        break;
                    }
                }

                if (tramo == null)
                {
                    throw new InvalidOperationException("El camino contiene un tramo inexistente: " + origen + " -> " + destino);
                }

                total += tramo.Distancia;
            }

            return total;
        }

        public List<Camino> ObtenerCaminosDirectos(string origen)
        {
            string claveOrigen = ResolverClaveExistente(origen);
            List<Camino> caminos = new List<Camino>();

            foreach (Camino camino in adyacencia[claveOrigen])
            {
                caminos.Add(new Camino(camino.Origen, camino.Destino, camino.Distancia));
            }

            return caminos;
        }

        public List<Camino> ObtenerTodosLosCaminosUnicos()
        {
            List<Camino> caminos = new List<Camino>();
            HashSet<string> usados = new HashSet<string>();

            foreach (string origen in ObtenerClavesOrdenadas())
            {
                foreach (Camino camino in adyacencia[origen])
                {
                    string clave1;
                    string clave2;

                    if (origen.CompareTo(camino.Destino) < 0)
                    {
                        clave1 = origen;
                        clave2 = camino.Destino;
                    }
                    else
                    {
                        clave1 = camino.Destino;
                        clave2 = origen;
                    }
                    string llave = clave1 + "-" + clave2;

                    if (usados.Contains(llave))
                    {
                        continue;
                    }

                    usados.Add(llave);
                    caminos.Add(new Camino(clave1, clave2, camino.Distancia));
                }
            }

            return caminos;
        }

        public string FormatearEdificio(string claveONombre)
        {
            string clave = ResolverClave(claveONombre);

            if (edificios.ContainsKey(clave))
            {
                return edificios[clave].Nombre + " (" + clave + ")";
            }

            return claveONombre;
        }

        public string FormatearEdificioCorto(string claveONombre)
        {
            string clave = ResolverClave(claveONombre);

            if (edificios.ContainsKey(clave))
            {
                return NombreCorto(clave) + " (" + clave + ")";
            }

            return claveONombre;
        }

        public string NombreCorto(string claveONombre)
        {
            string clave = ResolverClave(claveONombre);

            if (!edificios.ContainsKey(clave))
            {
                return claveONombre;
            }

            switch (clave.ToUpperInvariant())
            {
                case "A": return "Biblioteca";
                case "B": return "Cafetería";
                case "C": return "Laboratorio";
                case "D": return "Rectoría";
                case "E": return "Gimnasio";
                case "F": return "Aulas";
                case "G": return "Estacionam.";
                default: return edificios[clave].Nombre;
            }
        }

        public string ResolverClaveExistente(string entrada)
        {
            string clave = ResolverClave(entrada);

            if (!edificios.ContainsKey(clave))
            {
                throw new ArgumentException("No existe el edificio: " + entrada);
            }

            return clave;
        }

        public string ResolverClave(string entrada)
        {
            if (string.IsNullOrWhiteSpace(entrada))
            {
                throw new ArgumentException("El edificio no puede estar vacío.");
            }

            string texto = entrada.Trim();
            int inicioParentesis = texto.LastIndexOf('(');
            int finParentesis = texto.LastIndexOf(')');

            if (inicioParentesis >= 0 && finParentesis > inicioParentesis + 1)
            {
                string dentro = texto.Substring(inicioParentesis + 1, finParentesis - inicioParentesis - 1).Trim();

                if (dentro.Length > 0)
                {
                    string posibleClave = dentro.Substring(0, 1).ToUpperInvariant();

                    if (NombresCampus.ContainsKey(posibleClave) || edificios.ContainsKey(posibleClave))
                    {
                        return posibleClave;
                    }
                }
            }

            string mayus = texto.ToUpperInvariant();

            if (NombresCampus.ContainsKey(mayus) || edificios.ContainsKey(mayus))
            {
                return mayus;
            }

            foreach (KeyValuePair<string, Edificio> edificio in edificios)
            {
                if (edificio.Value.Nombre.ToUpper() == texto.ToUpper())
                {
                    return edificio.Key;
                }
            }

            foreach (KeyValuePair<string, string> nombre in NombresCampus)
            {
                if (nombre.Value.ToUpper() == texto.ToUpper())
                {
                    return nombre.Key;
                }
            }

            return mayus;
        }

        public List<string> ObtenerClavesOrdenadas()
        {
            List<string> claves = new List<string>();

            foreach (string clave in edificios.Keys)
            {
                claves.Add(clave);
            }

            claves.Sort(CompararClaves);
            return claves;
        }

        private int ObtenerNivelMayor(Dictionary<string, int> niveles)
        {
            int mayor = 0;

            foreach (KeyValuePair<string, int> nivel in niveles)
            {
                if (nivel.Value > mayor)
                {
                    mayor = nivel.Value;
                }
            }

            return mayor;
        }

        private List<string> ObtenerEdificiosPorNivel(Dictionary<string, int> niveles, int nivelBuscado)
        {
            List<string> resultado = new List<string>();

            foreach (KeyValuePair<string, int> nivel in niveles)
            {
                if (nivel.Value == nivelBuscado)
                {
                    resultado.Add(nivel.Key);
                }
            }

            resultado.Sort(CompararClaves);
            return resultado;
        }

        private void OrdenarCaminosPorClave(List<Camino> caminos)
        {
            caminos.Sort(CompararCaminosPorDestino);
        }

        private int CompararCaminosPorDestino(Camino caminoA, Camino caminoB)
        {
            return CompararClaves(caminoA.Destino, caminoB.Destino);
        }

        private int CompararClaves(string claveA, string claveB)
        {
            return ObtenerIndiceOrden(claveA).CompareTo(ObtenerIndiceOrden(claveB));
        }

        private int ObtenerIndiceOrden(string clave)
        {
            string orden = "ABCDEFG";
            int indice = orden.IndexOf(clave.ToUpperInvariant());
            if (indice >= 0)
            {
                return indice;
            }

            return int.MaxValue;
        }
    }

    public class Edificio
    {
        public string Clave { get; set; }
        public string Nombre { get; set; }

        public Edificio(string clave, string nombre)
        {
            Clave = clave;
            Nombre = nombre;
        }

        public override string ToString()
        {
            return Nombre + " (" + Clave + ")";
        }
    }

    public class Camino
    {
        public string Origen { get; set; }
        public string Destino { get; set; }
        public int Distancia { get; set; }

        public Camino(string origen, string destino, int distancia)
        {
            Origen = origen;
            Destino = destino;
            Distancia = distancia;
        }
    }
}

