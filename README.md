# Sistema de Navegación del Campus Universitario (SNCU)

Proyecto en C# Windows Forms para el examen práctico de Programación con Estructura de Datos.

## Integrantes

 - `BS242634`: Gerardo Rafael Bonilla Saz
 - `GC242649`: Christian Levi González Castro

## Estructura incluida

- `Grafo.cs`: grafo bidireccional con pesos, BFS y DFS.
- `TablaHash.cs`: registro de visitas con `Dictionary<string, int>`.
- `MinHeap.cs`: heap mínimo manual para ordenar rutas por distancia.
- `MainForm.cs`: interfaz gráfica con mapa, controles y panel de resultados.
- `Program.cs`: punto de entrada de la aplicación.

## Datos del campus

Edificios:

- A: Biblioteca Central
- B: Cafetería
- C: Laboratorio de Cómputo
- D: Rectoría
- E: Gimnasio
- F: Aulas Generales
- G: Estacionamiento

Caminos bidireccionales:

- A — B: 120 m
- A — C: 200 m
- B — D: 150 m
- B — E: 300 m
- C — F: 100 m
- D — F: 80 m
- E — G: 250 m
- F — G: 180 m

## Cómo ejecutar

1. Abrir `DFinalPed.csproj` en Visual Studio 2022.
2. Restaurar/compilar el proyecto.
3. Ejecutar con el botón **Start**.

El proyecto usa `net9.0-windows` y Windows Forms. Si Visual Studio pide instalar el SDK de .NET 9, aceptar la instalación.

## Operaciones implementadas

### 1. Mostrar Grafo
Imprime la lista de adyacencia completa de cada edificio.

### 2. Recorrido BFS
Usa `Queue<string>` para recorrer desde el edificio origen y muestra niveles por saltos.

### 3. Recorrido DFS
Usa `Stack<string>` para buscar un camino desde origen hasta destino, imprime los edificios visitados, el camino encontrado y la distancia total.

### 4. Tabla Hash
Después de ejecutar BFS o DFS, cada visita se registra en `TablaHash`. La opción muestra estadísticas ordenadas de mayor a menor y el edificio más visitado.

### 5. Min-Heap Rutas
Inserta las 8 rutas del campus y las extrae de menor a mayor distancia usando un heap mínimo manual.
