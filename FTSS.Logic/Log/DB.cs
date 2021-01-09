﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FTSS.Logic.Log
{
    public class DB : ILog
    {
        public DB()
        {

        }

        /// <summary>
        /// Log an Exception with custom message
        /// </summary>
        /// <param name="customMessage"></param>
        /// <param name="e"></param>
        public void Add(Exception e, string customMessage = null)
        {
            string text = string.Format("{0}\nException: {1}\nStackTrace: {2}\n",
                customMessage ?? "", e.Message, e.StackTrace);
            this.Add(text);
        }

        public void Add(string msg)
        {
            throw new NotImplementedException();
        }
    }
}