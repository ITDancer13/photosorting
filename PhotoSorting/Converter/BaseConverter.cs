﻿using System;
using System.Windows.Markup;

namespace PhotoSorting.Converter
{
    public abstract class BaseConverter : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}