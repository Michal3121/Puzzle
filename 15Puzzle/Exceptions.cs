﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _15Puzzle
{
    public class NotSquareImageSizeException : Exception
    {
        public NotSquareImageSizeException()
        {
        }

        public NotSquareImageSizeException(string message)
            : base(message)
        {
        }

        public NotSquareImageSizeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
