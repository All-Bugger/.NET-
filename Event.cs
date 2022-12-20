using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace termFinal
{
    internal class Event
    {
        string Title;
        string Type;
        DateTime Time;
        int State;
        public Event(string title,string type,DateTime time,int state)
        {
            this.Title = title;
            this.Type = type;
            this.Time = time;
            this.State = state;
        }

        public DateTime getTime()
        {
            return this.Time;
        }

        public string getTitle()
        {
            return this.Title;
        }

        public void setState(int newState)
        {
            this.State = newState;
        }

    }
}
