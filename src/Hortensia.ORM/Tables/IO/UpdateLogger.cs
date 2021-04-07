using System;

namespace Hortensia.ORM.Tables.IO
{
    public class UpdateLogger
    {
        private object m_lastValue;

        public int PositionX
        {
            get;
            set;
        }

        public int PositionY
        {
            get;
            set;
        }

        public UpdateLogger()
        {
            this.PositionX = Console.CursorLeft;
            this.PositionY = Console.CursorTop;
        }

        public UpdateLogger(int positionX, int positionY)
        {
            this.PositionX = positionX;
            this.PositionY = positionY;
        }

        public void End()
        {
            string empty = string.Empty;
            for (int i = 0; i < Console.BufferWidth - this.PositionX; i++)
            {
                empty = string.Concat(empty, " ");
            }
            int cursorLeft = Console.CursorLeft;
            int cursorTop = Console.CursorTop;
            Console.SetCursorPosition(this.PositionX, this.PositionY);
            Console.Write(empty);
            Console.SetCursorPosition(cursorLeft, cursorTop);
        }

        public void Update(int value)
        {
            if (!value.Equals(this.m_lastValue))
            {
                this.m_lastValue = value;
                int cursorLeft = Console.CursorLeft;
                int cursorTop = Console.CursorTop;
                Console.SetCursorPosition(this.PositionX, this.PositionY);
                Console.Write(string.Concat(value, "%"));
                Console.SetCursorPosition(cursorLeft, cursorTop);
            }
        }

        public void Update(string value)
        {
            if (!value.Equals(this.m_lastValue))
            {
                int cursorLeft = Console.CursorLeft;
                int cursorTop = Console.CursorTop;
                Console.SetCursorPosition(this.PositionX, this.PositionY);
                Console.Write(value);
                Console.SetCursorPosition(cursorLeft, cursorTop);
            }
        }
    }
}
