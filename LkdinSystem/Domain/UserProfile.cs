using Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class UserProfile
    {
        public string UserId { get; set; }
        public string Description { get; set; }
        public string[] Abilities { get; set; }
        public string Image { get; set; }

        public UserProfile()
        {
            UserId = String.Empty;
            Description = String.Empty;
            Abilities = Array.Empty<string>();
            Image = String.Empty;
        }

        public UserProfile(string userId, string description, string[] abilities, string image)
        {
            UserId = userId;
            Description = description;
            Abilities = abilities;
            Image = image;
        }

        override public string ToString()
        {
            string abilities = string.Join(SpecialChars.ArrayDivider, Abilities);
            return UserId + SpecialChars.Separator + Description + SpecialChars.Separator + abilities + SpecialChars.Separator + Image;
        }

        public static UserProfile ToEntity(string str)
        {
            string[] splited = str.Split(SpecialChars.Separator);
            string[] abilities = splited[2].Split(SpecialChars.ArrayDivider);

            return new UserProfile()
            {
                UserId = splited[0],
                Description = splited[1],
                Abilities = abilities,
                Image = splited[3]?.Split("\r")[0] ?? ""
            };
        }

        public static string ToProfileString(UserProfile up)
        {
            string abilitiesString = " ";
            foreach (string ab in up.Abilities){
                abilitiesString += ab + " ";
            }
            string response = " (Id:) " + up.UserId + "(Descripcion:) " + up.Description + " (Habilidades:) " + abilitiesString +
                " (Foto perfil:) " + up.Image;
            if (up.Image.Equals("")) {
                response += "No tiene foto de perfil.";
            }
            return response;
        }
    }
}
