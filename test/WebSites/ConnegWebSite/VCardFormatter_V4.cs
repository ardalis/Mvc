﻿using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ConnegWebsite.Models;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.HeaderValueAbstractions;

namespace ConnegWebsite
{
    /// <summary>
    /// Provides contact information of a person through VCard format.
    /// In version 4.0 of VCard format, Gender is a supported property.
    /// </summary>
    public class VCardFormatter_V4 : OutputFormatter
    {
        public VCardFormatter_V4()
        {
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/vcard;version=v4.0"));
        }

        protected override bool CanWriteType(Type declaredType, Type runtimeType)
        {
            return typeof(Contact).GetTypeInfo().IsAssignableFrom(runtimeType.GetTypeInfo());
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterContext context)
        {
            Contact contact = (Contact)context.Object;

            var builder = new StringBuilder();
            builder.AppendLine("BEGIN:VCARD");
            builder.AppendFormat("FN:{0}", contact.Name);
            builder.AppendLine();
            builder.AppendFormat("GENDER:{0}", (contact.Gender == GenderType.Male) ? "M" : "F");
            builder.AppendLine();
            builder.AppendLine("END:VCARD");

            var writer = new StreamWriter(context.ActionContext.HttpContext.Response.Body);
            await writer.WriteAsync(builder.ToString());
            await writer.FlushAsync();
        }
    }
}