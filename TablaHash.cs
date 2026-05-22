using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DFinalPed
{
    public class TablaHash
    {
        private readonly Dictionary<string, int> visitas;

        public TablaHash()
        {
            visitas = new Dictionary<string, int>();
        }

        public void RegistrarVisita(string edificio)
        {
            if (string.IsNullOrWhiteSpace(edificio))
            {
                throw new ArgumentException("El edificio no puede estar vacío.");
            }

            string clave = edificio.Trim();

            if (!visitas.ContainsKey(clave))
            {
                visitas[clave] = 0;
            }

            visitas[clave]++;
        }

        public int ObtenerConteo(string edificio)
        {
            if (string.IsNullOrWhiteSpace(edificio))
            {
                return 0;
            }

            string clave = edificio.Trim();
            if (visitas.ContainsKey(clave))
            {
                return visitas[clave];
            }

            return 0;
        }

        public string MostrarEstadisticas()
        {
            StringBuilder salida = new StringBuilder();
            salida.AppendLine("=== ESTADÍSTICAS DE VISITAS ===");

            if (visitas.Count == 0)
            {
                salida.AppendLine("Aún no hay visitas registradas. Ejecute BFS o DFS primero.");
                string textoVacio = salida.ToString();
                Console.WriteLine(textoVacio);
                return textoVacio;
            }

            List<KeyValuePair<string, int>> lista = ObtenerVisitasOrdenadas();

            foreach (KeyValuePair<string, int> item in lista)
            {
                string barra = new string('█', item.Value);
                string plural = "";

                if (item.Value != 1)
                {
                    plural = "s";
                }

                salida.AppendLine(item.Key + " : " + barra + " " + item.Value + " visita" + plural);
            }

            salida.AppendLine();
            salida.AppendLine("Edificio más visitado: " + EdificioMasVisitado());

            string texto = salida.ToString();
            Console.WriteLine(texto);
            return texto;
        }

        public string EdificioMasVisitado()
        {
            if (visitas.Count == 0)
            {
                return "No hay visitas registradas";
            }

            KeyValuePair<string, int> mayor = ObtenerVisitasOrdenadas()[0];

            string plural = "";

            if (mayor.Value != 1)
            {
                plural = "s";
            }

            return mayor.Key + " con " + mayor.Value + " visita" + plural;
        }

        public void Limpiar()
        {
            visitas.Clear();
        }

        private List<KeyValuePair<string, int>> ObtenerVisitasOrdenadas()
        {
            List<KeyValuePair<string, int>> lista = new List<KeyValuePair<string, int>>();

            foreach (KeyValuePair<string, int> visita in visitas)
            {
                lista.Add(visita);
            }

            lista.Sort(CompararVisitas);
            return lista;
        }

        private int CompararVisitas(KeyValuePair<string, int> visitaA, KeyValuePair<string, int> visitaB)
        {
            if (visitaA.Value > visitaB.Value)
            {
                return -1;
            }

            if (visitaA.Value < visitaB.Value)
            {
                return 1;
            }

            return visitaA.Key.CompareTo(visitaB.Key);
        }
    }
}