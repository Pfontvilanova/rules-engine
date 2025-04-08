\ ***********************************************************************************
\ autor: Pere Font
\ data: 4/4/025
\ nom: sistema expert, modul bucle de 1000000 de inferencies
\ ***********************************************************************************

\ SIMULACION bucle

variable a
0 a !
: print a @ . 2 spaces ;
: cond1 a @ 0= ;
: cond2 a @ 1 = ;
: ac1 1 a ! ( print) ;
: ac2 0 a ! ( print) ;


\ sistema regles


\ Variables globales para el sistema
variable num-reglas \ Numero de reglas registradas
0 num-reglas !       \ Inicializar el numero de reglas a 0
variable prioridad
0 prioridad !

create condiciones 10 cells allot \ Array para las condiciones
condiciones 10 cells erase
create acciones 10 cells allot    \ Array para las acciones
cells acciones 10 cells erase
create mypad 32 allot  \ variable on guardar string counted de accept.


    \ Pedir la prioridad
: posicion
    cr ." Introduce la prioridad de la regla (numero entero, menor es mas prioritario): " 
    pad 10 accept >r 0. pad r> >number 2drop drop \ Leer y convertir la prioridad a numero
   \ es podria substituir per : pad 10 accept pad swap number, que es mes curt.
   prioridad !
;
    \ Pedir la condicion
  : condicion   cr ." Introduce el nombre de la palabra Forth para la condicion (condicion1, condicion2, ...): " 
     pad 15 accept pad swap mypad place  \ (addr n --- c-addr) pasa a counting string
     mypad find  \ Buscar la direccion de la palabra condicion, torna ( -- addr -1|0)
    dup 0= if ." palabra no encontrada" cr exit then \ Verificar si se encontro
    drop num-reglas @ cells condiciones + ! \ Guardar la direccion de la condicion
 ;   
    \ Pedir la accion
  : accion  cr ." Introduce el nombre de la palabra Forth para la accion (accion1, accion2, ...): " 
    pad 15 accept pad swap mypad place
    mypad find  \ Buscar la direccion de la palabra de accion
    dup 0= if ." palabra no encontrada" cr exit then \ Verificar si se encontro
    drop num-reglas @ cells acciones + ! \ Guardar la direcciÃƒÂ³n de la accion
   ; 
    \ Incrementar el numero de reglas
 : contador-up prioridad @ num-reglas ! 
    cr ." Regla registrada con exito." cr num-reglas @ .
;
\ Palabra para registrar una regla automaticamente
: registrar-regla ( -- ) posicion condicion accion contador-up ;



\ Mostrar las reglas definidas
: mostrar-reglas ( -- )
	 num-reglas @  0= if ." No hay reglas " exit then
    cr ." Reglas definidas: " cr
    num-reglas @  0 do
        cr ." Regla " i 1 + . ." :"
        cr ."  Condicion: " i cells condiciones + @ >name count type
        cr ."  Accion: " i cells acciones + @ >name count type
    loop
;
: ejecutar
 
num-reglas @  0 do
        \ Obtener las direcciones de las condiciones y acciones
        i cells condiciones + @ execute \ Evaluar condicion
        if
            i cells acciones + @ execute \ Ejecutar accion si la condicion es verdadera
        else
           cr ." La condicion de la regla " i 1+ . ." no se cumple." cr
        then
    loop
;

\ Configuracion inicial: pedir al usuario que programe las palabras condicionales y de acciones
: configurar ( -- )
    cr ." Configurando el sistema de reglas..." cr
    cr ." Programa antes las palabras condicionales (condicion1, condicion2, ...) que devuelven true o false."
    cr ." Tambien programa las palabras de accion (accion1, accion2, ...) que ejecutaran codigo especifico." cr
;



\ Menu interactivo para el sistema
: menuinici ( -- )
    begin
        cr ." Menu Principal:"
        cr ." 1. Configurar el sistema de reglas"
        cr ." 2. Registrar una nueva regla"
        cr ." 3. Mostrar reglas"
        cr ." 4. Ejecutar motor de inferencia"
        cr ." 5. Salir"
        cr ." Selecciona una opcion (1-5): "
        key dup emit
        case
            [char] 1 of configurar endof
            [char] 2 of registrar-regla endof
            [char] 3 of mostrar-reglas endof
            [char] 4 of ejecutar endof
            [char] 5 of exit endof
             cr ." Opcion invalida, intentalo de nuevo." cr
        endcase
    again
;


\ Iniciar el menu principal
menuinici

: bucle 1000000 0 do ejecutar loop ;
: measure counter ' execute timer ." ms" ;

