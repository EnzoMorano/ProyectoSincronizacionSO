# Sistema de venta de tickets

Trabajo práctico para la materia **Sistemas Operativos**, comisión 3, de la **Universidad Nacional de Hurlingham**.

## Descripción

Aplicación de consola desarrollada en C# que simula a varios clientes intentando comprar butacas al mismo tiempo. El objetivo es observar cómo el acceso concurrente a un recurso compartido puede provocar inconsistencias y comparar el resultado al proteger ese acceso con mecanismos de sincronización.

El programa permite ejecutar la simulación:

- Sin sincronización.
- Utilizando un semáforo.
- Utilizando `lock`.

Se pueden configurar la cantidad de filas y columnas de butacas y el número de clientes. Al finalizar, se muestra el estado de las butacas, las compras registradas, las butacas ocupadas, las posibles sobreventas, los clientes que no consiguieron lugar y el tiempo de ejecución.
