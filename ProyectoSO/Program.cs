const int filas = 5;
const int columnas = 5;
const int cantidadClientes = 50;

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
        ComprarButaca(clienteId);
    });
    hilos[i].Start();
}

Console.WriteLine($"Iniciando {cantidadClientes} clientes para comprar {filas * columnas} butacas...");
manejoHilos.Set();

foreach (Thread hilo in hilos)
{
    hilo.Join();
}

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

void ComprarButaca(int clienteId)
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
