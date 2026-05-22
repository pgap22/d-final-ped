using System;
using System.Collections.Generic;
using System.Text;

namespace DFinalPed
{
    public class MinHeap
    {
        private readonly List<RutaHeap> heap;

        public MinHeap()
        {
            heap = new List<RutaHeap>();
        }

        public void Insertar(string edificio, int distancia)
        {
            if (string.IsNullOrWhiteSpace(edificio))
            {
                throw new ArgumentException("El edificio/ruta no puede estar vacío.");
            }

            if (distancia < 0)
            {
                throw new ArgumentException("La distancia no puede ser negativa.");
            }

            heap.Add(new RutaHeap(edificio.Trim(), distancia));
            Flotar(heap.Count - 1);
        }

        public RutaHeap ExtraerMinimo()
        {
            if (EstaVacio())
            {
                throw new InvalidOperationException("El heap está vacío.");
            }

            RutaHeap minimo = heap[0];
            int ultimoIndice = heap.Count - 1;
            heap[0] = heap[ultimoIndice];
            heap.RemoveAt(ultimoIndice);

            if (!EstaVacio())
            {
                Hundir(0);
            }

            return minimo;
        }

        public bool EstaVacio()
        {
            return heap.Count == 0;
        }

        public string MostrarRutasOrdenadas()
        {
            StringBuilder salida = new StringBuilder();
            int posicion = 1;

            while (!EstaVacio())
            {
                RutaHeap ruta = ExtraerMinimo();
                salida.AppendLine(posicion + "° " + ruta.Edificio + " " + ruta.Distancia + " metros");
                posicion++;
            }

            string texto = salida.ToString();
            Console.WriteLine(texto);
            return texto;
        }

        private void Flotar(int indice)
        {
            while (indice > 0)
            {
                int padre = (indice - 1) / 2;

                if (heap[indice].Distancia >= heap[padre].Distancia)
                {
                    break;
                }

                Intercambiar(indice, padre);
                indice = padre;
            }
        }

        private void Hundir(int indice)
        {
            while (true)
            {
                int izquierdo = 2 * indice + 1;
                int derecho = 2 * indice + 2;
                int menor = indice;

                if (izquierdo < heap.Count && heap[izquierdo].Distancia < heap[menor].Distancia)
                {
                    menor = izquierdo;
                }

                if (derecho < heap.Count && heap[derecho].Distancia < heap[menor].Distancia)
                {
                    menor = derecho;
                }

                if (menor == indice)
                {
                    break;
                }

                Intercambiar(indice, menor);
                indice = menor;
            }
        }

        private void Intercambiar(int indiceA, int indiceB)
        {
            RutaHeap temporal = heap[indiceA];
            heap[indiceA] = heap[indiceB];
            heap[indiceB] = temporal;
        }
    }

    public class RutaHeap
    {
        public string Edificio { get; set; }
        public int Distancia { get; set; }

        public RutaHeap(string edificio, int distancia)
        {
            Edificio = edificio;
            Distancia = distancia;
        }
    }
}