using System.Diagnostics;

const int filas = 5;
const int columnas = 5;
const int cantidadClientes = 50;

Console.WriteLine("** Sistema de ventas de tickets **");
Console.WriteLine("1) Ejecución sin sincronización de hilos");
Console.WriteLine("2) Ejecución con sincronización de hilos Semaforo");
Console.Write("Opción: ");

int opcionEjec = Convert.ToInt32(Console.ReadLine());
Stopwatch stopwatch = Stopwatch.StartNew();

int[,] butacas = new int[filas, columnas];
bool[] comprasRealizadas = new bool[cantidadClientes];
bool[] comprasNoRealizadas = new bool[cantidadClientes];
Thread[] hilos = new Thread[cantidadClientes];
using ManualResetEventSlim manejoHilos = new(false); /* Ayudado con IA, agregarlo al informe */
SemaphoreSlim semaforoHilos = new SemaphoreSlim(1,1);

for (int i = 0; i < cantidadClientes; i++)
{
    int clienteId = i + 1;
    hilos[i] = new Thread(() =>
    {
        manejoHilos.Wait();
        if(opcionEjec == 1){ComprarButaca(clienteId);}
        else if(opcionEjec == 2){ComprarButacaSemaforo(clienteId);}
    });
    hilos[i].Start();
}

Console.WriteLine($"Iniciando {cantidadClientes} clientes para comprar {filas * columnas} butacas...");
manejoHilos.Set();

foreach (Thread hilo in hilos)
{
    hilo.Join();
}

TimeSpan te = stopwatch.Elapsed;
int ventasRegistradas = comprasRealizadas.Count(compra => compra);
int ventasNoConcentradas = comprasNoRealizadas.Count(compra => compra);
int butacasOcupadas = 0;

for (int fila = 0; fila < filas; fila++)
{
    for (int columna = 0; columna < columnas; columna++)
    {
        if (butacas[fila, columna] != 0)
        {
            butacasOcupadas++;
        }
    }
}

Console.WriteLine("\n--------------------------------------------------------------------------");
Console.WriteLine("Estado final de las butacas (0 = Libre / otro número = Cliente Registrado):\n");

for (int fila = 0; fila < filas; fila++)
{
    for (int columna = 0; columna < columnas; columna++)
    {
        Console.Write($"{butacas[fila, columna],4}");
    }

    Console.WriteLine();
}

Console.WriteLine($"\nCompras informadas por clientes: {ventasRegistradas}");
Console.WriteLine($"Butacas ocupadas al final:      {butacasOcupadas}");
Console.WriteLine($"Sobreventas posibles:           {ventasRegistradas - butacasOcupadas}");
Console.WriteLine($"Clientes que no pudieron comprar:           {ventasNoConcentradas}");
Console.WriteLine($"Tiempo de ejecución:           {te.Milliseconds}ms");

void ComprarButaca(int clienteId)
{
    
    int fila = Random.Shared.Next(filas);
    int columna = Random.Shared.Next(columnas);

    if (butacas[fila, columna] == 0)
    {
        Thread.Sleep(10); /* Simula latencia para que se genere la race-condition (Ayudado con IA, agregarlo al informe) */
        butacas[fila, columna] = clienteId;
        comprasRealizadas[clienteId - 1] = true;
        Console.WriteLine($"Cliente {clienteId,3} compró la butaca ({fila + 1}, {columna + 1})");
        return;
    }
    
    comprasNoRealizadas[clienteId - 1] = true;
    Console.WriteLine($"Cliente {clienteId,3} no encontró una butaca libre.");
}

void ComprarButacaSemaforo(int clienteId)
{
    
    int fila = Random.Shared.Next(filas);
    int columna = Random.Shared.Next(columnas);
    
    semaforoHilos.Wait();
    if (butacas[fila, columna] == 0)
    {
        Thread.Sleep(10); /* Simula latencia para que se genere la race-condition (Ayudado con IA, agregarlo al informe) */
        butacas[fila, columna] = clienteId;
        semaforoHilos.Release();
        comprasRealizadas[clienteId - 1] = true;
        Console.WriteLine($"Cliente {clienteId,3} compró la butaca ({fila + 1}, {columna + 1})");
        return;
    }
    
    semaforoHilos.Release();
    comprasNoRealizadas[clienteId - 1] = true;
    Console.WriteLine($"Cliente {clienteId,3} no encontró una butaca libre.");
}
