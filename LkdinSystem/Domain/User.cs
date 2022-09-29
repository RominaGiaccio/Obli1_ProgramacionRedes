using Utils;
using Enums;
using System.Collections.Generic;

namespace Domain
{
    public class User
    {
        public enum Status
        {
            Logged = 'L',
            NotLogged = 'N'
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CurrentState { get; set; }

        public User() {
            Id = String.Empty;
            Name = String.Empty;
            Email = String.Empty;
            CurrentState = "" + Status.NotLogged;
        }

        public User(string name, string email, string status)
        {
            this.Id = RandomData.GenerateRandomID();
            this.Name = name;
            this.Email = email;
            this.CurrentState = status;
        }

        override public string ToString()
        {
            return Id + SpecialChars.Separator + Name + SpecialChars.Separator + Email + SpecialChars.Separator + CurrentState;
        }

        public static User ToEntity(string str)
        {
            string[] splited = str.Split(SpecialChars.Separator);

            return new User()
            {
                Id = splited[0],
                Name = splited[1],
                Email = splited[2],
                CurrentState = splited[3]
            };
        }
    }
}