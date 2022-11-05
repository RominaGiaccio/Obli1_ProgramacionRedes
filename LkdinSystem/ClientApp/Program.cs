using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Configuration;
using System.Security.Policy;
using System.Xml.Linq;
using Utils;
using Protocol;
using Domain;
using Protocol.Commands;
using Protocol;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace ClientApp
{
    public class Program
    {

        static string ipServer = SettingsManager.ReadSettings(ServerConfig.serverIPConfigKey);
        static string ipClient = SettingsManager.ReadSettings(ServerConfig.clientIPconfigkey);
        static int serverPort = int.Parse(SettingsManager.ReadSettings(ServerConfig.serverPortConfigKey));
        static int clientPort = int.Parse(SettingsManager.ReadSettings(ServerConfig.clientPortconfigkey));
        static string emaiLogged = string.Empty;
        static User userLogged = null;

        static bool disconnected = true;
        static bool exit = false;

        static NetworkStream networkStream;
        static tcpHelper tch;

        public static async Task Main(string[] args)
        {
            Console.WriteLine("Iniciando Aplicacion Cliente....!!!");

            var localEndPoint = new IPEndPoint(IPAddress.Parse(ipClient), clientPort);
            var remoteEndPoint = new IPEndPoint(IPAddress.Parse(ipServer), serverPort);
            Console.WriteLine("Voy a conectar ...");
            var tcpClient = new TcpClient(localEndPoint);

            bool cont = false;
            bool login = false;

            try
            {
                while (!exit)
                {
                    while (!cont)
                    {
                        printStartMenu(disconnected);
                        string res = messageLoop("Para seleccionar una opción ingresa el número correspondiente sin espacios.", "El número no puede ser vacio.");
                        switch (res)
                        {
                            /*case "0": //CONFIGURACION                        
                                clientConfiguration(sh);
                                break;*/
                            case "1"://CONECTAR A SERVIDOR
                                if (disconnected)
                                {   // Crear la conexion.
                                    printPrincipal();
                                    await tcpClient.ConnectAsync(remoteEndPoint).ConfigureAwait(false);
                                    networkStream = tcpClient.GetStream();
                                    tch = new tcpHelper(networkStream);
                                    Console.WriteLine("Cliente Conectado al Servidor...!!!");
                                    disconnected = false;
                                    cont = true;
                                }
                                else
                                {
                                    // Cerrar la conexion.
                                    printPrincipal();
                                    disconnected = true;
                                    Console.WriteLine("Se cerró el Cliente");
                                    networkStream.Close();
                                    disconnected = true;
                                    exit = true;
                                    cont = true;
                                }
                                break;
                            case "2": //SALIR DEL CLIENTE                        
                                exit = true;
                                cont = true;
                                break;
                            default:
                                Console.WriteLine("Para seleccionar una opción ingresa el número correspondiente sin espacios.");
                                Console.ReadLine();
                                break;
                        }
                    }

                    while (cont && !exit)
                    {
                        string select;
                        if (login)
                        {
                            printUserMenu();
                            select = messageLoop("Para seleccionar una opción ingresa el número correspondiente sin espacios.", "El número no puede ser vacio.");
                            switch (select)
                            {
                                case "1"://DAR DE ALTA PERFIL
                                    await createProfileAsync(tch);
                                    break;
                                case "2"://ACTUALIZAR FOTO DE PERFIL
                                    await updateProfilePhotoAsync(tch);
                                    break;
                                case "3"://DESCARGAR FOTO DE PERFIL
                                    await downloadProfilePhotoAsync(tch);
                                    break;
                                case "4"://CONSULTAR PERFILES
                                    await consultProfileAsync(tch);
                                    break;
                                case "5"://MENSAJES
                                    await consultMessagesAsync(tch);
                                    break;
                                case "6"://LOGOUT
                                    await logOutMethodAsync(tch);
                                    login = false;
                                    emaiLogged = string.Empty;
                                    break;
                                default:
                                    Console.WriteLine("Para seleccionar una opción ingresa el número correspondiente sin espacios.");
                                    break;
                            }
                        }
                        else
                        {
                            printClientMenu();
                            select = messageLoop("Para seleccionar una opción ingresa el número correspondiente sin espacios.", "El número no puede ser vacio.");
                            switch (select)
                            {
                                case "1":
                                    login = await userLoginAsync(login, tch);
                                    break;
                                case "2":
                                    await createUserAsync(tch);
                                    break;
                                case "3":
                                    cont = false;
                                    break;
                                default:
                                    Console.WriteLine("Para seleccionar una opción ingresa el número correspondiente sin espacios.");
                                    break;
                            }
                        }
                    }
                }

                Console.Clear();
            }
            catch(Exception e)
            {
                Console.Clear();
                Console.WriteLine("Ocurrio un error inesperado");
            }
            Console.WriteLine("FIN.");
        }

        static string messageLoop(string message, string messageError)
        {
            string solution = String.Empty;
            string messageToWrite = message;
            while (string.IsNullOrWhiteSpace(solution))
            {
                Console.WriteLine(messageToWrite);
                solution = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(solution))
                {
                    messageToWrite = messageError;
                    //messageToWrite = messageError + " " + message;
                }
            }
            return solution;
        }
        static void printClientMenu()
        {
            printPrincipal();
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.WriteLine("# MENU CLIENTE #");
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.WriteLine("1 LOGIN");
            Console.WriteLine("2 DAR DE ALTA USUARIO");
            Console.WriteLine("3 EXIT");
            Console.WriteLine("----------------------------------------------------------------------------");
        }
        static void printUserMenu()
        {
            printPrincipal();
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.WriteLine("# MENU USUARIO #");
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.WriteLine("1 DAR DE ALTA PERFIL");
            Console.WriteLine("2 ACTUALIZAR FOTO DE PERFIL");
            Console.WriteLine("3 DESCARGAR FOTO DE PERFIL");
            Console.WriteLine("4 CONSULTAR PERFILES");
            Console.WriteLine("5 MENSAJES");
            Console.WriteLine("6 LOGOUT");
            Console.WriteLine("----------------------------------------------------------------------------");
        }
        static void printSearchProfileMenu()
        {
            printPrincipal();
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.WriteLine("# BUSCAR PERFILES PUBLICOS #");
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.WriteLine("1 FILTRAR POR HABILIDAD");
            Console.WriteLine("2 FILTRAR POR PALABRA CLAVE");
            Console.WriteLine("3 ESPECIFICO POR ID");
            Console.WriteLine("4 ATRAS");
            Console.WriteLine("----------------------------------------------------------------------------");
        }

        static void printMessageMenu()
        {
            printPrincipal();
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.WriteLine("# MENSAJES #");
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.WriteLine("1 CONSULTA HISTORICA");
            Console.WriteLine("2 MENSAJES SIN LEER");
            Console.WriteLine("3 ENVIAR MENSAJE");
            Console.WriteLine("4 ATRAS");
            Console.WriteLine("----------------------------------------------------------------------------");
        }

        static void printConfigurationMenu()
        {
            printPrincipal();
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.WriteLine("# CONFIGURACION #");
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.WriteLine("1 CAMBIAR PUERTO");
            Console.WriteLine("2 CAMBIAR IP");
            Console.WriteLine("3 ATRAS");
            Console.WriteLine("----------------------------------------------------------------------------");
        }

        static void printStartMenu(bool disconnected)
        {
            printPrincipal();
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.WriteLine("# START MENU #");
            Console.WriteLine("----------------------------------------------------------------------------");
            //Console.WriteLine("1 CONFIGURACION ");
            if (disconnected)
            {
                Console.WriteLine("1 CONECTAR A SERVIDOR");
            }
            else
            {
                Console.WriteLine("1 DESCONECTAR DEL SERVIDOR");
            }
            Console.WriteLine("2 SALIR DEL CLIENTE ");
            Console.WriteLine("----------------------------------------------------------------------------");
        }

        static void printBasicMenu(string name)
        {
            printPrincipal();
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.WriteLine("# " + name + " #");
            Console.WriteLine("----------------------------------------------------------------------------");
        }

        static void actionFinished()
        {
            string exit = string.Empty;
            while (!exit.Equals("X", StringComparison.InvariantCultureIgnoreCase))
            {
                exit = messageLoop("Ingresar X para continuar. ", "Debe ingresar X para continuar.");
            }
        }

        static void clientConfiguration(tcpHelper tch)
        {
            bool cont = true;
            while (cont)
            {
                printConfigurationMenu();
                string res = messageLoop("Para seleccionar una opción ingresa el número correspondiente sin espacios.", "Debe elegir una opción ingresando el número sin espacios.");
                switch (res)
                {
                    case "1": //CAMBIAR PUERTO
                        setPort(tch);
                        break;
                    case "2": //CAMBIAR IP
                        setIp(tch);
                        break;
                    case "3": //ATRAS
                        cont = false;
                        break;
                        Console.WriteLine("Debe elegir una opción ingresando el número sin espacios.");
                        break;
                }
            }
        }

        static void setPort(tcpHelper tch)
        {
            printBasicMenu("CAMBIAR PUERTO");
            bool number = false;
            while (!number)
            {
                string res = messageLoop("Ingresar número de puerto:", "Debe ingresar un número de puerto:");
                try
                {
                    clientPort = Int32.Parse(res);
                    setPortMethod(res, tch);
                    number = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: Debe ingresar un número.");
                }
            }
            printBasicMenu("El puerto fue actualizado: " + clientPort.ToString());
            actionFinished();
        }
        static void setIp(tcpHelper tch)
        {
            printBasicMenu("CAMBIAR IP");
            string res = messageLoop("Ingresar una IP.", "Debe ingresar una nueva IP:");
            ipClient = res;
            setIPMethod(res, tch);
            printBasicMenu("La ip fue actualizada: " + ipClient);
            actionFinished();
        }
        static async Task createProfileAsync(tcpHelper tch)
        {
            printBasicMenu("DAR DE ALTA PERFIL");
            //Revisar si ya existe perfil
            bool selectProf = false;
            string res = "Y";
            printBasicMenu("CREAR PERFIL");
            string profileDescription = messageLoop("Ingresar descripcion de usuario:", "La descripcion no puede ser vacia.");
            List<string> skills = new List<string>();
            string profileSkill;
            while (res.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
            {
                profileSkill = messageLoop("Ingresar habilidad al perfil:", "La habilidad no puede ser vacia.");
                skills.Add(profileSkill);
                res = messageLoop("Ingresar Y para agregar mas habilidades o N para continuar:", "Debe ingresar Y para agregar mas habilidades o N para continuar.");

            }
            if (await createProfileMethodAsync(profileDescription, skills, tch))
            {
                printBasicMenu("Perfil creado correctamente.");
            }
            else
            {
                Console.WriteLine("ERROR: Perfil no pudo ser creado.");
            }
            actionFinished();
        }

        static async Task consultProfileAsync(tcpHelper tch)
        {
            bool filterProfile = true;
            string res;
            while (filterProfile)
            {
                printSearchProfileMenu();
                res = messageLoop("Para seleccionar una opcion ingresa el numero correspondiente sin espacios.", "Debe elegir una opcion inresando el numero de la opcion sin espacios.");
                switch (res)
                {
                    case "1": //FILTRAR POR HABILIDAD
                        await filterByAbilityAsync(tch);
                        break;
                    case "2": //FILTRAR POR PALABRA CLAVE
                        await filterByKeywordAsync(tch);
                        break;
                    case "3": //ESPECIFICO POR ID
                        await filterByIDAsync(tch);
                        break;
                    case "4": //ATRAS
                        filterProfile = false;
                        break;
                    default:
                        Console.WriteLine("Debe elegir una opcion inresando 1, 2, 3 .... sin espacios.");
                        break;
                }
            }
        }

        static async Task updateProfilePhotoAsync(tcpHelper tch)
        {
            printBasicMenu("AGREAR FOTO DE PERFIL");
            string res = string.Empty;
            res = messageLoop("Ingresar foto de perfil.", "Debe ingresar una foto de perfil.");
            if (await updateProfilePhotoMethodAsync(res, tch))
            {
                printBasicMenu("Foto guardada.");
            }
            else
            {
                Console.WriteLine("No es posible guardar la foto ingresada.");
            }
            actionFinished();
        }

        static async Task downloadProfilePhotoAsync(tcpHelper tch)
        {
            printBasicMenu("DESCARGAR FOTO DE PERFIL");
            string res = string.Empty;
            res = messageLoop("Ingresar el id del perfil que posee la foto a descargar.", "Debe ingresar el id del perfil para descargar la foto.");
            if (await downloadProfileImageMethodAsync(res, tch))
            {
                printBasicMenu("Foto descargada.");
            }
            else
            {
                Console.WriteLine("No es posible descargar la foto con el id ingresado.");
            }
            actionFinished();
        }

        static async Task consultMessagesAsync(tcpHelper tch)
        {
            printBasicMenu("CONSULTA MENSAJES");
            bool contMessage = true;
            string res;
            while (contMessage)
            {
                printMessageMenu();
                res = messageLoop("Para seleccionar una opcion ingresa el numero correspondiente sin espacios.", "Debe elegir una opcion inresando 1, 2, 3 .... sin espacios.");
                switch (res)
                {
                    case "1": //CONSULTA HISTORICA
                        await readHistoryAsync(tch);
                        break;
                    case "2": //MENSAJES SIN LEER
                        await viewUnreadMessagesAsync(tch);
                        break;
                    case "3": //ENVIAR MENSAJE
                        await sendMessageAsync(tch);
                        break;
                    case "4": //ATRAS
                        contMessage = false;
                        break;
                    default:
                        Console.WriteLine("Debe elegir una opcion inresando 1, 2, 3 .... sin espacios.");
                        break;
                }
            }
        }
        static async Task<bool> userLoginAsync(bool login, tcpHelper sh)
        {
            bool loginRes = login;
            string res = string.Empty;
            string email = string.Empty;
            while (!loginRes && !res.Equals("X", StringComparison.InvariantCultureIgnoreCase))
            {
                printBasicMenu("LOGIN");
                email = messageLoop("Ingresar email de usuario:", "El email no puede ser vacio.");
                if (await loginUserMethodAsync(email, sh))
                {
                    printBasicMenu("Login exitoso.");
                    loginRes = true;
                    emaiLogged = email;
                }
                else
                {
                    Console.WriteLine("ERROR: Error al logear.");
                    loginRes = false;
                    res = messageLoop("Ingresar X para cancelar, Y para reintentar:", "Ingresar X para cancelar, Y para reintentar:");
                }
            }
            return loginRes;
        }

        static async Task createUserAsync(tcpHelper sh)
        {
            bool createUser = true;

            printBasicMenu("CREAR USUARIO");
            string userName = messageLoop("Ingresar nombre de usuario:", "El nombre no puede ser vacio.");
            string userSurname = messageLoop("Ingresar apellido:", "El apellido no puede ser vacio.");
            string userEmail = messageLoop("Ingresar email:", "El email no puede ser vacio:");
            string res = messageLoop("Ingresar A para aceptar o C para cancelar.", "Debe elegir una opcion ingresando A o C sin espacios.");
            while (createUser)
            {
                switch (res)
                {
                    case "A":
                        //Crear usuario y volver a menu cliente
                        if (await createUserMethodAsync(userName, userSurname, userEmail, sh))
                        {
                            printBasicMenu("Usuario se creo exitosamente.");
                            actionFinished();
                        }
                        else
                        {
                            printBasicMenu("Usuario no pudo crearce.");
                            actionFinished();
                        };
                        createUser = false;
                        break;
                    case "C":
                        //No crear usuario y volver a menu cliente
                        createUser = false;
                        break;
                    default:
                        res = messageLoop("", "Debe elegir una opcion ingresando A o C sin espacios.");
                        break;
                }
                Console.WriteLine("se obtuvo respuesta");
            }
        }

        static async Task filterByAbilityAsync(tcpHelper sh)
        {
            printBasicMenu("FILTRO POR HABILIDAD");
            string res = string.Empty;
            res = messageLoop("Ingrese la habilidad por la cual desea filtrar: ", "Debe ingresar la habilidad por la cual desea filtrar: ");
            printBasicMenu("RESULTADO FILTRO POR HABILIDAD");
            await requestfilterAbilityMethodAsync(res, sh);
            actionFinished();
        }

        static async Task filterByKeywordAsync(tcpHelper sh)
        {
            printBasicMenu("FILTRO POR PALABRA CLAVE");
            string res = string.Empty;
            res = messageLoop("Ingrese la palabra clave por la cual desea filtrar: ", "Debe ingresar la palabra clave por la cual desea filtrar: ");
            printBasicMenu("RESULTADO FILTRO POR PALABRA CLAVE");
            await requestfilterKeywordMethodAsync(res, sh);
            actionFinished();
        }

        static async Task filterByIDAsync(tcpHelper sh)
        {
            printBasicMenu("FILTRO POR ID");
            string res = string.Empty;
            res = messageLoop("Ingrese el ID por la cual desea filtrar: ", "Debe ingresar el ID por la cual desea filtrar: ");
            printBasicMenu("RESULTADO FILTRO POR ID");
            await requestfilterIdMethodAsync(res, sh);
            actionFinished();
        }


        static void printPrincipal()
        {
            Console.Clear();
            if (emaiLogged.Equals(string.Empty))
            {
                Console.WriteLine("Puerto: " + clientPort + " IP: " + ipClient + " Conectado: " + (!disconnected).ToString());
            }
            else
            {
                Console.WriteLine("Puerto: " + clientPort + " IP: " + ipClient + " Conectado: " + (!disconnected).ToString() + " Usuario: " + emaiLogged);
            }
        }

        static async Task readHistoryAsync(tcpHelper sh)
        {
            printBasicMenu("CONSULTA HISTORICA");
            await historicalQueryMethodAsync(emaiLogged, sh);
            actionFinished();
        }

        static async Task viewUnreadMessagesAsync(tcpHelper sh)
        {
            printBasicMenu("MENSAJES SIN LEER");
            await unreadMessagesQueryMethodAsync(emaiLogged, sh);
            actionFinished();
        }

        static async Task sendMessageAsync(tcpHelper sh)
        {
            printBasicMenu("ENVIAR MENSAJE");
            string res;
            string userEmail = messageLoop("Ingresa email del usuario al que desea enviar el mensaje:", "Debe ingresar email de usuario receptor:");
            string messageText = messageLoop("Ingresar texto del mensaje a enviar:", "Debe ingresar el texo del mensaje:");
            Console.WriteLine("El mensjae que desea enviar es: " + messageText);
            res = messageLoop("Ingresa A para enviar el mensaje o X para cancelar:", "Ingresa A para enviar el mensaje o X para cancelar:");
            if (res.Equals("A", StringComparison.InvariantCultureIgnoreCase))
            {
                if (await sendMessageMethodAsync(emaiLogged, userEmail, messageText, sh))
                {
                    printBasicMenu("Mensaje enviado.");
                }
                else
                {
                    Console.WriteLine("Error al enviar el mensaje.");
                }
            }
            else
            {
                Console.WriteLine("Mensaje NO enviado.");
            }
            actionFinished();
        }

        //Methods

        static void setPortMethod(string newPort, tcpHelper tch)
        {

        }

        static void setIPMethod(string newIP, tcpHelper tch)
        {

        }

        static async Task<bool> sendMessageMethodAsync(string transmitterEmail, string receiverEmail, string messageText, tcpHelper tch)
        {
            var msg = new Message(transmitterEmail, receiverEmail, messageText, "" + Message.Status.NotReaded);
            return await ClientCommands.SendMessageAsync(msg, tch);
        }

        static async Task<bool> loginUserMethodAsync(string userEmail, tcpHelper tch)
        {
            User usu = new User() { Email = userEmail };
            userLogged = await ClientCommands.SignInAsync(usu, tch);
            return userLogged != null;
        }

        static async Task<bool> logOutMethodAsync(tcpHelper tch)
        {
            return await ClientCommands.SignOutAsync(userLogged, tch);
        }

        static async Task historicalQueryMethodAsync(string emaiLogged, tcpHelper tch)
        {
            User usu = new User() { Email = emaiLogged };
            await ClientCommands.GetMessagesHistoryAsync(usu, tch);
        }

        static async Task unreadMessagesQueryMethodAsync(string emaiLogged, tcpHelper tch)
        {
            User usu = new User() { Email = emaiLogged };
            await ClientCommands.GetUnreadedMessagesAsync(usu, tch);
        }

        static async Task<bool> createUserMethodAsync(string userName, string userSurname, string userEmail, tcpHelper tch)
        {
            User usu = new User(userName, userEmail, "" + User.Status.NotLogged);
            return await ClientCommands.CreateNewUserAsync(usu, tch);
        }

        static async Task<bool> createProfileMethodAsync(string profileDescription, List<string> skills, tcpHelper tch)
        {
            string[] a = skills.ToArray();
            UserProfile up = new UserProfile(userLogged.Id, profileDescription, a, "");
            return await ClientCommands.CreateUserProfileAsync(up, tch);
        }

        static async Task<bool> updateProfilePhotoMethodAsync(string photo, tcpHelper tch)
        {
            UserProfile up = new UserProfile() { UserId = userLogged.Id, Image = photo };
            return await ClientCommands.UploadUserProfileImageAsync(up, photo, tch);
        }

        //Retorna si se obtuvo resultado, en caso positivo lo imprime en consola
        static async Task<bool> requestfilterAbilityMethodAsync(string skill, tcpHelper tch)
        {
            string[] abilities = new string[] { skill };
            return await ClientCommands.GetAllProfilesAsync("", "", abilities, tch);
        }

        static async Task<bool> requestfilterKeywordMethodAsync(string word, tcpHelper tch)
        {
            string[] keywords = new string[] { word };
            return await ClientCommands.GetAllProfilesAsync("", word, keywords, tch);
        }

        static async Task<bool> requestfilterIdMethodAsync(string id, tcpHelper tch)
        {
            return await ClientCommands.GetAllProfilesAsync(id, "", Array.Empty<string>(), tch);
        }

        static async Task<bool> downloadProfileImageMethodAsync(string userId, tcpHelper tch)
        {
            UserProfile up = new UserProfile() { UserId = userId };
            return await ClientCommands.DownloadUserProfileImageAsync(up, tch);
        }
    }
}
