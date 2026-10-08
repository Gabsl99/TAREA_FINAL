# Producto Académico 4: Prototipo de Videojuego 3D en Unity

Proyecto final desarrollado para la asignatura de Desarrollo de Videojuegos. Consiste en un juego de plataformas y recolección en tercera persona implementado en Unity 6, donde el jugador debe recolectar monedas distribuidas en un escenario cerrado mientras evita ser alcanzado por un enemigo con inteligencia artificial.

## Requisitos y Entorno de Desarrollo
* Motor: Unity 6 (6000.0.5f1)
* Lenguaje: C#
* Plataforma: PC (Windows x64)

## Controles del Juego
* Desplazamiento: Teclas W, A, S, D o flechas direccionales.
* Salto: Barra espaciadora (Espacio).
* Reinicio: Tecla R (cuando se muestra la pantalla de Game Over o Victoria).

## Mecánicas y Sistemas Implementados

1. Control del Jugador (PlayerMovement):
   * Control basado en CharacterController para asegurar físicas estables sin atravesar el suelo ni colisionadores.
   * Manejo de gravedad y salto.
   * Cámara en tercera persona con seguimiento suave (SmoothCameraFollow).

2. Inteligencia Artificial del Enemigo (EnemyAI):
   * Comportamiento de patrulla: Recorre una serie de waypoints definidos en el mapa a velocidad constante.
   * Comportamiento de persecución: Al detectar al jugador dentro de un radio de visión, el enemigo cambia de estado y persigue activamente al jugador.
   * Condición de derrota: Si el enemigo alcanza al jugador, se detiene el flujo y se activa el estado de Game Over.

3. Sistema de Recolección y Optimización (Object Pooling):
   * Coleccionables (Coin): Monedas distribuidas en el escenario con rotación continua y detección de recolección por trigger.
   * ObjectPooler: Implementación de un gestor de objetos reutilizables para instanciar las partículas de recolección de monedas sin sobrecargar la memoria ni el recolector de basura (Garbage Collector).

4. Interfaz de Usuario y Flujo de Juego (GameManager y SimpleUI):
   * HUD en pantalla que muestra el contador de monedas recolectadas en tiempo real.
   * Condición de victoria al recolectar todas las monedas del nivel.
   * Condición de derrota si el jugador entra en contacto con el enemigo.
   * Posibilidad de reiniciar la partida presionando la tecla R.

5. Efectos Visuales y Sonido:
   * Sistema de partículas doradas activado al recolectar cada moneda.
   * Efecto de sonido (SFX) sincronizado con la recolección.

## Estructura del Repositorio
* /ProyectoFinalPA4: Proyecto completo con assets, scripts, materiales y escenas de Unity.
* /Build_PA4: Carpeta con el juego compilado y el archivo ejecutable (.exe) probado y listo para ejecutar en Windows sin necesidad de abrir el editor.

## Instrucciones de Ejecución
1. Para jugar directamente: Abrir la carpeta `Build_PA4` y ejecutar el archivo `ProyectoFinalPA4.exe`.
2. Para revisar en el editor: Abrir la carpeta `ProyectoFinalPA4` desde Unity Hub (versión Unity 6) y cargar la escena ubicada en `Assets/Scenes/NivelPrincipal.unity`.
