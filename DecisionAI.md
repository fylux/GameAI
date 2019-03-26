## Notas iniciales
El diseño del sistema es casi 100% original, ya que no se dispone de un diseño de una IA completa RTS que podamos tomar de referencia. Los únicos recursos útiles encontrados son:
 - A
 - B

Cabe destacar que la IA es dificil que tenga sentido de por si ya que no hay forma de hacerse mas fuerte, no hay puntos de control, recursos, etc, por lo que lo unico que se puede hacer es atacar o esperar.

#### Terminología
Aquellos términos que son una idea general requieren que se añadan detalles cuando se refiera a ellos.
 - BT persistente: Arbol de comportamiento que sigue la idea tradicional de los BT. Cuando se llega a una acción se queda esperando a que esa acción termine para obtener el resultado (V o F) y continuar.
 - BT volatil: (Idea general) Arbol de comportamiento en el que más que acciones hay órdenes. Las órdenes no devuelven un valor (V o F) por lo que el árbol debe estar diseñado de forma que las acciones reales las ejecutarán otros subarboles.
 - IA Estratégica: Tambien llamada Cerebro estratégico / IA Jugador solo hay una e imita el comportamiento de un humano jugando. Se centra en órdenes generales.
 - IA Táctica: Comportamiento de la unidad en base a su propia información. No confundir con la mecánica para realizar órdenes solicitadas por la IA Estratégica.
 - Tarea: Conjunto de comportamientos en una unidad para realizar una acción.

<hr />

## Arquitectura IA Decisiones
Preguntas que debe resolver el diseño de la IA.

### Cerebro estrategico / Jugador
Suponemo que el cerebro estratégico sigue la estructura de un árbol de comportamiento adaptado a nuestras necesidades.

¿Hay un solo árbol estrategico (con subarboles) o hay varios arboles distintos que se ejecutan en paralelo?
Ejecutar en paralelo por ejemplo a veces se hace si hay un arbol para estrategia militar, estrategia economica, estregia diplomatica, etc.

¿Espera que se ejecuten las acciones para continuar de recorrer el arbol?

 - Si espera a que las acciones ejecuta, ¿como considera interrumpir un proceso? Por ejemplo esta con una acción que supone un ataque coordinado a la base enemiga y de repente hay una ataque sorpresa en la base, ¿como dedice dejar esa estrategia?

 - ¿Como hace para evitar que al volver a ejecutarse el arbol repita la misma accion continuamente?
Ejemplo, si la base esta siendo atacada que mande continuamente la orden de volver a la base cuando esta orden ya se mandó.

¿La IA estregica solo puede ejecutar una accion al mismo tiempo?
Esto afecta si se mandan ordenes a grupos de unidades en vez de a todas. Por ejemplo, si ordena que un grupo de unidades defienda la base, no podría al mismo tiempo ordenar a otro grupo que explore. Esta relacionado con la cuestión de si las acciones son bloqueantes.

¿Como es el proceso por el que IA estrategica consigue hacer llegar las ordenes a las unidades?

¿Cuando se considera una accion estrategica terminada?

¿La IA estrategica siempre da ordenes o solo en determinadas situaciones?
 - Si no hay una orden estratégica asignada, ¿como se comporta las unidades?

### IA Táctica
¿Como reacciona una unidad a eventos particulares que le suceden? Ejemplo tiene orden de volver a la base pero hay un monton de unidades enemigas que intentan atacar a esta unidad.

¿Como decide que hacer cuando tiene una orden estratégica pero sucede un evento importante?

¿Una unidad solo puede ejecutar una tarea al mismo tiempo?

¿Hay una lista de órdenes recibidas, o simplemente se decide si aceptar o no aceptar una petición?
 - Si hay una lista
   - ¿Cómo se decide cual aplicar?
   - ¿Hay un timeout para las ordenes?
 - Si no hay una lista
   - ¿Se guarda la orden estratégica asignada incluso si se decide seguir la orden táctica?

<hr />

## Propuesta 1
### Resumen

### Respuestas a preguntas

### Problemas conocidos
