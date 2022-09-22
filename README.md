# ObliProramacionRedes

Equipo:
Biladoniga-231749
Giaccio-206127

APUNTES: (ES APLICACION POR CONSOLA NO REQUIERE VENTANAS)

AppServidor:
Se maneja la información relacionada a usuarios y sus perfiles laborales y archivos relacionados con los mismos.

AppCliente:
Se encarga de la interacción de los usuarios con el sistema.

La plataforma debe brindar las siguientes funcionalidades:
1. Conexión de un cliente al servidor.
2. Dar de alta a un usuario.
3. Crear Perfil de trabajo de usuario.
4. Asociar foto al perfil.
5. Consultar perfiles existentes.
6. Enviar y recibir mensajes.

MENU DE SERVIDOR: (suponiendo vista de usuario en servidor)
    1 BUSCAR USUARIOS DEL SISTEMA (DADOS DE ALTA, INCLUYE FILTROS!!!)
        1 DESCARGAR IMAGEN DE PERFIL
        2 ASOCIAR FOTO A PERFIL
        3 CREAR PERFIL (PARA TRABAJO DE USUARIOS)
    2 DAR DE ALTA A UN USUARIO
    3 BANDEJA DE MENSAJES (DE UN USUARIO A OTRO)
        1 SIN LEER
        2 LEIDOS
        3 RESPONDER 
        4 ENVIAR
    
MENU DE CLIENTE:
    1 IDENTIFICAR USUARIO?
    2 CONEXION A SERVIDOR
        1 CONECTAR A SERVIDOR 
        2 DESCONECTAR DE SERVIDOR
    3 DAR DE ALTA UN USUARIO 
        1 CREAR USUARIO?
    4 DAR DE ALTA PERFIL (SOLO AL PROPIO PERFIL DEL USUARIO?) 
    5 ACTUALIZAR PERFIL
        1 ACTUALIZAR FOTO
    6 CONSULTAR PERFILES
        1 FILTRAR POR HABILIDAD
        2 FILTRAR POR PALABRA CLAVE
        3 ESPECIFICO POR ID
        4 ESPECIFICO POR NOMBRE
    7 MENSAJES
        1 ENVIAR A OTRO USARIO
            1 ENVIAR
            2 CANCELAR?
        2 REVISAR MENSAJES (HISTORICO)
            1 RESPONDER
            2 VER
    8 CONFIURACION
        1 CAMBIAR PUERTO
        2 CAMBIAR IP

TENER EN CUENTA:
- Dos clientes no pueden intentar conectarse al servidor con el mismo usuario ...
- Dos clientes no pueden acceder a cambiar un perfil a la vez? ...
- Que mas?

DOMINIO: 
    USUARIO (NOMBRE, ID, EMPRESA, LISTA DE PERFILES, FECHA INGRESO, FECHA EGRESO)
    PERFIL (USUARIO, LISTA DE HABILIDADES - ENUM, DESCRIPCION, FOTO - STRING?)
    MENSAJE (USUARIO EMISOR, USUARIO DESTINATARIO, MENSAJE, ESTADO - ENUM)
    HABILIDADES (BUENO TRABAJANDO EN EQUIPO, PROACTIVO, APRENDE RAPIDO, ORGANIZADO)
    ESTADO (VISTO, NO VISTO) O (TRUE/FALSE)

REQUERIMIENTOS/FUNCIONALIDADES SERVIDOR:

    SRF1. Aceptar pedidos de conexión de un cliente. El servidor debe ser capaz de aceptar
pedidos de conexión de varios clientes a la vez. (VISTO EN CLASE)

    SRF2. Dar de Alta a un usuario. El sistema debe permitir dar de alta a un usuario en el
sistema.(VER)

    SRF3. Crear Perfil de trabajo de usuario. El sistema debe permitir crear un perfil de trabajo
para el usuario, incluyendo habilidades y una descripción. (VER)

    SRF4 Asociar una foto al perfil de trabajo. El sistema debe permitir subir una foto y
asociarla al perfil de trabajo de un usuario. (VER, esto es actualizar el archivo de BD, es solo un perfil por usuario?) 

    SRF5. Consultar perfiles existentes. El usuario deberá poder buscar perfiles existentes,
incluyendo búsquedas por habilidades y por palabras claves. También deberá ser capaz de
descargar la imagen asociada al perfil, en caso de existir la misma. (VER)

    SRF6. Enviar y recibir mensajes. El sistema debe permitir que un usuario envíe mensajes a
otro, y que el usuario receptor chequee sus mensajes sin leer, así como también revisar su
historial de mensajes.(VER, ESTADO DE MENSAJES Y ALMACENAMIENTO EN ARCHIVO?)

    SRF7. Configuración. Se deberá ser capaz de modificar los puertos e ip utilizados por el
servidor sin necesidad de recompilar el proyecto. Dichos valores no deben estar
“hardcodeados” en el código.(VER, ESTO ES TENER ARCHIVO DE CONFIGURACION?)

REQUERIMIENTOS/FUNCIONALIDADES CLIENTE:

    CRF1. Conectarse y desconectarse al servidor. Se deberá ser capaz de conectarse y
desconectarse del servidor. (VISTO EN CLASE)

    CRF2. Alta de usuario. Se debe poder dar de alta a un usuario para que el mismo pueda
acceder al sistema. (VER)

    CRF3. Alta de Perfil de trabajo. Un usuario debe ser capaz de dar de alta a su perfil de
trabajo.(VER)

    CRF4. Asociar foto a perfil de trabajo. El usuario deberá ser capaz de actualizar su perfil de
trabajo adjuntando una foto.(VER)

    CRF5. Consultar perfiles existentes. Se deberá, seleccionando la opción de búsqueda,
especificar habilidades o palabras a buscar dentro del perfil de los usuarios y devolver una
lista de perfiles que coinciden con la búsqueda.(VER)

    CRF6. Consultar un perfil específico. Se deberá permitir que el usuario pida un perfil
específico, indicando nombre o id del usuario a buscar. (VER)

    CRF7. Enviar y recibir mensajes. El sistema debe brindar la posibilidad de enviar mensajes a
otros usuarios, así como también consultar los mensajes recibidos.(VER)

    CRF8. Configuración. Se deberá ser capaz de modificar los puertos e ip utilizados por el
cliente sin necesidad de recompilar el proyecto. Dichos valores no deben estar
“hardcodeados” en el código.(VER)




------------------------------------------------------------------------------------------------------

DOCUMENTACION: 

Como documentación se espera que se incluya el alcance de la aplicación, una descripción de la
arquitectura, así como el diseño detallado de cada uno de sus componentes.
Es muy importante (tanto como la aplicación) que se entregue documentación que explique las principales
decisiones tomadas para construirla. Esas decisiones dependerán de lo que cada trabajo persiga, pero a
modo de ejemplo, sería bueno documentar por qué se tomaron las diferentes decisiones a nivel de
protocolo, cómo se manejan los errores que se pueden presentar, cuáles son los mecanismos de
concurrencia utilizados, etc.
Se debe documentar además el funcionamiento de la aplicación. No es necesario un manual de usuario,
pero si aquellas aclaraciones que consideren necesarias para poder probar y utilizar la aplicación, así como para comprender su estructura.
No se espera ningún formato particular para la entrega. El orden, las técnicas y nomenclaturas a utilizar serán decisión de los alumnos. Se evaluará la correcta selección y uso de los mismos.
Se debe aclarar que versión de .NET utilizaron.
Datos de prueba (opcional pero recomendado)
Entregar el sistema con una fuente de datos de prueba propia para facilitar la corrección.

------------------------------------------------------------------------------------------------------
ENTREGA 

Para facilitar la ejecución de la aplicación, pueden incluir carpetas con las aplicaciones compiladas en release y todo lo necesario para poder ejecutarlas (archivos de configuración, etc.)

Se debe entregar por Gestión un archivo comprimido, conteniendo lo siguiente:
- Fuentes de la aplicación, incluyendo el proyecto que permita probar y ejecutar.
- Archivo PDF con la documentación.
El tamaño máximo del archivo a subir es de 40mb Y solo se aceptará lo entregado en gestión