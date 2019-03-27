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

### IA Estratégica
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



## Modelo inicial
### Estrategias
 - Defender base
 - Defender su mitad del mapa (o defender X distancia alreadedor de la base para que sea más flexible)
 - Atacar mitad del mapa enemigo
 - Atacar base enemiga

### Ordenes a unidades:
 - Dirigete a un punto
 - Dirigete a una zona (radio entorno a un punto)
 - Defender posición
 - Ataque puntual (ataca enemigo hasta que huye)
 - Ataque hostil (ataca enemigo y si huye (mas de X distancia?) los sigue)
 - Patrulla por itinerario
 - Unirse a grupo (Persigue a una unidad aliada hasta que esta suficientemente cerca para unirse al grupo)
 - Atacar zona

### Métodos para obtener información (Zona se refiere a un Radio X desde una casilla del mapa):
 - Influencia media del enemigo/total/aliada en una zona
   - Influencia entorno a la base/base enemiga
   - Influencia entorno a la frontera
 - Fuerza de las unidades en una zona
   - Numero de unidades
   - Vida media de las unidades
   - Ataque medio de unidades
   - % que representa cada tipo de unidad
 - grupo de enemigos más cercanos a la base
 - % del mapa con influencia aliada/enemiga
 - numero de unidades con una determinada estrategia asignada en una zona
 - Obtener peligro de un camino (en base a la influencia de las casillas que cruza)
 - Ver si se ha producido un cambio estable en el interes de objetivos (unos nuevos valores distintos que se han mantenido bastantes segundos)


## 1º Capa
 - Crear clase IA Estratégica cuyo método principal es llamado en el Update cada X segundos
 - Para cada estrategica llama a su método de evaluación que genera un % de su interés en base a funciones de información
 - Se evaluan condiciones mínimas para cada estrategia (se ponen a 0% aquellas que no las cumplen)
 - Se pasa la lista de Interes a la siguiente capa


## 2º Capa
Clase MilitaryScheduling
 - Se multiplican los % por coeficientes de correción para ponderar el interés en las estrategias según nuestro estilo de juego
 - Se seleccionan un máximo de X estrategias (depende de tamaño del mapa, numero unidades con más interés)
 - Para cada estrategia seleccionada se marcan las unidades que mejor se ajustan para esa estrategia
   - Si la unidad ya tenía seleccionada esa estrategia se coge sin problema
   - Si no se deja marcada hasta la siguiente iteración
   - De las unidades que no se han cogido todavía se seleccionan aquellas que mejor se ajustan por ciertas heuristicas
    - Numero de unidades con esa estrategia a su alrededor
    - Cercanía a ciertas zonas (dividido entre velocidad media?)


## 3º Capa
Cada X segundos se llama a la Clase, CoordinateAI que llama a la ejecución de la lógica de cada estrategia

#### Defender base
Es la más fácil, solo hay que mandar todas las unidades a lo loco a la base
 - ¿Hay unidades que no están en la base y no se están reagrupando?
   - Ordenar ir a la base
 - ¿Hay unidades yendo a la base que están cercanas o cuyos caminos se vayan a cruzar?
   - Ordenar agruparse
 - ¿Hay unidades en la base sin orden de defender posición?
   - Ordenar defender posición

#### Defender su mitad del mapa

#### Atacar mitad del mapa enemigo

#### Atacar base enemiga
 - Obtener lista de unidades ordenada por cercanía a la base 
   - ¿Hay unidades muy alejadas de la mayoría cercana a la base?
     - Ordenar unirse al grupo más cercano
   - Si el grupo más cercano a la base tiene más de X unidades
     - Elegir siguiente posición más cercana a la base
     - Mandar al grupo a esa posición
   - Si la distancia a la base es pequeña y numero de unidades > X
     - Ordenar atacar zona

## 4º Capa
Cada frame en el update de la unidad se llama a la función para obtener steering de la tarea asignada.

#### Dirigete a un punto
 - Calcula camino a un punto con factor de miedo
   - Factor de miedo función de vida, numero unidades aliadas cercanas, influencia, etc.
 - Seguir camino
 - (Opcional) Cada X tiempo recalcular camino por si ya no fuera seguro
 - Si estrategia==(Atacar) y fuerza_unidad_y_aliados * actitud_riesgo < peligro camino
   - ¿Replantearse que hacer?


#### Dirigete a una zona (radio entorno a un punto)

#### Defender posición
 - Si posición actual > radioAmpliado
   - Dirigete a zona radioReducido (Esto evita que las unidades en el borde esten atacando/retirandose continuamente)
 - Si posición actual < radioAmpliada
   - Si unidad enemiga muy cercana o es atacada
     - Atacar unidad
   - Si no, ¿dar vueltas para no estar parado?

#### Ataque puntual (ataca enemigo hasta que huye)

#### Ataque hostil (ataca enemigo y si huye (mas de X distancia?) los sigue)

#### Patrulla por itinerario
 - Seguir ruta
 - While enemigo esta cerca de siguiente punto y fuerza_unidad_y_aliados * actitud_riesgo > fuerza_enemigp
   - Defender zona
 - While enemigo esta cerca de siguiente punto y fuerza_unidad_y_aliados * actitud_riesgo < fuerza_enemigo
   - Reforzar base

#### Unirse a grupo (Persigue a una unidad aliada hasta que esta suficientemente cerca para unirse al grupo)



#### Atacar zona





### TODO
 - Eliminar ComplexTask
 - Crear algún efecto visual para ver que una unidad ataca a otra
 - Poder pasar a pathfinding factor de miedo
