using System.Diagnostics;

int filas = 5;
int columnas = 5;
int cantidadClientes = 100;
string tipoEjecucion = "Sin valor";

while(true){

Console.Clear();
Console.WriteLine("** Sistema de ventas de tickets **");
Console.WriteLine($"Simulacion con: {filas} filas, {columnas} columnas, {cantidadClientes} clientes");
Console.WriteLine("1) Ejecución sin sincronización de hilos");
Console.WriteLine("2) Ejecución con sincronización de hilos -Semaforo-");
Console.WriteLine("3) Ejecución con sincronización de hilos -Lock-");
Console.WriteLine("4) Cambiar cantidad de clientes y butacas");
Console.WriteLine("5) Salir");
Console.Write("Opción: ");

int opcionEjec = Convert.ToInt32(Console.ReadLine());

switch (opcionEjec)
{
    case 4:
        Console.Write($"Ingrese la cantidad de filas (Actual: {filas}): ");
        int filasNuevo = Convert.ToInt32(Console.ReadLine());
        Console.Write($"Ingrese la cantidad de columnas (Actual: {columnas}): ");
        int columnasNuevo = Convert.ToInt32(Console.ReadLine());
        Console.Write($"Ingrese la cantidad de clientes (Actual: {cantidadClientes}): ");
        int clientesNuevo = Convert.ToInt32(Console.ReadLine());

        if (filasNuevo > 0 && columnasNuevo > 0 && clientesNuevo > 0)
        {
            filas = filasNuevo;
            columnas = columnasNuevo;
            cantidadClientes = clientesNuevo;
            Console.WriteLine("Cantidad de filas, columnas y clientes actualizada correctamente.");
        }
        else
        {
            Console.WriteLine("Los valores ingresados deben ser mayores a cero. No se realizaron cambios.");
        }

        Console.WriteLine("Presione cualquier tecla para continuar...");
        Console.ReadKey();
        continue; 
    case 5:
        return;
    case < 1 or > 5:
        Console.WriteLine("Opción no válida.");
        Console.ReadKey();
        continue;
}

int[,] butacas = new int[filas, columnas];
bool[] comprasRealizadas = new bool[cantidadClientes];
bool[] comprasNoRealizadas = new bool[cantidadClientes];
Thread[] hilos = new Thread[cantidadClientes]; 

/* Variables sincronizacion */
using ManualResetEventSlim manejoHilos = new(false); 
object sync = new object();
SemaphoreSlim semaforoHilos = new SemaphoreSlim(1,1);

/* Cronometro */
Stopwatch stopwatch = Stopwatch.StartNew();

for (int i = 0; i < cantidadClientes; i++)
{
    int clienteId = i + 1;
    hilos[i] = new Thread(() =>
    {
        manejoHilos.Wait();
        if(opcionEjec == 1){ComprarButaca(clienteId);}
        else if(opcionEjec == 2){ComprarButacaSemaforo(clienteId);}
        else if(opcionEjec == 3){ComprarButacaLock(clienteId);}
    });
    hilos[i].Start();
}

Console.WriteLine($"Iniciando {cantidadClientes} clientes para comprar {filas * columnas} butacas...");
manejoHilos.Set();

foreach (Thread hilo in hilos)
{
    hilo.Join();
}

if(opcionEjec == 1){tipoEjecucion = "sin sincronización";}
else if(opcionEjec == 2){tipoEjecucion = "con semáforo";}
else if(opcionEjec == 3){tipoEjecucion = "con lock";}

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
Console.WriteLine($"*Ejecución {tipoEjecucion}*");
Console.WriteLine("Estado final de las butacas (0 = Libre / otro número = Cliente Registrado):\n");

for (int fila = 0; fila < filas; fila++)
{
    for (int columna = 0; columna < columnas; columna++)
    {
        Console.Write($"{butacas[fila, columna],4}");
    }

    Console.WriteLine();
}

Console.WriteLine($"\n-Compras informadas por clientes: {ventasRegistradas}");
Console.WriteLine($"-Butacas ocupadas al final: {butacasOcupadas}");
Console.WriteLine($"-Sobreventas posibles: {ventasRegistradas - butacasOcupadas}");
Console.WriteLine($"-Clientes que no pudieron comprar: {ventasNoConcentradas}");
Console.WriteLine($"-Tiempo de ejecución: {te.TotalMilliseconds}ms");
Console.ReadLine();


void ComprarButaca(int clienteId)
{
    
    int fila = Random.Shared.Next(filas);
    int columna = Random.Shared.Next(columnas);

    if (butacas[fila, columna] == 0)
    {
        Thread.Sleep(10); 
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
        Thread.Sleep(10); 
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

void ComprarButacaLock(int clienteId)
{
    int fila = Random.Shared.Next(filas);
    int columna = Random.Shared.Next(columnas);
    bool pudoComprar = false;

    lock (sync)
    {
        if (butacas[fila, columna] == 0)
        {
            Thread.Sleep(10);
            butacas[fila, columna] = clienteId;
            pudoComprar = true;
        }
    }

    if (pudoComprar)
    {
        comprasRealizadas[clienteId - 1] = true;
        Console.WriteLine($"Cliente {clienteId,3} compró la butaca ({fila + 1}, {columna + 1})");
    }
    else
    {
        comprasNoRealizadas[clienteId - 1] = true;
        Console.WriteLine($"Cliente {clienteId,3} no encontró una butaca libre.");
    }
}
}