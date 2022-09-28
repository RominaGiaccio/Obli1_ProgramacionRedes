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
using System.Diagnostics.CodeAnalysis;

namespace ClientApp
{
    public class Program
    {

        static string ipServer = SettingsManager.ReadSettings(ServerConfig.serverIPConfigKey);
        static string ipClient = SettingsManager.ReadSettings(ServerConfig.clientIPconfigkey);
        static int serverPort = int.Parse(SettingsManager.ReadSettings(ServerConfig.serverPortConfigKey));
        static int clientPort = 0;
        static string emaiLogged = string.Empty;
        static User userLogged = null;

        static bool disconnected = true;
        static bool exit = false;

        public static void Main(string[] args)
        {
            Console.WriteLine("Iniciando Aplicacion Cliente....!!!");

            var socketCliente = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //string ipServer = settingsMngr.ReadSettings(ClientConfig.serverIPconfigkey);
            //string ipClient = settingsMngr.ReadSettings(ClientConfig.clientIPconfigkey);
            //int serverPort = int.Parse(settingsMngr.ReadSettings(ClientConfig.serverPortconfigkey));

            var localEndPoint = new IPEndPoint(IPAddress.Parse(ipClient), clientPort);
            socketCliente.Bind(localEndPoint);
            var serverEndpoint = new IPEndPoint(IPAddress.Parse(ipServer), serverPort);
            var sh = new SocketHelper(socketCliente);

            bool cont = false;
            bool login = false;

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
                                socketCliente.Connect(serverEndpoint);
                                Console.WriteLine("Cliente Conectado al Servidor...!!!");
                                disconnected = false;
                                cont = true;
                            }
                            else
                            {
                                // Cerrar la conexion.
                                printPrincipal();
                                disconnected = true;
                                Console.WriteLine("Cierro el Cliente");
                                socketCliente.Shutdown(SocketShutdown.Both);
                                socketCliente.Close();
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
                                createProfile(sh);
                                break;
                            case "2"://ACTUALIZAR FOTO DE PERFIL
                                updateProfilePhoto(sh);
                                break;
                            case "3"://CONSULTAR PERFILES
                                consultProfile(sh);
                                break;
                            case "4"://MENSAJES
                                consultMessages(sh);
                                break;
                            case "5"://LOGOFF
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
                                login = userLogin(login, sh);
                                break;
                            case "2":
                                createUser(sh);
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
            Console.WriteLine("3 CONSULTAR PERFILES");
            Console.WriteLine("4 MENSAJES");
            Console.WriteLine("5 LOGOUT");
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
            Console.WriteLine("4 ESPECIFICO POR NOMBRE");
            Console.WriteLine("5 ATRAS");
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

        static void clientConfiguration(SocketHelper sh)
        {
            bool cont = true;
            while (cont)
            {
                printConfigurationMenu();
                string res = messageLoop("Para seleccionar una opción ingresa el número correspondiente sin espacios.", "Debe elegir una opción ingresando el número sin espacios.");
                switch (res)
                {
                    case "1": //CAMBIAR PUERTO
                        setPort(sh);
                        break;
                    case "2": //CAMBIAR IP
                        setIp(sh);
                        break;
                    case "3": //ATRAS
                        cont = false;
                        break;
                        Console.WriteLine("Debe elegir una opción ingresando el número sin espacios.");
                        break;
                }
            }
        }

        static void setPort(SocketHelper sh)
        {
            printBasicMenu("CAMBIAR PUERTO");
            bool number = false;
            while (!number)
            {
                string res = messageLoop("Ingresar número de puerto:", "Debe ingresar un número de puerto:");
                try
                {
                    clientPort = Int32.Parse(res);
                    setPortMethod(res, sh);
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
        static void setIp(SocketHelper sh)
        {
            printBasicMenu("CAMBIAR IP");
            string res = messageLoop("Ingresar una IP.", "Debe ingresar una nueva IP:");
            ipClient = res;
            setIPMethod(res, sh);
            printBasicMenu("La ip fue actualizada: " + ipClient);
            actionFinished();
        }
        static void createProfile(SocketHelper sh)
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
            if (createProfileMethod(profileDescription, skills, sh))
            {
                printBasicMenu("Perfil creado correctamente.");
            }
            else
            {
                Console.WriteLine("ERROR: Perfil no pudo ser creado.");
            }
            actionFinished();
        }

        static void consultProfile(SocketHelper sh)
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
                        filterByAbility(sh);
                        break;
                    case "2": //FILTRAR POR PALABRA CLAVE
                        filterByKeyword(sh);
                        break;
                    case "3": //ESPECIFICO POR ID
                        filterByID(sh);
                        break;
                    case "4": //ESPECIFICO POR NOMBRE
                        filterByUsername(sh);
                        break;
                    case "5": //ATRAS
                        filterProfile = false;
                        break;
                    default:
                        Console.WriteLine("Debe elegir una opcion inresando 1, 2, 3 .... sin espacios.");
                        break;
                }
            }
        }

        static void updateProfilePhoto(SocketHelper sh)
        {
            printBasicMenu("AGREAR FOTO DE PERFIL");
            string res = string.Empty;
            res = messageLoop("Ingresar foto de perfil.", "Debe ingresar una foto de perfil.");
            if (updateProfilePhotoMethod(res, sh))
            {
                printBasicMenu("Foto guardada.");
            }
            else
            {
                Console.WriteLine("No es posible guardar la foto ingresada.");
            }
            actionFinished();
        }
        static void consultMessages(SocketHelper sh)
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
                        readHistory(sh);
                        break;
                    case "2": //MENSAJES SIN LEER
                        viewUnreadMessages(sh);
                        break;
                    case "3": //ENVIAR MENSAJE
                        sendMessage(sh);
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
        static bool userLogin(bool login, SocketHelper sh)
        {
            bool loginRes = login;
            string res = string.Empty;
            string email = string.Empty;
            while (!loginRes && !res.Equals("X", StringComparison.InvariantCultureIgnoreCase))
            {
                printBasicMenu("LOGIN");
                email = messageLoop("Ingresar email de usuario:", "El email no puede ser vacio.");
                if (loginUserMethod(email, sh))
                {
                    printBasicMenu("Login exitoso.");
                    Console.ReadLine();
                    loginRes = true;
                    emaiLogged = email;
                }
                else
                {
                    Console.WriteLine("ERROR: El usario es incorrecto o no existe, intente nuevamente.");
                    loginRes = false;
                }
                res = messageLoop("Ingresar X para cancelar, Y para reintentar:", "Ingresar X para cancelar, Y para reintentar:");
            }
            return loginRes;
        }

        static void createUser(SocketHelper sh)
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
                        if (createUserMethod(userName, userSurname, userEmail, sh))
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
            }
        }

        static void filterByAbility(SocketHelper sh)
        {
            printBasicMenu("FILTRO POR HABILIDAD");
            string res = string.Empty;
            res = messageLoop("Ingrese la habilidad por la cual desea filtrar: ", "Debe ingresar la habilidad por la cual desea filtrar: ");
            printBasicMenu("RESULTADO FILTRO POR HABILIDAD");
            requestfilterAbilityMethod(res, sh);
            actionFinished();
        }

        static void filterByKeyword(SocketHelper sh)
        {
            printBasicMenu("FILTRO POR PALABRA CLAVE");
            string res = string.Empty;
            res = messageLoop("Ingrese la palabra clave por la cual desea filtrar: ", "Debe ingresar la palabra clave por la cual desea filtrar: ");
            printBasicMenu("RESULTADO FILTRO POR PALABRA CLAVE");
            requestfilterKeywordMethod(res, sh);
            actionFinished();
        }

        static void filterByID(SocketHelper sh)
        {
            printBasicMenu("FILTRO POR ID");
            string res = string.Empty;
            res = messageLoop("Ingrese el ID por la cual desea filtrar: ", "Debe ingresar el ID por la cual desea filtrar: ");
            printBasicMenu("RESULTADO FILTRO POR ID");
            requestfilterIdMethod(res, sh);
            actionFinished();
        }

        static void filterByUsername(SocketHelper sh)
        {
            printBasicMenu("FILTRO POR NOMBRE");
            string res = string.Empty;
            res = messageLoop("Ingrese el nombre por el cual desea filtrar: ", "Debe ingresar el nombre por la cual desea filtrar: ");
            printBasicMenu("RESULTADO FILTRO POR NOMBRE");
            requestfilterNameMethod(res, sh);
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

        static void readHistory(SocketHelper sh)
        {
            printBasicMenu("CONSULTA HISTORICA");
            historicalQueryMethod(emaiLogged, sh);
            actionFinished();
        }

        static void viewUnreadMessages(SocketHelper sh)
        {
            printBasicMenu("MENSAJES SIN LEER");
            unreadMessagesQueryMethod(emaiLogged, sh);
            actionFinished();
        }

        static void sendMessage(SocketHelper sh)
        {
            printBasicMenu("ENVIAR MENSAJE");
            string res;
            string userEmail = messageLoop("Ingresa email del usuario al que desea enviar el mensaje:", "Debe ingresar email de usuario receptor:");
            string messageText = messageLoop("Ingresar texto del mensaje a enviar:", "Debe ingresar el texo del mensaje:");
            Console.WriteLine("El mensjae que desea enviar es: " + messageText);
            res = messageLoop("Ingresa A para enviar el mensaje o X para cancelar:", "Ingresa A para enviar el mensaje o X para cancelar:");
            if (res.Equals("A", StringComparison.InvariantCultureIgnoreCase))
            {
                if (sendMessageMethod(emaiLogged, userEmail, messageText, sh))
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

        static void setPortMethod(string newPort, SocketHelper sh)
        {

        }

        static void setIPMethod(string newIP, SocketHelper sh)
        {

        }

        static bool sendMessageMethod(string transmitterEmail, string receiverEmail, string messageText, SocketHelper sh)
        {
            var msg = new Message(transmitterEmail, receiverEmail, "Some message content", "" + Message.Status.NotReaded);
            return ClientCommands.SendMessage(msg, sh);
        }

        static bool loginUserMethod(string userEmail, SocketHelper sh)
        {
           // Console.WriteLine("email " + userEmail);
            User usu = new User() { Email = userEmail };
           // Console.WriteLine("email " + usu.Email);
            userLogged = ClientCommands.SignIn(usu, sh);
            
            return userLogged != null;
        }

        static void historicalQueryMethod(string emaiLogged, SocketHelper sh)
        {
            User usu = new User() { Email = emaiLogged };
            ClientCommands.GetMessagesHistory(usu, sh);
        }

        static void unreadMessagesQueryMethod(string emaiLogged, SocketHelper sh)
        {
            User usu = new User() { Email = emaiLogged };
            ClientCommands.GetUnreadedMessages(usu, sh);
        }

        static bool createUserMethod(string userName, string userSurname, string userEmail, SocketHelper sh)
        {
            User usu = new User(userName, userEmail, "" + User.Status.NotLogged);
            return ClientCommands.CreateNewUser(usu, sh);
        }

        static bool createProfileMethod(string profileDescription, List<string> skills, SocketHelper sh)
        {
            string[] a = skills.ToArray();
            UserProfile up = new UserProfile(userLogged.Id, profileDescription, a, "");
            return ClientCommands.CreateUserProfile(up, sh);
        }

        static bool updateProfilePhotoMethod(string photo, SocketHelper sh)
        {
            UserProfile up = new UserProfile() { UserId = userLogged.Id };
            return ClientCommands.UploadUserProfileImage(up, photo, sh);
        }

        //Retorna si se obtuvo resultado, en caso positivo lo imprime en consola
        static bool requestfilterAbilityMethod(string skill, SocketHelper sh)
        {
            string[] abilities = new string[] { skill };
            return ClientCommands.GetAllProfiles("", "", abilities, sh);
        }

        static bool requestfilterKeywordMethod(string word, SocketHelper sh)
        {
            string[] keywords = new string[] { word };
            return ClientCommands.GetAllProfiles("", "", keywords, sh);
        }

        static bool requestfilterNameMethod(string name, SocketHelper sh)
        {
            //Buscar perfil por nombre de usuario no implementado
            return false;
        }

        static bool requestfilterIdMethod(string id, SocketHelper sh)
        {
            string[] ids = new string[] { id };
            return ClientCommands.GetAllProfiles(id, "", Array.Empty<string>(), sh);
        }

        static bool downloadProfileImageMethod(string imagePath, SocketHelper sh)
        {
            UserProfile up = new UserProfile() { Image = imagePath };
            ClientCommands.DownloadUserProfileImage(up, sh);
            return true;
        }
    }
}
