﻿using System;

namespace GmWeb.Logic.Data.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class EncryptedDateTimeColumnAttribute : Attribute, IEncryptedColumnAttribute
    {
    }
}