﻿using System;

namespace NCalc
{
    public class FunctionArgs : EventArgs
    {

        private object _result;
        public object Result
        {
            get { return _result; }
            set 
            { 
                _result = value;
                HasResult = true;
            }
        }

        public bool HasResult { get; set; }

        private Expression[] _parameters = new Expression[0];

        public Expression[] Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }
    }
}
