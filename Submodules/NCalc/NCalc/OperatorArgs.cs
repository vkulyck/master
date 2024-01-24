using System;
using NCalc.Domain;

namespace NCalc
{
    public class OperatorArgs : EventArgs
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

        public Expression LeftOperand { get; set; }
        public Expression RightOperand { get; set; }
    }
}
