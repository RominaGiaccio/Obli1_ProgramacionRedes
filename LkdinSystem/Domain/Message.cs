using Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Domain
{
    public class Message
    {
        public enum Status
        {
            Readed = 'R',
            NotReaded = 'N'
        }

        public string Id { get; set; }
        public string SenderEmail { get; set; }
        public string ReceiverEmail { get; set; }
        public string Text { get; set; }
        public string CurrentState { get; set; }

        public Message()
        {
            Id = String.Empty;
            SenderEmail = String.Empty;
            ReceiverEmail = String.Empty;
            Text = String.Empty;
            CurrentState = "" + Status.NotReaded;
        }

        public Message(string senderEmail, string receiverEmail, string text, string status)
        {
            this.Id = RandomData.GenerateRandomID();
            this.SenderEmail = senderEmail;
            this.ReceiverEmail = receiverEmail;
            this.Text = text;
            this.CurrentState = status;
        }
        
        override public string ToString()
        {
            return Id + SpecialChars.Separator + SenderEmail + SpecialChars.Separator + ReceiverEmail + SpecialChars.Separator + Text + SpecialChars.Separator + CurrentState;
        }

        public static Message ToEntity(string str)
        {
            string[] splited = str.Split(SpecialChars.Separator);

            return new Message()
            {
                Id = splited[0],
                SenderEmail = splited[1],
                ReceiverEmail = splited[2],
                Text = splited[3],
                CurrentState = splited[4]
            };
        }

        public static string ToMessageString(Message mesg)
        {
            return "(De:) " + mesg.SenderEmail + " (Para:) " + mesg.ReceiverEmail +
                " (Mensaje:) " + mesg.Text + " (Id:) " + mesg.Id;
        }
    }
}
